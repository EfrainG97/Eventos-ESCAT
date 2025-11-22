using AppRegistro.Services;
using AppRegistro.Models;
using System.Windows.Input;

namespace AppRegistro.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly ISecureStorageService _secureStorageService;

        private string _ip = string.Empty;
        private string _port = string.Empty;
        private string _user = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isErrorMessageVisible = false;
        private bool _isLoading = false;

        public LoginViewModel(IApiService apiService, ISecureStorageService secureStorageService)
        {
            _apiService = apiService;
            _secureStorageService = secureStorageService;
            LoginCommand = new Command(async () => await ExecuteLoginAsync(), () => !IsBusy);
            LoadSavedConfigCommand = new Command(async () => await LoadSavedConfigAsync());
            
            Title = "Iniciar Sesión";
            
            // Cargar configuración guardada al inicializar
            _ = LoadSavedConfigAsync();
        }

        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsErrorMessageVisible
        {
            get => _isErrorMessageVisible;
            set => SetProperty(ref _isErrorMessageVisible, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                IsBusy = value;
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand LoadSavedConfigCommand { get; }

        public event EventHandler<LoginSuccessEventArgs>? LoginSuccess;

        private async Task LoadSavedConfigAsync()
        {
            try
            {
                var (ip, port) = await _secureStorageService.GetApiConfigAsync();
                if (!string.IsNullOrEmpty(ip))
                    Ip = ip;
                if (!string.IsNullOrEmpty(port))
                    Port = port;
            }
            catch (Exception ex)
            {
                // Log error si es necesario
                System.Diagnostics.Debug.WriteLine($"Error loading saved config: {ex.Message}");
            }
        }

        private async Task ExecuteLoginAsync()
        {
            // Ocultar mensaje de error anterior
            IsErrorMessageVisible = false;
            ErrorMessage = string.Empty;

            // Validar campos
            if (string.IsNullOrWhiteSpace(Ip))
            {
                ShowError("Por favor ingresa la IP del servidor");
                return;
            }

            if (string.IsNullOrWhiteSpace(Port))
            {
                ShowError("Por favor ingresa el puerto");
                return;
            }

            if (string.IsNullOrWhiteSpace(User))
            {
                ShowError("Por favor ingresa tu usuario");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ShowError("Por favor ingresa tu contraseña");
                return;
            }

            IsLoading = true;

            try
            {
                // Realizar login
                var response = await _apiService.LoginAsync(
                    Ip.Trim(),
                    Port.Trim(),
                    User.Trim(),
                    Password
                );

                if (response != null && response.Success && !string.IsNullOrEmpty(response.Token))
                {
                    // Guardar token y configuración
                    await _secureStorageService.SaveTokenAsync(response.Token);
                    await _secureStorageService.SaveApiConfigAsync(
                        Ip.Trim(),
                        Port.Trim()
                    );

                    // Notificar éxito
                    LoginSuccess?.Invoke(this, new LoginSuccessEventArgs());
                }
                else
                {
                    var errorMessage = response?.Message ?? "Error desconocido al iniciar sesión";
                    ShowError(errorMessage);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            IsErrorMessageVisible = true;
        }
    }

    public class LoginSuccessEventArgs : EventArgs
    {
    }
}

