using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Payments.Commands.ProcessPayment;
using FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;
using FoodApp.Domain.Entities;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

[QueryProperty(nameof(TotalAmount), "TotalAmount")]
public partial class PaymentViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPaymentViewVisible))]
    private bool isProcessing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPaymentViewVisible))]
    private bool isPaymentFailed;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool IsPaymentViewVisible =>
        !IsProcessing && !IsPaymentFailed;

    [ObservableProperty]
    private decimal totalAmount;

    [ObservableProperty]
    private bool hasCard;

    [ObservableProperty]
    private UserPaymentMethod? savedCard;

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading)
            return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsPaymentFailed = false;
            ErrorMessage = string.Empty;
            IsProcessing = false;
            IsLoading = true;
            await LoadPaymentMethodAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Error al cargar los datos", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadPaymentMethodAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _mediator.Send(new GetPaymentMethodsQuery(), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (result.IsFailure)
        {
            await Shell.Current.DisplayAlertAsync("Error", result.ErrorMessage, "OK");
            return;
        }
        var cardModel = result.Value.FirstOrDefault();

        if (cardModel != null)
        {
            SavedCard = cardModel;
            HasCard = true; // Muestra la tarjeta registrada (Payment Method.png)
        }
        else
        {
            HasCard = false; // Muestra el estado vacío (Payment Method_No Mastercard.png)
        }
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task NavigateToAddCardAsync()
    {
        // Navega a la vista de agregar tarjeta
        await Shell.Current.GoToAsync(nameof(AddCardPage));
    }

    [RelayCommand]
    private async Task PayAndConfirmAsync(CancellationToken cancellationToken)
    {
        if (IsProcessing || !HasCard)
            return;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsPaymentFailed = false;
            ErrorMessage = string.Empty;
            IsProcessing = true;

            if (SavedCard == null)
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Por favor agrega o selecciona una tarjeta de crédito.", "OK");
                return;
            }

            // Ejecutamos la solicitud de cobro en tiempo real a la API de Stripe
            var result = await _mediator.Send(new ProcessPaymentCommand(
                TotalAmount,
                SavedCard.CardNumberMasked, // O el número de tarjeta introducido
                SavedCard.ExpiryDate,
                "123" // CVC de prueba
            ), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (result.IsSuccess)
            {
                // Limpiar el carrito en la base de datos si el pago fue aprobado...

                // Navegar a la pantalla de felicitaciones con Lottie
                await Shell.Current.GoToAsync(nameof(PaymentSuccessPage));
            }
            else
            {
                ErrorMessage = string.IsNullOrWhiteSpace(
                    result.ErrorMessage)
                    ? "No se pudo completar el pago."
                    : result.ErrorMessage;

                IsPaymentFailed = true;
                return;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ErrorMessage =
                "Ocurrió un problema al procesar el pago.";
            System.Diagnostics.Debug.WriteLine($"Error al procesar el pago: {ex.Message}");

            IsPaymentFailed = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
