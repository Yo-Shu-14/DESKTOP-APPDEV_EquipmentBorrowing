namespace EquipmentBorrowing.Api.Dtos;

// ─── Student DTOs ─────────────────────────────────────────────────────────────

public record StudentDto(
    int Id,
    string StudentNumber,
    string FullName,
    string? Contact,
    bool IsEligible);

public record CreateStudentRequest(
    string StudentNumber,
    string FullName,
    string? Contact);

public record UpdateStudentRequest(
    string StudentNumber,
    string FullName,
    string? Contact);

public record ChangeEligibilityRequest(bool IsEligible);

// ─── Equipment DTOs ───────────────────────────────────────────────────────────

public record EquipmentDto(
    Guid Id,
    string EquipmentCode,
    string Name,
    string? Description,
    bool IsAvailable);

public record CreateEquipmentRequest(
    string EquipmentCode,
    string Name,
    string? Description);

public record UpdateEquipmentRequest(
    string EquipmentCode,
    string Name,
    string? Description);

// ─── Borrowing DTOs ───────────────────────────────────────────────────────────

public record BorrowingDto(
    Guid Id,
    int StudentId,
    string StudentNumber,
    string StudentName,
    Guid EquipmentId,
    string EquipmentCode,
    string EquipmentName,
    string BorrowedDate,
    string ExpectedReturnDate,
    string? ReturnedDate,
    string Status);

public record CreateBorrowingRequest(
    int StudentId,
    Guid EquipmentId,
    string ExpectedReturnDate);

public record CheckoutRequest(
    int StudentId,
    List<Guid> EquipmentIds,
    string ExpectedReturnDate);

public record CheckoutResponse(List<BorrowingDto> Borrowings);

public record ErrorResponse(string Code, string Message);
