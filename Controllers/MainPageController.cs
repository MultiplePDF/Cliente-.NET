using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace client.Controllers
{
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View("~/Views/MainPage/Profile/Profile.cshtml");
        }
        public IActionResult MyFiles()
        {
            return View("~/Views/MainPage/MyFiles/MyFiles.cshtml");
        }
        public IActionResult Downloads()
        {
            return View("~/Views/MainPage/Downloads/Downloads.cshtml");
        }

    }
}