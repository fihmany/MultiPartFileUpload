using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiPartFileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Files : ControllerBase
    {
        private readonly ILogger<Files> _logger;

        public Files(ILogger<Files> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
