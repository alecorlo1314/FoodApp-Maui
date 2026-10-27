using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
using FoodApp.Presentacion.Views.Autentificacion;
using FoodApp.Presentacion.Views.OnBoarding;
using FoodApp.Presentacion.Views.Usuarios;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell;

using AndroidNavigationBar = CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific.NavigationBar;
namespace FoodApp;

public partial class AppShell : SimpleShell
{
    private bool _isMenuOpen = false;
    private bool _isAnimating;
    private readonly Color _menuBarColor = Color.FromArgb("#FF7622");

    private readonly Color _normalBarColor = Colors.White;

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

    public Task ToggleMenuAsync()
    {
        // Abre o cierra el menu
        return SetMenuOpenAsync(!_isMenuOpen);
    }

    public Task CloseMenuAsync()
    {
        return SetMenuOpenAsync(false);
    }

    //Configuración del menu
    private async Task SetMenuOpenAsync(bool open)
    {
        //Si ya se esta animando o el menu esta abierto o cerrado, no hacer nada
        if (_isAnimating || _isMenuOpen == open)
            return;

        // Marca el menu como animando
        _isAnimating = true;

        try
        {
            //Duración de la animación
            const uint duration = 500;

            //Cuando el menu se abre
            if (open)
            {
                //Vas a redonde las esquinas con 30 de radio
                MainSurface.StrokeShape =
                    new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(30)
                    };
            }

            //cuando este abierto el la vista sera menor escala
            var scale = open ? 0.72 : 1.0;

            //cuando este abierto el la vista se movera a la derecha
            var x = open ? MainSurface.Width * 0.38 : 0;
            var y = open ? MainSurface.Height * 0.06 : 0;

            // Animar escala y posición al mismo tiempo.
            await Task.WhenAll(
                MainSurface.ScaleToAsync(
                    scale, duration, Easing.CubicInOut),

                MainSurface.TranslateToAsync(
                    x, y, duration, Easing.CubicInOut)
            );

            _isMenuOpen = open;

            UpdateSystemBars();
            // Cuando el menú está abierto,
            // tocar la vista previa lo cierra.
            PreviewOverlay.IsVisible = open;

            if (!open)
            {
                MainSurface.StrokeShape =
                    new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(0)
                    };
            }
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async void CloseMenuClicked(
        object sender, EventArgs e)
    {
        await CloseMenuAsync();
    }

    private async void PreviewTapped(
        object sender, TappedEventArgs e)
    {
        await CloseMenuAsync();
    }

    private async void MenuItemClicked(
        object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        string option =
            button.CommandParameter?.ToString() ?? "";

        await CloseMenuAsync();

        // De momento, las rutas del menú son ilustrativas.
        // Aquí conectarás cada opción a su pantalla.
        await DisplayAlertAsync(
            "FoodApp",
            $"Seleccionaste: {option}",
            "OK");
    }

    private void UpdateSystemBars()
    {
        Color barColor = _isMenuOpen
            ? _menuBarColor
            : _normalBarColor;

        StatusBarStyle statusStyle = _isMenuOpen
            ? StatusBarStyle.LightContent
            : StatusBarStyle.DarkContent;

        NavigationBarStyle navigationStyle = _isMenuOpen
            ? NavigationBarStyle.LightContent
            : NavigationBarStyle.DarkContent;

        // STATUS BAR
        StatusBar.SetColor(barColor);
        StatusBar.SetStyle(statusStyle);

#if ANDROID 
        // NAVIGATION BAR
        var currentPage = Shell.Current.CurrentPage;

        if (currentPage != null)
        {
            AndroidNavigationBar.SetColor(currentPage, barColor);
            AndroidNavigationBar.SetStyle(currentPage, navigationStyle);
        }
#endif
    }
}
