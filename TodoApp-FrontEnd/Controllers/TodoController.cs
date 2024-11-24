using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TodoApp_FrontEnd.Models;

namespace TodoApp_FrontEnd.Controllers
{
    public class TodoController : Controller
    {
        private readonly HttpClient _httpClient;
        public TodoController(HttpClient httpClient)
        {

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://todoapp-backend-4b6j.onrender.com/");
        }
        private void AddAUthToken()
        {
            var token = HttpContext.Session.GetString("token");
            _httpClient.DefaultRequestHeaders.Remove("token");
            _httpClient.DefaultRequestHeaders.Add("token", token);
        }
        public async Task<IActionResult> Index()
        { var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token)) 
            {
             return RedirectToAction("Login","User");
            }
            IEnumerable<Todo> todos = null;
            AddAUthToken();
            try
            {
                var response = await _httpClient.GetAsync("/todo");

                if (response.IsSuccessStatusCode)
                {
                    todos = await response.Content.ReadFromJsonAsync<IEnumerable<Todo>>();
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


            return View(todos ?? new List<Todo>());
        }
        public IActionResult Create ()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {

            AddAUthToken();
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(todo), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/todo/create", data);

                if (response.IsSuccessStatusCode) 
                {
                    return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(string _id)
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            IEnumerable<Todo> todos = null;
            AddAUthToken();
            try
            {
                var response = await _httpClient.GetAsync("/todo");

                if (response.IsSuccessStatusCode)
                {
                    todos = await response.Content.ReadFromJsonAsync<IEnumerable<Todo>>();
                    Todo todo = todos?.FirstOrDefault(t => t._id == _id);
                    if (todo != null)
                    {
                        return View(todo);
                    }
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

        [HttpPost]
        public async Task<IActionResult> Edit(string _id,Todo todo)
        {
            
            AddAUthToken();
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(todo), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/todo/update/{_id}", data);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string _id)
        {
            AddAUthToken();
            try
            {
                var response = await _httpClient.DeleteAsync($"/todo/delete/{_id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }
    }
   



}
