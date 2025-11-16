# ?? Inicio Rápido - Despliegue en IIS

Esta es una guía ultra rápida para desplegar la API. Para detalles completos, consulta `DESPLIEGUE_IIS.md`.

## ? Checklist Rápido

### 1?? SQL Server (en el servidor de base de datos)

```powershell
# 1. Ejecutar Scripts/CreateDatabaseUser.sql en SQL Server Management Studio
#    ?? CAMBIAR LA CONTRASEÑA PRIMERO

# 2. Habilitar autenticación mixta (requiere reinicio de SQL Server)
#    SQL Server Properties ? Security ? "SQL Server and Windows Authentication mode"

# 3. Habilitar TCP/IP (requiere reinicio de SQL Server)
#    SQL Server Configuration Manager ? Protocols ? TCP/IP ? Enable

# 4. Configurar Firewall
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow

# 5. Reiniciar SQL Server
# Services ? SQL Server (MSSQLSERVER) ? Restart
```

### 2?? Servidor IIS

```powershell
# 1. Instalar .NET 10 Hosting Bundle
# Descargar de: https://dotnet.microsoft.com/download/dotnet/10.0

# 2. Reiniciar IIS
iisreset
```

### 3?? Generar Secretos (usa nuestro script)

```powershell
# Navega a la carpeta del proyecto
cd APIRegistro\Scripts

# Carga el script
. .\DeploymentUtilities.ps1

# Inicia el menú - opción 8 genera todo
Start-DeploymentUtilities

# O usa las funciones directamente:
New-JWTSecretKey        # Copia el resultado
New-SecurePassword      # Copia el resultado
```

### 4?? Publicar Aplicación

```powershell
# Opción A: Usar el script
Publish-APIRegistro -OutputPath "C:\inetpub\wwwroot\EventosESCAT"

# Opción B: Manual
dotnet publish -c Release -o "C:\inetpub\wwwroot\EventosESCAT"
```

### 5?? Configurar web.config

```powershell
# 1. Copia web.config.example a la carpeta publicada
Copy-Item "APIRegistro\web.config.example" "C:\inetpub\wwwroot\EventosESCAT\web.config"

# 2. Edita C:\inetpub\wwwroot\EventosESCAT\web.config
# 3. Reemplaza:
#    - TU_SERVIDOR_SQL ? IP o nombre del servidor SQL
#    - TU_CONTRASEÑA ? Contraseña generada en el paso 3
#    - GENERA_UNA_CLAVE... ? JWT Secret Key del paso 3
```

**Ejemplo de valores en web.config:**
```xml
<environmentVariable name="ConnectionStrings__DefaultConnection" 
    value="Server=192.168.1.100;Database=EventosESCAT;User Id=EventosESCATUser;Password=Ev3nt0s#SQL@2024;TrustServerCertificate=True;" />

<environmentVariable name="JWT__SecretKey" 
    value="aB3dEfGhIjKlMnOpQrStUvWxYz0123456789aBcDeFgHiJkLmNoPqRsTuVwXyZ01" />
```

### 6?? Configurar IIS

```powershell
# 1. Abrir IIS Manager (inetmgr)

# 2. Crear Application Pool
#    - Name: EventosESCAT_Pool
#    - .NET CLR: No Managed Code
#    - Pipeline: Integrated

# 3. Crear Sitio Web
#    - Name: EventosESCAT
#    - App Pool: EventosESCAT_Pool
#    - Path: C:\inetpub\wwwroot\EventosESCAT
#    - Binding: HTTP:80 (o HTTPS:443 con certificado)

# 4. Iniciar el sitio
```

### 7?? Verificar

```powershell
# Probar endpoint
Invoke-RestMethod -Uri "http://localhost/api/auth/login" -Method Post `
    -Body '{"user":"admin","password":"tu_contraseña"}' `
    -ContentType "application/json"

# Ver logs si hay errores
Get-APILogs -LogPath "C:\inetpub\wwwroot\EventosESCAT\logs"
```

## ?? Solución Rápida de Problemas

| Error | Solución Rápida |
|-------|----------------|
| Error 500.19 | Instalar .NET 10 Hosting Bundle + `iisreset` |
| Error 502.5 | Ver logs en `C:\inetpub\wwwroot\EventosESCAT\logs\` |
| No conecta a SQL | `Test-SQLConnection -ServerName "IP_SQL" -Port 1433` |
| Login failed | Verificar contraseña en web.config |
| Cannot open DB | Ejecutar `CreateDatabaseUser.sql` |

## ?? Documentación Completa

- **DESPLIEGUE_IIS.md** - Guía detallada paso a paso
- **SEGURIDAD.md** - Configuración de seguridad y variables de entorno
- **README.md** - Documentación general de la API

## ?? Herramientas Útiles

```powershell
# Verificar que .NET 10 está instalado
dotnet --list-runtimes

# Probar conectividad a SQL
Test-SQLConnection -ServerName "192.168.1.100"

# Ver estado de IIS
Get-Website | Select-Object Name, State, PhysicalPath

# Reiniciar sitio específico
Restart-WebAppPool -Name "EventosESCAT_Pool"

# Ver logs recientes
Get-Content "C:\inetpub\wwwroot\EventosESCAT\logs\stdout*.log" -Tail 50
```

## ?? Recordatorios de Seguridad

- ? Cambiar la contraseña del script SQL antes de ejecutar
- ? Generar JWT Secret Key única (mínimo 64 caracteres)
- ? NO subir web.config al repositorio
- ? Configurar HTTPS con certificado SSL en producción
- ? Restringir firewall a IPs específicas si es posible
- ? Revisar logs regularmente
- ? Cambiar contraseñas cada 90 días

---

**¿Necesitas más ayuda?**
- Consulta `DESPLIEGUE_IIS.md` para guía detallada
- Usa `Start-DeploymentUtilities` para menú interactivo
- Revisa `SEGURIDAD.md` para recomendaciones de seguridad

**Última actualización:** 2024  
**Versión:** 1.0
