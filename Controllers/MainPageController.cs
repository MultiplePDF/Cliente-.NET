using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServiceReference1;
using System.Text;

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

        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("token");
            return RedirectToAction("Index", "Home");
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
                String justFileName = Path.GetFileNameWithoutExtension(fileName);

                //Unique name
                string fileNameWithoutSpecialChars = Regex.Replace(justFileName, "[^0-9a-zA-Z._-]", "");
                string uniqueName = DateTime.Now.Ticks.ToString() + "_" + fileNameWithoutSpecialChars;

                //Size
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
                    fileName = uniqueName,
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

            if (res.successful)
            {
                return View("Views/MainPage/Downloads/Downloads.cshtml");
            }
            else
            {
                ViewData["ErrorMessage"] = res.response;
                return View("Views/MainPage/Index.cshtml");

            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadLinks(string links)
        {
            var idFile = 0;
            if (string.IsNullOrEmpty(links))
            {
                return View("Views/MainPage/Index.cshtml");
            }
            var results = new List<object>();
            var listLinks = links.Split(",");
            var validLinks = new List<string>();
            Regex regex = new Regex(@"^(http|https)://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,63}(/\S*)?$");

            foreach (var link in listLinks)
            {
                if (regex.IsMatch(link.Trim()))
                {
                    idFile++;
                    var fileInfo = new
                    {
                        idFile = idFile,
                        base64 = link,
                        fileName = "",
                        fileExtension = "URL",
                        size = 1,
                        checksum = "",
                    };

                    results.Add(fileInfo);
                }
            }

            var json = JsonConvert.SerializeObject(results);

            var token = HttpContext.Request.Cookies["token"];
            var res = ConvertFiles(json, token);


            return View("Views/MainPage/Downloads/Downloads.cshtml");
        }
        public sendBatchResponse ConvertFiles(String json, String token)
        {
            MultiplepdfPortClient client = new();
            sendBatchRequest req = new();
            req.listJSON = json;
            req.token = token;
            sendBatchResponse res = client.sendBatch(req);
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
        public async Task<IActionResult> MyFiles()
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            var list = await GetFilesDetails();
            var listBatch = new BatchListModel
            {
                objectList = list
            };

            //Save in Session
            var listBatchJson = JsonConvert.SerializeObject(listBatch);
            HttpContext.Session.Set("listBatch", Encoding.UTF8.GetBytes(listBatchJson)); // Almacenar lista en sesión
            return View("~/Views/MainPage/MyFiles/MyFiles.cshtml", listBatch);
        }

        public async Task<List<dynamic>> GetFilesDetails()
        {
            string token = Request.Cookies["token"]!;
            MultiplepdfPortClient client = new();
            getBatchDetailsRequest req = new();
            req.token = token;
            getBatchDetailsResponse res = client.getBatchDetails(req);
            List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(res.batchesList);
            return list;
        }
        public IActionResult Downloads()
        {
            return RedirectToMainIfTokenExists("~/Views/MainPage/Downloads/Downloads.cshtml");
        }
        public IActionResult Details(int index)
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            // Obtener el objeto serializado de la sesión
            var listBatchBytes = HttpContext.Session.Get("listBatch");
            if (listBatchBytes == null)
            {
                // El objeto no se encuentra en la sesión
                return RedirectToAction("MyFiles");
            }
            // Deserializar el objeto y obtener el objeto original
            var listBatchJson = Encoding.UTF8.GetString(listBatchBytes);
            var listBatch = JsonConvert.DeserializeObject<BatchListModel>(listBatchJson);

            List<dynamic> filesList = new List<dynamic>();
            foreach (dynamic obj in listBatch.objectList)
            {
                filesList.Add(obj.files);
            }
            var viewModel = new DetailsViewModel
            {
                FilesList = filesList,
                currentIndex = index,
            };
            return View("~/Views/MainPage/MyFiles/Details/Details.cshtml", viewModel);
        }

    }
}