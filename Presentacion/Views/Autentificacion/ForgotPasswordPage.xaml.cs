using FoodApp.Presentacion.ViewModels.Autentificacion;

namespace FoodApp.Presentacion.Views.Autentificacion;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}