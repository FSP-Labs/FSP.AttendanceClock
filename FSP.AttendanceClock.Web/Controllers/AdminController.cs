using System.Linq;
using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Infrastructure.Data;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Infrastructure.Security;
using FSP.AttendanceClock.Web.Configuration;
using FSP.AttendanceClock.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSP.AttendanceClock.Web.Controllers
{
    /// <summary>
    /// Controlador para tareas administrativas (Gestión de usuarios, Reportes, Auditoría).
    /// Requiere rol de "Administrador".
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IAttendanceReportService _attendanceReportService;
        private readonly AttendanceClockSettings _settings;

        public AdminController(AppDbContext context, IAuditService auditService, IAttendanceReportService attendanceReportService, IOptions<AttendanceClockSettings> settings)
        {
            _context = context;
            _auditService = auditService;
            _attendanceReportService = attendanceReportService;
            _settings = settings.Value;
        }

        /// <summary>
        /// Muestra el reporte general de fichajes con opciones de filtrado.
        /// </summary>
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? userId)
        {
            var query = _context.Attendances.Include(a => a.User).AsQueryable();

            if (startDate.HasValue) 
                query = query.Where(a => a.Timestamp >= startDate.Value.ToUniversalTime());
            
            if (endDate.HasValue) 
                query = query.Where(a => a.Timestamp < endDate.Value.AddDays(1).ToUniversalTime());
            
            if (userId.HasValue) 
                query = query.Where(a => a.UserId == userId.Value);

            var attendances = await query.OrderByDescending(a => a.Timestamp).ToListAsync();

            // Cargar datos para los filtros de la vista
            ViewBag.Users = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _context.Users.OrderBy(u => u.Username).ToListAsync(), "Id", "Username", userId);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(attendances);
        }

        /// <summary>
        /// Listado de todos los usuarios del sistema.
        /// </summary>
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.OrderBy(u => u.Username).ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Vista para crear nuevo usuario.
        /// </summary>
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        /// <summary>
        /// Procesa la creación de un nuevo usuario.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string username, string password, string confirmPassword, UserRole role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("", "El usuario ya existe.");
                return View();
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return RedirectToAction(nameof(CreateUser));
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                TempData["Error"] = "La contraseña debe tener al menos 8 caracteres.";
                return RedirectToAction(nameof(CreateUser));
            }

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Log de auditoría
            var adminId = User.GetCurrentUserId();
            var adminName = User.Identity!.Name!;
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(adminId, adminName, "UsuarioCreado", $"Usuario creado: {user.Username} con rol {user.Role}", ip, userAgent);

            return RedirectToAction(nameof(Users));
        }

        /// <summary>
        /// Elimina un usuario del sistema (Acción irreversible).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var username = user.Username;
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                var adminId = User.GetCurrentUserId();
                var adminName = User.Identity!.Name!;
                var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();
                if (userAgent.Length > 500) userAgent = userAgent[..500];
                await _auditService.LogAsync(adminId, adminName, "UsuarioEliminado", $"Usuario eliminado: {username}", ip, userAgent);
            }
            return RedirectToAction(nameof(Users));
        }

        /// <summary>
        /// Vista para restablecer contraseña de un usuario.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        /// <summary>
        /// Procesa el cambio de contraseña de un usuario.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                TempData["Error"] = "La contraseña debe tener al menos 8 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            var adminId = User.GetCurrentUserId();
            var adminName = User.Identity!.Name!;
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(adminId, adminName, "ContrasenaRestablecida", $"Contraseña restablecida para usuario: {user.Username}", ip, userAgent);
            
            TempData["Success"] = $"Contraseña de {user.Username} restablecida correctamente.";

            return RedirectToAction(nameof(Users));
        }

        /// <summary>
        /// Muestra el registro completo de auditoría del sistema.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AuditLog(int page = 1)
        {
            const int PageSize = 50;
            var totalCount = await _context.SystemLogs.CountAsync();
            var logs = await _context.SystemLogs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            return View(logs);
        }

        /// <summary>
        /// Exporta los fichajes a Excel, aplicando los filtros seleccionados.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate, int? userId)
        {
            var query = _context.Attendances.Include(a => a.User).AsQueryable();

            if (startDate.HasValue) 
                query = query.Where(a => a.Timestamp >= startDate.Value.ToUniversalTime());
            
            if (endDate.HasValue) 
                query = query.Where(a => a.Timestamp < endDate.Value.AddDays(1).ToUniversalTime());
            
            if (userId.HasValue) 
                query = query.Where(a => a.UserId == userId.Value);

            var attendances = await query.OrderByDescending(a => a.Timestamp).ToListAsync();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Fichajes");
                var currentRow = 1;

                // Cabecera
                worksheet.Cell(currentRow, 1).Value = "ID Empleado";
                worksheet.Cell(currentRow, 2).Value = "Nombre";
                worksheet.Cell(currentRow, 3).Value = "Fecha";
                worksheet.Cell(currentRow, 4).Value = "Hora";
                worksheet.Cell(currentRow, 5).Value = "Tipo";
                
                // Estilo Cabecera
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                foreach (var item in attendances)
                {
                    currentRow++;
                    var localTime = item.Timestamp.ToLocalTime();
                    
                    worksheet.Cell(currentRow, 1).Value = item.User.Id;
                    worksheet.Cell(currentRow, 2).Value = item.User.Username;
                    worksheet.Cell(currentRow, 3).Value = localTime.ToShortDateString();
                    worksheet.Cell(currentRow, 4).Value = localTime.ToShortTimeString();
                    worksheet.Cell(currentRow, 5).Value = item.Type == AttendanceType.CheckIn ? "Entrada" : "Salida";
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Fichajes_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }

        /// <summary>
        /// Exporta el log de auditoría completo a Excel.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportAuditLog()
        {
            var logs = await _context.SystemLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("System Logs");
                var currentRow = 1;

                // Cabecera
                worksheet.Cell(currentRow, 1).Value = "Fecha";
                worksheet.Cell(currentRow, 2).Value = "Usuario (Actor)";
                worksheet.Cell(currentRow, 3).Value = "Acción";
                worksheet.Cell(currentRow, 4).Value = "Detalles";
                worksheet.Cell(currentRow, 5).Value = "IP";
                worksheet.Cell(currentRow, 6).Value = "User-Agent";

                // Estilo Cabecera
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                foreach (var item in logs)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Timestamp.ToLocalTime();
                    worksheet.Cell(currentRow, 2).Value = item.Username ?? "System";
                    worksheet.Cell(currentRow, 3).Value = item.Action;
                    worksheet.Cell(currentRow, 4).Value = item.Details;
                    worksheet.Cell(currentRow, 5).Value = item.IpAddress;
                    worksheet.Cell(currentRow, 6).Value = item.UserAgent;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Logs_Sistema_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }

        }

        /// <summary>
        /// Sección: Reporte Horario (panel administrativo)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReporteHorario(int? userId, DateTime? startDate, DateTime? endDate)
        {
            // Preparar lista de usuarios para selector
            ViewBag.Users = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _context.Users.OrderBy(u => u.Username).ToListAsync(), "Id", "Username");
            
            var start = startDate?.Date ?? DateTime.UtcNow.Date.AddDays(-7);
            var end = endDate?.Date ?? DateTime.UtcNow.Date;
            
            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");
            ViewBag.SelectedUserId = userId;

            var attendanceRecords = new List<dynamic>();
            double totalOrdinaryHours = 0;
            double totalExtraHours = 0;

            // Si se seleccionó un usuario, obtener sus registros de entrada/salida
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    var attendances = await _context.Attendances
                        .Where(a => a.UserId == userId.Value && a.Timestamp >= start.ToUniversalTime() && a.Timestamp < end.AddDays(1).ToUniversalTime())
                        .ToListAsync();

                    // Agrupar por día: primera entrada y última salida
                    var dailySummaries = _attendanceReportService.CalculateDailyHours(attendances, _settings.OrdinaryHoursPerDay);

                    foreach (var day in dailySummaries)
                    {
                        totalOrdinaryHours += day.OrdinaryHours.TotalHours;
                        totalExtraHours += day.ExtraHours.TotalHours;

                        attendanceRecords.Add(new
                        {
                            Date = day.Date.ToString("dd/MM/yyyy"),
                            CheckInTime = day.CheckInTime?.ToString("HH:mm") ?? "-",
                            CheckOutTime = day.CheckOutTime?.ToString("HH:mm") ?? "-",
                            WorkedHours = day.WorkedHours,
                            OrdinaryHours = day.OrdinaryHours,
                            ExtraHours = day.ExtraHours
                        });
                    }
                    
                    ViewBag.Username = user.Username;
                }
            }

            ViewBag.AttendanceRecords = attendanceRecords;
            ViewBag.TotalOrdinaryHours = totalOrdinaryHours;
            ViewBag.TotalExtraHours = totalExtraHours;

            return View();
        }

        /// <summary>
        /// Exporta el reporte horario a Excel.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportReporteHorarioToExcel(int? userId, DateTime? startDate, DateTime? endDate)
        {
            if (!userId.HasValue) return BadRequest("Se requiere seleccionar un usuario");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return NotFound();

            var start = startDate?.Date ?? DateTime.UtcNow.Date.AddDays(-7);
            var end = endDate?.Date ?? DateTime.UtcNow.Date;

            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId.Value && a.Timestamp >= start.ToUniversalTime() && a.Timestamp < end.AddDays(1).ToUniversalTime())
                .ToListAsync();

            // Agrupar por día
            var dailySummaries = _attendanceReportService.CalculateDailyHours(attendances, _settings.OrdinaryHoursPerDay);

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Horario");
                var row = 1;

                // Cabecera
                worksheet.Cell(row, 1).Value = "Fecha";
                worksheet.Cell(row, 2).Value = "Hora Entrada";
                worksheet.Cell(row, 3).Value = "Hora Salida";
                worksheet.Cell(row, 4).Value = "Horas Trabajadas";
                worksheet.Cell(row, 5).Value = "Horas Ordinarias";
                worksheet.Cell(row, 6).Value = "Horas Extra";
                var header = worksheet.Range("A1:F1");
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                double totalOrdinary = 0;
                double totalExtra = 0;

                foreach (var day in dailySummaries)
                {
                    row++;
                    totalOrdinary += day.OrdinaryHours.TotalHours;
                    totalExtra += day.ExtraHours.TotalHours;

                    worksheet.Cell(row, 1).Value = day.Date.ToShortDateString();
                    worksheet.Cell(row, 2).Value = day.CheckInTime?.ToString("HH:mm") ?? "-";
                    worksheet.Cell(row, 3).Value = day.CheckOutTime?.ToString("HH:mm") ?? "-";
                    worksheet.Cell(row, 4).Value = Math.Round(day.WorkedHours.TotalHours, 2);
                    worksheet.Cell(row, 5).Value = Math.Round(day.OrdinaryHours.TotalHours, 2);
                    worksheet.Cell(row, 6).Value = Math.Round(day.ExtraHours.TotalHours, 2);
                }

                // Totales
                row++;
                worksheet.Cell(row, 1).Value = "Totales";
                worksheet.Cell(row, 5).Value = Math.Round(totalOrdinary, 2);
                worksheet.Cell(row, 6).Value = Math.Round(totalExtra, 2);
                worksheet.Range($"A{row}:F{row}").Style.Font.Bold = true;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"ReporteHorario_{user.Username}_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        }
}
