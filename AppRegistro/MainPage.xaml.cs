using AppRegistro.Models;
using AppRegistro.Services;

namespace AppRegistro
{
    public partial class MainPage : ContentPage
    {
        private readonly IApiService _apiService;
        
        public MainPage(IApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        public bool IsCheckM { get; set; }
        public bool IsCheckJ { get; set; }
        public bool IsCheckConf { get; set; }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (int.TryParse(IdEntry.Text, out int identificador))
            {
                try
                {
                    var alumno = await _apiService.ObtenerAlumnoPorIdentificadorAsync(identificador);
                    if (alumno != null)
                    {
                        IDAlumnoLabel.Text = alumno.IDAlumno.ToString();
                        IdentificadorLabel.Text = alumno.Identificador.ToString();
                        NombreLabel.Text = alumno.Nombre ?? string.Empty;
                        ApellidosLabel.Text = alumno.Apellidos ?? string.Empty;
                        ModalidadLabel.Text = alumno.Modalidad ?? string.Empty;
                        
                        IsCheckM = alumno.AsisM == 1;
                        IsCheckJ = alumno.AsisJ == 1;
                        IsCheckConf = alumno.AsisConf == 1;
                        
                        if (IsCheckM)
                        {
                            CheckBoxM.IsEnabled = false;
                        }
                        else
                        {
                            CheckBoxM.IsEnabled = true;
                        }
                        
                        if (IsCheckJ)
                        {
                            CheckBoxJ.IsEnabled = false;
                        }
                        else
                        {
                            CheckBoxJ.IsEnabled = true;
                        }
                        
                        if (IsCheckConf)
                        {
                            CheckBoxConf.IsEnabled = false;
                        }
                        else
                        {
                            CheckBoxConf.IsEnabled = true;
                        }
                        
                        CheckBoxM.IsChecked = IsCheckM;
                        CheckBoxJ.IsChecked = IsCheckJ;
                        CheckBoxConf.IsChecked = IsCheckConf;
                    }
                    else
                    {
                        await DisplayAlert("No encontrado", "No se encontró un alumno con el Identificador ingresado", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al obtener los datos: {ex.Message}", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Por favor ingresa un Identificador válido", "Ok");
            }
        }

        private async void updButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(IdentificadorLabel.Text))
            {
                await DisplayAlert("Error", "Por favor busca un alumno primero", "Ok");
                return;
            }

            if (!int.TryParse(IdentificadorLabel.Text, out int identificador))
            {
                await DisplayAlert("Error", "Identificador inválido", "Ok");
                return;
            }

            var alumno = new Alumno
            {
                IDAlumno = int.TryParse(IDAlumnoLabel.Text, out int idAlumno) ? idAlumno : 0,
                Identificador = identificador,
                Nombre = NombreLabel.Text,
                Apellidos = ApellidosLabel.Text,
                Modalidad = ModalidadLabel.Text,
                AsisM = CheckBoxM.IsChecked ? 1 : 0,
                AsisJ = CheckBoxJ.IsChecked ? 1 : 0,
                AsisConf = CheckBoxConf.IsChecked ? 1 : 0
            };

            try
            {
                if (await _apiService.ActualizarAlumnoAsync(identificador, alumno))
                {
                    LimpiarCampos();
                    await DisplayAlert("Éxito", "Se ha revisado asistencia con éxito", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Hubo un problema al revisar asistencia", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al actualizar los datos: {ex.Message}", "Ok");
            }
        }

        private void LimpiarCampos()
        {
            NombreLabel.Text = string.Empty;
            ApellidosLabel.Text = string.Empty;
            ModalidadLabel.Text = string.Empty;
            IdEntry.Text = string.Empty;
            IDAlumnoLabel.Text = string.Empty;
            IdentificadorLabel.Text = string.Empty;
            IsCheckM = false;
            IsCheckJ = false;
            IsCheckConf = false;
            CheckBoxM.IsChecked = false;
            CheckBoxJ.IsChecked = false;
            CheckBoxConf.IsChecked = false;
            CheckBoxM.IsEnabled = true;
            CheckBoxJ.IsEnabled = true;
            CheckBoxConf.IsEnabled = true;
        }

        private async void scanButton_Clicked(object sender, EventArgs e)
        {
            var qrScannerPage = new Pages.QRScannerPage(this);
            await Navigation.PushAsync(qrScannerPage);
        }

        public async void ConsultarQR(int identificador)
        {
            try
            {
                var alumno = await _apiService.ObtenerAlumnoPorIdentificadorAsync(identificador);
                if (alumno != null)
                {
                    IDAlumnoLabel.Text = alumno.IDAlumno.ToString();
                    IdentificadorLabel.Text = alumno.Identificador.ToString();
                    NombreLabel.Text = alumno.Nombre ?? string.Empty;
                    ApellidosLabel.Text = alumno.Apellidos ?? string.Empty;
                    ModalidadLabel.Text = alumno.Modalidad ?? string.Empty;
                    
                    IsCheckM = alumno.AsisM == 1;
                    IsCheckJ = alumno.AsisJ == 1;
                    IsCheckConf = alumno.AsisConf == 1;
                    
                    if (IsCheckM)
                    {
                        CheckBoxM.IsEnabled = false;
                    }
                    else
                    {
                        CheckBoxM.IsEnabled = true;
                    }
                    
                    if (IsCheckJ)
                    {
                        CheckBoxJ.IsEnabled = false;
                    }
                    else
                    {
                        CheckBoxJ.IsEnabled = true;
                    }
                    
                    if (IsCheckConf)
                    {
                        CheckBoxConf.IsEnabled = false;
                    }
                    else
                    {
                        CheckBoxConf.IsEnabled = true;
                    }
                    
                    CheckBoxM.IsChecked = IsCheckM;
                    CheckBoxJ.IsChecked = IsCheckJ;
                    CheckBoxConf.IsChecked = IsCheckConf;
                }
                else
                {
                    await DisplayAlert("No encontrado", "No se encontró un alumno con el Identificador ingresado", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al obtener los datos: {ex.Message}", "Ok");
            }
        }
    }
}
