using System;

namespace VehiclesAPI.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string VehicleId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ConnectUpdated { get; set; }
        public int CustomerId { get; set; }
        public CustomerModel Customer { get; set; }
    }
}
