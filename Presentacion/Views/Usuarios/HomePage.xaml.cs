using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomeViewModel ViewModel => _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (!_viewModel.LoadDataCommand.IsRunning)
            {
                _ = _viewModel.LoadDataCommand.ExecuteAsync(null);
            }
        }
        catch (OperationCanceledException)
        {
            // La consulta fue cancelada.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error al cargar Home: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        _viewModel.LoadDataCommand.Cancel();
        base.OnDisappearing();
    }
}