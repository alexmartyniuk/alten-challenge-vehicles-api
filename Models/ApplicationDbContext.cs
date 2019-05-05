using Microsoft.EntityFrameworkCore;

namespace VehiclesAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=vehicles.db");
        }
    }
}
