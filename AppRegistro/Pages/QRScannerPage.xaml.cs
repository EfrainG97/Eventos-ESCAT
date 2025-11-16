using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AppRegistro.Pages
{
    public partial class QRScannerPage : ContentPage
    {
        private readonly AppRegistro.MainPage _mainPage;

        public QRScannerPage(AppRegistro.MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BarcodeReader.IsDetecting = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BarcodeReader.IsDetecting = false;
        }

        private void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
        {
            if (e.Results?.Any() == true)
            {
                var barcode = e.Results.First();
                var qrValue = barcode.Value;

                // Intentar parsear el valor del QR como identificador
                if (int.TryParse(qrValue, out int identificador))
                {
                    // Detener el escaneo
                    BarcodeReader.IsDetecting = false;

                    // Volver a MainPage y consultar el identificador
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        _mainPage.ConsultarQR(identificador);
                    });
                }
                else
                {
                    // Si no es un número válido, mostrar error
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Error", "El código QR no contiene un identificador válido", "Ok");
                    });
                }
            }
        }

        private async void CancelButton_Clicked(object? sender, EventArgs e)
        {
            BarcodeReader.IsDetecting = false;
            await Navigation.PopAsync();
        }
    }
}

