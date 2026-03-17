using System;

namespace FSP.AttendanceClock.Core.Entities
{
    public enum AttendanceType
    {
        CheckIn,
        CheckOut
    }

    public class Attendance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public AttendanceType Type { get; set; }
        
        /// <summary>
        /// Propiedad de navegación al usuario propietario del fichaje.
        /// </summary>
        public User User { get; set; } = null!;
    }
}
