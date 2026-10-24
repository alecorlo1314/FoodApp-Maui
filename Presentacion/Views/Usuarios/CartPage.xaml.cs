using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class CartPage : ContentPage
{
    private readonly CartViewModel _viewModel;
    public CartViewModel ViewModel => _viewModel;

    public CartPage(CartViewModel viewModel)
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
        {
            _ = _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
            
    }

    protected override void OnDisappearing()
    {
        _viewModel.LoadDataCommand.Cancel();
        _viewModel.GetCurrentLocationCommand.Cancel();
        _viewModel.IncreaseQuantityCommand.Cancel();
        _viewModel.DecreaseQuantityCommand.Cancel();
        _viewModel.RemoveItemCommand.Cancel();
        base.OnDisappearing();
    }
}