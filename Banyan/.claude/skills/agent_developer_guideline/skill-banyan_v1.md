---
name: dotnet-layered-banyanfin-developer
description: >
  Senior .NET developer skill for maintaining and extending layered ASP.NET Core
  applications using Razor MVC, Bootstrap 5, jQuery, Entity Framework Core,
  Repository + Unit of Work, and the Banyan.* project structure. Use this
  skill whenever implementing, modifying, reviewing, refactoring, testing, or
  troubleshooting application code in this solution.
---

# .NET Layered URF Developer Skill

## 1. Mission

Act as a senior ASP.NET Core / C# developer working inside a layered solution
inspired by the `urfnet/URF.Core.Sample` architecture.

Reference repository:
https://github.com/urfnet/URF.Core.Sample/tree/master

The application uses:

- ASP.NET Core MVC
- Modern .NET / .NET SDK (use the version already declared by the solution)
- Entity Framework Core
- Razor View Engine (`.cshtml`)
- Bootstrap 5
- jQuery (use the version already configured by the application; upgrade only when explicitly requested)
- Repository Pattern
- Unit of Work Pattern
- Service Layer
- Dependency Injection
- SQL Server / EF Core migrations where applicable
- xUnit or the test framework already used by `Banyan.Test`

The solution is organized into these projects:

```text
Banyan.Data
Banyan.Entity
Banyan.Repository
Banyan.Service
Banyan.Api
Banyan.Web
Banyan.Test
```

The developer must preserve this architecture unless the user explicitly asks
for an architectural change.

---

## 2. Architecture and Project Responsibilities

### Banyan.Entity

Purpose:

- Domain/entity models.
- DTOs and view-independent model classes when appropriate.
- Model-specific metadata/configuration when the existing project uses it.
- No UI logic.
- No controller logic.
- Avoid direct dependency on `Banyan.Web`.

Typical structure:

```text
Banyan.Entity/
├── Models/
│   ├── Category.cs
│   ├── Product.cs
│   └── ...
└── Banyan.Entity.csproj
```

Rules:

- Keep entities focused on domain/data representation.
- Do not put EF queries in entities.
- Do not put Razor/UI concerns in entities.
- Do not reference Web, API, Service, or Repository unless the existing project
  explicitly requires a dependency.

---

### Banyan.Data

Purpose:

- EF Core persistence infrastructure.
- `DbContext`.
- EF Core configuration.
- Database-specific implementation.
- Design-time context factory.
- Migrations, if migrations are kept in this project.

Typical structure:

```text
Banyan.Data/
├── BanyanContext.cs
├── BanyanContextFactory.cs
├── Migrations/
└── Banyan.Data.csproj
```

Rules:

- `BanyanContext` derives from `DbContext`.
- Prefer constructor injection of `DbContextOptions<BanyanContext>`.
- Keep connection strings outside source code.
- Use `appsettings.json`, environment variables, user secrets, or deployment
  configuration as appropriate.
- Do not put business rules in the DbContext.
- Use `AsNoTracking()` for read-only queries when tracking is not required.
- Keep EF Core-specific code in the data/repository boundary.
- Do not create database access directly in controllers.

Example:

```csharp
public class BanyanContext : DbContext
{
    public BanyanContext(
        DbContextOptions<BanyanContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
}
```

For design-time EF operations, use `BanyanContextFactory` when required by
the existing solution.

---

### Banyan.Repository

Purpose:

- Generic repository abstractions and implementations.
- Entity-specific repository extensions when needed.
- Data-access query composition.
- Unit-of-work integration.

Expected core types:

```text
IRepositoryBase.cs
RepositoryBase.cs
Model/
```

URF-style repository concepts should be preserved where they already exist.

Rules:

- Controllers must not access `DbContext` directly.
- Services should normally access repositories/unit of work rather than
  directly querying EF Core.
- Repository methods should represent reusable data-access operations.
- Do not add a repository method for every trivial LINQ expression unless
  abstraction or reuse justifies it.
- Prefer IQueryable exposure only where the existing architecture expects
  composable queries; otherwise return materialized results.
- Do not call `SaveChanges` from individual repository methods when the
  Unit of Work is responsible for transaction persistence.
- Keep persistence concerns here, not in the service or controller layer.

Example conceptual abstraction:

```csharp
public interface IRepositoryBase<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> Queryable();
    Task<TEntity?> FindAsync(params object[] keyValues);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
```

Adapt signatures to the actual URF interfaces already installed in the
solution rather than inventing a second repository abstraction.

---

### Banyan.Service

Purpose:

- Business/application logic.
- Service interfaces.
- Service implementations.
- Validation and orchestration.
- Coordination between repositories/unit of work.
- Transaction boundaries where appropriate.

Expected patterns:

```text
Banyan.Service/
├── IServiceBase.cs
├── ServiceBase.cs
├── ICategoriesService.cs
├── CategoriesService.cs
└── ...
```

Rules:

- Every substantial business operation belongs in a service.
- Prefer interface + implementation:
  `ICategoriesService` -> `CategoriesService`.
- Services should not return `IActionResult`.
- Services should not depend on Razor views.
- Services should not contain HTML or JavaScript.
- Services should not know about HTTP-specific concerns such as
  `HttpContext`, `Request`, `Response`, or MVC routing.
- Use the Unit of Work to coordinate multiple repository changes.
- Save changes once per logical business operation unless a separate
  transaction boundary is explicitly required.
- Keep business validation in services or dedicated validators rather than
  duplicating it across controllers and JavaScript.
- Use asynchronous APIs for database operations.

Example:

```csharp
public interface ICategoriesService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
```

---

### Banyan.Api

Purpose:

- HTTP API endpoints.
- API-specific controllers.
- API request/response contracts.
- API authentication/authorization and middleware when applicable.

Rules:

- API controllers should be thin.
- Inject services through interfaces.
- Do not put EF Core queries directly in API controllers.
- Return appropriate HTTP status codes.
- Validate incoming request models.
- Do not expose internal persistence models unnecessarily.
- Prefer DTOs for public API contracts.
- Keep API-specific behavior out of `Banyan.Web`.

Example flow:

```text
HTTP Request
    ↓
API Controller
    ↓
Service Interface
    ↓
Service
    ↓
Unit of Work / Repository
    ↓
EF Core / DbContext
    ↓
SQL Server
```

---

### Banyan.Web

Purpose:

- Main ASP.NET Core MVC web application.
- MVC controllers.
- Razor views.
- ViewModels.
- Web-specific JavaScript and CSS.
- Layouts, partials, validation UI, and page composition.

Typical structure:

```text
Banyan.Web/
├── Controllers/
├── Views/
│   ├── Shared/
│   └── <ControllerName>/
├── ViewModels/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
├── Areas/
└── Banyan.Web.csproj
```

Important:

`Banyan.Web` is the main web project. All MVC controller and Razor view
work belongs here.

Rules:

- Controllers remain thin.
- Controllers call application services.
- Controllers do not directly use repositories unless the existing
  architecture explicitly requires it.
- Do not put business logic in Razor views.
- Do not put database queries in Razor views.
- Prefer strongly typed ViewModels.
- Keep reusable UI in partial views.
- Keep common page structure in `_Layout.cshtml`.
- Use Razor Tag Helpers and standard ASP.NET Core MVC conventions.
- Use unobtrusive validation where available.
- Keep JavaScript in `wwwroot/js` rather than inline script blocks when the
  script is reusable.
- Use Bootstrap 5 classes and components instead of creating custom CSS for
  functionality Bootstrap already provides.

Typical MVC flow:

```text
Browser
  ↓
Banyan.Web Controller
  ↓
ViewModel / Service
  ↓
Banyan.Service
  ↓
Banyan.Repository
  ↓
Banyan.Data
  ↓
Database

Service result
  ↓
Controller
  ↓
Razor View
  ↓
HTML + Bootstrap + jQuery
```

---

### Banyan.Test

Purpose:

- Unit tests.
- Service tests.
- Repository tests where useful.
- Controller tests where useful.
- Integration tests if the existing project supports them.

Rules:

- Test business behavior rather than implementation details.
- Prefer deterministic tests.
- Mock interfaces at architectural boundaries.
- Do not make ordinary unit tests depend on a live production database.
- Add regression tests for bug fixes when practical.
- Test both success and relevant failure/validation paths.
- Keep tests independent and repeatable.

---

## 3. Dependency Direction

Prefer this dependency direction:

```text
Banyan.Web ───────────────┐
Banyan.Api ───────────────┤
                            ↓
                     Banyan.Service
                            ↓
                    Banyan.Repository
                            ↓
                      Banyan.Data
                            ↓
                     Banyan.Entity
```

`Banyan.Entity` should remain the lowest-level shared domain/model project.

Do not introduce circular project references.

Before adding a project reference, ask:

1. Does this dependency belong at this architectural layer?
2. Can an interface or DTO remove the dependency?
3. Is the dependency required by the existing URF architecture?
4. Will this make testing harder?

If the answer is unclear, inspect the existing `.csproj` files and neighboring
implementations before making the change.

---

## 4. Repository + Unit of Work Rules

The Repository and Unit of Work patterns are mandatory architectural
conventions for this solution.

Use this conceptual flow:

```csharp
var entity = await UnitOfWork.SomeRepository.FindAsync(id);

if (entity == null)
{
    // handle missing entity
}

entity.Name = request.Name;

await UnitOfWork.SaveChangesAsync();
```

Do not write this in a controller:

```csharp
_dbContext.Products
    .Where(...)
    .ToListAsync();
```

Instead, use the service/repository abstraction.

Do not create unnecessary nested `SaveChangesAsync()` calls.

For a multi-entity operation:

```text
Service
  ├── Repository A change
  ├── Repository B change
  └── UnitOfWork.SaveChangesAsync()
```

The service owns the logical business operation; the Unit of Work coordinates
persistence.

Use transactions when a business operation requires atomicity across multiple
changes and the existing infrastructure supports it.

---

## 5. Service Layer Rules

Use service classes for application/business operations.

Preferred naming:

```text
ICategoriesService
CategoriesService

IProductsService
ProductsService

IOrdersService
OrdersService
```

For CRUD operations, keep the service API clear and asynchronous.

Example:

```csharp
public async Task<CategoryDto?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    var entity = await UnitOfWork.CategoriesRepository
        .FindAsync(id);

    return entity == null
        ? null
        : MapToDto(entity);
}
```

Do not blindly add generic CRUD methods to every service if the domain needs
specific business operations.

Prefer meaningful operations such as:

```text
ApproveOrderAsync
CancelOrderAsync
GetActiveCategoriesAsync
SearchProductsAsync
```

over leaking persistence details into callers.

---

## 6. ASP.NET Core MVC Controller Rules

Controllers should coordinate HTTP concerns only.

Good:

```csharp
public async Task<IActionResult> Edit(
    int id,
    CancellationToken cancellationToken)
{
    var model = await _productsService
        .GetEditModelAsync(id, cancellationToken);

    if (model == null)
    {
        return NotFound();
    }

    return View(model);
}
```

Avoid:

```csharp
public async Task<IActionResult> Edit(int id)
{
    var product = await _db.Products
        .Include(x => x.Category)
        .FirstOrDefaultAsync(x => x.Id == id);

    // business logic here...
}
```

Controller responsibilities:

- Read route/query/form input.
- Call services.
- Decide HTTP/MVC result.
- Populate ModelState where appropriate.
- Return View / PartialView / Redirect / NotFound / BadRequest / etc.
- Handle authorization attributes.
- Pass cancellation tokens through to async application operations.

Controller non-responsibilities:

- SQL/EF queries.
- Business rules.
- Complex mapping logic.
- Transaction orchestration.
- HTML generation.
- Direct database mutation.

---

## 7. Razor View Rules

Use strongly typed Razor views:

```cshtml
@model ProductEditViewModel
```

Rules:

- Keep views declarative.
- Avoid complex C# logic in `.cshtml`.
- Do not query databases from views.
- Use ViewModels rather than passing database entities directly when the
  screen has presentation-specific fields.
- Use partial views for reusable sections.
- Use `_ViewImports.cshtml` and `_ViewStart.cshtml` consistently.
- Keep forms compatible with ASP.NET Core model binding.
- Use tag helpers where appropriate:
  `asp-controller`, `asp-action`, `asp-route-*`, `asp-for`, etc.
- Display validation messages using the built-in validation helpers.

For forms:

```cshtml
<div asp-validation-summary="ModelOnly" class="text-danger"></div>

<div class="mb-3">
    <label asp-for="Name" class="form-label"></label>
    <input asp-for="Name" class="form-control" />
    <span asp-validation-for="Name" class="text-danger"></span>
</div>
```

---

## 8. Bootstrap 5 Rules

Use Bootstrap 5 for the primary UI framework.

Rules:

- Use Bootstrap 5 utility classes where practical.
- Prefer semantic Bootstrap components.
- Use responsive layout classes.
- Avoid unnecessary custom CSS.
- Do not mix Bootstrap 3/4 APIs into Bootstrap 5 code.
- Verify the project's actual Bootstrap version before modifying markup.
- Do not assume a CDN is used; follow the existing local/CDN asset strategy.
- Preserve existing theme variables and design tokens when they exist.

For tables:

```html
<div class="table-responsive">
    <table class="table table-striped table-hover align-middle">
        ...
    </table>
</div>
```

For forms:

```html
<div class="mb-3">
    <label class="form-label">Name</label>
    <input class="form-control" />
</div>
```

For buttons:

```html
<button type="submit" class="btn btn-primary">
    Save
</button>
```

Avoid using deprecated Bootstrap APIs without checking the project's version.

---

## 9. jQuery Rules

The application uses jQuery.

Rules:

- Use the version already configured by the project.
- Do not upgrade jQuery merely because a newer version exists.
- Upgrade dependencies only when explicitly requested or required to resolve
  a documented compatibility/security problem.
- Prefer unobtrusive, delegated event handling for dynamically rendered
  content.
- Keep page-specific JavaScript in appropriately named files.
- Avoid large inline `<script>` blocks.
- Use `$(function () { ... });` or the project's established initialization
  convention.
- Avoid global variables.
- Use `data-*` attributes for simple UI configuration.
- Return JSON/HTTP results from endpoints rather than rendering complex HTML
  inside JavaScript when the existing architecture separates UI and server
  concerns.

Example:

```javascript
$(function () {
    $('#productForm').on('submit', function (event) {
        // page-specific behavior
    });
});
```

For dynamic elements:

```javascript
$(document).on('click', '[data-action="delete"]', function () {
    // delegated event
});
```

---

## 10. AJAX / JSON Endpoints

When Razor pages need asynchronous operations:

```text
Razor View
   ↓
jQuery AJAX
   ↓
Banyan.Web Controller
   ↓
Service
   ↓
Repository / Unit of Work
```

Rules:

- Validate input server-side even if client-side validation exists.
- Return appropriate HTTP status codes.
- Return small DTOs or explicit JSON contracts.
- Do not expose EF tracking entities unnecessarily.
- Handle error responses consistently.
- Do not duplicate business logic in JavaScript.
- Protect state-changing requests against CSRF/antiforgery attacks.

For MVC POST/AJAX operations, use the application's antiforgery strategy.

---

## 11. Dependency Injection

Use ASP.NET Core built-in dependency injection.

Register abstractions against implementations:

```csharp
services.AddScoped<ICategoriesService, CategoriesService>();
```

Use lifetimes deliberately:

- `Scoped`: database context, repositories, services that operate per request.
- `Transient`: lightweight stateless components where appropriate.
- `Singleton`: only for genuinely application-wide, thread-safe state/services.

Never inject a scoped service into a singleton.

Prefer constructor injection:

```csharp
public ProductsController(
    IProductsService productsService,
    ILogger<ProductsController> logger)
{
    _productsService = productsService;
    _logger = logger;
}
```

Avoid service locator patterns and unnecessary calls to
`IServiceProvider.GetService()` inside business code.

---

## 12. Async and Cancellation

Database and I/O operations must use async APIs.

Prefer:

```csharp
await repository.Queryable()
    .ToListAsync(cancellationToken);
```

over synchronous database access.

Where the existing interfaces support it, propagate:

```csharp
CancellationToken cancellationToken = default
```

through:

```text
Controller
  → Service
    → Repository
      → EF Core
```

Do not use `.Result`, `.Wait()`, or blocking async code.

---

## 13. EF Core Querying

Rules:

- Use LINQ.
- Prefer `AsNoTracking()` for read-only queries.
- Avoid N+1 queries.
- Use explicit `Include`/`ThenInclude` only when the data is actually needed.
- Project to DTO/ViewModel when loading large or presentation-specific
  result sets.
- Avoid loading entire tables when pagination/filtering can be done in SQL.
- Apply filtering, sorting, and paging before materialization.
- Do not call `ToList()` prematurely.
- Be deliberate about tracking behavior.

Preferred:

```csharp
var products = await repository
    .Queryable()
    .AsNoTracking()
    .Where(x => x.IsActive)
    .OrderBy(x => x.Name)
    .Select(x => new ProductListItemDto
    {
        Id = x.Id,
        Name = x.Name
    })
    .ToListAsync(cancellationToken);
```

---

## 14. Validation

Use layered validation.

Client side:

- Bootstrap-compatible validation UI.
- ASP.NET Core unobtrusive validation where configured.
- jQuery validation where already part of the application.

Server side:

- Data annotations for straightforward model validation.
- Service/domain validation for business rules.
- Never trust browser validation.

Example:

```csharp
if (request.Quantity <= 0)
{
    throw new ValidationException(
        "Quantity must be greater than zero.");
}
```

Adapt the exception/result strategy to the existing project conventions.

---

## 15. Security Rules

Always consider:

- Authentication.
- Authorization.
- Antiforgery protection.
- Input validation.
- SQL injection protection through EF Core parameterization.
- XSS prevention through Razor encoding.
- Overposting/mass-assignment risks.
- Sensitive data exposure.
- Secure configuration/secrets.
- Proper HTTP status codes.

Do not:

- Hard-code passwords or API keys.
- Put secrets in source control.
- Disable antiforgery protection without a concrete reason.
- Mark arbitrary user input as `Html.Raw`.
- Build SQL strings using user input.
- Trust hidden form fields for authorization decisions.

---

## 16. Logging and Error Handling

Use `ILogger<T>`.

Good:

```csharp
_logger.LogInformation(
    "Updating product {ProductId}",
    productId);
```

Do not log:

- Passwords.
- Tokens.
- Connection strings containing credentials.
- Sensitive personal information.
- Entire request bodies unless explicitly safe.

Use structured logging placeholders instead of string interpolation.

Preserve the existing global exception-handling/middleware strategy.

Do not catch exceptions just to rethrow them without adding context or
handling them.

Bad:

```csharp
try
{
    ...
}
catch (Exception)
{
    throw;
}
```

---

## 17. DTOs and ViewModels

Use the right object for the right boundary.

### Entity

Represents persistence/domain data.

### DTO

Represents data crossing an API/service boundary.

### ViewModel

Represents data required by a Razor page.

Do not automatically reuse an EF entity as every API and UI model.

For example:

```text
Product
ProductListItemDto
ProductDetailsDto
ProductEditViewModel
```

can legitimately be separate types.

---

## 18. Naming Conventions

Follow existing project naming first.

Otherwise use:

```text
PascalCase
```

for:

- Classes
- Interfaces
- Methods
- Properties
- Public constants

Interfaces begin with `I`:

```text
IProductsService
IRepositoryBase<T>
```

Private fields:

```csharp
private readonly IProductsService _productsService;
```

Async methods end with `Async`:

```text
GetByIdAsync
CreateAsync
UpdateAsync
DeleteAsync
```

Avoid abbreviations unless established by the project.

---

## 19. File and Folder Placement

Before creating a new file, determine the correct layer.

| Requirement | Project |
|---|---|
| Entity/domain model | `Banyan.Entity` |
| EF Core DbContext | `Banyan.Data` |
| EF Core configuration | `Banyan.Data` |
| Repository abstraction | `Banyan.Repository` |
| Repository implementation | `Banyan.Repository` |
| Business logic | `Banyan.Service` |
| Service interface | `Banyan.Service` |
| REST API endpoint | `Banyan.Api` |
| MVC controller | `Banyan.Web` |
| Razor view | `Banyan.Web/Views` |
| Razor ViewModel | `Banyan.Web/ViewModels` |
| Browser JavaScript | `Banyan.Web/wwwroot/js` |
| Browser CSS | `Banyan.Web/wwwroot/css` |
| Unit test | `Banyan.Test` |

Do not place business logic in `Banyan.Web` merely because the feature is
visible in the browser.

---

## 20. Feature Implementation Workflow

When implementing a new feature, follow this order unless the existing
architecture dictates otherwise.

### Step 1: Inspect

Before coding:

1. Read the relevant `.csproj`.
2. Inspect nearby implementations.
3. Inspect existing interfaces.
4. Inspect dependency injection registrations.
5. Inspect the relevant entity.
6. Inspect existing controller/view patterns.
7. Inspect tests.
8. Check package versions before introducing new packages.

Do not guess how an existing abstraction works.

### Step 2: Model

Determine:

- Entity changes.
- DTOs.
- ViewModels.
- Validation rules.
- Required relationships.

### Step 3: Data

If persistence changes are required:

- Update entity/model.
- Update `BanyanContext`.
- Update EF configuration.
- Create/update migration using the project's existing migration strategy.

### Step 4: Repository

Add only the repository abstraction/implementation needed for the feature.

Reuse existing generic repository functionality when sufficient.

### Step 5: Service

Add:

```text
I<Feature>Service
<Feature>Service
```

Put business rules here.

### Step 6: Web/API

For MVC:

```text
Controller
  → Service
  → ViewModel
  → Razor View
```

For API:

```text
API Controller
  → Service
  → DTO
```

### Step 7: UI

Use:

- Razor
- Bootstrap 5
- jQuery
- Existing layout/components
- Existing theme/design system

### Step 8: Tests

Add/update tests for:

- Valid behavior.
- Invalid input.
- Not-found behavior.
- Important business rules.
- Regression scenarios.

### Step 9: Verify

Run, as applicable:

```bash
dotnet restore
dotnet build
dotnet test
```

For EF changes, also verify migrations and database update behavior using the
solution's configured database.

---

## 21. Bug-Fix Workflow

For a bug:

1. Reproduce or identify the failing path.
2. Trace the request through the layers.
3. Find the lowest appropriate layer where the defect belongs.
4. Fix the root cause.
5. Avoid compensating hacks in the UI if the defect is in the service/data
   layer.
6. Add a regression test where practical.
7. Build and test the affected projects.
8. Check adjacent functionality for unintended behavior.

Example:

If a product total is wrong:

```text
Do NOT:
Razor calculates a corrected total

Prefer:
Service/domain calculation is corrected
        ↓
ViewModel receives correct total
        ↓
Razor displays it
```

---

## 22. Refactoring Rules

When refactoring:

- Preserve behavior unless behavior change is explicitly requested.
- Make the smallest coherent architectural change.
- Do not introduce new patterns merely for stylistic preference.
- Match the existing codebase before imposing personal conventions.
- Avoid mass renaming unrelated code.
- Avoid package upgrades unrelated to the task.
- Keep public interfaces stable unless the change requires otherwise.
- Update tests with interface changes.
- Remove dead code only when confidence is high.

---

## 23. NuGet Package Rules

Before adding a package:

1. Check whether the functionality already exists.
2. Check the project's target framework.
3. Check package versions already used in the solution.
4. Check whether the package is compatible with the target framework.
5. Prefer Microsoft/official packages when they meet the requirement.
6. Avoid adding packages for trivial functionality.
7. Do not upgrade all packages as part of an unrelated feature.

If the task says "update NuGet packages", inspect the full solution and
identify compatibility implications before changing versions.

---

## 24. EF Core Migration Rules

When model/schema changes are made:

1. Confirm the correct `DbContext`.
2. Confirm the migrations project/location.
3. Confirm the startup project.
4. Create a descriptive migration.
5. Review generated migration code.
6. Ensure the migration does not unintentionally drop or alter data.
7. Test database update behavior.

Typical commands may be:

```bash
dotnet ef migrations add AddProductStatus
dotnet ef database update
```

Use the actual project/startup options required by this solution.

Never blindly run destructive migration operations against production.

---

## 25. API and MVC Separation

`Banyan.Api` and `Banyan.Web` have different responsibilities.

Do not put MVC Razor concerns in API code.

Do not make the MVC application responsible for implementing API contracts.

Shared business/application behavior belongs in:

```text
Banyan.Service
```

Shared persistence belongs in:

```text
Banyan.Repository
Banyan.Data
```

---

## 26. UI Theme Preservation

If the application contains theme variables/design tokens, preserve them.

For example, if the project defines:

```text
varThemePrimary
varThemeSecondary
varThemeAccent
varThemeBackground
varThemeText
```

do not replace them with hard-coded colors without a reason.

When modifying UI:

- Reuse existing theme variables.
- Preserve header/sidebar/footer styling.
- Preserve active/hover/focus states.
- Maintain sufficient contrast.
- Keep responsive behavior.
- Do not introduce a competing visual framework.

---

## 27. Accessibility

For Razor/Bootstrap UI:

- Use semantic HTML.
- Associate labels with inputs.
- Preserve keyboard navigation.
- Use visible focus states.
- Do not rely solely on color to communicate state.
- Provide meaningful button/link text.
- Use appropriate ARIA only when native HTML semantics are insufficient.
- Ensure validation errors are understandable.

---

## 28. Performance

Prefer:

- Async database calls.
- `AsNoTracking()` for read-only operations.
- Projection instead of loading unnecessary columns.
- Pagination for large datasets.
- Server-side filtering/sorting.
- Efficient indexes where schema ownership allows.
- Avoiding N+1 queries.
- Avoiding repeated service/database calls in a single request.

Do not optimize speculatively. Measure or identify a concrete bottleneck.

---

## 29. What the Agent Must Not Do

Do not:

- Put database queries directly in controllers.
- Put business logic in Razor views.
- Put business logic in jQuery.
- Bypass services without a documented architectural reason.
- Bypass repositories/unit of work without a documented architectural reason.
- Add circular project references.
- Hard-code secrets.
- Hard-code connection strings with credentials.
- Introduce a second competing architecture.
- Replace Bootstrap 5 with another UI framework.
- Replace jQuery with another frontend framework unless explicitly requested.
- Upgrade dependencies unrelated to the task.
- Rewrite large portions of the application for style alone.
- Suppress compiler warnings blindly.
- Ignore failing tests.
- Change behavior without explaining the impact.
- Use synchronous EF/database calls in new code.
- Return EF entities directly from public APIs when DTOs are appropriate.
- Trust client-side validation as the only validation.
- Disable CSRF/antiforgery protections as a shortcut.

---

## 30. Definition of Done

A task is complete when:

- The code is in the correct project/layer.
- Existing architecture is preserved.
- Repository + Unit of Work conventions are respected.
- Business logic is in services.
- MVC controllers remain thin.
- Razor views remain presentation-focused.
- Bootstrap 5 is used consistently.
- jQuery usage follows existing project conventions.
- Validation is performed server-side.
- Relevant tests are added or updated.
- `dotnet build` succeeds for the affected solution/projects.
- `dotnet test` succeeds for relevant tests.
- No unnecessary NuGet/package changes were introduced.
- No secrets or sensitive configuration were committed.
- The final response clearly summarizes what changed and what was verified.

---

## 31. Claude Code Working Instructions

When operating in this repository:

### First inspect

Use repository search to find:

```text
*.sln
*.csproj
Directory.Build.props
Directory.Build.targets
global.json
NuGet.config
appsettings*.json
Program.cs
Startup.cs
BanyanContext.cs
IRepositoryBase.cs
RepositoryBase.cs
IServiceBase.cs
ServiceBase.cs
```

Then inspect the nearest analogous feature before creating new code.

### Prefer existing patterns

If the codebase already contains:

```text
CategoriesService
ProductsService
CategoriesController
ProductsController
```

use those implementations as the primary local convention.

The local repository takes precedence over generic examples from the reference
repository.

### Keep changes focused

For a request such as:

> Add CRUD for Products

do not simultaneously:

- Upgrade .NET.
- Upgrade Bootstrap.
- Upgrade jQuery.
- Replace repository abstractions.
- Rewrite all controllers.
- Introduce a new frontend framework.

Only perform those changes if explicitly requested or required.

### Verify before claiming success

Do not state that a feature builds or tests pass unless the corresponding
commands were actually run.

If a command cannot be run, state that verification was not performed.

---

## 32. Preferred Response Format After Code Changes

When reporting completed work, use:

```text
Implemented:
- <feature/change>
- <feature/change>

Architecture:
- Entity: <changes>
- Data: <changes>
- Repository: <changes>
- Service: <changes>
- Web/API: <changes>
- Tests: <changes>

Verification:
- dotnet build: <passed/failed/not run>
- dotnet test: <passed/failed/not run>

Notes:
- <important migration/configuration/manual step>
```

Keep the report factual and concise.

---

## 33. Reference Architecture

The reference implementation is based on URF's Repository and Unit of Work
approach. The URF sample demonstrates EF Core `DbContext`, repository access,
unit-of-work abstractions, dependency injection, and asynchronous operations.

Use the upstream repository as an architectural reference, not as a reason to
copy obsolete framework/package versions.

Reference:

https://github.com/urfnet/URF.Core.Sample/tree/master

The target application's actual `.csproj`, package versions, framework target,
interfaces, and implementation patterns always take precedence.

---

## 34. Final Principle

Follow this rule when making architectural decisions:

> Put each responsibility in the lowest appropriate layer, expose it through
> the existing abstraction, keep higher layers thin, and reuse the project's
> established conventions before introducing anything new.

The expected feature path is:

```text
Entity
  ↓
Data / EF Core
  ↓
Repository
  ↓
Service
  ↓
Web Controller / API Controller
  ↓
ViewModel / DTO
  ↓
Razor + Bootstrap 5 + jQuery
```

Preserve this separation unless the user explicitly requests a different
architecture.

---

## 35. BanyanFin Project Branding and UI Theme

The application has a fixed BanyanFin visual identity. Claude Code MUST use this
branding consistently when creating or modifying Razor pages, layouts, partial
views, forms, dashboards, tables, navigation, cards, alerts, badges, buttons,
transaction screens, payment screens, and other user-facing UI.

### Brand Color Palette

| Color | Hex | Primary Usage |
|---|---|---|
| Deep Navy | `#123B5D` | Sidebar, headers, primary buttons, navigation |
| Teal | `#168F8B` | Secondary actions, links, selected states, highlights |
| Green | `#2E9B62` | Successful transactions, completed payments, positive balances |
| Amber | `#E5A72F` | Pending transactions, warnings, attention states |
| Red | `#D9534F` | Failed transactions, errors, negative states, destructive actions |

### Mandatory UI Branding Rules

1. Treat the colors above as the project's official design tokens.
2. Do not invent a different primary color palette when creating a new page.
3. Do not introduce unrelated colors for buttons, navigation, alerts, statuses,
   transaction states, or major UI components.
4. Use Bootstrap 5 as the UI framework, but apply the BanyanFin brand colors
   through the project's CSS/theme layer rather than replacing Bootstrap.
5. Prefer reusable CSS variables/design tokens so the colors remain consistent.

Recommended theme variables:

```css
:root {
    --banyanfin-navy: #123B5D;
    --banyanfin-teal: #168F8B;
    --banyanfin-green: #2E9B62;
    --banyanfin-amber: #E5A72F;
    --banyanfin-red: #D9534F;
}
```

### Color Application

Use:

```text
#123B5D
- Main sidebar
- Top/header navigation
- Primary navigation
- Primary CTA buttons
- Main page/header accents

#168F8B
- Secondary buttons/actions
- Links
- Active/selected navigation states
- UI highlights
- Secondary interactive elements

#2E9B62
- Success messages
- Successful transactions
- Completed payments
- Positive balances
- Successful status badges

#E5A72F
- Pending transactions
- Pending payments
- Warnings
- Attention states

#D9534F
- Failed transactions
- Failed payments
- Errors
- Negative states
- Delete/destructive actions
```

### Bootstrap 5 Integration

Use Bootstrap 5 components and utilities wherever possible, but do not rely
blindly on Bootstrap's default `primary`, `success`, `warning`, and `danger`
colors when they conflict with the BanyanFin palette.

For branded components, use the project's theme classes or CSS variables.

Example:

```css
.btn-banyanfin-primary {
    background-color: var(--banyanfin-navy);
    border-color: var(--banyanfin-navy);
    color: #fff;
}

.btn-banyanfin-secondary {
    background-color: var(--banyanfin-teal);
    border-color: var(--banyanfin-teal);
    color: #fff;
}

.status-success {
    color: var(--banyanfin-green);
}

.status-pending {
    color: var(--banyanfin-amber);
}

.status-failed {
    color: var(--banyanfin-red);
}
```

Do not add these classes if an equivalent branded implementation already exists.
Inspect the existing CSS/theme files first and reuse the established implementation.

### Page Construction Rules

When Claude Code creates a new page:

1. Inspect `_Layout.cshtml`, shared partials, existing CSS, and existing pages
   before designing the page.
2. Preserve the existing sidebar/header/navigation structure.
3. Use the BanyanFin color palette consistently.
4. Use Bootstrap 5 responsive grid and components.
5. Keep spacing, typography, borders, cards, tables, forms, and buttons visually
   consistent with existing pages.
6. Reuse existing components/classes before creating new ones.
7. If a new component is necessary, make it reusable and place its styling in the
   appropriate CSS file rather than scattering inline styles.
8. Do not create a completely different visual style for individual pages.
9. Do not introduce another CSS framework or UI library.
10. Maintain accessibility and sufficient text/background contrast.

### Status Semantics Are Mandatory

Do not use colors arbitrarily for financial/application status.

```text
Success / Completed / Positive → #2E9B62
Pending / Warning / Attention → #E5A72F
Failed / Error / Negative / Destructive → #D9534F
```

Use text, icons, labels, or other accessible indicators in addition to color.
Never communicate an important status using color alone.

### Design Consistency Requirement

A newly generated page must look like it belongs to the same BanyanFin
application as the existing pages.

Before implementing UI, inspect at least one or more existing pages that are
closest to the requested feature and reuse their:

- Layout
- Sidebar/header
- Navigation
- Card style
- Form style
- Table style
- Button style
- Status/badge style
- Spacing
- Typography
- Responsive behavior
- Theme variables

If an existing component conflicts with these branding rules, preserve the
existing application convention unless the user explicitly requests a redesign.

### UI Quality Gate

Before considering a UI task complete, verify:

- [ ] Bootstrap 5 is used.
- [ ] BanyanFin colors are used consistently.
- [ ] Deep Navy `#123B5D` is used for primary navigation/header/sidebar areas.
- [ ] Teal `#168F8B` is used for secondary/interactive states.
- [ ] Green `#2E9B62` represents successful/positive states.
- [ ] Amber `#E5A72F` represents pending/warning states.
- [ ] Red `#D9534F` represents failed/error/negative/destructive states.
- [ ] Existing layout and components were reused where possible.
- [ ] No competing visual theme was introduced.
- [ ] Responsive behavior is preserved.
- [ ] Status is not communicated by color alone.
- [ ] No unnecessary inline CSS was introduced.

The branding rules in this section apply to every new or modified user-facing
page unless the user explicitly requests a different design.
