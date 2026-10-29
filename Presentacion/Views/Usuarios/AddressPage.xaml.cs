using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class AddressPage
{
    private readonly AddressViewModel _viewModel;
    public AddressViewModel ViewModel => _viewModel;
    public AddressPage(AddressViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }
}
