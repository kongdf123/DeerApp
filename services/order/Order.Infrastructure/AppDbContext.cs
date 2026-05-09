using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Order.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Domain.Order> Orders => Set<Domain.Order>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
