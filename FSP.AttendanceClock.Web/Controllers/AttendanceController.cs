using System;
using System.Linq;
using System.Threading.Tasks;
using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Infrastructure.Data;
using FSP.AttendanceClock.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FSP.AttendanceClock.Web.Controllers
{
    /// <summary>
    /// Controlador principal para las acciones del empleado (Fichar, Ver historial).
    /// </summary>
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public AttendanceController(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        /// <summary>
        /// Muestra el panel de fichajes del usuario actual con el historial del día.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = User.GetCurrentUserId();
            
            // Obtener fichajes de hoy para el usuario actual
            var today = DateTime.UtcNow.Date;
            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId && a.Timestamp.Date == today)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return View(attendances);
        }

        /// <summary>
        /// Muestra el historial completo de fichajes del usuario (con filtros opcionales).
        /// </summary>
        public async Task<IActionResult> History(DateTime? from, DateTime? to)
        {
            var userId = User.GetCurrentUserId();

            var query = _context.Attendances
                .Where(a => a.UserId == userId);

            if (from.HasValue)
            {
                query = query.Where(a => a.Timestamp >= from.Value.ToUniversalTime());
            }
            if (to.HasValue)
            {
                // include end of day by default if time not specified
                query = query.Where(a => a.Timestamp <= to.Value.ToUniversalTime());
            }

            var results = await query.OrderByDescending(a => a.Timestamp).ToListAsync();

            ViewData["From"] = from?.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
            ViewData["To"] = to?.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

            return View(results);
        }

        /// <summary>
        /// Registra una ENTRADA para el usuario actual.
        /// Valida que no haya una entrada pendiente de cerrar.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var userId = User.GetCurrentUserId();
            
            var lastRecord = await _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefaultAsync();

            if (lastRecord != null && lastRecord.Type == AttendanceType.CheckIn)
            {
                TempData["Error"] = "Ya has fichado entrada. Debes fichar salida primero.";
                return RedirectToAction(nameof(Index));
            }
            
            var attendance = new Attendance
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                Type = AttendanceType.CheckIn
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            
            var username = User.Identity!.Name!;
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(userId, username, "Entrada", $"Usuario fichó entrada a las {attendance.Timestamp}", ip, userAgent);
            TempData["Success"] = "Entrada registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Registra una SALIDA para el usuario actual.
        /// Valida que haya una entrada abierta.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var userId = User.GetCurrentUserId();
            
            var lastRecord = await _context.Attendances
                .Where(a => a.UserId == userId && a.Timestamp.Date == DateTime.UtcNow.Date)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefaultAsync();

            if (lastRecord == null || lastRecord.Type == AttendanceType.CheckOut)
            {
                TempData["Error"] = "No has fichado entrada o ya has fichado salida.";
                return RedirectToAction(nameof(Index));
            }
            
            var attendance = new Attendance
            {
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                Type = AttendanceType.CheckOut
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            
            var username = User.Identity!.Name!;
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(userId, username, "Salida", $"Usuario fichó salida a las {attendance.Timestamp}", ip, userAgent);

            TempData["Success"] = "Salida registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Muestra el formulario para editar un fichaje existente.
        /// </summary>
        /// <param name="id">ID del fichaje.</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.GetCurrentUserId();
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null) return NotFound();

            if (attendance.UserId != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            return View(attendance);
        }

        /// <summary>
        /// Procesa la edición de un fichaje, creando un registro de auditoría.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime newTimestamp, string reason)
        {
            var userId = User.GetCurrentUserId();
            var attendance = await _context.Attendances.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null) return NotFound();

            if (attendance.UserId != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            if (newTimestamp > DateTime.UtcNow.AddMinutes(5))
            {
                ModelState.AddModelError("", "No puedes registrar fichajes en el futuro.");
                return View(attendance);
            }

            var audit = new AttendanceAudit
            {
                AttendanceId = attendance.Id,
                ChangedByUserId = userId,
                ChangeDate = DateTime.UtcNow,
                OldTimestamp = attendance.Timestamp,
                NewTimestamp = newTimestamp.ToUniversalTime(),
                Reason = reason
            };

            attendance.Timestamp = newTimestamp.ToUniversalTime();

            _context.AttendanceAudits.Add(audit);
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

            var username = User.Identity!.Name!;
            string affectedUser = attendance.User.Username;
            
            var spanishZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var oldTimeSpanish = TimeZoneInfo.ConvertTimeFromUtc(audit.OldTimestamp, spanishZone);
            var newTimeSpanish = TimeZoneInfo.ConvertTimeFromUtc(audit.NewTimestamp, spanishZone);
            
            string logDetail = $"Editado por: {username} | Usuario Afectado: {affectedUser} | Registro {attendance.Id} cambiado de {oldTimeSpanish:G} a {newTimeSpanish:G}. Motivo: {reason}";

            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(userId, username, "EdicionFichaje", logDetail, ip, userAgent);

            TempData["Success"] = "Fichaje actualizado y auditado correctamente.";
            
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
