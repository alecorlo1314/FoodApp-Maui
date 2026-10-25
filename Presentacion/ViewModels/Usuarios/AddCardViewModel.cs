using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Payments.Commands.AddPaymenthMethod;
using FoodApp.Domain.Entities;
using MediatR;
using System.Text.RegularExpressions;

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

    // Controla si el formulario completo es válido para desbloquear el botón
    [ObservableProperty]
    private bool isFormValid;

    // --- PROPIEDADES DE ERROR POR CAMPO ---
    [ObservableProperty] private string? cardHolderNameError;
    [ObservableProperty] private bool hasCardHolderNameError;

    [ObservableProperty] private string? cardNumberError;
    [ObservableProperty] private bool hasCardNumberError;

    [ObservableProperty] private string? expireDateError;
    [ObservableProperty] private bool hasExpireDateError;

    [ObservableProperty] private string? cvcError;
    [ObservableProperty] private bool hasCvcError;

    // ==========================================
    // MÉTODOS REACTIVOS (Se ejecutan al escribir)
    // ==========================================
    partial void OnCardHolderNameChanged(string value)
    {
        ValidateCardHolderName(value);
        UpdateFormValidity();
    }

    partial void OnCardNumberChanged(string value)
    {
        ValidateCardNumber(value);
        UpdateFormValidity();
    }

    partial void OnExpireDateChanged(string value)
    {
        ValidateExpireDate(value);
        UpdateFormValidity();
    }

    partial void OnCvcChanged(string value)
    {
        ValidateCvc(value);
        UpdateFormValidity();
    }

    private void ValidateCardHolderName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Split(' ').Length < 2)
        {
            CardHolderNameError = "Ingresa nombre y apellido válidos.";
            HasCardHolderNameError = true;
        }
        else
        {
            CardHolderNameError = null;
            HasCardHolderNameError = false;
        }
    }

    private void ValidateCardNumber(string value)
    {
        var clean = (value ?? "").Replace(" ", "").Trim();
        if (string.IsNullOrWhiteSpace(clean) || clean.Length < 16)
        {
            CardNumberError = "El número de tarjeta debe tener 16 dígitos.";
            HasCardNumberError = true;
        }
        else
        {
            CardNumberError = null;
            HasCardNumberError = false;
        }
    }

    private void ValidateExpireDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^(0[1-9]|1[0-2])\/?([0-9]{2})$"))
        {
            ExpireDateError = "Formato inválido. Usa MM/YY.";
            HasExpireDateError = true;
        }
        else
        {
            ExpireDateError = null;
            HasExpireDateError = false;
        }
    }

    private void ValidateCvc(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
        {
            CvcError = "El CVC requiere 3 dígitos.";
            HasCvcError = true;
        }
        else
        {
            CvcError = null;
            HasCvcError = false;
        }
    }

    private void UpdateFormValidity()
    {
        // El formulario es válido solo si no hay errores y ningún campo está vacío
        IsFormValid = !HasCardHolderNameError && !HasCardNumberError && !HasExpireDateError && !HasCvcError &&
                      !string.IsNullOrWhiteSpace(CardHolderName) &&
                      !string.IsNullOrWhiteSpace(CardNumber) &&
                      !string.IsNullOrWhiteSpace(ExpireDate) &&
                      !string.IsNullOrWhiteSpace(Cvc);
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task SaveAndMakePaymentAsync(CancellationToken cancellationToken)
    {
        if (!IsFormValid) return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cleanCardNumber = CardNumber.Replace(" ", "").Trim();
            string lastFour = cleanCardNumber.Length >= 4 ? cleanCardNumber[^4..] : "1234";
            string detectedBrand = string.IsNullOrWhiteSpace(CardBrand) || CardBrand.Equals("All", StringComparison.OrdinalIgnoreCase)
                ? DetectCardBrand(cleanCardNumber)
                : CardBrand;

            var newCard = new UserPaymentMethod
            {
                CardHolderName = CardHolderName.Trim(),
                CardNumberMasked = lastFour,
                Brand = detectedBrand,
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
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"No se pudo guardar la tarjeta: {ex.Message}", "OK");
        }
    }

    private string DetectCardBrand(string number)
    {
        if (number.StartsWith("4")) return "Visa";
        if (Regex.IsMatch(number, @"^5[1-5]") || Regex.IsMatch(number, @"^2[2-7]")) return "Mastercard";
        return "Visa";
    }
}