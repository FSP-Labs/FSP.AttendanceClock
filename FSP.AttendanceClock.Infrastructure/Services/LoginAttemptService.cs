using System;
using System.Collections.Concurrent;
using System.Linq;
using FSP.AttendanceClock.Core.Interfaces;

namespace FSP.AttendanceClock.Infrastructure.Services
{
    /// <summary>
    /// Servicio para prevenir ataques de fuerza bruta limitando intentos de login por IP.
    /// </summary>
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly ConcurrentDictionary<string, LoginAttemptData> _attempts
            = new ConcurrentDictionary<string, LoginAttemptData>();

        private readonly int _maxAttempts;
        private readonly TimeSpan _lockoutDuration;
        private readonly TimeSpan _attemptWindow;

        public LoginAttemptService(int maxAttempts, int lockoutMinutes)
        {
            _maxAttempts = maxAttempts;
            _lockoutDuration = TimeSpan.FromMinutes(lockoutMinutes);
            _attemptWindow = TimeSpan.FromMinutes(lockoutMinutes);
        }

        /// <summary>
        /// Registra un intento fallido de login para una IP.
        /// Si se alcanza el máximo de intentos, bloquea la IP temporalmente.
        /// </summary>
        public void RecordFailedAttempt(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return;

            var now = DateTime.UtcNow;
            _attempts.AddOrUpdate(ipAddress, 
                new LoginAttemptData { FirstAttempt = now, FailedCount = 1, LastAttempt = now },
                (key, existing) =>
                {
                    // Si pasó más tiempo que la ventana, reiniciar contador
                    if ((now - existing.FirstAttempt) > _attemptWindow)
                    {
                        return new LoginAttemptData { FirstAttempt = now, FailedCount = 1, LastAttempt = now };
                    }

                    // Incrementar intentos fallidos
                    existing.FailedCount++;
                    existing.LastAttempt = now;

                    // Si llegamos al máximo, marcar como bloqueado
                    if (existing.FailedCount >= _maxAttempts)
                    {
                        existing.LockedUntil = now.Add(_lockoutDuration);
                    }

                    return existing;
                });
        }

        /// <summary>
        /// Registra un intento exitoso de login, limpiando el historial de intentos fallidos.
        /// </summary>
        public void RecordSuccessfulAttempt(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return;
            _attempts.TryRemove(ipAddress, out _);
        }

        /// <summary>
        /// Verifica si una IP está bloqueada temporalmente.
        /// </summary>
        public bool IsBlocked(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return false;

            if (_attempts.TryGetValue(ipAddress, out var data))
            {
                var now = DateTime.UtcNow;

                // Si el lockout expiró, limpiar registro
                if (data.LockedUntil.HasValue && now > data.LockedUntil.Value)
                {
                    _attempts.TryRemove(ipAddress, out _);
                    return false;
                }

                // Si está en período de lockout, está bloqueado
                if (data.LockedUntil.HasValue && now <= data.LockedUntil.Value)
                {
                    return true;
                }

                // Si pasó más tiempo que la ventana, limpiar registro
                if ((now - data.FirstAttempt) > _attemptWindow)
                {
                    _attempts.TryRemove(ipAddress, out _);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Retorna el número de intentos fallidos para una IP.
        /// </summary>
        public int GetFailedAttempts(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return 0;
            return _attempts.TryGetValue(ipAddress, out var data) ? data.FailedCount : 0;
        }

        private class LoginAttemptData
        {
            public DateTime FirstAttempt { get; set; }
            public DateTime LastAttempt { get; set; }
            public int FailedCount { get; set; }
            public DateTime? LockedUntil { get; set; }
        }
    }
}
