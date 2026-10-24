using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Views.OnBoarding;

namespace FoodApp.Presentacion.ViewModels;

public partial class OnboardingFavoritesViewModel : ObservableObject
{
    [RelayCommand]
    private async Task Next()
    {
        // Navega a la siguiente pantalla (Chef)
        await Shell.Current.GoToAsync(nameof(OnboardingChefPage), animate: true);
    }

    [RelayCommand]
    private void Skip()
    {
        // Sin lógica por el momento
    }
}
