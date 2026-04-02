using System.Globalization;
using Quan_ly_trung_tam_ngoai_ngu.Models;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Public;

namespace Quan_ly_trung_tam_ngoai_ngu.Infrastructure;

public static class AppUi
{
    private static readonly CultureInfo ViCulture = new("vi-VN");

    public static string Currency(decimal amount)
    {
        return string.Format(ViCulture, "{0:N0} đ", amount);
    }

    public static string StatusBadge(string value)
    {
        return $"<span class=\"badge {StatusBadgeClass(value)}\">{value}</span>";
    }

    public static string StatusBadgeClass(string value)
    {
        return value switch
        {
            "Đang hoạt động" or "Đang học" or "Đã thanh toán" or "Đạt" or "Có mặt" or "Hoàn tất" or "Đã ghi nhận" or "Đã xếp lớp" or "Đang giảng dạy"
                => "bg-success-subtle text-success-emphasis",
            "Sắp khai giảng" or "Sắp mở lớp" or "Sắp đến hạn" or "Đóng một phần" or "Đóng cọc" or "Muộn" or "Khai giảng sớm"
                => "bg-warning-subtle text-warning-emphasis",
            "Quá hạn" or "Còn nợ" or "Chờ xác nhận" or "Bảo lưu" or "Cần cải thiện"
                => "bg-danger-subtle text-danger-emphasis",
            "Đang tuyển sinh" or "Mở đăng ký" or "Hôm nay" or "Sắp diễn ra"
                => "bg-info-subtle text-info-emphasis",
            _ => "bg-secondary-subtle text-secondary-emphasis"
        };
    }

    public static CourseCardViewModel ToCourseCard(Course course)
    {
        return new CourseCardViewModel
        {
            Id = course.Id,
            Slug = course.Slug,
            Name = course.Name,
            Level = course.Level,
            Duration = course.Duration,
            ScheduleSummary = course.ScheduleSummary,
            TuitionFee = course.TuitionFee,
            Status = course.Status,
            ShortDescription = course.ShortDescription,
            NextOpening = course.NextOpening
        };
    }

    public static ClassCardViewModel ToClassCard(CourseClass item)
    {
        return new ClassCardViewModel
        {
            Id = item.Id,
            Code = item.Code,
            CourseName = item.CourseName,
            TeacherName = item.TeacherName,
            Schedule = item.Schedule,
            Room = item.Room,
            Status = item.Status,
            StartDate = item.StartDate,
            SeatsLeft = Math.Max(0, item.Capacity - item.Enrolled)
        };
    }

    public static NewsCardViewModel ToNewsCard(NewsArticle article)
    {
        return new NewsCardViewModel
        {
            Id = article.Id,
            Slug = article.Slug,
            Title = article.Title,
            Category = article.Category,
            Summary = article.Summary,
            Author = article.Author,
            PublishedOn = article.PublishedOn
        };
    }

    public static List<BreadcrumbItemViewModel> Breadcrumbs(params (string Label, string? Url, bool IsActive)[] items)
    {
        return items.Select(x => new BreadcrumbItemViewModel
        {
            Label = x.Label,
            Url = x.Url,
            IsActive = x.IsActive
        }).ToList();
    }
}
