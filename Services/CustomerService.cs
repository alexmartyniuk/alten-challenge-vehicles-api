using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehiclesAPI.Models;

namespace VehiclesAPI.Services
{
    public class CustomerService
    {
        private readonly ApplicationContext _dbContext;

        public CustomerService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CustomerModel>> GetAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }
    }
}
