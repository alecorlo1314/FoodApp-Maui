namespace FoodApp.Presentacion.Models;

public class PopularFoodModel
{
    public string Name { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
