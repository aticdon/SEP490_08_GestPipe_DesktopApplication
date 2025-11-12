using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;
        public CategoryController(CategoryService service) => _service = service;

        [HttpGet]
        public ActionResult<List<Category>> Get() => _service.GetAll();

        [HttpGet("{id}")]
        public ActionResult<Category> Get(string id)
        {
            var cat = _service.Get(id);
            if (cat == null) return NotFound();
            return cat;
        }

        [HttpPost]
        public ActionResult<Category> Create(Category cat)
        {
            _service.Create(cat);
            return CreatedAtAction(nameof(Get), new { id = cat.Id }, cat);
        }
    }
}