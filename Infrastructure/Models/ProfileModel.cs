using FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;
using FoodApp.Domain.Entities;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FoodApp.Infrastructure.Models;

[Table("profiles")]
public class ProfileModel : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("photo")]
    public string? Photo { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    public ProfileDto ToDomain() => 
        new(Id, Name, Email, Photo, Bio, PhoneNumber);
}