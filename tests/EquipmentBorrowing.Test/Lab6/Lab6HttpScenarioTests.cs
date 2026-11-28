using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EquipmentBorrowing.Tests.Lab6;

[Collection("Lab 6 PostgreSQL database")]
public sealed class Lab6HttpScenarioTests
{
    private static readonly Guid E1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid E2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid E3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid E4 = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid E5 = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid E6 = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private static readonly Guid E7 = Guid.Parse("77777777-7777-7777-7777-777777777777");

    private readonly Lab6DatabaseFixture _database;

    public Lab6HttpScenarioTests(Lab6DatabaseFixture database) => _database = database;

    [Fact(DisplayName = "TC6-U01 Catalog")]
    public async Task Catalog_contains_the_controlled_fixture()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.GetAsync("api/v1/equipment");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var items = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(items);
        Assert.Equal(7, items!.Length);
        Assert.Contains(items, item => item.GetProperty("equipmentCode").GetString() == "EQ-001" && !item.GetProperty("isAvailable").GetBoolean());
        Assert.Contains(items, item => item.GetProperty("equipmentCode").GetString() == "EQ-002" && item.GetProperty("isAvailable").GetBoolean());
        Assert.Contains(items, item => item.GetProperty("equipmentCode").GetString() == "EQ-007" && item.GetProperty("isAvailable").GetBoolean());
    }

    [Fact(DisplayName = "TC6-U02 Available filtering")]
    public async Task Available_filter_excludes_active_items()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var items = await client.GetFromJsonAsync<JsonElement[]>("api/v1/equipment?available=true");
        Assert.NotNull(items);
        var codes = items!.Select(item => item.GetProperty("equipmentCode").GetString()).ToHashSet();
        Assert.Equal(new[] { "EQ-002", "EQ-003", "EQ-006", "EQ-007" }, codes.OrderBy(x => x));
        Assert.DoesNotContain("EQ-001", codes);
        Assert.DoesNotContain("EQ-004", codes);
        Assert.DoesNotContain("EQ-005", codes);
    }

    [Fact(DisplayName = "TC6-U03 Single borrowing")]
    public async Task Single_borrowing_returns_active_record_and_persists_item_state()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsJsonAsync("api/v1/borrowings", new
        {
            studentId = 4,
            equipmentId = E2,
            expectedReturnDate = _database.D.AddDays(2).ToString("yyyy-MM-dd")
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var borrowing = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(borrowing);
        Assert.Equal("Active", borrowing!.GetProperty("status").GetString());
        Assert.Equal(4, borrowing.GetProperty("studentId").GetInt32());
        Assert.Equal(E2, borrowing.GetProperty("equipmentId").GetGuid());

        await using var db = _database.CreateContext();
        var persisted = await db.Borrowings.AsNoTracking().SingleAsync(b => b.EquipmentId == E2 && b.StudentId == 4);
        var equipment = await db.Equipment.AsNoTracking().SingleAsync(e => e.EquipmentId == E2);
        Assert.Equal(EquipmentBorrowing.Domain.BorrowingStatus.Active, persisted.Status);
        Assert.False(equipment.IsAvailable);
    }

    [Fact(DisplayName = "TC6-U04 Active borrowings")]
    public async Task Active_borrowings_include_only_active_fixture_records()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var items = await client.GetFromJsonAsync<JsonElement[]>("api/v1/borrowings?status=Active");
        Assert.NotNull(items);
        var equipment = items!.Select(item => item.GetProperty("equipmentCode").GetString()).ToHashSet();
        Assert.Equal(new[] { "EQ-001", "EQ-004", "EQ-005" }, equipment.OrderBy(x => x));
        Assert.DoesNotContain("EQ-003", equipment);
    }

    [Fact(DisplayName = "TC6-U05 Return and history")]
    public async Task Return_transitions_record_and_rejects_repeat_return()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsync("api/v1/borrowings/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/return", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var returned = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Returned", returned.GetProperty("status").GetString());
        Assert.False(string.IsNullOrWhiteSpace(returned.GetProperty("returnedDate").GetString()));

        using var repeat = await client.PostAsync("api/v1/borrowings/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/return", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.Conflict, repeat.StatusCode);

        await using var db = _database.CreateContext();
        var equipment = await db.Equipment.AsNoTracking().SingleAsync(e => e.EquipmentId == E1);
        Assert.True(equipment.IsAvailable);
    }

    [Fact(DisplayName = "TC6-U06 Atomic multi-item success")]
    public async Task Multi_item_checkout_creates_two_records_and_updates_both_items()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsJsonAsync("api/v1/checkout-requests", new
        {
            studentId = 4,
            equipmentIds = new[] { E2, E6 },
            expectedReturnDate = _database.D.AddDays(2).ToString("yyyy-MM-dd")
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, result.GetProperty("borrowings").GetArrayLength());

        await using var db = _database.CreateContext();
        Assert.Equal(2, await db.Borrowings.CountAsync(b => b.StudentId == 4 && b.Status == EquipmentBorrowing.Domain.BorrowingStatus.Active));
        Assert.False(await db.Equipment.Where(e => e.EquipmentId == E2 || e.EquipmentId == E6).AnyAsync(e => e.IsAvailable));
    }

    [Fact(DisplayName = "TC6-U07 Student history")]
    public async Task Student_history_returns_records_and_empty_existing_history()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var s1 = await client.GetFromJsonAsync<JsonElement[]>("api/v1/students/1/borrowings");
        var s4 = await client.GetFromJsonAsync<JsonElement[]>("api/v1/students/4/borrowings");
        using var missing = await client.GetAsync("api/v1/students/999/borrowings");

        Assert.NotNull(s1);
        Assert.Equal(2, s1!.Length);
        Assert.Empty(s4!);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact(DisplayName = "TC6-U08 Equipment history")]
    public async Task Equipment_history_returns_related_records_and_empty_history()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var e3 = await client.GetFromJsonAsync<JsonElement[]>("api/v1/equipment/33333333-3333-3333-3333-333333333333/borrowings");
        var e7 = await client.GetFromJsonAsync<JsonElement[]>("api/v1/equipment/77777777-7777-7777-7777-777777777777/borrowings");

        Assert.NotNull(e3);
        Assert.Single(e3!);
        Assert.Equal("Returned", e3[0].GetProperty("status").GetString());
        Assert.Empty(e7!);
    }

    [Fact(DisplayName = "TC6-U09 Search and filter borrowings")]
    public async Task Search_and_status_filter_return_expected_records()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var active = await client.GetFromJsonAsync<JsonElement[]>("api/v1/borrowings?status=Active&search=EQ-00");
        Assert.NotNull(active);
        Assert.Equal(3, active!.Length);
        Assert.All(active, item => Assert.Equal("Active", item.GetProperty("status").GetString()));
    }

    [Fact(DisplayName = "TC6-U12 Register student")]
    public async Task Register_student_returns_created_eligible_student()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsJsonAsync("api/v1/students", new
        {
            studentNumber = "ST-MA6-12",
            fullName = "Lab Six Student",
            contact = (string?)null
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var student = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("ST-MA6-12", student.GetProperty("studentNumber").GetString());
        Assert.True(student.GetProperty("isEligible").GetBoolean());
    }

    [Fact(DisplayName = "TC6-U13 Update student")]
    public async Task Update_student_preserves_identity_and_eligibility()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PutAsJsonAsync("api/v1/students/4", new
        {
            studentNumber = "ST-400",
            fullName = "Dani Updated",
            contact = (string?)null
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var student = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(4, student.GetProperty("id").GetInt32());
        Assert.Equal("Dani Updated", student.GetProperty("fullName").GetString());
        Assert.True(student.GetProperty("isEligible").GetBoolean());
    }

    [Fact(DisplayName = "TC6-U14 Change eligibility")]
    public async Task Eligibility_change_is_visible_and_same_value_is_noop()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PatchAsJsonAsync("api/v1/students/2/eligibility", new { isEligible = true });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var student = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(student.GetProperty("isEligible").GetBoolean());

        using var noOp = await client.PatchAsJsonAsync("api/v1/students/2/eligibility", new { isEligible = true });
        Assert.Equal(HttpStatusCode.OK, noOp.StatusCode);
    }

    [Fact(DisplayName = "TC6-U15 Register equipment")]
    public async Task Register_equipment_returns_available_item()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsJsonAsync("api/v1/equipment", new
        {
            equipmentCode = "EQ-MA6-15",
            name = "Lab Six Equipment",
            description = (string?)null
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var equipment = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("EQ-MA6-15", equipment.GetProperty("equipmentCode").GetString());
        Assert.True(equipment.GetProperty("isAvailable").GetBoolean());
    }

    [Fact(DisplayName = "TC6-U16 Update equipment")]
    public async Task Update_equipment_preserves_identity_and_availability()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PutAsJsonAsync($"api/v1/equipment/{E1}", new
        {
            equipmentCode = "EQ-001",
            name = "Camera Updated",
            description = (string?)null
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var equipment = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(E1, equipment.GetProperty("id").GetGuid());
        Assert.Equal("Camera Updated", equipment.GetProperty("name").GetString());
        Assert.False(equipment.GetProperty("isAvailable").GetBoolean());
    }

    [Fact(DisplayName = "TC6-E03 Invalid routes and malformed requests")]
    public async Task Invalid_route_and_malformed_json_are_rejected()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var route = await client.GetAsync("api/v1/not-a-real-route");
        using var malformed = new HttpRequestMessage(HttpMethod.Post, "api/v1/students")
        {
            Content = new StringContent("{\"studentNumber\":", System.Text.Encoding.UTF8, "application/json")
        };
        using var malformedResponse = await client.SendAsync(malformed);

        Assert.Equal(HttpStatusCode.NotFound, route.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, malformedResponse.StatusCode);
    }

    [Fact(DisplayName = "TC6-E01 Invalid input and duplicate records")]
    public async Task Invalid_and_duplicate_registration_requests_are_rejected()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var blankStudent = await client.PostAsJsonAsync("api/v1/students", new
        {
            studentNumber = "",
            fullName = "",
            contact = (string?)null
        });
        using var duplicateStudent = await client.PostAsJsonAsync("api/v1/students", new
        {
            studentNumber = "ST-100",
            fullName = "Duplicate Student",
            contact = (string?)null
        });
        using var blankEquipment = await client.PostAsJsonAsync("api/v1/equipment", new
        {
            equipmentCode = "",
            name = "",
            description = (string?)null
        });
        using var duplicateEquipment = await client.PostAsJsonAsync("api/v1/equipment", new
        {
            equipmentCode = "EQ-001",
            name = "Duplicate Equipment",
            description = (string?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, blankStudent.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateStudent.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, blankEquipment.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateEquipment.StatusCode);
    }

    [Fact(DisplayName = "TC6-E02 Borrowing rules")]
    public async Task Borrowing_rules_reject_ineligible_unavailable_and_past_due_requests()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        var pastDate = _database.D.AddDays(-1).ToString("yyyy-MM-dd");
        var futureDate = _database.D.AddDays(2).ToString("yyyy-MM-dd");
        using var ineligible = await client.PostAsJsonAsync("api/v1/borrowings", new { studentId = 2, equipmentId = E2, expectedReturnDate = futureDate });
        using var unavailable = await client.PostAsJsonAsync("api/v1/borrowings", new { studentId = 4, equipmentId = E1, expectedReturnDate = futureDate });
        using var past = await client.PostAsJsonAsync("api/v1/borrowings", new { studentId = 4, equipmentId = E2, expectedReturnDate = pastDate });

        Assert.Equal(HttpStatusCode.Conflict, ineligible.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, unavailable.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, past.StatusCode);
    }

    [Fact(DisplayName = "TC6-E04 PostgreSQL rollback on atomic conflict")]
    public async Task Failed_multi_item_checkout_leaves_both_items_unchanged()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var response = await client.PostAsJsonAsync("api/v1/checkout-requests", new
        {
            studentId = 4,
            equipmentIds = new[] { E2, E1 },
            expectedReturnDate = _database.D.AddDays(2).ToString("yyyy-MM-dd")
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await using var db = _database.CreateContext();
        Assert.False(await db.Equipment.Where(e => e.EquipmentId == E2).Select(e => e.IsAvailable).SingleAsync() == false);
        Assert.Equal(0, await db.Borrowings.CountAsync(b => b.StudentId == 4));
    }

    [Fact(DisplayName = "TC6-E05 Stale and conflicting state")]
    public async Task Repeated_stale_borrow_request_returns_conflict()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();
        var request = new { studentId = 4, equipmentId = E2, expectedReturnDate = _database.D.AddDays(2).ToString("yyyy-MM-dd") };

        using var first = await client.PostAsJsonAsync("api/v1/borrowings", request);
        using var second = await client.PostAsJsonAsync("api/v1/borrowings", request);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        await using var db = _database.CreateContext();
        Assert.Equal(1, await db.Borrowings.CountAsync(b => b.EquipmentId == E2 && b.Status == EquipmentBorrowing.Domain.BorrowingStatus.Active));
    }

    [Fact(DisplayName = "TC6-E06 Optional and protected fields")]
    public async Task Optional_fields_clear_and_protected_state_is_preserved()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();

        using var clear = await client.PutAsJsonAsync($"api/v1/equipment/{E1}", new
        {
            equipmentCode = "EQ-001",
            name = "Camera Updated",
            description = (string?)null
        });
        using var invalid = await client.PutAsJsonAsync("api/v1/students/1", new
        {
            studentNumber = "",
            fullName = "Ada Cruz",
            contact = (string?)null
        });

        Assert.Equal(HttpStatusCode.OK, clear.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        await using var db = _database.CreateContext();
        var equipment = await db.Equipment.AsNoTracking().SingleAsync(e => e.EquipmentId == E1);
        Assert.Null(equipment.Description);
        Assert.False(equipment.IsAvailable);
    }

    [Fact(DisplayName = "TC6-E08 Fresh connections")]
    public async Task Borrowing_and_return_survive_fresh_contexts()
    {
        await _database.ExecuteResetScriptAsync();
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("api/v1/borrowings", new
        {
            studentId = 4,
            equipmentId = E2,
            expectedReturnDate = _database.D.AddDays(2).ToString("yyyy-MM-dd")
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        await using (var fresh = _database.CreateContext())
        {
            Assert.True(await fresh.Borrowings.AnyAsync(b => b.BorrowingId == id && b.Status == EquipmentBorrowing.Domain.BorrowingStatus.Active));
        }

        using var returned = await client.PostAsync($"api/v1/borrowings/{id}/return", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, returned.StatusCode);

        await using var anotherFresh = _database.CreateContext();
        var borrowing = await anotherFresh.Borrowings.AsNoTracking().SingleAsync(b => b.BorrowingId == id);
        var equipment = await anotherFresh.Equipment.AsNoTracking().SingleAsync(e => e.EquipmentId == E2);
        Assert.Equal(EquipmentBorrowing.Domain.BorrowingStatus.Returned, borrowing.Status);
        Assert.NotNull(borrowing.ActualReturnDate);
        Assert.True(equipment.IsAvailable);
    }

    private HttpClient CreateClient()
    {
        var factory = new Lab6ApiFactory(_database.ConnectionString!);
        return factory.CreateClient();
    }
}
