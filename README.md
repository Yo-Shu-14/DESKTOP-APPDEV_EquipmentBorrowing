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


### 3. Which project would eventually contain Avalonia Views?


### 4. Should an Avalonia button directly execute database queries? Why or why not?


### 5. What part of your implementation represents the actual business operation requested by the actor?
