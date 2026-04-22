using Bibliotheque.Areas.Admin.DTOs;
using Bibliotheque.Areas.Admin.Services.Etudiant;
using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Areas.Etudiant.Services;
using Bibliotheque.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEtudiantService _etudiantService;
        private readonly ILivreService _livreService;

        private readonly BibliothequeContext _context;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IEtudiantService etudiantService,
                                 ILivreService livreService,
                                 BibliothequeContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _etudiantService = etudiantService;
            _livreService = livreService;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Dashboard", "Account", new { area = "" });
                }
            }

            ModelState.AddModelError(string.Empty, "Email ou mot de passe invalide.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                CIN = model.CIN,
                Filiere = model.Filière,
                Niveau = model.Niveau,
                Email = model.Email,
                NomComplet = model.NomComplet,
                Role = "Etudiant"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                

                        return View(model);
            }

            var isFirstUser = await _userManager.Users.CountAsync() == 1;
            var assignedRole = isFirstUser ? "Admin" : "Etudiant";

            if (!await _roleManager.RoleExistsAsync(assignedRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(assignedRole));
            }

            await _userManager.AddToRoleAsync(user, assignedRole);
            user.Role = assignedRole;
            await _userManager.UpdateAsync(user);

            if (user != null && !isFirstUser)
            {
                Bibliotheque.Models.Etudiant etud = await _context.Etudiants.Where(_ => _.Cin == user.CIN).FirstOrDefaultAsync();

                if (etud == null)
                {
                    _context.Etudiants.Add(new Models.Etudiant
                    {
                        Cin = user.CIN,
                        Nom = user.NomComplet,
                        Email = user.Email,
                        Filiere = user.Filiere,
                        Niveau = user.Niveau,
                        Matricule = user.CIN,
                        Qr = user.CIN,
                        Rfid = user.CIN
                    });
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Compte cree avec succes. Connectez-vous pour continuer.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new ProfileViewModel
            {
                Email = user.Email ?? string.Empty,
                NomComplet = user.NomComplet,
                Role = user.Role
            };

            return View(model);
        }

        [Authorize(Roles = "Etudiant")]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

            if (etudiant is null)
            {
                TempData["Error"] = "Tableau de bord indisponible. Votre profil etudiant doit etre valide par l'administration.";
                return RedirectToAction(nameof(Profile));
            }

            var today = DateTime.UtcNow.Date;
            var reservations = await _livreService.GetReservationsForStudentAsync(etudiant.Cin);
            var rows = reservations.Select(r =>
            {
                var status = GetReservationStatus(r, today);

                return new EtudiantReservationRowViewModel
                {
                    ReservationId = r.Id,
                    Numinventaire = r.Numinv,
                    Titre = r.NuminvNavigation?.Titre ?? string.Empty,
                    Auteur = r.NuminvNavigation?.Auteur ?? string.Empty,
                    StartDate = r.Dateemprunt?.Date ?? today,
                    EndDate = r.Dateretour?.Date ?? today,
                    Status = status,
                    CanCancel = status == ReservationStatus.Active || status == ReservationStatus.Reserved
                };
            }).ToList();

            var currentReservations = rows
                .Where(r => r.Status == ReservationStatus.Active || r.Status == ReservationStatus.Reserved)
                .OrderBy(r => r.StartDate)
                .ToList();

            var alerts = new List<EtudiantDashboardAlertViewModel>();

            alerts.AddRange(currentReservations
                .Where(r => r.EndDate.Date == today)
                .Select(r => new EtudiantDashboardAlertViewModel
                {
                    Level = EtudiantDashboardAlertLevel.Danger,
                    Message = "Retour prevu aujourd'hui.",
                    Numinventaire = r.Numinventaire,
                    Titre = r.Titre,
                    Date = r.EndDate.Date
                }));

            alerts.AddRange(currentReservations
                .Where(r => r.Status == ReservationStatus.Reserved && r.StartDate.Date == today.AddDays(1))
                .Select(r => new EtudiantDashboardAlertViewModel
                {
                    Level = EtudiantDashboardAlertLevel.Info,
                    Message = "Reservation qui commence demain.",
                    Numinventaire = r.Numinventaire,
                    Titre = r.Titre,
                    Date = r.StartDate.Date
                }));

            alerts.AddRange(currentReservations
                .Where(r => r.EndDate.Date > today && r.EndDate.Date <= today.AddDays(7))
                .Select(r => new EtudiantDashboardAlertViewModel
                {
                    Level = EtudiantDashboardAlertLevel.Warning,
                    Message = "Retour prevu cette semaine.",
                    Numinventaire = r.Numinventaire,
                    Titre = r.Titre,
                    Date = r.EndDate.Date
                }));

            alerts = alerts
                .OrderBy(a => a.Date)
                .ThenByDescending(a => a.Level)
                .ToList();

            var model = new EtudiantReservationDashboardViewModel
            {
                Etudiant = etudiant,
                CurrentReservations = currentReservations,
                HistoryReservations = rows
                    .Where(r => r.Status == ReservationStatus.Returned || r.Status == ReservationStatus.Cancelled)
                    .OrderByDescending(r => r.StartDate)
                    .ToList(),
                TodayReservationsCount = currentReservations.Count(r => r.StartDate.Date <= today && r.EndDate.Date >= today),
                DueTodayCount = currentReservations.Count(r => r.EndDate.Date == today),
                DueThisWeekCount = currentReservations.Count(r => r.EndDate.Date > today && r.EndDate.Date <= today.AddDays(7)),
                CurrentReservationCount = currentReservations.Count,
                Alerts = alerts,
                Stats = new EtudiantReservationStatsViewModel
                {
                    Total = rows.Count,
                    Active = rows.Count(r => r.Status == ReservationStatus.Active),
                    Upcoming = rows.Count(r => r.Status == ReservationStatus.Reserved),
                    Returned = rows.Count(r => r.Status == ReservationStatus.Returned),
                    Cancelled = rows.Count(r => r.Status == ReservationStatus.Cancelled)
                }
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(Login));
            }

            user.NomComplet = model.NomComplet;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profil mis a jour.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Mot de passe modifie avec succes.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SeedAdmin()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return RedirectToAction(nameof(Login));
            }

            user.Role = "Admin";
            await _userManager.UpdateAsync(user);

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            TempData["Success"] = "Votre compte est admin.";
            return RedirectToAction(nameof(Profile));
        }

        private static ReservationStatus GetReservationStatus(Emprunt reservation, DateTime today)
        {
            if (reservation.Estretour.HasValue)
            {
                if (reservation.Dateemprunt.HasValue && reservation.Estretour.Value.Date < reservation.Dateemprunt.Value.Date)
                {
                    return ReservationStatus.Cancelled;
                }

                return ReservationStatus.Returned;
            }

            var start = reservation.Dateemprunt?.Date ?? today;
            return start <= today ? ReservationStatus.Active : ReservationStatus.Reserved;
        }
    }
}
