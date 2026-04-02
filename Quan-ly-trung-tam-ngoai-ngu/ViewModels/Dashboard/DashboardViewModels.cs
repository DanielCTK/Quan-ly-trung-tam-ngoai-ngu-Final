using Quan_ly_trung_tam_ngoai_ngu.ViewModels.Common;

namespace Quan_ly_trung_tam_ngoai_ngu.ViewModels.Dashboard;

public class ChartCardViewModel
{
    public string ChartId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ChartType { get; set; } = "bar";
    public List<string> Labels { get; set; } = [];
    public List<decimal> Values { get; set; } = [];
    public List<string> Colors { get; set; } = [];
}

public class PanelItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? BadgeText { get; set; }
    public string? BadgeClass { get; set; }
}

public class DashboardPanelViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string EmptyText { get; set; } = "Chưa có dữ liệu";
    public string? ActionLabel { get; set; }
    public string? ActionUrl { get; set; }
    public List<PanelItemViewModel> Items { get; set; } = [];
}

public class DashboardPageViewModel : AppPageViewModel
{
    public string RoleName { get; set; } = string.Empty;
    public List<SummaryCardViewModel> SummaryCards { get; set; } = [];
    public List<QuickActionViewModel> QuickActions { get; set; } = [];
    public List<ChartCardViewModel> Charts { get; set; } = [];
    public List<DashboardPanelViewModel> Panels { get; set; } = [];
    public List<TimelineItemViewModel> Timeline { get; set; } = [];
}
