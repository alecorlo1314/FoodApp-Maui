using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Domain.Entities;

namespace FoodApp.Infrastructure.Models
{
    [Table("restaurant_categories")]
    public class RestaurantCategoryModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("restaurant_id")]
        public long RestaurantId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public RestaurantCategory ToDomain() => new()
        {
            Id = Id,
            RestaurantId = RestaurantId,
            Name = Name
        };
    }
}