namespace FoodApp.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public User(string id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}
