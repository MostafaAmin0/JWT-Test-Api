using Microsoft.AspNetCore.Mvc;

namespace JWT_Test_Api.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
