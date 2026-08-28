EQUIPMENT BORROWING SYSTEM

* Solution Structure
### 1. Domain

- The `EquipmentBorrowing.Domain` contains the project's important concepts and rules.
- It contains:
  - `Student` – represents a student who borrows equipment.
  - `Equipment` – the item that can be borrowed.
  - `Borrowing` – a borrowing record containing the student, equipment, dates, and status.
  - `BorrowingStatus` – the status of the borrowing process, such as Active or Returned.

2. Application
	- the EquipmentBorrowing.Application contains the business logic and use cases of the system
	- it contains:
		services - the services that handle the borrowing process, such as BorrowEquipmentService and ReturnEquipmentService
				BorrowEquipmentService - performs the Borrow Equipment use case and validates the required conditions.
				ReturnEquipmentService - performs the Return Equipment use case.
				CheckAvailableEquipmentService - etrieves equipment that is currently available.
		Interfaces - the interfaces that define the contracts for the services, such as IStudentRepository, IEquipmentRepository, and IBorrowingRepository.

3. Infrastructure
	- The EquipmentBorrowing.Infrastructure project contains the technical implementations of the repository abstractions.
	- it contains:
		InMemoryStudentRepository
        InMemoryEquipmentRepository
        InMemoryBorrowingRepository

4. Console
	- The EquipmentBorrowing.Console project an executable program for demonstrating the application flow

5. Tests
	- The EquipmentBorrowing.Tests project is used for automated tests of domain or application behavior
