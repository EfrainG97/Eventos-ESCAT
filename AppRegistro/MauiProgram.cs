using Microsoft.Extensions.Logging;
using AppRegistro.Services;
using AppRegistro.Pages;
using AppRegistro.ViewModels;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;

namespace AppRegistro
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
            builder.Services.AddSingleton<IApiService>(sp => 
                new ApiService(sp.GetRequiredService<ISecureStorageService>()));
            
            // Registrar ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<QRScannerPageViewModel>();
            
            // Registrar páginas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<QRScannerPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
