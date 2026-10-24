using FoodApp.Presentacion.ViewModels.Autentificacion;

namespace FoodApp.Presentacion.Views.Autentificacion;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        _viewModel.LoginCommand.Cancel();
        base.OnDisappearing();
    }
}