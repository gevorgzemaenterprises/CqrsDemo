using Microsoft.EntityFrameworkCore;
using Physical.ReadApi.Models;
using System.Collections.Generic;

namespace Physical.ReadApi.Data
{
    public class ReadDbContext : DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
