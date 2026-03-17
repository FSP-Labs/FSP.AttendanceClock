using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Core.Models;

namespace FSP.AttendanceClock.Core.Interfaces;

public interface IAttendanceReportService
{
    IReadOnlyList<DailyHoursSummary> CalculateDailyHours(
        IEnumerable<Attendance> attendances,
        double ordinaryHoursPerDay);
}
