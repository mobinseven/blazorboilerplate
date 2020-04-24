using BlazorBoilerplate.Shared.DataModels;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logistique.Storage
{
    public class LogistiqueDbContext : MultiTenantDbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Order> Orders { get; set; }
        private readonly IConfiguration _configuration;

        public LogistiqueDbContext(TenantInfo tenantInfo, DbContextOptions options,IConfiguration configuration) : private ba se(tenantInfo, options)

        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ServiceCollectionExtensions.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>()
                .HasOne(l => l._LocationType)
                .WithMany()
                .HasForeignKey(l => l.LocationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Plan)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Location)
                .WithMany(l => l.Orders)
                .HasForeignKey(o => o.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>().IsMultiTenant();
            modelBuilder.Entity<LocationType>().IsMultiTenant();
            modelBuilder.Entity<Plan>().IsMultiTenant();
            modelBuilder.Entity<Order>().IsMultiTenant();
        }
    }
}