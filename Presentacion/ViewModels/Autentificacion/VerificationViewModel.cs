using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FoodApp.Presentacion.ViewModels.Autentificacion;

[QueryProperty(nameof(Email), "email")]
public partial class VerificationViewModel : ObservableObject
{
    [ObservableProperty]
    private string email = "example@gmail.com";

    [ObservableProperty]
    private string code1 = string.Empty;

    [ObservableProperty]
    private string code2 = string.Empty;

    [ObservableProperty]
    private string code3 = string.Empty;

    [ObservableProperty]
    private string code4 = string.Empty;

    [ObservableProperty]
    private string timerText = "in.50sec";

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void ResendCode()
    {
        // Lógica para reenviar el código
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        string fullCode = $"{Code1}{Code2}{Code3}{Code4}";
        // Lógica de validación del código
    }
}