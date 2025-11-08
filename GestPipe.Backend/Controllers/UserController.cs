using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/user/{id}
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST /api/user/{id}/language
        // Body: raw string JSON e.g. "vi-VN"
        [HttpPost("{id}/language")]
        public IActionResult SetLanguage(string id, [FromBody] string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return BadRequest("language required");

            var ok = _userService.SetLanguage(id, language);
            if (!ok) return BadRequest("Invalid language or user not found");
            return NoContent();
        }
    }
}