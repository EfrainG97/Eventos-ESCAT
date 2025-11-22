using System.Windows.Input;

namespace AppRegistro.ViewModels
{
    public class QRScannerPageViewModel : BaseViewModel
    {
        private bool _isDetecting = true;

        public QRScannerPageViewModel()
        {
            Title = "Escanear Código QR";
            CancelCommand = new Command(async () => await ExecuteCancelAsync());
        }

        public bool IsDetecting
        {
            get => _isDetecting;
            set => SetProperty(ref _isDetecting, value);
        }

        public ICommand CancelCommand { get; }

        public event EventHandler<int>? QRCodeDetected;
        public event EventHandler? CancelScanning;

        public void OnBarcodeDetected(string qrValue)
        {
            // Intentar parsear el valor del QR como identificador
            if (int.TryParse(qrValue, out int identificador))
            {
                // Detener el escaneo
                IsDetecting = false;

                // Notificar que se detectó un código QR válido
                QRCodeDetected?.Invoke(this, identificador);
            }
            else
            {
                // Si no es un número válido, notificar error
                InvalidQRCodeDetected?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? InvalidQRCodeDetected;

        private async Task ExecuteCancelAsync()
        {
            IsDetecting = false;
            CancelScanning?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }
    }
}

