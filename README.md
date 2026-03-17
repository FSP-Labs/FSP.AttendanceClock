# FSP Attendance Clock

Sistema de control de fichajes (check-in/check-out) para empleados. Desarrollado en ASP.NET Core 9 con PostgreSQL.

## Funcionalidades

- **Empleados**: fichar entrada/salida, ver historial, cambiar contraseña
- **Administradores**: gestión de usuarios, auditoría completa, exportación de informes de horas en Excel

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- PostgreSQL 14+

## Configuración

1. Copia el fichero de ejemplo y rellena tus datos de conexión:
   ```bash
   cp FSP.AttendanceClock.Web/appsettings.Example.json FSP.AttendanceClock.Web/appsettings.json
   ```

2. Edita `FSP.AttendanceClock.Web/appsettings.json` con tu cadena de conexión a PostgreSQL.

3. Aplica las migraciones de base de datos:
   ```bash
   dotnet ef database update --project FSP.AttendanceClock.Infrastructure --startup-project FSP.AttendanceClock.Web
   ```

4. Arranca la aplicación:
   ```bash
   dotnet run --project FSP.AttendanceClock.Web
   ```

## Logo personalizado

Reemplaza el fichero `FSP.AttendanceClock.Web/wwwroot/img/logo.png` con tu propio logo. El fichero actual es un placeholder blanco (200×50 px).

## Seguridad

Lee [`SECURITY_REVIEW_X_FORWARDED_FOR.md`](SECURITY_REVIEW_X_FORWARDED_FOR.md) antes de desplegar en producción detrás de un proxy inverso.

## Licencia

MIT
