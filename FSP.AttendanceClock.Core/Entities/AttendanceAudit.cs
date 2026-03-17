using System;

namespace FSP.AttendanceClock.Core.Entities
{
    public class AttendanceAudit
    {
        public int Id { get; set; }
        
        public int AttendanceId { get; set; }
        public Attendance Attendance { get; set; } = null!;
        
        public int ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; } = null!;
        
        public DateTime ChangeDate { get; set; }
        
        public DateTime OldTimestamp { get; set; }
        public DateTime NewTimestamp { get; set; }
        
        public string Reason { get; set; } = string.Empty;
    }
}
