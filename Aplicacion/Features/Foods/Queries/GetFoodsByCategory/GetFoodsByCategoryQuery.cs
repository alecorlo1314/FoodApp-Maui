using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;

public record GetFoodsByCategoryQuery(long RestaurantId, long CategoryId) : IRequest<Result<IEnumerable<FoodItem>>>;