# Open-Source Documentation Design

**Date:** 2026-03-17
**Approach:** Option C — Mid-ground

## Goals

Make FSP.AttendanceClock a proper open-source project that attracts both .NET developers and general web developers.

## Decisions

- **Target audience:** Mix — .NET devs for feature/backend work, general web devs for UI/docs/CSS
- **README tone:** Professional intro + developer-friendly technical sections
- **Good first issues:** Mix of UI/docs (no .NET knowledge needed) and code (.NET) issues

## Deliverables

### 1. README.md (full rewrite)

Sections:
- Title + badges (MIT license, .NET 9, PostgreSQL, open issues count)
- One-liner description + screenshot of the running app
- Features table (Employee column / Administrator column)
- Tech Stack list
- Quick Start (5 numbered steps: clone → configure → migrate → run)
- Architecture overview (3-layer: Core / Infrastructure / Web)
- Contributing link
- License

Screenshot: taken from the live running app using Playwright browser tools, saved to `docs/screenshot.png`.

### 2. LICENSE

Full MIT license text with year 2025 and author "FSP Labs".

### 3. CONTRIBUTING.md

Sections:
- How to report a bug (link to bug report template)
- How to suggest a feature (link to feature request template)
- Development setup (prerequisites, clone, configure, run locally)
- PR checklist (build passes, follows existing patterns, no secrets committed)
- Coding conventions (brief: follow existing patterns, English only, conventional commits)

### 4. GitHub issue templates

- `.github/ISSUE_TEMPLATE/bug_report.md` — Describe the bug / Steps to reproduce / Expected / Actual / Environment
- `.github/ISSUE_TEMPLATE/feature_request.md` — Problem statement / Proposed solution / Alternatives

### 5. PR template

- `.github/PULL_REQUEST_TEMPLATE.md` — Type of change / Description / Testing done / `dotnet build` passes / No secrets

### 6. Good first issues (7 issues via `gh` CLI)

UI/Docs (no .NET required):
1. **Add dark mode toggle** — CSS + minimal JS in `site.css` and `_Layout.cshtml`
2. **Translate UI to Spanish** — All Razor views, no code changes
3. **Improve mobile layout on History page** — Responsive CSS tweaks in `site.css`

Code (.NET):
4. **Add email field to User** — New `Email` column in `Usuarios`, EF migration, form field in `CreateUser.cshtml`
5. **Add pagination to History view** — `AttendanceController.History()` + Skip/Take + nav buttons
6. **Write unit tests for PasswordHasher** — New xUnit test project, test `Hash()` and `Verify()`
7. **Add "export my data" button for employees** — New action in `AttendanceController`, reuse ClosedXML logic

### 7. CLAUDE.md update

Update to reflect:
- Project is now fully in English
- `ReporteHorario` renamed to `HoursReport`
- New files: `AttendanceClockSettings.cs`, `ClaimsPrincipalExtensions.cs`, `IAttendanceReportService.cs`, `DailyHoursSummary.cs`, `AttendanceReportService.cs`
- Note about open-source documentation files added

## Out of scope

- CODE_OF_CONDUCT.md (too early for the project size)
- SECURITY.md (covered by existing `SECURITY_REVIEW_X_FORWARDED_FOR.md` reference in README)
- Docker setup (separate future task)
- CI/CD pipeline (separate future task)
