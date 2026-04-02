using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Quan_ly_trung_tam_ngoai_ngu.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class DemoAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public DemoAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userRole = session.GetString(AppConstants.SessionDemoUserRole);

        if (string.IsNullOrWhiteSpace(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;
        }

        if (_roles.Length > 0 && !_roles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = userRole switch
            {
                AppConstants.Roles.Admin => new RedirectToActionResult("Index", "Dashboard", new { area = "Admin" }),
                AppConstants.Roles.Staff => new RedirectToActionResult("Index", "Dashboard", new { area = "Staff" }),
                AppConstants.Roles.Teacher => new RedirectToActionResult("Index", "Dashboard", new { area = "Teacher" }),
                _ => new RedirectToActionResult("Login", "Account", new { area = "" })
            };
        }
    }
}
