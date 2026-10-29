using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class PersonalInfoPage
{
    private readonly PersonalInfoViewModel _viewModel;

    public PersonalInfoViewModel ViewModel => _viewModel;
    public PersonalInfoPage(PersonalInfoViewModel viewModel)
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
}
