using Bibliotheque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                NomComplet = "MEHACH Ayoub",
                Role = "Admin"
            };

            await _userManager.CreateAsync(user, "Mehach@$1995");

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new User { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return RedirectToAction("Login");

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
