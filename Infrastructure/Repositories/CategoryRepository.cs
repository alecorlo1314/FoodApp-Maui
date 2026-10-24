using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class CategoryRepository(Client _supabaseClient) : ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<CategoryModel>()
                .Get(cancellationToken: cancellationToken);
            // Ordenamos en memoria: primero la categoría "All", luego el resto alfabéticamente
            return response.Models
                .Select(m => m.ToDomain())
                .OrderByDescending(c => c.Name.Equals("All", StringComparison.OrdinalIgnoreCase))
                .ThenBy(c => c.Name);
        }
    }
}