using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace FSP.AttendanceClock.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de auditoría usando la base de datos local.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Registra una acción del sistema en la base de datos de auditoría.
        /// </summary>
        /// <param name="userId">ID del usuario que realiza la acción.</param>
        /// <param name="username">Nombre de usuario (snapshot).</param>
        /// <param name="action">Tipo de acción (clave).</param>
        /// <param name="details">Detalles descriptivos.</param>
        /// <param name="ipAddress">Dirección IP opcional.</param>
        public async Task LogAsync(int? userId, string username, string action, string details, string? ipAddress = null, string? userAgent = null)
        {
            var log = new SystemLog
            {
                UserId = userId,
                Username = username,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
