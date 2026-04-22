using Microsoft.AspNetCore.Mvc;

namespace Bibliotheque.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
