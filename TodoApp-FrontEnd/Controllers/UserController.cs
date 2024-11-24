using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using TodoApp_FrontEnd.Models;
using System.Text.Json;
namespace TodoApp_FrontEnd.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://todoapp-backend-4b6j.onrender.com/");
        }
        public IActionResult Signup()
        {
            var token = HttpContext.Session.GetString("token");
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Todo");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(signup SignupDetails)
        {
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(SignupDetails), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/user/Signup", data);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Login));
                }

                else
                {
                    ViewBag.Error = $"Error: {response.StatusCode}, Message: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }
            return View();
        }
        public IActionResult Login()
        {
            var token = HttpContext.Session.GetString("token");
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Todo");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> login(login loginDetails)
        {
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(loginDetails), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/user/login", data);

                if (response.IsSuccessStatusCode)
                {
                    var token = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync()).GetProperty("token").GetString();
                    HttpContext.Session.SetString("token", token);
                    Console.WriteLine("Token");
                    return RedirectToAction("Index","Todo");
                }

                else
                {
                    ViewBag.Error = $"Error: {response.StatusCode}, Message: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");

            return RedirectToAction("login", "User");
        }
    }
}
