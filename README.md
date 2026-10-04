# EQUIPMENT BORROWING SYSTEM

## 1. Solution Structure
Actor: Student

Expectation: The student expects the system to allow them to request available equipment and validate whether they are allowed to borrow it.
### 1. Domain

* The `EquipmentBorrowing.Domain` contains the project's important concepts and rules.
* It Contains:

  * `Student` – represent a student who borrows the equipment
  * `Equipment` – the item that can be borrowed
  * `Borrowing` – a borrowing record containing the student, equipment, dates, and status.
  * `BorrowingStatus` – the status of the borrowing process, such as Active or Returned

### 2. Application

* the `EquipmentBorrowing.Application` contains the business logic and use cases of the system
* it contains:

  **Services**

  * the services that handle the borrowing process, such as BorrowEquipmentService and ReturnEquipmentService
  * `BorrowEquipmentService` - performs the Borrow Equipment use case and validates the required conditions.
  * `ReturnEquipmentService` - performs the Return Equipment use case.
  * `CheckAvailableEquipmentService` - retrieves equipment that is currently available.

  **Interfaces**

  * the interfaces that define the contracts for the services, such as IStudentRepository, IEquipmentRepository, and IBorrowingRepository.

### 3. Infrastructure

* The `EquipmentBorrowing.Infrastructure` project contains the technical implementations of the repository abstractions.
* it contains:

  * `EquipmentBorrowingDbContext` – the EF Core DbContext used to access the SQLite database.

  * `EfStudentRepository` – the EF Core implementation of `IStudentRepository`.

  * `EfEquipmentRepository` – the EF Core implementation of `IEquipmentRepository`.

  * `EfBorrowingRepository` – the EF Core implementation of `IBorrowingRepository`.

  * Entity configurations for `Student`, `Equipment`, and `Borrowing`.

  * Database initialization and EF Core migration support.

* SQLite is used as the persistent database, while EF Core handles database access and object-relational mapping.


### 4. Desktop

* The `EquipmentBorrowing.Desktop` project is an executable Avalonia application that provides the graphical user interface.

* It contains the Avalonia Views and ViewModels used to interact with the Application services and repository abstractions.


### 5. Tests

* The `EquipmentBorrowing.Tests` project is used for automated tests of domain or application behavior
 
---

## 2. Dependency Direction (Architecture Diagram)

The architecture direction of the current solution is:

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

* the EquipmentBorrowing.Application project depends on the EquipmentBorrowing.Domain
* the EquipmentBorrowing.Infrastructure project depends on both the EquipmentBorrowing.Application and EquipmentBorrowing.Domain

  * mplements the repository interfaces defined in the Application project and uses the Domain models to store and retrieve information through Entity Framework Core and SQLite.
* the EquipmentBorrowing.Desktop project depends on the EquipmentBorrowing.Application and EquipmentBorrowing.Infrastructure

  * It uses the Application services and repository abstractions to perform the system operations and provides the dependencies through dependency injection.
* the EquipmentBorrowing.Domain does not depend on any other project, as it contains the core concepts and rules of the system.

---

## 3. Requirements and Use Case Analysis

### A. Actors

**Student**

The student is the primary actor who interacts with the Campus Equipment Borrowing System. The student expects the system to allow them to borrow available equipment if they are authorized and have not reached the maximum number of active borrowings. The student may also return borrowed equipment.

### B. Use Cases

#### Use Case 1: Borrow Equipment

| Item                 | Description                                                                                                                                                                           |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Use Case**         | Borrow Equipment                                                                                                                                                                      |
| **Primary Actor**    | Student                                                                                                                                                                               |
| **Preconditions**    | The student exists, is allowed to borrow equipment, the equipment exists and is available, and the student has not reached the maximum number of active borrowings.                   |
| **Main Action**      | The student requests to borrow an available piece of equipment. The application validates the student, equipment, and borrowing rules, then creates a borrowing record.               |
| **Expected Result**  | The borrowing is created successfully, and the equipment becomes unavailable.                                                                                                         |
| **Possible Failure** | The student does not exist, is not allowed to borrow, the equipment does not exist, the equipment is unavailable, or the student has reached the maximum number of active borrowings. |

#### Use Case 2: Return Equipment

| Item                 | Description                                                                                   |
| -------------------- | --------------------------------------------------------------------------------------------- |
| **Use Case**         | Return Equipment                                                                              |
| **Primary Actor**    | Student                                                                                       |
| **Preconditions**    | An active borrowing record exists for the student and equipment.                              |
| **Main Action**      | The student returns the borrowed equipment, and the application updates the borrowing status. |
| **Expected Result**  | The borrowing is marked as returned and the equipment becomes available again.                |
| **Possible Failure** | The borrowing record does not exist or the borrowing is already returned.                     |

#### Use Case 3: Find Available Equipment

| Item                 | Description                                                           |
| -------------------- | --------------------------------------------------------------------- |
| **Use Case**         | Find Available Equipment                                              |
| **Primary Actor**    | Student                                                               |
| **Preconditions**    | Equipment records are available in the system.                        |
| **Main Action**      | The student requests a list of equipment that is currently available. |
| **Expected Result**  | The system provides the available equipment.                          |
| **Possible Failure** | No equipment is currently available.                                  |

### C. Domain Concepts

#### Student

**Information:**

* Student ID
* Student name
* Whether the student is currently allowed to borrow

**Rules or State:**

* A student must be allowed to borrow equipment.
* A student must not exceed the maximum number of active borrowings.

**Not the responsibility of Student:**

* Storing equipment records
* Creating repository connections
* Coordinating the entire borrowing operation

#### Equipment

**Information:**

* Equipment ID
* Equipment name
* Equipment description
* Availability status

**Rules or State:**

* Equipment can be available or unavailable.
* Equipment becomes unavailable when successfully borrowed.
* Equipment becomes available again when returned.

**Not the responsibility of Equipment:**

* Managing students
* Creating borrowing records
* Accessing repositories or databases

#### Borrowing

**Information:**

* Student
* Equipment
* Date borrowed
* Expected return date
* Current borrowing status

**Rules or State:**

* A borrowing has a current status such as Active or Returned.
* A returned borrowing should no longer represent an active borrowing.

**Not the responsibility of Borrowing:**

* Retrieving students or equipment from repositories
* Managing database connections
* Coordinating the complete application use case

---

## 4. Use Case Mapping

### Borrow Equipment

| Item                                    | Implementation                                                                            |
| --------------------------------------- | ----------------------------------------------------------------------------------------- |
| **Actor**                               | Student                                                                                   |
| **Use Case**                            | Borrow Equipment                                                                          |
| **Application Service**                 | `BorrowEquipmentService`                                                                  |
| **Domain Objects Used**                 | `Student`, `Equipment`, `Borrowing`, `BorrowingStatus`                                    |
| **Repository Interfaces Used**          | `IStudentRepository`, `IEquipmentRepository`, `IBorrowingRepository`                      |
| **Infrastructure Implementations Used** | `EfStudentRepository`, `EfEquipmentRepository`, `EfBorrowingRepository`                   |

---

## 5. Reflection

### 1. Why should the application service depend on a repository interface instead of directly depending on a database implementation?

The application service should depend on a repository interface because it separates the business logic from the specific data storage technology. This allows the application service to work with different repository implementations without changing the business logic.

### 2. Which parts of your current solution could remain unchanged if SQLite were added later?

The Domain models, Application services, and repository interfaces can remain unchanged when using SQLite. The Infrastructure project contains the SQLite configuration, EF Core DbContext, entity configurations, migrations, and EF Core repository implementations, keeping the application logic independent from the database technology.

### 3. Which project would eventually contain Avalonia Views?

The `EquipmentBorrowing.Desktop` project contains the Avalonia Views. It serves as the graphical user interface of the system, while the Application project contains the use cases and business logic. This keeps the user interface separated from the application's use cases and business rules.

### 4. Should an Avalonia button directly execute database queries? Why or why not?

No, an Avalonia button should not directly execute database queries. The button should only handle the user's interaction and call the appropriate use case or service in the Application project. This keeps the user interface separate from the database and makes the system easier to maintain, test, and modify.

### 5. What part of your implementation represents the actual business operation requested by the actor?

The Application use case or service represents the actual business operation requested by the actor. In the project, for example, BorrowEquipmentService represents the operation of borrowing equipment, while ReturnEquipmentService represents returning equipment. These services contain the rules and steps needed to complete the requested operation.

---

## Avalonian UI and MVVM

## 1. Desktop Project

Explain the responsibility of EquipmentBorrowing.Desktop and how it interacts with the existing projects.

* The `EquipmentBorrowing.Desktop` contains the user interface of your system, such as the Avalonia Views and ViewModels, and serves as the executable application that users interact with.

## 2. Updated Architecture

```text
Avalonia View
        |
        |  Binding / Command
        |
ViewModel
        |
        |  Application Operation
        |
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

## 3. Borrow Equipment flow

The user selects a student, an available equipment item, and an expected return date on the Equipment view, then presses Borrow. This triggers EquipmentViewModel.BorrowAsync, a [RelayCommand], which performs presentation-level checks (student selected, date selected, date not in the past) and then calls BorrowEquipmentService.BorrowEquipmentAsync. The service validates the student, equipment availability, and borrowing limit, creates the Borrowing record, and marks the equipment unavailable. The ViewModel reloads the equipment list and shows a success or error message via FeedbackMessage.

## 4. Return Equipment Flow

On the Active Borrowings view, the user presses Return on a borrowing. This triggers ActiveBorrowingsViewModel.ReturnAsync, which calls ReturnEquipmentService.ReturnEquipmentAsync. The service checks that the borrowing exists and hasn't already been returned, marks it Returned, and makes the equipment available again. The ViewModel reloads the active borrowings list and shows a success or error message.

## 5. Architectural Reflection

### 1. Why should the View not call a repository directly?

Because the View is only responsible for displaying data and collecting input. Calling a repository directly would let the UI bypass business rules and tie it to a specific storage technology.

### 2. Why should business rules not be implemented in the ViewModel?

The ViewModel is a presentation-layer component. Business rules belong to the Application/ Domain layers so they can be reused, tested independently, and reasoned about without the UI.

### 3. What is the responsibility of the ViewModel?

To hold presentation state, expose commands, collect user input, and delegate the actual work to Application services.

### 4. Why can the existing Application layer work without knowing that Avalonia is being used?

Because it only depends on repository interfaces and domain models — it has no reference to Avalonia at all, so any UI technology can call it the same way.

### 5. What advantage is gained from registering dependencies in one composition point?

All wiring of interfaces to implementations happens in one place (App.axaml.cs), so the rest of the app depends only on abstractions and doesn't need to know how objects are built.

### 6. If the in-memory repository were replaced by SQLite later, which parts of the current interface should remain largely unchanged?

The Domain models, Application services, repository interfaces, and the entire Desktop project (Views and ViewModels) should remain largely unchanged. The Infrastructure project contains the EF Core and SQLite implementation, including the DbContext, entity configurations, migrations, and repository implementations.

## 6. Relational Database Design

The system uses a relational database to permanently store students, equipment, and borrowing records. The database is implemented using SQLite and Entity Framework Core.

### 1. Students Table

The `Students` table stores the information about students who can borrow equipment.

| Column              | Data Type  | Description                                                   |
|---------------------|------------|---------------------------------------------------------------|
| `Id`                | Integer    | Primary key that uniquely identifies a student.               |
| `Name`              | String     | Required name of the student.                                 |
| `IsAllowedToBorrow` | Boolean    | Indicates whether the student is allowed to borrow equipment. |

### 2. Equipment Table

The `Equipment` table stores the equipment available in the system.

| Column        | Data Type | Description                                             |
|---------------|-----------|---------------------------------------------------------|
| `EquipmentId` | GUID      | Primary key that uniquely identifies an equipment item. |
| `Name`        | String    | Required name of the equipment.                         |
| `Description` | String    | Required description of the equipment.                  |
| `IsAvailable` | Boolean   | Indicates whether the equipment is currently available. |

### 3. Borrowings Table

The `Borrowings` table stores the records of equipment borrowed by students.

| Column               | Data Type | Description                                                  |
|----------------------|-----------|--------------------------------------------------------------|
| `BorrowingId`        | GUID      | Primary key that uniquely identifies a borrowing record.     |
| `StudentId`          | Integer   | Foreign key that references the `Students` table.            |
| `EquipmentId`        | GUID      | Foreign key that references the `Equipment` table.           |
| `DateBorrowed`       | DateTime  | Date and time when the equipment was borrowed.               |
| `ExpectedReturnDate` | DateTime  | Expected date when the equipment should be returned.         |
| `Status`             | String    | Stores the borrowing status, such as `Active` or `Returned`. |

### 4. Relationships

The database contains two main one-to-many relationships:

* One `Student` can have many `Borrowing` records.

* One `Equipment` item can appear in many `Borrowing` records over time.

* Each `Borrowing` record belongs to one `Student` and one `Equipment` item.

The `StudentId` and `EquipmentId` columns in the `Borrowings` table act as foreign keys that connect the borrowing records to their related records.

### 5. Indexes

Indexes are configured for the `StudentId` and `EquipmentId` foreign keys in the `Borrowings` table. These indexes help improve queries that retrieve borrowing records based on a specific student or equipment item.

### 6. Database Diagram

The database relationship diagram is provided in:

`docs/database-diagram.png`

The diagram shows the `Students`, `Equipment`, and `Borrowings` tables, including their primary keys, foreign keys, and relationships.

## 7. SQLite and Entity Framework Core Configuration

The system uses SQLite as the persistent database and Entity Framework Core as the Object-Relational Mapper (ORM). Entity Framework Core handles the connection between the application and the SQLite database while keeping the database implementation inside the Infrastructure layer.

### 1. Entity Framework Core Packages

The Infrastructure project uses the following Entity Framework Core packages:

* `Microsoft.EntityFrameworkCore` – provides the main EF Core functionality.
* `Microsoft.EntityFrameworkCore.Design` – provides tools needed for migrations and database development.
* `Microsoft.EntityFrameworkCore.Sqlite` – allows EF Core to use SQLite as the database provider.

All three packages use version `10.0.12`.

### 2. SQLite Database Connection

The application uses the following SQLite connection:

```text
Data Source=equipmentborrow.db
```

### 3. DbContext

The `EquipmentBorrowingDbContext` is located in:

```text
EquipmentBorrowing.Infrastructure.Data
```

It defines the database sets for the three main entities:

* `Students`
* `Equipment`
* `Borrowings`

The `DbContext` also applies the entity configurations from the Infrastructure assembly.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(EquipmentBorrowingDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
}
```

This keeps the database configuration separate from the domain classes.

### 4. Entity Configurations

Each entity has its own Entity Framework Core configuration.

#### StudentConfiguration

The `Students` table is configured with:

* `Id` as the primary key.
* `Name` as a required field with a maximum length of 100 characters.
* `IsAllowedToBorrow` as a required field.

#### EquipmentConfiguration

The `Equipment` table is configured with:

* `EquipmentId` as the primary key.
* `Name` as a required field with a maximum length of 100 characters.
* `Description` as a required field with a maximum length of 500 characters.
* `IsAvailable` as a required field.

#### BorrowingConfiguration

The `Borrowings` table is configured with:

* `BorrowingId` as the primary key.
* `StudentId` as a foreign key to `Students`.
* `EquipmentId` as a foreign key to `Equipment`.
* `DateBorrowed` as a required field.
* `ExpectedReturnDate` as a required field.
* `Status` stored as a string.
* Indexes on `StudentId` and `EquipmentId`.

The relationships ensure that each borrowing record is connected to an existing student and equipment record.

### 5. Database Configuration

The SQLite database is registered through the `AddDatabase()` extension method. This allows the application to configure the `DbContext` through dependency injection.

```csharp
services.AddDbContext<EquipmentBorrowingDbContext>(options =>
    options.UseSqlite("Data Source=equipmentborrow.db"));
```

This keeps the database connection and EF Core configuration outside the Views and ViewModels.

---

## 8. Migrations and Database Initialization

Entity Framework Core migrations are used to create and update the SQLite database schema. This allows the database structure to be reproduced consistently from the application's entity configurations.

### 1. Initial Migration

The project contains an initial EF Core migration:

```text
20261002130944_InitialCreate
```

The migration creates the following tables:

* `Students`
* `Equipment`
* `Borrowings`

It also creates the primary keys, foreign keys, indexes, and other configured columns required by the database design.

### 2. Applying the Migration

The application applies pending migrations during database initialization through the `DatabaseInitializer`.

```csharp
await db.Database.MigrateAsync();
```

This ensures that the SQLite database has the schema defined by the current EF Core migrations.

### 3. Database Initialization

The `DatabaseInitializer` is located in the Infrastructure project.

It first applies any pending migrations. It then checks whether student records already exist before adding the initial seed data.

```csharp
if (await db.Students.AnyAsync())
{
    return;
}
```

This prevents the application from recreating the seed data every time it starts.

### 4. Seed Data

The initial database contains:

* Multiple student records.
* Multiple equipment records.
* Equipment records that are initially available.

The seed data is used to demonstrate the borrowing and validation workflows of the application.

### 5. Persistence

The database is stored in the SQLite file:

```text
equipmentborrow.db
```

Because the data is stored in the SQLite database instead of memory, changes made during the application are preserved after the application is closed and opened again.

For example:

* Borrowed equipment remains unavailable after restarting the application.
* Active borrowing records remain stored in the database.
* Returned equipment becomes available again, and the borrowing record remains recorded as returned.
