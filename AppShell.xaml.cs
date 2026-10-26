using FoodApp.Presentacion.Views.Autentificacion;
using FoodApp.Presentacion.Views.OnBoarding;
using FoodApp.Presentacion.Views.Usuarios;

namespace FoodApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Registro de rutas para la navegación
            Routing.RegisterRoute(nameof(OnboardingChefPage),typeof(OnboardingChefPage));
            Routing.RegisterRoute(nameof(OnboardingDeliveryPage),typeof(OnboardingDeliveryPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage),typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(VerificationPage),typeof(VerificationPage));
            Routing.RegisterRoute(nameof(SignUpPage),typeof(SignUpPage));
            Routing.RegisterRoute(nameof(RestaurantDetailPage), typeof(RestaurantDetailPage));
            Routing.RegisterRoute(nameof(FoodDetailPage), typeof(FoodDetailPage));
            Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
            Routing.RegisterRoute(nameof(AddCardPage), typeof(AddCardPage));
            Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
            Routing.RegisterRoute(nameof(PaymentSuccessPage), typeof(PaymentSuccessPage));
            Routing.RegisterRoute(nameof(TrackingOrderPage), typeof(TrackingOrderPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            //Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            //Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        }
    }
}
