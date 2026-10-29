namespace FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories;

public sealed record RestaurantCategoryDto(long Id = 0, long RestaurantId = 0, string Name = "");
