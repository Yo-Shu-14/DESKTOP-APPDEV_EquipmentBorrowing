-- 1. Basic Retrieval
-- Retrieve all equipment
SELECT *
FROM Equipment;


-- 2. Filtering
-- Retrieve only currently available equipment
SELECT *
FROM Equipment
WHERE IsAvailable = TRUE;


-- 3. Join
-- Retrieve active borrowings with student and equipment information
SELECT
    s.Name AS Student,
    e.Name AS Equipment,
    b.DateBorrowed AS Borrowed,
    b.ExpectedReturnDate AS Due
FROM Borrowings b
JOIN Students s ON b.StudentId = s.Id
JOIN Equipment e ON b.EquipmentId = e.EquipmentId
WHERE b.Status = 'Active';


-- 4. Aggregate
-- Count the number of active borrowings
SELECT COUNT(*) AS ActiveBorrowings
FROM Borrowings
WHERE Status = 'Active';


-- 5. Update
-- Change the availability of an equipment record
UPDATE Equipment
SET IsAvailable = FALSE
WHERE EquipmentId = "EA1711C4-AB35-499D-A4EB-3C277866BE52";