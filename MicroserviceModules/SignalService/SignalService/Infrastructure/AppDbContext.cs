using Microsoft.EntityFrameworkCore;
using SignalService.Domain.Entities;
using System.Reflection.Emit;

namespace SignalService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobRequest> JobRequests => Set<JobRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobRequest>(entity =>
            {
                entity.ToTable("JobRequests");
                entity.HasIndex(j => j.CreatedAtUtc);
                entity.HasIndex(j => j.JobType);
            });
        }
    }
}
