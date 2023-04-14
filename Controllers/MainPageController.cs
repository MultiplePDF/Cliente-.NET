using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace client.Controllers
{
    public class MainPageController : Controller
    {
        private readonly ILogger<MainPageController> _logger;
        private IWebHostEnvironment hostingEnvironment;

        public MainPageController(ILogger<MainPageController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
        }

        private IActionResult RedirectToMainIfTokenExists(string viewName)
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            return View(viewName);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> file)
        {
            var results = new List<object>();
            foreach (var fileItem in file)
            {
                var fileName = fileItem.FileName;
                var fileContent = string.Empty;
                using (var streamReader = new StreamReader(fileItem.OpenReadStream()))
                {
                    fileContent = await streamReader.ReadToEndAsync();
                }
                var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContent));
                var fileInfo = new
                {
                    fileName,
                    base64 = base64String
                };

                results.Add(fileInfo);


            }
            var json = JsonConvert.SerializeObject(results);
            return Content(json, "application/json");
            //return RedirectToAction("Index");
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