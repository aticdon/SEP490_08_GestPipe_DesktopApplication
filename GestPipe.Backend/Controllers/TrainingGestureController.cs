using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingGestureController : ControllerBase
    {
        private readonly ITrainingGestureService _service;

        public TrainingGestureController(ITrainingGestureService service)
        {
            _service = service;
        }
        [HttpGet]
        public ActionResult<List<TrainingGesture>> Get() => _service.GetAll();

        [HttpGet("{id}")]
        public ActionResult<TrainingGesture> Get(string id)
        {
            var tg = _service.Get(id);
            if (tg == null) return NotFound();
            return tg;
        }

        [HttpPost]
        public ActionResult<TrainingGesture> Create(TrainingGesture trainingGesture)
        {
            _service.Create(trainingGesture);
            return CreatedAtAction(nameof(Get), new { id = trainingGesture.Id }, trainingGesture);
        }
    }
}