# Banyan Project Instructions

- Before coding, inspect the solution structure, project references, target frameworks, package versions, and the closest existing implementation.
- Follow `.claude/skills/agent_developer_guideline/SKILL.md` for architecture, coding, testing, security, and UI conventions. Read the skill before applicable development tasks.
- Preserve the existing Banyan architecture and local code conventions. Existing source code, project files, and installed URF interfaces take precedence over generic examples.
- Keep changes focused. Do not rewrite unrelated modules, introduce competing patterns, or upgrade unrelated dependencies.
- Do not claim a build, test, migration, or other verification succeeded unless you actually ran it. Report commands that could not be run.

## Expected Project Responsibilities

Verify the actual solution before relying on this list:

- `Banyan.Entity`: entities and domain models
- `Banyan.Data`: EF Core persistence, DbContext, configuration, migrations
- `Banyan.Repository`: repository abstractions and implementations
- `Banyan.Service`: business logic and service interfaces/implementations
- `Banyan.Api`: REST API
- `Banyan.Web`: ASP.NET Core MVC, Razor views, ViewModels, CSS and JavaScript
- `Banyan.Test`: automated tests

## Technology and UI

- Use the framework and package versions declared by the solution; use .NET 10 when the solution targets it.
- Follow existing ASP.NET Core MVC/Razor, Bootstrap 5, jQuery, and EF Core conventions.
- Preserve BanyanFin branding: Navy `#123B5D`, Teal `#168F8B`, Green `#2E9B62`, Amber `#E5A72F`, Red `#D9534F`.
- Reuse existing layouts, components, theme variables, and CSS classes before adding new ones.

## Verification

Run relevant commands from the correct solution/project directory:

```bash
dotnet restore
dotnet build
dotnet test
```

Inspect migration commands and database targets before running them. Never perform destructive database operations without explicit authorization.
