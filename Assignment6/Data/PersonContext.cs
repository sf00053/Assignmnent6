using Assignment6.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment6.Data
{
    public class PersonContext : DbContext
    {
        public DbSet<Person> People { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=people.db");
        }
    }
}