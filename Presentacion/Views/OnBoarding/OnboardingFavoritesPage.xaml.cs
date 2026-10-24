using FoodApp.Presentacion.ViewModels;

namespace FoodApp.Presentacion.Views.OnBoarding;

public partial class OnboardingFavoritesPage : ContentPage
{
    public OnboardingFavoritesPage(OnboardingFavoritesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}