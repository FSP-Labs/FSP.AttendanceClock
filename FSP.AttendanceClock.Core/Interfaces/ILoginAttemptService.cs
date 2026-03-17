namespace FSP.AttendanceClock.Core.Interfaces
{
    public interface ILoginAttemptService
    {
        void RecordFailedAttempt(string ipAddress);
        void RecordSuccessfulAttempt(string ipAddress);
        bool IsBlocked(string ipAddress);
        int GetFailedAttempts(string ipAddress);
    }
}
