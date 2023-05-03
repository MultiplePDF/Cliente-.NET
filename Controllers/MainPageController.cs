using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReference1;


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
                    fileName = fileName,
                    fileExtension = fileNameExt,
                    size = fileSizeInKb,
                    checksum = checksum
                };

                results.Add(fileInfo);
            }
            var json = JsonConvert.SerializeObject(results);

            //Send to batch 
            var token = HttpContext.Request.Cookies["token"];
            var res = ConvertFiles(json, token);
            var download = res.downloadPath;

            Console.WriteLine(token);
            Console.WriteLine(json);
            if (res.successful)
            {
                return Content(download, "application/json");
            }
            else
            {
                ViewData["ErrorMessage"] = res.response;
                return View("Views/MainPage/Index.cshtml");

            }

            //return RedirectToAction("Index");
        }

        public sendBatchResponse ConvertFiles(String json, String token)
        {
            MultiplepdfPortClient client = new();
            sendBatchRequest req = new();
            req.listJSON = json;
            req.token = token;
            sendBatchResponse res = client.sendBatch(req);
            Console.WriteLine(res.response);
            Console.WriteLine("link " + res.downloadPath);
            return res;
        }

        public string GetSha256HashFromIFormFile(IFormFile file)
        {
            byte[] hashBytes;

            using (var sha256 = SHA256.Create())
            {
                using (var stream = file.OpenReadStream())
                {
                    hashBytes = sha256.ComputeHash(stream);
                }
            }
            string hashValue = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            return hashValue;
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