using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using FoodApp.Aplicacion.Features.Categories.Queries.GetCategories;

namespace FoodApp.Infrastructure.Models
{
    [Table("categories")]
    public class CategoryModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("icon_url")]
        public string IconUrl { get; set; } = string.Empty;

        // Mapeo hacia el DTO de la query
        public CategoryDto ToDto() => new()
        {
            Id = Id,
            Name = Name,
            IconUrl = IconUrl
        };
    }
}