using Microsoft.AspNetCore.Mvc;

namespace client.Controllers
{
    public class MainPageController : Controller
    {
        private IActionResult RedirectToMainIfTokenExists(string viewName)
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            return View(viewName);
        }

        public IActionResult Index()
        {
            return RedirectToMainIfTokenExists(null);
        }
        public IActionResult Profile()
        {
            return RedirectToMainIfTokenExists("~/Views/MainPage/Profile/Profile.cshtml");
        }
        public IActionResult MyFiles()
        {
            return RedirectToMainIfTokenExists("~/Views/MainPage/MyFiles/MyFiles.cshtml");
        }
        public IActionResult Downloads()
        {
            return RedirectToMainIfTokenExists("~/Views/MainPage/Downloads/Downloads.cshtml");
        }

    }
}