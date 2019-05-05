using Microsoft.EntityFrameworkCore;

namespace VehiclesAPI.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=vehicles.db");
        }
    }
}
