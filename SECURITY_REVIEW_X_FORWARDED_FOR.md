# Security Review: X-Forwarded-For / Forwarded Headers

## Current State Analysis

### 1. Program.cs Configuration (Lines 48-56)
```csharp
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);
```

**ISSUE**: Clearing `KnownNetworks` and `KnownProxies` accepts forwarded headers from **ANY** source, including untrusted clients. This is a **header spoofing vulnerability** in production.

---

## Vulnerability Details

### Attack Scenario
1. **Attacker** sends HTTP request with `X-Forwarded-For: <victim-IP>`
2. **RemoteIpAddress** is set to victim's IP (even from untrusted client)
3. **Result**: 
   - Brute-force attacks appear from victim IP (login blocking at `AccountController:47`)
   - Audit logs record victim's IP instead of attacker
   - IP-based rate limiting is bypassed

### Current IP Reading Pattern (4 occurrences)
Found in **AccountController, AdminController (3x), AttendanceController (3x)**:
```csharp
var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() 
         ?? HttpContext.Connection.RemoteIpAddress?.ToString();
```

**ISSUE**: This pattern reads raw header without validating the source. Combined with cleared `KnownNetworks`, it trusts any client.

---

## Recommended Safe Configuration

### For Production (with trusted reverse proxy)
```csharp
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    // REQUIRED: Only trust specific proxy IPs
    KnownProxies = { IPAddress.Parse("10.0.0.1") }, // e.g., load balancer IP
};
// DO NOT clear KnownNetworks - leave defaults
app.UseForwardedHeaders(forwardedOptions);
```

### For Development (localhost only)
```csharp
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

if (app.Environment.IsDevelopment())
{
    // Only in dev: accept from local proxies
    forwardedOptions.KnownProxies.Add(IPAddress.Loopback);
    forwardedOptions.KnownProxies.Add(IPAddress.IPv6Loopback);
}
else
{
    // Production: explicitly set trusted proxies
    forwardedOptions.KnownProxies.Add(IPAddress.Parse("YOUR_LOAD_BALANCER_IP"));
    // Remove: forwardedOptions.KnownNetworks.Clear();
}

app.UseForwardedHeaders(forwardedOptions);
```

---

## Recommended Controller Pattern

### Create a Helper Extension (new file: `HttpContextExtensions.cs`)
```csharp
using System.Net;
using Microsoft.AspNetCore.Http;

namespace FSP.AttendanceClock.Web.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Safely retrieves the client IP address.
        /// Uses HttpContext.Connection.RemoteIpAddress (already processed by UseForwardedHeaders)
        /// instead of reading raw X-Forwarded-For header.
        /// </summary>
        public static string GetClientIpAddress(this HttpContext context)
        {
            // HttpContext.Connection.RemoteIpAddress contains the processed IP
            // after UseForwardedHeaders has validated and applied forwarded headers
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
```

### Update Controllers (6 locations total)

**Before:**
```csharp
var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() 
         ?? HttpContext.Connection.RemoteIpAddress?.ToString();
```

**After:**
```csharp
var ip = HttpContext.GetClientIpAddress();
```

**Locations to update:**
1. `AccountController.cs:47` - Login IP tracking
2. `AdminController.cs:98` - User creation audit
3. `AdminController.cs:120` - User deletion audit
4. `AdminController.cs:158` - Password reset audit
5. `AttendanceController.cs:105` - Check-in audit
6. `AttendanceController.cs:144` - Check-out audit
7. `AttendanceController.cs:220` - Edit attendance audit

---

## Risk Assessment

| Risk | Current State | After Fix |
|------|---------------|-----------|
| **Header Spoofing** | 🔴 HIGH | 🟢 MITIGATED |
| **IP Spoofing in Logs** | 🔴 HIGH | 🟢 MITIGATED |
| **Rate Limiting Bypass** | 🔴 HIGH | 🟢 MITIGATED |
| **Code Clarity** | 🟡 MEDIUM | 🟢 IMPROVED |

---

## Implementation Checklist

### Step 1: Update Program.cs
- [ ] Choose deployment model (Dev vs Production)
- [ ] Replace `KnownNetworks.Clear()` and `KnownProxies.Clear()` with explicit trusted proxies
- [ ] Add conditional logic for development/production

### Step 2: Create Helper Extension
- [ ] Create `FSP.AttendanceClock.Web/Extensions/HttpContextExtensions.cs`
- [ ] Add `GetClientIpAddress()` method

### Step 3: Update Controllers (7 locations)
- [ ] **AccountController.cs:47** - Login method
- [ ] **AdminController.cs:98** - CreateUser method
- [ ] **AdminController.cs:120** - DeleteUser method
- [ ] **AdminController.cs:158** - ResetPassword method
- [ ] **AttendanceController.cs:105** - CheckIn method
- [ ] **AttendanceController.cs:144** - CheckOut method
- [ ] **AttendanceController.cs:220** - Edit method

### Step 4: Configuration
- [ ] Update appsettings.json with proxy IP (if needed)
- [ ] Test with direct request (no proxy)
- [ ] Test with proxy (set X-Forwarded-For header)

### Step 5: Validation
- [ ] Verify audit logs show correct IP
- [ ] Test rate limiting with spoofed headers (should fail)
- [ ] Run application in both Dev and Production mode

---

## Environment-Specific Notes

### Development (localhost)
- Can accept from `::1` (IPv6 localhost) or `127.0.0.1`
- Useful for testing Docker containers or local reverse proxies

### Production (with AWS ALB, Nginx, IIS)
- **AWS ALB**: Trust `10.0.0.0/8` (VPC range) or specific ALB IP
- **Nginx/Apache**: Trust `127.0.0.1` and loopback only (if on same host)
- **Azure App Service**: Trust `127.0.0.1` (Azure injects X-Forwarded-For)

### When NOT to clear KnownNetworks
- The default includes RFC 1918 private ranges (`10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`)
- Clearing these allows **any client** to spoof an IP from these ranges
- Only clear if you have **explicit reason** (rare)

---

## Additional Security Notes

1. **Middleware Order**: `UseForwardedHeaders()` must be called BEFORE `UseAuthentication()`
   - ✅ Current code is correct (line 56, before auth at line 58)

2. **Audit Logging**: Already logging IP correctly to database
   - Ensure `IpAddress` column is indexed for audit analysis

3. **Rate Limiting**: `LoginAttemptService` uses IP correctly
   - Will be more secure after header validation

4. **Configuration Management**:
   - Consider externalizing proxy IP to environment variables
   - Use `IConfiguration` instead of hardcoding IPs

---

## References
- [Microsoft Docs: X-Forwarded-For Header Spoofing](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-9.0)
- [OWASP: X-Forwarded-For](https://owasp.org/www-project-web-security-testing-guide/)
- [ForwardedHeadersOptions Documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.httpoverrides.forwardedheadersoptions)
