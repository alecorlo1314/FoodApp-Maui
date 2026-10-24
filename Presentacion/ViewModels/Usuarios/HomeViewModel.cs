using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Cart.Queries.GetCountCart;
using FoodApp.Aplicacion.Features.Categories.Queries.GetCategories;
using FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants;
using FoodApp.Aplicacion.Features.Users.Queries.GetActiveUserProfile;
using FoodApp.Domain.Entities;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Collections.ObjectModel;
namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class HomeViewModel(IMediator _mediator) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotLoading))]
    private bool isLoading;

    public bool IsNotLoading => !IsLoading;

    [ObservableProperty]
    private string userName = "Cargando...";

    [ObservableProperty]
    private string userAddress = "La Cruz,Guancaste";

    [ObservableProperty]
    private int cartCount = 0;
    [ObservableProperty]
    private bool isCartCountVisible = false;

    [ObservableProperty]
    private string searchText = string.Empty;
    [ObservableProperty]
    private Category? selectedCategory;
    [ObservableProperty]
    private ObservableCollection<Category> categories = new();
    [ObservableProperty]
    private ObservableCollection<Restaurant> restaurants = new();

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
                LoadUserDataAsync(cancellationToken),
                LoadCategoriesAsync(cancellationToken),
                LoadRestaurantsAsync(cancellationToken),
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

    private async Task LoadUserDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // Solicitamos el nombre a través de MediatR
            var name = await _mediator.Send(new GetActiveUserProfileQuery(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            UserName = !string.IsNullOrEmpty(name) ? name : "Usuario";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            UserName = "Usuario";
        }
    }

    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Invocamos el Query mediante MediatR
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (result.IsSuccess && result.Value != null)
        {
            Categories.Clear();
            foreach (var category in result.Value)
            {
                Categories.Add(category);
            }
        }
    }

    private async Task LoadRestaurantsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Invocamos el Query mediante MediatR
        var result = await _mediator.Send(new GetRestaurantsQuery(), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        if (result.IsSuccess && result.Value != null)
        {
            Restaurants.Clear();
            foreach (var restaurant in result.Value)
            {
                Restaurants.Add(restaurant);
            }
        }
    }


    [RelayCommand]
    private void OpenMenu()
    {
        // Esto ordena de manera nativa a Shell que abra el menú lateral (Flyout)
        Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
    }

    [RelayCommand]
    private void OpenCart() => Shell.Current.GoToAsync(nameof(CartPage));

    [RelayCommand]
    private async Task SelectCategoryAsync(Category category)
    {
        SelectedCategory = category;

        if (category.Name.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            // Cargar todos los restaurantes sin filtrar
            //await LoadAllRestaurantsAsync();
        }
        else
        {
            // Filtrar restaurantes por la categoría seleccionada
            //await FilterRestaurantsByCategoryAsync(category.Id);
        }
    }

    [RelayCommand]
    private void SeeAllRestaurants() { }


    [RelayCommand]
    private async Task SelectRestaurantAsync(Restaurant restaurant)
    {
        if (restaurant is null)
            return;

        var navigationParameter = new Dictionary<string, object>
    {
        { "Restaurant", restaurant }
    };

        await Shell.Current.GoToAsync(
            nameof(RestaurantDetailPage),
            navigationParameter);
    }

}
