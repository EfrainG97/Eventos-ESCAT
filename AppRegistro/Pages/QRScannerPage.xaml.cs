using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using AppRegistro.ViewModels;

namespace AppRegistro.Pages
{
    public partial class QRScannerPage : ContentPage
    {
        public QRScannerPageViewModel ViewModel { get; }
        private readonly MainPageViewModel? _mainPageViewModel;

        public QRScannerPage(QRScannerPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;

            // Suscribirse a eventos
            ViewModel.QRCodeDetected += OnQRCodeDetected;
            ViewModel.CancelScanning += OnCancelScanning;
            ViewModel.InvalidQRCodeDetected += OnInvalidQRCodeDetected;
        }

        public QRScannerPage(MainPageViewModel mainPageViewModel) 
            : this(new QRScannerPageViewModel())
        {
            _mainPageViewModel = mainPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.IsDetecting = true;
            BarcodeReader.IsDetecting = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.IsDetecting = false;
            BarcodeReader.IsDetecting = false;
        }

        private void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            if (e.Results?.Any() == true)
            {
                var barcode = e.Results.First();
                var qrValue = barcode.Value;

                // Notificar al ViewModel
                ViewModel.OnBarcodeDetected(qrValue);
            }
        }

        private async void OnQRCodeDetected(object? sender, int identificador)
        {
            // Volver a MainPage y consultar el identificador
            await Navigation.PopAsync();
            
            if (_mainPageViewModel != null)
            {
                await _mainPageViewModel.ConsultarPorIdentificadorAsync(identificador);
            }
        }

        private async void OnInvalidQRCodeDetected(object? sender, EventArgs e)
        {
            await DisplayAlert("Error", "El código QR no contiene un identificador válido", "Ok");
        }

        private async void OnCancelScanning(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

