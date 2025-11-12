using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _service;
        public SessionController(SessionService service) => _service = service;

        [HttpGet]
        public ActionResult<List<Session>> Get() => _service.GetAll();

        [HttpGet("{id}")]
        public ActionResult<Session> Get(string id)
        {
            var session = _service.Get(id);
            if (session == null) return NotFound();
            return session;
        }

        [HttpPost]
        public ActionResult<Session> Create(Session session)
        {
            _service.Create(session);
            return CreatedAtAction(nameof(Get), new { id = session.Id }, session);
        }
    }
}