using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _service;
        public TopicController(ITopicService service) => _service = service;

        [HttpGet]
        public ActionResult<List<Topic>> Get() => _service.GetAll();
    }
}