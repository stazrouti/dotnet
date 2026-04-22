using System.Diagnostics;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bibliotheque.Areas.Etudiant.Services;
using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Areas.Admin.Services.Etudiant;

namespace Bibliotheque.Areas.Etudiant.Controllers
{
    [Authorize(Roles = "Etudiant")]
    [Area("Etudiant")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEtudiantService _etudiantService;
        private readonly ILivreService _livreService;
        private const int MaxReservations = 3;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            IEtudiantService etudiantService,
            ILivreService livreService)
        {
            _logger = logger;
            _userManager = userManager;
            _etudiantService = etudiantService;
            _livreService = livreService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeDashboardViewModel();

            if (User.IsInRole("User"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

                if (etudiant is not null)
                {
                    var today = DateTime.UtcNow.Date;
                    var reservations = await _livreService.GetReservationsForStudentAsync(etudiant.Cin);

                    var currentReservations = reservations
                        .Where(r => !r.Estretour.HasValue)
                        .Select(r => new
                        {
                            StartDate = r.Dateemprunt?.Date ?? today,
                            EndDate = r.Dateretour?.Date ?? today
                        })
                        .ToList();

                    model.ShowEtudiantSummary = true;
                    model.TodayReservationsCount = currentReservations.Count(r => r.StartDate <= today && r.EndDate >= today);
                    model.DueTodayCount = currentReservations.Count(r => r.EndDate == today);
                    model.DueThisWeekCount = currentReservations.Count(r => r.EndDate > today && r.EndDate <= today.AddDays(7));
                    model.CurrentReservationCount = currentReservations.Count;
                    model.MaxReservationCount = MaxReservations;
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
