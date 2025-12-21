# CLAUDE.md - ABCBank Project Guide

## Project Overview

**ABCBank** is a banking application built with .NET 10.0 following Clean Architecture principles. The system manages account holders, accounts, and transactions with a focus on maintainability, testability, and separation of concerns.

### Technology Stack
- **.NET 10.0** - Target framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 10.0** - ORM with SQL Server
- **MediatR 12.5.0** - CQRS pattern implementation
- **Mapster 7.4.0** - Object-to-object mapping
- **Swashbuckle/OpenAPI** - API documentation

---

## Architecture Overview

The solution follows **Clean Architecture** with clear separation of concerns across 5 projects:

```
ABCBank/
├── Domain/          # Core business entities and contracts
├── Application/     # Business logic, CQRS handlers, interfaces
├── Infrastructure/  # Data access, repositories, EF Core context
├── Common/          # Shared DTOs, enums, wrappers
└── WebApi/          # API controllers, entry point, DI configuration
```

### Dependency Flow
```
WebApi → Infrastructure → Application → Domain → Common
```

**Critical Rule**: Dependencies flow inward only. Domain and Common have no external dependencies.

---

## Layer Responsibilities

### 1. Domain Layer (`Domain/`)
**Purpose**: Core business entities and domain contracts

**Contents**:
- Entity classes (AccountHolder, Account, Transaction, Person)
- Base contracts (`BaseEntity<TId>`, `IEntity<TId>`)
- Domain models with business properties

**Key Files**:
- `Contracts/BaseEntity.cs` - Generic base entity with Id
- `Contracts/IEntity.cs` - Entity interface
- `AccountHolder.cs` - Extends Person, has Accounts collection
- `Account.cs` - Banking account entity
- `Transaction.cs` - Transaction entity
- `Person.cs` - Base person entity

**Dependencies**: Only Common project

**Conventions**:
- All entities inherit from `BaseEntity<TId>` (typically `BaseEntity<int>`)
- Use navigation properties for relationships
- Keep domain logic pure (no infrastructure concerns)

### 2. Application Layer (`Application/`)
**Purpose**: Business logic, CQRS implementation, repository interfaces

**Structure**:
```
Application/
├── Features/
│   └── AccountHolders/
│       ├── Command/
│       │   └── CreateAccountHolderCommand.cs
│       └── Queries/  (empty - ready for query handlers)
└── Repositories/
    ├── IReadRepositoryAsync.cs
    ├── IWriteRepositoryAsync.cs
    └── IUnitOfWork.cs
```

**Key Patterns**:
- **CQRS**: Commands and Queries separated under `Features/`
- **MediatR**: All commands implement `IRequest<ResponseWrapper<T>>`
- **Mapster**: DTOs map to domain entities using `.Adapt<T>()`
- **Repository Pattern**: Generic repositories with Unit of Work

**Command Structure Example**:
```csharp
public class CreateAccountHolderCommand : IRequest<ResponseWrapper<int>>
{
    public CreateAccountHolder CreateAccountHolder { get; set; }
}

public class CreateAccountHolderCommandHandler : IRequestHandler<CreateAccountHolderCommand, ResponseWrapper<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public async Task<ResponseWrapper<int>> Handle(CreateAccountHolderCommand request, CancellationToken cancellationToken)
    {
        var entity = request.CreateAccountHolder.Adapt<AccountHolder>();
        await _unitOfWork.WriteRepositoryFor<AccountHolder>().AddAsync(entity);
        await _unitOfWork.CommitAsync(cancellationToken);
        return new ResponseWrapper<int>().Success(entity.Id, "Success message");
    }
}
```

**Dependencies**: Domain, Common, MediatR, Mapster

### 3. Infrastructure Layer (`Infrastructure/`)
**Purpose**: Data access implementation, EF Core context, migrations

**Structure**:
```
Infrastructure/
├── Contexts/
│   ├── ApplicationDbContext.cs
│   └── BankDbConfigurations.cs
├── Repositories/
│   ├── ReadRepositoryAsync.cs
│   ├── WriteRepositoryAsync.cs
│   └── UnitOfWork.cs
├── Migrations/
│   └── 20251206163325_InitialDb.cs
└── Startup.cs  (DI registration)
```

**Key Files**:
- `Contexts/ApplicationDbContext.cs` - Main DbContext
  - Configures decimal precision to `decimal(18,2)` globally
  - Applies configurations from assembly
  - DbSets: AccountHolders, Accounts, Transactions

- `Startup.cs` - Extension methods for DI:
  - `AddDatabase()` - Registers DbContext with SQL Server
  - `AddRepositories()` - Registers generic repositories and UnitOfWork

**Database Configuration**:
- Connection string: `DefaultConnection` from appsettings.json
- Provider: SQL Server
- EF Core 10.0 with migrations

**Dependencies**: Application (for interfaces), EF Core packages

### 4. Common Layer (`Common/`)
**Purpose**: Shared contracts, DTOs, enums, response wrappers

**Structure**:
```
Common/
├── Requests/
│   └── AccountHolder.cs  (CreateAccountHolder, UpdateAccountHolder records)
├── Enums/
│   ├── AccountType.cs  (Cheque, Current, Savings)
│   └── TransactionType.cs  (Deposit, Withdrawal)
├── Wrapper/
│   └── ResponseWrapper.cs
└── Responses/  (folder exists, ready for response DTOs)
```

**Response Wrapper Pattern**:
```csharp
public class ResponseWrapper<T>
{
    public bool IsSuccessful { get; set; }
    public List<string> Messages { get; set; }
    public T Data { get; set; }

    public ResponseWrapper<T> Success(T data, string message = null);
    public ResponseWrapper<T> Failed(string message = null);
}
```

**Enums**:
- Use `byte` as underlying type for efficiency
- AccountType: Cheque, Current, Savings
- TransactionType: Deposit, Withdrawal

**DTOs**:
- Use `record` types for immutability
- Example: `CreateAccountHolder(string FirstName, string LastName, DateTime DateOfBirth, string ContactNumber, string Email)`

**Dependencies**: None

### 5. WebApi Layer (`WebApi/`)
**Purpose**: HTTP API endpoints, dependency injection setup, middleware configuration

**Key Files**:
- `Program.cs` - Application entry point
  - Configures services (Controllers, OpenAPI, Swagger)
  - Calls `AddDatabase()` and `AddRepositories()` from Infrastructure
  - Development environment: Swagger UI with dual endpoints

**Configuration**:
- ImplicitUsings: disabled (explicit usings required)
- Nullable: enabled
- Swagger endpoints:
  - `/swagger/v1/swagger.json` - Swashbuckle
  - `/openapi/v1/openapi.json` - Microsoft OpenAPI

**Dependencies**: Infrastructure (transitively includes all layers)

---

## Development Conventions

### Naming Conventions

1. **Files and Classes**:
   - PascalCase for all files, classes, properties, methods
   - Match file name to primary class name
   - Entities: Singular nouns (`Account`, not `Accounts`)
   - Commands: `{Verb}{Entity}Command` (e.g., `CreateAccountHolderCommand`)
   - Handlers: `{CommandName}Handler`

2. **Folders**:
   - PascalCase for all folders
   - Feature folders by entity plural (e.g., `AccountHolders/`)
   - Separate Command and Queries folders under each feature

3. **Variables**:
   - camelCase for local variables and parameters
   - Private fields: `_camelCase` with underscore prefix
   - Avoid abbreviations unless well-known (e.g., `id` is acceptable)

### Code Organization

1. **Feature-Based Organization**:
   - Group by feature/entity in `Application/Features/`
   - Each feature has Command and Queries subfolders
   - One command/query per file

2. **Repository Pattern**:
   - Use generic repositories: `IReadRepositoryAsync<T, TId>`, `IWriteRepositoryAsync<T, TId>`
   - Access via UnitOfWork: `_unitOfWork.WriteRepositoryFor<AccountHolder>()`
   - Always commit changes: `await _unitOfWork.CommitAsync(cancellationToken)`

3. **Response Handling**:
   - All MediatR handlers return `ResponseWrapper<T>`
   - Use `.Success(data, message)` for successful operations
   - Use `.Failed(message)` for failures
   - Generic type T is typically the entity Id (int) for commands

4. **Entity Mapping**:
   - Use Mapster's `.Adapt<T>()` for DTO to entity conversion
   - Configure complex mappings in a dedicated configuration class if needed

### CQRS Pattern Implementation

**Commands** (modify state):
```csharp
// Location: Application/Features/{Entity}/Command/
public class {Verb}{Entity}Command : IRequest<ResponseWrapper<TResult>>
{
    public {RequestDTO} {PropertyName} { get; set; }
}

public class {Verb}{Entity}CommandHandler : IRequestHandler<{Command}, ResponseWrapper<TResult>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    // Constructor injection
    public async Task<ResponseWrapper<TResult>> Handle({Command} request, CancellationToken cancellationToken)
    {
        // 1. Map DTO to entity
        // 2. Call repository
        // 3. Commit changes
        // 4. Return ResponseWrapper
    }
}
```

**Queries** (read state):
```csharp
// Location: Application/Features/{Entity}/Queries/
// Similar structure but use ReadRepository, no commit needed
```

### Entity Framework Conventions

1. **DbContext**:
   - Use DbSet properties with `=> Set<T>()` syntax
   - Configure decimal precision globally in `OnModelCreating`
   - Apply configurations via `modelBuilder.ApplyConfigurationsFromAssembly()`

2. **Migrations**:
   - Migration naming: Descriptive of changes (e.g., `InitialDb`)
   - Run from Infrastructure project directory
   - Commands:
     ```bash
     dotnet ef migrations add {MigrationName} --project Infrastructure
     dotnet ef database update --project Infrastructure
     ```

3. **Entity Configuration**:
   - Create `IEntityTypeConfiguration<T>` classes in `Contexts/` folder
   - Keep DbContext clean, move complex configs to separate classes

---

## Development Workflows

### Adding a New Feature

1. **Define the Domain Entity** (if needed):
   ```bash
   # Location: Domain/{EntityName}.cs
   # Inherit from BaseEntity<int>
   # Add navigation properties
   ```

2. **Create Request/Response DTOs**:
   ```bash
   # Location: Common/Requests/{EntityName}.cs
   # Use record types for DTOs
   # Location: Common/Responses/{EntityName}.cs (if custom response needed)
   ```

3. **Add Command/Query**:
   ```bash
   # Location: Application/Features/{EntityName}s/Command/{Verb}{EntityName}Command.cs
   # Implement IRequest<ResponseWrapper<T>>
   # Add handler class in same file
   ```

4. **Register in DbContext** (if new entity):
   ```csharp
   // Infrastructure/Contexts/ApplicationDbContext.cs
   public DbSet<{Entity}> {Entities} => Set<{Entity}>();
   ```

5. **Create Migration**:
   ```bash
   cd Infrastructure
   dotnet ef migrations add Add{EntityName} --startup-project ../WebApi
   dotnet ef database update --startup-project ../WebApi
   ```

6. **Add Controller Endpoint**:
   ```bash
   # Location: WebApi/Controllers/{EntityName}Controller.cs
   # Inject IMediator
   # Send command via mediator
   ```

### Adding a New Command Example

**Scenario**: Add UpdateAccountHolder command

1. Create command file: `Application/Features/AccountHolders/Command/UpdateAccountHolderCommand.cs`
2. Implement command and handler:
```csharp
public class UpdateAccountHolderCommand : IRequest<ResponseWrapper<int>>
{
    public UpdateAccountHolder UpdateAccountHolder { get; set; }
}

public class UpdateAccountHolderCommandHandler : IRequestHandler<UpdateAccountHolderCommand, ResponseWrapper<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public UpdateAccountHolderCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseWrapper<int>> Handle(UpdateAccountHolderCommand request, CancellationToken cancellationToken)
    {
        var accountHolder = await _unitOfWork.ReadRepositoryFor<AccountHolder>()
            .GetByIdAsync(request.UpdateAccountHolder.Id);

        if (accountHolder == null)
            return new ResponseWrapper<int>().Failed("Account holder not found");

        // Map updates
        accountHolder.FirstName = request.UpdateAccountHolder.FirstName;
        // ... map other properties

        await _unitOfWork.WriteRepositoryFor<AccountHolder>().UpdateAsync(accountHolder);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ResponseWrapper<int>().Success(accountHolder.Id, "Updated successfully");
    }
}
```

### Adding a New Query Example

**Scenario**: Get AccountHolder by Id

1. Create query file: `Application/Features/AccountHolders/Queries/GetAccountHolderByIdQuery.cs`
2. Create response DTO in `Common/Responses/` if needed
3. Implement query and handler using `ReadRepositoryFor<T>()`

---

## Common Tasks for AI Assistants

### When Adding New Entities

1. **Always** create the entity in Domain layer first
2. **Always** inherit from `BaseEntity<TId>`
3. Add to DbContext and create migration
4. Create corresponding request/response DTOs in Common
5. Follow the feature folder pattern in Application

### When Implementing CQRS

1. **Commands**: Modify state, return entity Id or void wrapped in ResponseWrapper
2. **Queries**: Read-only, return DTOs wrapped in ResponseWrapper
3. **Always** use UnitOfWork pattern
4. **Always** commit after write operations
5. Use Mapster for DTO ↔ Entity conversion

### When Working with Database

1. **Connection String**: Check `appsettings.json` for `DefaultConnection`
2. **Migrations**: Run from Infrastructure project context
3. **Update Database**: Use `dotnet ef database update`
4. **Decimal Precision**: Automatically configured as `decimal(18,2)`

### When Adding Dependencies

1. Install packages in the appropriate project:
   - Domain: Minimal dependencies (only Common)
   - Application: Business logic packages (MediatR, Mapster)
   - Infrastructure: Data access packages (EF Core)
   - WebApi: ASP.NET packages
2. Update `.csproj` files
3. Register services in `Program.cs` or `Startup.cs` extension methods

### When Creating API Endpoints

1. Create controller in `WebApi/Controllers/`
2. Inject `IMediator` from MediatR
3. Send commands/queries via mediator:
   ```csharp
   var result = await _mediator.Send(new CreateAccountHolderCommand { ... });
   ```
4. Return appropriate HTTP status codes based on ResponseWrapper.IsSuccessful

---

## Project Configuration Details

### Target Framework
- **.NET 10.0** across all projects

### Nullable Reference Types
- **Domain**: Disabled
- **Application**: Disabled
- **Infrastructure**: Disabled
- **Common**: Disabled
- **WebApi**: Enabled

### Implicit Usings
- **Domain**: Enabled
- **Application**: Enabled
- **Infrastructure**: Enabled
- **Common**: Enabled
- **WebApi**: Disabled (explicit usings required)

---

## Domain Model Reference

### Entities Hierarchy

```
BaseEntity<TId>
├── Person (FirstName, LastName, DateOfBirth)
│   └── AccountHolder (ContactNumber, Email, Accounts[])
├── Account (AccountNumber, AccountHolderId, Balance, IsActive, Type, Transactions[])
└── Transaction (AccountId, Type, Amount, Date)
```

### Relationships

- **AccountHolder** → has many **Accounts** (1:N)
- **Account** → belongs to **AccountHolder** (N:1)
- **Account** → has many **Transactions** (1:N)
- **Transaction** → belongs to **Account** (N:1)

---

## Important Notes for AI Assistants

### Do's ✓
- Follow Clean Architecture dependency rules strictly
- Use ResponseWrapper for all API responses
- Implement CQRS pattern with MediatR
- Use Mapster for object mapping
- Access data only through UnitOfWork and repositories
- Create migrations after entity changes
- Keep domain entities pure (no infrastructure dependencies)
- Use feature-based folder organization
- Follow existing naming conventions

### Don'ts ✗
- Don't reference Infrastructure from Domain or Application
- Don't bypass UnitOfWork (no direct DbContext access in handlers)
- Don't forget to call `CommitAsync()` after write operations
- Don't mix commands and queries in the same folder
- Don't create anemic domain models (add behavior when appropriate)
- Don't use `var` when type isn't obvious
- Don't skip migrations after entity changes
- Don't create controllers that directly access DbContext

### Code Quality Standards

1. **Async/Await**: All I/O operations must be async
2. **CancellationToken**: Pass through from controller to repository
3. **Using Statements**: Explicit in WebApi, implicit elsewhere
4. **Error Handling**: Use ResponseWrapper.Failed() for business errors
5. **Validation**: Implement in command handlers or separate validators

---

## Git Workflow

### Current Branch
- **Development Branch**: `claude/claude-md-mjfy2il63ol1y3f2-9tp0a`
- All changes should be committed to this branch
- Push with: `git push -u origin claude/claude-md-mjfy2il63ol1y3f2-9tp0a`

### Commit Message Conventions
- Use descriptive messages in Polish (project language)
- Format: `{Action} {What was done}`
- Examples:
  - `Dodanie CreateAccountHolderCommand i integracja MediatR/Mapster`
  - `Dodano migrację InitialDb i zaktualizowano projekt`

---

## Quick Reference Commands

```bash
# Build solution
dotnet build

# Run API
cd WebApi
dotnet run

# Add migration
cd Infrastructure
dotnet ef migrations add {MigrationName} --startup-project ../WebApi

# Update database
dotnet ef database update --startup-project ../WebApi

# List migrations
dotnet ef migrations list --startup-project ../WebApi

# Restore packages
dotnet restore
```

---

## Questions to Ask Users

When implementing new features, consider asking:

1. **New Entity**: Should it have audit fields (CreatedAt, UpdatedAt)?
2. **Relationships**: What are the navigation properties and cardinality?
3. **Business Rules**: Any validation rules or constraints?
4. **API Design**: What HTTP methods and routes are needed?
5. **Response Format**: Standard ResponseWrapper or custom response?

---

**Last Updated**: 2025-12-21
**Project Version**: .NET 10.0
**Database**: SQL Server with EF Core 10.0
