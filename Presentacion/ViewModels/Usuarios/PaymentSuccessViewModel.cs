using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Views.Usuarios;

namespace FoodApp.Presentacion.ViewModels.Usuarios;

public partial class PaymentSuccessViewModel : ObservableObject
{
    [RelayCommand]
    private async Task TrackOrderAsync()
    {
        // Navega a la vista del mapa de seguimiento
        await Shell.Current.GoToAsync(nameof(TrackingOrderPage));
    }
}
