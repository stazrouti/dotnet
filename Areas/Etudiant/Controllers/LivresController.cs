using Bibliotheque.Areas.Admin.Services.Etudiant;
using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Areas.Etudiant.Services;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Areas.Etudiant.Controllers;

[Area("Etudiant")]
[Authorize]
public class LivresController : Controller
{
    private readonly ILivreService _livreService;
    private readonly IEtudiantService _etudiantService;
    private readonly UserManager<User> _userManager;
    private const int MaxReservations = 3;

    public LivresController(ILivreService livreService, IEtudiantService etudiantService, UserManager<User> userManager)
    {
        _livreService = livreService;
        _etudiantService = etudiantService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        const int pageSize = 10;
        if (page < 1)
        {
            page = 1;
        }

        var (livres, totalItems) = await _livreService.GetPagedAsync(search, page, pageSize);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));

        if (page > totalPages)
        {
            page = totalPages;
            (livres, totalItems) = await _livreService.GetPagedAsync(search, page, pageSize);
        }

        var model = new LivreIndexViewModel
        {
            Search = search,
            Livres = livres,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var livre = await _livreService.GetByIdAsync(id);
        if (livre is null)
        {
            return NotFound();
        }

        var today = DateTime.UtcNow.Date;
        var reservations = await _livreService.GetReservationsForBookAsync(livre.Numinventaire, today.AddDays(-30), today.AddDays(180));
        
        string? currentUserCin = null;
        if (User.IsInRole("Etudiant"))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);
            if (etudiant is not null)
            {
                currentUserCin = etudiant.Cin;
            }
        }

        var reviews = await _livreService.GetReviewsForBookAsync(livre.Numinventaire, currentUserCin);
        
        var detailsModel = new LivreDetailsViewModel
        {
            Livre = livre,
            Reservations = reservations.Select(r => new ReservationItemViewModel
            {
                Id = r.Id,
                Cin = r.Cin,
                StartDate = r.Dateemprunt?.Date ?? today,
                EndDate = r.Dateretour?.Date ?? today,
                Status = GetReservationStatus(r, today),
                CanCancel = r.CanCancel,
            }).ToList(),
            Reviews = reviews
        };

        if (User.IsInRole("Etudiant"))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

            if (etudiant is null)
            {
                detailsModel.CanReserve = false;
                detailsModel.ReservationBlockedReason = "La reservation est disponible seulement pour les comptes etudiants verifies avec un profil complet par l'administration.";
            }
            else
            {
                detailsModel.CurrentReservationCount = await _etudiantService.CountCurrentReservationsAsync(etudiant.Cin);
                detailsModel.CanReserve = detailsModel.CurrentReservationCount < MaxReservations;

                /*foreach (var reservation in detailsModel.Reservations)
                {
                    reservation.CanCancel = string.Equals(reservation.Cin, etudiant.Cin, StringComparison.OrdinalIgnoreCase)
                        && reservation.Status != ReservationStatus.Cancelled;
                }*/

                if (!detailsModel.CanReserve)
                {
                    detailsModel.ReservationBlockedReason = $"Vous avez atteint la limite de {MaxReservations} reservations actives.";
                }
            }
        }

        return View(detailsModel);
    }

    [Authorize(Roles = "Etudiant")]
    [HttpGet]
    public async Task<IActionResult> Reserve(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var livre = await _livreService.GetByIdAsync(id);
        if (livre is null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

        if (etudiant is null)
        {
            TempData["Error"] = "Reservation refusee. Votre compte doit etre lie a un profil etudiant verifie et complet.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var currentCount = await _etudiantService.CountCurrentReservationsAsync(etudiant.Cin);
        if (currentCount >= MaxReservations)
        {
            TempData["Error"] = $"Reservation refusee. Vous avez deja {MaxReservations} reservations actives.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var today = DateTime.UtcNow.Date;
        var minReservationDate = livre.Datearrivage?.ToDateTime(TimeOnly.MinValue).Date ?? today;
        if (minReservationDate < today)
        {
            minReservationDate = today;
        }

        var reservations = await _livreService.GetReservationsForBookAsync(id, today.AddDays(-30), today.AddDays(180));

        var model = new LivreReserveViewModel
        {
            Numinventaire = livre.Numinventaire,
            Titre = livre.Titre ?? string.Empty,
            BookDateArrivage = livre.Datearrivage,
            StartDate = minReservationDate,
            EndDate = minReservationDate,
            CurrentReservationCount = currentCount,
            MaxReservationCount = MaxReservations,
            Reservations = reservations.Select(r => new ReservationItemViewModel
            {
                Id = r.Id,
                Cin = r.Cin,
                StartDate = r.Dateemprunt?.Date ?? today,
                EndDate = r.Dateretour?.Date ?? today,
                Status = GetReservationStatus(r, today)
            }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Etudiant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(string id, LivreReserveViewModel model)
    {
        if (string.IsNullOrWhiteSpace(id) || !string.Equals(id, model.Numinventaire, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        var livre = await _livreService.GetByIdAsync(id);
        if (livre is null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);
        if (etudiant is null)
        {
            TempData["Error"] = "Reservation refusee. Votre profil etudiant doit etre valide par l'administration.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (model.StartDate.Date > model.EndDate.Date)
        {
            ModelState.AddModelError(string.Empty, "La date de fin doit etre superieure ou egale a la date de debut.");
        }

        var today = DateTime.UtcNow.Date;
        if (model.StartDate.Date < today)
        {
            ModelState.AddModelError(nameof(model.StartDate), "La date de debut ne peut pas etre dans le passe.");
        }

        var arrivalDate = livre.Datearrivage?.ToDateTime(TimeOnly.MinValue).Date;
        if (arrivalDate.HasValue && model.StartDate.Date < arrivalDate.Value)
        {
            ModelState.AddModelError(nameof(model.StartDate), "La date de debut ne peut pas etre anterieure a la date d'arrivage du livre.");
        }

        var currentCount = await _etudiantService.CountCurrentReservationsAsync(etudiant.Cin);
        if (currentCount >= MaxReservations)
        {
            ModelState.AddModelError(string.Empty, $"Vous avez atteint la limite de {MaxReservations} reservations actives.");
        }

        var isAvailable = await _livreService.IsBookAvailableAsync(id, model.StartDate, model.EndDate);
        if (!isAvailable)
        {
            ModelState.AddModelError(string.Empty, "Ce livre est deja reserve sur la periode selectionnee.");
        }

        if (!ModelState.IsValid)
        {
            model.Titre = livre.Titre ?? string.Empty;
            model.BookDateArrivage = livre.Datearrivage;
            model.CurrentReservationCount = currentCount;
            model.MaxReservationCount = MaxReservations;
            model.Reservations = (await _livreService.GetReservationsForBookAsync(id, today.AddDays(-30), today.AddDays(180)))
                .Select(r => new ReservationItemViewModel
                {
                    Id = r.Id,
                    Cin = r.Cin,
                    StartDate = r.Dateemprunt?.Date ?? today,
                    EndDate = r.Dateretour?.Date ?? today,
                    Status = GetReservationStatus(r, today)
                }).ToList();

            return View(model);
        }

        try
        {
            await _livreService.CreateReservationAsync(etudiant.Cin, id, model.StartDate, model.EndDate);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Titre = livre.Titre ?? string.Empty;
            model.BookDateArrivage = livre.Datearrivage;
            model.CurrentReservationCount = currentCount;
            model.MaxReservationCount = MaxReservations;
            model.Reservations = (await _livreService.GetReservationsForBookAsync(id, today.AddDays(-30), today.AddDays(180)))
                .Select(r => new ReservationItemViewModel
                {
                    Id = r.Id,
                    Cin = r.Cin,
                    StartDate = r.Dateemprunt?.Date ?? today,
                    EndDate = r.Dateretour?.Date ?? today,
                    Status = GetReservationStatus(r, today)
                }).ToList();

            return View(model);
        }

        TempData["Success"] = "Reservation creee avec succes.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Etudiant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(string id, decimal reservationId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);
        if (etudiant is null)
        {
            TempData["Error"] = "Annulation refusee. Votre profil etudiant doit etre valide par l'administration.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var reservation = await _livreService.GetReservationByIdAsync(reservationId);
        if (reservation is null || !string.Equals(reservation.Numinv, id, StringComparison.OrdinalIgnoreCase))
        {
            return NotFound();
        }

        var cancelled = await _livreService.CancelReservationAsync(reservationId, etudiant.Cin);
        if (!cancelled)
        {
            TempData["Error"] = "Annulation impossible. Vous ne pouvez pas annuler cette reservation.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Reservation annulee avec succes.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Livre());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Livre model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Numinventaire))
        {
            ModelState.AddModelError(nameof(model.Numinventaire), "Le numero d'inventaire est obligatoire.");
            return View(model);
        }

        if (await _livreService.ExistsAsync(model.Numinventaire))
        {
            ModelState.AddModelError(nameof(model.Numinventaire), "Ce numero d'inventaire existe deja.");
            return View(model);
        }

        await _livreService.CreateAsync(model);
        TempData["Success"] = "Livre cree avec succes.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var livre = await _livreService.GetByIdAsync(id);
        if (livre is null)
        {
            return NotFound();
        }

        return View(livre);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Livre model)
    {
        if (id != model.Numinventaire)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _livreService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Livre mis a jour avec succes.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var livre = await _livreService.GetByIdAsync(id);
        if (livre is null)
        {
            return NotFound();
        }

        return View(livre);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var deleted = await _livreService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Livre supprime avec succes.";
        return RedirectToAction(nameof(Index));
    }

    // Review/Rating actions
    [Authorize(Roles = "Etudiant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReview(string id, int note, string? commentaire)
    {
        if (string.IsNullOrWhiteSpace(id) || note < 1 || note > 5)
        {
            return BadRequest();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

        if (etudiant is null)
        {
            TempData["Error"] = "Vous devez avoir un profil etudiant verifie pour evaluer un livre.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            await _livreService.CreateReviewAsync(etudiant.Cin, id, note, commentaire);
            TempData["Success"] = "Evaluation creee avec succes.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Etudiant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReview(string id, decimal reviewId, int note, string? commentaire)
    {
        if (string.IsNullOrWhiteSpace(id) || note < 1 || note > 5)
        {
            return BadRequest();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

        if (etudiant is null)
        {
            TempData["Error"] = "Vous devez avoir un profil etudiant verifie.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            var updated = await _livreService.UpdateReviewAsync(reviewId, etudiant.Cin, note, commentaire);
            if (!updated)
            {
                TempData["Error"] = "Evaluation introuvable ou non autorisee.";
            }
            else
            {
                TempData["Success"] = "Evaluation mise a jour avec succes.";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Etudiant")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(string id, decimal reviewId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var etudiant = await _etudiantService.GetVerifiedEtudiantForUserAsync(currentUser?.Email);

        if (etudiant is null)
        {
            TempData["Error"] = "Vous devez avoir un profil etudiant verifie.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var deleted = await _livreService.DeleteReviewAsync(reviewId, etudiant.Cin);
        if (!deleted)
        {
            TempData["Error"] = "Evaluation introuvable ou non autorisee.";
        }
        else
        {
            TempData["Success"] = "Evaluation supprimee avec succes.";
        }

        return RedirectToAction(nameof(Details), new { id });
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