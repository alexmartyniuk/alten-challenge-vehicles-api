using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehiclesAPI.Models;

namespace VehiclesAPI.Dtos
{
    public class VehicleDto
    {
        public int Id { get; private set; }

        public string VehicleId { get; private set; }

        public string RegistrationNumber { get; private set; }

        public bool Connected { get; private set; }

        public CustomerDto Customer { get; private set; }

        public VehicleDto(VehicleModel model, bool connected)
        {
            Id = model.Id;
            VehicleId = model.VehicleId;
            RegistrationNumber = model.RegistrationNumber;
            Customer = new CustomerDto(model.Customer);
            Connected = connected;
        }
    }
}