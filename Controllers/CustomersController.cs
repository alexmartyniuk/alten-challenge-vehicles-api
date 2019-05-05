using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using VehiclesAPI.Dtos;
using VehiclesAPI.Services;

namespace VehiclesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService vehicleService)
        {
            _customerService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerDto[]>> GetAsync()
        {
            var customers = await _customerService.GetAsync();

            var dtos = customers.Select(cs => new CustomerDto(cs));

            return Ok(dtos);
        }
    }
}