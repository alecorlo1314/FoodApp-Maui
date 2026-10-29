using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Domain.Entities;

namespace FoodApp.Infrastructure.Models;

[Table("cart_items")]
public class CartItemModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; } 

    [Column("food_id")]
    public long FoodId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("size_name")]
    public string SizeName { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    public CartItem ToDomain() => new()
    {
        Id = Id,
        UserId = UserId,
        FoodId = FoodId,
        Name = Name,
        ImageUrl = ImageUrl,
        SizeName = SizeName,
        Price = Price,
        Quantity = Quantity
    };
    public CartItemEntidad ToEntidad() => new(
        UserId, FoodId, Name, ImageUrl, SizeName, Price, Quantity, Id);

    public static CartItemModel ToModel(CartItemEntidad item) => new()
    {
        Id = item.Id,
        UserId = item.UserId,
        FoodId = item.FoodId,
        Name = item.Name,
        ImageUrl = item.ImageUrl,
        SizeName = item.SizeName,
        Price = item.Price,
        Quantity = item.Quantity
    };
}
