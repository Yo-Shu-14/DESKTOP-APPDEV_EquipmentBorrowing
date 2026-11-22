using EquipmentBorrowing.Api.Dtos;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
[Route("api/v1/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly RegisterStudentService _registerService;
    private readonly UpdateStudentInformationService _updateService;
    private readonly ChangeStudentEligibilityService _eligibilityService;
    private readonly ViewStudentBorrowingHistoryService _historyService;

    public StudentsController(
        IStudentRepository studentRepository,
        RegisterStudentService registerService,
        UpdateStudentInformationService updateService,
        ChangeStudentEligibilityService eligibilityService,
        ViewStudentBorrowingHistoryService historyService)
    {
        _studentRepository = studentRepository;
        _registerService = registerService;
        _updateService = updateService;
        _eligibilityService = eligibilityService;
        _historyService = historyService;
    }

    // API-02: GET /api/v1/students
    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAll([FromQuery] string? search)
    {
        var students = await _studentRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLowerInvariant();
            students = students
                .Where(s => s.Name.ToLowerInvariant().Contains(q) ||
                            s.InstitutionalNumber.ToLowerInvariant().Contains(q))
                .ToList();
        }

        return Ok(students.Select(ToDto).ToList());
    }

    // API-03: POST /api/v1/students
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StudentNumber))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Institutional number is required."));

        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Student name is required."));

        try
        {
            var created = await _registerService.RegisterStudentAsync(
                request.StudentNumber,
                request.FullName,
                request.Contact);

            var dto = ToDto(created);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse("CONFLICT", ex.Message));
        }
    }

    // API-04: GET /api/v1/students/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Student not found."));

        return Ok(ToDto(student));
    }

    // API-05: PUT /api/v1/students/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentDto>> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StudentNumber))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Institutional number is required."));

        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Student name is required."));

        var existing = await _studentRepository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Student not found."));

        try
        {
            var updated = await _updateService.UpdateStudentAsync(
                id,
                request.StudentNumber,
                request.FullName,
                request.Contact);

            return Ok(ToDto(updated));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ErrorResponse("CONFLICT", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_OPERATION", ex.Message));
        }
    }

    // API-06: PATCH /api/v1/students/{id}/eligibility
    [HttpPatch("{id:int}/eligibility")]
    public async Task<ActionResult<StudentDto>> ChangeEligibility(int id, [FromBody] ChangeEligibilityRequest request)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Student not found."));

        // Spec requirement: same value is a 200 OK no-op (not error)
        if (student.IsAllowedToBorrow == request.IsEligible)
            return Ok(ToDto(student));

        try
        {
            var updated = await _eligibilityService.ChangeEligibilityAsync(id, request.IsEligible);
            return Ok(ToDto(updated));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_OPERATION", ex.Message));
        }
    }

    // API-14: GET /api/v1/students/{id}/borrowings
    [HttpGet("{id:int}/borrowings")]
    public async Task<ActionResult<List<BorrowingDto>>> GetBorrowings(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Student not found."));

        var borrowings = await _historyService.GetStudentHistoryAsync(id);

        var dtos = borrowings.Select(b => new BorrowingDto(
            b.BorrowingId,
            b.StudentId,
            b.Student?.InstitutionalNumber ?? student.InstitutionalNumber,
            b.Student?.Name ?? student.Name,
            b.EquipmentId,
            b.Equipment?.EquipmentIdentifier ?? "",
            b.Equipment?.Name ?? "",
            b.DateBorrowed.ToString("yyyy-MM-dd"),
            b.ExpectedReturnDate.ToString("yyyy-MM-dd"),
            b.ActualReturnDate?.ToString("yyyy-MM-dd"),
            b.Status.ToString()
        )).ToList();

        return Ok(dtos);
    }

    private static StudentDto ToDto(Student s) => new(
        s.Id,
        s.InstitutionalNumber,
        s.Name,
        s.ContactInformation,
        s.IsAllowedToBorrow);
}
