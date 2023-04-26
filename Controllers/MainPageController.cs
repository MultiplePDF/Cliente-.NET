using System.Security.Cryptography;
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
            var idFile = 0;
            foreach (var fileItem in file)
            {
                idFile++;
                var fileName = fileItem.FileName;
                FileInfo fi = new FileInfo(fileName);
                var fileNameExt = fi.Extension;
                string[] subs = fileName.Split('.');
                var justFileName = subs[0];

                var fileContent = string.Empty;
                var fileSizeInKb = (int)fileItem.Length / 1000;
                StreamReader sr = new StreamReader(fileItem.OpenReadStream());

                //SHA256
                String checksum = GetSha256HashFromIFormFile(fileItem);

                var memoryStream = new MemoryStream();
                fileItem.OpenReadStream().CopyTo(memoryStream);
                byte[] byteArray = memoryStream.ToArray();


                var base64String = Convert.ToBase64String(byteArray);
                var fileInfo = new
                {
                    idFile = idFile,
                    base64 = base64String,
                    fileName = justFileName,
                    fileExtension = fileNameExt,
                    size = fileSizeInKb,
                    checksum = checksum
                };

                results.Add(fileInfo);
            }
            var json = JsonConvert.SerializeObject(results);
            return Content(json, "application/json");
            //return RedirectToAction("Index");
        }

        public string GetSha256HashFromIFormFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(stream);
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "");
            return hashString;
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