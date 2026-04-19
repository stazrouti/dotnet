using Bibliotheque.Models;
using Bibliotheque.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Controllers;

public class LivresController : Controller
{
    private readonly ILivreService _livreService;

    public LivresController(ILivreService livreService)
    {
        _livreService = livreService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var livres = await _livreService.GetAllAsync(search);
        var model = new LivreIndexViewModel
        {
            Search = search,
            Livres = livres
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

        return View(livre);
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
}