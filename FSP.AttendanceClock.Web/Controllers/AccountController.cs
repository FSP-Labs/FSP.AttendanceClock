using System.Security.Claims;
using FSP.AttendanceClock.Core.Entities;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Infrastructure.Data;
using FSP.AttendanceClock.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSP.AttendanceClock.Infrastructure.Security;
using FSP.AttendanceClock.Web.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FSP.AttendanceClock.Web.Controllers
{
    /// <summary>
    /// Gestiona la autenticación de usuarios (Login, Logout, Cambio de contraseña).
    /// </summary>
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoginAttemptService _loginAttemptService;

        public AccountController(AppDbContext context, ILoginAttemptService loginAttemptService)
        {
            _context = context;
            _loginAttemptService = loginAttemptService;
        }

        /// <summary>
        /// Muestra la vista de inicio de sesión.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Procesa la solicitud de inicio de sesión con protección contra fuerza bruta.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Verificar si la IP está bloqueada
            if (_loginAttemptService.IsBlocked(ipAddress))
            {
                ModelState.AddModelError(string.Empty, "Demasiados intentos fallidos. Intenta de nuevo en 15 minutos.");
                return View(model);
            }

            // Buscar usuario en base de datos
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            
            // Verificar contraseña usando PBKDF2 hash
            if (user == null || !PasswordHasher.Verify(user.PasswordHash, model.Password))
            {
                _loginAttemptService.RecordFailedAttempt(ipAddress);
                int remainingAttempts = 5 - _loginAttemptService.GetFailedAttempts(ipAddress);
                
                if (remainingAttempts > 0)
                {
                    ModelState.AddModelError(string.Empty, $"Usuario o contraseña inválidos. ({remainingAttempts} intentos restantes)");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Demasiados intentos fallidos. Intenta de nuevo en 15 minutos.");
                }
                return View(model);
            }

            // Login exitoso - limpiar intentos fallidos
            _loginAttemptService.RecordSuccessfulAttempt(ipAddress);

            // Crear identidad del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Iniciar sesión con cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Muestra el formulario para cambiar la contraseña.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Procesa el cambio de contraseña del usuario autenticado.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                return View(model);
            }

            // Verificar que la contraseña actual sea correcta
            if (!PasswordHasher.Verify(user.PasswordHash, model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "La contraseña actual es incorrecta.");
                return View(model);
            }

            // Actualizar contraseña
            user.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Contraseña cambiada exitosamente. Por favor inicia sesión de nuevo.";

            // Cerrar sesión después de cambiar contraseña
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
