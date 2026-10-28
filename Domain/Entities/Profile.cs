namespace FoodApp.Domain.Entities;

public class Profile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
}
