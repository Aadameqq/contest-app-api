# Contest App

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

"dev" keyword in commands indicates that it should NOT be run in production

## App quick start
In progress...