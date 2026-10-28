using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class AddressPage : ContentPage
{
    public AddressViewModel ViewModel { get; }

    public AddressPage(AddressViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        BindingContext = viewModel;
        InitializeComponent();
    }
}
