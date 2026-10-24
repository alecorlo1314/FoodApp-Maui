using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class UserPaymentRepository(Client _supabaseClient) : IUserPaymentRepository
    {
        public async Task<IEnumerable<UserPaymentMethod>> GetCardsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<UserPaymentMethodModel>()
                .Where(x => x.UserId == userId)
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDomain());
        }

        public async Task AddCardAsync(UserPaymentMethod card, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var model = new UserPaymentMethodModel
            {
                UserId = card.UserId,
                CardHolderName = card.CardHolderName,
                CardLastFour = card.CardNumberMasked,
                Brand = card.Brand,
                ExpiryDate = card.ExpiryDate,
                IsDefault = card.IsDefault
            };

            await _supabaseClient.From<UserPaymentMethodModel>().Insert(model, cancellationToken: cancellationToken);
        }
    }
}