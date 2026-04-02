using Microsoft.AspNetCore.Mvc;
using Quan_ly_trung_tam_ngoai_ngu.Infrastructure;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Public;

namespace Quan_ly_trung_tam_ngoai_ngu.Controllers;

[DemoAuthorize]
public class ProfileController : Controller
{
    private readonly IMockDataService _dataService;

    public ProfileController(IMockDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var email = HttpContext.Session.GetString(AppConstants.SessionDemoUserEmail);
        var account = _dataService.GetAccounts().FirstOrDefault(x => x.Email == email) ?? _dataService.GetAccounts().First();

        var model = new ProfileViewModel
        {
            Title = "Hồ sơ cá nhân",
            Subtitle = "Thông tin hồ sơ và hoạt động gần đây trong hệ thống demo.",
            Breadcrumbs = [new BreadcrumbItemViewModel { Label = "Hồ sơ cá nhân", IsActive = true }],
            FullName = account.FullName,
            Email = account.Email,
            Phone = account.Phone,
            CurrentRole = account.Role,
            SummaryCards =
            [
                new SummaryCardViewModel { Title = "Vai trò", Value = account.Role, Description = "Điều hướng dashboard theo quyền hiện tại", Icon = "bi-person-badge", AccentClass = "primary" },
                new SummaryCardViewModel { Title = "Trạng thái tài khoản", Value = account.Status, Description = "Mock trạng thái dùng cho phân quyền và hiển thị UI", Icon = "bi-shield-check", AccentClass = "success" },
                new SummaryCardViewModel { Title = "Phòng ban", Value = account.Department, Description = "Thông tin nội bộ để hiển thị hồ sơ", Icon = "bi-diagram-3", AccentClass = "info" }
            ],
            Sections =
            [
                new DetailSectionViewModel
                {
                    Title = "Thông tin cơ bản",
                    Items =
                    [
                        new DetailItemViewModel { Label = "Họ và tên", Value = account.FullName },
                        new DetailItemViewModel { Label = "Email", Value = account.Email },
                        new DetailItemViewModel { Label = "Số điện thoại", Value = account.Phone },
                        new DetailItemViewModel { Label = "Vai trò", Value = account.Role, IsBadge = true, BadgeClass = AppUi.StatusBadgeClass("Đang hoạt động") }
                    ]
                },
                new DetailSectionViewModel
                {
                    Title = "Ghi chú triển khai",
                    Description = "Các trường dưới đây phục vụ giai đoạn demo UI trước khi nối backend.",
                    Items =
                    [
                        new DetailItemViewModel { Label = "Nguồn dữ liệu", Value = "Session + MockDataService" },
                        new DetailItemViewModel { Label = "Tình trạng", Value = "Chưa kết nối database", IsBadge = true, BadgeClass = "bg-warning-subtle text-warning-emphasis" },
                        new DetailItemViewModel { Label = "TODO", Value = "Kết nối bảng Account và Student/Teacher sau" }
                    ]
                }
            ],
            RecentActivities =
            [
                new TimelineItemViewModel { Title = "Đăng nhập dashboard", Meta = "Hôm nay", Description = $"Phiên demo đã ghi nhận người dùng {account.Role.ToLowerInvariant()} truy cập hệ thống.", AccentClass = "primary" },
                new TimelineItemViewModel { Title = "Kiểm tra hồ sơ", Meta = "Hôm nay", Description = "Trang hồ sơ đang dùng dữ liệu giả để phục vụ trình bày UX/UI.", AccentClass = "info" },
                new TimelineItemViewModel { Title = "Sẵn sàng tích hợp DB", Meta = "TODO", Description = "Mapping tài khoản thật sẽ được bổ sung ở giai đoạn backend.", AccentClass = "warning" }
            ]
        };

        return View(model);
    }
}
