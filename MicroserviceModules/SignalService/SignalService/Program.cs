using Grpc.Health.V1;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SignalService.Application.Jobs;
using SignalService.Application.Jobs.Middleware;
using SignalService.Application.Queues;
using SignalService.Application.Services;
using SignalService.HealthChecks;
using SignalService.Hubs;
using SignalService.Infrastructure;
using SignalService.Infrastructure.Auth;
using SignalService.Infrastructure.Health;
using SignalService.Services;
using SignalService.Tests.Functional;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("webapi", client =>
{
    client.BaseAddress = new Uri("https://localhost:44330"); // or your API URL
});

// MVC + API
builder.Services.AddControllersWithViews();

// EF Core SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Queue + background worker + SignalR + Grpc + Service Bus
builder.Services.AddSingleton<IJobQueue, JobQueue>();
builder.Services.AddHostedService<JobWorker>();
builder.Services.AddSignalR();

// Register gRPC + Health service
builder.Services.AddGrpc();
builder.Services.AddSingleton<Grpc.HealthCheck.HealthServiceImpl>();
builder.Services.AddGrpcHealthChecks();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<ServiceBusJobConsumer>();
builder.Services.AddSingleton<ServiceBusJobSender>();
// builder.Services.AddSingleton<IJobMiddleware, YourMiddleware>();
builder.Services.AddSingleton<IJobPipeline, JobPipeline>();
builder.Services.AddSingleton<IJobMiddleware, LoggingMiddleware>();
// Register Authentication API Key
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthHandler>("ApiKey", null);

builder.Services.AddAuthorization();

// Register Functional Tests in Program.cs
builder.Services.AddScoped<IWebApiTestClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var baseUrl = config["FunctionalTests:WebApi:BaseUrl"];
    var apiKey = config["FunctionalTests:WebApi:ApiKey"];

    return new WebApiTestClient(baseUrl, apiKey);
}); 
builder.Services.AddScoped<IGrpcTestClient>(sp =>
    new GrpcTestClient("https://localhost:44330", "super-secret-key-123"));
builder.Services.AddScoped<ISignalRTestClient>(sp =>
    new SignalRTestClient(
        hubUrl: "https://localhost:44330/hubs/jobhub",
        apiKey: "super-secret-key-123"
    ));
builder.Services.AddSingleton<IServiceBusTestClient, FakeServiceBusTestClient>();

// Health Check Registrations
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddCheck<WorkerHealthCheck>("job_worker")
    .AddCheck<ServiceBusHealthCheck>("service_bus")
    .AddCheck<WebApiHealthCheck>("web_api");

builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.HealthChecks", LogLevel.Warning);


var app = builder.Build();

var health = app.Services.GetRequiredService<Grpc.HealthCheck.HealthServiceImpl>();
health.SetStatus(string.Empty, HealthCheckResponse.Types.ServingStatus.Serving);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // applies migrations or creates DB if missing
    db.Database.Migrate();  // for production
    // db.Database.EnsureCreated(); // for development
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// MVC default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API controllers
app.MapControllers();
// Map the SignalR endpoint
app.MapHub<JobHub>("/hubs/jobhub");
// Map the Grpc service endpoint
app.MapGrpcService<JobGrpcService>();
app.MapGrpcHealthChecksService();
// Map Functional Test Hub
app.MapHub<FunctionalTestHub>("/functionalTestHub");
// Map Health Check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            results = report.Entries.ToDictionary(
                e => e.Key,
                e => new {
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description
                })
        });
        await context.Response.WriteAsync(json);
    }
});

app.Run();
