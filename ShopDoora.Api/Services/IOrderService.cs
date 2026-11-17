using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopDoora.Api.DTOs;

namespace ShopDoora.Api.Services
{
    public interface IOrderService
    {
        Task<PagedResult<OrderResponseDto>> GetAllAsync(string? userId, bool isAdmin, int pageNumber, int pageSize);
        Task<(bool Found, bool Forbidden, OrderResponseDto? Order)> GetByIdAsync(int id, string? userId, bool isAdmin);
        Task<(bool Success, string? ErrorMessage, OrderResponseDto? Order)> CreateAsync(string userId, CreateOrderDto dto);
    }
}