# Contest App API - Copilot Instructions

## Architecture Overview

This is a .NET 8 Web API using a **layered architecture** with strict separation of concerns:

- **Core** (`src/Core/`): Domain logic, entities, services, and infrastructure implementations
- **Web** (`src/Web/`): HTTP layer, controllers, authentication, and OpenAPI configuration
- **IntegrationTests** (`tests/IntegrationTests/`): Tests using `TestWebApplicationFactory`

### Core Layer Structure

The Core project is organized by **feature modules** (e.g., `Problems/`, `Auth/`), each containing:

```
Problems/
├── Application/
│   ├── Ports/          # Interfaces (repositories, services)
│   └── Services/       # Business logic with Inputs/ subdirectory
├── Entities/           # Domain models
└── Infrastructure/     # Implementations (repositories, EF configurations)
```

### Key Architectural Patterns

1. **Repository Pattern**: Interfaces in `Application/Ports/`, EF implementations in `Infrastructure/Persistence/`
   - Example: `TagsRepository` interface → `EfTagsRepository` implementation
   - Repositories define domain operations, not generic CRUD

2. **Service Layer**: Services in `Application/Services/[Feature]/` inherit from marker interface `Service`
   - Services accept `Input` records (e.g., `CreateTagInput`) from `Inputs/` subdirectory
   - Services coordinate repositories and domain logic
   - Example: `TagsService` handles tag creation with slug generation

3. **Unit of Work**: `UnitOfWork` interface implemented by `AppDbContext`
   - All persistence changes saved explicitly via `uow.SaveChangesAsync()`
   - No auto-save in repositories

4. **Dependency Injection**: Registered in `Core/Dependencies.cs` using extension method `SetUpCore()`
   - Repositories: Scoped
   - Services: Scoped
   - UnitOfWork: Resolved from `AppDbContext`

## Development Workflows

### Setup and Running

```bash
npm run restore                    # Install dependencies and dotnet tools
npm run start:services:dev         # Start PostgreSQL via docker-compose
npm run push:migration             # Apply EF migrations
npm run start:dev                  # Run with hot reload (dotnet watch)
```

### Database Migrations

**Creating migrations:**
```bash
npm run create:migration -- MigrationName
```
This runs `dotnet ef migrations add` with correct Core/Web project paths.

**Removing migrations (dev only):**
```bash
npm run remove:migration:dev
```

**Important**: Migrations live in `src/Core/Migrations/` but require `--startup-project ./src/Web` because Web has the EF Design package and app configuration.

### Code Formatting

This project uses **CSharpier** (required IDE setup):
```bash
npm run check    # Verify formatting
```
No format command exists—configure CSharpier in your IDE.

## Coding Conventions

### Namespace Structure

Namespaces follow folder structure exactly:
- `Core.Problems.Entities` → `src/Core/Problems/Entities/`
- `Core.Problems.Application.Services.Tags.Inputs` → `src/Core/Problems/Application/Services/Tags/Inputs/`

### Entity Conventions

Entities are POCOs with:
- `Guid Id { get; init; } = Guid.NewGuid();` for primary keys
- `required` keyword for non-nullable properties
- EF configuration via `IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/`

Example from `Tag.cs`:
```csharp
public class Tag
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Slug { get; set; }
}
```

### Service Input Records

Service inputs are **immutable records** in `Services/[Feature]/Inputs/`:
```csharp
public record CreateTagInput(string Title);
```

### Controller Patterns

Controllers in `Web/Controllers/`:
- Use constructor injection for services
- Map HTTP requests to service inputs
- Use `[CheckAuth(Role.Admin, Role.Moderator)]` for role-based authorization
- Return entities directly (serialized automatically)

Example:
```csharp
[HttpPost]
[CheckAuth(Role.Admin, Role.Moderator)]
public async Task<IActionResult> Create([FromBody] CreateTagRequest req)
{
    var input = new CreateTagInput(req.Title);
    var tag = await service.Create(input);
    return CreatedAtAction(nameof(Get), new { slug = tag.Slug }, null);
}
```

### Exception Handling

Custom exceptions in `Core.Common.Application.Exceptions/`:
- `NoSuch` → mapped to 404 in `StatusCodesConfiguration.cs`
- Uses `Hellang.Middleware.ProblemDetails` for RFC 7807 responses

### Testing

Integration tests inherit from `TestBase<TService>`:
- Override `Seed(AppDbContext ctx)` for test data
- Use `UseScope()` to get scoped services
- Database reset after each test via `ResetDatabase()`

## Web Layer Configuration

`Program.cs` uses extension methods for setup:
- `builder.SetUpOptions()` - Configuration binding
- `builder.Services.SetUpCore()` - Core dependencies
- `builder.Services.SetUpOpenApi()` - Swagger/Scalar
- `builder.Services.SetUpAuth()` - ASP.NET Identity

## Important Notes

- **Do not** create repositories in `Common/`—they belong in feature modules
- **Do not** save changes in repositories—use `UnitOfWork` in services
- Service methods should be async and accept `CancellationToken` where applicable
- Use primary constructors for dependency injection (C# 12 feature)
- OpenAPI docs available at `/docs` (Scalar UI)
