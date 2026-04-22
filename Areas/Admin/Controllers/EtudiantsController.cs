using Bibliotheque.Areas.Admin.DTOs;
using Bibliotheque.Areas.Admin.Services.Etudiant;
using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class EtudiantsController : Controller
{
    private readonly IEtudiantService _etudiantService;

    public EtudiantsController(IEtudiantService etudiantService)
    {
        _etudiantService = etudiantService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var etudiants = await _etudiantService.GetAllAsync(search);
        var model = new EtudiantIndexViewModel
        {
            Search = search,
            Etudiants = etudiants.ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        var activeLoans = await _etudiantService.GetActiveLoansAsync(id);
        var recentVisits = await _etudiantService.GetRecentVisitesAsync(id);

        var model = new EtudiantDetailsViewModel
        {
            Etudiant = etudiant,
            ActiveLoans = activeLoans.ToList(),
            RecentVisits = recentVisits.ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Models.Etudiant());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Models.Etudiant model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Cin))
        {
            ModelState.AddModelError(nameof(model.Cin), "Le numero d'identification est obligatoire.");
            return View(model);
        }

        if (await _etudiantService.ExistsAsync(model.Cin))
        {
            ModelState.AddModelError(nameof(model.Cin), "Cet etudiant existe deja.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Matricule) && await _etudiantService.MatriculeExistsAsync(model.Matricule))
        {
            ModelState.AddModelError(nameof(model.Matricule), "Ce numero de matricule existe deja.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Email) && await _etudiantService.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Cet email est deja utilise.");
            return View(model);
        }

        var created = await _etudiantService.CreateAsync(model);
        if (!created)
        {
            ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de la creation de l'etudiant.");
            return View(model);
        }

        TempData["Success"] = "Etudiant cree avec succes.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        return View(etudiant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Models.Etudiant model)
    {
        if (id != model.Cin)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _etudiantService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Etudiant mis a jour avec succes.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var etudiant = await _etudiantService.GetByIdAsync(id);
        if (etudiant is null)
        {
            return NotFound();
        }

        return View(etudiant);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var deleted = await _etudiantService.DeleteAsync(id);
        if (!deleted)
        {
            TempData["Error"] = "Impossible de supprimer cet etudiant. L'etudiant a des emprunts ou des visites associes.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Etudiant supprime avec succes.";
        return RedirectToAction(nameof(Index));
    }
}
