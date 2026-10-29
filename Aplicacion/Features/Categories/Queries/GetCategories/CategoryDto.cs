namespace FoodApp.Aplicacion.Features.Categories.Queries.GetCategories;

public sealed record CategoryDto(Guid Id = default, string Name = "", string IconUrl = "");
