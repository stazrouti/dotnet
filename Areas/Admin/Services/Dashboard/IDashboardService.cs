using Bibliotheque.Areas.Admin.DTOs;

namespace Bibliotheque.Areas.Admin.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetStatsAsync();

        Task<byte[]> ExportToExcelAsync();
    }
}
