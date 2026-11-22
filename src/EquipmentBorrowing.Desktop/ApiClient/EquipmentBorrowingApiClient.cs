using System.Net.Http.Json;
using System.Text.Json;

namespace EquipmentBorrowing.Desktop.ApiClient;

/// <summary>
/// Typed HTTP client for the Equipment Borrowing REST API.
/// Distinguishes connection failures (no response) from server errors (5xx) and client errors (4xx).
/// </summary>
public class EquipmentBorrowingApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public EquipmentBorrowingApiClient(HttpClient http)
    {
        _http = http;
    }

    // ─── Equipment ───────────────────────────────────────────────────────────────

    public Task<List<EquipmentDto>> GetEquipmentAsync(bool? availableOnly = null)
    {
        var url = availableOnly == true ? "api/v1/equipment?available=true" : "api/v1/equipment";
        return GetAsync<List<EquipmentDto>>(url);
    }

    public Task<EquipmentDto> GetEquipmentByIdAsync(Guid id) =>
        GetAsync<EquipmentDto>($"api/v1/equipment/{id}");

    public Task<EquipmentDto> CreateEquipmentAsync(CreateEquipmentRequest request) =>
        PostAsync<CreateEquipmentRequest, EquipmentDto>("api/v1/equipment", request);

    public Task<EquipmentDto> UpdateEquipmentAsync(Guid id, UpdateEquipmentRequest request) =>
        PutAsync<UpdateEquipmentRequest, EquipmentDto>($"api/v1/equipment/{id}", request);

    public Task<List<BorrowingDto>> GetEquipmentBorrowingsAsync(Guid id) =>
        GetAsync<List<BorrowingDto>>($"api/v1/equipment/{id}/borrowings");

    // ─── Students ────────────────────────────────────────────────────────────────

    public Task<List<StudentDto>> GetStudentsAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "api/v1/students"
            : $"api/v1/students?search={Uri.EscapeDataString(search)}";
        return GetAsync<List<StudentDto>>(url);
    }

    public Task<StudentDto> GetStudentByIdAsync(int id) =>
        GetAsync<StudentDto>($"api/v1/students/{id}");

    public Task<StudentDto> CreateStudentAsync(CreateStudentRequest request) =>
        PostAsync<CreateStudentRequest, StudentDto>("api/v1/students", request);

    public Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentRequest request) =>
        PutAsync<UpdateStudentRequest, StudentDto>($"api/v1/students/{id}", request);

    public Task<StudentDto> ChangeEligibilityAsync(int id, bool isEligible) =>
        PatchAsync<ChangeEligibilityRequest, StudentDto>(
            $"api/v1/students/{id}/eligibility",
            new ChangeEligibilityRequest(isEligible));

    public Task<List<BorrowingDto>> GetStudentBorrowingsAsync(int id) =>
        GetAsync<List<BorrowingDto>>($"api/v1/students/{id}/borrowings");

    // ─── Borrowings ──────────────────────────────────────────────────────────────

    public Task<BorrowingDto> GetBorrowingByIdAsync(Guid id) =>
        GetAsync<BorrowingDto>($"api/v1/borrowings/{id}");

    public Task<List<BorrowingDto>> GetBorrowingsAsync(
        string? search = null,
        string? status = null,
        string? sort = null,
        string? direction = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(status)) query.Add($"status={Uri.EscapeDataString(status)}");
        if (!string.IsNullOrWhiteSpace(sort)) query.Add($"sort={Uri.EscapeDataString(sort)}");
        if (!string.IsNullOrWhiteSpace(direction)) query.Add($"direction={Uri.EscapeDataString(direction)}");

        var url = "api/v1/borrowings" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
        return GetAsync<List<BorrowingDto>>(url);
    }

    public Task<BorrowingDto> CreateBorrowingAsync(CreateBorrowingRequest request) =>
        PostAsync<CreateBorrowingRequest, BorrowingDto>("api/v1/borrowings", request);

    public Task<CheckoutResponse> CheckoutMultipleAsync(CheckoutRequest request) =>
        PostAsync<CheckoutRequest, CheckoutResponse>("api/v1/checkout-requests", request);

    public Task<BorrowingDto> ReturnBorrowingAsync(Guid id) =>
        PostAsync<object, BorrowingDto>($"api/v1/borrowings/{id}/return", new { });

    // ─── Helpers ─────────────────────────────────────────────────────────────────

    private async Task<T> GetAsync<T>(string url)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.GetAsync(url);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiConnectionException("Cannot reach the API server. Check that the API is running.", ex);
        }

        return await ReadResponse<T>(response);
    }

    private async Task<TResult> PostAsync<TBody, TResult>(string url, TBody body)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync(url, body, _json);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiConnectionException("Cannot reach the API server. Check that the API is running.", ex);
        }

        return await ReadResponse<TResult>(response);
    }

    private async Task<TResult> PutAsync<TBody, TResult>(string url, TBody body)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.PutAsJsonAsync(url, body, _json);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiConnectionException("Cannot reach the API server. Check that the API is running.", ex);
        }

        return await ReadResponse<TResult>(response);
    }

    private async Task<TResult> PatchAsync<TBody, TResult>(string url, TBody body)
    {
        HttpResponseMessage response;
        try
        {
            var content = JsonContent.Create(body, options: _json);
            response = await _http.PatchAsync(url, content);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiConnectionException("Cannot reach the API server. Check that the API is running.", ex);
        }

        return await ReadResponse<TResult>(response);
    }

    private static async Task<T> ReadResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>(_json);
            return result!;
        }

        string? rawBody = null;
        string message;
        try
        {
            rawBody = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(rawBody, _json);
            message = error?.Message ?? rawBody;
        }
        catch
        {
            message = rawBody ?? response.ReasonPhrase ?? "Unknown error.";
        }

        throw response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => new ApiNotFoundException(message),
            System.Net.HttpStatusCode.Conflict => new ApiConflictException(message),
            System.Net.HttpStatusCode.BadRequest => new ApiBadRequestException(message),
            System.Net.HttpStatusCode.InternalServerError => new ApiServerException($"Server error: {message}"),
            _ => new ApiException($"HTTP {(int)response.StatusCode}: {message}")
        };
    }
}

// ─── Custom Exceptions ─────────────────────────────────────────────────────────

public class ApiException(string message, Exception? inner = null) : Exception(message, inner);
public class ApiConnectionException(string message, Exception? inner = null) : ApiException(message, inner);
public class ApiNotFoundException(string message) : ApiException(message);
public class ApiConflictException(string message) : ApiException(message);
public class ApiBadRequestException(string message) : ApiException(message);
public class ApiServerException(string message) : ApiException(message);
