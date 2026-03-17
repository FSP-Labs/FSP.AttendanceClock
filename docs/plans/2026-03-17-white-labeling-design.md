# White-labeling Design: D3Fichadora → FSP.AttendanceClock

**Date:** 2026-03-17
**Goal:** Remove all references to D3Fichadora/Desing3 and prepare the project for open-source release on GitHub under the name "FSP Attendance Clock".

---

## Approved Design

### 1. Directory & file renames

| Before | After |
|---|---|
| `D3Fichadora.sln` | `FSP.AttendanceClock.sln` |
| `D3Fichadora.Core/` | `FSP.AttendanceClock.Core/` |
| `D3Fichadora.Infrastructure/` | `FSP.AttendanceClock.Infrastructure/` |
| `D3Fichadora.Web/` | `FSP.AttendanceClock.Web/` |
| `D3FichadoraInfrastructureModelSnapshot.cs` | `FSPAttendanceClockInfrastructureModelSnapshot.cs` |

### 2. Namespaces & using statements (all .cs and .cshtml files)

- `D3Fichadora.Core.*` → `FSP.AttendanceClock.Core.*`
- `D3Fichadora.Infrastructure.*` → `FSP.AttendanceClock.Infrastructure.*`
- `D3Fichadora.Web.*` → `FSP.AttendanceClock.Web.*`
- Class `D3FichadoraInfrastructureModelSnapshot` → `FSPAttendanceClockInfrastructureModelSnapshot`

### 3. Solution & project files

- `.sln`: update project names and relative paths for all 3 projects
- Each `.csproj`: add `<RootNamespace>` and `<AssemblyName>` set to `FSP.AttendanceClock.*`
- Cross-project `<ProjectReference>` paths updated to new directory names

### 4. Visual branding

- `_Layout.cshtml` title: `@ViewData["Title"] - FSP Attendance Clock`
- Logo: replace `<img src="~/img/desing3_logo-principal-05.png">` with `<img src="~/img/logo.png" alt="FSP Attendance Clock">`
- Create a placeholder `wwwroot/img/logo.png` (simple PNG with text)
- Delete `wwwroot/img/desing3_logo-principal-05.png`
- README instructs users to replace `wwwroot/img/logo.png` with their own logo

### 5. Configuration (appsettings.json) — security for GitHub

- Create `appsettings.Example.json` with placeholder values:
  - `Host=localhost;Port=5433;Database=FSPAttendanceClock;Username=<your-db-user>;Password=<your-db-password>`
- Add `appsettings.json` and `appsettings.Development.json` to `.gitignore`
- Keep `appsettings.Example.json` in the repo as the reference template

### 6. GitHub preparation

- Add `.gitignore` (standard .NET template: excludes `bin/`, `obj/`, `.vs/`, `*.user`, `appsettings.json`, `appsettings.Development.json`)
- Add `README.md` with: project description, requirements (PostgreSQL, .NET 9), setup steps, logo replacement instructions
- Update `SECURITY_REVIEW_X_FORWARDED_FOR.md`: replace `D3Fichadora` references
- Update `CLAUDE.md`: reflect new project/directory names

### 7. Out of scope

- Migration SQL content (table names `Usuarios`, `Fichajes`, etc. are domain names, not brand names)
- Third-party libraries in `wwwroot/lib/`
- `bin/` and `obj/` directories (handled by `.gitignore`)
