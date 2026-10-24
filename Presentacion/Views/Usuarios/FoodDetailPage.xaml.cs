using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class FoodDetailPage : ContentPage
{
    private readonly FoodDetailViewModel _viewModel;

    public FoodDetailPage(FoodDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
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
        _viewModel.AddToCartCommand.Cancel();
        base.OnDisappearing();
    }
}
