using Microsoft.EntityFrameworkCore;
using RPDemo.Domain.Entities;

namespace RPDemo.Infrastructure
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Person> Persons { get; set; }

        protected DemoDbContext() { }
    }
}
