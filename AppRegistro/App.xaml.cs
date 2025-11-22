using Microsoft.Extensions.DependencyInjection;
using AppRegistro.Pages;

namespace AppRegistro
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Obtener LoginPage del contenedor de servicios
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            
            return new Window(loginPage);
        }
    }
}