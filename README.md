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

  * `InMemoryStudentRepository`
  * `InMemoryEquipmentRepository`
  * `InMemoryBorrowingRepository`

### 4. Console

* The `EquipmentBorrowing.Console` project is an executable program for demonstrating the application flow

### 5. Tests

* The `EquipmentBorrowing.Tests` project is used for automated tests of domain or application behavior
 
---

## 2. Dependency Direction (Architecture Diagram)

The architecture direction of the current solution is:

```text
EquipmentBorrowing.Console
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

  * implements the repository interfaces defined in the Application project and uses the Domain models to store and retrieve information.
* the EquipmentBorrowing.Console project depends on the EquipmentBorrowing.Application and EquipmentBorrowing.Infrastructure

  * It uses the Application services and Infrastructure repository implementations and provides their dependencies manually.
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
| **Infrastructure Implementations Used** | `InMemoryStudentRepository`, `InMemoryEquipmentRepository`, `InMemoryBorrowingRepository` |

---

## 5. Reflection

### 1. Why should the application service depend on a repository interface instead of directly depending on a database implementation?

The application service should depend on a repository interface because it separates the business logic from the specific data storage technology. This allows the application service to work with different repository implementations without changing the business logic.

### 2. Which parts of your current solution could remain unchanged if SQLite were added later?

The Domain models, Application services, and repository interfaces could remain unchanged. The Infrastructure project could be updated by adding SQLite-based repository implementations while keeping the existing application logic independent from the database technology.

### 3. Which project would eventually contain Avalonia Views?

The Console project would eventually contain the Avalonia Views. In our current structure, the Console project is used to run and demonstrate the system through the terminal, while Avalonia would be used as the graphical user interface. The Views should stay in the UI project so that the user interface is separated from the application's use cases and business logic.

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
EquipmentBorrowing.Console
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

The Domain models, Application services, repository interfaces, and the entire Desktop project (Views and ViewModels) should remain unchanged — only the Infrastructure implementations would need to change.

