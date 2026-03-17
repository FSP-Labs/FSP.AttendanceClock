using System;
using System.Collections.Generic;

namespace FSP.AttendanceClock.Web.Models.Admin
{
    public class DayRecord
    {
        public DateTime Date { get; set; }
        public DateTime? FirstCheckIn { get; set; }
        public DateTime? LastCheckOut { get; set; }
    }

    public class UserDailyHistoryViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DayRecord> Records { get; set; } = new List<DayRecord>();
    }
}
