using Microsoft.AspNetCore.Mvc;
using Quan_ly_trung_tam_ngoai_ngu.Controllers;
using Quan_ly_trung_tam_ngoai_ngu.Infrastructure;
using Quan_ly_trung_tam_ngoai_ngu.Services.Interfaces;
using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;

namespace Quan_ly_trung_tam_ngoai_ngu.Areas.Teacher.Controllers;

[Area("Teacher")]
[DemoAuthorize(AppConstants.Roles.Teacher)]
public abstract class TeacherControllerBase : ModuleControllerBase
{
    protected TeacherControllerBase(IMockDataService dataService)
        : base(dataService)
    {
    }

    public static List<BreadcrumbItemViewModel> Breadcrumbs(string current, string? previousLabel = null, string? previousUrl = null)
    {
        var items = new List<BreadcrumbItemViewModel>
        {
            new() { Label = "Giáo viên", Url = "/Teacher", IsActive = false }
        };

        if (!string.IsNullOrWhiteSpace(previousLabel))
        {
            items.Add(new BreadcrumbItemViewModel { Label = previousLabel, Url = previousUrl, IsActive = false });
        }

        items.Add(new BreadcrumbItemViewModel { Label = current, IsActive = true });
        return items;
    }
}
