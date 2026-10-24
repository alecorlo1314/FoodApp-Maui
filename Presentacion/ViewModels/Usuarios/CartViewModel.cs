using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;
using FoodApp.Aplicacion.Features.Cart.Commands.RemoveCartItem;
using FoodApp.Aplicacion.Features.Cart.Commands.UpdateQuantity;
using FoodApp.Aplicacion.Features.Cart.Queries.GetCart;
using FoodApp.Domain.Entities;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Collections.ObjectModel;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class CartViewModel(IMediator _mediator, ILocationService locationService) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CartItem> cartItems = new();

    [ObservableProperty]
    private decimal subTotal;

    [ObservableProperty]
    private decimal deliveryFee = 0.00m;

    [ObservableProperty]
    private decimal total;

    [ObservableProperty]
    private string deliveryAddress = "2118 Thornridge Cir. Syracuse"; // aun por definir

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
                        CartItems.Add(item);
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

    private bool CanPlaceOrder() => CartItems.Any();

    #region Commands Cambiar cantidades de productos en el carrito
    [RelayCommand]
    private async Task IncreaseQuantity(CartItem item, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null) return;

            // Incrementamos la cantidad localmente
            item.Quantity++;

            // Enviamos el cambio a Supabase a través de MediatR
            var result = await _mediator.Send(new UpdateCartQuantityCommand(item.Id, item.Quantity), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (!result.IsSuccess)
            {
                // Si falla en la base de datos, revertimos el cambio local
                item.Quantity--;
                await Shell.Current.DisplayAlertAsync("Error", "No se pudo actualizar la cantidad", "OK");
            }

            // Recalculamos los totales de la pantalla del carrito
            CalculateTotals();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    [RelayCommand]
    private async Task DecreaseQuantity(CartItem item, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null) return;

            if (item.Quantity > 1)
            {
                item.Quantity--;

                var result = await _mediator.Send(new UpdateCartQuantityCommand(item.Id, item.Quantity), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (!result.IsSuccess)
                {
                    item.Quantity++;
                    await Shell.Current.DisplayAlertAsync("Error", "No se pudo actualizar la cantidad", "OK");
                }

                CalculateTotals();
            }
            else
            {
                // Si llega a 0, opcionalmente podemos invocar la eliminación del ítem
                await RemoveItemAsync(item, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    #endregion

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
    private async Task PlaceOrderAsync()
    {
        // Pasamos el Total a la vista de pago mediante un parámetro de navegación
        var navigationParameter = new Dictionary<string, object>
            {
                { "TotalAmount", Total }
            };

        await Shell.Current.GoToAsync(nameof(PaymentPage), navigationParameter);
    }

    [RelayCommand]
    private async Task RemoveItemAsync(CartItem item, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null) return;

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
    }
}
