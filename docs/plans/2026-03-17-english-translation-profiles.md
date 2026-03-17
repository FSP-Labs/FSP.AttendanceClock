# English Translation + GitHub Profiles Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Translate the entire FSP.AttendanceClock UI/code to English, hide CLAUDE.md from public repos, and set up GitHub profiles for saasixx (casual) and FSP-Labs (professional).

**Architecture:** Purely text/content changes across Razor views and C# controllers — no structural refactoring. Action rename `ReporteHorario→HoursReport` requires updating the view filename, two controller methods, and every `asp-action` reference that points to them. GitHub profiles are created via `gh` CLI in separate repos. Each task ends with `dotnet build` or a `gh` verification as its "test".

**Tech Stack:** ASP.NET Core 9 MVC (Razor views, C# controllers), GitHub CLI (`gh`), Markdown (GitHub profile READMEs).

---

## Part 0 — Hide CLAUDE.md from public repos

### Task 0: Add CLAUDE.md to .gitignore and untrack it

**Files:**
- Modify: `D:\Proyectos\FSP Attendance Clock\.gitignore`
- Git command (untrack without deleting)

**Step 1: Add CLAUDE.md to .gitignore**

Open `.gitignore` and append at the end:

```
## Claude Code context file (local dev only)
CLAUDE.md
```

**Step 2: Untrack CLAUDE.md without deleting the file**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git rm --cached CLAUDE.md
```

Expected output:
```
rm 'CLAUDE.md'
```

The file stays on disk but will no longer be tracked by git.

**Step 3: Verify build still passes**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add .gitignore
git commit -m "chore: exclude CLAUDE.md from version control"
```

---

## Part 1 — Complete English Translation

> All user-visible strings (navbar, page titles, labels, buttons, badges, TempData messages, comments) must be in English. Code identifiers (variable names, method names) are already English except for `ReporteHorario` which is handled in Task 8.

### Task 1: Translate _Layout.cshtml (navbar)

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Shared/_Layout.cshtml`

**Step 1: Replace all Spanish strings**

| Spanish | English |
|---------|---------|
| `lang="es"` | `lang="en"` |
| `Mi Fichaje` | `My Attendance` |
| `Historial` | `History` |
| `Administración` | `Administration` |
| `Hola, @User.Identity?.Name` | `Hello, @User.Identity?.Name` |
| `title="Cambiar contraseña"` | `title="Change password"` |
| `Cerrar Sesión` | `Sign Out` |

Full replacement (show final relevant section):

```html
<html lang="en">
```

```html
<a class="nav-link text-white" asp-controller="Attendance" asp-action="Index">My Attendance</a>
```

```html
<a class="nav-link text-white" asp-controller="Attendance" asp-action="History"><i class="bi bi-clock-history"></i> History</a>
```

```html
<a class="nav-link text-white" asp-controller="Admin" asp-action="Index">Administration</a>
```

```html
<span class="nav-link text-white-50">Hello, @User.Identity?.Name</span>
```

```html
<a class="nav-link text-white" asp-controller="Account" asp-action="ChangePassword" title="Change password">
```

```html
<a class="nav-link text-white" asp-controller="Account" asp-action="Logout">Sign Out</a>
```

**Step 2: Build and verify**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 3: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Shared/_Layout.cshtml"
git commit -m "i18n: translate layout/navbar to English"
```

---

### Task 2: Translate Attendance/Index.cshtml (employee dashboard)

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Attendance/Index.cshtml`

**Step 1: Replace all Spanish strings and comments**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Mi Panel"` | `ViewData["Title"] = "My Dashboard"` |
| `// ya viene ordenado descendente` | `// already sorted descending` |
| `// Calcular horas trabajadas hoy: sumar pares CheckIn-CheckOut` | `// Calculate hours worked today: sum CheckIn-CheckOut pairs` |
| `Registro de Fichaje` | `Clock In / Clock Out` |
| `Registra tu hora de entrada o salida actual.` | `Record your check-in or check-out time.` |
| `ENTRADA` (badge inside foreach) | `CHECK IN` |
| `SALIDA` (badge inside foreach) | `CHECK OUT` |
| `Editar` (Edit button) | `Edit` |
| `Horas trabajadas hoy` | `Hours worked today` |
| `En curso` | `In progress` |
| `Historial de Hoy` | `Today's History` |
| `Hora` (table header) | `Time` |
| `Tipo` (table header) | `Type` |
| `Acciones` (table header) | `Actions` |
| `No hay registros para hoy.` | `No records for today.` |
| `// Reloj en tiempo real` | `// Live clock` |
| `// Anti-doble-click` | `// Prevent double-click` |
| `'Procesando...'` | `'Processing...'` |

Also translate the button text (keeping icon):
```html
<i class="bi bi-box-arrow-in-right"></i> CHECK IN
```
```html
<i class="bi bi-box-arrow-left"></i> CHECK OUT
```

**Step 2: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 3: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Attendance/Index.cshtml"
git commit -m "i18n: translate attendance dashboard to English"
```

---

### Task 3: Translate Attendance/History.cshtml and Attendance/Edit.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Attendance/History.cshtml`
- Modify: `FSP.AttendanceClock.Web/Views/Attendance/Edit.cshtml`

**Step 1: History.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Historial Completo"` | `ViewData["Title"] = "Full History"` |
| `Historial de Fichajes` | `Attendance History` |
| `Volver al Panel` | `Back to Dashboard` |
| `Desde` (label) | `From` |
| `Hasta` (label) | `To` |
| `Filtrar` (button) | `Filter` |
| `Limpiar` (button) | `Clear` |
| `Fecha y Hora` (table header) | `Date and Time` |
| `Tipo` (table header) | `Type` |
| `Acciones` (table header) | `Actions` |
| `ENTRADA` (badge) | `CHECK IN` |
| `SALIDA` (badge) | `CHECK OUT` |
| `Editar` (button) | `Edit` |
| `No hay registros para los filtros seleccionados.` | `No records found for the selected filters.` |

**Step 2: Edit.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Editar Fichaje"` | `ViewData["Title"] = "Edit Record"` |
| `Editar Fichaje` (card header h2) | `Edit Attendance Record` |
| `Hora actual registrada` | `Current recorded time` |
| `Nueva Hora` | `New Time` |
| `Motivo de la corrección` | `Reason for correction` |
| `Por favor explica por qué cambias la hora...` | `Please explain why you are changing this time...` |
| `Es obligatorio indicar un motivo.` | `A reason is required.` |
| `Guardar Cambios` | `Save Changes` |
| `Cancelar` | `Cancel` |

**Step 3: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Attendance/History.cshtml" "FSP.AttendanceClock.Web/Views/Attendance/Edit.cshtml"
git commit -m "i18n: translate attendance history and edit views to English"
```

---

### Task 4: Translate Account/Login.cshtml and Account/ChangePassword.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Account/Login.cshtml`
- Modify: `FSP.AttendanceClock.Web/Views/Account/ChangePassword.cshtml`

**Step 1: Login.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Iniciar Sesión"` | `ViewData["Title"] = "Sign In"` |
| `Bienvenido` | `Welcome` |
| `Introduce tus credenciales para acceder` | `Enter your credentials to sign in` |
| `Usuario` (label and placeholder) | `Username` |
| `Contraseña` (label and placeholder) | `Password` |
| `Entrar` (submit button) | `Sign In` |

**Step 2: ChangePassword.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Cambiar Contraseña"` | `ViewData["Title"] = "Change Password"` |
| `Cambiar Contraseña` (card header h4) | `Change Password` |
| `Contraseña Actual` (label) | `Current Password` |
| `Introduce tu contraseña actual` (placeholder) | `Enter your current password` |
| `Nueva Contraseña` (label) | `New Password` |
| `Nueva contraseña (mín. 6 caracteres)` (placeholder) | `New password (min. 8 characters)` |
| `Usa una contraseña fuerte con letras, números y símbolos` | `Use a strong password with letters, numbers and symbols` |
| `Confirmar Nueva Contraseña` (label) | `Confirm New Password` |
| `Confirma tu nueva contraseña` (placeholder) | `Confirm your new password` |
| `Cancelar` (button) | `Cancel` |
| `Cambiar Contraseña` (submit button) | `Change Password` |
| `Nota:` | `Note:` |
| `Después de cambiar tu contraseña, deberás iniciar sesión de nuevo.` | `After changing your password, you will be signed out and must sign in again.` |

**Step 3: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Account/Login.cshtml" "FSP.AttendanceClock.Web/Views/Account/ChangePassword.cshtml"
git commit -m "i18n: translate account views to English"
```

---

### Task 5: Translate Admin/Index.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Admin/Index.cshtml`

**Step 1: Replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Administración - Historial"` | `ViewData["Title"] = "Administration - History"` |
| `Panel de Administración` (h1) | `Administration Panel` |
| `Historial completo de fichajes` (p) | `Complete attendance records` |
| `Auditoría` (button text) | `Audit Log` |
| `Reporte Horario` (button text) | `Hours Report` |
| `asp-action="ReporteHorario"` | `asp-action="HoursReport"` |
| `Exportar Excel` | `Export Excel` |
| `Gestionar Usuarios` | `Manage Users` |
| `Registro General` (h3) | `All Records` |
| `Empleado` (table header) | `Employee` |
| `Fecha` (table header) | `Date` |
| `Hora` (table header) | `Time` |
| `Tipo` (table header) | `Type` |
| `Acciones` (table header) | `Actions` |
| `Entrada` (badge) | `Check In` |
| `Salida` (badge) | `Check Out` |
| `Editar` (button) | `Edit` |

> Note: `asp-action="ReporteHorario"` changes here; Task 8 will rename the controller method and view file to match.

**Step 2: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)` (the action rename will be completed in Task 8)

**Step 3: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Admin/Index.cshtml"
git commit -m "i18n: translate admin index view to English"
```

---

### Task 6: Translate Admin/Users.cshtml and Admin/CreateUser.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Admin/Users.cshtml`
- Modify: `FSP.AttendanceClock.Web/Views/Admin/CreateUser.cshtml`

**Step 1: Users.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Gestión de Usuarios"` | `ViewData["Title"] = "User Management"` |
| `Usuarios` (h1) | `Users` |
| `Gestionar empleados y administradores` (p) | `Manage employees and administrators` |
| `Nuevo Usuario` (button) | `New User` |
| `Volver` (button) | `Back` |
| `No hay usuarios creados aún.` | `No users created yet.` |
| `Crear primer usuario` | `Create first user` |
| `Usuario` (table header) | `Username` |
| `Rol` (table header) | `Role` |
| `Acciones` (table header) | `Actions` |
| `Administrador` (badge text) | `Administrator` |
| `Empleado` (badge text) | `Employee` |
| `Acciones` (dropdown button) | `Actions` |
| `Reset contraseña` | `Reset password` |
| `Eliminar` | `Delete` |
| `data-confirm="¿Estás seguro de que quieres eliminar este usuario? Esta acción no se puede deshacer."` | `data-confirm="Are you sure you want to delete this user? This action cannot be undone."` |

**Step 2: CreateUser.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Crear Usuario"` | `ViewData["Title"] = "Create User"` |
| `Crear Nuevo Usuario` (h2) | `Create New User` |
| `Rellena los detalles para el nuevo empleado` | `Fill in the details for the new employee` |
| `Nombre de Usuario` (label) | `Username` |
| `ej. juan.perez` (placeholder) | `e.g. john.doe` |
| `Contraseña` (label) | `Password` |
| `Contraseña inicial para el usuario.` | `Initial password for this user.` |
| `Confirmar contraseña` (label) | `Confirm password` |
| `Rol` (label) | `Role` |
| `Cancelar` (button) | `Cancel` |
| `Crear Usuario` (submit button) | `Create User` |

**Step 3: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Admin/Users.cshtml" "FSP.AttendanceClock.Web/Views/Admin/CreateUser.cshtml"
git commit -m "i18n: translate user management views to English"
```

---

### Task 7: Translate Admin/ResetPassword.cshtml and Admin/AuditLog.cshtml

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Admin/ResetPassword.cshtml`
- Modify: `FSP.AttendanceClock.Web/Views/Admin/AuditLog.cshtml`

**Step 1: ResetPassword.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Restablecer Contraseña"` | `ViewData["Title"] = "Reset Password"` |
| `Restablecer Contraseña para @Model.Username` | `Reset Password for @Model.Username` |
| `Nueva Contraseña` (label) | `New Password` |
| `Introduce la nueva contraseña para el empleado.` | `Enter the new password for this employee.` |
| `Restablecer Contraseña` (submit button) | `Reset Password` |
| `Cancelar` | `Cancel` |

**Step 2: AuditLog.cshtml replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Auditoría Universal"` | `ViewData["Title"] = "Universal Audit Log"` |
| `Exportar Auditoría` | `Export Audit Log` |
| `Volver` | `Back` |
| `Auditoría Completa` (h1) | `Full Audit Log` |
| `Registro de todas las acciones del sistema (Fichajes, Ediciones, Usuarios)` | `Record of all system actions (Attendance, Edits, Users)` |
| `Fecha` (header) | `Date` |
| `Usuario (Actor)` (header) | `User (Actor)` |
| `Acción` (header) | `Action` |
| `Detalles` (header) | `Details` |
| `Anterior` (pagination) | `Previous` |
| `Siguiente` (pagination) | `Next` |

For the row class logic (the `.Contains()` checks), update the Spanish action name strings to also match English ones — or leave as-is since `log.Action` values are stored in English already (CheckIn, CheckOut, Edit, User). The badge labels:

| Spanish | English |
|---------|---------|
| `Entrada` (badge) | `Check In` |
| `Salida` (badge) | `Check Out` |
| `Edición` (badge) | `Edit` |
| `Usuario` (badge) | `User` |

**Step 3: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Admin/ResetPassword.cshtml" "FSP.AttendanceClock.Web/Views/Admin/AuditLog.cshtml"
git commit -m "i18n: translate reset password and audit log views to English"
```

---

### Task 8: Rename ReporteHorario → HoursReport (controller + view file)

This is the most surgical task. Four changes must happen atomically:
1. Rename view file on disk
2. Rename two controller action methods
3. Update `asp-action` references in views
4. Update Excel filename string

**Files:**
- Rename: `FSP.AttendanceClock.Web/Views/Admin/ReporteHorario.cshtml` → `FSP.AttendanceClock.Web/Views/Admin/HoursReport.cshtml`
- Modify: `FSP.AttendanceClock.Web/Controllers/AdminController.cs` (lines ~324 and ~384)
- Already done in Task 5: `Views/Admin/Index.cshtml` `asp-action="HoursReport"` ✓

**Step 1: Rename the view file**

```bash
cd "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.Web\Views\Admin"
git mv ReporteHorario.cshtml HoursReport.cshtml
```

**Step 2: Rename the controller actions**

In `AdminController.cs`, find and replace:

```csharp
public async Task<IActionResult> ReporteHorario(int? userId, DateTime? startDate, DateTime? endDate)
```
→
```csharp
public async Task<IActionResult> HoursReport(int? userId, DateTime? startDate, DateTime? endDate)
```

And:

```csharp
public async Task<IActionResult> ExportReporteHorarioToExcel(int? userId, DateTime? startDate, DateTime? endDate)
```
→
```csharp
public async Task<IActionResult> ExportHoursReport(int? userId, DateTime? startDate, DateTime? endDate)
```

Also update the Excel filename string (~line 447):

```csharp
var fileName = $"ReporteHorario_{user.Username}_{DateTime.Now:yyyyMMdd}.xlsx";
```
→
```csharp
var fileName = $"HoursReport_{user.Username}_{DateTime.Now:yyyyMMdd}.xlsx";
```

**Step 3: Update asp-action reference in the newly renamed HoursReport.cshtml**

In `Views/Admin/HoursReport.cshtml` (just renamed), line 7:

```html
<form method="get" asp-action="ReporteHorario" class="row g-3">
```
→
```html
<form method="get" asp-action="HoursReport" class="row g-3">
```

And line 43:

```html
<a asp-action="ExportReporteHorarioToExcel" asp-route-userId="@ViewBag.SelectedUserId" ...>
```
→
```html
<a asp-action="ExportHoursReport" asp-route-userId="@ViewBag.SelectedUserId" ...>
```

**Step 4: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 5: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add -A
git commit -m "refactor: rename ReporteHorario to HoursReport (action, view, export)"
```

---

### Task 9: Translate HoursReport.cshtml content

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Admin/HoursReport.cshtml`

**Step 1: Replacements**

| Spanish | English |
|---------|---------|
| `ViewData["Title"] = "Reporte Horario"` | `ViewData["Title"] = "Hours Report"` |
| `Reporte Horario` (h2) | `Hours Report` |
| `Usuario` (form label) | `User` |
| `Selecciona un usuario` (option) | `Select a user` |
| `Desde` (form label) | `From` |
| `Hasta` (form label) | `To` |
| `Generar` (button) | `Generate` |
| `Exportar a Excel` | `Export to Excel` |
| `Historial - @ViewBag.Username` | `History - @ViewBag.Username` |
| `Horas ordinarias:` | `Ordinary hours:` |
| `Horas extra:` | `Extra hours:` |
| `Fecha` (table header) | `Date` |
| `Hora Entrada` (table header) | `Check-In Time` |
| `Hora Salida` (table header) | `Check-Out Time` |
| `Horas Trabajadas` (table header) | `Hours Worked` |
| `Horas Ordinarias (<= 8h)` (table header) | `Ordinary Hours (<= 8h)` |
| `Horas Extra` (table header) | `Extra Hours` |
| `No hay registros para el usuario seleccionado en el rango de fechas.` | `No records found for the selected user in this date range.` |

**Step 2: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 3: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Views/Admin/HoursReport.cshtml"
git commit -m "i18n: translate hours report view content to English"
```

---

### Task 10: Translate TempData messages in controllers

**Files:**
- Modify: `FSP.AttendanceClock.Web/Controllers/AttendanceController.cs`
- Modify: `FSP.AttendanceClock.Web/Controllers/AdminController.cs`
- Modify: `FSP.AttendanceClock.Web/Controllers/AccountController.cs`

**Step 1: AttendanceController.cs — replace TempData strings**

| Spanish | English |
|---------|---------|
| `"Ya has fichado entrada. Debes fichar salida primero."` | `"You have already checked in. You must check out first."` |
| `"Entrada registrada correctamente."` | `"Check-in recorded successfully."` |
| `"No has fichado entrada o ya has fichado salida."` | `"You have not checked in, or you have already checked out."` |
| `"Salida registrada correctamente."` | `"Check-out recorded successfully."` |
| `"Fichaje actualizado y auditado correctamente."` | `"Attendance record updated and audited successfully."` |

**Step 2: AdminController.cs — replace TempData strings**

| Spanish | English |
|---------|---------|
| `"Las contraseñas no coinciden."` | `"Passwords do not match."` |
| `"La contraseña debe tener al menos 8 caracteres."` (×2 occurrences) | `"Password must be at least 8 characters."` |
| `$"Contraseña de {user.Username} restablecida correctamente."` | `$"Password for {user.Username} has been reset successfully."` |

**Step 3: AccountController.cs — replace TempData strings**

| Spanish | English |
|---------|---------|
| `"Contraseña cambiada exitosamente. Por favor inicia sesión de nuevo."` | `"Password changed successfully. Please sign in again."` |

**Step 4: Also translate any Spanish code comments** in these three controllers while you have them open. Look for lines starting with `//` that contain Spanish words and translate them to English. (There are not many — this is opportunistic cleanup, not a requirement.)

**Step 5: Build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 6: Commit**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add "FSP.AttendanceClock.Web/Controllers/AttendanceController.cs" "FSP.AttendanceClock.Web/Controllers/AdminController.cs" "FSP.AttendanceClock.Web/Controllers/AccountController.cs"
git commit -m "i18n: translate controller TempData messages and comments to English"
```

---

### Task 11: Translate remaining shared views and push to GitHub

**Files:**
- Modify: `FSP.AttendanceClock.Web/Views/Shared/_Notifications.cshtml` (none needed — no user-visible text, only TempData values)
- Modify: `FSP.AttendanceClock.Web/Views/Home/Privacy.cshtml` (minimal, likely auto-generated)
- Modify: `FSP.AttendanceClock.Web/Views/Home/Index.cshtml` (check for Spanish text)
- Push to GitHub

**Step 1: Check Home/Index.cshtml for Spanish text**

Read the file. If it has Spanish text, translate it. If it's the default empty template, no change needed.

**Step 2: Check Home/Privacy.cshtml for Spanish text**

Same approach. The default ASP.NET template has English text; translate any Spanish if present.

**Step 3: Final build**

```bash
dotnet build "D:\Proyectos\FSP Attendance Clock\FSP.AttendanceClock.sln"
```

Expected: `Build succeeded. 0 Error(s)`

**Step 4: Commit any remaining changes**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git add -A
git commit -m "i18n: translate remaining views; complete full English translation"
```

**Step 5: Push Part 0 + Part 1 to GitHub**

```bash
cd "D:\Proyectos\FSP Attendance Clock"
git push origin main
```

Verify: open https://github.com/FSP-Labs/FSP.AttendanceClock — CLAUDE.md should not appear, all UI text in repo should be English.

---

## Part 2 — Personal GitHub Profile (saasixx)

> Creates the special `saasixx/saasixx` repo. GitHub renders its README.md as the profile page.

### Task 12: Create saasixx personal profile README

**No project files modified.** This task works entirely through `gh` CLI.

**Step 1: Create the repo (if it doesn't exist)**

```bash
gh repo create saasixx/saasixx --public --description "Profile README" --confirm
```

If it already exists, clone it:
```bash
gh repo clone saasixx/saasixx
```

**Step 2: Create the README.md**

Create file `README.md` in the cloned `saasixx` repo with this content:

```markdown
# hey, I'm Felix 👋

I build things. Sometimes they work. Sometimes they teach me why they didn't.

Software, hardware, the stuff in between — I make all of it.

---

### what I'm up to

- 🔧 Building tools that solve real problems — check out [FSP Labs](https://github.com/FSP-Labs)
- 🛠️ Making things with my hands (yes, physical things too)
- 🧪 Breaking stuff so I can learn how to fix it

---

### find me

[![GitHub](https://img.shields.io/badge/FSP--Labs-org-181717?logo=github)](https://github.com/FSP-Labs)

---

*if it compiles, ship it* 🚀
```

**Step 3: Commit and push**

```bash
cd saasixx
git add README.md
git commit -m "feat: add profile README"
git push
```

**Step 4: Verify**

Open https://github.com/saasixx — the README content should appear as the profile page.

---

## Part 3 — FSP-Labs Organization Profile

> Creates the `FSP-Labs/.github` special repo. GitHub renders `profile/README.md` as the org profile page.

### Task 13: Create FSP-Labs org profile README

**No project files modified.** This task works entirely through `gh` CLI.

**Step 1: Create the `.github` repo under FSP-Labs**

```bash
gh repo create FSP-Labs/.github --public --description "FSP Labs organization profile" --confirm
```

If it already exists, clone it:
```bash
gh repo clone FSP-Labs/.github
```

**Step 2: Create the profile directory and README**

```bash
cd .github
mkdir -p profile
```

Create `profile/README.md` with this content:

```markdown
# FSP Labs

Open-source tools built to solve real problems.

---

## Projects

| Project | Description |
|---------|-------------|
| [FSP.AttendanceClock](https://github.com/FSP-Labs/FSP.AttendanceClock) | Employee attendance system — clock in/out, admin reports, audit logs |

---

## About

FSP Labs publishes practical, production-grade software.
Clean architecture. Real deployments. No bloat.

---

*Built by [@saasixx](https://github.com/saasixx)*
```

**Step 3: Commit and push**

```bash
cd .github
git add profile/README.md
git commit -m "feat: add organization profile README"
git push
```

**Step 4: Verify**

Open https://github.com/FSP-Labs — the README content should appear as the organization profile.

---

## Summary — Execution Order

| # | Task | Commit message |
|---|------|----------------|
| 0 | Hide CLAUDE.md | `chore: exclude CLAUDE.md from version control` |
| 1 | Layout/navbar | `i18n: translate layout/navbar to English` |
| 2 | Attendance dashboard | `i18n: translate attendance dashboard to English` |
| 3 | History + Edit views | `i18n: translate attendance history and edit views to English` |
| 4 | Login + ChangePassword | `i18n: translate account views to English` |
| 5 | Admin Index | `i18n: translate admin index view to English` |
| 6 | Users + CreateUser | `i18n: translate user management views to English` |
| 7 | ResetPassword + AuditLog | `i18n: translate reset password and audit log views to English` |
| 8 | Rename ReporteHorario→HoursReport | `refactor: rename ReporteHorario to HoursReport` |
| 9 | HoursReport content | `i18n: translate hours report view content to English` |
| 10 | Controller TempData messages | `i18n: translate controller TempData messages and comments to English` |
| 11 | Remaining views + push | `i18n: translate remaining views; complete full English translation` |
| 12 | saasixx profile | (in saasixx repo) |
| 13 | FSP-Labs org profile | (in .github repo) |
