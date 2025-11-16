using AppRegistro.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppRegistro.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly IApiService _apiService;
        private readonly ISecureStorageService _secureStorageService;

        public LoginPage(IApiService apiService, ISecureStorageService secureStorageService)
        {
            InitializeComponent();
            _apiService = apiService;
            _secureStorageService = secureStorageService;

            // Cargar configuración guardada si existe
            LoadSavedConfig();
        }

        private async void LoadSavedConfig()
        {
            var (ip, port) = await _secureStorageService.GetApiConfigAsync();
            if (!string.IsNullOrEmpty(ip))
                IpEntry.Text = ip;
            if (!string.IsNullOrEmpty(port))
                PortEntry.Text = port;
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            // Ocultar mensaje de error anterior
            ErrorMessageLabel.IsVisible = false;
            ErrorMessageText.Text = "";

            // Validar campos
            if (string.IsNullOrWhiteSpace(IpEntry.Text))
            {
                ShowError("Por favor ingresa la IP del servidor");
                return;
            }

            if (string.IsNullOrWhiteSpace(PortEntry.Text))
            {
                ShowError("Por favor ingresa el puerto");
                return;
            }

            if (string.IsNullOrWhiteSpace(UserEntry.Text))
            {
                ShowError("Por favor ingresa tu usuario");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                ShowError("Por favor ingresa tu contraseña");
                return;
            }

            // Mostrar indicador de carga
            LoginButton.IsEnabled = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                // Realizar login
                var response = await _apiService.LoginAsync(
                    IpEntry.Text.Trim(),
                    PortEntry.Text.Trim(),
                    UserEntry.Text.Trim(),
                    PasswordEntry.Text
                );

                if (response != null && response.Success && !string.IsNullOrEmpty(response.Token))
                {
                    // Guardar token y configuración
                    await _secureStorageService.SaveTokenAsync(response.Token);
                    await _secureStorageService.SaveApiConfigAsync(
                        IpEntry.Text.Trim(),
                        PortEntry.Text.Trim()
                    );

                    // Navegar a MainPage
                    var mainPage = App.Current?.Handler?.MauiContext?.Services.GetService<MainPage>()
                        ?? new MainPage(_apiService);
                    
                    Application.Current!.MainPage = new NavigationPage(mainPage);
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
                // Ocultar indicador de carga
                LoginButton.IsEnabled = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private void ShowError(string message)
        {
            ErrorMessageText.Text = message;
            ErrorMessageLabel.IsVisible = true;
        }
    }
}

