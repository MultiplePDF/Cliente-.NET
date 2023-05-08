using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReference1;
using System.Text;
using client.Models;

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
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                // var res = ConvertFiles(json, token);
                ProcessFilesInBackground(json, token);
                return View("Views/MainPage/Downloads/Downloads.cshtml");
            }
            catch (System.Exception)
            {
                ViewData["ErrorMessage"] = "Error inesperado";
                return View("Views/MainPage/Index.cshtml");
                throw;
            }
        }

        public async Task<IActionResult> ProcessFilesInBackground(string json, string token)
        {

            Task<sendBatchResponse> conversionTask = Task.Run(() => ConvertFiles(json, token));

            return View("Views/MainPage/Downloads/Downloads.cshtml");
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
        public async Task<IActionResult> Profile(UserDetailsModel model)
        {
            var token = Request.Cookies["token"];
            var modelView = new ProfileViewModel();
            if (token == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var userDetails = await getUserDetails(token);
                model = new UserDetailsModel
                {
                    name = userDetails.name,
                    email = userDetails.email,
                };

                var modelUserDetails = JsonConvert.SerializeObject(model);
                HttpContext.Session.Set("modelUserDetails", Encoding.UTF8.GetBytes(modelUserDetails));

                modelView = new ProfileViewModel
                {
                    UserDetails = model,
                };
                return View("~/Views/MainPage/Profile/Profile.cshtml", modelView);
            }
            catch (System.Exception)
            {
                ViewData["ErrorMessage"] = "Ocurrió un error, intenta de nuevo más tarde";
                // procesar la solicitud de registro
                return View("~/Views/MainPage/Profile/Profile.cshtml", modelView);
            }

        }

        public async Task<getUserDetailsResponse> getUserDetails(string token)
        {
            MultiplepdfPortClient client = new();
            getUserDetailsRequest req = new();
            req.token = token;
            getUserDetailsResponse res = client.getUserDetails(req);
            return res;
        }

        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUserDetailsService(UserEditModel model)
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            var modelView = new ProfileViewModel();
            // Obtener el objeto serializado de la sesión
            var modelUserDetails = HttpContext.Session.Get("modelUserDetails");
            // Deserializar el objeto y obtener el objeto original
            var modelUserDetailsJson = Encoding.UTF8.GetString(modelUserDetails);
            var modelUserDetailsV = JsonConvert.DeserializeObject<UserDetailsModel>(modelUserDetailsJson);

            modelView = new ProfileViewModel
            {
                UserEdit = model,
                UserDetails = modelUserDetailsV,
            };
            if (ModelState.IsValid)
            {
                try
                {
                    var token = Request.Cookies["token"];
                    var res = await UpdateUserDetails(model, token);
                    if (res.successful)
                    {
                        return RedirectToMainIfTokenExists("~/Views/MainPage/Profile/Profile.cshtml");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = res.response;
                        // procesar la solicitud de registro
                        return View("Views/Home/Register/RegisterForm.cshtml", modelView);
                    }
                }
                catch (System.Exception)
                {
                    ViewData["ErrorMessage"] = "Ocurrió un error, intenta de nuevo más tarde";
                    // procesar la solicitud de registro
                    return View("~/Views/MainPage/Profile/Profile.cshtml", modelView);
                }
            }
            return View("~/Views/MainPage/Profile/Profile.cshtml", modelView);
        }

        public async Task<editUserDetailsResponse> UpdateUserDetails(UserEditModel model, string token)
        {
            MultiplepdfPortClient client = new();
            editUserDetailsRequest req = new();
            req.name = model.Username;
            req.email = model.Email;
            req.token = token;
            editUserDetailsResponse res = client.editUserDetails(req);
            return res;
        }
        public async Task<IActionResult> MyFiles()
        {
            if (Request.Cookies["token"] == null)
            {
                // If it exists, redirect to MainPage
                return RedirectToAction("Index", "Home");
            }
            var list = await GetFilesDetails();
            if (list.Count > 0)
            {
                var listBatch = new BatchListModel
                {
                    objectList = list
                };

                //Save in Session
                var listBatchJson = JsonConvert.SerializeObject(listBatch);
                HttpContext.Session.Set("listBatch", Encoding.UTF8.GetBytes(listBatchJson)); // Almacenar lista en sesión
                return View("~/Views/MainPage/MyFiles/MyFiles.cshtml", listBatch);
            }
            return View("~/Views/MainPage/MyFiles/MyFiles.cshtml");
        }

        public async Task<List<dynamic>> GetFilesDetails()
        {
            string token = Request.Cookies["token"]!;
            MultiplepdfPortClient client = new();
            getBatchDetailsRequest req = new();
            req.token = token;
            getBatchDetailsResponse res = client.getBatchDetails(req);
            try
            {
                List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(res.batchesList);
                return list;
            }
            catch (System.Exception)
            {
                return new List<dynamic>();
            }
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