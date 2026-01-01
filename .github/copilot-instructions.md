# Contest App API - Copilot Instructions

## Architecture Overview

This is a .NET 8 Web API using a **feature-based architecture** with strict separation of concerns:

- **App** (`src/App/`): Single project containing all application layers
- **Common** (`src/App/Common/`): Shared components, base services, and infrastructure
- **Features** (`src/App/Features/`): Feature modules organized by domain area
- **IntegrationTests** (`tests/IntegrationTests/`): Tests using `TestWebApplicationFactory`

### Feature Module Structure

Each feature module in `src/App/Features/` (e.g., `Problems/`, `Auth/`, `Tags/`) contains:

```
Tags/
├── Controllers/        # HTTP controllers and request/response models
├── Domain/             # Domain entities and value objects
├── Infrastructure/     # EF configurations and external service implementations
├── Logic/              # Business services and application logic
│   ├── Inputs/         # Input records for services
│   ├── Ports/          # Interface definitions (repositories, external services)
│   └── TagsService.cs  # Service implementation
└── Dependencies.cs     # DI registration for the feature
```

### Key Architectural Patterns

1. **Repository Pattern**: Interfaces in `Logic/Ports/`, EF implementations in `Infrastructure/`
   - Example: `TagsRepository` interface → `EfTagsRepository` implementation
   - Repositories define domain operations, not generic CRUD

2. **Service Layer**: Services in `Logic/` inherit from marker interface `Service`
   - Services accept `Input` records (e.g., `CreateTagInput`) from `Logic/Inputs/` subdirectory
   - Services coordinate repositories and domain logic
   - Example: `TagsService` handles tag creation with slug generation

3. **Unit of Work**: `UnitOfWork` interface implemented by `AppDbContext`
   - All persistence changes saved explicitly via `uow.SaveChangesAsync()`
   - No auto-save in repositories

4. **Dependency Injection**: 
   - Common services registered in `Common/Dependencies.cs` using extension method `SetUpCommon()`
   - Feature services registered in each `Features/[Feature]/Dependencies.cs` using extension method `SetUp[Feature]()`
   - All combined via `Features/Dependencies.cs` using `SetUpFeatures()`
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
This runs `dotnet ef migrations add` with the App project.

**Removing migrations (dev only):**
```bash
npm run remove:migration:dev
```

**Important**: Migrations live in `src/App/Migrations/` and run from the single App project.

### Code Formatting

This project uses **CSharpier** (required IDE setup):
```bash
npm run check    # Verify formatting
```
No format command exists—configure CSharpier in your IDE.

## Coding Conventions

### Namespace Structure

Namespaces follow folder structure exactly:
- `App.Features.Problems.Domain` → `src/App/Features/Problems/Domain/`
- `App.Features.Tags.Logic.Inputs` → `src/App/Features/Tags/Logic/Inputs/`

### Entity Conventions

Entities are POCOs with:
- `Guid Id { get; init; } = Guid.NewGuid();` for primary keys
- `required` keyword for non-nullable properties
- EF configuration via `IEntityTypeConfiguration<T>` in `Infrastructure/`

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

Service inputs are **immutable records** in `Logic/Inputs/`:
```csharp
public record CreateTagInput(string Title);
```

### Controller Patterns

Controllers in `Features/[Feature]/Controllers/`:
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

Custom exceptions in `App.Common.Logic.Exceptions/`:
- `NoSuch` → mapped to 404 in `StatusCodesConfiguration.cs`
- Uses `Hellang.Middleware.ProblemDetails` for RFC 7807 responses

### Testing

The project uses **xUnit** for testing with two distinct testing approaches:

#### Test Structure

Tests are organized in `tests/Tests/` with the following structure:

```
tests/Tests/
├── Features/                    # Feature-specific tests
│   ├── [Feature]/
│   │   ├── IntegrationTests/    # Full-stack tests with real database
│   │   └── UnitTests/           # Isolated unit tests with stubs
├── Tools/
│   ├── IntegrationTests/        # Integration test infrastructure
│   ├── UnitTests/               # Unit test utilities
│   └── OutputTracking/          # Test output tracking utilities
└── Common/                      # Shared test utilities
```

#### Integration Tests

Integration tests inherit from `IntegrationTestBase<TService>` and test features end-to-end:

- **Database**: Uses `TestWebApplicationFactory` with real PostgreSQL database
- **Seeding**: Override `Seed(AppDbContext ctx)` for test data setup
- **Scoping**: Use `UseScope()` to get scoped services and database context
- **Cleanup**: Database automatically reset after each test via `ResetDatabase()`
- **Collection**: Marked with `[Collection("IntegrationTests")]` for proper test isolation

Example integration test:
```csharp
[Collection("IntegrationTests")]
public class TagsFeatureTests(TestWebApplicationFactory factory)
    : IntegrationTestBase<TagsService>(factory)
{
    protected override async Task Seed(AppDbContext ctx)
    {
        await ctx.Tags.AddAsync(new Tag { Title = "Test", Slug = "test" });
    }

    [Fact]
    public async Task testFind()
    {
        using var scope = UseScope();
        var result = await scope.Service.Find(new FindTagInput("test"));
        result.ShouldNotBeNull();
    }
}
```

#### Unit Tests

Unit tests use **stub implementations** for isolated testing:

- **Stubs**: Located in each feature's `Logic/Stubs/` directory (e.g., `StubTagsRepository`)
- **Output Tracking**: Use `OutputTracker<T>` to verify repository operations
- **Test Data**: Use in-memory collections for predictable test scenarios
- **Service Creation**: Manually wire dependencies with stubs and trackers

##### Unit Testing Principles

Follow these core principles when writing unit tests:

1. **Test Responsibility, Not Implementation**: Tests should verify the behavior and responsibility of the tested service, not the internal implementation details of dependencies.

2. **Contract Testing**: Test the contract with dependencies to catch important API changes, but avoid creating brittle tests that break with minor implementation changes.

3. **Focus Areas**: Concentrate testing efforts on:
   - Business logic and domain rules
   - Service orchestration between dependencies
   - Data mapping and transformations
   - Error handling and edge cases

4. **Repository Stub Behavior**: In stub repository implementations:
   - Mutating methods (Create, Update, Delete) should **only** add events to the tracker
   - **Do not** modify the underlying data collections in stubs
   - Let the test setup define the initial state through collections

5. **Test Data Access**: Use direct references to collection elements in tests (e.g., `existingTags[0].Slug`) instead of creating additional constants or helper properties.

Example unit test:
```csharp
public class TagsServiceTests
{
    private readonly List<Tag> existingTags = [/* test data */];

    [Fact] 
    public async Task testCreate()
    {
        var service = CreateServiceWithTracker(out var tracker);
        var tag = await service.Create(new CreateTagInput("New Tag"));
        
        var createdTags = GetCreatedTags(tracker);
        createdTags.Count.ShouldBe(1);
    }

    private TagsService CreateServiceWithTracker(out OutputTracker<TagsTrackerEvent> tracker)
    {
        var repo = new StubTagsRepository(existingTags);
        tracker = repo.GetTracker();
        return new TagsService(SlugGenerator.CreateNull(), repo, new StubUnitOfWork(tracker));
    }
}
```

#### Test Utilities

- **Shouldly**: Preferred assertion library for fluent assertions
- **Global Usings**: xUnit globally imported via `GlobalUsings.cs`
- **Output Tracking**: Custom tracking system to verify stub operations
- **Null Services**: Services provide `CreateNull()` methods for testing

## Web Layer Configuration

`Program.cs` uses extension methods for setup:
- `builder.SetUpOptions()` - Configuration binding
- `builder.Services.SetUpCommon()` - Core dependencies
- `builder.Services.SetUpOpenApi()` - Swagger/Scalar
- `builder.Services.SetUpAuth()` - ASP.NET Identity

## Important Notes

- **Do not** create repositories in `Common/`—they belong in feature modules
- **Do not** save changes in repositories—use `UnitOfWork` in services
- Service methods should be async and accept `CancellationToken` where applicable
- Use primary constructors for dependency injection (C# 12 feature)
- OpenAPI docs available at `/docs` (Scalar UI)
- **Avoid code comments**—use them only when absolutely necessary (which is rare). Focus on writing self-documenting code with clear naming and structure
