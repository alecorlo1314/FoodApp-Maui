using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories;

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

        public RestaurantCategoryDto ToDto() => new()
        {
            Id = Id,
            RestaurantId = RestaurantId,
            Name = Name
        };
    }
}