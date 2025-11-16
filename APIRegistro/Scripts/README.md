# ?? Scripts de Utilidad - API Eventos ESCAT

Esta carpeta contiene scripts y herramientas para facilitar la configuración y despliegue de la API.

## ?? Índice de Scripts

| Script | Descripción | Cuándo Usarlo |
|--------|-------------|---------------|
| **CreateDatabaseUser.sql** | Script SQL para crear usuario de BD con acceso remoto | Antes de desplegar en IIS |
| **DeploymentUtilities.ps1** | Utilidades PowerShell interactivas para despliegue | Durante todo el proceso de despliegue |
| **CreateUser.cs** | Código C# para crear usuarios de aplicación | Para crear usuarios iniciales |

---

## ??? CreateDatabaseUser.sql

**Propósito:** Crear un usuario de SQL Server con permisos limitados para que la aplicación se conecte de forma segura.

### Qué hace este script:

1. ? Crea el login `EventosESCATUser` con autenticación SQL Server
2. ? Crea el usuario de base de datos asociado
3. ? Asigna permisos mínimos necesarios:
   - `db_datareader` - Leer datos
   - `db_datawriter` - Escribir datos
   - `EXECUTE` - Ejecutar stored procedures
4. ? Verifica que todo se creó correctamente

### Cómo usarlo:

1. **Abre SQL Server Management Studio**
2. **Conéctate al servidor** donde está la base de datos `EventosESCAT`
3. **Abre el archivo** `CreateDatabaseUser.sql`
4. **?? IMPORTANTE: Cambia la contraseña** en la línea:
   ```sql
   WITH PASSWORD = N'TuContraseñaSegura123!',  -- CAMBIAR ESTO
   ```
   Usa una contraseña fuerte (ver recomendaciones abajo)
5. **Ejecuta el script completo** (F5 o botón Execute)
6. **Verifica los mensajes** en la ventana de resultados

### Recomendaciones de contraseña:

- ? Mínimo 12 caracteres
- ? Incluir mayúsculas, minúsculas, números y símbolos
- ? No usar palabras del diccionario
- ? Ejemplo: `Ev3nt0s#ESCAT@2024$Secure!`

**?? Tip:** Usa el script PowerShell para generar una contraseña segura:
```powershell
. .\DeploymentUtilities.ps1
New-SecurePassword
```

### Después de ejecutar el script:

- Guarda la contraseña en un lugar seguro
- Úsala en la cadena de conexión de `web.config`
- No la subas al repositorio

---

## ?? DeploymentUtilities.ps1

**Propósito:** Herramientas PowerShell interactivas para automatizar tareas comunes de despliegue.

### Funciones disponibles:

#### 1. Menú Interactivo
```powershell
# Cargar el script
. .\DeploymentUtilities.ps1

# Iniciar menú
Start-DeploymentUtilities
```

**Opciones del menú:**
1. Generar JWT Secret Key
2. Generar Contraseña Segura (para SQL Server)
3. Probar Conectividad a SQL Server
4. Crear Regla de Firewall para SQL Server
5. Publicar Aplicación
6. Ver Logs Recientes
7. Reiniciar IIS
8. Todas las utilidades de seguridad (JWT + Contraseña)
9. Salir

#### 2. Uso Directo de Funciones

**Generar JWT Secret Key:**
```powershell
New-JWTSecretKey
# Genera clave de 64 caracteres y la copia al portapapeles
# Usa esta clave en web.config como JWT__SecretKey
```

**Generar Contraseña Segura:**
```powershell
New-SecurePassword
# Genera contraseña de 16 caracteres con mayúsculas, minúsculas, números y símbolos
# Usa esta contraseña para el usuario EventosESCATUser en SQL Server
```

**Probar Conectividad a SQL Server:**
```powershell
Test-SQLConnection -ServerName "192.168.1.100" -Port 1433
# Verifica que puedes conectarte al servidor SQL
# Útil para diagnosticar problemas de red o firewall
```

**Crear Regla de Firewall:**
```powershell
# Permitir desde cualquier IP (requiere PowerShell como Admin)
New-SQLFirewallRule

# Permitir solo desde IP específica (más seguro)
New-SQLFirewallRule -RemoteIP "192.168.1.50"
```

**Publicar Aplicación:**
```powershell
Publish-APIRegistro -OutputPath "C:\inetpub\wwwroot\EventosESCAT"
# Compila y publica la aplicación en la carpeta especificada
# Equivalente a: dotnet publish -c Release -o [ruta]
```

**Ver Logs Recientes:**
```powershell
Get-APILogs -LogPath "C:\inetpub\wwwroot\EventosESCAT\logs"
# Muestra las últimas 50 líneas del log más reciente
# Útil para diagnosticar errores en IIS
```

**Reiniciar IIS:**
```powershell
Restart-IISServer
# Equivalente a ejecutar: iisreset
# Requiere PowerShell como Administrador
```

### Requisitos:

- PowerShell 5.1 o superior
- Windows Server o Windows con IIS
- Algunas funciones requieren permisos de administrador:
  - `New-SQLFirewallRule`
  - `Restart-IISServer`

### Ejemplos de uso típico:

**Configuración inicial completa:**
```powershell
# 1. Cargar script
. .\DeploymentUtilities.ps1

# 2. Generar secretos
$jwt = New-JWTSecretKey          # Copia el resultado
$password = New-SecurePassword    # Copia el resultado

# 3. Modificar y ejecutar CreateDatabaseUser.sql con la contraseña

# 4. Probar conectividad
Test-SQLConnection -ServerName "192.168.1.100"

# 5. Crear regla de firewall (como Admin)
New-SQLFirewallRule -RemoteIP "192.168.1.50"

# 6. Publicar aplicación
Publish-APIRegistro -OutputPath "C:\inetpub\wwwroot\EventosESCAT"

# 7. Configurar web.config con JWT y contraseña

# 8. Reiniciar IIS (como Admin)
Restart-IISServer
```

**Troubleshooting:**
```powershell
# Probar conectividad
Test-SQLConnection -ServerName "tu-servidor"

# Ver logs si hay errores
Get-APILogs -LogPath "C:\inetpub\wwwroot\EventosESCAT\logs"

# Reiniciar IIS si cambias configuración
Restart-IISServer
```

---

## ?? CreateUser.cs

**Propósito:** Código C# de ejemplo para crear usuarios de aplicación desde código.

### Cuándo usarlo:

- Para crear el usuario administrador inicial
- Para crear usuarios programáticamente en lugar de usar el endpoint
- Para scripts de migración de datos

### Cómo usarlo:

1. **Opción A: Endpoint de Admin (recomendado para desarrollo)**
   ```bash
   POST /api/admin/create-user
   Content-Type: application/json
   
   {
     "user": "admin",
     "password": "contraseña_segura",
     "email": "admin@escat.edu.mx",
     "role": "Admin"
   }
   ```

2. **Opción B: Código directo**
   - El código en `CreateUser.cs` muestra cómo crear usuarios
   - Útil para entender cómo funciona internamente
   - Puedes adaptarlo para scripts de migración

### ?? Importante:

- En producción, **protege o elimina** el endpoint `/api/admin/create-user`
- Usa contraseñas fuertes para usuarios administradores
- Las contraseñas se hashean con BCrypt automáticamente

---

## ?? Mejores Prácticas

### Seguridad:

1. **Contraseñas:**
   - Genera contraseñas con `New-SecurePassword`
   - Mínimo 12 caracteres
   - Combina mayúsculas, minúsculas, números y símbolos
   - Cambia cada 90 días

2. **JWT Secret Keys:**
   - Genera con `New-JWTSecretKey`
   - Mínimo 64 caracteres
   - Única para cada entorno (dev, staging, prod)
   - Nunca la compartas públicamente

3. **Scripts SQL:**
   - Ejecuta solo en servidores de confianza
   - Verifica los permisos que otorgas
   - Usa el principio de mínimo privilegio

4. **Firewall:**
   - Restringe acceso solo a IPs necesarias
   - No abras puertos innecesarios
   - Monitorea intentos de conexión

### Organización:

1. **Antes de desplegar:**
   - [ ] Ejecutar `CreateDatabaseUser.sql`
   - [ ] Generar JWT Secret Key
   - [ ] Generar contraseña para SQL
   - [ ] Probar conectividad

2. **Durante el despliegue:**
   - [ ] Publicar aplicación
   - [ ] Configurar web.config
   - [ ] Crear Application Pool
   - [ ] Crear sitio en IIS

3. **Después de desplegar:**
   - [ ] Probar endpoints
   - [ ] Verificar logs
   - [ ] Crear usuario admin inicial
   - [ ] Configurar HTTPS

---

## ?? Documentación Relacionada

- **DESPLIEGUE_IIS.md** - Guía paso a paso completa
- **SEGURIDAD.md** - Recomendaciones de seguridad detalladas
- **INICIO_RAPIDO_IIS.md** - Checklist rápido
- **README.md** - Documentación general de la API

---

## ?? Ayuda y Soporte

**Problemas comunes:**

| Problema | Script para Diagnosticar |
|----------|-------------------------|
| No conecta a SQL | `Test-SQLConnection -ServerName "IP"` |
| Error al publicar | Revisar logs de `dotnet publish` |
| Firewall bloqueando | `Test-NetConnection -ComputerName "IP" -Port 1433` |
| IIS no responde | `Get-APILogs -LogPath "ruta\logs"` |

**Necesitas ayuda adicional:**
- Consulta los archivos `.md` en la carpeta raíz de `APIRegistro`
- Revisa los logs en la carpeta `logs\` de la aplicación publicada
- Verifica que todos los prerrequisitos estén instalados

---

**Última actualización:** 2024  
**Versión:** 1.0
