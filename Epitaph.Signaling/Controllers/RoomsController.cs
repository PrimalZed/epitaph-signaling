using System;
using System.Linq;
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
            // Cast to stupid class because KeyValuePair doesn't use PropertyNamingPolicy
            // https://github.com/dotnet/runtime/issues/1197
            return Ok(_service.GetRooms().Select((pair) => new DummyKeyValuePair(pair.Key, pair.Value)));
        }

        private class DummyKeyValuePair
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public DummyKeyValuePair(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
