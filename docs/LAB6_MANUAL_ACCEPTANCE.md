# Lab 6 Manual Acceptance Record

Complete these records with the actual date, observer, screenshots, and observed results. Screenshots from Lab 5 may be referenced as historical supporting evidence, but they must not be presented as Lab 6 execution.

## MA6-01 — Desktop responsiveness

| Field | Student entry |
|---|---|
| Date/time and observer |  |
| API process and URL |  |
| Database/fixture and `D` |  |
| Normal desktop loading observed |  |
| Delay mechanism and duration |  |
| UI remained responsive during delay |  |
| API stopped; desktop error observed |  |
| API restored; desktop recovered |  |
| Database before/after readback |  |
| Screenshot/evidence locations |  |
| Result: Passed / Failed / Blocked |  |

Required evidence: normal loading, delayed request, understandable unreachable-API message, recovery after API restart, and database readback showing no unintended writes.

## MA6-02 — Independent API and synchronization

| Field | Student entry |
|---|---|
| Date/time and observer |  |
| API process and URL |  |
| API request made while desktop was closed |  |
| HTTP response and persisted values |  |
| Desktop refresh/navigation action |  |
| Same change visible in desktop |  |
| OpenAPI URL and status |  |
| API/controller source inspection |  |
| Desktop `EquipmentBorrowingApiClient` evidence |  |
| No desktop EF Core/DbContext/database reference evidence |  |
| Screenshot/evidence locations |  |
| Result: Passed / Failed / Blocked |  |

Required evidence: independent API operation, OpenAPI availability, synchronized persisted data, and source inspection proving the desktop uses HTTP rather than a direct database connection.
