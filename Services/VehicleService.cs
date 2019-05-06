using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehiclesAPI.Models;

namespace VehiclesAPI.Services
{
    public class VehicleService
    {
        private const int SecondsConnected = -60;
        private readonly ApplicationDbContext _dbContext;
        private readonly DateTimeService _dateTimeService;

        public VehicleService(ApplicationDbContext dbContext, DateTimeService dateTimeService)
        {
            _dbContext = dbContext;
            _dateTimeService = dateTimeService;
        }

        public async Task<VehicleModel> GetAsync(int id)
        {
            return await _dbContext.Vehicles.FindAsync(id);
        }

        public async Task<List<VehicleModel>> SearchAsync(bool? connected, int? customerId)
        {
            IQueryable<VehicleModel> query = _dbContext.Vehicles;

            if (connected.HasValue)
            {
                if (connected.Value)
                {
                    query = query.Where(vh => vh.ConnectUpdated >= _dateTimeService.Now.AddSeconds(SecondsConnected));
                }
                else
                {
                    query = query.Where(vh => vh.ConnectUpdated < _dateTimeService.Now.AddSeconds(SecondsConnected));
                }
            }

            if (customerId.HasValue)
            {
                query = query.Where(vh => vh.CustomerId == customerId.Value);                
            }

            query = query.Include(vh => vh.Customer);

            return await query.ToListAsync();
        }

        public async Task ConnectedAsync(VehicleModel vehicle)
        {
            var originalVehicle = await GetAsync(vehicle.Id);

            originalVehicle.ConnectUpdated = _dateTimeService.Now;

            await _dbContext.SaveChangesAsync();
        }

        public bool IsConnected(VehicleModel vehicle)
        {
            return vehicle.ConnectUpdated >= _dateTimeService.Now.AddSeconds(SecondsConnected);
        }
    }
}
