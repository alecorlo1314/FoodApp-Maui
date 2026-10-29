using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;
using FoodApp.Aplicacion.Features.Cart.Commands.RemoveCartItem;
using FoodApp.Aplicacion.Features.Cart.Commands.UpdateQuantity;
using FoodApp.Aplicacion.Features.Cart.Queries.GetCart;
using FoodApp.Presentacion.Models;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Collections.ObjectModel;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class CartViewModel(IMediator _mediator, ILocationService locationService) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CartItemViewModel> cartItems = new();

    [ObservableProperty]
    private decimal subTotal;

    [ObservableProperty]
    private decimal deliveryFee = 0.00m;

    [ObservableProperty]
    private decimal total;

    [ObservableProperty]
    private string deliveryAddress;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditButtonText))]
    private bool isNotEditing = true;

    // Texto dinámico para el botón superior derecho
    public string EditButtonText => IsEditing ? "DONE" : "EDIT ITEMS";

    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditing = !IsEditing;
        IsNotEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading)
            return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsLoading = true;

            // Operaciones asíncronas independientes
            await Task.WhenAll(
                LoadCartAsync(cancellationToken),
                GetCurrentLocationAsync(cancellationToken)
            );
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GetCurrentLocationAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var currentAddress = await locationService.GetCurrentAddressAsync(cancellationToken);
            if (!string.IsNullOrEmpty(currentAddress))
            {
                DeliveryAddress = currentAddress;
            }

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Error", "No se pudo obtener la ubicación actual.", "OK");
        }
    }

    public async Task LoadCartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // Obtener el ID del usuario actual logeoado en supabase
            var userId = await _mediator.Send(new GetCurrentUser(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            //si existe alguno entonces se realiza la consulta para obtener el carrito de compras
            if (userId.IsSuccess && !string.IsNullOrEmpty(userId.Value?.Id.ToString()))
            {
                var result = await _mediator.Send(new GetCartQuery(userId.Value.Id.ToString()), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                //si existe carrito de compras entonces llenamos la lista de productos en el carrito
                if (result.IsSuccess && result.Value != null)
                {
                    CartItems.Clear();
                    foreach (var item in result.Value)
                    {
                        CartItems.Add(new CartItemViewModel(item));
                    }

                    CalculateTotals();
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Manejar errores, por ejemplo, mostrar un mensaje al usuario
            await Shell.Current.DisplayAlertAsync("Error", $"{ex.Message}", "OK");
        }
    }

    private void CalculateTotals()
    {
        SubTotal = CartItems
            .Sum(x => x.TotalItemPrice);
        Total = SubTotal + DeliveryFee;

        //Notificamos a MAUI para que pueda ejecutar el comando PlcaOrderCommand
        PlaceOrderCommand.NotifyCanExecuteChanged();
    }

    private bool CanPlaceOrder() => CartItems.Any() && !CartItems.Any(item => item.IsUpdating);

    #region Commands Cambiar cantidades de productos en el carrito
    [RelayCommand]
    private Task IncreaseQuantity(CartItemViewModel item, CancellationToken cancellationToken)
        => UpdateQuantityAsync(item, 1, cancellationToken);

    [RelayCommand]
    private async Task DecreaseQuantity(CartItemViewModel item, CancellationToken cancellationToken)
    {
        if (item is null || item.IsUpdating)
            return;

        if (item.Quantity > 1)
            await UpdateQuantityAsync(item, -1, cancellationToken);
        else
            await RemoveItemAsync(item, cancellationToken);
    }

    private async Task UpdateQuantityAsync(CartItemViewModel item, int change, CancellationToken cancellationToken)
    {
        if (item is null || item.IsUpdating || cancellationToken.IsCancellationRequested)
            return;

        var previousQuantity = item.Quantity;
        var saved = false;
        string? errorMessage = null;
        item.IsUpdating = true;
        item.Quantity += change;
        CalculateTotals();

        try
        {
            var result = await _mediator.Send(
                new UpdateCartQuantityCommand(item.Id, item.Quantity), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            saved = result.IsSuccess;
            if (!saved)
                errorMessage = "No se pudo actualizar la cantidad";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            errorMessage = "No se pudo actualizar la cantidad";
        }
        finally
        {
            if (!saved)
                item.Quantity = previousQuantity;

            item.IsUpdating = false;
            CalculateTotals();
        }

        if (errorMessage is not null)
            await Shell.Current.DisplayAlertAsync("Error", errorMessage, "OK");
    }

    #endregion

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    private async Task PlaceOrderAsync()
    {
        var orderItems = CartItems.Select(item => new OrderItemPayloadModel
        {
            ProductId = item.FoodId,
            Quantity = item.Quantity
        }).ToList();


        var navigationParameter = new Dictionary<string, object>
        {
            { "TotalAmount", Total },
            { "OrderItems", orderItems }
        };

        await Shell.Current.GoToAsync(nameof(PaymentPage), navigationParameter);
    }

    [RelayCommand]
    private async Task RemoveItemAsync(CartItemViewModel item, CancellationToken cancellationToken)
    {
        if (item is null || item.IsUpdating)
            return;

        item.IsUpdating = true;
        CalculateTotals();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _mediator.Send(new RemoveCartItemCommand(item.Id), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (result.IsSuccess)
            {
                CartItems.Remove(item);
                CalculateTotals(); // Esto actualizará automáticamente si el botón PlaceOrder debe bloquearse o no
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", $"No se pudo eliminar el producto: {result.ErrorMessage}", "OK");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            item.IsUpdating = false;
            CalculateTotals();
        }
    }
}
