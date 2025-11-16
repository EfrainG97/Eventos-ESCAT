# ?? Guía Rápida de Despliegue en IIS

Esta es una guía paso a paso para desplegar la API de Eventos ESCAT en IIS con acceso remoto a SQL Server.

## ?? Prerrequisitos

- Windows Server o Windows con IIS instalado
- SQL Server (puede estar en un servidor diferente)
- .NET 10 Hosting Bundle instalado en el servidor IIS
- Acceso administrativo al servidor IIS y SQL Server

---

## ??? PASO 1: Configurar SQL Server

### 1.1 Crear el Usuario de Base de Datos

1. Abre **SQL Server Management Studio**
2. Conéctate al servidor SQL donde está la base de datos `EventosESCAT`
3. Abre el archivo `Scripts/CreateDatabaseUser.sql`
4. **?? IMPORTANTE**: Cambia la contraseña en la línea:
   ```sql
   WITH PASSWORD = N'TuContraseñaSegura123!',  -- CAMBIAR ESTO
   ```
   Por una contraseña fuerte, ejemplo:
   ```sql
   WITH PASSWORD = N'Ev3nt0s#ESCAT@2024$Secure!',
   ```
5. Ejecuta el script completo (F5)
6. Verifica que el usuario se creó correctamente (el script muestra mensajes de confirmación)

### 1.2 Habilitar Autenticación Mixta

**Opción A: Desde SQL Server Management Studio**
1. Clic derecho en el servidor ? **Properties**
2. **Security** ? Seleccionar **"SQL Server and Windows Authentication mode"**
3. Click **OK**
4. **Reiniciar el servicio de SQL Server**

**Opción B: Desde Services (servicemsc)**
1. Presiona `Win + R`, escribe `services.msc`, Enter
2. Busca **SQL Server (MSSQLSERVER)** o tu instancia
3. Clic derecho ? **Restart**

### 1.3 Habilitar TCP/IP

1. Abre **SQL Server Configuration Manager**
2. Ve a **SQL Server Network Configuration** ? **Protocols for [TU_INSTANCIA]**
3. Clic derecho en **TCP/IP** ? **Enable**
4. Doble clic en **TCP/IP** ? Pestaña **IP Addresses**
5. Desplázate hasta **IPAll**
6. Configura:
   - **TCP Dynamic Ports**: (dejar vacío)
   - **TCP Port**: `1433`
7. Clic **OK**
8. **Reiniciar el servicio de SQL Server**

### 1.4 Configurar Firewall

**PowerShell (como Administrador):**
```powershell
# Permitir puerto 1433 para SQL Server
New-NetFirewallRule -DisplayName "SQL Server - Puerto 1433" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow

# Opcional: Permitir SQL Browser (para instancias nombradas)
New-NetFirewallRule -DisplayName "SQL Server Browser" -Direction Inbound -Protocol UDP -LocalPort 1434 -Action Allow
```

**Firewall GUI (Windows Defender):**
1. Panel de Control ? **Windows Defender Firewall** ? **Configuración avanzada**
2. **Reglas de entrada** ? **Nueva regla...**
3. Tipo: **Puerto** ? Siguiente
4. **TCP**, Puerto específico: `1433` ? Siguiente
5. **Permitir la conexión** ? Siguiente
6. Seleccionar todos los perfiles ? Siguiente
7. Nombre: `SQL Server - Puerto 1433` ? Finalizar

### 1.5 Verificar Conectividad

**Desde el servidor IIS (PowerShell):**
```powershell
# Reemplaza TU_SERVIDOR_SQL con la IP o nombre del servidor SQL
Test-NetConnection -ComputerName TU_SERVIDOR_SQL -Port 1433

# Si funciona, deberías ver:
# TcpTestSucceeded : True
```

---

## ?? PASO 2: Instalar ASP.NET Core Hosting Bundle

1. Descarga el **.NET 10 Hosting Bundle** desde:
   - https://dotnet.microsoft.com/download/dotnet/10.0
   - Busca "Hosting Bundle" en la sección de "Run apps"

2. Ejecuta el instalador como administrador

3. **Reiniciar IIS** después de la instalación:
   ```powershell
   # PowerShell como Administrador
   iisreset
   ```

4. Verificar instalación:
   ```powershell
   # Verificar que .NET 10 está instalado
   dotnet --list-runtimes
   ```

---

## ?? PASO 3: Publicar la Aplicación

### 3.1 Publicar desde Visual Studio

**Opción A: Publicar Localmente**
1. En Visual Studio, clic derecho en el proyecto **APIRegistro**
2. Selecciona **Publish...**
3. Target: **Folder**
4. Location: `C:\Publish\EventosESCAT` (o la ruta que prefieras)
5. Configuration: **Release**
6. Target Framework: **net10.0**
7. Clic **Publish**

**Opción B: Desde la Terminal**
```bash
# Desde la carpeta del proyecto APIRegistro
dotnet publish -c Release -o C:\Publish\EventosESCAT
```

### 3.2 Copiar Archivos al Servidor IIS

Si publicaste en tu PC local y el IIS está en otro servidor:
1. Copia la carpeta completa `C:\Publish\EventosESCAT`
2. Pégala en el servidor IIS, por ejemplo en: `C:\inetpub\wwwroot\EventosESCAT`

---

## ?? PASO 4: Configurar IIS

### 4.1 Crear Application Pool

1. Abre **IIS Manager** (`inetmgr`)
2. En el panel izquierdo, expande el servidor
3. Clic derecho en **Application Pools** ? **Add Application Pool...**
4. Configuración:
   - **Name**: `EventosESCAT_Pool`
   - **.NET CLR version**: **No Managed Code**
   - **Managed pipeline mode**: **Integrated**
5. Clic **OK**
6. Clic derecho en el nuevo pool ? **Advanced Settings...**
7. Verifica:
   - **Start Mode**: `AlwaysRunning` (opcional, mejora rendimiento)
   - **Identity**: `ApplicationPoolIdentity` (recomendado)
8. Clic **OK**

### 4.2 Crear el Sitio Web

1. En **IIS Manager**, clic derecho en **Sites** ? **Add Website...**
2. Configuración:
   - **Site name**: `EventosESCAT`
   - **Application pool**: `EventosESCAT_Pool`
   - **Physical path**: `C:\inetpub\wwwroot\EventosESCAT` (donde copiaste los archivos)
   - **Binding**:
     - Type: `http`
     - IP address: `All Unassigned`
     - Port: `80` (o el puerto que desees, ej: 8080)
     - Host name: (dejar vacío o agregar tu dominio)
3. Clic **OK**

### 4.3 Configurar HTTPS (Recomendado)

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
New-SelfSignedCertificate -DnsName "localhost", "tu-servidor" -CertStoreLocation "cert:\LocalMachine\My"
```
Luego asigna este certificado en IIS como se indicó arriba.

---

## ?? PASO 5: Configurar Secretos (Cadena de Conexión y JWT)

**?? NUNCA incluyas contraseñas en archivos que subirás al repositorio**

### Método 1: Editar web.config (Recomendado)

1. Ve a la carpeta publicada: `C:\inetpub\wwwroot\EventosESCAT`
2. Abre o crea el archivo `web.config`
3. Agrega las variables de entorno:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\APIRegistro.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess">
        <environmentVariables>
          <!-- Cadena de conexión -->
          <environmentVariable name="ConnectionStrings__DefaultConnection" 
                               value="Server=TU_SERVIDOR_SQL;Database=EventosESCAT;User Id=EventosESCATUser;Password=TU_CONTRASEÑA;TrustServerCertificate=True;Encrypt=True;" />
          
          <!-- JWT Secret Key -->
          <environmentVariable name="JWT__SecretKey" 
                               value="TU_CLAVE_SECRETA_JWT_DE_AL_MENOS_32_CARACTERES" />
          
          <!-- Entorno de producción -->
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" 
                               value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

4. **Reemplaza:**
   - `TU_SERVIDOR_SQL`: IP o nombre del servidor SQL (ej: `192.168.1.100` o `sql.tudominio.com`)
   - `TU_CONTRASEÑA`: La contraseña que configuraste para `EventosESCATUser`
   - `TU_CLAVE_SECRETA_JWT_DE_AL_MENOS_32_CARACTERES`: Genera una clave segura (ver más abajo)

5. Guarda el archivo

### Generar JWT Secret Key Segura

**PowerShell:**
```powershell
# Genera una clave aleatoria de 64 caracteres
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

**Copia la salida y úsala como JWT Secret Key**

### Método 2: Configuration Editor en IIS (Alternativa)

1. En **IIS Manager**, selecciona tu sitio **EventosESCAT**
2. Doble clic en **Configuration Editor**
3. En la parte superior:
   - Section: `system.webServer/aspNetCore`
4. Expande `environmentVariables`
5. Agrega las variables como en el XML anterior
6. Clic **Apply** en el panel derecho

---

## ?? PASO 6: Probar la Aplicación

### 6.1 Iniciar el Sitio

1. En **IIS Manager**, selecciona tu sitio **EventosESCAT**
2. En el panel derecho, clic en **Browse *:80 (http)** o **Browse *:443 (https)**
3. Debería abrir el navegador

### 6.2 Probar Endpoints

**Opción A: Usar el navegador**
- Si dejaste Swagger habilitado, ve a: `http://tu-servidor/swagger`
- **Nota**: Por seguridad, Swagger está deshabilitado en modo Production

**Opción B: Usar PowerShell**
```powershell
# Probar endpoint de login
$body = @{
    user = "admin"
    password = "tu_contraseña"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://tu-servidor/api/auth/login" `
                  -Method Post `
                  -Body $body `
                  -ContentType "application/json"
```

**Opción C: Usar Postman**
1. Crea una nueva request: `POST http://tu-servidor/api/auth/login`
2. Body ? raw ? JSON:
   ```json
   {
     "user": "admin",
     "password": "tu_contraseña"
   }
   ```
3. Send

Si recibes un token JWT, ¡la API está funcionando! ??

### 6.3 Revisar Logs en Caso de Errores

**Ubicación de logs:**
```
C:\inetpub\wwwroot\EventosESCAT\logs\
```

**Ver logs recientes (PowerShell):**
```powershell
Get-ChildItem "C:\inetpub\wwwroot\EventosESCAT\logs\" -Filter "stdout*.log" | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object -First 1 | 
    Get-Content -Tail 50
```

---

## ? Solución de Problemas

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
3. Revisa los logs en la carpeta `logs\`
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
   -- Verificar que el usuario existe
   USE EventosESCAT;
   SELECT name FROM sys.database_principals WHERE name = 'EventosESCATUser';
   
   -- Si no existe, ejecuta el script completo
   ```

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

### La aplicación funciona pero no puede autenticar usuarios

**Causa**: JWT SecretKey no configurada

**Solución**:
1. Verifica que `web.config` tenga la variable `JWT__SecretKey`
2. La clave debe tener al menos 32 caracteres
3. Reinicia el sitio en IIS

---

## ? Checklist Final

Antes de considerar el despliegue completo:

- [ ] Usuario `EventosESCATUser` creado en SQL Server con contraseña segura
- [ ] Autenticación mixta habilitada en SQL Server
- [ ] TCP/IP habilitado y puerto 1433 configurado
- [ ] Firewall configurado para permitir puerto 1433
- [ ] Conectividad verificada desde servidor IIS a SQL Server
- [ ] .NET 10 Hosting Bundle instalado en servidor IIS
- [ ] Aplicación publicada y copiada al servidor
- [ ] Application Pool creado con "No Managed Code"
- [ ] Sitio web creado en IIS
- [ ] HTTPS configurado con certificado SSL (producción)
- [ ] Variables de entorno configuradas en `web.config`
- [ ] Cadena de conexión con SQL Authentication
- [ ] JWT SecretKey configurada (mínimo 32 caracteres)
- [ ] Endpoint de login probado y funcionando
- [ ] Logs revisados (sin errores críticos)
- [ ] Usuario inicial de administración creado

---

## ?? Seguridad Post-Despliegue

1. **Cambiar contraseñas regularmente** (cada 90 días)
2. **Monitorear logs** para intentos de acceso sospechosos
3. **Habilitar auditoría** en SQL Server
4. **Restringir firewall** a IPs específicas si es posible
5. **Configurar backups** automáticos de la base de datos
6. **Actualizar certificado SSL** antes de que expire
7. **Aplicar actualizaciones** de seguridad de Windows y SQL Server

---

## ?? Recursos Adicionales

- [Documentación oficial de IIS](https://learn.microsoft.com/en-us/iis/)
- [Despliegue de ASP.NET Core en IIS](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/)
- [SQL Server Configuration Manager](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-configuration-manager)
- Archivo `SEGURIDAD.md` para recomendaciones detalladas

---

**¿Necesitas ayuda?** Consulta los archivos `README.md` y `SEGURIDAD.md` para información adicional.

**Última actualización**: 2024  
**Versión**: 1.0
