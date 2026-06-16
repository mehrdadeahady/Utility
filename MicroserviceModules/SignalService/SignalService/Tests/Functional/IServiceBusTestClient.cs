namespace SignalService.Tests.Functional
{
    public interface IServiceBusTestClient
    {
        Task PublishJobAsync(string json);
        Task SubscribeAsync(Func<string, Task> handler);
    }
}
