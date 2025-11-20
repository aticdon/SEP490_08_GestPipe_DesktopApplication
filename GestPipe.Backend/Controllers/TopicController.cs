using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly TopicService _service;
        public TopicController(TopicService service) => _service = service;

        [HttpGet]
        public ActionResult<List<Topic>> Get() => _service.GetAll();

        //[HttpGet("{id}")]
        //public ActionResult<Topic> Get(string id)
        //{
        //    var topic = _service.Get(id);
        //    if (topic == null) return NotFound();
        //    return topic;
        //}

        //[HttpPost]
        //public ActionResult<Topic> Create(Topic topic)
        //{
        //    _service.Create(topic);
        //    return CreatedAtAction(nameof(Get), new { id = topic.Id }, topic);
        //}
    }
}