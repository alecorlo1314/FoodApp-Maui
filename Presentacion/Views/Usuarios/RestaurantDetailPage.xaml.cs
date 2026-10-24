using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class RestaurantDetailPage : ContentPage
{
    private readonly RestaurantDetailViewModel _viewModel;

    public RestaurantDetailViewModel ViewModel => _viewModel;

    public RestaurantDetailPage(RestaurantDetailViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
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
        _viewModel.SelectCategoryCommand.Cancel();
        _viewModel.AddToCartCommand.Cancel();
        base.OnDisappearing();
    }
}
