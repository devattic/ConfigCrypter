using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Example.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly NestedSettings _settingsSnapshot;

        public ConfigController(IConfiguration configuration, IOptionsSnapshot<NestedSettings> settingsSnapshot)
        {
            _configuration = configuration;
            _settingsSnapshot = settingsSnapshot.Value;
        }

        [HttpGet]
        public IActionResult GetEncryptedKey()
        {
            var content = new
            {
                FromSnapshot = _settingsSnapshot.KeyToEncrypt,
                FromConfiguration = _configuration["Nested:KeyToEncrypt"]
            };

            return Ok(content);
        }
    }
}
