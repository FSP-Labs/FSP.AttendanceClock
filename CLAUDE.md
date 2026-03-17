# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FSP.AttendanceClock is a Spanish-language ASP.NET Core 9 employee attendance (fichaje) system. Employees clock in/out; administrators manage users, view audit logs, and generate reports. The UI and code comments are in Spanish.

## Commands

```bash
# Build
dotnet build

# Run the web app
dotnet run --project FSP.AttendanceClock.Web

# Apply pending EF Core migrations
dotnet ef database update --project FSP.AttendanceClock.Infrastructure --startup-project FSP.AttendanceClock.Web

# Create a new migration
dotnet ef migrations add <MigrationName> --project FSP.AttendanceClock.Infrastructure --startup-project FSP.AttendanceClock.Web
```

There are no test projects or linting configurations in this repo.

## Architecture

3-layer clean architecture across 3 projects:

- **FSP.AttendanceClock.Core** — Domain entities (`User`, `Attendance`, `AttendanceAudit`, `SystemLog`) and interfaces (`IAuditService`, `ILoginAttemptService`). No dependencies on other projects.
- **FSP.AttendanceClock.Infrastructure** — EF Core `AppDbContext`, `DbInitializer` (seeds DB on startup), `AuditService`, `LoginAttemptService` (in-memory brute-force rate limiting), and `PasswordHasher` (PBKDF2).
- **FSP.AttendanceClock.Web** — ASP.NET Core MVC: 4 controllers (`AccountController`, `AttendanceController`, `AdminController`, `HomeController`), Razor views, ViewModels, and `GlobalExceptionHandlingMiddleware`.

### Key flows

- **Authentication**: Cookie-based auth. Unauthenticated users are redirected to `/Account/Login`. Rate limiting is enforced by `LoginAttemptService` (singleton) — 5 failed attempts triggers a 15-minute IP block.
- **Attendance recording**: `AttendanceController` creates `Attendance` records and calls `IAuditService` to write `SystemLog` entries on every action.
- **Admin operations**: `AdminController` requires `Administrador` role. Handles user CRUD, audit log viewing, and Excel report generation via ClosedXML.

### Database

PostgreSQL via EF Core. Tables: `Usuarios`, `Fichajes`, `AuditoriasFichajes`, `RegistrosSistema`. Connection string is in `appsettings.json` (`Host=localhost;Port=5433`).

### Known security issue

`Program.cs` clears `KnownNetworks`/`KnownProxies`, which means `X-Forwarded-For` headers are trusted from any source. This allows IP spoofing in audit logs and can bypass brute-force rate limiting. See `SECURITY_REVIEW_X_FORWARDED_FOR.md` for details.
