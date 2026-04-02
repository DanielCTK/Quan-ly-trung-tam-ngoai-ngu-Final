using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quan_ly_trung_tam_ngoai_ngu.Infrastructure;
using Quan_ly_trung_tam_ngoai_ngu.Models;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Public;

namespace Quan_ly_trung_tam_ngoai_ngu.Controllers;

public class HomeController : ModuleControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IMockDataService dataService)
        : base(dataService)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new HomePageViewModel
        {
            Title = "Trang chủ",
            Subtitle = "Hệ thống quản lý trung tâm ngoại ngữ NorthStar English",
            HeroTitle = "Nền tảng quản lý trung tâm ngoại ngữ hiện đại, trực quan và sẵn sàng mở rộng",
            HeroSubtitle = "Tập trung vào trải nghiệm demo, quy trình nghiệp vụ và giao diện quản trị rõ ràng cho trung tâm TOEIC, IELTS và giao tiếp.",
            HighlightStats =
            [
                new SummaryCardViewModel { Title = "Học viên đang theo học", Value = DataService.GetStudents().Count.ToString(), Description = "Theo dõi realtime theo lớp và trạng thái", Icon = "bi-people", AccentClass = "primary", Trend = "+12% tháng này" },
                new SummaryCardViewModel { Title = "Lớp đang mở", Value = DataService.GetClasses().Count(x => x.Status != "Đã đủ chỗ").ToString(), Description = "Có thể điều phối ngay từ dashboard", Icon = "bi-easel2", AccentClass = "info", Trend = "4 lớp sắp khai giảng" },
                new SummaryCardViewModel { Title = "Giáo viên phụ trách", Value = DataService.GetTeachers().Count.ToString(), Description = "Phân công và theo dõi lịch dạy", Icon = "bi-person-workspace", AccentClass = "success", Trend = "3 GV đang hoạt động" },
                new SummaryCardViewModel { Title = "Tổng học phí dự kiến", Value = AppUi.Currency(DataService.GetEnrollments().Sum(x => x.TotalFee)), Description = "Mock revenue cho kỳ tuyển sinh hiện tại", Icon = "bi-cash-stack", AccentClass = "warning", Trend = "Thu tốt ở nhóm IELTS" }
            ],
            FeaturedCourses = DataService.GetCourses().Take(3).Select(AppUi.ToCourseCard).ToList(),
            OpenClasses = DataService.GetClasses().Where(x => x.Status != "Đã đủ chỗ").Take(3).Select(AppUi.ToClassCard).ToList(),
            LatestNews = DataService.GetNewsArticles().OrderByDescending(x => x.PublishedOn).Take(3).Select(AppUi.ToNewsCard).ToList()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
