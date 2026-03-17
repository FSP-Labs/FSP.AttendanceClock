namespace FSP.AttendanceClock.Web.Configuration;

public class AttendanceClockSettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
    public double OrdinaryHoursPerDay { get; set; } = 8.0;
    public string TimeZoneId { get; set; } = "Central European Standard Time";
}
