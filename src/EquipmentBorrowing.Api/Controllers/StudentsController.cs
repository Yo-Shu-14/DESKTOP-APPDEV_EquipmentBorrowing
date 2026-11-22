using EquipmentBorrowing.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    // API-02: GET /api/v1/students
    [HttpGet]
    public ActionResult<List<StudentDto>> GetAll([FromQuery] string? search)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-03: POST /api/v1/students
    [HttpPost]
    public ActionResult<StudentDto> Create([FromBody] CreateStudentRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-04: GET /api/v1/students/{id}
    [HttpGet("{id:int}")]
    public ActionResult<StudentDto> GetById(int id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-05: PUT /api/v1/students/{id}
    [HttpPut("{id:int}")]
    public ActionResult<StudentDto> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-06: PATCH /api/v1/students/{id}/eligibility
    [HttpPatch("{id:int}/eligibility")]
    public ActionResult<StudentDto> ChangeEligibility(int id, [FromBody] ChangeEligibilityRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-14: GET /api/v1/students/{id}/borrowings
    [HttpGet("{id:int}/borrowings")]
    public ActionResult<List<BorrowingDto>> GetBorrowings(int id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}
