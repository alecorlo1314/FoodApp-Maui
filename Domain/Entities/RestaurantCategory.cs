namespace FoodApp.Domain.Entities;

public class RestaurantCategory
{
    public long Id { get; set; }
    public long RestaurantId { get; set; }
    public string Name { get; set; } = string.Empty;
}