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
    /// Main controller for employee attendance actions (Clock In, View history).
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
        /// Displays the attendance panel for the current user with today's history.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = User.GetCurrentUserId();
            
            // Get today's attendance records for the current user
            var today = DateTime.UtcNow.Date;
            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId && a.Timestamp.Date == today)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return View(attendances);
        }

        /// <summary>
        /// Displays the full attendance history for the user (with optional filters).
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
        /// Records a CHECK-IN for the current user.
        /// Validates that there is no open check-in pending a check-out.
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
                TempData["Error"] = "You have already checked in. You must check out first.";
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
            await _auditService.LogAsync(userId, username, "CheckIn", $"User checked in at {attendance.Timestamp.ToLocalTime():HH:mm}", ip, userAgent);
            TempData["Success"] = "Check-in recorded successfully.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Records a CHECK-OUT for the current user.
        /// Validates that there is an open check-in.
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
                TempData["Error"] = "You have not checked in, or you have already checked out.";
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
            await _auditService.LogAsync(userId, username, "CheckOut", $"User checked out at {attendance.Timestamp.ToLocalTime():HH:mm}", ip, userAgent);

            TempData["Success"] = "Check-out recorded successfully.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the form to edit an existing attendance record.
        /// </summary>
        /// <param name="id">Attendance record ID.</param>
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
        /// Processes the edit of an attendance record, creating an audit entry.
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
                ModelState.AddModelError("", "You cannot record attendance entries in the future.");
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
            
            string logDetail = $"Edited by: {username} | Affected user: {affectedUser} | Record {attendance.Id} changed from {oldTimeSpanish:G} to {newTimeSpanish:G}. Reason: {reason}";

            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500) userAgent = userAgent[..500];
            await _auditService.LogAsync(userId, username, "EditRecord", logDetail, ip, userAgent);

            TempData["Success"] = "Attendance record updated and audited successfully.";
            
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
