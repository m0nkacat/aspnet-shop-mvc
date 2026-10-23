using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}