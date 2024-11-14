using Microsoft.AspNetCore.Mvc;

namespace Service_Academy1.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult ProfilePage()
        {
            ViewData["ActivePage"] = "Profile";
            return View();
        }
    }
}
