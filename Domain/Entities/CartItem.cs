namespace FoodApp.Domain.Entities;

public class CartItem
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public long FoodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string SizeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalItemPrice => Price * Quantity;
    public string TotalItemPriceFormatted => $"${TotalItemPrice:F0}";
}