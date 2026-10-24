using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default);
    Task AddItemAsync(CartItem item, CancellationToken cancellationToken = default);
    Task UpdateQuantityAsync(long cartItemId, int quantity, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(long cartItemId, CancellationToken cancellationToken = default);
    Task<int> GetCountCartQuery(string userId, CancellationToken cancellationToken = default);
}