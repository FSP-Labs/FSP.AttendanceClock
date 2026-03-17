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
    /// Manages user authentication (Login, Logout, Change password).
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
        /// Displays the login view.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Processes the login request with brute-force protection.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Check if the IP is blocked
            if (_loginAttemptService.IsBlocked(ipAddress))
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in 15 minutes.");
                return View(model);
            }

            // Look up user in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            
            // Verify password using PBKDF2 hash
            if (user == null || !PasswordHasher.Verify(user.PasswordHash, model.Password))
            {
                _loginAttemptService.RecordFailedAttempt(ipAddress);
                int remainingAttempts = 5 - _loginAttemptService.GetFailedAttempts(ipAddress);
                
                if (remainingAttempts > 0)
                {
                    ModelState.AddModelError(string.Empty, $"Invalid username or password. ({remainingAttempts} attempts remaining)");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again in 15 minutes.");
                }
                return View(model);
            }

            // Successful login — clear failed attempts
            _loginAttemptService.RecordSuccessfulAttempt(ipAddress);

            // Build the user identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in with cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Displays the change-password form.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Processes the password change for the authenticated user.
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
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            // Verify that the current password is correct
            if (!PasswordHasher.Verify(user.PasswordHash, model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "The current password is incorrect.");
                return View(model);
            }

            // Update password
            user.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password changed successfully. Please sign in again.";

            // Sign out after password change
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
