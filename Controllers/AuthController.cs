using client.Models;
using client.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServiceReference1;
using System.Reflection;

namespace client.Controllers
{
    public class AuthController : Controller, IAuthenticationService
    {

        public async Task<string> Login(string email, string password)
        {
            //...Logic
            string responseToken = "fiudh121io4u120912";
            return responseToken;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginService(UserModel model)
        {
            var email = model.Email;
            var password = model.Password;
            // Call Login
            var token = await Login(email, password);

            if (token != null) // If a valid token is obtained
            {
                // Set token in localStorage
                HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });
                // Redirect to MainPage with token
                return RedirectToAction("Index", "MainPage");
            }
            else // If an invalid token is obtained
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterService(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var res = await Register(model);
                var token = res.token;
                if (string.IsNullOrEmpty(token))
                {

                    ViewData["ErrorMessage"] = res.response;
                    // procesar la solicitud de registro
                    return View("Views/Home/Register/RegisterForm.cshtml", model);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });
                    return RedirectToAction("Index", "MainPage");

                }
            }
            else
            {
                // devolver una respuesta con errores de validación
                return View("Views/Home/Register/RegisterForm.cshtml", model);
            }
        }

        public async Task<registerResponse> Register(UserModel model)
        {
            MultiplepdfPortClient client = new();
            registerRequest req = new();
            req.name = model.Username;
            req.email = model.Email;
            req.password = model.Password;
            req.confirm_password = model.ConfirmPassword;
            registerResponse res = client.register(req);
            return res;
        }
    }
}
