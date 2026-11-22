using EquipmentBorrowing.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentBorrowing.Api.Controllers;

[ApiController]
public class BorrowingsController : ControllerBase
{
    // API-10: POST /api/v1/borrowings
    [HttpPost("api/v1/borrowings")]
    public ActionResult<BorrowingDto> Create([FromBody] CreateBorrowingRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-11: GET /api/v1/borrowings/{id}
    [HttpGet("api/v1/borrowings/{id:guid}")]
    public ActionResult<BorrowingDto> GetById(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-12: POST /api/v1/borrowings/{id}/return
    [HttpPost("api/v1/borrowings/{id:guid}/return")]
    public ActionResult<BorrowingDto> ReturnBorrowing(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-13: GET /api/v1/borrowings
    [HttpGet("api/v1/borrowings")]
    public ActionResult<List<BorrowingDto>> Search(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? sort,
        [FromQuery] string? direction)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    // API-16: POST /api/v1/checkout-requests
    [HttpPost("api/v1/checkout-requests")]
    public ActionResult<CheckoutResponse> CheckoutMultiple([FromBody] CheckoutRequest request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}
