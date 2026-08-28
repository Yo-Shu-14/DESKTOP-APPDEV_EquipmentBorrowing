# EQUIPMENT BORROWING SYSTEM

## 1. Solution Structure

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

## 2. Dependency Direction

The dependency direction of the current solution is:

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
