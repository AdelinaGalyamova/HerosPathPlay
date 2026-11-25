using HerosPath.Requests;
using HerosPath.Requests;
using HerosPath.Service;
using Microsoft.AspNetCore.Mvc;

namespace HerosPath.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HerosPathController : ControllerBase
    {
        private readonly HerosPathService _service;

        public HerosPathController(HerosPathService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _service.Register(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _service.Login(request);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            var result = await _service.Logout(token);
            return Ok(result);
        }

        [HttpGet("user/{token}")]
        public async Task<IActionResult> GetUser(string token)
        {
            var result = await _service.GetUser(token);
            return Ok(result);
        }

        [HttpGet("levels/{token}")]
        public async Task<IActionResult> GetLevels(string token)
        {
            var result = await _service.GetLevels(token);
            return Ok(result);
        }

        [HttpGet("progress/{token}")]
        public async Task<IActionResult> GetProgress(string token)
        {
            var result = await _service.GetProgress(token);
            return Ok(result);
        }

        [HttpPost("save-progress")]
        public async Task<IActionResult> SaveProgress([FromBody] SaveProgressRequest request)
        {
            var result = await _service.SaveProgress(request);
            return Ok(result);
        }
    }
}