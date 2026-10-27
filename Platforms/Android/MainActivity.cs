using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Graphics;
using AndroidNavigationBar = CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific.NavigationBar;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace FoodApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private MauiApplication? _application;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Window is { } window)
            {
                // Evitar que Android mezcle el color con su capa de contraste.
                if (OperatingSystem.IsAndroidVersionAtLeast(29))
                    window.NavigationBarContrastEnforced = false;

                if (!OperatingSystem.IsAndroidVersionAtLeast(35))
                    window.SetNavigationBarColor(Android.Graphics.Color.White);

                // LightContent: iconos claros en la barra de navegación.
                if (WindowCompat.GetInsetsController(window, window.DecorView) is { } controller)
                    controller.AppearanceLightNavigationBars = false;
            }

            _application = MauiApplication.Current;
            if (_application is not null)
                _application.PageAppearing += OnPageAppearing;

            var initialPage = Microsoft.Maui.Controls.Shell.Current?.CurrentPage
                ?? _application?.Windows.FirstOrDefault()?.Page;
            if (initialPage is not null)
                ApplyNavigationBarDefaults(initialPage);
        }

        private void OnPageAppearing(object? sender, Page page)
            => ApplyNavigationBarDefaults(page);

        private static void ApplyNavigationBarDefaults(Page page)
        {
            if (page is not ContentPage || !OperatingSystem.IsAndroidVersionAtLeast(23))
                return;

            // Las propiedades declaradas en la vista tienen prioridad.
            if (!page.IsSet(AndroidNavigationBar.ColorProperty))
                AndroidNavigationBar.SetColor(page, Colors.White);

            if (!page.IsSet(AndroidNavigationBar.StyleProperty))
                AndroidNavigationBar.SetStyle(page, NavigationBarStyle.LightContent);

            // Reaplicar al volver a una página cuyo handler ya existe.
            page.Handler?.UpdateValue(AndroidNavigationBar.ColorProperty.PropertyName);
            page.Handler?.UpdateValue(AndroidNavigationBar.StyleProperty.PropertyName);
        }

        protected override void OnDestroy()
        {
            if (_application is not null)
                _application.PageAppearing -= OnPageAppearing;

            base.OnDestroy();
        }
    }
}
