using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDoora.Api.DTOs;
using ShopDoora.Api.Services;

namespace ShopDoora.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetAll(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _orderService.GetAllAsync(userId, isAdmin, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var (found, forbidden, order) = await _orderService.GetByIdAsync(id, userId, isAdmin);

        if (!found) return NotFound();
        if (forbidden) return Forbid();

        return Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var (success, errorMessage, order) = await _orderService.CreateAsync(userId, dto);
        if (!success)
            return BadRequest(errorMessage);

        return CreatedAtAction(nameof(GetById), new { id = order!.Id }, order);
    }
}