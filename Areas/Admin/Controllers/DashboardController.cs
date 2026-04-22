using Bibliotheque.Areas.Admin.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetStatsAsync();
            return View(vm);
        }


        public async Task<IActionResult> Export()
        {
            var content = await _service.ExportToExcelAsync();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Reservations.xlsx");
        }
    }
}