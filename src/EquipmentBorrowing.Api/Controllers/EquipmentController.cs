using EquipmentBorrowing.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
[Route("api/v1/equipment")]
public class EquipmentController : ControllerBase
{
    // API-01: GET /api/v1/equipment
    [HttpGet]
    public ActionResult<List<EquipmentDto>> GetAll([FromQuery] bool? available)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-07: POST /api/v1/equipment
    [HttpPost]
    public ActionResult<EquipmentDto> Create([FromBody] CreateEquipmentRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-08: GET /api/v1/equipment/{id}
    [HttpGet("{id:guid}")]
    public ActionResult<EquipmentDto> GetById(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-09: PUT /api/v1/equipment/{id}
    [HttpPut("{id:guid}")]
    public ActionResult<EquipmentDto> Update(Guid id, [FromBody] UpdateEquipmentRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-15: GET /api/v1/equipment/{id}/borrowings
    [HttpGet("{id:guid}/borrowings")]
    public ActionResult<List<BorrowingDto>> GetBorrowings(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}
