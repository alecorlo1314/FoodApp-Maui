using CommunityToolkit.Maui;
using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Infrastructure.Repositories;
using FoodApp.Infrastructure.Services;
using FoodApp.Presentacion.ViewModels;
using FoodApp.Presentacion.ViewModels.Autentificacion;
using FoodApp.Presentacion.ViewModels.Usuarios;
using FoodApp.Presentacion.Views.Autentificacion;
using FoodApp.Presentacion.Views.OnBoarding;
using FoodApp.Presentacion.Views.Usuarios;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace FoodApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureSyncfusionCore()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Sen-Regular.ttf", "SenRegular");
                    fonts.AddFont("Sen-Medium.ttf", "SenMedium");
                    fonts.AddFont("Sen-SemiBold.ttf", "SenSemiBold");
                    fonts.AddFont("Sen-Bold.ttf", "SenBold");
                    fonts.AddFont("Sen-ExtraBold.ttf", "SenExtraBold");
                });

            //Configurar y registrar Supabase como un Singleton
            var supabaseUrl = "https://hdtsshzstljffllzoqkb.supabase.co";
            var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImhkdHNzaHpzdGxqZmZsbHpvcWtiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3OTIyODk1NDMsImV4cCI6MjEwNzg2NTU0M30.XgYwew-r7WS2U5SZkCTt8hTAXL3KsDxkfnWakDIxd9I";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
                AutoRefreshToken = true,
                SessionHandler = new SupabaseSessionHandler()
            };

            var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);

            Task.Run(async () => {
                await supabaseClient.InitializeAsync();
            }).GetAwaiter().GetResult();

            // Lo registramos para poder inyectarlo en tus ViewModels o páginas
            builder.Services.AddSingleton(supabaseClient);

            // Registrar MediatR con todos los handlers del ensamblado
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MauiProgram).Assembly));

            //Regitrar vistas
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<OnboardingFavoritesPage>();
            builder.Services.AddTransient<OnboardingChefPage>();
            builder.Services.AddTransient<OnboardingDeliveryPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<VerificationPage>();
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<RestaurantDetailPage>();
            builder.Services.AddSingleton<FoodDetailPage>();
            builder.Services.AddSingleton<CartPage>();
            builder.Services.AddSingleton<AddCardPage>();
            builder.Services.AddSingleton<PaymentPage>();
            builder.Services.AddSingleton<PaymentSuccessPage>();
            builder.Services.AddSingleton<TrackingOrderPage>();

            // ViewModels
            builder.Services.AddTransient<OnboardingFavoritesViewModel>();
            builder.Services.AddTransient<OnboardingChefViewModel>();
            builder.Services.AddTransient<OnboardingDeliveryViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<VerificationViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<RestaurantDetailViewModel>();
            builder.Services.AddTransient<FoodDetailViewModel>();
            builder.Services.AddTransient<CartViewModel>();
            builder.Services.AddSingleton<AddCardViewModel>();
            builder.Services.AddSingleton<PaymentViewModel>();
            builder.Services.AddSingleton<PaymentSuccessViewModel>();
            builder.Services.AddSingleton<TrackingOrderViewModel>();

            // Repositorios
            builder.Services.AddTransient<IAuthRepository, SupabaseAuthRepository>();
            builder.Services.AddTransient<IUserRepository, SupabaseUserRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddTransient<IRestaurantRepository, RestaurantRepository>();
            builder.Services.AddTransient<IRestaurantCategoryRepository, RestaurantCategoryRepository>();
            builder.Services.AddTransient<IFoodRepository, FoodRepository>();
            builder.Services.AddTransient<ICartRepository, CartRepository>();
            builder.Services.AddTransient<IProductMetricsRepository, ProductMetricsRepository>();
            builder.Services.AddScoped<IUserPaymentRepository, UserPaymentRepository>();
            builder.Services.AddTransient<ILocationService, LocationService>();

            // Servicios
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddHttpClient<IPaymentGatewayService, StripePaymentService>(cliente =>
            {
                cliente.BaseAddress = new Uri("https://hdtsshzstljffllzoqkb.supabase.co/functions/v1/process-payment");
            })  .SetHandlerLifetime(TimeSpan.FromMinutes(1)) //Definir el tiempo de vida de la solicitud
                .AddPolicyHandler(GetRetryPolicy());

            static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) //reintentar cuando codigo sea 404
                    .WaitAndRetryAsync(3, //Haz hasta 3 reintentos.
                                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); //Calculo tiempo de 
            }

#if DEBUG
            builder.Logging.AddDebug(); 
#endif
            // CONFIGURACIÓN GLOBAL: Eliminar la línea inferior predeterminada del Entry
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("BorderlessEntry", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif __IOS__
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
            return builder.Build();
        }
    }
}
