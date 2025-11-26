using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestPipe.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) => _service = service;

        [HttpGet]
        public ActionResult<List<Category>> Get() => _service.GetAll();
    }
}