using Bibliotheque.Areas.Admin.Services.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 5;

            var vm = await _service.GetPagedAsync(search, page, pageSize);

            return View(vm);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Return(int id)
        {
            await _service.MarkAsReturnedAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Confirm(int id)
        {
            await _service.MarkAsConfirmedAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}