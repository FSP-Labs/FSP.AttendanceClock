using System;

namespace FSP.AttendanceClock.Core.Entities
{
    public enum UserRole
    {
        Empleado,
        Administrador
    }

    public class User
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Nombre único de inicio de sesión.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Contraseña del usuario hasheada con PBKDF2.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// Rol del usuario en el sistema (Administrador o Empleado).
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Empleado;
        
        /// <summary>
        /// Registros de fichaje asociados al usuario.
        /// </summary>
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
