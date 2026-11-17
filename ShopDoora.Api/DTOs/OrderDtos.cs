using System.ComponentModel.DataAnnotations;

namespace ShopDoora.Api.DTOs;

public class OrderItemRequestDto
{
    [Required]
    public int ProductId { get; set; } 

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least one product.")]
    public List<OrderItemRequestDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public OrderItemResponseDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal SubTotal)
    {
        this.ProductId = ProductId;
        this.ProductName = ProductName;
        this.Quantity = Quantity;
        this.UnitPrice = UnitPrice;
        this.SubTotal = SubTotal;
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}

public class OrderResponseDto
{
    public OrderResponseDto(int Id, string UserEmail, DateTime OrderDate, decimal TotalAmount, List<OrderItemResponseDto> Items)
    {
        this.Id = Id;
        this.UserEmail = UserEmail;
        this.OrderDate = OrderDate;
        this.TotalAmount = TotalAmount;
        this.Items = Items;
    }

    public int Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}