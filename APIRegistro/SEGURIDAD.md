# 🔒 Recomendaciones de Seguridad para la API

## ⚠️ IMPORTANTE: Configuración Inicial Requerida

### 1. Cambiar la Clave Secreta JWT
**CRÍTICO**: La clave secreta en `appsettings.json` es solo un ejemplo. **DEBES cambiarla** antes de usar en producción.

```json
"JWT": {
  "SecretKey": "GENERA_UNA_CLAVE_ALEATORIA_DE_AL_MENOS_64_CARACTERES"
}
```

**Cómo generar una clave segura:**
- Usa un generador de claves aleatorias
- Mínimo 32 caracteres, recomendado 64+
- Combina letras, números y símbolos especiales
- **NUNCA** compartas esta clave públicamente

### 2. Configurar Variables de Entorno
**NO** almacenes secretos en `appsettings.json` en producción. Usa:
- Variables de entorno del sistema
- Azure Key Vault
- Secretos de usuario (desarrollo local)

## 🛡️ Medidas de Seguridad Implementadas

### ✅ Autenticación JWT
- Tokens con expiración configurable
- Validación completa de firma, issuer y audience
- Sin delay de expiración (ClockSkew = 0)

### ✅ Hash de Contraseñas
- Uso de BCrypt con salt automático
- **NUNCA** se almacenan contraseñas en texto plano
- Verificación segura de contraseñas

### ✅ Validación de Entrada
- Validación de modelos con Data Annotations
- Longitud mínima de contraseñas (6 caracteres)
- Validación de formato de usuario

### ✅ CORS Configurado
- Solo origenes permitidos pueden acceder
- Configuración restrictiva por defecto
- Configurado en `appsettings.json` con `AllowedOrigins`

### ✅ Swagger/OpenAPI Seguro
- Swagger solo disponible en modo Development
- Autenticación JWT integrada en Swagger UI
- Soporte para Bearer tokens con formato JWT

### ✅ Logging de Seguridad
- Registro de intentos de login fallidos
- Registro de logins exitosos
- Manejo de errores sin exponer información sensible

## 🔐 Recomendaciones Adicionales de Seguridad

### 1. Base de Datos

#### ✅ Usuario de Base de Datos para IIS (Producción)

Para desplegar en IIS con acceso remoto, se ha creado un script para configurar un usuario específico de SQL Server.

**Ejecutar el Script:**
```bash
# Ubicación: APIRegistro/Scripts/CreateDatabaseUser.sql
# Ejecutar en SQL Server Management Studio conectado a tu servidor
```

**El script crea:**
- **Login**: `EventosESCATUser` con autenticación SQL Server
- **Usuario de BD**: Asociado al login con permisos limitados
- **Permisos**: `db_datareader`, `db_datawriter`, `EXECUTE`

**⚠️ IMPORTANTE - Cambiar la Contraseña:**

El script incluye una contraseña de ejemplo que **DEBES cambiar**:
```sql
-- En el script, cambiar:
CREATE LOGIN [EventosESCATUser] 
WITH PASSWORD = N'TuContraseñaSegura123!', -- ¡CAMBIAR ESTO!
```

**Requisitos de contraseña segura:**
- Mínimo 12 caracteres
- Letras mayúsculas y minúsculas
- Números y caracteres especiales
- No usar palabras comunes

**Ejemplo de contraseña fuerte:**
```
Ev3nt0s#ESCAT@2024$Secure!
```

#### ✅ Cadena de Conexión para IIS

**Desarrollo (Windows Authentication):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=PC-Efra;Database=EventosESCAT;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Producción (SQL Server Authentication - para IIS):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA_SEGURA;TrustServerCertificate=True;Encrypt=True;"
}
```

**Reemplaza:**
- `TU_SERVIDOR`: Nombre del servidor SQL, IP o dominio
  - Ejemplos: `192.168.1.100`, `sql.tudominio.com`, `localhost\SQLEXPRESS`
  - Con puerto específico: `192.168.1.100,1433`
- `TU_CONTRASEÑA_SEGURA`: La contraseña que configuraste para `EventosESCATUser`

**⚠️ NUNCA subas la contraseña al control de versiones**

#### 🌐 Configurar Acceso Remoto a SQL Server

Para que IIS pueda conectarse remotamente a SQL Server, sigue estos pasos:

**1. Habilitar Autenticación Mixta en SQL Server**

En SQL Server Management Studio:
1. Clic derecho en el servidor → **Properties**
2. **Security** → Seleccionar **SQL Server and Windows Authentication mode**
3. **Reiniciar el servicio de SQL Server**

**2. Habilitar TCP/IP**

En SQL Server Configuration Manager:
1. **SQL Server Network Configuration** → **Protocols for [INSTANCIA]**
2. Clic derecho en **TCP/IP** → **Enable**
3. Doble clic en **TCP/IP** → Pestaña **IP Addresses**
4. En **IPAll**, configurar **TCP Port**: `1433`
5. **Reiniciar el servicio de SQL Server**

**3. Configurar Firewall de Windows**

PowerShell (como Administrador):
```powershell
# Permitir puerto 1433 para SQL Server
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow

# Opcional: Restringir solo a IP del servidor IIS
New-NetFirewallRule -DisplayName "SQL Server - IIS Only" -Direction Inbound -Protocol TCP -LocalPort 1433 -RemoteAddress 192.168.1.50 -Action Allow
```

**4. Verificar Conectividad**

Desde el servidor IIS (PowerShell):
```powershell
# Probar conexión al puerto SQL
Test-NetConnection -ComputerName TU_SERVIDOR_SQL -Port 1433
# Si funciona, verás: TcpTestSucceeded : True
```

**Recomendaciones de Seguridad para Acceso Remoto:**
- 🔒 Usa **Encrypt=True** en la cadena de conexión
- 🔒 Configura el firewall para permitir solo la IP del servidor IIS
- 🔒 Usa contraseñas fuertes y únicas
- 🔒 Cambia las contraseñas regularmente (cada 90 días)
- 🔒 Habilita auditoría de SQL Server para registrar conexiones
- 🔒 No uses el usuario `sa` ni usuarios con permisos de administrador
- 🔒 Considera usar una VPN o Azure Private Link para conexiones remotas

**Recomendaciones:**
- ✅ Usar autenticación de Windows (Trusted_Connection=True) es seguro para desarrollo local
- ⚠️ En producción con IIS, usa SQL Server Authentication con el usuario `EventosESCATUser`
- ⚠️ **NO** uses `sa` o usuarios con permisos administrativos
- ✅ Usa conexiones encriptadas (TrustServerCertificate=True en desarrollo, certificados reales en producción)

### 2. Protección de Endpoints

#### Proteger un Controlador Completo:
```csharp
[Authorize] // Requiere autenticación
public class MiController : ControllerBase
{
    // Todos los endpoints requieren token JWT
}
```

#### Proteger un Endpoint Específico:
```csharp
[HttpGet]
[Authorize] // Solo este endpoint requiere autenticación
public IActionResult GetData()
{
    return Ok();
}
```

#### Proteger por Rol:
```csharp
[HttpGet("admin")]
[Authorize(Roles = "Admin")] // Solo administradores
public IActionResult AdminEndpoint()
{
    return Ok();
}
```

#### Permitir Acceso Público (excepción):
```csharp
[HttpGet("public")]
[AllowAnonymous] // No requiere autenticación
public IActionResult PublicEndpoint()
{
    return Ok();
}
```

## 🌐 Despliegue en IIS

### Configuración del Application Pool

1. **Crear Application Pool dedicado:**
   - Nombre: `EventosESCAT_Pool`
   - .NET CLR Version: **No Managed Code** (para .NET 10)
   - Pipeline Mode: **Integrated**
   - Identity: **ApplicationPoolIdentity** (recomendado)

2. **Configurar el Sitio Web:**
   - Physical Path: Carpeta donde publicaste la aplicación
   - Application Pool: `EventosESCAT_Pool`
   - Bindings:
     - HTTP: puerto 80 (o el que desees)
     - HTTPS: puerto 443 con certificado SSL (obligatorio en producción)

### Variables de Entorno en IIS

**Método 1: Editar web.config** (se genera automáticamente al publicar)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <aspNetCore processPath="dotnet" arguments=".\APIRegistro.dll" stdoutLogEnabled="false" hostingModel="inprocess">
      <environmentVariables>
        <environmentVariable name="JWT__SecretKey" value="TU_CLAVE_SECRETA_AQUI" />
        <environmentVariable name="ConnectionStrings__DefaultConnection" value="Server=TU_SERVIDOR;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA;TrustServerCertificate=True;" />
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
```

**⚠️ IMPORTANTE:** Si usas `web.config` para secretos, **NO lo subas a control de versiones**. Agrégalo a `.gitignore`:

```gitignore
# Ignorar archivos con secretos
**/web.config
**/appsettings.Production.json
```

**Método 2: Configuration Editor en IIS Manager**
1. Selecciona tu sitio web en IIS
2. Abre **Configuration Editor**
3. Section: `system.webServer/aspNetCore`
4. Expande `environmentVariables` y agrega las variables

### Publicación desde Visual Studio

```bash
# Desde la terminal o Package Manager Console
dotnet publish -c Release -o C:\inetpub\wwwroot\EventosESCAT
```

**Checklist post-publicación:**
- [ ] Verificar que los archivos DLL estén en la carpeta
- [ ] Configurar variables de entorno con secretos
- [ ] Verificar conectividad a SQL Server
- [ ] Probar endpoints con Postman o curl
- [ ] Verificar logs en caso de errores

### 3. HTTPS en Producción

**CRÍTICO**: Siempre usa HTTPS en producción:
- Configura certificados SSL/TLS válidos
- Redirige HTTP a HTTPS automáticamente (ya configurado)
- Usa HSTS (HTTP Strict Transport Security)

### 4. Rate Limiting

**Recomendado**: Implementa límites de tasa para prevenir ataques:
- Límite de intentos de login por IP
- Límite de requests por usuario
- Considera usar `AspNetCoreRateLimit` NuGet package

### 5. Validación de Entrada

✅ Ya implementado:
- Data Annotations en DTOs
- Validación automática de modelos

**Recomendaciones adicionales:**
- Sanitiza inputs para prevenir SQL Injection (Entity Framework lo hace automáticamente)
- Valida tipos de datos y rangos
- Usa whitelist en lugar de blacklist cuando sea posible

### 6. Manejo de Errores

✅ Ya implementado:
- No se exponen detalles internos en respuestas
- Logging de errores sin información sensible

**Recomendaciones:**
- Usa códigos de estado HTTP apropiados
- No reveles información sobre existencia de usuarios
- Mensajes de error genéricos para el cliente

### 7. Headers de Seguridad

**Recomendado**: Agrega middleware para headers de seguridad:
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

### 8. Auditoría y Monitoreo

**Recomendado**:
- Registra todos los accesos a endpoints sensibles
- Monitorea intentos fallidos de autenticación
- Implementa alertas para actividad sospechosa
- Considera usar Application Insights o similar

### 9. Gestión de Tokens

**Recomendaciones:**
- Tokens con expiración corta (24 horas por defecto)
- Implementa refresh tokens para renovación
- Invalida tokens al hacer logout
- Considera blacklist de tokens revocados

### 10. Seguridad de la Base de Datos

**Recomendaciones:**
- Usa stored procedures para operaciones sensibles
- Implementa backups regulares
- Encripta datos sensibles en la BD
- Usa roles y permisos granulares
- Audita cambios importantes

### 11. Endpoints de Administración

**⚠️ IMPORTANTE**: El `AdminController` está disponible sin autenticación solo para facilitar el desarrollo inicial.

**Recomendaciones para producción:**
- Protege el `AdminController` con `[Authorize(Roles = "Admin")]`
- O elimina completamente los endpoints de administración
- Usa un sistema de gestión de usuarios separado para producción
- Considera usar Azure AD o Identity Server para gestión de usuarios

**Endpoints afectados:**
- `POST /api/admin/create-user` - Crear usuarios
- `POST /api/admin/change-password` - Cambiar contraseñas

## 🚨 Checklist de Seguridad Pre-Producción

- [ ] Cambiar JWT SecretKey a una clave segura y única
- [ ] Mover secretos a variables de entorno o Azure Key Vault
- [ ] Configurar HTTPS con certificados válidos
- [ ] Revisar y ajustar CORS según necesidades
- [ ] **Proteger o eliminar AdminController** (endpoints de administración)
- [ ] Deshabilitar Swagger en producción o protegerlo con autenticación
- [ ] Implementar rate limiting
- [ ] Configurar logging y monitoreo
- [ ] Revisar permisos de base de datos
- [ ] Realizar pruebas de penetración básicas
- [ ] Documentar políticas de seguridad
- [ ] Configurar backups automáticos
- [ ] Establecer procedimientos de respuesta a incidentes
- [ ] Verificar que todas las contraseñas estén hasheadas (nunca en texto plano)
- [ ] Revisar y validar todos los endpoints públicos vs protegidos

## 📚 Recursos Adicionales

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc8725)
- [Swashbuckle/Swagger Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Entity Framework Core Security](https://learn.microsoft.com/en-us/ef/core/security/)

## 🔧 Configuración de Swagger en Producción

Si necesitas Swagger en producción (no recomendado), protégelo:

```csharp
// En Program.cs, cambiar de:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// A:
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    // Agregar autenticación básica o IP whitelist
});
```

**Mejor práctica:** Mantén Swagger solo en Development y usa herramientas como Postman o documentación estática para producción.

