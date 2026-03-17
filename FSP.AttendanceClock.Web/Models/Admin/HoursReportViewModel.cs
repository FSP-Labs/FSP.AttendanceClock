using System;
using System.Collections.Generic;

namespace FSP.AttendanceClock.Web.Models.Admin
{
    public class UserHoursResult
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public TimeSpan Total { get; set; }
    }

    public class HoursReportViewModel
    {
        public int? SelectedUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<UserHoursResult> Results { get; set; } = new List<UserHoursResult>();
    }
}
