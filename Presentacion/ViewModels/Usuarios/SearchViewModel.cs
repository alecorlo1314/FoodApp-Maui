using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Cart.Queries.GetCountCart;
using FoodApp.Presentacion.Models;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class SearchViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private int cartCount = 0;
    [ObservableProperty]
    private bool isCartCountVisible = false;

    // Controla la visibilidad de la sección de recientes
    [ObservableProperty]
    private bool hasRecentKeywords;

    [ObservableProperty]
    private ObservableCollection<string> recentKeywords = new();

    [ObservableProperty]
    private ObservableCollection<RestaurantModel> suggestedRestaurants = new();

    [ObservableProperty]
    private ObservableCollection<PopularFoodModel> popularFastFoods = new();

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
                LoadRecentKeywords(cancellationToken),
                LoadSuggestedRestaurantsAsync(cancellationToken),
                LoadPopularFastFoodsAsync(cancellationToken),
                LoadCountCartAsync(cancellationToken)
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

    private async Task LoadRecentKeywords(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Ejemplo simulado / o desde SecureStorage / Base de datos local
        var savedKeywords = new List<string> { "Burger", "Sandwich", "Pizza" };

        RecentKeywords.Clear();
        foreach (var kw in savedKeywords)
        {
            RecentKeywords.Add(kw);
        }

        HasRecentKeywords = RecentKeywords.Any();
    }

    [RelayCommand]
    private async Task SearchKeywordAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return;

        SearchQuery = keyword;

        // Agregar a recientes (manteniendo máximo 5 y sin duplicados)
        if (!RecentKeywords.Contains(keyword))
        {
            RecentKeywords.Insert(0, keyword);
            if (RecentKeywords.Count > 5) RecentKeywords.RemoveAt(5);
        }
        HasRecentKeywords = true;

        // Ejecutar lógica de búsqueda real...
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    private async Task LoadSuggestedRestaurantsAsync(CancellationToken cancellationToken = default)
    {
        // Consulta simulada ordenada por Calificación (Descending)
        // En tu backend: .OrderByDescending(r => r.Rating)
        cancellationToken.ThrowIfCancellationRequested();
        var list = new List<RestaurantModel>
        {
            new() { Name = "Pansi Restaurant", Rating = 4.7m },
            new() { Name = "American Spicy Burger Shop", Rating = 4.3m },
            new() { Name = "Cafenio Coffee Club", Rating = 4.0m }
        };

        SuggestedRestaurants = new ObservableCollection<RestaurantModel>(list.OrderByDescending(r => r.Rating));
    }

    private async Task LoadPopularFastFoodsAsync(CancellationToken cancellationToken = default)
    {
        // Consulta a la BD basada en la cantidad total de pedidos acumulados
        // En tu backend: .OrderByDescending(p => p.TotalOrderedCount)
        cancellationToken.ThrowIfCancellationRequested();
        var list = new List<PopularFoodModel>
        {
            new() { Name = "European Pizza", RestaurantName = "Uttora Coffe House", TotalOrders = 150 },
            new() { Name = "Buffalo Pizza.", RestaurantName = "Cafenio Coffee Club", TotalOrders = 95 },
            new() { Name = "Burger", RestaurantName = "Pansi Restaurant", TotalOrders = 80 },
            new() { Name = "Burger", RestaurantName = "Cafenio Coffee Club", TotalOrders = 60 }
        };

        PopularFastFoods = new ObservableCollection<PopularFoodModel>(list.OrderByDescending(f => f.TotalOrders));
    }

    private async Task LoadCountCartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var cantidadCarrito = await _mediator.Send(new GetCountCartQuery(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            CartCount = cantidadCarrito.Value;
            IsCartCountVisible = CartCount > 0;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Error al obtener la cantidad de productos en el carrito", "OK");
        }
    }

    [RelayCommand]
    private async Task OpenCartAsync() => await Shell.Current.GoToAsync(nameof(CartPage));
}