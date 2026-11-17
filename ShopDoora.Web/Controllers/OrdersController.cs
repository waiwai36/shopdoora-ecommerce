using Microsoft.AspNetCore.Mvc;
using ShopDoora.Web.Models;

namespace ShopDoora.Web.Controllers;

public class OrdersController : BaseApiController
{
    public OrdersController(IHttpClientFactory clientFactory) : base(clientFactory) { }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        var orders = await client.GetFromJsonAsync<PagedResult<OrderViewModel>>($"orders?pageNumber={pageNumber}&pageSize={pageSize}");
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        var products = await client.GetFromJsonAsync<PagedResult<ProductViewModel>>("products?pageNumber=1&pageSize=100");
        ViewBag.Products = products?.Items ?? new List<ProductViewModel>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(List<int> productIds, List<int> quantities)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var client = CreateAuthorizedClient();
        bool hasError = false;
        int count = Math.Min(productIds.Count, quantities.Count);

        for (int i = 0; i < count; i++)
        {
            if (quantities[i] > 0)    
            {
                var singleOrderDto = new 
                { 
                    Items = new[] 
                    { 
                        new { ProductId = productIds[i], Quantity = quantities[i] } 
                    } 
                };

                var res = await client.PostAsJsonAsync("orders", singleOrderDto);
                if (!res.IsSuccessStatusCode)
                {
                    hasError = true;
                    TempData["Error"] = await res.Content.ReadAsStringAsync();
                    break;
                }
            }
        }

        if (hasError)
        {
            return RedirectToAction("Create");
        }

        return RedirectToAction("Index");
    }
}