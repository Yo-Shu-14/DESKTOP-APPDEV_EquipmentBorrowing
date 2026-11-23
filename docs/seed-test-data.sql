-- ============================================================================
-- Lab 5 (ITSD81) — Controlled Seed Dataset (S1-S4, E1-E7)
-- Resets and populates the disposable database for testing T01 to T08.
-- ============================================================================

-- Clean existing data
DELETE FROM "Borrowings";
DELETE FROM "Equipment";
DELETE FROM "Students";

-- Reset identity sequence for Students if applicable
ALTER SEQUENCE IF EXISTS "Students_Id_seq" RESTART WITH 1;

-- ─── 1. Students (S1–S4) ─────────────────────────────────────────────────────
-- S1: ST-100, Ada Cruz, eligible
-- S2: ST-200, Ben Diaz, ineligible
-- S3: ST-300, Cara Lim, eligible
-- S4: ST-400, Dani Ong, eligible
INSERT INTO "Students" ("Id", "InstitutionalNumber", "Name", "ContactInformation", "IsAllowedToBorrow")
VALUES
    (1, 'ST-100', 'Ada Cruz', 'ada.cruz@campus.edu', TRUE),
    (2, 'ST-200', 'Ben Diaz', 'ben.diaz@campus.edu', FALSE),
    (3, 'ST-300', 'Cara Lim', 'cara.lim@campus.edu', TRUE),
    (4, 'ST-400', 'Dani Ong', 'dani.ong@campus.edu', TRUE);

-- Update sequence to next available ID
SELECT setval(pg_get_serial_sequence('"Students"', 'Id'), COALESCE(MAX("Id"), 1)) FROM "Students";

-- ─── 2. Equipment (E1–E7) ────────────────────────────────────────────────────
-- E1: EQ-001, Camera, unavailable (active with S1)
-- E2: EQ-002, Laptop, available
-- E3: EQ-003, Projector, available (returned by S1)
-- E4: EQ-004, Tablet, unavailable (active with S3)
-- E5: EQ-005, Microphone, unavailable (active with S3)
-- E6: EQ-006, Tripod, available
-- E7: EQ-007, Speaker, available
INSERT INTO "Equipment" ("EquipmentId", "EquipmentIdentifier", "Name", "Description", "IsAvailable")
VALUES
    ('11111111-1111-1111-1111-111111111111', 'EQ-001', 'Camera', 'DSLR Camera Kit', FALSE),
    ('22222222-2222-2222-2222-222222222222', 'EQ-002', 'Laptop', 'Dell Core i7 Laptop', TRUE),
    ('33333333-3333-3333-3333-333333333333', 'EQ-003', 'Projector', 'HDMI Digital Projector', TRUE),
    ('44444444-4444-4444-4444-444444444444', 'EQ-004', 'Tablet', 'Drawing Graphics Tablet', FALSE),
    ('55555555-5555-5555-5555-555555555555', 'EQ-005', 'Microphone', 'Wireless Microphone Set', FALSE),
    ('66666666-6666-6666-6666-666666666666', 'EQ-006', 'Tripod', 'Heavy Duty Camera Tripod', TRUE),
    ('77777777-7777-7777-7777-777777777777', 'EQ-007', 'Speaker', 'Portable Bluetooth Speaker', TRUE);

-- ─── 3. Borrowing Records ───────────────────────────────────────────────────
-- S1 borrows E1: Active (borrowed 2026-11-01, due 2026-11-09)
-- S1 borrows E3: Returned (borrowed 2026-11-02, due 2026-11-12, returned 2026-11-08)
-- S3 borrows E4: Active (borrowed 2026-11-03, due 2026-11-12)
-- S3 borrows E5: Active (borrowed 2026-11-03, due 2026-11-12)
INSERT INTO "Borrowings" ("BorrowingId", "StudentId", "EquipmentId", "DateBorrowed", "ExpectedReturnDate", "ActualReturnDate", "Status")
VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 1, '11111111-1111-1111-1111-111111111111', '2026-11-01T08:00:00Z', '2026-11-09T17:00:00Z', NULL, 'Active'),
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 1, '33333333-3333-3333-3333-333333333333', '2026-11-02T08:00:00Z', '2026-11-12T17:00:00Z', '2026-11-08T14:30:00Z', 'Returned'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 3, '44444444-4444-4444-4444-444444444444', '2026-11-03T09:00:00Z', '2026-11-12T17:00:00Z', NULL, 'Active'),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 3, '55555555-5555-5555-5555-555555555555', '2026-11-03T09:15:00Z', '2026-11-12T17:00:00Z', NULL, 'Active');
