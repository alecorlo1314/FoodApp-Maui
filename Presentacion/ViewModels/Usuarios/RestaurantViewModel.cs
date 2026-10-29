using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class RestaurantViewModel : ObservableObject
{
    [ObservableProperty]
    private string restaurantName = string.Empty;

    [ObservableProperty]
    private decimal rating;

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");
}