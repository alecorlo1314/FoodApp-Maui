using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Domain.Entities;

namespace FoodApp.Infrastructure.Models;

[Table("foods")]
public class FoodItemModel : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("restaurant_id")]
    public long RestaurantId { get; set; }

    [Column("category_id")]
    public long RestaurantCategoryId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    public FoodItem ToDomain() => new()
    {
        Id = Id,
        RestaurantId = RestaurantId,
        RestaurantCategoryId = RestaurantCategoryId,
        Name = Name,
        Description = Description,
        ImageUrl = ImageUrl,
        Price = Price
    };
}