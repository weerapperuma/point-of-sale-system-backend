**Table of contents**

- **Purpose**
- **Prerequisites**
- **Quick start** (build & run)
- **Recommended folder structure** and explanations
- **Database & migrations**
- **Debugging / Running in VS Code or Visual Studio**
- **Tests**
- **Contributing**

---

**Purpose**

This codebase implements a backend API for a Point of Sale (POS) system. It is split into separate layers so business rules are isolated from infrastructure and the web API surface.

---

**Prerequisites**

- **.NET SDK**: This project targets `net10.0` (check `TargetFramework` in `.csproj`). Install the .NET 10 SDK from https://dotnet.microsoft.com.
- **Git** for source control.
- **Optional but helpful**: VS Code or Visual Studio, Docker (if you want to containerize), and `dotnet-ef` for EF Core migrations.

Install `dotnet-ef` (global tool) if you will run migrations locally:

```powershell
dotnet tool install --global dotnet-ef
```

Note for Windows PowerShell: when running cross-environment commands below, use PowerShell syntax (examples provided).

---

**Quick start — build and run (PowerShell)**

1. Restore packages and build:

```powershell
dotnet restore
dotnet build --configuration Debug
```

2. Run the API (choose one):

- Run the top-level project (if this is the executable entry):

```powershell
dotnet run --project .\Point_of_Sale_System.csproj
```

- Or run the API project (project files are grouped under `/src`):

```powershell
dotnet run --project .\src\POS.Api
```

If you need to set the environment to Development in PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'; dotnet run --project .\src\POS.Api
```

Open your browser to the printed URL (usually `https://localhost:5001` or `http://localhost:5000`) and visit `/swagger/index.html` to view API docs if Swagger is enabled.

---

**Recommended folder structure (explanation for beginners)**

This repository already follows a layered approach. The recommended, clear layout is:

```
/ (repo root)
  README.md
  Point_of_Sale_System.slnx (solution)
  Point_of_Sale_System.csproj (optional root project)
  /src
    /POS.Api                # Web API project (controllers, startup, Program.cs)
    /POS.Application        # Application layer (commands, queries, DTOs, handlers)
    /POS.Domain             # Domain entities, value objects, enums, domain events
    /POS.Infrastructure     # DB, repositories, EF Core, external services
  /tests                   # Unit & integration tests (e.g. POS.Application.Tests)
  /docs                    # Design docs, ER diagrams, API specs
  /scripts                 # Helper scripts (migrations, db seeds, CI helpers)
  /docker                  # Dockerfiles and compose files (optional)

```

Why this layout?

- **Separation of concerns:** Each folder has a focused responsibility (business logic vs persistence vs API surface).
- **Easier to test:** Tests can target the Application and Domain layers without starting the web host.
- **Better for teams:** Teams can work on API, domain logic, or infra independently.

If you plan to keep everything at repo root currently, grouping them under `/src` and adding a `/tests` folder is a small, helpful reorganization.

---

**What each folder contains (beginner-friendly)**

- `POS.Domain`: Plain C# classes representing business concepts (Product, Invoice). No EF, no controllers — just the rules.
- `POS.Application`: Use-cases and business workflows (e.g., CreateInvoiceHandler). This layer talks to interfaces (e.g., `IProductRepository`) not to EF directly.
- `POS.Infrastructure`: EF Core DbContext, repository implementations, migrations, external integrations (email, file storage).
- `POS.Api`: Controllers, DTO mappings, startup configuration, authentication, and swagger endpoints.
- `tests`: Unit tests for Application & Domain and integration tests that may run against an in-memory or test database.

---

**Real-World Example: Create Invoice Flow**

Scenario: A cashier creates an invoice for a customer buying two products.

Request (from client app):

```http
POST /api/invoices/create
Content-Type: application/json

{
  "customerId": 10,
  "items": [
   { "productId": 1, "qty": 2 },
   { "productId": 5, "qty": 1 }
  ]
}
```

Step-by-step flow (what happens inside the app):

1. API Layer (`POS.Api`) — `InvoiceController` receives the request and maps the JSON to a command or DTO, then calls the Application layer.
2. Application Layer (`POS.Application`) — `CreateInvoiceHandler`:
  - Validates the request (required fields, quantities > 0).
  - Loads product details from `IProductRepository` and verifies stock availability.
  - Calculates line totals, taxes, and invoice total.
  - Reduces product quantities (domain operations) and prepares domain `Invoice` entity.
  - Calls repositories (`IInvoiceRepository`, `IUnitOfWork`) to persist changes.
3. Domain Layer (`POS.Domain`) — Entities such as `Invoice` and `Product` enforce business rules (for example, disallow selling more than available stock).
4. Infrastructure Layer (`POS.Infrastructure`) — Concrete repository implementations use `ApplicationDbContext` (EF Core) to save data and apply a transaction if using `IUnitOfWork`.
5. API Layer — Handler returns an outcome; the controller returns an HTTP response with `201 Created` or an error code and a body (for example, `{ "invoiceId": 1005 }`).

Visual flow:

```
CLIENT
  ↓
API (InvoiceController)
  ↓
APPLICATION (CreateInvoiceHandler)
  ↓
DOMAIN (Invoice, Product rules)
  ↓
INFRASTRUCTURE (Repositories → EF Core → DB)
```

Notes & best practices:

- Validate idempotency for create endpoints (avoid duplicate invoices on retries).
- Use transactions (unit of work) when changing multiple aggregates (invoice + stock adjustments).
- Return meaningful errors for validation failures (HTTP 400) and server errors (HTTP 500).
- Consider background tasks for non-blocking work (printing receipts, sending notifications).

---

**Database & migrations**

Typical workflow to create and apply EF Core migrations (adjust project names if your projects have explicit `.csproj` names):

1. Ensure `dotnet-ef` is installed (see earlier).
2. Add a migration (example):

```powershell
dotnet ef migrations add InitialCreate --project .\src\POS.Infrastructure --startup-project .\src\POS.Api
```

3. Apply migrations to the database:

```powershell
dotnet ef database update --project .\src\POS.Infrastructure --startup-project .\src\POS.Api
```

Notes:

- `--project` points to the project that contains your `DbContext` (usually `POS.Infrastructure`).
- `--startup-project` points to the project that contains the app startup (usually `POS.Api`).
- If your projects have explicit `.csproj` file names (for example `POS.Infrastructure.csproj`), you can pass the path to that file instead of the folder.

If you are unsure which project is which, open the `.csproj` files and look for `<TargetFramework>` and package references like `Microsoft.EntityFrameworkCore`.

---

**Running and debugging in VS Code**

- Open the repository: `code .`
- Add the C# extension (`ms-dotnettools.csharp`) if you haven't already.
- Press F5 to run the project selected in launch settings, or use the Run/Debug side panel and select the `POS.Api` (or solution) configuration.

**Running in Visual Studio**

- Open the solution file (`Point_of_Sale_System.slnx`) and set the API project as the startup project, then debug (F5).

---

**Tests**

If the repo contains a `/tests` folder, run:

```powershell
dotnet test
```

This will discover and run all tests in the workspace.

---

**Common commands (PowerShell friendly)**

- Restore packages: `dotnet restore`
- Build: `dotnet build --configuration Debug`
-- Run API: `dotnet run --project .\src\POS.Api`
-- Run a specific project: `dotnet run --project .\Point_of_Sale_System.csproj`
- Run tests: `dotnet test`
-- Add migration: `dotnet ef migrations add <Name> --project .\src\POS.Infrastructure --startup-project .\src\POS.Api`
-- Apply migrations: `dotnet ef database update --project .\src\POS.Infrastructure --startup-project .\src\POS.Api`

---

**Contributing**

- Fork & branch: create a feature branch for changes.
- Write tests for new behavior.
- Keep changes small and focused.
- Update this README with any new setup steps your change requires.

---
