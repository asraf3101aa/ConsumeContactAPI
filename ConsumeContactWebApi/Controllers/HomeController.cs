using ConsumeContactWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace ConsumeContactWebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string baseURL = "https://localhost:7044/";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Calling the web API and populating the data in view using DataTable
            DataTable dataTable = new DataTable();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.GetAsync("Contacts");
                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    dataTable = JsonConvert.DeserializeObject<DataTable>(results);
                }
                else
                {
                    Console.WriteLine("Error calling with API");

                }
                ViewData.Model = dataTable;
            }
            return View();
        }
        public async Task<IActionResult> Index2()
        {
            //Calling the web API and populating the data in view using Entity Model Class
            //DataTable dataTable = new DataTable();
            IList<UserEntity> users = new List<UserEntity>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.GetAsync("Contacts");
                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    //dataTable = JsonConvert.DeserializeObject<DataTable>(results);
                    users = JsonConvert.DeserializeObject<List<UserEntity>>(results);
                }
                else
                {
                    Console.WriteLine("Error calling with API");

                }
                //ViewData.Model = dataTable;
                ViewData.Model = users;
            }
            return View();
        }

        public async Task<ActionResult<String>> Contacts(UserEntity user)
        {
            UserEntity entity = new UserEntity()
            {  
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,

            };
            if (user.FullName != null && user.Phone != 0 && user.Email != null && user.Address != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL + "api/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PostAsJsonAsync<UserEntity>("Contacts", entity);
                    if (getData.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        Console.WriteLine("Error calling with API");

                    }
                    
                }

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}