using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Domain.Entities;

namespace FoodApp.Infrastructure.Models
{
    [Table("restaurants")]
    public class RestaurantModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("tags")]
        public List<string> Tags { get; set; } = new();

        [Column("rating")]
        public double Rating { get; set; }

        [Column("delivery_fee")]
        public string DeliveryFee { get; set; } = string.Empty;

        [Column("delivery_time")]
        public string DeliveryTime { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        public Restaurant ToDomain() => new()
        {
            Id = Id,
            Name = Name,
            ImageUrl = ImageUrl,
            Tags = Tags ?? new(),
            Rating = Rating,
            DeliveryFee = DeliveryFee,
            DeliveryTime = DeliveryTime,
            Description = Description
        };
    }
}