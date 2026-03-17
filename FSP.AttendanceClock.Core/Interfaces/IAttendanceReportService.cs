using FSP.AttendanceClock.Core.Entities;

namespace FSP.AttendanceClock.Core.Interfaces;

public record DailyHoursSummary(
    DateTime Date,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    TimeSpan WorkedHours,
    TimeSpan OrdinaryHours,
    TimeSpan ExtraHours
);

public interface IAttendanceReportService
{
    IReadOnlyList<DailyHoursSummary> CalculateDailyHours(
        IEnumerable<Attendance> attendances,
        double ordinaryHoursPerDay);
}
