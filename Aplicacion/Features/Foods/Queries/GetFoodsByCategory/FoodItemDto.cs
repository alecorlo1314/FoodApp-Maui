namespace FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;

public sealed record FoodItemDto(
    long Id = 0,
    long RestaurantId = 0,
    long RestaurantCategoryId = 0,
    string Name = "",
    string Description = "",
    string ImageUrl = "",
    decimal Price = 0)
{
    public string PriceFormatted => $"${Price:F2}";
}
