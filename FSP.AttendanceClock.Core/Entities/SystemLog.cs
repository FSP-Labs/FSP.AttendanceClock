using System;

namespace FSP.AttendanceClock.Core.Entities
{
    public class SystemLog
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Puede ser nulo si la acción fue del sistema o el usuario fue eliminado; normalmente indica el actor.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Snapshot del nombre de usuario por si el usuario es eliminado posteriormente.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Acción registrada (por ejemplo: "CreateUser", "DeleteUser", "CheckIn", "CheckOut", "EditAttendance").
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Detalles adicionales (texto o JSON) sobre la acción.
        /// </summary>
        public string Details { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
