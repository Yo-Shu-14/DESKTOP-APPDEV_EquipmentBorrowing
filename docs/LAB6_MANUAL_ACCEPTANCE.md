# Lab 6 Manual Acceptance Record

## MA6-01 — Desktop responsiveness

| Field | Student entry |
|---|---|
| Date/time and observer | 2026-11-28, student-observed |
| API process and URL | Recorded in the separate Lab 6 evidence document |
| Database/fixture and `D` | Lab 6 disposable PostgreSQL database; `D = 2026-11-27` |
| Normal desktop loading observed | Passed; screenshot provided in the separate Lab 6 evidence document |
| Delay mechanism and duration | Passed; details and screenshot provided in the separate Lab 6 evidence document |
| UI remained responsive during delay | Passed; screenshot provided in the separate Lab 6 evidence document |
| API stopped; desktop error observed | Passed; screenshot provided in the separate Lab 6 evidence document |
| API restored; desktop recovered | Passed; screenshot provided in the separate Lab 6 evidence document |
| Database before/after readback | Passed; readback screenshot provided in the separate Lab 6 evidence document |
| Screenshot/evidence locations | Screenshots are provided in a separate Lab 6 evidence document |
| Result: Passed / Failed / Blocked | Passed |

Required evidence: normal loading, delayed request, understandable unreachable-API message, recovery after API restart, and database readback showing no unintended writes.

## MA6-02 — Independent API and synchronization

| Field | Student entry |
|---|---|
| Date/time and observer | 2026-11-28, student-observed |
| API process and URL | Recorded in the separate Lab 6 evidence document |
| API request made while desktop was closed | Passed; screenshot provided in the separate Lab 6 evidence document |
| HTTP response and persisted values | Passed; screenshot provided in the separate Lab 6 evidence document |
| Desktop refresh/navigation action | Passed; screenshot provided in the separate Lab 6 evidence document |
| Same change visible in desktop | Passed; screenshot provided in the separate Lab 6 evidence document |
| OpenAPI URL and status | Passed; screenshot provided in the separate Lab 6 evidence document |
| API/controller source inspection | Passed; screenshot provided in the separate Lab 6 evidence document |
| Desktop `EquipmentBorrowingApiClient` evidence | Passed; screenshot provided in the separate Lab 6 evidence document |
| No desktop EF Core/DbContext/database reference evidence | Passed; screenshot provided in the separate Lab 6 evidence document |
| Screenshot/evidence locations | Screenshots are provided in a separate Lab 6 evidence document |
| Result: Passed / Failed / Blocked | Passed |

Required evidence: independent API operation, OpenAPI availability, synchronized persisted data, and source inspection proving the desktop uses HTTP rather than a direct database connection.
