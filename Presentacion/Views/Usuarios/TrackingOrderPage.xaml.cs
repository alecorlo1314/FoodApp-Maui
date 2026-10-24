using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class TrackingOrderPage : ContentPage
{
    private readonly TrackingOrderViewModel _viewModel;
	public TrackingOrderPage(TrackingOrderViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = _viewModel = viewModel;
	}

    private void TrackingBottomSheet_StateChanged(object sender, Syncfusion.Maui.Toolkit.BottomSheet.StateChangedEventArgs e)
    {

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
        base.OnDisappearing();
    }
}