using client.Models;
using client.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;

namespace client.Controllers
{
    public class AuthController : Controller, IAuthenticationService
    {

        public async Task<loginResponse> Login(LoginModel model)
        {
            MultiplepdfPortClient client = new();
            loginRequest req = new();
            req.email = model.Email;
            req.password = model.Password;
            loginResponse res = client.login(req);
            return res;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginService(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Call Login
                var res = await Login(model);
                var token = res.token;
                if (String.IsNullOrEmpty(token))
                {
                    ViewData["ErrorMessage"] = res.response;
                    // procesar la solicitud de registro
                    return View("Views/Home/Login/loginForm.cshtml", model);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });
                    return RedirectToAction("Index", "MainPage");
                }
            }
            else
            {
                return View("Views/Home/Login/loginForm.cshtml", model);
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
