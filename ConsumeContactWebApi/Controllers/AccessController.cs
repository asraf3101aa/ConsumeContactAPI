using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ConsumeContactWebApi.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Todo_list.Controllers
{
    public class AccessController : Controller
    {
        string baseURL = "https://localhost:7022/";

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login modelLogin)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.GetAsync("Login/"+modelLogin.Email);
                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    JObject jsonObject = JObject.Parse(results);
                    
                    if (jsonObject["email"].ToString() == modelLogin.Email && jsonObject["password"].ToString() == modelLogin.Password)
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                            new Claim("OtherProperties","Example Role")
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = modelLogin.KeepLoggedIn
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), properties);
                        return RedirectToAction("Index", "Home");
                    }

                    ViewData["ValidateMessage"] = "Incorrect Password";
                    return View();
                }
                else
                {
                    ViewData["ValidateMessage"] = "User not found";

                }

            }
            return View();

        }
    }

}
