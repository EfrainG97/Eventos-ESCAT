# API Registro Eventos ESCAT

API REST con autenticación JWT para el registro de eventos de ESCAT.

## 🚀 Inicio Rápido

### Prerrequisitos
- .NET 10.0 SDK
- SQL Server (local o remoto)
- Visual Studio 2022 o VS Code

### Paquetes NuGet Utilizados
- `Microsoft.AspNetCore.Authentication.JwtBearer` (10.0.0) - Autenticación JWT
- `Microsoft.EntityFrameworkCore` (10.0.0) - Entity Framework Core
- `Microsoft.EntityFrameworkCore.SqlServer` (10.0.0) - Proveedor SQL Server
- `Microsoft.EntityFrameworkCore.Tools` (10.0.0) - Herramientas de migraciones
- `BCrypt.Net-Next` (4.0.3) - Hash de contraseñas
- `Swashbuckle.AspNetCore` (6.9.0) - Swagger/OpenAPI

### Instalación

1. **Restaurar paquetes NuGet:**
```bash
dotnet restore
```

2. **Configurar Base de Datos:**
   - Asegúrate de que SQL Server esté ejecutándose
   - La cadena de conexión está configurada en `appsettings.json`
   - Ajusta el nombre del servidor si es necesario

3. **Crear Migraciones de Base de Datos:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Configurar Usuario de Base de Datos (para IIS/Producción):**
   - Si vas a desplegar en IIS con acceso remoto, ejecuta el script:
   - **Ubicación**: `Scripts/CreateDatabaseUser.sql`
   - Abre el script en SQL Server Management Studio
   - **⚠️ IMPORTANTE**: Cambia la contraseña por defecto antes de ejecutar
   - Ejecuta el script completo en tu servidor SQL
   - Ver detalles completos en `SEGURIDAD.md`

5. **Configurar JWT Secret Key:**
   - **IMPORTANTE**: Abre `appsettings.json`
   - Cambia `JWT:SecretKey` por una clave segura de al menos 32 caracteres
   - En producción, usa variables de entorno

6. **Crear Usuario Inicial:**
   - Usa el endpoint `/api/admin/create-user` (solo disponible en desarrollo)
   - O ejecuta el script de creación de usuario (ver sección de Scripts)

7. **Ejecutar la API:**
```bash
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7108`
- HTTP: `http://localhost:5129`
- Swagger UI: `https://localhost:7108/swagger` o `http://localhost:5129/swagger`

**Nota:** Los puertos pueden variar según tu configuración. Verifica los puertos en `Properties/launchSettings.json` o en la consola al ejecutar `dotnet run`.

## 📖 Uso de la API

### 1. Autenticación (Login)

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "user": "admin",
  "password": "tu_contraseña"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-02T12:00:00Z",
  "message": "Autenticación exitosa",
  "user": {
    "idLogin": 1,
    "user": "admin",
    "email": "admin@escat.edu.mx",
    "role": "Admin"
  }
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Usuario o contraseña incorrectos"
}
```

### 2. Usar el Token JWT

Para acceder a endpoints protegidos, incluye el token en el header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Ejemplo con cURL:**
```bash
curl -X GET "https://localhost:7108/WeatherForecast" \
  -H "Authorization: Bearer TU_TOKEN_AQUI"
```

**Ejemplo con JavaScript (fetch):**
```javascript
fetch('https://localhost:7108/WeatherForecast', {
  headers: {
    'Authorization': 'Bearer ' + token
  }
})
```

**Ejemplo usando Swagger UI:**
1. Accede a `https://localhost:7108/swagger`
2. Haz clic en el botón **"Authorize"** (🔒) en la parte superior
3. Ingresa el token JWT en el formato: `Bearer TU_TOKEN_AQUI` o solo `TU_TOKEN_AQUI`
4. Haz clic en **"Authorize"** y luego en **"Close"**
5. Ahora puedes probar los endpoints protegidos directamente desde Swagger

### 3. Endpoints Protegidos

Todos los endpoints marcados con `[Authorize]` requieren el token JWT.

**Ejemplo - Endpoint protegido:**
```csharp
[HttpGet]
[Authorize]
public IActionResult GetData()
{
    return Ok("Datos protegidos");
}
```

**Ejemplo - Solo Administradores:**
```csharp
[HttpGet("admin")]
[Authorize(Roles = "Admin")]
public IActionResult AdminOnly()
{
    return Ok("Solo administradores");
}
```

## 🗂️ Estructura del Proyecto

```
APIRegistro/
├── Controllers/
│   ├── AuthController.cs          # Endpoints de autenticación
│   ├── AdminController.cs         # Endpoints de administración (crear usuarios)
│   ├── AlumnoController.cs        # Endpoints de gestión de alumnos
│   └── WeatherForecastController.cs  # Ejemplo de endpoint protegido
├── Services/
│   ├── IAuthService.cs            # Interfaz del servicio de autenticación
│   └── AuthService.cs             # Implementación del servicio
├── Data/
│   └── ApplicationDbContext.cs    # Contexto de Entity Framework
├── DTOs/
│   ├── LoginRequest.cs            # DTO para requests de login
│   └── LoginResponse.cs           # DTO para respuestas de login
├── Model/
│   ├── Login.cs                   # Modelo de usuario/login
│   └── Alumno.cs                  # Modelo de alumno
├── Scripts/
│   ├── CreateUser.cs              # Script de utilidad para crear usuarios
│   ├── CreateDatabaseUser.sql     # ⭐ Script SQL para crear usuario de BD con acceso remoto
│   └── DeploymentUtilities.ps1    # ⭐ Utilidades PowerShell para despliegue (generar claves, etc.)
├── Program.cs                     # Configuración de la aplicación
├── appsettings.json                # Configuración (incluye secretos)
├── web.config.example              # ⭐ Ejemplo de configuración para IIS
├── README.md                       # Este archivo - Guía general de la API
├── SEGURIDAD.md                    # ⭐ Guía de seguridad y configuración de BD
└── DESPLIEGUE_IIS.md               # ⭐ Guía paso a paso para desplegar en IIS

⭐ = Archivos nuevos para despliegue en IIS
```

## 🔐 Seguridad

**Lee el archivo `SEGURIDAD.md` para recomendaciones completas de seguridad.**

### Puntos Críticos:
1. ✅ **Cambiar JWT SecretKey** antes de producción
2. ✅ Usar **HTTPS** en producción
3. ✅ Las contraseñas se almacenan con **hash BCrypt**
4. ✅ Validación de entrada en todos los endpoints
5. ✅ CORS configurado restrictivamente
6. ✅ **Usuario de base de datos dedicado** para IIS con permisos limitados

## 🌐 Despliegue en IIS

Para desplegar la API en IIS con acceso remoto a SQL Server:

### Opción 1: Usar el Script de Utilidades (Recomendado)

Hemos creado un script de PowerShell que automatiza muchas tareas comunes:

```powershell
# Navega a la carpeta del proyecto
cd APIRegistro\Scripts

# Carga el script (requiere permisos de administrador para algunas funciones)
. .\DeploymentUtilities.ps1

# Inicia el menú interactivo
Start-DeploymentUtilities
```

**El script incluye:**
- ✅ Generador de JWT Secret Key
- ✅ Generador de contraseñas seguras para SQL Server
- ✅ Probar conectividad a SQL Server
- ✅ Crear reglas de firewall automáticamente
- ✅ Publicar la aplicación
- ✅ Ver logs recientes
- ✅ Reiniciar IIS

**Uso directo de funciones:**
```powershell
# Generar JWT Secret Key
New-JWTSecretKey

# Generar contraseña para SQL Server
New-SecurePassword

# Probar conectividad a SQL Server
Test-SQLConnection -ServerName "192.168.1.100" -Port 1433

# Crear regla de firewall (requiere admin)
New-SQLFirewallRule -RemoteIP "192.168.1.50"

# Publicar aplicación
Publish-APIRegistro -OutputPath "C:\inetpub\wwwroot\EventosESCAT"

# Ver logs
Get-APILogs -LogPath "C:\inetpub\wwwroot\EventosESCAT\logs"

# Reiniciar IIS (requiere admin)
Restart-IISServer
```

### Opción 2: Configuración Manual

1. **Ejecutar el script de usuario de base de datos:**
   ```bash
   # Ubicación: Scripts/CreateDatabaseUser.sql
   # Ejecutar en SQL Server Management Studio
   # ⚠️ Cambiar la contraseña antes de ejecutar
   ```

2. **Actualizar la cadena de conexión en `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA;TrustServerCertificate=True;Encrypt=True;"
   }
   ```

3. **Configurar acceso remoto en SQL Server:**
   - Habilitar autenticación mixta (Windows + SQL Server)
   - Habilitar TCP/IP en SQL Server Configuration Manager
   - Configurar firewall para permitir puerto 1433
   - Ver guía completa en `SEGURIDAD.md`

4. **Publicar la aplicación:**
   ```bash
   dotnet publish -c Release -o C:\inetpub\wwwroot\EventosESCAT
   ```

5. **Configurar en IIS:**
   - Crear Application Pool (No Managed Code)
   - Crear sitio web apuntando a la carpeta publicada
   - Configurar variables de entorno para secretos
   - Configurar HTTPS con certificado SSL

**📖 Ver la guía completa de despliegue en `DESPLIEGUE_IIS.md`**

## 🛠️ Desarrollo

### Crear un Nuevo Usuario

**Opción 1: Usar el endpoint de administración (recomendado para desarrollo)**

```bash
POST /api/admin/create-user
Content-Type: application/json

{
  "user": "nuevo_usuario",
  "password": "contraseña_segura",
  "email": "usuario@escat.edu.mx",
  "role": "User"
}
```

**Opción 2: Usar código directamente**

```csharp
// Ejemplo de código para crear usuario
var user = new Login
{
    User = "nuevo_usuario",
    PasswordHash = AuthService.HashPassword("contraseña_segura"),
    Email = "usuario@escat.edu.mx",
    Role = "User",
    IsActive = true
};

context.Logins.Add(user);
await context.SaveChangesAsync();
```

**⚠️ IMPORTANTE:** El `AdminController` está disponible sin autenticación solo para facilitar el desarrollo inicial. **Debes protegerlo o eliminarlo en producción.**

### Migraciones de Base de Datos

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update NombreMigracionAnterior
```

## 📝 Notas

- El token JWT expira después de 24 horas (configurable en `appsettings.json`)
- Los usuarios inactivos (`IsActive = false`) no pueden autenticarse
- Los roles disponibles son: `Admin`, `User` (configurables)
- Swagger UI está disponible **solo en modo Development** para probar la API
- Swagger incluye soporte para autenticación JWT - usa el botón "Authorize" para agregar tu token
- Los endpoints de administración (`/api/admin/*`) están disponibles sin autenticación solo en desarrollo
- Para producción en IIS, usa el usuario `EventosESCATUser` creado por el script SQL
- El usuario de BD tiene permisos limitados (db_datareader, db_datawriter, EXECUTE)

## 📚 Documentación Adicional

### Archivos de Referencia

| Archivo | Descripción |
|---------|-------------|
| **README.md** | Este archivo - Guía general de la API |
| **SEGURIDAD.md** | Guía completa de seguridad, configuración de BD y variables de entorno |
| **DESPLIEGUE_IIS.md** | Guía paso a paso para desplegar en IIS con acceso remoto a SQL Server |
| **Scripts/CreateDatabaseUser.sql** | Script SQL para crear usuario de BD con permisos limitados |
| **Scripts/CreateUser.cs** | Utilidad para crear usuarios de aplicación |

### Guías Rápidas

- **Para desarrollo local**: Sigue la sección [Inicio Rápido](#-inicio-rápido)
- **Para desplegar en IIS**: Lee `DESPLIEGUE_IIS.md` (guía paso a paso completa)
- **Para configuración de seguridad**: Consulta `SEGURIDAD.md`
- **Para crear usuario de BD**: Ejecuta `Scripts/CreateDatabaseUser.sql`

