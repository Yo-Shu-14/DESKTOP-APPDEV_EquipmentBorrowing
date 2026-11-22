using System.Globalization;
using EquipmentBorrowing.Api.Dtos;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
public class BorrowingsController : ControllerBase
{
    private const int MaxActiveBorrowings = 3;

    private readonly IBorrowingRepository _borrowingRepository;
    private readonly BorrowEquipmentService _borrowService;
    private readonly ReturnEquipmentService _returnService;
    private readonly SearchBorrowingsService _searchService;

    public BorrowingsController(
        IBorrowingRepository borrowingRepository,
        BorrowEquipmentService borrowService,
        ReturnEquipmentService returnService,
        SearchBorrowingsService searchService)
    {
        _borrowingRepository = borrowingRepository;
        _borrowService = borrowService;
        _returnService = returnService;
        _searchService = searchService;
    }

    // API-10: POST /api/v1/borrowings
    [HttpPost("api/v1/borrowings")]
    public async Task<ActionResult<BorrowingDto>> Create([FromBody] CreateBorrowingRequest request)
    {
        if (request.StudentId <= 0)
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "A valid student ID is required."));

        if (request.EquipmentId == Guid.Empty)
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "A valid equipment ID is required."));

        if (!TryParseFutureDate(request.ExpectedReturnDate, out var returnDate, out var dateError))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", dateError!));

        try
        {
            var created = await _borrowService.BorrowEquipmentAsync(
                request.StudentId,
                request.EquipmentId,
                MaxActiveBorrowings,
                returnDate);

            var loaded = await _borrowingRepository.GetByIdAsync(created.BorrowingId) ?? created;
            var dto = ToDto(loaded);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ErrorResponse("NOT_FOUND", ex.Message));
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("not available", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("maximum number", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("no longer available", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ErrorResponse("CONFLICT", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_OPERATION", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
        }
    }

    // API-11: GET /api/v1/borrowings/{id}
    [HttpGet("api/v1/borrowings/{id:guid}")]
    public async Task<ActionResult<BorrowingDto>> GetById(Guid id)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(id);
        if (borrowing is null)
            return NotFound(new ErrorResponse("NOT_FOUND", "Borrowing record not found."));

        return Ok(ToDto(borrowing));
    }

    // API-12: POST /api/v1/borrowings/{id}/return
    [HttpPost("api/v1/borrowings/{id:guid}/return")]
    public async Task<ActionResult<BorrowingDto>> ReturnBorrowing(Guid id)
    {
        try
        {
            await _returnService.ReturnEquipmentAsync(id);

            var updated = await _borrowingRepository.GetByIdAsync(id);
            if (updated is null)
                return NotFound(new ErrorResponse("NOT_FOUND", "Borrowing does not exist."));

            return Ok(ToDto(updated));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ErrorResponse("NOT_FOUND", ex.Message));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already been returned", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ErrorResponse("CONFLICT", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_OPERATION", ex.Message));
        }
    }

    // API-13: GET /api/v1/borrowings
    [HttpGet("api/v1/borrowings")]
    public async Task<ActionResult<List<BorrowingDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sort,
        [FromQuery] string? direction)
    {
        var all = await _searchService.GetAllBorrowingsAsync();
        IEnumerable<Borrowing> query = all;

        // Status filter (Active | Returned | All)
        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                query = query.Where(b => b.Status == BorrowingStatus.Active);
            else if (status.Equals("Returned", StringComparison.OrdinalIgnoreCase))
                query = query.Where(b => b.Status == BorrowingStatus.Returned);
        }

        // Search text
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(b =>
                (b.Student?.Name != null && b.Student.Name.ToLowerInvariant().Contains(s)) ||
                (b.Student?.InstitutionalNumber != null && b.Student.InstitutionalNumber.ToLowerInvariant().Contains(s)) ||
                (b.Equipment?.Name != null && b.Equipment.Name.ToLowerInvariant().Contains(s)) ||
                (b.Equipment?.EquipmentIdentifier != null && b.Equipment.EquipmentIdentifier.ToLowerInvariant().Contains(s)) ||
                b.BorrowingId.ToString().ToLowerInvariant().Contains(s));
        }

        // Sort field (borrowedDate | expectedReturnDate) - default borrowedDate
        bool isExpectedReturn = string.Equals(sort, "expectedReturnDate", StringComparison.OrdinalIgnoreCase);
        bool isDescending = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase);

        query = (isExpectedReturn, isDescending) switch
        {
            (true, true) => query.OrderByDescending(b => b.ExpectedReturnDate),
            (true, false) => query.OrderBy(b => b.ExpectedReturnDate),
            (false, true) => query.OrderByDescending(b => b.DateBorrowed),
            (false, false) => query.OrderBy(b => b.DateBorrowed)
        };

        var dtos = query.Select(ToDto).ToList();
        return Ok(dtos);
    }

    // API-16: POST /api/v1/checkout-requests
    [HttpPost("api/v1/checkout-requests")]
    public async Task<ActionResult<CheckoutResponse>> CheckoutMultiple([FromBody] CheckoutRequest request)
    {
        if (request.StudentId <= 0)
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "A valid student ID is required."));

        if (request.EquipmentIds == null || request.EquipmentIds.Count < 2)
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Please select at least two equipment items."));

        if (request.EquipmentIds.Distinct().Count() != request.EquipmentIds.Count)
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", "Duplicate equipment items are not allowed."));

        if (!TryParseFutureDate(request.ExpectedReturnDate, out var returnDate, out var dateError))
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", dateError!));

        try
        {
            var createdList = await _borrowService.BorrowMultipleEquipmentAsync(
                request.StudentId,
                request.EquipmentIds,
                MaxActiveBorrowings,
                returnDate);

            var dtos = new List<BorrowingDto>();
            foreach (var created in createdList)
            {
                var loaded = await _borrowingRepository.GetByIdAsync(created.BorrowingId) ?? created;
                dtos.Add(ToDto(loaded));
            }

            return StatusCode(StatusCodes.Status201Created, new CheckoutResponse(dtos));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ErrorResponse("NOT_FOUND", ex.Message));
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("not available", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("exceed the maximum", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("maximum number", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("no longer available", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ErrorResponse("CONFLICT", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_OPERATION", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("VALIDATION_ERROR", ex.Message));
        }
    }

    private static bool TryParseFutureDate(string? input, out DateTime date, out string? error)
    {
        date = default;
        error = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Expected return date is required.";
            return false;
        }

        if (!DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) &&
            !DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            error = "Expected return date must be in YYYY-MM-DD format.";
            return false;
        }

        if (date.Date <= DateTime.Today)
        {
            error = "Expected return date must be in the future.";
            return false;
        }

        return true;
    }

    private static BorrowingDto ToDto(Borrowing b) => new(
        b.BorrowingId,
        b.StudentId,
        b.Student?.InstitutionalNumber ?? "",
        b.Student?.Name ?? "",
        b.EquipmentId,
        b.Equipment?.EquipmentIdentifier ?? "",
        b.Equipment?.Name ?? "",
        b.DateBorrowed.ToString("yyyy-MM-dd"),
        b.ExpectedReturnDate.ToString("yyyy-MM-dd"),
        b.ActualReturnDate?.ToString("yyyy-MM-dd"),
        b.Status.ToString());
}
