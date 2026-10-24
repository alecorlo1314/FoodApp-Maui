using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories;

public record GetRestaurantCategoriesQuery(long RestaurantId) : IRequest<Result<IEnumerable<RestaurantCategory>>>;