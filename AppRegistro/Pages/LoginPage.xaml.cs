using AppRegistro.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AppRegistro.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;

            // Suscribirse al evento de login exitoso
            ViewModel.LoginSuccess += OnLoginSuccess;
        }

        private void OnLoginSuccess(object? sender, LoginSuccessEventArgs e)
        {
            // Navegar a MainPage
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var mainPage = App.Current?.Handler?.MauiContext?.Services.GetService<MainPage>()
                    ?? App.Current?.Handler?.MauiContext?.Services.GetRequiredService<MainPage>();
                
                if (mainPage != null)
                {
                    Application.Current!.MainPage = new NavigationPage(mainPage);
                }
            });
        }
    }
}

