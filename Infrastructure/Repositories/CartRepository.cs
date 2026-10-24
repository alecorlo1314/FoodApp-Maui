using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class CartRepository(Client _supabaseClient) : ICartRepository
    {
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<CartItemModel>()
                .Where(x => x.UserId == Guid.Parse(userId))
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDomain());
        }

        public async Task AddItemAsync(CartItem item, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Verificar si el producto con el mismo tamaño ya está en el carrito del usuario
            var existing = await _supabaseClient
                .From<CartItemModel>()
                .Where(x => x.UserId == item.UserId &&
                x.FoodId == item.FoodId && 
                x.SizeName == item.SizeName)
                .Get(cancellationToken: cancellationToken);

            var existingModel = existing.Models.FirstOrDefault();

            if (existingModel != null)
            {
                // Si ya existe, sumamos la cantidad
                existingModel.Quantity += item.Quantity;
                await _supabaseClient.From<CartItemModel>().Update(existingModel, cancellationToken: cancellationToken);
            }
            else
            {
                // Si no existe, lo creamos nuevo
                var model = new CartItemModel
                {
                    UserId = item.UserId,
                    FoodId = item.FoodId,
                    Name = item.Name,
                    ImageUrl = item.ImageUrl,
                    SizeName = item.SizeName,
                    Price = item.Price,
                    Quantity = item.Quantity
                };
                await _supabaseClient.From<CartItemModel>().Insert(model, cancellationToken: cancellationToken);
            }
        }

        public async Task UpdateQuantityAsync(long cartItemId, int quantity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var model = new CartItemModel { Id = cartItemId, Quantity = quantity };
            // En Supabase Postgrest para actualizar un campo específico de forma sencilla:
            await _supabaseClient
                .From<CartItemModel>()
                .Where(x => x.Id == cartItemId)
                .Set(x => x.Quantity, quantity)
                .Update(cancellationToken: cancellationToken);
        }

        public async Task RemoveItemAsync(long cartItemId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _supabaseClient.From<CartItemModel>()
                .Where(x => x.Id == cartItemId)
                .Delete(cancellationToken: cancellationToken);
        }

        public async Task<int> GetCountCartQuery(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var resupuesta = await _supabaseClient
                            .From<CartItemModel>()
                            .Where(x => x.UserId == Guid.Parse(userId))
                            .Get(cancellationToken: cancellationToken);

            return resupuesta.Count;
        }
    }
}