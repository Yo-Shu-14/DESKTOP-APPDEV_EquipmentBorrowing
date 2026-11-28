# Lab 6 Test Execution Record

`Passed` means the test executed with enabled assertions and satisfied them. The recorded run below used the isolated disposable PostgreSQL database.

## Environment

- Baseline: `b360254` (`Lab 5 Final Submission`)
- Fixed fixture date `D`: `2026-11-27`
- PostgreSQL connection variable: `LAB6_TEST_CONNECTION`
- Test runner: `tests/EquipmentBorrowing.Test/bin/Debug/net10.0/EquipmentBorrowing.Test.exe`
- Latest result: `43 total, 0 errors, 0 failed, 0 skipped`

## Current execution status

| Case | Implementation | Status | Evidence / limitation |
|---|---|---|---|
| TC6-U01 | `Lab6HttpScenarioTests.Catalog_contains_the_controlled_fixture` | Passed | Isolated PostgreSQL run |
| TC6-U02 | `Lab6HttpScenarioTests.Available_filter_excludes_active_items` | Passed | Isolated PostgreSQL run |
| TC6-U03 | `Lab6HttpScenarioTests.Single_borrowing_returns_active_record_and_persists_item_state` | Passed | Isolated PostgreSQL run |
| TC6-U04 | `Lab6HttpScenarioTests.Active_borrowings_include_only_active_fixture_records` | Passed | Isolated PostgreSQL run |
| TC6-U05 | `Lab6HttpScenarioTests.Return_transitions_record_and_rejects_repeat_return` | Passed | Isolated PostgreSQL run |
| TC6-U06 | `Lab6HttpScenarioTests.Multi_item_checkout_creates_two_records_and_updates_both_items` | Passed | Isolated PostgreSQL run |
| TC6-U07 | `Lab6HttpScenarioTests.Student_history_returns_records_and_empty_existing_history` | Passed | Isolated PostgreSQL run |
| TC6-U08 | `Lab6HttpScenarioTests.Equipment_history_returns_related_records_and_empty_history` | Passed | Isolated PostgreSQL run |
| TC6-U09 | `Lab6HttpScenarioTests.Search_and_status_filter_return_expected_records` | Passed | Isolated PostgreSQL run |
| TC6-U12 | `Lab6HttpScenarioTests.Register_student_returns_created_eligible_student` | Passed | Isolated PostgreSQL run |
| TC6-U13 | `Lab6HttpScenarioTests.Update_student_preserves_identity_and_eligibility` | Passed | Isolated PostgreSQL run |
| TC6-U14 | `Lab6HttpScenarioTests.Eligibility_change_is_visible_and_same_value_is_noop` | Passed | Isolated PostgreSQL run |
| TC6-U15 | `Lab6HttpScenarioTests.Register_equipment_returns_available_item` | Passed | Isolated PostgreSQL run |
| TC6-U16 | `Lab6HttpScenarioTests.Update_equipment_preserves_identity_and_availability` | Passed | Isolated PostgreSQL run |
| TC6-E01 | `Lab6HttpScenarioTests.Invalid_and_duplicate_registration_requests_are_rejected` | Passed | Isolated PostgreSQL run |
| TC6-E02 | `Lab6HttpScenarioTests.Borrowing_rules_reject_ineligible_unavailable_and_past_due_requests` | Passed | Isolated PostgreSQL run |
| TC6-E03 | `Lab6HttpScenarioTests.Invalid_route_and_malformed_json_are_rejected` | Passed | Isolated PostgreSQL run |
| TC6-E04 | `Lab6HttpScenarioTests.Failed_multi_item_checkout_leaves_both_items_unchanged` | Passed | Atomic conflict rollback verified; post-write fault seam remains a limitation |
| TC6-E05 | `Lab6HttpScenarioTests.Repeated_stale_borrow_request_returns_conflict` | Passed | Isolated PostgreSQL run |
| TC6-E06 | `Lab6HttpScenarioTests.Optional_fields_clear_and_protected_state_is_preserved` | Passed | Isolated PostgreSQL run |
| TC6-E07a | `Lab6TransportScenarioTests.Client_reports_connection_failure_without_an_http_response` | Passed | Local xUnit runner |
| TC6-E07b | `Lab6TransportScenarioTests.Client_distinguishes_received_server_error_from_transport_failure` | Passed | Local xUnit runner |
| TC6-E07c | `Lab6TransportScenarioTests.Client_reports_unknown_outcome_without_retry_after_commit` | Passed | Isolated PostgreSQL run |
| TC6-E08 | `Lab6HttpScenarioTests.Borrowing_and_return_survive_fresh_contexts` | Passed | Fresh-context persistence verified; actual process restart remains manual evidence |
| API contract | `Lab6OpenApiTests.OpenApi_documents_required_routes_and_methods` | Passed | Local xUnit runner; uses unreachable placeholder DB because OpenAPI generation is database-independent |
| MA6-01 | Supervised desktop check | Not independently verified here | Attach the user's screenshots and observation record |
| MA6-02 | Supervised API/desktop synchronization check | Not independently verified here | Attach the user's screenshots and source-inspection record |
