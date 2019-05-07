using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VehiclesAPI.Models;

namespace VehiclesAPI.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CustomerModel>> GetAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }
    }
}
