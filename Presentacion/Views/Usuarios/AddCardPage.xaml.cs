using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class AddCardPage : ContentPage
{
    private readonly AddCardViewModel _viewModel;

	public AddCardPage(AddCardViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    protected override void OnDisappearing()
    {
        _viewModel.SaveAndMakePaymentCommand.Cancel();
        base.OnDisappearing();
    }
}