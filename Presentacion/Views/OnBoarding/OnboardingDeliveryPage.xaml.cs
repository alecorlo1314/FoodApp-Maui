using FoodApp.Presentacion.ViewModels;

namespace FoodApp.Presentacion.Views.OnBoarding;

public partial class OnboardingDeliveryPage : ContentPage
{
    public OnboardingDeliveryPage(OnboardingDeliveryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}