using Microsoft.AspNetCore.Mvc;
using Quan_ly_trung_tam_ngoai_ngu.Infrastructure;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Public;

namespace Quan_ly_trung_tam_ngoai_ngu.Controllers;

public class AccountController : Controller
{
    private readonly IDemoAuthService _authService;

    public AccountController(IDemoAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(CreateLoginModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        model.Title = "Đăng nhập";
        model.Subtitle = "Đăng nhập demo theo vai trò để truy cập dashboard tương ứng.";
        model.Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Đăng nhập", IsActive = true }];

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var account = _authService.ValidateLogin(model.Email, model.Password);
        if (account is null)
        {
            model.ErrorMessage = "Tài khoản demo không hợp lệ. Hãy dùng một trong 3 tài khoản mẫu.";
            return View(model);
        }

        HttpContext.Session.SetString(AppConstants.SessionDemoUserEmail, account.Email);
        HttpContext.Session.SetString(AppConstants.SessionDemoUserRole, account.Role);
        HttpContext.Session.SetString(AppConstants.SessionDemoUserDisplayName, account.FullName);

        TempData[AppConstants.ToastMessageKey] = $"Đăng nhập thành công với vai trò {account.Role}.";
        TempData[AppConstants.ToastTypeKey] = "success";

        return account.Role switch
        {
            AppConstants.Roles.Admin => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
            AppConstants.Roles.Staff => RedirectToAction("Index", "Dashboard", new { area = "Staff" }),
            AppConstants.Roles.Teacher => RedirectToAction("Index", "Dashboard", new { area = "Teacher" }),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel
        {
            Title = "Đăng ký",
            Subtitle = "Form mock dành cho học viên mới. // TODO: connect database later",
            Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Đăng ký", IsActive = true }]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        model.Title = "Đăng ký";
        model.Subtitle = "Form mock dành cho học viên mới. // TODO: connect database later";
        model.Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Đăng ký", IsActive = true }];

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData[AppConstants.ToastMessageKey] = "Đăng ký thành công ở mức giao diện mock. Dữ liệu chưa được lưu thật.";
        TempData[AppConstants.ToastTypeKey] = "success";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel
        {
            Title = "Quên mật khẩu",
            Subtitle = "Mô phỏng quy trình lấy lại mật khẩu cho tài khoản học viên.",
            Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Quên mật khẩu", IsActive = true }]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        model.Title = "Quên mật khẩu";
        model.Subtitle = "Mô phỏng quy trình lấy lại mật khẩu cho tài khoản học viên.";
        model.Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Quên mật khẩu", IsActive = true }];

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TempData[AppConstants.ToastMessageKey] = "Đã gửi email khôi phục ở mức mock. // TODO: connect email service later";
        TempData[AppConstants.ToastTypeKey] = "success";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData[AppConstants.ToastMessageKey] = "Đã đăng xuất khỏi phiên demo.";
        TempData[AppConstants.ToastTypeKey] = "info";
        return RedirectToAction(nameof(Login));
    }

    private static LoginViewModel CreateLoginModel()
    {
        return new LoginViewModel
        {
            Title = "Đăng nhập",
            Subtitle = "Đăng nhập demo theo vai trò để truy cập dashboard tương ứng.",
            Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Đăng nhập", IsActive = true }]
        };
    }
}
