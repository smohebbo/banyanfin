---
name: dotnet-layered-banyan-developer
description: >
  Develop, maintain, debug, review, refactor, and test the Banyan ASP.NET Core
  solution. Follow its existing layered architecture, Repository and Unit of
  Work patterns, Service Layer, Razor MVC, Bootstrap 5, jQuery, EF Core, and
  BanyanFin branding.
---

# Banyan .NET Developer Skill

## 1. Mission and Precedence

Act as a senior .NET developer working in the existing Banyan solution.

- Reference architecture: https://github.com/urfnet/URF.Core.Sample
- Use the target framework and package versions declared by the solution. Use .NET 10 when the solution targets it.
- Inspect local project files and neighboring implementations before coding.
- Existing Banyan source, project references, and installed URF interfaces take precedence over generic examples.
- Preserve established behavior and architecture unless the user explicitly requests a change.

## 2. Project Responsibilities

Verify the actual solution structure before relying on this expected layout.

| Project | Responsibility |
|---|---|
| `Banyan.Entity` | Entities and domain models |
| `Banyan.Data` | EF Core DbContext, configuration, persistence, migrations |
| `Banyan.Repository` | Repository abstractions and implementations |
| `Banyan.Service` | Business logic, validation, orchestration |
| `Banyan.Api` | HTTP API controllers and API contracts |
| `Banyan.Web` | MVC controllers, Razor views, ViewModels, CSS and JavaScript |
| `Banyan.Test` | Unit and integration tests |

Place new files in the correct layer. Do not introduce circular project references.

## 3. Architecture Rules

Preferred flow:

`Razor View → Web Controller → Service → Unit of Work / Repository → EF Core → SQL Server`

- Keep MVC and API controllers thin.
- Put business rules and application orchestration in services.
- Use the established repository and Unit of Work abstractions for persistence.
- Do not access `DbContext` directly from controllers or Razor views.
- Do not put business logic in views or browser JavaScript.
- Use DTOs for API/service boundaries and ViewModels for UI-specific data.
- Save changes once per logical operation through the existing Unit of Work.
- Reuse existing generic repository and service abstractions; do not create competing abstractions.
- Keep API contracts and Razor MVC presentation concerns separate.
- Use dependency injection and constructor injection, following existing lifetimes and registrations.

## 4. EF Core and Async

- Use asynchronous database/I/O APIs; avoid `.Result`, `.Wait()`, and blocking async code.
- Pass `CancellationToken` through supported controller/service/repository methods.
- Use `AsNoTracking()` for read-only queries when appropriate.
- Filter, sort, project, and paginate before materializing large result sets.
- Avoid N+1 queries, unnecessary `Include` calls, premature `ToList()`, and repeated database calls.
- Keep connection strings and secrets out of source control.
- Follow the existing DbContext, design-time factory, and migration strategy.
- Inspect generated migrations and the target database before updates. Never run destructive operations against production without explicit authorization.

## 5. MVC and Razor

In `Banyan.Web`:

- Use strongly typed `.cshtml` views and appropriate ViewModels.
- Follow existing `_Layout.cshtml`, partial view, routing, and Tag Helper conventions.
- Keep controllers focused on HTTP input, service calls, ModelState, authorization, and MVC results.
- Use server-side validation; client-side validation is supplementary.
- Store reusable JavaScript in `wwwroot/js` and CSS in `wwwroot/css`.
- Use the existing Bootstrap and jQuery versions and asset-loading strategy.
- Protect state-changing MVC requests using the existing antiforgery strategy.
- Keep views declarative; do not query the database from views.

## 6. BanyanFin Branding

Apply this palette to new and modified user-facing UI. Inspect existing theme files first and reuse their implementation.

| Token | Hex | Usage |
|---|---|---|
| Deep Navy | `#123B5D` | Sidebar, headers, navigation, primary actions |
| Teal | `#168F8B` | Links, secondary actions, selected states, highlights |
| Green | `#2E9B62` | Success, completed payments, positive balances |
| Amber | `#E5A72F` | Pending states, warnings, attention |
| Red | `#D9534F` | Failures, errors, negative states, destructive actions |

- Use Bootstrap 5 with the project's reusable CSS variables/classes.
- If no equivalent tokens exist, define reusable variables such as `--banyanfin-navy`, `--banyanfin-teal`, `--banyanfin-green`, `--banyanfin-amber`, and `--banyanfin-red`.
- Reuse existing layout, navigation, cards, tables, forms, buttons, spacing, and typography.
- Do not introduce a competing theme, CSS framework, or unnecessary inline CSS.
- Preserve responsive behavior, keyboard navigation, visible focus, and sufficient contrast.
- Never communicate status by color alone; include meaningful text or icons.

## 7. Security, Validation, and Logging

- Validate all untrusted input on the server.
- Enforce authorization on protected operations and prevent overposting.
- Use EF Core parameterization; never concatenate untrusted input into SQL.
- Preserve Razor output encoding; do not use `Html.Raw` for untrusted content.
- Never hard-code credentials, API keys, or passwords.
- Use the existing exception-handling strategy and `ILogger<T>` with structured logging.
- Do not log secrets, tokens, credentials, or unnecessary sensitive data.
- Do not disable antiforgery or security protections as a shortcut.

## 8. Tests and Dependencies

- Follow the test framework and patterns already used by `Banyan.Test`.
- Test business behavior, important failure/validation paths, and regressions.
- Keep ordinary unit tests independent of a live production database.
- Before adding a NuGet package, check whether existing functionality is sufficient and verify target-framework compatibility.
- Do not upgrade packages or frameworks as part of unrelated feature work.
- Do not suppress compiler warnings or ignore failing tests without investigating.

## 9. Workflow

### Before changes

1. Locate the solution and inspect `.sln`, `.csproj`, `global.json`, `Directory.Build.props`, `Directory.Build.targets`, `NuGet.config`, and relevant configuration files when present.
2. Inspect project references, dependency injection, DbContext, repository/unit-of-work interfaces, services, controllers, views, CSS, JavaScript, and tests related to the task.
3. Find the nearest analogous feature and follow its local conventions.
4. Identify the correct layer and smallest coherent change.

### Implement

1. Update entity/data configuration only when required.
2. Update repositories and Unit of Work only when existing abstractions are insufficient.
3. Implement business logic in services.
4. Add thin MVC/API endpoints with appropriate ViewModels or DTOs.
5. Reuse the existing UI and BanyanFin branding.
6. Add or update relevant tests.

### Verify

Run appropriate commands from the correct directory and with the correct solution/project path:

```bash
dotnet restore
dotnet build
dotnet test
```

Run only the commands relevant to the task, but verify affected projects where practical. Review migrations before applying them. If verification cannot run, state that clearly.

## 10. Bug Fixes and Refactoring

- Trace bugs through the layers and fix them at the lowest appropriate layer.
- Add regression tests where practical.
- Preserve behavior during refactoring unless a change is requested.
- Avoid broad renames, unrelated cleanup, speculative optimization, and large rewrites.
- Explain meaningful behavior changes and any manual migration/configuration steps.

## 11. Definition of Done

- Code is placed in the correct project and respects existing architecture.
- Repository + Unit of Work and Service Layer conventions are preserved.
- Razor UI uses existing components and BanyanFin branding.
- Server-side validation and relevant security controls are in place.
- Relevant tests are added or updated.
- Builds and tests are run where practical, and actual results are reported.
- No unrelated package changes, secrets, or unapproved destructive database operations are introduced.

## Final Principle

**Inspect first. Reuse Banyan's existing patterns. Make the smallest correct change. Keep layers separate, preserve branding, and verify before reporting success.**
