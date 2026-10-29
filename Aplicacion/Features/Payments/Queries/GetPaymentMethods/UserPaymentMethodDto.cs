namespace FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

public sealed record UserPaymentMethodDto(
    long Id = 0,
    string UserId = "",
    string CardHolderName = "",
    string CardNumberMasked = "",
    string Brand = "",
    string ExpiryDate = "",
    bool IsDefault = false);
