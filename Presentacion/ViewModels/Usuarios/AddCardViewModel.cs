using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Payments.Commands.AddPaymenthMethod;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

[QueryProperty(nameof(CardBrand), "CardBrand")]
public partial class AddCardViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    private string cardHolderName = string.Empty;

    [ObservableProperty]
    private string cardNumber = string.Empty;

    [ObservableProperty]
    private string expireDate = string.Empty;

    [ObservableProperty]
    private string cvc = string.Empty;

    [ObservableProperty]
    private string cardBrand = string.Empty;

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task SaveAndMakePaymentAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Extraemos los últimos 4 dígitos del número de tarjeta ingresado
            string lastFour = CardNumber.Length >= 4 ? CardNumber[^4..] : "1234";

            var newCard = new UserPaymentMethod
            {
                CardHolderName = CardHolderName,
                CardNumberMasked = lastFour,
                Brand = CardBrand, // O detectarlo según el número
                ExpiryDate = ExpireDate,
                IsDefault = true
            };

            var resultado = await _mediator.Send(new AddPaymentMethodCommand(newCard), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            await Shell.Current.DisplayAlertAsync("Éxito", "Tarjeta agregada correctamente", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}
