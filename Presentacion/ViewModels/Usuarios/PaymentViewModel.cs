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

    public bool IsPaymentViewVisible => !IsProcessing && !IsPaymentFailed;

    [ObservableProperty]
    private decimal totalAmount;

    [ObservableProperty]
    private bool hasCard;

    [ObservableProperty]
    private UserPaymentMethod? savedCard;

    [ObservableProperty]
    private bool isLoading;

    // Propiedades nuevas para el BottomSheet y filtrado
    [ObservableProperty]
    private bool isBottomSheetOpen;

    [ObservableProperty]
    private List<UserPaymentMethod> allCards = new();

    [ObservableProperty]
    private List<UserPaymentMethod> filteredCards = new();

    [ObservableProperty]
    private string selectedBrandFilter = "All";

    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading) return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsBottomSheetOpen = false;
            IsPaymentFailed = false;
            ErrorMessage = string.Empty;
            IsProcessing = false;
            IsLoading = true;
            await LoadPaymentMethodAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
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

        AllCards = result.Value.ToList();
        FilteredCards = AllCards;

        SavedCard = AllCards.FirstOrDefault();
        HasCard = SavedCard is not null;
    }

    [RelayCommand]
    private void OpenBottomSheet()
    {
        FilteredCards = AllCards; // Muestra todas por defecto al abrir
        IsBottomSheetOpen = true;
    }

    [RelayCommand]
    private void FilterCards(string brand)
    {
        SelectedBrandFilter = brand;

        if (brand.Equals("Cash", StringComparison.OrdinalIgnoreCase))
        {
            // Opcional: manejar lógica si selecciona efectivo
            return;
        }

        // Filtra las tarjetas según la marca seleccionada en los iconos superiores
        FilteredCards = AllCards
            .Where(c => c.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Abre automáticamente el BottomSheet para mostrar las tarjetas de esa marca
        IsBottomSheetOpen = true;
    }

    [RelayCommand]
    private void SelectCard(UserPaymentMethod card)
    {
        if (card != null)
        {
            SavedCard = card;
            HasCard = true;
        }
        IsBottomSheetOpen = false;
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task NavigateToAddCardAsync()
    {
        // Pasa el tipo de tarjeta seleccionado (ej. "Visa" o "Mastercard") como parámetro de navegación
        var parameters = new Dictionary<string, object>
        {
            { "CardBrand", SelectedBrandFilter }
        };
        await Shell.Current.GoToAsync(nameof(AddCardPage), parameters);
    }

    [RelayCommand]
    private async Task PayAndConfirmAsync(CancellationToken cancellationToken)
    {
        if (IsProcessing || !HasCard) return;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsPaymentFailed = false;
            ErrorMessage = string.Empty;
            IsProcessing = true;

            if (SavedCard == null)
            {
                await Shell.Current.DisplayAlertAsync("Aviso", "Por favor selecciona una tarjeta.", "OK");
                return;
            }

            var result = await _mediator.Send(new ProcessPaymentCommand(
                TotalAmount,
                SavedCard.CardNumberMasked,
                SavedCard.ExpiryDate,
                "123"
            ), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (result.IsSuccess)
            {
                await Shell.Current.GoToAsync(nameof(PaymentSuccessPage));
            }
            else
            {
                ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage) ? "No se pudo completar el pago." : result.ErrorMessage;
                IsPaymentFailed = true;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception ex)
        {
            ErrorMessage = "Ocurrió un problema al procesar el pago.";
            System.Diagnostics.Debug.WriteLine($"Error al procesar el pago: {ex.Message}");
            IsPaymentFailed = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
