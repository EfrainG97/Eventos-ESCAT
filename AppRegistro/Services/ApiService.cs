using System.Text;
using System.Text.Json;
using AppRegistro.Models;

namespace AppRegistro.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ISecureStorageService _secureStorageService;
        private string? _baseUrl;

        public ApiService(ISecureStorageService secureStorageService)
        {
            _httpClient = new HttpClient();
            _secureStorageService = secureStorageService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<string> GetBaseUrlAsync()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                var (ip, port) = await _secureStorageService.GetApiConfigAsync();
                if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                {
                    // Intentar HTTPS primero, luego HTTP
                    _baseUrl = $"https://{ip}:{port}";
                }
            }
            return _baseUrl ?? string.Empty;
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var token = await _secureStorageService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return _httpClient;
        }

        public async Task<LoginResponse?> LoginAsync(string ip, string port, string user, string password)
        {
            try
            {
                // Construir la URL base de la API (intentar HTTPS primero, luego HTTP)
                var baseUrl = $"https://{ip}:{port}";
                var loginUrl = $"{baseUrl}/api/auth/login";

                // Crear el request
                var request = new LoginRequest
                {
                    User = user,
                    Password = password
                };

                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Realizar la petición
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(loginUrl, content);
                }
                catch (HttpRequestException)
                {
                    // Si HTTPS falla, intentar con HTTP
                    baseUrl = $"http://{ip}:{port}";
                    loginUrl = $"{baseUrl}/api/auth/login";
                    response = await _httpClient.PostAsync(loginUrl, content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Guardar la baseUrl para uso futuro
                    _baseUrl = baseUrl;
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                    return loginResponse;
                }
                else
                {
                    // Intentar deserializar la respuesta de error
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                        return errorResponse;
                    }
                    catch
                    {
                        return new LoginResponse
                        {
                            Success = false,
                            Message = $"Error: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<Alumno?> ObtenerAlumnoPorIdentificadorAsync(int identificador)
        {
            try
            {
                var baseUrl = await GetBaseUrlAsync();
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("No se ha configurado la URL de la API. Por favor, inicia sesión primero.");
                }

                var client = await GetAuthenticatedClientAsync();
                var url = $"{baseUrl}/api/alumno/{identificador}";

                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(url);
                }
                catch (HttpRequestException)
                {
                    // Si HTTPS falla, intentar con HTTP
                    var (ip, port) = await _secureStorageService.GetApiConfigAsync();
                    if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                    {
                        baseUrl = $"http://{ip}:{port}";
                        _baseUrl = baseUrl;
                        url = $"{baseUrl}/api/alumno/{identificador}";
                        response = await client.GetAsync(url);
                    }
                    else
                    {
                        throw;
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var alumno = JsonSerializer.Deserialize<Alumno>(responseContent, _jsonOptions);
                    return alumno;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"Error al obtener alumno: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión al obtener alumno: {ex.Message}", ex);
            }
        }

        public async Task<bool> ActualizarAlumnoAsync(int identificador, Alumno alumno)
        {
            try
            {
                var baseUrl = await GetBaseUrlAsync();
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("No se ha configurado la URL de la API. Por favor, inicia sesión primero.");
                }

                var client = await GetAuthenticatedClientAsync();
                var url = $"{baseUrl}/api/alumno/{identificador}";

                var json = JsonSerializer.Serialize(alumno, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await client.PutAsync(url, content);
                }
                catch (HttpRequestException)
                {
                    // Si HTTPS falla, intentar con HTTP
                    var (ip, port) = await _secureStorageService.GetApiConfigAsync();
                    if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                    {
                        baseUrl = $"http://{ip}:{port}";
                        _baseUrl = baseUrl;
                        url = $"{baseUrl}/api/alumno/{identificador}";
                        response = await client.PutAsync(url, content);
                    }
                    else
                    {
                        throw;
                    }
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión al actualizar alumno: {ex.Message}", ex);
            }
        }
    }
}

