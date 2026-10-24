using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FoodApp.Infrastructure.Models;

[Table("profiles")]
public class ProfileModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("default_address")]
    public string DefaultAddress { get; set; } = string.Empty;
}
