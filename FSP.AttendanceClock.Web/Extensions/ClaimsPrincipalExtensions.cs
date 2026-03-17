using System.Security.Claims;

namespace FSP.AttendanceClock.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetCurrentUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim not found.");
        return int.Parse(value);
    }
}
