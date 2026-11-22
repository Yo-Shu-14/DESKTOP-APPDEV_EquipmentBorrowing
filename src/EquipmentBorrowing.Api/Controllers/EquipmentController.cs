using EquipmentBorrowing.Api.Dtos;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
[Route("api/v1/equipment")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ViewEquipmentCatalogService _catalogService;
    private readonly CheckAvailableEquipmentService _availableService;
    private readonly RegisterEquipmentService _registerService;
    private readonly UpdateEquipmentInformationService _updateService;
    private readonly ViewEquipmentBorrowingHistoryService _historyService;

    public EquipmentController(
        IEquipmentRepository equipmentRepository,
        ViewEquipmentCatalogService catalogService,
        CheckAvailableEquipmentService availableService,
        RegisterEquipmentService registerService,
        UpdateEquipmentInformationService updateService,
        ViewEquipmentBorrowingHistoryService historyService)
    {
        _equipmentRepository = equipmentRepository;
        _catalogService = catalogService;
        _availableService = availableService;
        _registerService = registerService;
        _updateService = updateService;
        _historyService = historyService;
    }

    // API-01: GET /api/v1/equipment
    [HttpGet]
    public async Task<ActionResult<List<EquipmentDto>>> GetAll([FromQuery] bool? available)
    {
        var items = available == true
            ? await _availableService.CheckAvailableEquipmentAsync()
            : await _catalogService.ViewEquipmentCatalogAsync();

        return Ok(items.Select(ToDto).ToList());
    }

    // API-07: POST /api/v1/equipment
    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> Create([FromBody] CreateEquipmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentCode))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Equipment identifier is required."));

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Equipment name is required."));

        try
        {
            var created = await _registerService.RegisterEquipmentAsync(
                request.EquipmentCode,
                request.Name,
                request.Description);

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

    // API-08: GET /api/v1/equipment/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EquipmentDto>> GetById(Guid id)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(id);
        if (equipment is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Equipment not found."));

        return Ok(ToDto(equipment));
    }

    // API-09: PUT /api/v1/equipment/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EquipmentDto>> Update(Guid id, [FromBody] UpdateEquipmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentCode))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Equipment identifier is required."));

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Equipment name is required."));

        var existing = await _equipmentRepository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Equipment not found."));

        try
        {
            await _updateService.UpdateEquipmentInformationAsync(
                id,
                request.EquipmentCode,
                request.Name,
                request.Description);

            var updated = await _equipmentRepository.GetByIdAsync(id);
            return Ok(ToDto(updated!));
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

    // API-15: GET /api/v1/equipment/{id}/borrowings
    [HttpGet("{id:guid}/borrowings")]
    public async Task<ActionResult<List<BorrowingDto>>> GetBorrowings(Guid id)
    {
        var equipment = await _equipmentRepository.GetByIdAsync(id);
        if (equipment is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Equipment not found."));

        var borrowings = await _historyService.GetEquipmentHistoryAsync(id);

        var dtos = borrowings.Select(b => new BorrowingDto(
            b.BorrowingId,
            b.StudentId,
            b.Student?.InstitutionalNumber ?? "",
            b.Student?.Name ?? "",
            b.EquipmentId,
            b.Equipment?.EquipmentIdentifier ?? equipment.EquipmentIdentifier,
            b.Equipment?.Name ?? equipment.Name,
            b.DateBorrowed.ToString("yyyy-MM-dd"),
            b.ExpectedReturnDate.ToString("yyyy-MM-dd"),
            b.ActualReturnDate?.ToString("yyyy-MM-dd"),
            b.Status.ToString()
        )).ToList();

        return Ok(dtos);
    }

    private static EquipmentDto ToDto(Equipment e) => new(
        e.EquipmentId,
        e.EquipmentIdentifier,
        e.Name,
        e.Description,
        e.IsAvailable);
}
