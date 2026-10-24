using FoodApp.Presentacion.ViewModels.Autentificacion;

namespace FoodApp.Presentacion.Views.Autentificacion;

public partial class SignUpPage : ContentPage
{
    private readonly SignUpViewModel _viewModel;

    public SignUpPage(SignUpViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        _viewModel.SignUpCommand.Cancel();
        base.OnDisappearing();
    }
}