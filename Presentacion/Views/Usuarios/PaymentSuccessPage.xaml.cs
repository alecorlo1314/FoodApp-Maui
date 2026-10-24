using FoodApp.Presentacion.ViewModels.Usuarios;

namespace FoodApp.Presentacion.Views.Usuarios;

public partial class PaymentSuccessPage : ContentPage
{
	public PaymentSuccessPage(PaymentSuccessViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
}