using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class PaymentPage : ContentPage
{
    private readonly PaymentViewModel _viewModel;
    public PaymentPage(PaymentViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_viewModel.LoadDataCommand.IsRunning)
            _ = _viewModel.LoadDataCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        _viewModel.LoadDataCommand.Cancel();
        _viewModel.PayAndConfirmCommand.Cancel();
        base.OnDisappearing();
    }
}
