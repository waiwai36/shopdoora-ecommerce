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
    public class ProductService : IProductService
    {
       private readonly AppDbContext _context;

       public ProductService(AppDbContext context)
       {
           _context = context;
       }

    public async Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await _context.Products.AsNoTracking().CountAsync();
        var items = await _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<(bool Success, Product? Product, string? ErrorMessage)> CreateAsync(CreateProductDto dto)
    {
        if (dto.Price <= 0) return (false, null, "Product price must be greater than 0.");
        if (dto.Stock < 0) return (false, null, "Product stock must be a non-negative number or 0.");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return (true, product, null);
    }

    public async Task<(bool Success, Product? Product, string? ErrorMessage)> UpdateAsync(int id, UpdateProductDto dto)
    {
        if (dto.Price <= 0) return (false, null, "Product price must be greater than 0.");
        if (dto.Stock < 0) return (false, null, "Product stock must be a non-negative number or 0.");

        var product = await _context.Products.FindAsync(id);
        if (product == null) return (false, null, "Product not found.");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();

        return (true, product, null);
    }
    
 }

}

