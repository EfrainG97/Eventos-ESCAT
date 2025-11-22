using AppRegistro.ViewModels;
using AppRegistro.Pages;

namespace AppRegistro
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel { get; }

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;

            // Suscribirse a eventos
            ViewModel.ShowAlert += OnShowAlert;
            ViewModel.NavigateToQRScanner += OnNavigateToQRScanner;
        }

        private async void OnShowAlert(object? sender, string message)
        {
            var parts = message.Split('|');
            var title = parts.Length > 1 ? parts[0] : "Información";
            var body = parts.Length > 1 ? parts[1] : parts[0];
            
            await DisplayAlert(title, body, "Ok");
        }

        private async void OnNavigateToQRScanner(object? sender, EventArgs e)
        {
            var qrScannerPage = App.Current?.Handler?.MauiContext?.Services.GetService<QRScannerPage>()
                ?? new QRScannerPage(ViewModel);
            await Navigation.PushAsync(qrScannerPage);
        }

        public async Task ConsultarQR(int identificador)
        {
            await ViewModel.ConsultarPorIdentificadorAsync(identificador);
        }
    }
}
