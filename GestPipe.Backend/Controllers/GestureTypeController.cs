using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GestureTypeController : ControllerBase
{
    private readonly GestureTypeService _gestureTypeService;

    public GestureTypeController(GestureTypeService gestureTypeService)
    {
        _gestureTypeService = gestureTypeService;
    }

    [HttpGet]
    public ActionResult<List<GestureType>> Get() => _gestureTypeService.GetAll();

    [HttpGet("{id:length(24)}")]
    public ActionResult<GestureType> Get(string id)
    {
        var gestureType = _gestureTypeService.GetById(id);
        if (gestureType == null) return NotFound();
        return gestureType;
    }

    [HttpPost]
    public ActionResult<GestureType> Create(GestureType gestureType)
    {
        _gestureTypeService.Create(gestureType);
        return CreatedAtAction(nameof(Get), new { id = gestureType.Id }, gestureType);
    }

    [HttpPut("{id:length(24)}")]
    public IActionResult Update(string id, GestureType gestureTypeIn)
    {
        var gestureType = _gestureTypeService.GetById(id);
        if (gestureType == null) return NotFound();
        _gestureTypeService.Update(id, gestureTypeIn);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public IActionResult Delete(string id)
    {
        var gestureType = _gestureTypeService.GetById(id);
        if (gestureType == null) return NotFound();
        _gestureTypeService.Delete(id);
        return NoContent();
    }
}