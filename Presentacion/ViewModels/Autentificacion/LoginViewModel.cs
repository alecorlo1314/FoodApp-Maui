using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Authentication.Commands.Login;
using FoodApp.Presentacion.Views.Autentificacion;
using FoodApp.Presentacion.Views.Usuarios;
using MediatR;
using System.Text.RegularExpressions;

namespace FoodApp.Presentacion.ViewModels.Autentificacion;

public partial class LoginViewModel(IMediator mediator) : ObservableObject
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe;

    [ObservableProperty]
    private bool isPasswordHidden = true;

    [ObservableProperty]
    private string eyeIcon = "ic_eye_close.png";

    // Propiedades para el mensaje de error en rojo
    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
        EyeIcon = IsPasswordHidden ? "ic_eye_close.svg" : "ic_eye_open.svg";
    }

    [RelayCommand]
    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        ErrorMessage = string.Empty;
        HasError = false;

        // 1. Validar campos vacíos
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Todos los campos son obligatorios.");
            return;
        }

        // 2. Validar formato estricto de Gmail
        string gmailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
        if (!Regex.IsMatch(Email.Trim(), gmailPattern, RegexOptions.IgnoreCase))
        {
            ShowError("El correo debe ser una cuenta válida de Gmail (@gmail.com).");
            return;
        }

        // 3. Validar contraseña (mínimo 6 caracteres, mayúscula, minúscula y símbolo)
        string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$";
        if (!Regex.IsMatch(Password, passwordPattern))
        {
            ShowError("La contraseña debe tener al menos 6 caracteres, incluir mayúsculas, minúsculas y un símbolo/carácter especial.");
            return;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Enviamos el comando a través de MediatR
            var result = await mediator.Send(new LoginCommand(Email, Password), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage);
                return;
            }

            // Si es exitoso, navegamos a la pantalla principal (Home)
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        // Lógica para recuperar contraseña
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }

    [RelayCommand]
    private async Task SignUpAsync()
    {
        // Lógica para ir a la pantalla de registro
        await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
