using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using client.Models;

namespace client.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    private IActionResult RedirectToMainIfTokenExists()
    {
        if (Request.Cookies["token"] != null)
        {
            // If it exists, redirect to MainPage
            return RedirectToAction("Index", "MainPage");
        }
        return null;
    }

    public IActionResult Index()
    {
        IActionResult result = RedirectToMainIfTokenExists();
        if (result != null) return result;
        return View("~/Views/Home/Login/loginForm.cshtml");
    }
    public IActionResult Register()
    {
        IActionResult result = RedirectToMainIfTokenExists();
        if (result != null) return result;
        return View("~/Views/Home/Register/RegisterForm.cshtml");
    }

    public IActionResult Inicio()
    {
        IActionResult result = RedirectToMainIfTokenExists();
        if (result != null) return result;
        return View("~/Views/Home/Main/Inicio.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
