# Contest App

## Introduction

Contest App is a modern web API built with .NET 8 that provides a platform for managing programming contests and challenges. The application follows clean architecture principles with a clear separation of concerns, making it maintainable and testable.

## Development

### Pre-requisites:
- .NET 8
- Node.js + npm (for commands and helper scripts)

### Setup:
1. Install dependencies:
    ```bash
    npm run restore
    ```
    Installs all dependencies and dotnet tools.
2. Configure [CSharpier](https://csharpier.com/docs/Editors) in your IDE
3. Run services (db, redis, etc.):
    ```bash
    npm run start:services:dev
    ```
4. Push migrations:
    ```bash
    npm run push:migration
    ```
5. Run the app in dev mode:
    ```bash
    npm run start:dev
    ```
6. Open [api docs](http://localhost:5163/docs)

### Helpful commands:
- `npm run check` - runs csharpier check
- `npm run create:migration -- MigrationName` - creates a new migration
- `npm run remove:migration:dev` - removes the latest migration
- `npm run test` - pretty self-explanatory

"dev" keyword in commands indicates that it should NOT be run in production

### Commit conventions:
Use simple, clear commit messages in past tense:
- `added X` - when adding new features or files
- `fixed X` - when fixing bugs or issues
- `updated X` - when modifying existing features
- `removed X` - when deleting code or features
- `refactored X` - when restructuring code without changing behavior

Examples:
- `added problems search endpoint`
- `fixed tags repository null handling`
- `updated auth middleware error messages`

## App quick start
In progress...