using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopDoora.Api.Data;
using ShopDoora.Api.DTOs;
using ShopDoora.Api.Models;

namespace ShopDoora.Api.Services
{
 public class OrderService : IOrderService
 {
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrderResponseDto>> GetAllAsync(string? userId, bool isAdmin, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _context.Orders.AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(o => o.UserId == userId);

        var totalCount = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = orders.Select(MapToDto).ToList();

        return new PagedResult<OrderResponseDto>
        {
            Items = result,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<(bool Found, bool Forbidden, OrderResponseDto? Order)> GetByIdAsync(int id, string? userId, bool isAdmin)
    {
        var order = await _context.Orders.AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) 
            return (false, false, null);
            
        if (!isAdmin && order.UserId != userId) 
            return (true, true, null);

        return (true, false, MapToDto(order));
    }

    public async Task<(bool Success, string? ErrorMessage, OrderResponseDto? Order)> CreateAsync(string userId, CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return (false, "Order must contain at least one product.", null);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0
            };

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return (false, $"Product ID {item.ProductId} not found.", null);
                if (product.Stock < item.Quantity)
                    return (false, $"Insufficient stock for product {product.Name}.", null);
                
                product.Stock -= item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.TotalAmount += product.Price * item.Quantity;
                order.OrderItems.Add(orderItem);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var createdOrder = await _context.Orders.AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstAsync(o => o.Id == order.Id);

            return (true, null, MapToDto(createdOrder));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static OrderResponseDto MapToDto(Order o) => new(
        o.Id,
        o.User?.Email ?? "Unknown",
        o.OrderDate,
        o.TotalAmount,
        o.OrderItems.Select(oi => new OrderItemResponseDto(
            oi.ProductId,
            oi.Product?.Name ?? "Unknown",
            oi.Quantity,
            oi.UnitPrice,
            oi.Quantity * oi.UnitPrice
        )).ToList()
    );
 }
}