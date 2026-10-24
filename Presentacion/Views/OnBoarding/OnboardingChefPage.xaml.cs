using FoodApp.Presentacion.ViewModels;

namespace FoodApp.Presentacion.Views.OnBoarding;

public partial class OnboardingChefPage : ContentPage
{
    public OnboardingChefPage(OnboardingChefViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}