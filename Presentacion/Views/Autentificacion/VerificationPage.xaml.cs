using FoodApp.Presentacion.ViewModels.Autentificacion;

namespace FoodApp.Presentacion.Views.Autentificacion;

public partial class VerificationPage : ContentPage
{
    public VerificationPage(VerificationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}