# Campus Equipment Borrowing System

## 1. Project Overview

The **Campus Equipment Borrowing System** is a desktop application developed to help a campus custodian manage students, equipment records, and equipment borrowing transactions.

The system allows the custodian to view and manage the equipment catalog, check equipment availability, process individual and multiple-item borrowings, record returns, review borrowing histories, and manage student and equipment information.

The application is developed using **C# and .NET**, with **Avalonia UI** for the desktop interface, **CommunityToolkit.Mvvm** for the MVVM pattern, **Entity Framework Core** for data access, and **PostgreSQL** as the relational database using the Npgsql provider.

### Primary Actor

**Custodian**

The custodian is the primary actor who operates the system. The custodian is responsible for managing student and equipment records and processing borrowing and return transactions.

### Main System Responsibilities

- View the equipment catalog.
- Find equipment that is currently available.
- Process equipment borrowing transactions.
- View active borrowing records.
- Process equipment returns.
- Check out multiple equipment items in one transaction.
- View student borrowing history.
- View equipment borrowing history.
- Search, filter, and sort borrowing records.
- Register and update student information.
- Change a student's borrowing eligibility.
- Register and update equipment information.
- Preserve borrowing records for historical reference.

---

## 2. Prerequisites

Before building and running the application, make sure the following software is installed:

- **.NET SDK** compatible with the solution's target framework.
- **PostgreSQL** database server.
- **Git** for source control.
- An IDE or code editor that supports C# and .NET development, such as Visual Studio, Visual Studio Code, or JetBrains Rider.
- Entity Framework Core command-line tools for managing migrations.

Verify that .NET is installed:

```bash
dotnet --version
```

Verify that Git is installed:

```bash
git --version
```

Make sure the PostgreSQL server is running before starting the application.

---

## 3. Solution Structure

The solution is organized into separate projects to keep the user interface, application operations, domain models, and database implementation independent.

### 3.1 Domain

The `EquipmentBorrowing.Domain` project contains the core concepts and business rules of the system.

It contains the main domain entities and related types, including:

- `Student` – represents a student who may borrow campus equipment.
- `Equipment` – represents an equipment item that can be borrowed.
- `Borrowing` – represents a borrowing transaction between a student and an equipment item.
- `BorrowingStatus` – represents the current status of a borrowing record, such as Active or Returned.

The Domain project does not depend on the user interface, database provider, or Infrastructure project.

### 3.2 Application

The `EquipmentBorrowing.Application` project contains the application's use cases and business operations.

It is responsible for coordinating operations and enforcing the applicable borrowing rules.

Examples of application operations include:

- Viewing the equipment catalog.
- Checking equipment availability.
- Borrowing equipment.
- Returning equipment.
- Checking out multiple equipment items.
- Viewing borrowing histories.
- Searching, filtering, and sorting borrowing records.
- Registering and updating students.
- Changing student borrowing eligibility.
- Registering and updating equipment.

The Application project also contains repository interfaces, such as:

- `IStudentRepository`
- `IEquipmentRepository`
- `IBorrowingRepository`

These interfaces define the data-access contracts required by the application without depending on a specific database implementation.

### 3.3 Infrastructure

The `EquipmentBorrowing.Infrastructure` project contains the technical implementations of the repository abstractions and database access.

It contains:

- `EquipmentBorrowingDbContext` – the Entity Framework Core DbContext used to access PostgreSQL.
- `EfStudentRepository` – the EF Core implementation of `IStudentRepository`.
- `EfEquipmentRepository` – the EF Core implementation of `IEquipmentRepository`.
- `EfBorrowingRepository` – the EF Core implementation of `IBorrowingRepository`.
- Entity configurations for `Student`, `Equipment`, and `Borrowing`.
- Database initialization and EF Core migration support.

The Infrastructure project uses **PostgreSQL** as the persistent database and **Npgsql** as the Entity Framework Core database provider.

### 3.4 Desktop

The `EquipmentBorrowing.Desktop` project is the executable Avalonia desktop application.

It contains:

- Avalonia Views that display the application's interface.
- ViewModels that manage presentation state and user interactions.
- Commands that respond to user actions.
- Dependency injection configuration.
- Application startup and navigation-related components.

The Desktop project communicates with the Application layer to perform system operations.

### 3.5 Tests

The `EquipmentBorrowing.Tests` project contains automated tests for the system.

Tests should verify important domain and application behavior, including:

- Student eligibility validation.
- Equipment availability validation.
- Borrowing limits.
- Borrowing and return operations.
- Multi-item checkout behavior.
- Search, filtering, and sorting behavior.
- Student and equipment management operations.
- Transaction success and failure behavior, where covered by the test setup.

---

## 4. System Architecture and Dependency Direction

The system follows a layered architecture that separates business rules from the user interface and database implementation.

### 4.1 Architecture Diagram

```text
EquipmentBorrowing.Desktop
        |
        +----------> EquipmentBorrowing.Application
        |                       |
        |                       v
        |                EquipmentBorrowing.Domain
        |
        +----------> EquipmentBorrowing.Infrastructure
                                |
                                +----------> Application
                                |
                                +----------> Domain
```

### 4.2 Dependency Responsibilities

- The `EquipmentBorrowing.Domain` project contains the core entities and business concepts. It does not depend on the other projects.
- The `EquipmentBorrowing.Application` project depends on the Domain project and defines the application's operations and repository interfaces.
- The `EquipmentBorrowing.Infrastructure` project depends on the Application and Domain projects. It implements repository interfaces and handles database access through Entity Framework Core and PostgreSQL.
- The `EquipmentBorrowing.Desktop` project depends on the Application and Infrastructure projects. It provides the graphical interface and configures the required dependencies.

This structure keeps the core business logic independent of the database provider and desktop UI technology.

### 4.3 Dependency Direction Principles

The application services depend on repository interfaces rather than directly accessing database implementations.

This allows the database implementation to change without requiring the application use cases to be rewritten.

The Views and ViewModels also avoid directly executing database queries. Instead, they delegate operations to the Application layer.

---

## 5. Avalonia UI and MVVM

The desktop application uses **Avalonia UI** and the **Model-View-ViewModel (MVVM)** pattern.

### 5.1 Desktop Project Responsibility

The `EquipmentBorrowing.Desktop` project provides the graphical interface used by the custodian.

It contains the Views and ViewModels that allow the custodian to:

- Navigate through the system.
- View equipment and student information.
- Select equipment for borrowing.
- Process returns.
- Review active borrowings and borrowing histories.
- Search, filter, and sort records.
- Register and update records.

### 5.2 MVVM Architecture

```text
Avalonia View
      |
      | Data Binding / Command
      |
   ViewModel
      |
      | Calls Application Operation
      |
      v
Application Service
      |
      v
Domain Models
      |
      ^
      |
Infrastructure
      |
      v
PostgreSQL Database
```

### 5.3 View Responsibilities

The View is responsible for displaying information and collecting user input.

Examples include:

- Displaying the equipment catalog.
- Showing student and equipment details.
- Providing borrowing and return controls.
- Displaying borrowing history.
- Providing search, filter, and sort controls.
- Displaying validation messages and operation feedback.

A View should not directly execute database queries or contain the application's main business rules.

### 5.4 ViewModel Responsibilities

The ViewModel manages the presentation state and responds to user interactions.

It is responsible for:

- Exposing properties for data binding.
- Providing commands for user actions.
- Collecting and validating presentation-level input.
- Calling the appropriate Application operation.
- Updating displayed data after an operation.
- Displaying success and error feedback.

Business rules that must apply regardless of the interface belong in the Application or Domain layers, not exclusively in a ViewModel.

### 5.5 Example: Borrow Equipment Flow

1. The custodian selects a student.
2. The custodian selects an available equipment item.
3. The custodian provides the expected return date.
4. The custodian presses the Borrow command.
5. The ViewModel calls the borrowing operation in the Application layer.
6. The Application layer validates the student, equipment availability, borrowing eligibility, and borrowing limit.
7. If validation succeeds, the borrowing record is created.
8. The ViewModel refreshes the displayed information and presents the operation result.

The exact service and method names depend on the implementation in the source code.

### 5.6 Example: Return Equipment Flow

1. The custodian opens the active borrowing records.
2. The custodian selects the borrowing to be returned.
3. The custodian activates the Return command.
4. The ViewModel calls the return operation in the Application layer.
5. The Application layer checks that the borrowing exists and is still active.
6. The borrowing status is updated to Returned.
7. The ViewModel refreshes the active borrowing list and displays the result.

---

## 6. Requirements and Use Case Analysis

The system is designed around the custodian's responsibilities in managing equipment and borrowing transactions.

### 6.1 Primary Actor

**Custodian**

The custodian interacts with the system to manage the equipment catalog, student records, borrowing eligibility, borrowing transactions, returns, and borrowing histories.

### 6.2 Required Use Cases

The following use cases describe the main functions of the Campus Equipment Borrowing System.

| Use Case ID | Use Case Name | Description |
|---|---|---|
| UC-01 | View Equipment Catalog | Displays the equipment records maintained by the system. |
| UC-02 | Find Available Equipment | Displays equipment that is currently available for borrowing. |
| UC-03 | Borrow Equipment | Processes a borrowing transaction for an equipment item. |
| UC-04 | View Active Borrowings | Displays borrowing records that are currently active. |
| UC-05 | Return Equipment | Records the return of borrowed equipment. |
| UC-06 | Check Out Multiple Equipment Items | Processes a borrowing transaction involving multiple equipment items. |
| UC-07 | View Student Borrowing History | Displays borrowing records associated with a selected student. |
| UC-08 | View Equipment Borrowing History | Displays borrowing records associated with a selected equipment item. |
| UC-09 | Search, Filter, and Sort Borrowings | Allows borrowing records to be searched, filtered, and sorted. |
| UC-12 | Register Student | Adds a new student record to the system. |
| UC-13 | Update Student Information | Updates the information stored for an existing student. |
| UC-14 | Change Student Borrowing Eligibility | Changes whether a student is allowed to borrow equipment. |
| UC-15 | Register Equipment | Adds a new equipment record to the system. |
| UC-16 | Update Equipment Information | Updates the information stored for an existing equipment item. |

**Note:** The use-case IDs above follow the required Lab Activity 4 numbering. The complete use-case descriptions, preconditions, main flows, alternative flows, and postconditions are documented separately in the Use-Case Specifications.

### 6.3 Important Business Rules

The system must enforce the applicable rules when processing borrowing operations.

These include:

- A student must exist in the system before borrowing equipment.
- A student must be eligible to borrow equipment.
- A student must not exceed the maximum permitted number of active borrowings.
- Equipment must exist in the system before it can be borrowed.
- Equipment must be available before a borrowing transaction can be completed.
- A borrowing record must be maintained for each successful borrowing.
- Returned equipment must no longer be treated as actively borrowed.
- A borrowing record must not be returned more than once.
- Multi-item checkout must handle the selected items as one transaction when required by the use-case rules.
- Borrowing history must remain available for reference after an item is returned.

The actual validation rules and limits should match the values and conditions implemented in the application and specified in the activity requirements.

---

## 7. Domain Concepts

The Domain project contains the main concepts used by the Campus Equipment Borrowing System.

### 7.1 Student

The `Student` entity represents a student who may borrow equipment.

**Information:**

- Student ID
- Student name
- Institutional student number, if included in the implementation
- Borrowing eligibility

**Rules or State:**

- A student must be registered in the system before borrowing.
- A student must be eligible to borrow equipment.
- A student must not exceed the permitted active borrowing limit.

**Not the responsibility of Student:**

- Storing equipment records.
- Creating repository connections.
- Managing database connections.
- Coordinating the complete borrowing operation.

### 7.2 Equipment

The `Equipment` entity represents an item that can be borrowed.

**Information:**

- Equipment ID
- Equipment name
- Equipment description
- Availability information

**Rules or State:**

- Equipment must be registered before it can be borrowed.
- Equipment may be available or unavailable.
- Equipment becomes unavailable when successfully borrowed.
- Equipment becomes available again after a successful return.

**Not the responsibility of Equipment:**

- Managing student records.
- Creating borrowing records.
- Accessing repositories or databases.
- Coordinating the complete borrowing operation.

### 7.3 Borrowing

The `Borrowing` entity represents a borrowing transaction involving a student and an equipment item.

**Information:**

- Borrowing ID
- Student
- Equipment
- Date borrowed
- Expected return date
- Current borrowing status

**Rules or State:**

- A borrowing record belongs to a student and an equipment item.
- A borrowing record has a status, such as Active or Returned.
- An active borrowing represents equipment that has not yet been returned.
- A returned borrowing remains part of the borrowing history.

**Not the responsibility of Borrowing:**

- Retrieving students or equipment from repositories.
- Managing database connections.
- Coordinating the entire application use case.

---

## 8. Relational Database Design

The system uses **PostgreSQL** as its relational database and **Entity Framework Core** as its Object-Relational Mapper (ORM).

The database stores student information, equipment records, and borrowing transactions so that information remains available after the application is closed.

### 8.1 Students Table

The `Students` table stores information about registered students.

| Column | Description |
|---|---|
| `Id` | Primary key that uniquely identifies a student. |
| `Name` | Student's name. |
| `IsAllowedToBorrow` | Indicates whether the student is allowed to borrow equipment. |
| `StudentNumber` | Institutional student number, if included in the implementation. |

The actual column names and data types must match the EF Core entity configuration and migrations.

### 8.2 Equipment Table

The `Equipment` table stores information about equipment maintained by the system.

| Column | Description |
|---|---|
| `EquipmentId` | Primary key that uniquely identifies an equipment item. |
| `Name` | Name of the equipment. |
| `Description` | Description of the equipment. |
| `IsAvailable` | Indicates whether the equipment is currently available. |

### 8.3 Borrowings Table

The `Borrowings` table stores borrowing transaction records.

| Column | Description |
|---|---|
| `BorrowingId` | Primary key that uniquely identifies a borrowing record. |
| `StudentId` | Foreign key referencing the student who borrowed the equipment. |
| `EquipmentId` | Foreign key referencing the borrowed equipment. |
| `DateBorrowed` | Date and time when the equipment was borrowed. |
| `ExpectedReturnDate` | Expected return date of the equipment. |
| `Status` | Current borrowing status, such as Active or Returned. |

The actual schema should be verified against the migrations in the project.

### 8.4 Relationships

The database contains the following relationships:

- One student can have many borrowing records over time.
- One equipment item can appear in many borrowing records over time.
- Each borrowing record belongs to one student.
- Each borrowing record refers to one equipment item.

The `StudentId` and `EquipmentId` columns in the `Borrowings` table serve as foreign keys.

### 8.5 Database Constraints and Indexes

Database constraints help protect the consistency of stored information.

The database design should enforce the appropriate:

- Primary key constraints.
- Foreign key constraints.
- Required-field constraints.
- Unique constraints for fields that must not be duplicated.
- Appropriate indexes for frequently queried fields.

For borrowing records, the database should also prevent conflicting active borrowing records for the same equipment item when required by the activity's business rules.

The actual constraints and indexes must be confirmed in the EF Core configurations and PostgreSQL migrations.

### 8.6 Database Diagram

The database relationship diagram is provided in:

```text
docs/database-diagram.png
```

The diagram should show the database tables, primary keys, foreign keys, and relationships.

Update the file path if the diagram is stored elsewhere in the final project.

---

## 9. PostgreSQL and Entity Framework Core Configuration

The system uses PostgreSQL for persistent data storage and Entity Framework Core for database access.

The Npgsql Entity Framework Core provider allows EF Core to communicate with PostgreSQL.

### 9.1 Required Packages

The Infrastructure project should include the packages required for EF Core and PostgreSQL.

Common packages include:

- `Microsoft.EntityFrameworkCore` – provides the main EF Core functionality.
- `Microsoft.EntityFrameworkCore.Design` – provides design-time support for migrations.
- `Npgsql.EntityFrameworkCore.PostgreSQL` – provides PostgreSQL database support for EF Core.

The package versions must be compatible with the target .NET and EF Core versions used by the solution.

### 9.2 PostgreSQL Database Connection

The application needs a PostgreSQL connection string to connect to the database.

A typical connection string has the following format:

```text
Host=localhost;Port=5432;Database=equipment_borrowing;Username=postgres;Password=YOUR_PASSWORD
```

Replace the database name, username, and password with the values configured on the local PostgreSQL server.

**Security note:** Do not commit real database passwords or other sensitive credentials to the Git repository. Use environment variables or an appropriate local configuration file excluded from source control.

### 9.3 DbContext

The `EquipmentBorrowingDbContext` is responsible for managing the application's database access through Entity Framework Core.

It defines the database sets for the main entities:

- `Students`
- `Equipment`
- `Borrowings`

Example:

```csharp
public DbSet<Student> Students { get; set; }
public DbSet<Equipment> Equipment { get; set; }
public DbSet<Borrowing> Borrowings { get; set; }
```

The DbContext also applies entity configurations from the Infrastructure assembly.

Example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(EquipmentBorrowingDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
}
```

This keeps database-specific configuration separate from the domain classes.

### 9.4 Entity Configurations

Entity configurations define how domain entities are mapped to database tables and columns.

#### StudentConfiguration

The student configuration defines the primary key, required fields, field lengths, and other applicable constraints for student records.

#### EquipmentConfiguration

The equipment configuration defines the primary key, required fields, field lengths, availability information, and other applicable constraints for equipment records.

#### BorrowingConfiguration

The borrowing configuration defines:

- The borrowing primary key.
- The student foreign key.
- The equipment foreign key.
- Required borrowing dates.
- Borrowing status storage.
- Relationship behavior.
- Relevant indexes and constraints, including a partial unique index on `EquipmentId` where `Status = 'Active'` to enforce that an equipment item cannot have more than one active borrowing simultaneously (BR-10, BR-16).

The exact configuration must match the application's current entity models and database requirements.

### 9.5 Database Provider Registration

The PostgreSQL provider is registered through the application's dependency injection configuration.

A typical EF Core registration looks like this:

```csharp
services.AddDbContext<EquipmentBorrowingDbContext>(options =>
    options.UseNpgsql(connectionString));
```

The actual implementation should obtain the connection string from the application's configuration rather than hardcoding credentials in the source code.

---

## 10. Database Migrations and Initialization

Entity Framework Core migrations are used to create and update the PostgreSQL database schema.

Migrations allow the database structure to be reproduced from the project's entity configurations.

### 10.1 Creating a Migration

After changing an entity or its database configuration, create a migration using the EF Core tools.

A typical command is:

```bash
dotnet ef migrations add MigrationName
```

If the solution uses separate Infrastructure and Desktop projects, specify the correct project and startup project:

```bash
dotnet ef migrations add MigrationName \
  --project EquipmentBorrowing.Infrastructure \
  --startup-project EquipmentBorrowing.Desktop
```

Adjust the project paths and names to match the actual solution.

### 10.2 Applying Migrations

To apply pending migrations to the configured PostgreSQL database:

```bash
dotnet ef database update
```

If separate projects are used:

```bash
dotnet ef database update \
  --project EquipmentBorrowing.Infrastructure \
  --startup-project EquipmentBorrowing.Desktop
```

The application may also apply pending migrations during startup through its database initialization process, if that behavior is implemented.

Example:

```csharp
await db.Database.MigrateAsync();
```

### 10.3 Solution Migrations

The project includes the following migrations:

1. `20261112084307_InitialPostgreSQL` - Initial PostgreSQL schema for students, equipment, and borrowings.
2. `20261112111647_AddActualReturnDate` - Adds nullable actual return date column.
3. `20261113022707_AddStudentRegistrationFields` - Adds institutional student number (unique) and contact information.
4. `20261113034359_AddEquipmentRegistrationFields` - Adds unique equipment identifier.
5. `20261120094403_AddSingleActiveBorrowingConstraint` - Adds unique filtered index preventing multiple active borrowings for the same equipment item (BR-10, BR-16).

### 10.4 Database Initialization

The database initializer is responsible for preparing the database when the application starts, if initialization is configured in the project.

Its responsibilities may include:

1. Applying pending migrations.
2. Checking whether initial records already exist.
3. Adding seed data when necessary.
4. Avoiding duplicate seed data during subsequent application launches.

Seed data should be inserted only when the database needs it.

### 10.4 Seed Data

The initial database may contain sample records used to demonstrate the system's functions.

Examples include:

- Registered student records.
- Equipment records.
- Available equipment items.
- Other records needed to demonstrate borrowing and return operations.

Document the actual seed data included in the implementation.

### 10.5 Persistence

PostgreSQL stores the system's records independently of the desktop application's running process.

The system should preserve:

- Student records.
- Equipment records.
- Borrowing records.
- Updated borrowing statuses.
- Borrowing history.

For example, a successfully borrowed item should remain unavailable after the application is closed and opened again, provided the database state has not been changed by another operation.

---

## 11. Installation and Setup Instructions

Follow these steps to prepare the application on a local development machine.

### 11.1 Obtain the Source Code

Clone the project repository:

```bash
git clone REPOSITORY_URL
```

Alternatively, extract the submitted project archive.

Open the solution file in the preferred .NET IDE.

### 11.2 Restore Dependencies

Open a terminal in the solution directory and run:

```bash
dotnet restore
```

This restores the NuGet packages required by the projects.

### 11.3 Configure PostgreSQL

1. Install and start PostgreSQL.
2. Create the database used by the application.
3. Configure the connection string.
4. Make sure the configured PostgreSQL user has the required database permissions.
5. Confirm that the application can connect to the database.

Example database name:

```text
equipment_borrowing
```

### 11.4 Configure the Connection String

Set the PostgreSQL connection string using the environment variable expected by the application:

**Environment Variable:** `EQUIPMENT_BORROWING_CONNECTION`

In PowerShell (Windows):
```powershell
$env:EQUIPMENT_BORROWING_CONNECTION = "Host=localhost;Port=5432;Database=equipment_borrowing;Username=postgres;Password=YOUR_PASSWORD"
```

In Bash (macOS / Linux):
```bash
export EQUIPMENT_BORROWING_CONNECTION="Host=localhost;Port=5432;Database=equipment_borrowing;Username=postgres;Password=YOUR_PASSWORD"
```

If the environment variable is not explicitly set, the application defaults to the standard local PostgreSQL development connection string:
```text
Host=localhost;Port=5432;Database=equipment_borrowing;Username=postgres;Password=postgres
```

**Security note:** Do not commit real passwords to the Git repository.

### 11.5 Apply Database Migrations

Run the migration command appropriate for the solution:

```bash
dotnet ef database update
```

If the application applies migrations automatically during startup, follow the project's initialization process instead.

### 11.6 Build the Solution

Run:

```bash
dotnet build
```

Resolve any build errors before running the application.

---

## 12. Running the Application

After restoring dependencies, configuring PostgreSQL, and preparing the database, run the desktop application.

From the solution directory:

```bash
dotnet run --project EquipmentBorrowing.Desktop
```

If the project file is located in a different directory, provide its correct relative path.

When the application starts, the custodian should be able to access the implemented system functions through the Avalonia desktop interface.

---

## 13. Database Context Lifetime

The `EquipmentBorrowingDbContext` manages communication between the application and PostgreSQL.

A DbContext should be used for a limited operation or unit of work and then disposed.

For desktop applications, a suitable approach is to create a short-lived DbContext for each application operation or service operation, rather than keeping one DbContext alive for the entire application session.

### Why a Short-Lived DbContext Is Appropriate

- It avoids keeping tracked entities alive longer than necessary.
- It reduces the risk of working with stale tracked data.
- It helps prevent unintended changes from unrelated operations from being saved together.
- It makes the boundaries of database operations easier to understand.
- It allows each operation to manage its own database work.

The actual lifetime should match the dependency injection configuration and service design used by the project.

A DbContext should not be used concurrently by multiple operations.

---

## 14. Transaction Handling

Database transactions help ensure that related database changes are completed consistently.

This is particularly important when an operation modifies multiple records.

### 14.1 Single-Item Borrowing

A successful borrowing operation may require changes to both:

- The borrowing record.
- The equipment availability state.

These changes should be saved consistently so that the database does not contain a borrowing record without the corresponding equipment state update.

### 14.2 Multiple-Item Checkout

The multi-item checkout operation is especially important because it involves multiple equipment items.

The operation should:

1. Receive the selected student and equipment items.
2. Validate the student and borrowing eligibility.
3. Validate the borrowing limit.
4. Confirm that every selected equipment item exists and is available.
5. Create the required borrowing records.
6. Update the availability state of the selected equipment.
7. Commit the transaction only after all required changes succeed.

If a required item cannot be checked out or a database operation fails, the transaction should be rolled back so that the database does not retain only part of the checkout operation.

### 14.3 Transaction Benefits

Transactions help provide:

- Consistency between borrowing records and equipment availability.
- Atomic behavior for multi-item checkout.
- Protection against partially completed operations.
- A clear boundary for committing or rolling back database changes.

### 14.4 Error Handling

The application should handle expected validation failures and database errors appropriately.

Examples include:

- Student does not exist.
- Student is not eligible to borrow.
- Student has reached the borrowing limit.
- Equipment does not exist.
- Equipment is no longer available.
- Borrowing record is already returned.
- Database operation fails.

The ViewModel should display useful feedback to the custodian without exposing sensitive database details.

**Implementation note:** The transaction behavior described here should be checked against the actual service and repository code. Include only the transaction mechanisms and rollback behavior that the application implements.

---

## 15. Testing and Verification

Testing is used to verify that the system's operations follow the required business rules and produce the expected results.

### 15.1 Automated Tests

The `EquipmentBorrowing.Tests` project contains automated tests for the implemented system behavior.

Relevant test areas include:

- Student registration and validation.
- Equipment registration and validation.
- Equipment availability.
- Borrowing eligibility.
- Borrowing limits.
- Individual borrowing.
- Multi-item checkout.
- Returning equipment.
- Student borrowing history.
- Equipment borrowing history.
- Search, filtering, and sorting.

The final test documentation should identify the actual test cases and their results.

### 15.2 Database Verification

Database verification should confirm that application operations correctly affect PostgreSQL.

Examples include:

- A newly registered student appears in the Students table.
- Newly registered equipment appears in the Equipment table.
- A successful borrowing creates the expected borrowing record.
- Borrowed equipment is no longer available.
- A successful return updates the borrowing status.
- Returned equipment becomes available again.
- Borrowing history remains stored.
- Multi-item checkout either completes fully or rolls back when a required operation fails.
- Data remains available after restarting the application.

### 15.3 Integration and Persistence Evidence

The project submission should include evidence showing that the application, services, and PostgreSQL database work together.

Useful evidence includes:

- Successful application startup.
- Database tables created through migrations.
- Database records after registration.
- Database records after borrowing.
- Database records after returning equipment.
- Multi-item checkout success.
- Multi-item checkout failure and rollback, where implemented.
- Borrowing history displayed by the application.
- Data persistence after restarting the application.

Refer to the separate integration and verification evidence submitted with the project.

---

## 16. Documentation and Project Deliverables

The README explains the system, architecture, database setup, and running instructions.

The remaining use-case and verification documents should be maintained as separate submission documents.

The project documentation should include:

1. **Use-Case Diagram** – shows the primary actor and the system's required use cases.
2. **Use-Case Catalog** – lists the use-case IDs, names, actors, and brief descriptions.
3. **Use-Case Specifications** – provides the detailed specifications for all fourteen required use cases.
4. **Use-Case-to-Acceptance Evidence Matrix** – connects each required use case to its acceptance criteria and corresponding verification evidence.
5. **Integration and Verification Evidence** – contains screenshots or other evidence showing the application's behavior and database results.

The source code, documentation, and evidence should correspond to the same final version of the application.

---

## 17. Reflection

### 1. Why should the Application layer depend on repository interfaces instead of directly depending on a database implementation?

The Application layer should depend on repository interfaces because this separates business logic from the specific database technology. This allows the Infrastructure implementation to change without requiring changes to the application's use cases.

### 2. Which parts of the solution should remain independent of PostgreSQL?

The Domain models, Application services, and repository interfaces should remain independent of PostgreSQL. The Infrastructure project contains the database-specific implementation, including the DbContext, entity configurations, migrations, and EF Core repository implementations.

### 3. Why should Avalonia Views not directly execute database queries?

Avalonia Views should be responsible for displaying information and collecting user input. Direct database access from the Views would couple the user interface to the storage implementation and could allow business rules to be bypassed.

### 4. Why should business rules not exist only in the ViewModel?

The ViewModel is responsible for presentation state and user interactions. Business rules belong in the Application or Domain layers so they can be reused, tested independently, and enforced regardless of which interface calls the application.

### 5. What is the responsibility of a ViewModel?

A ViewModel holds presentation state, exposes properties and commands, collects user input, and delegates application operations to the Application layer.

### 6. Why is transaction handling important for multi-item checkout?

Multi-item checkout involves several related database changes. A transaction ensures that all required changes can be committed together or rolled back if the operation fails, preventing partially completed checkout operations.

### 7. Why is a short-lived DbContext appropriate for a desktop application?

A short-lived DbContext limits entity tracking to a specific operation, reduces stale data and unintended state changes, and provides a clear boundary for database work.

---

## 18. Conclusion

The Campus Equipment Borrowing System uses a layered architecture to separate the user interface, application operations, domain concepts, and database implementation.

Avalonia UI provides the desktop interface, the MVVM pattern organizes presentation logic, the Application layer coordinates use cases, and Entity Framework Core with PostgreSQL provides persistent data storage.

The system is designed to support the custodian's equipment management and borrowing responsibilities while maintaining consistent borrowing records, equipment availability, and historical information.


---

## 19. Lab 5 � REST API & Desktop Migration (ITSD81)

### Architecture Overview

```text
EquipmentBorrowing.Desktop  (Avalonia UI � HTTP client only, zero direct DB dependencies)
        |
        | HTTP (JSON REST via typed EquipmentBorrowingApiClient)
        v
EquipmentBorrowing.Api  (ASP.NET Core Web API � port 5000)
        |
        +---------> EquipmentBorrowing.Application (Services & Repository Interfaces)
        |                       |
        |                       v
        +---------> EquipmentBorrowing.Infrastructure (EF Core + PostgreSQL)
                                |
                                v
                         EquipmentBorrowing.Domain (Core entities & business rules)
```

### Running the Solution

#### 1. Start the API
```powershell
dotnet run --project src/EquipmentBorrowing.Api
```
- Listens on `http://localhost:5000`
- Connects to PostgreSQL database `equipment_borrowing_db`
- Machine-readable OpenAPI spec available at `http://localhost:5000/openapi/v1.json`

#### 2. Start the Desktop App
```powershell
dotnet run --project src/EquipmentBorrowing.Desktop
```
- Connects to the API at `http://localhost:5000/` (override with `EQUIPMENT_BORROWING_API_URL` environment variable if needed)
- Communicates exclusively over HTTP via `EquipmentBorrowingApiClient`

#### 3. Run Unit Tests
```powershell
dotnet run --project tests/EquipmentBorrowing.Test
```

### API Endpoints Reference (API-01 to API-16)

| Endpoint | Method | Route | Description |
|---|---|---|---|
| API-01 | GET | /api/v1/equipment | List equipment catalog (supports ?available=true) |
| API-02 | GET | /api/v1/students | List registered students (supports ?search=...) |
| API-03 | POST | /api/v1/students | Register a new student |
| API-04 | GET | /api/v1/students/{id} | Get student details by ID |
| API-05 | PUT | /api/v1/students/{id} | Update student descriptive information |
| API-06 | PATCH | /api/v1/students/{id}/eligibility | Change student borrowing eligibility |
| API-07 | POST | /api/v1/equipment | Register a new equipment item |
| API-08 | GET | /api/v1/equipment/{id} | Get equipment details by ID |
| API-09 | PUT | /api/v1/equipment/{id} | Update equipment descriptive information |
| API-10 | POST | /api/v1/borrowings | Create a single equipment borrowing record |
| API-11 | GET | /api/v1/borrowings/{id} | Get borrowing record by ID |
| API-12 | POST | /api/v1/borrowings/{id}/return | Record equipment return |
| API-13 | GET | /api/v1/borrowings | Search, filter, and sort loans (status, search, sort, direction) |
| API-14 | GET | /api/v1/students/{id}/borrowings | Retrieve borrowing history for a specific student |
| API-15 | GET | /api/v1/equipment/{id}/borrowings | Retrieve loan history for a specific equipment item |
| API-16 | POST | /api/v1/checkout-requests | Multi-item checkout (minimum 2 items) |
