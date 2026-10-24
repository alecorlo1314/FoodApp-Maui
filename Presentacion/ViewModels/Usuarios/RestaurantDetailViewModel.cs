using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Cart.Commands.AddToCart;
using FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;
using FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories;
using FoodApp.Domain.Entities;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Collections.ObjectModel;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

[QueryProperty(nameof(Restaurant), "Restaurant")]
public partial class RestaurantDetailViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    private Restaurant restaurant = new();

    [ObservableProperty]
    private ObservableCollection<RestaurantCategory> categories = new();

    [ObservableProperty]
    private ObservableCollection<FoodItem> foods = new();

    [ObservableProperty]
    private RestaurantCategory? selectedCategory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool isLoading;
    public bool IsNotLoading => !IsLoading;

    
    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (IsLoading || Restaurant is null || Restaurant.Id <= 0)
            return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsLoading = true;
            await LoadRestaurantDataAsync(Restaurant.Id, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadRestaurantDataAsync(long restaurantId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // 1. Obtener categorías
            var categoriesResult = await _mediator.Send(
                new GetRestaurantCategoriesQuery(restaurantId), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            Categories.Clear();
            Foods.Clear();

            if (!categoriesResult.IsSuccess ||
                categoriesResult.Value is null)
                return;

            foreach (var category in categoriesResult.Value)
            {
                Categories.Add(category);
            }

            // 2. Seleccionar categoría inicial
            var firstCategory = Categories.FirstOrDefault();

            SelectedCategory = firstCategory;

            // 3. Cargar alimentos de la primera categoría
            if (firstCategory is not null)
            {
                var foodsResult = await _mediator.Send(
                    new GetFoodsByCategoryQuery(
                        restaurantId,
                        firstCategory.Id), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (foodsResult.IsSuccess &&
                    foodsResult.Value is not null)
                {
                    foreach (var food in foodsResult.Value)
                    {
                        Foods.Add(food);
                    }
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }


    [RelayCommand]
    private async Task SelectCategoryAsync(
        RestaurantCategory category, CancellationToken cancellationToken)
    {
        if (category is null || IsLoading)
            return;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            IsLoading = true;
            SelectedCategory = category;

            var result = await _mediator.Send(
                new GetFoodsByCategoryQuery(
                    Restaurant.Id,
                    category.Id), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            Foods.Clear();

            if (result.IsSuccess && result.Value is not null)
            {
                foreach (var food in result.Value)
                {
                    Foods.Add(food);
                }
            }
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
    private async Task NavigateToFoodDetailAsync(FoodItem foodItem)
    {
        if (foodItem == null) return;

        var navigationParameter = new Dictionary<string, object>
    {
        { "FoodItem", foodItem }
    };

        // Asegúrate de haber registrado esta ruta en tu AppShell.xaml.cs (Routing.RegisterRoute)
        await Shell.Current.GoToAsync(nameof(FoodDetailPage), navigationParameter);
    }

    [RelayCommand]
    private async Task AddToCartAsync(FoodItem foodItem, CancellationToken cancellationToken)
    {
        if(foodItem == null) return;

        var cartItem = new CartItem
        {
            FoodId = foodItem.Id,
            Name = foodItem.Name,
            ImageUrl = foodItem.ImageUrl,
            SizeName = "Standard",
            Price = foodItem.Price,
            Quantity = 1
        };

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _mediator.Send(new AddToCartCommand(cartItem), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (result.IsSuccess)
            {
                // Notificación visual rápida o toast
                await Shell.Current.DisplayAlertAsync("Carrito", $"¡{foodItem.Name} agregado al carrito!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Aviso", result.ErrorMessage, "OK");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Aviso", "Ocurrió un error al agregar el producto al carrito.", "OK");
        }
    }


    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void MoreOptions()
    {
        // Lógica para el botón de los tres puntos superiores
    }
}