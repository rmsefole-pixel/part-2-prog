using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDayAPI.Helpers;

/// <summary>Protects API actions using the authenticated user's server-side session and optional role.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string? _role;
    public SessionAuthorizeAttribute(string role = "") => _role = string.IsNullOrWhiteSpace(role) ? null : role;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");
        var userRole = session.GetString("Role");
        if (userId is null || userId <= 0 || string.IsNullOrWhiteSpace(userRole))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Authentication required. Please log in." });
            return;
        }
        if (_role is not null && !string.Equals(userRole, _role, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ObjectResult(new { message = $"Forbidden: this endpoint requires the {_role} role." }) { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }
        base.OnActionExecuting(context);
    }
}
