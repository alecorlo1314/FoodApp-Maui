using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class PersonalInfoPage
{
    public PersonalInfoPage(PersonalInfoViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
