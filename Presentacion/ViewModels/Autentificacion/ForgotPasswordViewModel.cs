using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Presentacion.Views.Autentificacion;

namespace FoodApp.Presentacion.ViewModels.Autentificacion;

public partial class ForgotPasswordViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SendCodeAsync()
    {
        // Lógica para enviar el código de recuperación

        //Dirigir a la pantalla de verificación
        await Shell.Current.GoToAsync(nameof(VerificationPage), animate: true);
    }
}
