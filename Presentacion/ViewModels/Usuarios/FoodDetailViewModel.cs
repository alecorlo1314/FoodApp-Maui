using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;
using FoodApp.Aplicacion.Features.Cart.Commands.AddToCart;
using FoodApp.Domain.Entities;
using MediatR;
using System.Collections.ObjectModel;

namespace FoodApp.Presentacion.ViewModels.Usuarios
{
    [QueryProperty(nameof(FoodItem), "FoodItem")]
    public partial class FoodDetailViewModel(IMediator _mediator) : ObservableObject
    {
        [ObservableProperty]
        private FoodItem foodItem = new();

        [ObservableProperty]
        private ObservableCollection<FoodSize> sizes = new();

        [ObservableProperty]
        private FoodSize? selectedSize;

        [ObservableProperty]
        private int quantity = 1;

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private bool isLoading;

        [RelayCommand]
        private async Task LoadDataAsync(CancellationToken cancellationToken)
        {
            if (IsLoading || FoodItem is null || FoodItem.Id <= 0)
                return;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                IsLoading = true;
                await LoadDetailsAsync(FoodItem.Id, cancellationToken);
                CalculateTotal();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedSizeChanged(FoodSize? value)
        {
            CalculateTotal();
        }

        partial void OnQuantityChanged(int value)
        {
            CalculateTotal();
        }

        private async Task LoadDetailsAsync(long foodId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Cargamos los tamaños disponibles para este plato específico desde Supabase
            Sizes.Clear();
            Sizes.Add(new FoodSize { Id = 1, Name = "10\"", PriceModifier = 0 });
            Sizes.Add(new FoodSize { Id = 2, Name = "14\"", PriceModifier = 5 });
            Sizes.Add(new FoodSize { Id = 3, Name = "16\"", PriceModifier = 10 });
            SelectedSize = Sizes.FirstOrDefault();
        }

        private void CalculateTotal()
        {
            decimal basePrice = FoodItem.Price;
            decimal modifier = SelectedSize?.PriceModifier ?? 0;
            TotalPrice = (basePrice + modifier) * Quantity;
        }

        [RelayCommand]
        private void IncreaseQuantity() => Quantity++;

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1) Quantity--;
        }

        [RelayCommand]
        private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task AddToCartAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Obtener el ID del usuario actual logueado en Supabase
                //var currentUser = _mediator; // O tu servicio de autenticación de Supabase
                //var userId = SupabaseClient.Auth.CurrentUser?.Id;
                var userId = await _mediator.Send(new GetCurrentUser(), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();


                if (string.IsNullOrEmpty(userId.Value.Id.ToString()))
                {
                    await Shell.Current.DisplayAlertAsync("Aviso", "Debes iniciar sesión para agregar productos al carrito.", "OK");
                    return;
                }

                // Calcular precio unitario incluyendo el modificador del tamaño seleccionado
                decimal finalPrice = FoodItem.Price + (SelectedSize?.PriceModifier ?? 0);

                var cartItem = new CartItem
                {
                    UserId = userId.Value.Id,
                    FoodId = FoodItem.Id,
                    Name = FoodItem.Name,
                    ImageUrl = FoodItem.ImageUrl,
                    SizeName = SelectedSize?.Name ?? "Standard",
                    Price = finalPrice,
                    Quantity = Quantity
                };

                var result = await _mediator.Send(new AddToCartCommand(cartItem), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlertAsync("Éxito", "¡Platillo agregado al carrito con éxito!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", result.ErrorMessage, "OK");
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }
    }
}