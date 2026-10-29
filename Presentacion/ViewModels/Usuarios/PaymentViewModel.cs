using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;
using FoodApp.Aplicacion.Features.Cart.Commands.ClearCart;
using FoodApp.Aplicacion.Features.Cart.Queries.GetCart;
using FoodApp.Aplicacion.Features.Orders.Command.RecordOrderMetrics;
using FoodApp.Aplicacion.Features.Payments.Commands.ProcessPayment;
using FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;
using FoodApp.Domain.Entities;
using FoodApp.Presentacion.Models;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

[QueryProperty(nameof(TotalAmount), "TotalAmount")]
[QueryProperty(nameof(OrderItems), "OrderItems")]
public partial class PaymentViewModel(IMediator _mediator) : ObservableObject
{
    private Guid _checkoutId = Guid.NewGuid();
    private Guid _checkoutUserId;
    private List<OrderItemPayload>? _checkoutItems;
    private UserPaymentMethodDto? _checkoutCard;
    private decimal _checkoutAmount;
    private string? _confirmedPaymentId;
    private bool _paymentSubmitted;
    private bool _metricsRecorded;
    private bool _cartCleared;
    private bool _checkoutCompleted;

    public string PaymentRetryText => _confirmedPaymentId is null ? "TRY AGAIN" : "COMPLETE ORDER";

    partial void OnOrderItemsChanged(List<OrderItemPayloadModel> value)
    {
        if (_paymentSubmitted && !_checkoutCompleted)
            return;

        _checkoutId = Guid.NewGuid();
        _checkoutUserId = Guid.Empty;
        _checkoutItems = null;
        _checkoutCard = null;
        _confirmedPaymentId = null;
        _paymentSubmitted = _metricsRecorded = _cartCleared = _checkoutCompleted = false;
        OnPropertyChanged(nameof(PaymentRetryText));
    }

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
    private List<OrderItemPayloadModel> orderItems = new();

    [ObservableProperty]
    private bool hasCard;

    [ObservableProperty]
    private UserPaymentMethodDto? savedCard;

    [ObservableProperty]
    private bool isLoading;

    // Propiedades nuevas para el BottomSheet y filtrado
    [ObservableProperty]
    private bool isBottomSheetOpen;

    [ObservableProperty]
    private List<UserPaymentMethodDto> allCards = new();

    [ObservableProperty]
    private List<UserPaymentMethodDto> filteredCards = new();

    [ObservableProperty]
    private string selectedBrandFilter = "All";

    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading || IsProcessing) return;

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
    private void SelectCard(UserPaymentMethodDto card)
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
        if (IsProcessing || IsLoading)
            return;

        try
        {
            //Iniciliza datos necesarios
            cancellationToken.ThrowIfCancellationRequested();
            IsPaymentFailed = false;
            ErrorMessage = string.Empty;
            IsBottomSheetOpen = false;
            IsProcessing = true;

            // Obtiene el usuario actual
            var user = await _mediator.Send(new GetCurrentUser(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            //Si no hay usuario, muestra un mensaje de error
            if (user.IsFailure || user.Value is null || user.Value.Id == Guid.Empty)
            {
                ShowPaymentFailure("Inicia sesión antes de completar el pedido.");
                return;
            }

            //Si el es diferente al que hizo el pago, muestra un mensaje de error
            if (_checkoutUserId != Guid.Empty && _checkoutUserId != user.Value.Id)
            {
                ShowPaymentFailure("La sesión ha cambiado. Vuelve a ingresar con el usuario del pago.");
                return;
            }

            //Si no hay productos en el carrito, muestra un mensaje de error
            if (_checkoutItems is null)
            {
                if (SavedCard is null)
                {
                    ShowPaymentFailure("Por favor selecciona una tarjeta.");
                    return;
                }
                if (TotalAmount <= 0)
                {
                    ShowPaymentFailure("El monto del pedido debe ser mayor que cero.");
                    return;
                }

                // Congelar los productos del carrito antes de enviar el cobro.
                var cart = await _mediator.Send(new GetCartQuery(user.Value.Id.ToString()), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                //Si no hay carrito de compras, muestra un mensaje de error
                if (cart.IsFailure)
                {
                    ShowPaymentFailure(cart.ErrorMessage);
                    return;
                }

                var items = cart.Value?.ToList();
                //Si no hay productos en el carrito, muestra un mensaje de error
                if (items is null || items.Count == 0 || items.Any(item => item.FoodId <= 0 || item.Quantity <= 0))
                {
                    ShowPaymentFailure("El carrito está vacío o contiene productos no válidos.");
                    return;
                }

                // Preparar los datos para el checkout
                _checkoutItems = items.GroupBy(item => item.FoodId).Select(group => new OrderItemPayload
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity)
                }).ToList();


                _checkoutUserId = user.Value.Id;
                _checkoutAmount = TotalAmount;
                _checkoutCard = new UserPaymentMethodDto
                {
                    CardNumberMasked = SavedCard.CardNumberMasked,
                    ExpiryDate = SavedCard.ExpiryDate
                };
            }

            // 1. Cobrar una sola vez por operación.
            if (_confirmedPaymentId is null)
            {
                _paymentSubmitted = true;
                var payment = await _mediator.Send(new ProcessPaymentCommand(
                    _checkoutAmount, _checkoutCard!.CardNumberMasked, _checkoutCard.ExpiryDate,
                    "123", _checkoutId), cancellationToken);
                // Conservar la aprobación antes de observar una cancelación tardía.
                if (payment.IsSuccess && !string.IsNullOrWhiteSpace(payment.Value))
                {
                    _confirmedPaymentId = payment.Value;
                    OnPropertyChanged(nameof(PaymentRetryText));
                }
                cancellationToken.ThrowIfCancellationRequested();
                if (_confirmedPaymentId is null)
                {
                    ShowPaymentFailure(payment.IsFailure ? payment.ErrorMessage : "No se recibió el identificador del pago.");
                    return;
                }
            }

            // 2. Confirmar las métricas antes de permitir la limpieza.
            if (!_metricsRecorded)
            {
                var metrics = await _mediator.Send(new RecordOrderMetricsCommand(
                    _checkoutItems, _checkoutUserId, _confirmedPaymentId), cancellationToken);
                _metricsRecorded = metrics.IsSuccess && metrics.Value;
                cancellationToken.ThrowIfCancellationRequested();
                if (!_metricsRecorded)
                {
                    ShowPaymentFailure(string.IsNullOrWhiteSpace(metrics.ErrorMessage)
                        ? "No se confirmó el registro de métricas." : metrics.ErrorMessage);
                    return;
                }
            }

            // 3. Vaciar y confirmar el carrito del usuario del pago.
            if (!_cartCleared)
            {
                var cleared = await _mediator.Send(new ClearCartCommand(_checkoutUserId), cancellationToken);
                _cartCleared = cleared.IsSuccess && cleared.Value;
                cancellationToken.ThrowIfCancellationRequested();
                if (!_cartCleared)
                {
                    ShowPaymentFailure(string.IsNullOrWhiteSpace(cleared.ErrorMessage)
                        ? "No se confirmó la limpieza del carrito." : cleared.ErrorMessage);
                    return;
                }
            }

            // 4. Navegar solamente cuando todas las etapas estén confirmadas.
            cancellationToken.ThrowIfCancellationRequested();
            _checkoutCompleted = true;
            await Shell.Current.GoToAsync(nameof(PaymentSuccessPage));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            ShowPaymentFailure("La finalización del pedido fue cancelada. Puedes reintentar la misma operación.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al completar el pedido: {ex.Message}");
            ShowPaymentFailure("No se pudo completar el pedido. Puedes reintentar la misma operación.");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private void ShowPaymentFailure(string message)
    {
        var detail = string.IsNullOrWhiteSpace(message) ? "No se pudo completar el pedido." : message;
        ErrorMessage = _confirmedPaymentId is null
            ? detail
            : $"El pago ya fue aprobado ({_confirmedPaymentId}). {detail} Al reintentar se completará el pedido sin volver a cobrar.";
        IsPaymentFailed = true;
    }
}
