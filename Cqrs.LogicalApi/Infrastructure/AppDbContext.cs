using Microsoft.EntityFrameworkCore;
using Cqrs.LogicalApi.Domain;

namespace Cqrs.LogicalApi.Infrastructure {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
