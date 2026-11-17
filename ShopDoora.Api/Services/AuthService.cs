using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShopDoora.Api.DTOs;
using ShopDoora.Api.Models;

namespace ShopDoora.Api.Services
{
    public class AuthService :IAuthService
    {
       private readonly UserManager<ApplicationUser> _userManager;
       private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<(bool Success, IEnumerable<IdentityError>? Errors, string? Message)> RegisterAsync(RegisterDto dto)
    {
        var role = dto.Role == "Admin" ? "Admin" : "User";
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, result.Errors, null);

        await _userManager.AddToRoleAsync(user, role);
        return (true, null, "User registered successfully.");
    }

    public async Task<(bool Success, AuthResponseDto? Response, string? Message)> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return (false, null, "Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var responseDto = new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Email!,
            role
        );

        return (true, responseDto, null);
    }
  }
}
