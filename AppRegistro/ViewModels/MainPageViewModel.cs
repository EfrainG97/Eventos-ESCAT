using AppRegistro.Models;
using AppRegistro.Services;
using System.Windows.Input;

namespace AppRegistro.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        private string _identificadorEntry = string.Empty;
        private string _idAlumno = string.Empty;
        private string _identificador = string.Empty;
        private string _nombre = string.Empty;
        private string _apellidos = string.Empty;
        private string _modalidad = string.Empty;
        private bool _isCheckM = false;
        private bool _isCheckJ = false;
        private bool _isCheckConf = false;
        private bool _isCheckMEnabled = true;
        private bool _isCheckJEnabled = true;
        private bool _isCheckConfEnabled = true;

        public MainPageViewModel(IApiService apiService)
        {
            _apiService = apiService;
            BuscarCommand = new Command(async () => await ExecuteBuscarAsync(), () => !IsBusy);
            GuardarAsistenciaCommand = new Command(async () => await ExecuteGuardarAsistenciaAsync(), () => !IsBusy);
            EscanearQRCommand = new Command(async () => await ExecuteEscanearQRAsync(), () => !IsBusy);
            
            Title = "Registro de Asistencia";
        }

        public string IdentificadorEntry
        {
            get => _identificadorEntry;
            set => SetProperty(ref _identificadorEntry, value);
        }

        public string IDAlumno
        {
            get => _idAlumno;
            set => SetProperty(ref _idAlumno, value);
        }

        public string Identificador
        {
            get => _identificador;
            set => SetProperty(ref _identificador, value);
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Apellidos
        {
            get => _apellidos;
            set => SetProperty(ref _apellidos, value);
        }

        public string Modalidad
        {
            get => _modalidad;
            set => SetProperty(ref _modalidad, value);
        }

        public bool IsCheckM
        {
            get => _isCheckM;
            set => SetProperty(ref _isCheckM, value);
        }

        public bool IsCheckJ
        {
            get => _isCheckJ;
            set => SetProperty(ref _isCheckJ, value);
        }

        public bool IsCheckConf
        {
            get => _isCheckConf;
            set => SetProperty(ref _isCheckConf, value);
        }

        public bool IsCheckMEnabled
        {
            get => _isCheckMEnabled;
            set => SetProperty(ref _isCheckMEnabled, value);
        }

        public bool IsCheckJEnabled
        {
            get => _isCheckJEnabled;
            set => SetProperty(ref _isCheckJEnabled, value);
        }

        public bool IsCheckConfEnabled
        {
            get => _isCheckConfEnabled;
            set => SetProperty(ref _isCheckConfEnabled, value);
        }

        public ICommand BuscarCommand { get; }
        public ICommand GuardarAsistenciaCommand { get; }
        public ICommand EscanearQRCommand { get; }

        public event EventHandler<string>? ShowAlert;
        public event EventHandler? NavigateToQRScanner;

        public async Task ConsultarPorIdentificadorAsync(int identificador)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ((Command)BuscarCommand).ChangeCanExecute();
            ((Command)GuardarAsistenciaCommand).ChangeCanExecute();
            ((Command)EscanearQRCommand).ChangeCanExecute();

            try
            {
                var alumno = await _apiService.ObtenerAlumnoPorIdentificadorAsync(identificador);
                if (alumno != null)
                {
                    IDAlumno = alumno.IDAlumno.ToString();
                    Identificador = alumno.Identificador.ToString();
                    Nombre = alumno.Nombre ?? string.Empty;
                    Apellidos = alumno.Apellidos ?? string.Empty;
                    Modalidad = alumno.Modalidad ?? string.Empty;

                    IsCheckM = alumno.AsisM == 1;
                    IsCheckJ = alumno.AsisJ == 1;
                    IsCheckConf = alumno.AsisConf == 1;

                    IsCheckMEnabled = !IsCheckM;
                    IsCheckJEnabled = !IsCheckJ;
                    IsCheckConfEnabled = !IsCheckConf;
                }
                else
                {
                    ShowAlert?.Invoke(this, "No encontrado|No se encontró un alumno con el Identificador ingresado");
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                ShowAlert?.Invoke(this, $"Error|Ocurrió un error al obtener los datos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                ((Command)BuscarCommand).ChangeCanExecute();
                ((Command)GuardarAsistenciaCommand).ChangeCanExecute();
                ((Command)EscanearQRCommand).ChangeCanExecute();
            }
        }

        private async Task ExecuteBuscarAsync()
        {
            if (string.IsNullOrWhiteSpace(IdentificadorEntry))
            {
                ShowAlert?.Invoke(this, "Error|Por favor ingresa un Identificador válido");
                return;
            }

            if (int.TryParse(IdentificadorEntry, out int identificador))
            {
                await ConsultarPorIdentificadorAsync(identificador);
            }
            else
            {
                ShowAlert?.Invoke(this, "Error|Por favor ingresa un Identificador válido");
            }
        }

        private async Task ExecuteGuardarAsistenciaAsync()
        {
            if (string.IsNullOrEmpty(Identificador))
            {
                ShowAlert?.Invoke(this, "Error|Por favor busca un alumno primero");
                return;
            }

            if (!int.TryParse(Identificador, out int identificador))
            {
                ShowAlert?.Invoke(this, "Error|Identificador inválido");
                return;
            }

            IsBusy = true;
            ((Command)BuscarCommand).ChangeCanExecute();
            ((Command)GuardarAsistenciaCommand).ChangeCanExecute();
            ((Command)EscanearQRCommand).ChangeCanExecute();

            var alumno = new Alumno
            {
                IDAlumno = int.TryParse(IDAlumno, out int idAlumno) ? idAlumno : 0,
                Identificador = identificador,
                Nombre = Nombre,
                Apellidos = Apellidos,
                Modalidad = Modalidad,
                AsisM = IsCheckM ? 1 : 0,
                AsisJ = IsCheckJ ? 1 : 0,
                AsisConf = IsCheckConf ? 1 : 0
            };

            try
            {
                if (await _apiService.ActualizarAlumnoAsync(identificador, alumno))
                {
                    LimpiarCampos();
                    ShowAlert?.Invoke(this, "Éxito|Se ha revisado asistencia con éxito");
                }
                else
                {
                    ShowAlert?.Invoke(this, "Error|Hubo un problema al revisar asistencia");
                }
            }
            catch (Exception ex)
            {
                ShowAlert?.Invoke(this, $"Error|Ocurrió un error al actualizar los datos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                ((Command)BuscarCommand).ChangeCanExecute();
                ((Command)GuardarAsistenciaCommand).ChangeCanExecute();
                ((Command)EscanearQRCommand).ChangeCanExecute();
            }
        }

        private async Task ExecuteEscanearQRAsync()
        {
            NavigateToQRScanner?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        private void LimpiarCampos()
        {
            Nombre = string.Empty;
            Apellidos = string.Empty;
            Modalidad = string.Empty;
            IdentificadorEntry = string.Empty;
            IDAlumno = string.Empty;
            Identificador = string.Empty;
            IsCheckM = false;
            IsCheckJ = false;
            IsCheckConf = false;
            IsCheckMEnabled = true;
            IsCheckJEnabled = true;
            IsCheckConfEnabled = true;
        }
    }
}

