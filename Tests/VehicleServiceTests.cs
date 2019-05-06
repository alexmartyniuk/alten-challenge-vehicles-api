using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VehiclesAPI.Models;
using VehiclesAPI.Services;
using Xunit;

namespace VehiclesAPI.Tests
{
    public class VehicleServiceTests : IDisposable
    {
        private SqliteConnection _connection;
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _serviceContext;
        private VehicleService _service;

        public VehicleServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseSqlite(_connection)
               .Options;
            
            _serviceContext = new ApplicationDbContext(_options);
            _serviceContext.Database.EnsureCreated();
            _service = new VehicleService(_serviceContext);
        }

        public void Dispose()
        {
            if (_serviceContext != null)
            {
                _serviceContext.Dispose();
                _serviceContext = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        [Fact]
        public async Task SearchAsync_ForEmptyDatabase_ShouldReturnEmptyCollection()
        {
            var vehicles = await _service.SearchAsync(null, null);

            Assert.Empty(vehicles);
        }

        [Fact]
        public async Task SearchAsync_ForOneVehicle_ShouldReturnOneElement()
        {
            const int VEHICLE_ID = 123;
            const int CUSTOMER_ID = 456;
            const string VEHICLE_VEHICLEID = "Vehicle Id";
            const string VEHICLE_REGNUMBER = "Reg Number";
            const string CUSTOMER_FULLNAME = "Customer Name";
            const string CUSTOMER_ADDRESS = "Customer Address";

            using (var context = new ApplicationDbContext(_options))
            {

                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE_ID,
                    VehicleId = VEHICLE_VEHICLEID,
                    RegistrationNumber = VEHICLE_REGNUMBER,
                    Customer = new CustomerModel
                    {
                        Id = CUSTOMER_ID,
                        FullName = CUSTOMER_FULLNAME,
                        Address = CUSTOMER_ADDRESS
                    }
                });
                context.SaveChanges();
            }
            
            var vehicles = await _service.SearchAsync(null, null);                      
            var vehicle = vehicles.Single();

            Assert.Equal(VEHICLE_ID, vehicle.Id);
            Assert.Equal(VEHICLE_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER_ID, customer.Id);
            Assert.Equal(CUSTOMER_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER_ADDRESS, customer.Address);            
        }
    }
}
