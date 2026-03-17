namespace FSP.AttendanceClock.Core.Models;

public record DailyHoursSummary(
    DateTime Date,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    TimeSpan WorkedHours,
    TimeSpan OrdinaryHours,
    TimeSpan ExtraHours
);
