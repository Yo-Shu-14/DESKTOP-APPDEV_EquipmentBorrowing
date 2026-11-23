# Lab 5 (ITSD81) — Cross-Laboratory Traceability Matrix

> Section 5 Traceability Matrix mapping Lab 4 Use Cases to Lab 5 API Endpoints, Source Code References, and Acceptance Tests.

| Lab 4 Use Case | Name | Applicable Rules | Lab 5 Endpoint | Acceptance Tests | Implementation Reference | Evidence / Notes |
|---|---|---|---|---|---|---|
| **UC-01** | View Equipment Catalog | BR-05–07, 10, 18–19 | **API-01** `GET /api/v1/equipment` | T01, T07 | `EquipmentController.GetAll` / `ViewEquipmentCatalogService` | Independent API read returns full array; Desktop EquipmentView refreshes |
| **UC-02** | Check Available Equipment | BR-10, 18–19 | **API-01** `GET /api/v1/equipment?available=true` | T01, T07 | `EquipmentController.GetAll` / `CheckAvailableEquipmentService` | Query `available=true` filters unavailable items |
| **UC-03** | Borrow Equipment (Single) | BR-08–11, 14–16, 18–19 | **API-10** `POST /api/v1/borrowings`<br>**API-13** `GET /api/v1/borrowings/{id}` | T04, T07 | `BorrowingsController.Create` / `BorrowEquipmentService.BorrowEquipmentAsync` | Revalidates future date, student eligibility, availability, and max 3 limit; returns 201 |
| **UC-04** | View Active Borrowings | BR-14, 18–19 | **API-14** `GET /api/v1/borrowings?status=Active` | T01, T06, T07 | `BorrowingsController.Search` / `SearchBorrowingsService` | Filters by `status=Active`; displays borrower and due dates |
| **UC-05** | Return Equipment | BR-10, 13–16, 18–19 | **API-12** `POST /api/v1/borrowings/{id}/return`<br>**API-13** `GET /api/v1/borrowings/{id}` | T04, T07 | `BorrowingsController.ReturnBorrowing` / `ReturnEquipmentService.ReturnEquipmentAsync` | Sets `ActualReturnDate`, marks status Returned, and sets equipment `IsAvailable = true` |
| **UC-06** | Multi-Item Checkout | BR-08–12, 14–16, 18–19 | **API-11** `POST /api/v1/checkout-requests` | T05, T07 | `BorrowingsController.CheckoutMultiple` / `BorrowEquipmentService.BorrowMultipleEquipmentAsync` | Atomic transaction across min 2 items; rolls back on conflict or limit overflow |
| **UC-07** | View Student Borrowing History | BR-14, 18–19 | **API-02** `GET /api/v1/students`<br>**API-03** `GET /api/v1/students/{id}`<br>**API-15** `GET /api/v1/students/{id}/borrowings` | T01, T06, T07 | `StudentsController.GetBorrowings` / `ViewStudentBorrowingHistoryService` | Returns all active and returned loans for a student |
| **UC-08** | View Equipment Borrowing History | BR-14, 18–19 | **API-01** `GET /api/v1/equipment`<br>**API-07** `GET /api/v1/equipment/{id}`<br>**API-16** `GET /api/v1/equipment/{id}/borrowings` | T01, T06, T07 | `EquipmentController.GetBorrowings` / `ViewEquipmentBorrowingHistoryService` | Returns all past and current borrowers of an equipment item |
| **UC-09** | Search / Filter Borrowings | BR-17–19 | **API-14** `GET /api/v1/borrowings` | T06, T07 | `BorrowingsController.Search` / `SearchBorrowingsService` | Supports `search`, `status` (Active/Returned/All), `sort`, and `direction` |
| **UC-12** | Register Student | BR-01–02, 16, 18–19 | **API-04** `POST /api/v1/students` | T02, T07 | `StudentsController.Create` / `RegisterStudentService` | Validates required fields, checks duplicate number (409 Conflict), defaults to eligible |
| **UC-13** | Update Student Information | BR-01–03, 14, 16, 18–19 | **API-02** `GET /api/v1/students`<br>**API-03** `GET /api/v1/students/{id}`<br>**API-05** `PUT /api/v1/students/{id}` | T02, T07 | `StudentsController.Update` / `UpdateStudentInformationService` | Preserves ID and eligibility; checks duplicate student number (409 Conflict) |
| **UC-14** | Change Student Eligibility | BR-04, 08, 13–14, 18–19 | **API-02** `GET /api/v1/students`<br>**API-03** `GET /api/v1/students/{id}`<br>**API-06** `PATCH /api/v1/students/{id}/eligibility` | T02, T07 | `StudentsController.ChangeEligibility` / `ChangeStudentEligibilityService` | Same state is 200 OK no-op; restricted students cannot borrow new items |
| **UC-15** | Register Equipment | BR-05–06, 16, 18–19 | **API-08** `POST /api/v1/equipment` | T03, T07 | `EquipmentController.Create` / `RegisterEquipmentService` | Validates code/name, checks duplicate identifier (409 Conflict), defaults to available |
| **UC-16** | Update Equipment Information | BR-05–07, 14, 18–19 | **API-07** `GET /api/v1/equipment/{id}`<br>**API-09** `PUT /api/v1/equipment/{id}` | T03, T07 | `EquipmentController.Update` / `UpdateEquipmentInformationService` | Preserves ID and availability; checks duplicate identifier (409 Conflict) |

---

## Acceptance Test Summary (T01–T08)

- **T01 (Independent API Reads):** Tested API read endpoints (`API-01, 02, 03, 07, 13, 14, 15, 16`) without Avalonia running.
- **T02 (Student Workflows):** Tested `API-04` (create), `API-05` (update), `API-06` (eligibility patch with no-op handling).
- **T03 (Equipment Workflows):** Tested `API-08` (create) and `API-09` (update) with duplicate 409 protection.
- **T04 (Single Borrow & Return):** Tested `API-10` checkout (201 Created), `API-12` return (200 OK), and duplicate return rejection (409 Conflict).
- **T05 (Multi-Item Checkout & Rollback):** Tested `API-11` checkout with limit boundaries, duplicate checks, and transaction rollback.
- **T06 (Search, Filter, Sort & History):** Tested `API-14, 15, 16` with partial query matching, status filters, and date sorting.
- **T07 (Desktop HTTP Integration):** Verified Avalonia desktop communicates exclusively via `EquipmentBorrowingApiClient` with zero database dependencies.
- **T08 (Error & Fault Diagnostics):** Verified desktop displays connection failure when API is down, and safe error message on 500 error.
