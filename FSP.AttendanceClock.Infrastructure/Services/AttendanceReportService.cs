using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Core.Models;

namespace FSP.AttendanceClock.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de cálculo de horas para reportes de asistencia.
/// Agrupa los fichajes por día y calcula horas trabajadas, ordinarias y extra.
/// </summary>
public class AttendanceReportService : IAttendanceReportService
{
    /// <summary>
    /// Calcula el resumen diario de horas a partir de una colección de fichajes.
    /// </summary>
    /// <param name="attendances">Registros de asistencia del usuario en el periodo.</param>
    /// <param name="ordinaryHoursPerDay">Umbral de horas ordinarias por día.</param>
    public IReadOnlyList<DailyHoursSummary> CalculateDailyHours(
        IEnumerable<Attendance> attendances,
        double ordinaryHoursPerDay)
    {
        var ordinaryThreshold = TimeSpan.FromHours(ordinaryHoursPerDay);

        var result = attendances
            .GroupBy(a => a.Timestamp.ToLocalTime().Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var firstCheckIn = g.Where(a => a.Type == AttendanceType.CheckIn).FirstOrDefault();
                var lastCheckOut = g.Where(a => a.Type == AttendanceType.CheckOut).LastOrDefault();

                var checkInLocal = firstCheckIn?.Timestamp.ToLocalTime();
                var checkOutLocal = lastCheckOut?.Timestamp.ToLocalTime();

                TimeSpan worked = TimeSpan.Zero;
                if (checkInLocal.HasValue && checkOutLocal.HasValue && checkOutLocal > checkInLocal)
                {
                    worked = checkOutLocal.Value - checkInLocal.Value;
                }

                var ordinary = worked > ordinaryThreshold ? ordinaryThreshold : worked;
                var extra = worked > ordinaryThreshold ? worked - ordinaryThreshold : TimeSpan.Zero;

                return new DailyHoursSummary(
                    Date: g.Key,
                    CheckInTime: checkInLocal,
                    CheckOutTime: checkOutLocal,
                    WorkedHours: worked,
                    OrdinaryHours: ordinary,
                    ExtraHours: extra
                );
            })
            .ToList();

        return result.AsReadOnly();
    }
}
