using System.ComponentModel.DataAnnotations;

namespace ShopDoora.Api.DTOs;

public class RegisterDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; 
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public AuthResponseDto(string Token, string Email, string Role)
    {
        this.Token = Token;
        this.Email = Email;
        this.Role = Role;
    }

    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}