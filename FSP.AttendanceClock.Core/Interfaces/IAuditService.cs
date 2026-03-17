using System.Threading.Tasks;

namespace FSP.AttendanceClock.Core.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string username, string action, string details, string? ipAddress = null, string? userAgent = null);
    }
}
