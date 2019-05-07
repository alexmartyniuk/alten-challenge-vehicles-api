using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VehiclesAPI.Dtos;
using VehiclesAPI.Services;

namespace VehiclesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehiclesController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<VehicleDto[]>> GetAsync(bool? connected, int? customerId)
        {
            var vehicleModels = await _vehicleService.SearchAsync(connected, customerId);

            var vehicleDtos = vehicleModels.Select(vh => new VehicleDto(vh, _vehicleService.IsConnected(vh)));

            return Ok(vehicleDtos);
        }

        [HttpPost("{vehicleId}/connect")]
        public async Task<ActionResult> Connect(int vehicleId)
        {
            var vehicleModel = await _vehicleService.GetAsync(vehicleId);
            if (vehicleModel == null)
            {
                return NotFound(new ErrorDto($"Vehicle by id '{vehicleId}' not found."));
            }

            await _vehicleService.ConnectedAsync(vehicleModel);

            return Ok();
        }
    }
}