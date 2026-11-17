using Microsoft.AspNetCore.Mvc;
using ShopDoora.Api.DTOs;
using ShopDoora.Api.Services;

namespace ShopDoora.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, errors, message) = await _authService.RegisterAsync(dto);
        
        if (!success)
            return BadRequest(errors);

        return Ok(new { message });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        var (success, response, message) = await _authService.LoginAsync(dto);

        if (!success)
            return Unauthorized(new { message });

        return Ok(response);
    }
}