using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _viewModel;

    public SearchViewModel ViewModel => _viewModel;
    public SearchPage(SearchViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        _viewModel = viewModel;
        BindingContext = _viewModel;

        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (!_viewModel.LoadDataCommand.IsRunning)
            {
                await _viewModel.LoadDataCommand.ExecuteAsync(null);
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelación de la consulta.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }


    protected override void OnDisappearing()
    {
        _viewModel.LoadDataCommand.Cancel();
        base.OnDisappearing();
    }
}