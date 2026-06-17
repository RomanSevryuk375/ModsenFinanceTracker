[English](README.md) | [Русский](README.ru.md)

# Modsen Finance Tracker

![.NET 10](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_12-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Spectre.Console](https://img.shields.io/badge/Spectre.Console-000000?style=for-the-badge&logo=cli&logoColor=white)
![Serilog](https://img.shields.io/badge/Serilog-E32029?style=for-the-badge&logo=serilog&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-5C2D91?style=for-the-badge&logo=xunit&logoColor=white)

> **Масштабируемая консольная система для учета личных финансов.**  
> Проект разработан в качестве демонстрации принципов многослойной архитектуры (Clean Architecture), Domain-Driven Design (DDD) и паттернов проектирования (GoF). Приложение абстрагировано от конкретного источника данных и пользовательского интерфейса, обеспечивая высокую степень тестируемости и гибкости.

---

## Структура решения

```text
Modsen.FinanceTracker.sln
├── src
│   ├── Modsen.FinanceTracker.Domain         # Бизнес-сущности и контракты
│   ├── Modsen.FinanceTracker.BLL            # Сценарии использования, Фабрики, Декораторы
│   ├── Modsen.FinanceTracker.DAL            # Работа с JSON (Repository, UnitOfWork)
│   ├── Modsen.FinanceTracker.Infrastructure # Внешние API, Экспорт (PDF/Docx), Cryptography
│   └── Modsen.FinanceTracker.UI             # Интерактивное консольное меню (Spectre.Console)
└── tests
    └── FinanceTracker.Tests                 # Unit-тесты для Domain, BLL и DAL слоев
```

## Архитектура системы

Проект строго разделен на 5 слоев в соответствии с принципами чистой архитектуры и инверсии зависимостей (Dependency Inversion):

1.  **Domain (`Modsen.FinanceTracker.Domain`)**: Ядро системы. Содержит бизнес-сущности (`Wallet`, `Transaction`), объекты-значения (`Money`, `TransactionDate`) и доменные события (`CategoryLimitExceededEventArgs`). Не имеет внешних зависимостей.
2.  **BLL (`Modsen.FinanceTracker.BLL`)**: Слой бизнес-логики. Содержит сервисы, координацию агрегатов и декораторы.
3.  **DAL (`Modsen.FinanceTracker.DAL`)**: Слой доступа к данным. Реализует хранение в JSON через абстракции `IRepository` и `IUnitOfWork`.
4.  **Infrastructure (`Modsen.FinanceTracker.Infrastructure`)**: Интеграция со сторонними библиотеками (генерация PDF/Docx, вызовы HTTP к внешнему API валют, хеширование паролей).
5.  **UI (`Modsen.FinanceTracker.UI`)**: Консольный интерфейс, построенный на компонентах `Spectre.Console`.

---

## Паттерны проектирования и принципы 

Кодовая база демонстрирует глубокое понимание паттернов проектирования, используемых для решения конкретных бизнес-задач из ТЗ:

### 1. Domain-Driven Design (DDD) & Result Pattern
*   **Value Objects**: Для базовых типов данных созданы объекты-значения (`Money`, `TransactionDescription`, `TransactionDate`). Это переносит валидацию на уровень системы типов: невозможно создать транзакцию с отрицательным весом или пустой строкой описания.
*   **Отказ от исключений в бизнес-логике**: Внедрен паттерн `Result` / `Result<T>` (Railway-Oriented Programming). Бизнес-ошибки возвращаются как объекты, что делает поток управления предсказуемым и безопасным.

### 2. Структурные паттерны (Structural)
*   **Decorator**: Использован для прозрачного расширения функционала сервисов без изменения их кода. 
    *   `*ServiceLoggingDecorator` логирует все вызовы методов в Serilog.
    *   `CachedCurrencyService` добавляет кэширование (MemoryCache) поверх HTTP-вызовов к внешнему API курсов валют.

### 3. Поведенческие паттерны (Behavioral)
*   **Strategy**: Применен для реализации системы отчетов. Интерфейс `IExportStrategy` имеет конкретные реализации: `CsvExportStrategy`, `TxtExportStrategy`, `PdfExportStrategy` и `DocxExportStrategy`. Добавление нового формата экспорта не требует модификации существующего кода (Open-Closed Principle).
*   **Observer (Events)**: Реализована событийная модель. `FinanceService` публикует событие `OnCategoryLimitExceeded`, на которое подписывается UI-слой, чтобы отобразить предупреждение о превышении бюджета, сохраняя слабую связность (Loose Coupling).

### 4. Порождающие паттерны (Creational)
*   **Factory Method**: `TransactionFactory` скрывает логику инициализации конкретных типов транзакций (`IncomeTransaction`, `ExpenseTransaction`).
*   **Singleton**: Использован для безопасного доступа к глобальной конфигурации приложения `AppConfiguration`.

### 5. Паттерны доступа к данным
*   **Repository & Unit of Work**: Логика работы с JSON-файлом (`JsonDbContext`) скрыта за абстракциями. Текущая in-memory/JSON реализация может быть легко заменена на Entity Framework Core и реляционную БД без изменения BLL и Domain слоев.

---

## Реализованный функционал

Система полностью покрывает базовые и расширенные требования технического задания:

*   **Мультивалютные кошельки**: Поддержка различных базовых валют с автоматической конвертацией при запросе баланса через внешнее API (Open ER-API).
*   **Транзакции и категории**: Полный CRUD для доходов и расходов. Контроль ухода баланса в отрицательную зону. Глобальные лимиты по категориям.
*   **Аналитика**: Полнотекстовый поиск, фильтрация по датам и визуализация распределения расходов в процентах.
*   **Планировщик (Recurring Tasks)**: Модуль `SchedulerService` позволяет настраивать периодические списания (подписки, аренда). Транзакции создаются автоматически при запуске приложения, если наступила дата платежа.
*   **Безопасность**: Защита входа в приложение по паролю (проверка осуществляется через сверку с SHA-256 хешем).
*   **Логирование**: Вся активность пользователя и системные исключения записываются в файл `app.log` с помощью Serilog.

---

## Стек технологий

| Компонент / Слой | Инструменты |
| --- | --- |
| **Платформа** | .NET 10 |
| **Архитектура** | Clean Architecture, DDD, SOLID |
| **Пользовательский интерфейс** | Spectre.Console |
| **Логирование** | Serilog, Serilog.Sinks.File |
| **Экспорт отчетов** | QuestPDF (PDF), DocX (Word) |
| **Интеграция** | `HttpClient`, `System.Text.Json` |
| **Внедрение зависимостей** | `Microsoft.Extensions.DependencyInjection` |
| **Тестирование** | xUnit, NSubstitute, FluentAssertions |

---

## Локальное развертывание

1. Склонируйте репозиторий:
   ```bash
   git clone https://github.com/ВашПользователь/romansevryuk375-modsenfinancetracker.git
   cd romansevryuk375-modsenfinancetracker
   ```

2. Соберите проект:
   ```bash
   dotnet build
   ```

3. Запустите тесты для проверки целостности бизнес-логики:
   ```bash
   dotnet test
   ```

4. Запустите приложение:
   ```bash
   dotnet run --project src/Modsen.FinanceTracker.UI
   ```

*Примечание: При первом запуске будет предложено ввести пароль. Пароль по умолчанию зашит в конфигурации в виде хеша. (admin123)*

---