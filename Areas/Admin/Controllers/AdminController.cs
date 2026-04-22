using Bibliotheque.Areas.Admin.Services.Admin;
using Bibliotheque.Areas.Etudiant.DTOs;
using Bibliotheque.Areas.Etudiant.Services;
using Bibliotheque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAdminService _adminService;

    public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,IAdminService adminService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var model = new AdminUsersViewModel
        {
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToList(),
            Users = new List<AdminUserRolesViewModel>()
        };

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Users.Add(new AdminUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                NomComplet = user.NomComplet,
                Roles = roles.ToList()
            });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
        {
            TempData["Error"] = "Utilisateur ou role invalide.";
            return RedirectToAction(nameof(Users));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["Error"] = "Utilisateur introuvable.";
            return RedirectToAction(nameof(Users));
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            TempData["Error"] = "Role introuvable.";
            return RedirectToAction(nameof(Users));
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        user.Role = role;
        await _userManager.UpdateAsync(user);

        await _adminService.AddEtudiant(user);

        TempData["Success"] = "Role assigne avec succes.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["Error"] = "Utilisateur introuvable.";
            return RedirectToAction(nameof(Users));
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.RemoveFromRoleAsync(user, role);
            await _adminService.UpdateEtudiant(user.Email);
        }

        var remainingRoles = await _userManager.GetRolesAsync(user);
        user.Role = remainingRoles.FirstOrDefault() ?? "User";
        await _userManager.UpdateAsync(user);

        TempData["Success"] = "Role retire avec succes.";
        return RedirectToAction(nameof(Users));
    }
}