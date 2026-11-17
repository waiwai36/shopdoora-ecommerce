using Microsoft.AspNetCore.Identity;

namespace ShopDoora.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}