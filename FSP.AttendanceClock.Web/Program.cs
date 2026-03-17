using Microsoft.EntityFrameworkCore;
using FSP.AttendanceClock.Core.Interfaces;
using FSP.AttendanceClock.Infrastructure.Data;
using FSP.AttendanceClock.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using FSP.AttendanceClock.Web.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<AttendanceClockSettings>(
    builder.Configuration.GetSection("AttendanceClockSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAttendanceReportService, AttendanceReportService>();
builder.Services.AddSingleton<ILoginAttemptService>(sp => {
    var settings = sp.GetRequiredService<IOptions<AttendanceClockSettings>>().Value;
    return new LoginAttemptService(settings.MaxLoginAttempts, settings.LockoutDurationMinutes);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        // Cookie hardening
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<FSP.AttendanceClock.Web.Middleware.GlobalExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Respect proxy headers (X-Forwarded-For / X-Forwarded-Proto).
// Important: call before authentication/authorization so the remote IP is correct.
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// In development behind unknown proxies, clear known networks/proxies to accept forwarded headers.
// DO NOT do this blindly in untrusted production environments.
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

// Create Db if not exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var initialPassword = builder.Configuration["AdminSettings:InitialPassword"];
        DbInitializer.Initialize(context, initialPassword);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();
