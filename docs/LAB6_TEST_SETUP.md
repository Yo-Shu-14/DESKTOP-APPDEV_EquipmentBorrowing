# Lab 6 Test Setup

This document records the reproducible starting point for Laboratory Activity 6.

## Baseline

- Baseline revision: `b360254` (`Lab 5 Final Submission`)
- Existing `lab5-submitted` tag: not present; the baseline is verified from the Git history.
- Lab 6 fixed local calendar date `D`: `2026-11-27` for the current controlled fixture.

## Test boundaries

Lab 6 tests must use the real ASP.NET Core pipeline for HTTP/routing assertions and PostgreSQL for persistence, constraints, and transaction claims. Unit-test fakes remain useful for isolated application rules and client transport simulation, but they are not evidence of PostgreSQL behavior.

## PostgreSQL test database

Use a disposable database that is separate from development data. Set the connection string only in the local process environment; do not commit credentials:

```powershell
$env:LAB6_TEST_CONNECTION = 'Host=localhost;Port=5432;Database=equipment_borrowing_lab6_test;Username=postgres;Password=<local-password>'
```

The fixture/reset implementation will apply migrations and load `docs/seed-test-data.sql` before each scenario or isolated subcase. A fresh `DbContext`/connection is required for persisted-state assertions.

## Running tests

The test project uses xUnit v3. With the installed .NET 10 SDK, the Microsoft Testing Platform runner must be enabled in the local SDK/test invocation; this repository does not commit a machine-specific runner override.

```powershell
dotnet build tests/EquipmentBorrowing.Test/EquipmentBorrowing.Test.csproj -m:1
& tests/EquipmentBorrowing.Test/bin/Debug/net10.0/EquipmentBorrowing.Test.exe -noLogo -noColor
```

The direct xUnit v3 runner is used here because the installed .NET 10 SDK rejects the legacy `dotnet test` VSTest target unless the machine-level Microsoft Testing Platform runner is opted in.

Until `LAB6_TEST_CONNECTION` is set, PostgreSQL-backed Lab 6 scenarios must be reported as `Blocked`/`Not Run`, never as passed. Existing unit tests may still run independently of PostgreSQL.

## Evidence convention

For each `TC6-*` case, record the actual test method, expected and observed result, execution status, defect/correction/retest information, and the relevant HTTP, database, UI, or source evidence in the Lab 6 execution record.
