using Microsoft.AspNetCore.Mvc;

namespace K_Gest.Controllers
{
    public class ReceitasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
