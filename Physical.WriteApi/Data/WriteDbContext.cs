using Microsoft.EntityFrameworkCore;
using Physical.WriteApi.Models;

namespace Physical.WriteApi.Data;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
}
