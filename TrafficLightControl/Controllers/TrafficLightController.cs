using Microsoft.AspNetCore.Mvc;
using TrafficLightControl.Models;
using TrafficLightControl.Services;

namespace TrafficLightControl.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficLightController : ControllerBase
    {
        private readonly TrafficLightService _service;

        public TrafficLightController(TrafficLightService service)
        {
            _service = service;
        }

        [HttpGet("status")]
        public ActionResult<TrafficLight> GetStatus() => Ok(_service.GetStatus());

        [HttpPost("manual")]
        public IActionResult SetManual([FromQuery] TrafficLightState state)
        {
            _service.SetManual(state);
            return Ok($"Manual override: {state}");
        }

        [HttpPost("pedestrian")]
        public async Task<IActionResult> PedestrianCross()
        {
            await _service.PedestrianCrossAsync();
            return Ok("Pedestrian crossing started");
        }
    }
}
