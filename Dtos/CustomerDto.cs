using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehiclesAPI.Models;

namespace VehiclesAPI.Dtos
{
    public class CustomerDto
    {
        public int Id { get; private set; }
        public string FullName { get; private set; }
        public string Address { get; private set; }

        public CustomerDto(CustomerModel model)
        {
            Id = model.Id;
            FullName = model.FullName;
            Address = model.Address;
        }
    }
}
