using System;
using Epitaph.Signaling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Epitaph.Signaling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _service;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomsService roomsService, ILogger<RoomsController> logger)
        {
            _service = roomsService
                ?? throw new ArgumentNullException();
            _logger = logger;
        }

        [HttpGet]
        // IEnumerable<KeyValuePair<string, string>>
        public IActionResult List()
        {
            return Ok(_service.GetRooms());
        }
    }
}
