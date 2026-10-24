using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodApp.Aplicacion.Features.Authentication.Commands.Register;
using FoodApp.Presentacion.Views.Autentificacion;
using MediatR;
using System.Text.RegularExpressions;

namespace FoodApp.Presentacion.ViewModels.Autentificacion;

public partial class SignUpViewModel(IMediator mediator) : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    // Propiedades para la visibilidad de la contraseña
    [ObservableProperty]
    private bool isPasswordHidden = true;

    [ObservableProperty]
    private string eyeIcon = "ic_eye_close.png";

    [ObservableProperty]
    private bool isConfirmPasswordHidden = true;

    [ObservableProperty]
    private string confirmEyeIcon = "ic_eye_close.png";

    // Propiedades para manejar el mensaje de error en rojo
    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
        EyeIcon = IsPasswordHidden ? "ic_eye_close.png" : "ic_eye_open.png";
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility()
    {
        IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
        ConfirmEyeIcon = IsConfirmPasswordHidden ? "ic_eye_close.png" : "ic_eye_open.png";
    }

    [RelayCommand]
    private async Task SignUpAsync(CancellationToken cancellationToken)
    {
        // Limpiamos errores previos
        ErrorMessage = string.Empty;
        HasError = false;

        // 1. Validar campos vacíos
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ShowError("Todos los campos son obligatorios.");
            return;
        }

        // 2. Validar que el nombre tenga al menos 3 caracteres
        if (Name.Trim().Length < 3)
        {
            ShowError("El nombre debe contener al menos 3 caracteres.");
            return;
        }

        // 3. Validar expresión regular para Gmail estricto
        // Permite letras, números, puntos y debe terminar exactamente en @gmail.com
        string gmailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
        if (!Regex.IsMatch(Email.Trim(), gmailPattern, RegexOptions.IgnoreCase))
        {
            ShowError("El correo debe ser una cuenta válida de Gmail (@gmail.com).");
            return;
        }

        // 4. Validar contraseña: Al menos 6 caracteres, una mayúscula, una minúscula y un símbolo/número
        // Expresión regular: (?=.*[a-z]) (minúscula), (?=.*[A-Z]) (mayúscula), (?=.*[\W_] or number) (símbolo o número), mínimo 6 chars.
        string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$";
        if (!Regex.IsMatch(Password, passwordPattern))
        {
            ShowError("La contraseña debe tener al menos 6 caracteres, incluir mayúsculas, minúsculas y un símbolo/carácter especial.");
            return;
        }

        // 5. Validar que ambas contraseñas coincidan
        if (Password != ConfirmPassword)
        {
            ShowError("Las contraseñas no coinciden.");
            return;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Enviamos a través de MediatR
            var result = await mediator.Send(new RegisterCommand(Name, Email, Password), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (!result.IsSuccess)
            {
                // Mostramos el mensaje limpio devuelto por la capa de infraestructura/dominio
                ErrorMessage = result.ErrorMessage;
                HasError = true;
                return;
            }

            // Si fue exitoso, navegamos al Home o a donde corresponda
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }
}