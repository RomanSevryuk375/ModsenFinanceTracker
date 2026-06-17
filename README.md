[English](README.md) | [Русский](README.ru.md)

# Modsen Finance Tracker

![.NET 10](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_12-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Spectre.Console](https://img.shields.io/badge/Spectre.Console-000000?style=for-the-badge&logo=cli&logoColor=white)
![Serilog](https://img.shields.io/badge/Serilog-E32029?style=for-the-badge&logo=serilog&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-5C2D91?style=for-the-badge&logo=xunit&logoColor=white)

> **A scalable console-based system for personal finance management.**  
> This project was developed to demonstrate the principles of Clean Architecture, Domain-Driven Design (DDD), and GoF design patterns. The application is abstracted from any specific data source or user interface, ensuring a high degree of testability and flexibility.

---

## Solution Structure

```text
Modsen.FinanceTracker.sln
├── src
│   ├── Modsen.FinanceTracker.Domain         # Business entities and contracts
│   ├── Modsen.FinanceTracker.BLL            # Use cases, Factories, Decorators
│   ├── Modsen.FinanceTracker.DAL            # JSON data access (Repository, UnitOfWork)
│   ├── Modsen.FinanceTracker.Infrastructure # External APIs, Export (PDF/Docx), Cryptography
│   └── Modsen.FinanceTracker.UI             # Interactive console menu (Spectre.Console)
└── tests
    └── FinanceTracker.Tests                 # Unit tests for Domain, BLL, and DAL layers
```

## System Architecture

The project is strictly divided into 5 layers in accordance with Clean Architecture and Dependency Inversion principles:

1.  **Domain (`Modsen.FinanceTracker.Domain`)**: The core of the system. Contains business entities (`Wallet`, `Transaction`), value objects (`Money`, `TransactionDate`), and domain events (`CategoryLimitExceededEventArgs`). It has no external dependencies.
2.  **BLL (`Modsen.FinanceTracker.BLL`)**: Business Logic Layer. Contains services, aggregate coordination, and decorators.
3.  **DAL (`Modsen.FinanceTracker.DAL`)**: Data Access Layer. Implements JSON storage via `IRepository` and `IUnitOfWork` abstractions.
4.  **Infrastructure (`Modsen.FinanceTracker.Infrastructure`)**: Integration with third-party libraries (PDF/Docx generation, HTTP calls to external currency APIs, password hashing).
5.  **UI (`Modsen.FinanceTracker.UI`)**: Console interface built on `Spectre.Console` components.

---

## Design Patterns and Principles

The codebase demonstrates a deep understanding of design patterns used to solve specific business requirements from the technical specification:

### 1. Domain-Driven Design (DDD) & Result Pattern
*   **Value Objects**: Created for primitive data types (`Money`, `TransactionDescription`, `TransactionDate`). This shifts validation to the type system level: it is impossible to create a transaction with a negative amount or an empty description string.
*   **Rejection of Exceptions in Business Logic**: Implemented the `Result` / `Result<T>` pattern (Railway-Oriented Programming). Business errors are returned as objects, making the control flow predictable and safe.

### 2. Structural Patterns
*   **Decorator**: Used to transparently extend service functionality without modifying their source code. 
    *   `*ServiceLoggingDecorator` logs all method calls to Serilog.
    *   `CachedCurrencyService` adds caching (MemoryCache) on top of HTTP calls to the external currency rate API.

### 3. Behavioral Patterns
*   **Strategy**: Applied to implement the reporting system. The `IExportStrategy` interface has concrete implementations: `CsvExportStrategy`, `TxtExportStrategy`, `PdfExportStrategy`, and `DocxExportStrategy`. Adding a new export format does not require modifying existing code (Open-Closed Principle).
*   **Observer (Events)**: Implemented an event-driven model. The `FinanceService` publishes an `OnCategoryLimitExceeded` event, which the UI layer subscribes to in order to display budget exceeded warnings, maintaining Loose Coupling.

### 4. Creational Patterns
*   **Factory Method**: `TransactionFactory` encapsulates the initialization logic for specific transaction types (`IncomeTransaction`, `ExpenseTransaction`).
*   **Singleton**: Used for thread-safe access to the global application configuration `AppConfiguration`.

### 5. Data Access Patterns
*   **Repository & Unit of Work**: The logic for working with the JSON file (`JsonDbContext`) is hidden behind abstractions. The current in-memory/JSON implementation can be easily replaced with Entity Framework Core and a relational database without altering the BLL or Domain layers.

---

## Implemented Features

The system fully covers the basic and advanced requirements of the technical specification:

*   **Multi-currency Wallets**: Support for various base currencies with automatic conversion when checking balances via an external API (Open ER-API).
*   **Transactions and Categories**: Full CRUD operations for incomes and expenses. Prevents balances from dropping below zero. Global category limits.
*   **Analytics**: Full-text search, date filtering, and visualization of expense distribution in percentages.
*   **Scheduler (Recurring Tasks)**: The `SchedulerService` module allows configuring recurring charges (subscriptions, rent). Transactions are created automatically upon application startup if the due date has arrived.
*   **Security**: Application login protected by a password (verified against a SHA-256 hash).
*   **Logging**: All user activities and system exceptions are recorded in an `app.log` file using Serilog.

---

## Technology Stack

| Component / Layer | Tools |
| --- | --- |
| **Platform** | .NET 10 |
| **Architecture** | Clean Architecture, DDD, SOLID |
| **User Interface** | Spectre.Console |
| **Logging** | Serilog, Serilog.Sinks.File |
| **Report Export** | QuestPDF (PDF), DocX (Word) |
| **Integration** | `HttpClient`, `System.Text.Json` |
| **Dependency Injection** | `Microsoft.Extensions.DependencyInjection` |
| **Testing** | xUnit, NSubstitute, FluentAssertions |

---

## Local Deployment

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/romansevryuk375-modsenfinancetracker.git
   cd romansevryuk375-modsenfinancetracker
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the tests to verify the integrity of the business logic:
   ```bash
   dotnet test
   ```

4. Launch the application:
   ```bash
   dotnet run --project src/Modsen.FinanceTracker.UI
   ```

*Note: Upon the first launch, you will be prompted to enter a password. The default password is hardcoded in the configuration as a hash. (admin123)*

---