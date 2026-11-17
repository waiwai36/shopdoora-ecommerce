using Microsoft.AspNetCore.Mvc;
using ShopDoora.Web.Models;

namespace ShopDoora.Web.Controllers;

public class ProductsController : BaseApiController
{
    public ProductsController(IHttpClientFactory clientFactory) : base(clientFactory) { }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        var res = await client.GetFromJsonAsync<PagedResult<ProductViewModel>>($"products?pageNumber={pageNumber}&pageSize={pageSize}");
        ViewBag.IsAdmin = IsAdmin;
        return View(res);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdmin) return Forbid();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!IsAdmin) return Forbid();
        var client = CreateAuthorizedClient();
        var res = await client.PostAsJsonAsync("products", model);
        if (!res.IsSuccessStatusCode)
        {
            ViewBag.Error = await res.Content.ReadAsStringAsync();
            return View(model);
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin) return Forbid();
        var client = CreateAuthorizedClient();
        var product = await client.GetFromJsonAsync<ProductViewModel>($"products/{id}");
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        if (!IsAdmin) return Forbid();
        var client = CreateAuthorizedClient();
        var res = await client.PutAsJsonAsync($"products/{model.Id}", model);
        if (!res.IsSuccessStatusCode)
        {
            ViewBag.Error = await res.Content.ReadAsStringAsync();
            return View(model);
        }
        return RedirectToAction("Index");
    }
}