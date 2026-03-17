# Contributing to FSP Attendance Clock

Thank you for your interest in contributing! This guide covers everything you need to get started.

---

## Reporting a Bug

1. Search [existing issues](https://github.com/FSP-Labs/FSP.AttendanceClock/issues) first — it may already be reported.
2. If not, open a [bug report](https://github.com/FSP-Labs/FSP.AttendanceClock/issues/new?template=bug_report.md).

Please include:
- Steps to reproduce (be specific)
- What you expected vs. what actually happened
- Your environment (.NET version, OS, PostgreSQL version)

---

## Suggesting a Feature

Open a [feature request](https://github.com/FSP-Labs/FSP.AttendanceClock/issues/new?template=feature_request.md). Describe the problem you're trying to solve, not just the solution — that helps us understand context.

---

## Development Setup

**Prerequisites:** .NET 9 SDK, PostgreSQL 14+, and a git client.

```bash
# Clone
git clone https://github.com/FSP-Labs/FSP.AttendanceClock.git
cd FSP.AttendanceClock

# Configure
cp FSP.AttendanceClock.Web/appsettings.Example.json FSP.AttendanceClock.Web/appsettings.json
# Edit appsettings.json with your local PostgreSQL connection string

# Migrate
dotnet ef database update \
  --project FSP.AttendanceClock.Infrastructure \
  --startup-project FSP.AttendanceClock.Web

# Run
dotnet run --project FSP.AttendanceClock.Web
```

**Build check:** Always run `dotnet build` before submitting a PR — it must produce `0 Error(s)`.

---

## Making Changes

1. Fork the repo and create a branch from `master`: `git checkout -b feat/your-feature`
2. Make your changes.
3. Run `dotnet build` — must pass with 0 errors.
4. Commit using [Conventional Commits](https://www.conventionalcommits.org/): `feat:`, `fix:`, `docs:`, `refactor:`, `style:`.
5. Open a pull request against `master`.

---

## Pull Request Checklist

Before submitting, make sure:

- [ ] `dotnet build` passes with 0 errors
- [ ] New features follow the existing 3-layer architecture (Core → Infrastructure → Web)
- [ ] No secrets or credentials are committed (check `.gitignore`)
- [ ] UI strings are in English
- [ ] Code comments are in English

---

## Coding Conventions

- Follow existing patterns in the codebase — consistency over personal style
- All user-visible strings and code comments in **English**
- Use `IOptions<AttendanceClockSettings>` for configuration values instead of magic numbers
- Use the `GetCurrentUserId()` extension method (in `ClaimsPrincipalExtensions.cs`) instead of parsing claims manually
- Conventional Commits for all commit messages

---

## Questions?

Open a [discussion](https://github.com/FSP-Labs/FSP.AttendanceClock/discussions) or file an issue with the `question` label.
