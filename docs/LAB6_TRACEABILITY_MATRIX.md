# Lab 6 Traceability Matrix

The method names below are the actual Lab 6 implementations. Execution status is maintained in `LAB6_EXECUTION_RECORD.md`; it must be updated only after the tests run against the isolated PostgreSQL database.

| UC | BR | API | TC6 | Actual test implementation |
|---|---|---|---|---|
| UC-01 | BR-05–07,10,19 | API-01 | U01 | `Catalog_contains_the_controlled_fixture` |
| UC-02 | BR-10,19 | API-01 | U02 | `Available_filter_excludes_active_items` |
| UC-03 | BR-08–11,14–16,19 | API-10,13 | U03,E02,E05,E08 | `Single_borrowing_returns_active_record_and_persists_item_state`; related edge tests |
| UC-04 | BR-14,19 | API-14 | U04,U09 | `Active_borrowings_include_only_active_fixture_records`; `Search_and_status_filter_return_expected_records` |
| UC-05 | BR-10,13–14,19 | API-12,13 | U05,E08 | `Return_transitions_record_and_rejects_repeat_return` |
| UC-06 | BR-08–12,14–16,19 | API-11 | U06,E04,E05,E07 | `Multi_item_checkout_creates_two_records_and_updates_both_items`; edge tests |
| UC-07 | BR-14,19 | API-03,15 | U07 | `Student_history_returns_records_and_empty_existing_history` |
| UC-08 | BR-14,19 | API-07,16 | U08,U16 | `Equipment_history_returns_related_records_and_empty_history` |
| UC-09 | BR-17,19 | API-14 | U09 | `Search_and_status_filter_return_expected_records` |
| UC-12 | BR-01–02,16,19 | API-03,04 | U12,E01,E08 | `Register_student_returns_created_eligible_student` |
| UC-13 | BR-01–03,14,16,19 | API-02,03,05,15 | U13,E01,E06 | `Update_student_preserves_identity_and_eligibility` |
| UC-14 | BR-04,08,13–14,19 | API-06,10,12 | U14 | `Eligibility_change_is_visible_and_same_value_is_noop` |
| UC-15 | BR-05–06,10,16,19 | API-08 | U15,E01,E08 | `Register_equipment_returns_available_item` |
| UC-16 | BR-05–07,10,14,16,19 | API-07,09,16 | U16,E01,E06 | `Update_equipment_preserves_identity_and_availability` |

## API coverage check

The HTTP scenario suite exercises the API route families for equipment, students, borrowings, history, return, and multi-item checkout. The OpenAPI response and the controller attributes must be captured separately as MA6-02 evidence.
