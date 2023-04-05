using client.Models;
using client.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReference1;
using System.Text;

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
    }
}
