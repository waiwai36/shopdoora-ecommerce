using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ShopDoora.Web.Controllers;

public class BaseApiController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public BaseApiController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    protected HttpClient CreateAuthorizedClient()
    {
        var client = _clientFactory.CreateClient("ShopDooraApi");
        var token = HttpContext.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    protected bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken"));
    protected bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";
}