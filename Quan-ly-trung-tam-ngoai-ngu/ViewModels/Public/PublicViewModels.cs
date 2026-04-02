using System.ComponentModel.DataAnnotations;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;

namespace Quan_ly_trung_tam_ngoai_ngu.ViewModels.Public;

public class CourseCardViewModel
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string ScheduleSummary { get; set; } = string.Empty;
    public decimal TuitionFee { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string NextOpening { get; set; } = string.Empty;
}

public class ClassCardViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int SeatsLeft { get; set; }
}

public class NewsCardViewModel
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedOn { get; set; }
}

public class HomePageViewModel : AppPageViewModel
{
    public string HeroTitle { get; set; } = string.Empty;
    public string HeroSubtitle { get; set; } = string.Empty;
    public List<SummaryCardViewModel> HighlightStats { get; set; } = [];
    public List<CourseCardViewModel> FeaturedCourses { get; set; } = [];
    public List<ClassCardViewModel> OpenClasses { get; set; } = [];
    public List<NewsCardViewModel> LatestNews { get; set; } = [];
}

public class AboutPageViewModel : AppPageViewModel
{
    public List<SummaryCardViewModel> Achievements { get; set; } = [];
    public List<string> CoreValues { get; set; } = [];
    public List<string> Advantages { get; set; } = [];
}

public class CourseCatalogPageViewModel : AppPageViewModel
{
    public List<FilterGroupViewModel> Filters { get; set; } = [];
    public List<CourseCardViewModel> Courses { get; set; } = [];
}

public class CourseDetailPageViewModel : AppPageViewModel
{
    public CourseCardViewModel Course { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public string TargetOutput { get; set; } = string.Empty;
    public List<string> Objectives { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
    public List<ClassCardViewModel> RelatedClasses { get; set; } = [];
}

public class ClassCatalogPageViewModel : AppPageViewModel
{
    public List<FilterGroupViewModel> Filters { get; set; } = [];
    public List<ClassCardViewModel> Classes { get; set; } = [];
}

public class NewsListPageViewModel : AppPageViewModel
{
    public List<NewsCardViewModel> Articles { get; set; } = [];
}

public class NewsDetailPageViewModel : AppPageViewModel
{
    public NewsCardViewModel Article { get; set; } = new();
    public string Content { get; set; } = string.Empty;
    public List<NewsCardViewModel> RelatedArticles { get; set; } = [];
}

public class ContactViewModel : AppPageViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn nhu cầu.")]
    [Display(Name = "Nhu cầu")]
    public string Topic { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    [Display(Name = "Nội dung")]
    public string Message { get; set; } = string.Empty;
}

public class LoginViewModel : AppPageViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel : AppPageViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận chưa khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ForgotPasswordViewModel : AppPageViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}

public class ProfileViewModel : AppPageViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CurrentRole { get; set; } = string.Empty;
    public List<SummaryCardViewModel> SummaryCards { get; set; } = [];
    public List<DetailSectionViewModel> Sections { get; set; } = [];
    public List<TimelineItemViewModel> RecentActivities { get; set; } = [];
}
