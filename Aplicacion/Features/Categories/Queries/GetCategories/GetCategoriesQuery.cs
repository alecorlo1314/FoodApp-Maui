using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery() : IRequest<Result<IEnumerable<CategoryDto>>>;