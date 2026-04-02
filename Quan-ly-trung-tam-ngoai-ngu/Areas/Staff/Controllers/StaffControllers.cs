using Microsoft.AspNetCore.Mvc;
using Quan_ly_trung_tam_ngoai_ngu.Infrastructure;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Dashboard;

namespace Quan_ly_trung_tam_ngoai_ngu.Areas.Staff.Controllers;

public class DashboardController : StaffControllerBase
{
    public DashboardController(IMockDataService dataService) : base(dataService) { }

    public IActionResult Index()
    {
        return DashboardView(new DashboardPageViewModel
        {
            Title = "Staff Dashboard",
            Subtitle = "Màn hình giáo vụ tập trung vào học viên, ghi danh, học phí và xếp lớp.",
            Breadcrumbs = Breadcrumbs("Dashboard"),
            RoleName = "Giáo vụ",
            SummaryCards =
            [
                new SummaryCardViewModel { Title = "Học viên mới", Value = "12", Description = "Mock trong 7 ngày gần nhất", Icon = "bi-person-plus", AccentClass = "primary" },
                new SummaryCardViewModel { Title = "Ghi danh mới", Value = DataService.GetEnrollments().Count.ToString(), Description = "Chờ xác nhận hoặc xếp lớp", Icon = "bi-journal-check", AccentClass = "info" },
                new SummaryCardViewModel { Title = "Khoản thu gần đây", Value = AppUi.Currency(DataService.GetReceipts().Sum(x => x.Amount)), Description = "Tổng biên nhận mock", Icon = "bi-cash-stack", AccentClass = "success" },
                new SummaryCardViewModel { Title = "Học viên còn nợ", Value = DataService.GetDebts().Count.ToString(), Description = "Danh sách cần theo dõi", Icon = "bi-wallet2", AccentClass = "danger" }
            ],
            Charts =
            [
                new ChartCardViewModel { ChartId = "staffDebtChart", Title = "Công nợ theo nhóm khóa học", Subtitle = "Mock tracking cho giáo vụ", ChartType = "bar", Labels = ["TOEIC", "IELTS Foundation", "Giao tiếp", "IELTS Intensive"], Values = [2.8m, 0, 2.6m, 7.8m], Colors = ["#1d4ed8", "#0ea5e9", "#f59e0b", "#ef4444"] }
            ],
            Panels =
            [
                new DashboardPanelViewModel
                {
                    Title = "Lớp sắp khai giảng",
                    Subtitle = "Cần chốt danh sách trước ngày mở lớp",
                    Items = DataService.GetClasses().Where(x => x.Status == "Sắp khai giảng" || x.Status == "Mở đăng ký").Select(x => new PanelItemViewModel
                    {
                        Title = x.Code,
                        Meta = $"{x.CourseName} • {x.Schedule}",
                        Value = $"{x.Enrolled}/{x.Capacity} HV",
                        BadgeText = x.Status,
                        BadgeClass = AppUi.StatusBadgeClass(x.Status)
                    }).ToList()
                }
            ],
            QuickActions =
            [
                new QuickActionViewModel { Label = "Ghi danh học viên", Url = "/Staff/Enrollments/Create", Icon = "bi-person-plus" },
                new QuickActionViewModel { Label = "Thu học phí", Url = "/Staff/Receipts/Create", Icon = "bi-receipt", CssClass = "btn btn-outline-primary" }
            ]
        });
    }
}

public class StudentsController : StaffControllerBase
{
    public StudentsController(IMockDataService dataService) : base(dataService) { }
    public IActionResult Index() => StaffPageHelpers.List(this, "Học viên", "/Staff/Students", DataService.GetStudents().Select(x => new TableRowViewModel { Id = x.Id.ToString(), Cells = [new() { Html = $"<strong>{x.FullName}</strong><div class='text-muted small'>{x.Code}</div>" }, new() { Html = x.ClassCode }, new() { Html = x.Phone }, new() { Html = AppUi.StatusBadge(x.Status) }, new() { Html = "" }], Actions = [new() { Label = "Chi tiết", Url = $"/Staff/Students/Details/{x.Id}", Icon = "bi-eye" }, new() { Label = "Sửa", Url = $"/Staff/Students/Edit/{x.Id}", Icon = "bi-pencil-square", CssClass = "btn btn-sm btn-outline-secondary" }] }).ToList());
    public IActionResult Create() => StaffPageHelpers.Form(this, "Thêm học viên", Breadcrumbs("Thêm học viên", "Học viên", "/Staff/Students"), "/Staff/Students", [new() { Label = "Họ và tên", Name = "FullName", Required = true }, new() { Label = "Số điện thoại", Name = "Phone", Required = true }, new() { Label = "Khóa học", Name = "CourseName", Required = true }, new() { Label = "Lớp học", Name = "ClassCode", Required = true }]);
    public IActionResult Edit(int id) => Create();
    public IActionResult Details(int id) => StaffPageHelpers.Details(this, "Chi tiết học viên", Breadcrumbs("Chi tiết học viên", "Học viên", "/Staff/Students"), "/Staff/Students");
}

public class EnrollmentsController : StaffControllerBase
{
    public EnrollmentsController(IMockDataService dataService) : base(dataService) { }
    public IActionResult Index() => StaffPageHelpers.List(this, "Ghi danh", "/Staff/Enrollments", DataService.GetEnrollments().Select(x => new TableRowViewModel { Id = x.Id.ToString(), Cells = [new() { Html = $"<strong>{x.StudentName}</strong><div class='text-muted small'>{x.EnrollmentCode}</div>" }, new() { Html = x.CourseName }, new() { Html = x.ClassCode }, new() { Html = AppUi.StatusBadge(x.Status) }, new() { Html = "" }], Actions = [new() { Label = "Chi tiết", Url = $"/Staff/Enrollments/Details/{x.Id}", Icon = "bi-eye" }, new() { Label = "Sửa", Url = $"/Staff/Enrollments/Edit/{x.Id}", Icon = "bi-pencil-square", CssClass = "btn btn-sm btn-outline-secondary" }] }).ToList());
    public IActionResult Create() => StaffPageHelpers.Form(this, "Ghi danh học viên", Breadcrumbs("Ghi danh học viên", "Ghi danh", "/Staff/Enrollments"), "/Staff/Enrollments", [new() { Label = "Học viên", Name = "StudentName", Required = true }, new() { Label = "Khóa học", Name = "CourseName", Required = true }, new() { Label = "Lớp học", Name = "ClassCode", Required = true }, new() { Label = "Đã đóng", Name = "PaidAmount", Type = "number", Required = true }]);
    public IActionResult Edit(int id) => Create();
    public IActionResult Details(int id) => StaffPageHelpers.Details(this, "Chi tiết ghi danh", Breadcrumbs("Chi tiết ghi danh", "Ghi danh", "/Staff/Enrollments"), "/Staff/Enrollments");
}

public class ClassesController : StaffControllerBase
{
    public ClassesController(IMockDataService dataService) : base(dataService) { }
    public IActionResult Index() => StaffPageHelpers.List(this, "Xếp lớp", "/Staff/Classes", DataService.GetClasses().Select(x => new TableRowViewModel { Id = x.Id.ToString(), Cells = [new() { Html = $"<strong>{x.Code}</strong><div class='text-muted small'>{x.CourseName}</div>" }, new() { Html = x.TeacherName }, new() { Html = $"{x.Enrolled}/{x.Capacity}" }, new() { Html = AppUi.StatusBadge(x.Status) }, new() { Html = "" }], Actions = [new() { Label = "Chi tiết", Url = $"/Staff/Classes/Details/{x.Id}", Icon = "bi-eye" }, new() { Label = "Sửa", Url = $"/Staff/Classes/Edit/{x.Id}", Icon = "bi-pencil-square", CssClass = "btn btn-sm btn-outline-secondary" }] }).ToList());
    public IActionResult Create() => StaffPageHelpers.Form(this, "Xếp lớp học viên", Breadcrumbs("Xếp lớp", "Xếp lớp", "/Staff/Classes"), "/Staff/Classes", [new() { Label = "Lớp học", Name = "ClassCode", Required = true }, new() { Label = "Học viên", Name = "StudentName", Required = true }, new() { Label = "Ngày bắt đầu", Name = "StartDate", Type = "date", Required = true }, new() { Label = "Ghi chú", Name = "Note", ColClass = "col-12" }]);
    public IActionResult Edit(int id) => Create();
    public IActionResult Details(int id) => StaffPageHelpers.Details(this, "Chi tiết xếp lớp", Breadcrumbs("Chi tiết xếp lớp", "Xếp lớp", "/Staff/Classes"), "/Staff/Classes");
}

public class ReceiptsController : StaffControllerBase
{
    public ReceiptsController(IMockDataService dataService) : base(dataService) { }
    public IActionResult Index() => StaffPageHelpers.List(this, "Thu học phí", "/Staff/Receipts", DataService.GetReceipts().Select(x => new TableRowViewModel { Id = x.Id.ToString(), Cells = [new() { Html = $"<strong>{x.StudentName}</strong><div class='text-muted small'>{x.ReceiptCode}</div>" }, new() { Html = x.ClassCode }, new() { Html = AppUi.Currency(x.Amount) }, new() { Html = AppUi.StatusBadge(x.Status) }, new() { Html = "" }], Actions = [new() { Label = "Chi tiết", Url = $"/Staff/Receipts/Details/{x.Id}", Icon = "bi-eye" }, new() { Label = "Sửa", Url = $"/Staff/Receipts/Edit/{x.Id}", Icon = "bi-pencil-square", CssClass = "btn btn-sm btn-outline-secondary" }, new() { Label = "In", Url = "#", Icon = "bi-printer", CssClass = "btn btn-sm btn-outline-dark", RequiresConfirm = true, ConfirmMessage = "Mock in biên nhận?" }] }).ToList());
    public IActionResult Create() => StaffPageHelpers.Form(this, "Thu học phí", Breadcrumbs("Thu học phí", "Thu học phí", "/Staff/Receipts"), "/Staff/Receipts", [new() { Label = "Học viên", Name = "StudentName", Required = true }, new() { Label = "Số tiền", Name = "Amount", Type = "number", Required = true }, new() { Label = "Phương thức", Name = "PaymentMethod", Required = true }, new() { Label = "Ghi chú", Name = "Note", ColClass = "col-12" }]);
    public IActionResult Edit(int id) => Create();
    public IActionResult Details(int id) => StaffPageHelpers.Details(this, "Chi tiết biên nhận", Breadcrumbs("Chi tiết biên nhận", "Thu học phí", "/Staff/Receipts"), "/Staff/Receipts");
}

internal static class StaffPageHelpers
{
    internal static IActionResult List(StaffControllerBase controller, string title, string baseUrl, List<TableRowViewModel> rows) => controller.ManagementListView(new ManagementListPageViewModel
    {
        Title = title,
        Subtitle = $"Module {title.ToLowerInvariant()} dành cho giáo vụ.",
        Breadcrumbs = StaffControllerBase.Breadcrumbs(title),
        PrimaryActionText = $"Thêm {title.ToLowerInvariant()}",
        PrimaryActionUrl = $"{baseUrl}/Create",
        Table = new TableViewModel { Columns = [new() { Header = title }, new() { Header = "Thông tin 1" }, new() { Header = "Thông tin 2" }, new() { Header = "Trạng thái" }, new() { Header = "Thao tác", Width = "220px" }], Rows = rows }
    });

    internal static IActionResult Form(StaffControllerBase controller, string title, List<BreadcrumbItemViewModel> breadcrumbs, string cancelUrl, List<FormFieldViewModel> fields) => controller.ManagementFormView(new ManagementFormPageViewModel
    {
        Title = title,
        Subtitle = "Form mock cho giáo vụ.",
        Breadcrumbs = breadcrumbs,
        FormTitle = title,
        FormDescription = "Dữ liệu chỉ phục vụ demo UI. // TODO: connect database later",
        CancelUrl = cancelUrl,
        Notice = "// TODO: connect database later",
        Sections = [new FormSectionViewModel { Title = "Thông tin", Fields = fields }]
    });

    internal static IActionResult Details(StaffControllerBase controller, string title, List<BreadcrumbItemViewModel> breadcrumbs, string backUrl) => controller.ManagementDetailsView(new ManagementDetailsPageViewModel
    {
        Title = title,
        Subtitle = "Trang chi tiết mock để minh họa luồng xử lý giáo vụ.",
        Breadcrumbs = breadcrumbs,
        Sections = [new DetailSectionViewModel { Title = "Ghi chú", Items = [new() { Label = "Tình trạng", Value = "Đang mock UI", IsBadge = true, BadgeClass = "bg-warning-subtle text-warning-emphasis" }, new() { Label = "TODO", Value = "Kết nối nghiệp vụ và DB sau" }] }],
        Actions = [new QuickActionViewModel { Label = "Quay lại", Url = backUrl, Icon = "bi-arrow-left", CssClass = "btn btn-outline-secondary" }]
    });
}
