using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Views.OnBoarding;

namespace FoodApp.Presentacion.ViewModels;

public partial class OnboardingChefViewModel : ObservableObject
{
    [RelayCommand]
    private async Task NextAsync()
    {
        // Navega a la siguiente pantalla (Delivery)
        await Shell.Current.GoToAsync(nameof(OnboardingDeliveryPage), animate: true);
    }

    [RelayCommand]
    private void Skip()
    {
        // Sin lógica por el momento
    }
}
