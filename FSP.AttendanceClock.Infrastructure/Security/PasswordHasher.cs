using System;
using System.Security.Cryptography;

namespace FSP.AttendanceClock.Infrastructure.Security
{
    public static class PasswordHasher
    {
        // Formato: {iterations}.{salt-base64}.{hash-base64}
        public static string HashPassword(string password, int iterations = 100_000)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string storedHash, string password)
        {
            if (storedHash == null) return false;
            if (password == null) throw new ArgumentNullException(nameof(password));

            var parts = storedHash.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out int iterations)) return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] hash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] computed = pbkdf2.GetBytes(hash.Length);

            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }

        public static bool IsHashed(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            var parts = value.Split('.');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out _)) return false;
            try
            {
                Convert.FromBase64String(parts[1]);
                Convert.FromBase64String(parts[2]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
