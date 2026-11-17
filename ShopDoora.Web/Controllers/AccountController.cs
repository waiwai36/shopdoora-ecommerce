using Microsoft.AspNetCore.Mvc;
using ShopDoora.Web.Models;

namespace ShopDoora.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AccountController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var client = _clientFactory.CreateClient("ShopDooraApi");
        var response = await client.PostAsJsonAsync("auth/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Invalid Email or Password.";
            return View();
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        HttpContext.Session.SetString("JWToken", auth!.Token);
        HttpContext.Session.SetString("UserEmail", auth.Email);
        HttpContext.Session.SetString("UserRole", auth.Role);

        return RedirectToAction("Index", "Products");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password, string role = "User")
    {
        var client = _clientFactory.CreateClient("ShopDooraApi");
        var response = await client.PostAsJsonAsync("auth/register", new { fullName, email, password, role });

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = await response.Content.ReadAsStringAsync();
            return View();
        }

        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}