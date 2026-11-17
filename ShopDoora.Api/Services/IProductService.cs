using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopDoora.Api.DTOs;
using ShopDoora.Api.Models;

namespace ShopDoora.Api.Services
{
    public interface IProductService
    {
      Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize);
      Task<Product?> GetByIdAsync(int id);
      Task<(bool Success, Product? Product, string? ErrorMessage)> CreateAsync(CreateProductDto dto);
      Task<(bool Success, Product? Product, string? ErrorMessage)> UpdateAsync(int id, UpdateProductDto dto);
    }
}