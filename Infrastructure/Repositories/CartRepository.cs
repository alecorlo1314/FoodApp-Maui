using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class CartRepository(Client _supabaseClient) : ICartRepository
    {
        private static readonly SemaphoreSlim AddItemLock = new(1, 1);

        #region Get Cart Items
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<CartItemModel>()
                .Where(x => x.UserId == Guid.Parse(userId))
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDomain());
        }
        #endregion

        #region Add or Update Item in Cart
        public async Task AddItemAsync(CartItemEntidad item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            cancellationToken.ThrowIfCancellationRequested();
            await AddItemLock.WaitAsync(cancellationToken);
            try
            {
                var existing = await _supabaseClient
                    .From<CartItemModel>()
                    .Where(x => x.UserId == item.UserId &&
                        x.FoodId == item.FoodId &&
                        x.SizeName == item.SizeName)
                    .Get(cancellationToken: cancellationToken);

                var existingModel = existing.Models.SingleOrDefault();

                if (existingModel != null)
                {
                    var cartItem = existingModel.ToEntidad();
                    cartItem.IncreaseQuantity(item.Quantity);
                    cartItem.UpdatePrice(item.Price);

                    await _supabaseClient.From<CartItemModel>()
                        .Where(x => x.Id == cartItem.Id && x.UserId == item.UserId)
                        .Set(x => x.Quantity, cartItem.Quantity)
                        .Set(x => x.Price, cartItem.Price)
                        .Update(cancellationToken: cancellationToken);
                }
                else
                {
                    var model = CartItemModel.ToModel(item);
                    await _supabaseClient.From<CartItemModel>()
                        .Insert(model, cancellationToken: cancellationToken);
                }
            }
            finally
            {
                AddItemLock.Release();
            }
        }
        #endregion

        #region Update Item Quantity
        public async Task UpdateQuantityAsync(long cartItemId, int quantity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));
            // En Supabase Postgrest para actualizar un campo específico de forma sencilla:
            await _supabaseClient
                .From<CartItemModel>()
                .Where(x => x.Id == cartItemId)
                .Set(x => x.Quantity, quantity)
                .Update(cancellationToken: cancellationToken);
        }
        #endregion

        #region Remove Specific Item
        public async Task RemoveItemAsync(long cartItemId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _supabaseClient.From<CartItemModel>()
                .Where(x => x.Id == cartItemId)
                .Delete(cancellationToken: cancellationToken);
        }
        #endregion

        #region Clear Entire Cart
        public async Task ClearAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == Guid.Empty)
                throw new ArgumentException("El usuario del carrito no es válido.", nameof(userId));
            await _supabaseClient.From<CartItemModel>()
                .Where(item => item.UserId == userId)
                .Delete(cancellationToken: cancellationToken);
        }
        #endregion

        #region Get Cart Items Count
        public async Task<int> GetCountCartQuery(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var resupuesta = await _supabaseClient
                            .From<CartItemModel>()
                            .Where(x => x.UserId == Guid.Parse(userId))
                            .Get(cancellationToken: cancellationToken);

            return resupuesta.Count;
        }
        #endregion
    }
}
