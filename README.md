# EventosESCAT - Sistema de Registro de Eventos Universitarios

Sistema completo para el registro de asistencia de alumnos a eventos universitarios mediante escaneo de códigos QR. Consta de una **API REST** backend y una **aplicación móvil .NET MAUI** multiplataforma.

## 📋 Tabla de Contenidos

1. [Descripción General](#-descripción-general)
2. [Arquitectura del Proyecto](#-arquitectura-del-proyecto)
3. [Requisitos Previos](#-requisitos-previos)
4. [Instalación Completa Paso a Paso](#-instalación-completa-paso-a-paso)
   - [Paso 1: Configurar SQL Server](#paso-1-configurar-sql-server)
   - [Paso 2: Crear la Base de Datos](#paso-2-crear-la-base-de-datos)
   - [Paso 3: Compilar y Configurar la API](#paso-3-compilar-y-configurar-la-api)
   - [Paso 4: Desplegar la API en IIS](#paso-4-desplegar-la-api-en-iis)
   - [Paso 5: Compilar la Aplicación Móvil](#paso-5-compilar-la-aplicación-móvil)
5. [Uso del Sistema](#-uso-del-sistema)
6. [Solución de Problemas](#-solución-de-problemas)
7. [Mantenimiento](#-mantenimiento)

---

## 📱 Descripción General

**EventosESCAT** es un sistema diseñado para eventos universitarios anuales que permite:

- ✅ **Autenticación JWT**: Login seguro con tokens JWT para administradores
- ✅ **Escaneo de Códigos QR**: Lectura de códigos QR para identificar alumnos
- ✅ **Registro de Eventos**: Actualización de información de alumnos en tiempo real
- ✅ **Almacenamiento Seguro**: Guardado seguro de credenciales y tokens
- ✅ **Configuración Flexible**: Configuración de IP y puerto del servidor API

### Características Técnicas

- **Backend**: API REST con ASP.NET Core 10.0
- **Base de Datos**: SQL Server con Entity Framework Core
- **Frontend**: Aplicación móvil .NET MAUI (Android, iOS, Windows)
- **Autenticación**: JWT (JSON Web Tokens)
- **Seguridad**: Hash de contraseñas con BCrypt, validación de entrada, CORS configurado

---

## 🏗️ Arquitectura del Proyecto

```
EventosESCAT/
├── APIRegistro/              # API REST Backend
│   ├── Controllers/          # Controladores de la API
│   │   ├── AuthController.cs      # Autenticación
│   │   ├── AdminController.cs     # Administración de usuarios
│   │   └── AlumnoController.cs    # Gestión de alumnos
│   ├── Services/             # Servicios de negocio
│   │   ├── AuthService.cs         # Servicio de autenticación
│   │   └── IAuthService.cs
│   ├── Data/                 # Contexto de base de datos
│   │   └── ApplicationDbContext.cs
│   ├── Models/               # Modelos de datos
│   │   ├── Login.cs
│   │   └── Alumno.cs
│   ├── DTOs/                 # Data Transfer Objects
│   ├── Migrations/           # Migraciones de Entity Framework
│   ├── Scripts/              # Scripts de utilidad
│   │   ├── CreateDatabaseUser.sql      # Crear usuario de BD
│   │   └── DeploymentUtilities.ps1     # Utilidades PowerShell
│   ├── Program.cs            # Configuración de la aplicación
│   └── appsettings.json      # Configuración
│
└── AppRegistro/              # Aplicación móvil .NET MAUI
    ├── Pages/                # Páginas de la aplicación
    │   ├── LoginPage.xaml
    │   ├── QRScannerPage.xaml
    │   └── MainPage.xaml
    ├── Services/             # Servicios de la aplicación
    │   ├── ApiService.cs
    │   └── SecureStorageService.cs
    ├── ViewModels/           # ViewModels (MVVM)
    ├── Models/               # Modelos de datos
    └── Platforms/            # Configuraciones específicas por plataforma
        ├── Android/
        └── iOS/
```

---

## 📋 Requisitos Previos

### Para el Servidor (API + IIS)

- **Windows Server** o **Windows 10/11 Pro** con IIS instalado
- **.NET 10.0 SDK** o superior
- **.NET 10.0 Hosting Bundle** (para IIS)
- **SQL Server** (2019 o superior, puede estar en servidor diferente)
- **SQL Server Management Studio** (SSMS)
- **Visual Studio 2022** (opcional, para desarrollo)
- **PowerShell 5.1** o superior

### Para Desarrollo de la App Móvil

- **.NET 10.0 SDK** o superior
- **Visual Studio 2022** con carga de trabajo de desarrollo móvil
- **Android SDK** (para desarrollo Android)
- **Java JDK 17** o superior
- **Android Emulator** o dispositivo físico con Android 5.0 (API 21) o superior

### Verificar Instalaciones

```powershell
# Verificar .NET SDK
dotnet --version

# Verificar .NET Runtime instalado
dotnet --list-runtimes

# Verificar IIS
Get-WindowsFeature -Name Web-Server

# Verificar SQL Server (desde SSMS)
SELECT @@VERSION
```

---

## 🚀 Instalación Completa Paso a Paso

### Paso 1: Configurar SQL Server

#### 1.1. Habilitar Autenticación Mixta

**Opción A: Desde SQL Server Management Studio**
1. Abre **SQL Server Management Studio**
2. Conéctate al servidor SQL Server
3. Clic derecho en el servidor → **Properties**
4. Ve a la pestaña **Security**
5. Selecciona **"SQL Server and Windows Authentication mode"**
6. Clic **OK**
7. **Reiniciar el servicio de SQL Server**:
   - Presiona `Win + R`, escribe `services.msc`, Enter
   - Busca **SQL Server (MSSQLSERVER)** o tu instancia
   - Clic derecho → **Restart**

**Opción B: Desde PowerShell (como Administrador)**
```powershell
# Reiniciar SQL Server
Restart-Service -Name "MSSQLSERVER" -Force
```

#### 1.2. Habilitar TCP/IP

1. Abre **SQL Server Configuration Manager**
   - Presiona `Win + R`, escribe `SQLServerManager20XX.msc` (reemplaza XX con tu versión)
2. Ve a **SQL Server Network Configuration** → **Protocols for [TU_INSTANCIA]**
3. Clic derecho en **TCP/IP** → **Enable**
4. Doble clic en **TCP/IP** → Pestaña **IP Addresses**
5. Desplázate hasta **IPAll**
6. Configura:
   - **TCP Dynamic Ports**: (dejar vacío)
   - **TCP Port**: `1433`
7. Clic **OK**
8. **Reiniciar el servicio de SQL Server** (ver paso 1.1)

#### 1.3. Configurar Firewall

**PowerShell (como Administrador):**
```powershell
# Permitir puerto 1433 para SQL Server
New-NetFirewallRule -DisplayName "SQL Server - Puerto 1433" `
    -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow

# Opcional: Permitir solo desde IP específica del servidor IIS (más seguro)
# New-NetFirewallRule -DisplayName "SQL Server - IIS Only" `
#     -Direction Inbound -Protocol TCP -LocalPort 1433 `
#     -RemoteAddress 192.168.1.50 -Action Allow
```

**Firewall GUI (Windows Defender):**
1. Panel de Control → **Windows Defender Firewall** → **Configuración avanzada**
2. **Reglas de entrada** → **Nueva regla...**
3. Tipo: **Puerto** → Siguiente
4. **TCP**, Puerto específico: `1433` → Siguiente
5. **Permitir la conexión** → Siguiente
6. Seleccionar todos los perfiles → Siguiente
7. Nombre: `SQL Server - Puerto 1433` → Finalizar

#### 1.4. Verificar Conectividad

```powershell
# Desde el servidor IIS, probar conexión al SQL Server
# Reemplaza TU_SERVIDOR_SQL con la IP o nombre del servidor SQL
Test-NetConnection -ComputerName TU_SERVIDOR_SQL -Port 1433

# Si funciona, deberías ver:
# TcpTestSucceeded : True
```

---

### Paso 2: Crear la Base de Datos

#### 2.1. Crear la Base de Datos EventosESCAT

1. Abre **SQL Server Management Studio**
2. Conéctate al servidor SQL Server
3. Clic derecho en **Databases** → **New Database...**
4. Nombre: `EventosESCAT`
5. Clic **OK**

#### 2.2. Crear el Usuario de Base de Datos

**⚠️ IMPORTANTE**: Este usuario será usado por IIS para conectarse remotamente a SQL Server.

1. Abre el archivo `APIRegistro/Scripts/CreateDatabaseUser.sql` en SQL Server Management Studio
2. **CAMBIAR LA CONTRASEÑA** en la línea 15:
   ```sql
   WITH PASSWORD = N'TuContraseñaSegura123!',  -- ⚠️ CAMBIAR ESTO
   ```
   
   **Recomendaciones de contraseña:**
   - Mínimo 12 caracteres
   - Incluir mayúsculas, minúsculas, números y símbolos
   - Ejemplo: `Ev3nt0s#ESCAT@2024$Secure!`
   
   **Generar contraseña segura con PowerShell:**
   ```powershell
   cd APIRegistro\Scripts
   . .\DeploymentUtilities.ps1
   New-SecurePassword
   # Copia el resultado y úsalo en el script SQL
   ```

3. Ejecuta el script completo (F5)
4. Verifica los mensajes de confirmación en la ventana de resultados
5. **Guarda la contraseña** en un lugar seguro (la necesitarás para configurar IIS)

#### 2.3. Aplicar Migraciones de Entity Framework

```powershell
# Navegar a la carpeta del proyecto API
cd APIRegistro

# Restaurar paquetes NuGet
dotnet restore

# Aplicar migraciones (crea las tablas automáticamente)
dotnet ef database update
```

**Si no tienes las herramientas de EF instaladas:**
```powershell
dotnet tool install --global dotnet-ef
```

**Verificar que las tablas se crearon:**
1. En SQL Server Management Studio
2. Expande **EventosESCAT** → **Tables**
3. Deberías ver tablas como: `Logins`, `Alumnos`, `__EFMigrationsHistory`

---

### Paso 3: Compilar y Configurar la API

#### 3.1. Restaurar Paquetes NuGet

```powershell
# Desde la raíz del proyecto
cd APIRegistro
dotnet restore
```

#### 3.2. Configurar appsettings.json

Abre `APIRegistro/appsettings.json` y configura:

```json
{
  "ConnectionStrings": {
    // Para desarrollo local con Windows Authentication:
    // "DefaultConnection": "Server=TU_SERVIDOR;Database=EventosESCAT;Trusted_Connection=True;TrustServerCertificate=True;"
    
    // Para producción en IIS con SQL Server Authentication:
    "DefaultConnection": "Server=TU_SERVIDOR_SQL;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA;TrustServerCertificate=True;Encrypt=True;"
  },
  "JWT": {
    "SecretKey": "GENERA_UNA_CLAVE_ALEATORIA_DE_AL_MENOS_64_CARACTERES",
    "Issuer": "APIRegistro",
    "Audience": "APIRegistroUsers",
    "ExpirationHours": "24"
  },
  "AllowedOrigins": "https://localhost:5001,http://localhost:5000"
}
```

**⚠️ IMPORTANTE - Generar JWT Secret Key:**

```powershell
# Usar el script de utilidades
cd APIRegistro\Scripts
. .\DeploymentUtilities.ps1
New-JWTSecretKey

# O generar manualmente con PowerShell:
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

**Reemplaza:**
- `TU_SERVIDOR_SQL`: IP o nombre del servidor SQL (ej: `192.168.1.100` o `localhost\SQLEXPRESS`)
- `TU_CONTRASEÑA`: La contraseña que configuraste para `EventosESCATUser` en el paso 2.2
- `GENERA_UNA_CLAVE_ALEATORIA...`: La clave JWT generada

#### 3.3. Probar la API Localmente (Opcional)

```powershell
# Ejecutar la API en modo desarrollo
cd APIRegistro
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7108`
- HTTP: `http://localhost:5129`
- Swagger UI: `https://localhost:7108/swagger`

**Crear usuario inicial (solo en desarrollo):**
```powershell
# Usar el endpoint de administración
$body = @{
    user = "admin"
    password = "tu_contraseña_segura"
    email = "admin@escat.edu.mx"
    role = "Admin"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5129/api/admin/create-user" `
    -Method Post -Body $body -ContentType "application/json"
```

---

### Paso 4: Desplegar la API en IIS

#### 4.1. Instalar .NET 10 Hosting Bundle

1. Descarga el **.NET 10 Hosting Bundle** desde:
   - https://dotnet.microsoft.com/download/dotnet/10.0
   - Busca "Hosting Bundle" en la sección "Run apps"

2. Ejecuta el instalador **como administrador**

3. **Reiniciar IIS** después de la instalación:
   ```powershell
   # PowerShell como Administrador
   iisreset
   ```

4. Verificar instalación:
   ```powershell
   dotnet --list-runtimes
   # Deberías ver: Microsoft.AspNetCore.App 10.0.x
   ```

#### 4.2. Publicar la Aplicación

**Opción A: Desde Visual Studio**
1. Abre el proyecto `APIRegistro` en Visual Studio
2. Clic derecho en el proyecto → **Publish...**
3. Target: **Folder**
4. Location: `C:\inetpub\wwwroot\EventosESCAT` (o la ruta que prefieras)
5. Configuration: **Release**
6. Target Framework: **net10.0**
7. Clic **Publish**

**Opción B: Desde PowerShell (Recomendado)**
```powershell
# Navegar a la carpeta del proyecto API
cd APIRegistro

# Publicar la aplicación
dotnet publish -c Release -o C:\inetpub\wwwroot\EventosESCAT
```

**Opción C: Usar el Script de Utilidades**
```powershell
cd APIRegistro\Scripts
. .\DeploymentUtilities.ps1
Publish-APIRegistro -OutputPath "C:\inetpub\wwwroot\EventosESCAT"
```

#### 4.3. Configurar web.config

1. Copia el archivo de ejemplo:
   ```powershell
   Copy-Item "APIRegistro\web.config.example" "C:\inetpub\wwwroot\EventosESCAT\web.config"
   ```

2. Edita `C:\inetpub\wwwroot\EventosESCAT\web.config` y reemplaza:

   ```xml
   <environmentVariable name="ConnectionStrings__DefaultConnection" 
        value="Server=TU_SERVIDOR_SQL;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA;TrustServerCertificate=True;Encrypt=True;" />
   
   <environmentVariable name="JWT__SecretKey" 
        value="TU_CLAVE_SECRETA_JWT_DE_64_CARACTERES" />
   ```

   **Reemplaza:**
   - `TU_SERVIDOR_SQL`: IP o nombre del servidor SQL
   - `TU_CONTRASEÑA`: Contraseña del usuario `EventosESCATUser`
   - `TU_CLAVE_SECRETA_JWT...`: JWT Secret Key generada

3. **Crear carpeta de logs:**
   ```powershell
   New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\EventosESCAT\logs" -Force
   ```

#### 4.4. Crear Application Pool en IIS

1. Abre **IIS Manager** (`inetmgr`)
2. En el panel izquierdo, expande el servidor
3. Clic derecho en **Application Pools** → **Add Application Pool...**
4. Configuración:
   - **Name**: `EventosESCAT_Pool`
   - **.NET CLR version**: **No Managed Code**
   - **Managed pipeline mode**: **Integrated**
5. Clic **OK**
6. Clic derecho en el nuevo pool → **Advanced Settings...**
7. Verifica:
   - **Start Mode**: `AlwaysRunning` (opcional, mejora rendimiento)
   - **Identity**: `ApplicationPoolIdentity` (recomendado)
8. Clic **OK**

#### 4.5. Crear el Sitio Web en IIS

1. En **IIS Manager**, clic derecho en **Sites** → **Add Website...**
2. Configuración:
   - **Site name**: `EventosESCAT`
   - **Application pool**: `EventosESCAT_Pool`
   - **Physical path**: `C:\inetpub\wwwroot\EventosESCAT`
   - **Binding**:
     - Type: `http`
     - IP address: `All Unassigned`
     - Port: `80` (o el puerto que desees, ej: 8080)
     - Host name: (dejar vacío o agregar tu dominio)
3. Clic **OK**

#### 4.6. Configurar HTTPS (Recomendado para Producción)

**Si tienes un certificado SSL:**
1. En **IIS Manager**, selecciona tu sitio **EventosESCAT**
2. En el panel derecho, clic en **Bindings...**
3. Clic **Add...**
4. Configuración:
   - Type: `https`
   - Port: `443`
   - SSL certificate: Selecciona tu certificado
5. Clic **OK**

**Para desarrollo/testing (certificado auto-firmado):**
```powershell
# PowerShell como Administrador
New-SelfSignedCertificate -DnsName "localhost", "tu-servidor" `
    -CertStoreLocation "cert:\LocalMachine\My"
```
Luego asigna este certificado en IIS como se indicó arriba.

#### 4.7. Probar la API Desplegada

**Opción A: Desde el navegador**
- Si Swagger está habilitado (solo en Development): `http://tu-servidor/swagger`
- **Nota**: Por seguridad, Swagger está deshabilitado en modo Production

**Opción B: Desde PowerShell**
```powershell
# Probar endpoint de login
$body = @{
    user = "admin"
    password = "tu_contraseña"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://tu-servidor/api/auth/login" `
    -Method Post -Body $body -ContentType "application/json"
```

**Si recibes un token JWT, ¡la API está funcionando! ✅**

**Ver logs si hay errores:**
```powershell
Get-Content "C:\inetpub\wwwroot\EventosESCAT\logs\stdout*.log" -Tail 50
```

---

### Paso 5: Compilar la Aplicación Móvil

#### 5.1. Restaurar Paquetes NuGet

```powershell
cd AppRegistro
dotnet restore
```

#### 5.2. Configurar la URL de la API

La URL de la API se configura desde la aplicación móvil en la pantalla de login. Los usuarios ingresarán:
- **IP del servidor**: La IP donde está desplegada la API en IIS
- **Puerto**: El puerto configurado en IIS (ej: 80 para HTTP, 443 para HTTPS)

**Ejemplo:**
- IP: `192.168.1.100`
- Puerto: `80`
- URL completa: `http://192.168.1.100`

#### 5.3. Compilar para Android (APK)

**Crear Keystore (solo la primera vez):**

```cmd
cd AppRegistro

keytool -genkeypair -v -storetype PKCS12 -keystore eventos-escat.keystore `
    -alias eventos-escat -keyalg RSA -keysize 2048 -validity 10000 `
    -storepass 123456 -keypass 123456
```

**⚠️ IMPORTANTE**: Cambia las contraseñas (`123456`) por contraseñas seguras antes de producción.

**Publicar APK Firmado:**

```cmd
cd AppRegistro

dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk `
    /p:AndroidKeyStore=true /p:AndroidSigningKeyStore=eventos-escat.keystore `
    /p:AndroidSigningKeyAlias=eventos-escat /p:AndroidSigningKeyPass=123456 `
    /p:AndroidSigningStorePass=123456
```

**Ubicación del APK generado:**
```
AppRegistro\bin\Release\net10.0-android\com.escat.appregistro-Signed.apk
```

#### 5.4. Ejecutar en Modo Debug

**Desde Visual Studio:**
1. Abre el proyecto `AppRegistro` en Visual Studio 2022
2. Selecciona el proyecto `AppRegistro`
3. Selecciona un emulador Android o dispositivo físico
4. Presiona F5 o clic en "Iniciar"

**Desde Línea de Comandos:**
```cmd
cd AppRegistro
dotnet build -f net10.0-android
dotnet run -f net10.0-android
```

---

## 📱 Uso del Sistema

### 1. Inicio de Sesión en la App Móvil

1. Abre la aplicación móvil
2. Ingresa la **IP del servidor** donde está desplegada la API
3. Ingresa el **puerto** del servidor (ej: 80 para HTTP, 443 para HTTPS)
4. Ingresa tu **usuario** y **contraseña**
5. Presiona "Iniciar Sesión"

**Nota**: La configuración de IP y puerto se guarda automáticamente para futuras sesiones.

### 2. Escanear Código QR

1. Después del login, presiona el botón "Escanear QR"
2. Apunta la cámara al código QR del alumno
3. La aplicación detectará automáticamente el código y consultará la información del alumno
4. Si el código no es válido, se mostrará un mensaje de error

### 3. Registrar Evento

1. Después de escanear un código QR válido, se mostrará la información del alumno
2. Completa los campos necesarios (si aplica)
3. Presiona "Registrar" para guardar los cambios

---

## 🔧 Solución de Problemas

### Error: "HTTP Error 500.19 - Internal Server Error"

**Causa**: No se encontró el módulo ASP.NET Core

**Solución**:
1. Instala el .NET 10 Hosting Bundle
2. Reinicia IIS: `iisreset`

---

### Error: "HTTP Error 502.5 - Process Failure"

**Causa**: Error al iniciar la aplicación

**Solución**:
1. Habilita logs en `web.config`:
   ```xml
   <aspNetCore ... stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout">
   ```
2. Reinicia el sitio en IIS
3. Revisa los logs en `C:\inetpub\wwwroot\EventosESCAT\logs\`
4. Posibles causas:
   - Cadena de conexión incorrecta
   - JWT SecretKey no configurada
   - No se puede conectar a SQL Server

---

### Error: "Cannot open database 'EventosESCAT'"

**Causa**: El usuario no tiene acceso a la base de datos

**Solución**:
1. Verifica que ejecutaste el script `CreateDatabaseUser.sql`
2. En SQL Server Management Studio:
   ```sql
   USE EventosESCAT;
   SELECT name FROM sys.database_principals WHERE name = 'EventosESCATUser';
   ```
3. Si no existe, ejecuta el script completo

---

### Error: "Login failed for user 'EventosESCATUser'"

**Causa**: Contraseña incorrecta o login deshabilitado

**Solución**:
1. Verifica la contraseña en `web.config`
2. En SQL Server:
   ```sql
   -- Verificar que el login está habilitado
   SELECT name, is_disabled FROM sys.sql_logins WHERE name = 'EventosESCATUser';
   
   -- Si está deshabilitado:
   ALTER LOGIN [EventosESCATUser] ENABLE;
   
   -- Cambiar contraseña si es necesario:
   ALTER LOGIN [EventosESCATUser] WITH PASSWORD = N'NuevaContraseña123!';
   ```

---

### No se puede conectar a SQL Server remotamente

**Solución paso a paso:**

1. **Verificar que SQL Server escucha en TCP/IP:**
   ```powershell
   # En el servidor SQL
   netstat -an | findstr 1433
   # Deberías ver: TCP  0.0.0.0:1433  0.0.0.0:0  LISTENING
   ```

2. **Probar conectividad desde el servidor IIS:**
   ```powershell
   Test-NetConnection -ComputerName IP_DEL_SERVIDOR_SQL -Port 1433
   # TcpTestSucceeded debe ser True
   ```

3. **Si falla**, verifica:
   - Firewall del servidor SQL (puerto 1433 abierto)
   - TCP/IP habilitado en SQL Server Configuration Manager
   - Servicio de SQL Server reiniciado después de cambios

---

### Error: "No se puede encontrar el keystore" (App Móvil)

**Solución**: Asegúrate de que el archivo `eventos-escat.keystore` esté en la carpeta `AppRegistro` y que la ruta en el comando de publicación sea correcta.

---

### Error: "No se puede conectar a la API" (App Móvil)

**Solución**: 
1. Verifica que la API esté ejecutándose en IIS
2. Verifica que la IP y puerto sean correctos
3. Verifica que el dispositivo/emulador tenga acceso a la red
4. Si usas HTTPS, verifica que el certificado sea válido
5. Verifica la configuración de CORS en la API

---

### Error: "Permiso de cámara denegado" (App Móvil)

**Solución**: 
1. Ve a Configuración del dispositivo → Aplicaciones → AppRegistro → Permisos
2. Habilita el permiso de Cámara

---

## 🔐 Checklist de Seguridad Pre-Producción

Antes de usar el sistema en producción, verifica:

- [ ] JWT SecretKey cambiada a una clave segura y única (mínimo 64 caracteres)
- [ ] Contraseña del usuario `EventosESCATUser` cambiada y segura (mínimo 12 caracteres)
- [ ] Variables de entorno configuradas en `web.config` (no en `appsettings.json`)
- [ ] HTTPS configurado con certificados válidos
- [ ] CORS configurado según necesidades
- [ ] Firewall configurado para permitir solo IPs necesarias
- [ ] Swagger deshabilitado en producción (ya configurado)
- [ ] Usuario administrador inicial creado
- [ ] Backups automáticos de la base de datos configurados
- [ ] Logs revisados (sin errores críticos)
- [ ] Contraseñas del keystore de Android cambiadas (si aplica)

---

## 🔄 Mantenimiento

### Actualizar la API

1. Hacer cambios en el código
2. Publicar nuevamente:
   ```powershell
   cd APIRegistro
   dotnet publish -c Release -o C:\inetpub\wwwroot\EventosESCAT
   ```
3. Reiniciar el sitio en IIS o el Application Pool

### Actualizar la Base de Datos

```powershell
cd APIRegistro
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

### Ver Logs de la API

```powershell
Get-Content "C:\inetpub\wwwroot\EventosESCAT\logs\stdout*.log" -Tail 50
```

### Crear Nuevos Usuarios

**Opción 1: Endpoint de administración (solo en desarrollo)**
```powershell
$body = @{
    user = "nuevo_usuario"
    password = "contraseña_segura"
    email = "usuario@escat.edu.mx"
    role = "User"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://tu-servidor/api/admin/create-user" `
    -Method Post -Body $body -ContentType "application/json"
```

**Opción 2: Directamente en SQL Server**
```sql
USE EventosESCAT;
-- Las contraseñas deben estar hasheadas con BCrypt
-- Es mejor usar el endpoint o el script CreateUser.cs
```

---

## 📚 Documentación Adicional

### Archivos de Referencia

| Archivo | Descripción |
|---------|-------------|
| `APIRegistro/README.md` | Documentación general de la API |
| `APIRegistro/SEGURIDAD.md` | Recomendaciones de seguridad detalladas |
| `APIRegistro/DESPLIEGUE_IIS.md` | Guía paso a paso de despliegue en IIS |
| `APIRegistro/INICIO_RAPIDO_IIS.md` | Checklist rápido de despliegue |
| `APIRegistro/Scripts/README.md` | Documentación de scripts de utilidad |

### Herramientas Útiles

**Scripts PowerShell:**
```powershell
cd APIRegistro\Scripts
. .\DeploymentUtilities.ps1

# Menú interactivo
Start-DeploymentUtilities

# Funciones individuales
New-JWTSecretKey        # Generar JWT Secret Key
New-SecurePassword      # Generar contraseña segura
Test-SQLConnection      # Probar conectividad SQL
Publish-APIRegistro     # Publicar aplicación
Get-APILogs            # Ver logs recientes
```

---

## 📝 Notas Importantes

- **Keystore Android**: Guarda el archivo `eventos-escat.keystore` en un lugar seguro. Si lo pierdes, no podrás actualizar tu aplicación.
- **Contraseñas**: En producción, usa contraseñas seguras (mínimo 12 caracteres, con mayúsculas, minúsculas, números y símbolos).
- **Secretos**: NUNCA subas `web.config` con contraseñas reales al repositorio. Usa variables de entorno.
- **Base de Datos**: Realiza backups regulares de la base de datos `EventosESCAT`.
- **Logs**: Revisa los logs regularmente para detectar problemas o intentos de acceso sospechosos.
- **Actualizaciones**: Mantén .NET, SQL Server e IIS actualizados con los últimos parches de seguridad.

---

## 🆘 Soporte

Si encuentras problemas:

1. Revisa la sección [Solución de Problemas](#-solución-de-problemas)
2. Consulta los logs en `C:\inetpub\wwwroot\EventosESCAT\logs\`
3. Verifica la documentación adicional en `APIRegistro/`
4. Usa los scripts de utilidad en `APIRegistro/Scripts/`

---

**Última actualización**: 2025  
**Versión**: 1.0
