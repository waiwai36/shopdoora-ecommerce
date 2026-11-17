using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ShopDoora.Api.DTOs;

namespace ShopDoora.Api.Services
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<IdentityError>? Errors, string? Message)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, AuthResponseDto? Response, string? Message)> LoginAsync(LoginDto dto);
    }
}