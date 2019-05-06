using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
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
        private const int VEHICLE1_ID = 111;
        private const string VEHICLE1_VEHICLEID = "Vehicle Id 1";
        private const string VEHICLE1_REGNUMBER = "Reg Number 1";

        private const int VEHICLE2_ID = 222;
        private const string VEHICLE2_VEHICLEID = "Vehicle Id 2";
        private const string VEHICLE2_REGNUMBER = "Reg Number 2";

        private const int VEHICLE3_ID = 333;
        private const string VEHICLE3_VEHICLEID = "Vehicle Id 3";
        private const string VEHICLE3_REGNUMBER = "Reg Number 3";

        private const int CUSTOMER1_ID = 444;
        private const string CUSTOMER1_FULLNAME = "Customer1 Name";
        private const string CUSTOMER1_ADDRESS = "Customer1 Address";

        private const int CUSTOMER2_ID = 555;
        private const string CUSTOMER2_FULLNAME = "Customer2 Name";
        private const string CUSTOMER2_ADDRESS = "Customer2 Address";

        private SqliteConnection _connection;
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _serviceContext;
        private VehicleService _service;

        private Mock<DateTimeService> _dateTimeService = new Mock<DateTimeService>();

        public VehicleServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseSqlite(_connection)
               .Options;
            
            _serviceContext = new ApplicationDbContext(_options);
            _serviceContext.Database.EnsureCreated();
            _service = new VehicleService(_serviceContext, _dateTimeService.Object);            
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
        public async Task SearchAsync_For1Vehicle_ShouldReturn1Element()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    Customer = new CustomerModel
                    {
                        Id = CUSTOMER1_ID,
                        FullName = CUSTOMER1_FULLNAME,
                        Address = CUSTOMER1_ADDRESS
                    }
                });
                context.SaveChanges();
            }
            
            var vehicles = await _service.SearchAsync(null, null);                      
            var vehicle = vehicles.Single();

            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);            
        }

        [Fact]
        public async Task SearchAsync_For2Vehicles_ShouldReturn2Elements()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });                              
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER1_ID
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(null, null);

            Assert.Equal(2, vehicles.Count);

            var vehicle1 = vehicles.Single(vh => vh.Id == VEHICLE1_ID);            
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle1.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle1.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle1.CustomerId);

            var customer1 = vehicle1.Customer;
            Assert.Equal(CUSTOMER1_ID, customer1.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer1.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer1.Address);

            var vehicle2 = vehicles.Single(vh => vh.Id == VEHICLE2_ID);
            Assert.Equal(VEHICLE2_ID, vehicle2.Id);
            Assert.Equal(VEHICLE2_VEHICLEID, vehicle2.VehicleId);
            Assert.Equal(VEHICLE2_REGNUMBER, vehicle2.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle2.CustomerId);

            var customer2 = vehicle1.Customer;
            Assert.Equal(CUSTOMER1_ID, customer2.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer2.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer2.Address);
        }

        [Fact]
        public async Task SearchAsync_ByConnected_ShouldReturn1Element()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-61)
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(true, null);

            var vehicle = vehicles.Single();
            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);
        }

        [Fact]
        public async Task SearchAsync_ByNotConnected_ShouldReturn1Element()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-61)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(false, null);

            var vehicle = vehicles.Single();
            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);
        }

        [Fact]
        public async Task SearchAsync_ByCustomerId_ShouldReturn1Element()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER2_ID,
                    FullName = CUSTOMER2_FULLNAME,
                    Address = CUSTOMER2_ADDRESS
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER2_ID
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(null, CUSTOMER1_ID);

            var vehicle = vehicles.Single();
            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);
        }

        [Fact]
        public async Task SearchAsync_ByCustomerIdAndConnected_ShouldReturn1Element()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER2_ID,
                    FullName = CUSTOMER2_FULLNAME,
                    Address = CUSTOMER2_ADDRESS
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-61)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE3_ID,
                    VehicleId = VEHICLE3_VEHICLEID,
                    RegistrationNumber = VEHICLE3_REGNUMBER,
                    CustomerId = CUSTOMER2_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(true, CUSTOMER1_ID);

            var vehicle = vehicles.Single();
            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);
        }

        [Fact]
        public async Task SearchAsync_ByCustomerIdAndNotConnected_ShouldReturn1Element()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            using (var context = new ApplicationDbContext(_options))
            {
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                });
                context.Customers.Add(new CustomerModel
                {
                    Id = CUSTOMER2_ID,
                    FullName = CUSTOMER2_FULLNAME,
                    Address = CUSTOMER2_ADDRESS
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE1_ID,
                    VehicleId = VEHICLE1_VEHICLEID,
                    RegistrationNumber = VEHICLE1_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-61)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE2_ID,
                    VehicleId = VEHICLE2_VEHICLEID,
                    RegistrationNumber = VEHICLE2_REGNUMBER,
                    CustomerId = CUSTOMER1_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.Vehicles.Add(new VehicleModel
                {
                    Id = VEHICLE3_ID,
                    VehicleId = VEHICLE3_VEHICLEID,
                    RegistrationNumber = VEHICLE3_REGNUMBER,
                    CustomerId = CUSTOMER2_ID,
                    ConnectUpdated = now.AddSeconds(-59)
                });
                context.SaveChanges();
            }

            var vehicles = await _service.SearchAsync(false, CUSTOMER1_ID);

            var vehicle = vehicles.Single();
            Assert.Equal(VEHICLE1_ID, vehicle.Id);
            Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
            Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
            Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);

            var customer = vehicle.Customer;
            Assert.Equal(CUSTOMER1_ID, customer.Id);
            Assert.Equal(CUSTOMER1_FULLNAME, customer.FullName);
            Assert.Equal(CUSTOMER1_ADDRESS, customer.Address);
        }

        [Fact]
        public void IsConnected_ForNewConnection_ShouldReturnTrue()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            var vehicle = new VehicleModel
            {
                Id = VEHICLE1_ID,
                VehicleId = VEHICLE1_VEHICLEID,
                RegistrationNumber = VEHICLE1_REGNUMBER,
                CustomerId = CUSTOMER1_ID,
                ConnectUpdated = now.AddSeconds(-59)
            };

            var connected = _service.IsConnected(vehicle);

            Assert.True(connected);
        }

        [Fact]
        public void IsConnected_ForOldConnection_ShouldReturnFalse()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            var vehicle = new VehicleModel
            {
                Id = VEHICLE1_ID,
                VehicleId = VEHICLE1_VEHICLEID,
                RegistrationNumber = VEHICLE1_REGNUMBER,
                CustomerId = CUSTOMER1_ID,
                ConnectUpdated = now.AddSeconds(-61)
            };

            var connected = _service.IsConnected(vehicle);

            Assert.False(connected);
        }

        [Fact]
        public async Task ConnectedAsync_ForOldConnection_ShouldUpdateConnectedDate()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            var originalVehicle = new VehicleModel
            {
                Id = VEHICLE1_ID,
                VehicleId = VEHICLE1_VEHICLEID,
                RegistrationNumber = VEHICLE1_REGNUMBER,
                Customer = new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                }
            };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Vehicles.Add(originalVehicle);
                context.SaveChanges();
            }            

            await _service.ConnectedAsync(originalVehicle);

            using (var context = new ApplicationDbContext(_options))
            {
                var vehicle = context.Vehicles.Find(VEHICLE1_ID);

                Assert.Equal(VEHICLE1_ID, vehicle.Id);
                Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
                Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
                Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);
                Assert.Equal(now, vehicle.ConnectUpdated);
            }            
        }

        [Fact]
        public async Task ConnectedAsync_ForNewConnection_ShouldUpdateConnectedDate()
        {
            var now = new DateTime(2019, 5, 6, 12, 34, 56);
            _dateTimeService.Setup(m => m.Now).Returns(now);

            var originalVehicle = new VehicleModel
            {
                Id = VEHICLE1_ID,
                VehicleId = VEHICLE1_VEHICLEID,
                RegistrationNumber = VEHICLE1_REGNUMBER,
                ConnectUpdated = now.AddSeconds(1),
                Customer = new CustomerModel
                {
                    Id = CUSTOMER1_ID,
                    FullName = CUSTOMER1_FULLNAME,
                    Address = CUSTOMER1_ADDRESS
                }
            };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Vehicles.Add(originalVehicle);
                context.SaveChanges();
            }

            await _service.ConnectedAsync(originalVehicle);

            using (var context = new ApplicationDbContext(_options))
            {
                var vehicle = context.Vehicles.Find(VEHICLE1_ID);

                Assert.Equal(VEHICLE1_ID, vehicle.Id);
                Assert.Equal(VEHICLE1_VEHICLEID, vehicle.VehicleId);
                Assert.Equal(VEHICLE1_REGNUMBER, vehicle.RegistrationNumber);
                Assert.Equal(CUSTOMER1_ID, vehicle.CustomerId);
                Assert.Equal(now, vehicle.ConnectUpdated);
            }
        }
    }
}
