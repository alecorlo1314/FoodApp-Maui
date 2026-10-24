using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler(ICategoryRepository _categoryRepository) : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<Category>>>
    {
        public async Task<Result<IEnumerable<Category>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
                return Result.Success(categories);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<IEnumerable<Category>>("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Category>>(ex.Message);
            }
        }
    }
}