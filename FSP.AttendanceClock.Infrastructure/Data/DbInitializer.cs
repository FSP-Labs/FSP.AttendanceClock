using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FSP.AttendanceClock.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context, string? initialAdminPassword = null)
        {
            // Apply EF Core migrations to ensure schema is up-to-date
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
            }

            // Ejecuta SQL de forma segura y centraliza el manejo de errores
            void ExecuteSqlSafe(string sql, string errorMessage)
            {
                try
                {
                    context.Database.ExecuteSqlRaw(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{errorMessage}: {ex.Message}");
                }
            }

            // Renombrar tablas al español si existen con nombres en inglés (operación idempotente)
            ExecuteSqlSafe(@"
                ALTER TABLE IF EXISTS ""Users"" RENAME TO ""Usuarios"";
                ALTER TABLE IF EXISTS ""Attendances"" RENAME TO ""Fichajes"";
                ALTER TABLE IF EXISTS ""AttendanceAudits"" RENAME TO ""AuditoriasFichajes"";
                ALTER TABLE IF EXISTS ""SystemLogs"" RENAME TO ""RegistrosSistema"";
            ", "Error renombrando tablas");

            // Actualizar valores de 'Action' en logs existentes a su equivalente en español (si existen)
            ExecuteSqlSafe(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'registrossistema') THEN
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'Entrada' WHERE ""Action"" = 'CheckIn';
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'Salida' WHERE ""Action"" = 'CheckOut';
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'EdicionFichaje' WHERE ""Action"" = 'EditAttendance' OR ""Action"" = 'Edit';
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'UsuarioCreado' WHERE ""Action"" = 'User Created' OR ""Action"" = 'UserCreated';
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'UsuarioEliminado' WHERE ""Action"" = 'User Deleted' OR ""Action"" = 'UserDeleted';
                        UPDATE ""RegistrosSistema"" SET ""Action"" = 'ContrasenaRestablecida' WHERE ""Action"" = 'Password Reset' OR ""Action"" = 'PasswordReset';
                    ELSIF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'systemlogs') THEN
                        UPDATE ""SystemLogs"" SET ""Action"" = 'Entrada' WHERE ""Action"" = 'CheckIn';
                        UPDATE ""SystemLogs"" SET ""Action"" = 'Salida' WHERE ""Action"" = 'CheckOut';
                        UPDATE ""SystemLogs"" SET ""Action"" = 'EdicionFichaje' WHERE ""Action"" = 'EditAttendance' OR ""Action"" = 'Edit';
                        UPDATE ""SystemLogs"" SET ""Action"" = 'UsuarioCreado' WHERE ""Action"" = 'User Created' OR ""Action"" = 'UserCreated';
                        UPDATE ""SystemLogs"" SET ""Action"" = 'UsuarioEliminado' WHERE ""Action"" = 'User Deleted' OR ""Action"" = 'UserDeleted';
                        UPDATE ""SystemLogs"" SET ""Action"" = 'ContrasenaRestablecida' WHERE ""Action"" = 'Password Reset' OR ""Action"" = 'PasswordReset';
                    END IF;
                END;
                $$;
            ", "Error actualizando acciones en logs");

            // Ensure immutability for audit tables by creating triggers that prevent UPDATE/DELETE
            ExecuteSqlSafe(@"
                CREATE OR REPLACE FUNCTION prevent_modification() RETURNS trigger AS $$
                BEGIN
                    RAISE EXCEPTION 'Modification of audit records is not allowed';
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;

                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'prevent_syslogs_mod') THEN
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'registrossistema') THEN
                            CREATE TRIGGER prevent_syslogs_mod
                            BEFORE UPDATE OR DELETE ON ""RegistrosSistema""
                            FOR EACH ROW EXECUTE FUNCTION prevent_modification();
                        ELSIF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'systemlogs') THEN
                            CREATE TRIGGER prevent_syslogs_mod
                            BEFORE UPDATE OR DELETE ON ""SystemLogs""
                            FOR EACH ROW EXECUTE FUNCTION prevent_modification();
                        END IF;
                    END IF;

                    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'prevent_attendanceaudits_mod') THEN
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'auditoriasfichajes') THEN
                            CREATE TRIGGER prevent_attendanceaudits_mod
                            BEFORE UPDATE OR DELETE ON ""AuditoriasFichajes""
                            FOR EACH ROW EXECUTE FUNCTION prevent_modification();
                        ELSIF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'attendanceaudits') THEN
                            CREATE TRIGGER prevent_attendanceaudits_mod
                            BEFORE UPDATE OR DELETE ON ""AttendanceAudits""
                            FOR EACH ROW EXECUTE FUNCTION prevent_modification();
                        END IF;
                    END IF;
                END;
                $$;
            ", "Error creating immutability triggers");

            // Ensure any existing plaintext passwords are hashed, or seed default users.
            var existingUsers = context.Users.ToList();

            if (existingUsers.Count == 0)
            {
                string adminPassword;
                if (!string.IsNullOrEmpty(initialAdminPassword))
                {
                    adminPassword = initialAdminPassword;
                }
                else
                {
                    // Generate a cryptographically random 16-character password
                    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
                    var randomBytes = new byte[16];
                    RandomNumberGenerator.Fill(randomBytes);
                    var passwordChars = new char[16];
                    for (int i = 0; i < 16; i++)
                    {
                        passwordChars[i] = chars[randomBytes[i] % chars.Length];
                    }
                    adminPassword = new string(passwordChars);

                    Console.WriteLine("⚠️  INITIAL ADMIN PASSWORD (only shown once): " + adminPassword);
                    Console.WriteLine("⚠️  Please log in and change it immediately.");
                }

                var users = new User[]
                {
                    new User { Username = "admin", PasswordHash = PasswordHasher.HashPassword(adminPassword), Role = UserRole.Administrador },
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
            else
            {
                var updated = false;
                foreach (var u in existingUsers)
                {
                    if (!PasswordHasher.IsHashed(u.PasswordHash))
                    {
                        // Assume stored value is plaintext password; re-hash it.
                        u.PasswordHash = PasswordHasher.HashPassword(u.PasswordHash ?? string.Empty);
                        updated = true;
                    }
                }

                if (updated)
                {
                    context.SaveChanges();
                }
            }
        }
    }
}
