using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FoodApp.Presentacion.ViewModels;

public partial class OnboardingDeliveryViewModel : ObservableObject
{
    [RelayCommand]
    private async Task NextAsync()
    {
        // Navega a la pantalla principal o de inicio de sesión de tu aplicación
        // Ej: await Shell.Current.GoToAsync("//MainPage");
    }

    // Nota: La pantalla de Delivery solo tiene el botón "GET STARTED",
    // pero si mantienes Skip por coherencia puedes dejarlo sin lógica.
    [RelayCommand]
    private void Skip()
    {
        // Sin lógica por el momento
    }
}