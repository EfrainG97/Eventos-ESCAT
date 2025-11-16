# =============================================================================
# Script de Utilidades para Despliegue de API Eventos ESCAT
# =============================================================================
# Este script proporciona funciones útiles para facilitar el despliegue
# y configuración de la API en IIS
#
# REQUISITOS:
# - Ejecutar como Administrador
# - PowerShell 5.1 o superior
# =============================================================================

# Función para generar JWT Secret Key segura
function New-JWTSecretKey {
    param(
        [int]$Length = 64
    )
    
    Write-Host "`n=== Generador de JWT Secret Key ===" -ForegroundColor Cyan
    Write-Host "Generando clave aleatoria de $Length caracteres..." -ForegroundColor Yellow
    
    $key = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count $Length | ForEach-Object {[char]$_})
    
    Write-Host "`nClave generada:" -ForegroundColor Green
    Write-Host $key -ForegroundColor White
    Write-Host "`n??  Guarda esta clave en un lugar seguro" -ForegroundColor Yellow
    Write-Host "??  Úsala en web.config como JWT__SecretKey" -ForegroundColor Yellow
    
    # Copiar al clipboard si está disponible
    try {
        Set-Clipboard -Value $key
        Write-Host "? Clave copiada al portapapeles" -ForegroundColor Green
    } catch {
        Write-Host "??  No se pudo copiar al portapapeles automáticamente" -ForegroundColor Gray
    }
    
    return $key
}

# Función para generar contraseña segura
function New-SecurePassword {
    param(
        [int]$Length = 16
    )
    
    Write-Host "`n=== Generador de Contraseña Segura ===" -ForegroundColor Cyan
    Write-Host "Generando contraseña de $Length caracteres..." -ForegroundColor Yellow
    
    # Asegurar que tenga al menos uno de cada tipo
    $lowercase = (97..122) | Get-Random -Count 4 | ForEach-Object {[char]$_}
    $uppercase = (65..90) | Get-Random -Count 4 | ForEach-Object {[char]$_}
    $numbers = (48..57) | Get-Random -Count 4 | ForEach-Object {[char]$_}
    $specials = "!@#$%^&*".ToCharArray() | Get-Random -Count 4
    
    # Completar hasta el largo deseado
    $remaining = $Length - 16
    if ($remaining -gt 0) {
        $all = ((65..90) + (97..122) + (48..57) + "!@#$%^&*".ToCharArray()) | Get-Random -Count $remaining
    } else {
        $all = @()
    }
    
    # Mezclar todo
    $password = ($lowercase + $uppercase + $numbers + $specials + $all) | Get-Random -Count $Length
    $passwordString = -join $password
    
    Write-Host "`nContraseña generada:" -ForegroundColor Green
    Write-Host $passwordString -ForegroundColor White
    Write-Host "`n??  Guarda esta contraseña en un lugar seguro" -ForegroundColor Yellow
    Write-Host "??  Úsala para el usuario EventosESCATUser en SQL Server" -ForegroundColor Yellow
    
    # Copiar al clipboard si está disponible
    try {
        Set-Clipboard -Value $passwordString
        Write-Host "? Contraseña copiada al portapapeles" -ForegroundColor Green
    } catch {
        Write-Host "??  No se pudo copiar al portapapeles automáticamente" -ForegroundColor Gray
    }
    
    return $passwordString
}

# Función para probar conectividad a SQL Server
function Test-SQLConnection {
    param(
        [Parameter(Mandatory=$true)]
        [string]$ServerName,
        [int]$Port = 1433
    )
    
    Write-Host "`n=== Probando Conectividad a SQL Server ===" -ForegroundColor Cyan
    Write-Host "Servidor: $ServerName" -ForegroundColor Yellow
    Write-Host "Puerto: $Port" -ForegroundColor Yellow
    
    $result = Test-NetConnection -ComputerName $ServerName -Port $Port -WarningAction SilentlyContinue
    
    if ($result.TcpTestSucceeded) {
        Write-Host "`n? Conexión exitosa al puerto $Port" -ForegroundColor Green
        Write-Host "El servidor SQL está accesible" -ForegroundColor Green
    } else {
        Write-Host "`n? No se pudo conectar al puerto $Port" -ForegroundColor Red
        Write-Host "`nPosibles causas:" -ForegroundColor Yellow
        Write-Host "  1. TCP/IP no está habilitado en SQL Server" -ForegroundColor White
        Write-Host "  2. El firewall está bloqueando el puerto" -ForegroundColor White
        Write-Host "  3. El servidor no está ejecutándose" -ForegroundColor White
        Write-Host "  4. El nombre del servidor es incorrecto" -ForegroundColor White
    }
    
    return $result.TcpTestSucceeded
}

# Función para configurar regla de firewall
function New-SQLFirewallRule {
    param(
        [string]$RemoteIP = $null
    )
    
    Write-Host "`n=== Configurando Regla de Firewall para SQL Server ===" -ForegroundColor Cyan
    
    # Verificar si se ejecuta como administrador
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    
    if (-not $isAdmin) {
        Write-Host "? Este comando requiere privilegios de administrador" -ForegroundColor Red
        Write-Host "   Ejecuta PowerShell como Administrador y vuelve a intentar" -ForegroundColor Yellow
        return
    }
    
    try {
        if ($RemoteIP) {
            # Regla específica para una IP
            New-NetFirewallRule -DisplayName "SQL Server - IIS ($RemoteIP)" `
                -Direction Inbound -Protocol TCP -LocalPort 1433 `
                -RemoteAddress $RemoteIP -Action Allow -ErrorAction Stop
            Write-Host "? Regla creada para permitir acceso desde $RemoteIP" -ForegroundColor Green
        } else {
            # Regla general
            New-NetFirewallRule -DisplayName "SQL Server - Puerto 1433" `
                -Direction Inbound -Protocol TCP -LocalPort 1433 `
                -Action Allow -ErrorAction Stop
            Write-Host "? Regla creada para permitir acceso desde cualquier IP" -ForegroundColor Green
            Write-Host "??  Considera restringir a IPs específicas para mayor seguridad" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "? Error al crear regla: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Función para publicar la aplicación
function Publish-APIRegistro {
    param(
        [Parameter(Mandatory=$true)]
        [string]$OutputPath,
        [string]$Configuration = "Release"
    )
    
    Write-Host "`n=== Publicando API Registro ===" -ForegroundColor Cyan
    Write-Host "Configuración: $Configuration" -ForegroundColor Yellow
    Write-Host "Destino: $OutputPath" -ForegroundColor Yellow
    
    # Buscar el archivo .csproj
    $projectPath = Get-ChildItem -Path "." -Filter "APIRegistro.csproj" -Recurse | Select-Object -First 1
    
    if (-not $projectPath) {
        Write-Host "? No se encontró el archivo APIRegistro.csproj" -ForegroundColor Red
        Write-Host "   Asegúrate de ejecutar este script desde la raíz del proyecto" -ForegroundColor Yellow
        return
    }
    
    Write-Host "`nArchivo del proyecto: $($projectPath.FullName)" -ForegroundColor Gray
    
    try {
        # Publicar
        dotnet publish $projectPath.FullName -c $Configuration -o $OutputPath
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "`n? Publicación exitosa" -ForegroundColor Green
            Write-Host "   Archivos publicados en: $OutputPath" -ForegroundColor Green
            
            # Verificar que existe web.config.example
            $webConfigExample = Join-Path $projectPath.DirectoryName "web.config.example"
            if (Test-Path $webConfigExample) {
                Write-Host "`n??  Recuerda configurar web.config con tus secretos:" -ForegroundColor Yellow
                Write-Host "   1. Copia web.config.example a $OutputPath" -ForegroundColor White
                Write-Host "   2. Renómbralo a web.config" -ForegroundColor White
                Write-Host "   3. Edita y reemplaza los valores de ejemplo" -ForegroundColor White
            }
        } else {
            Write-Host "`n? Error durante la publicación" -ForegroundColor Red
        }
    } catch {
        Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Función para mostrar logs recientes
function Get-APILogs {
    param(
        [Parameter(Mandatory=$true)]
        [string]$LogPath,
        [int]$Lines = 50
    )
    
    Write-Host "`n=== Logs Recientes de la API ===" -ForegroundColor Cyan
    
    if (-not (Test-Path $LogPath)) {
        Write-Host "? No se encontró la carpeta de logs: $LogPath" -ForegroundColor Red
        return
    }
    
    $latestLog = Get-ChildItem $LogPath -Filter "stdout*.log" | 
        Sort-Object LastWriteTime -Descending | 
        Select-Object -First 1
    
    if ($latestLog) {
        Write-Host "Archivo: $($latestLog.Name)" -ForegroundColor Yellow
        Write-Host "Última modificación: $($latestLog.LastWriteTime)" -ForegroundColor Yellow
        Write-Host "`nÚltimas $Lines líneas:" -ForegroundColor Gray
        Write-Host "?????????????????????????????????????????" -ForegroundColor Gray
        Get-Content $latestLog.FullName -Tail $Lines
    } else {
        Write-Host "? No se encontraron archivos de log" -ForegroundColor Red
    }
}

# Función para reiniciar IIS
function Restart-IISServer {
    Write-Host "`n=== Reiniciando IIS ===" -ForegroundColor Cyan
    
    # Verificar si se ejecuta como administrador
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    
    if (-not $isAdmin) {
        Write-Host "? Este comando requiere privilegios de administrador" -ForegroundColor Red
        Write-Host "   Ejecuta PowerShell como Administrador y vuelve a intentar" -ForegroundColor Yellow
        return
    }
    
    try {
        iisreset
        Write-Host "? IIS reiniciado exitosamente" -ForegroundColor Green
    } catch {
        Write-Host "? Error al reiniciar IIS: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Función para mostrar el menú
function Show-Menu {
    Clear-Host
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?   API Eventos ESCAT - Utilidades de Despliegue           ?" -ForegroundColor Cyan
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Generar JWT Secret Key" -ForegroundColor White
    Write-Host "  2. Generar Contraseña Segura (para SQL Server)" -ForegroundColor White
    Write-Host "  3. Probar Conectividad a SQL Server" -ForegroundColor White
    Write-Host "  4. Crear Regla de Firewall para SQL Server" -ForegroundColor White
    Write-Host "  5. Publicar Aplicación" -ForegroundColor White
    Write-Host "  6. Ver Logs Recientes" -ForegroundColor White
    Write-Host "  7. Reiniciar IIS" -ForegroundColor White
    Write-Host "  8. Todas las utilidades de seguridad (JWT + Contraseña)" -ForegroundColor White
    Write-Host "  9. Salir" -ForegroundColor White
    Write-Host ""
}

# Función principal - Menú interactivo
function Start-DeploymentUtilities {
    do {
        Show-Menu
        $choice = Read-Host "Selecciona una opción"
        
        switch ($choice) {
            "1" {
                New-JWTSecretKey
                Read-Host "`nPresiona Enter para continuar"
            }
            "2" {
                New-SecurePassword
                Read-Host "`nPresiona Enter para continuar"
            }
            "3" {
                $server = Read-Host "Ingresa el nombre o IP del servidor SQL"
                $port = Read-Host "Ingresa el puerto (Enter para usar 1433)"
                if ([string]::IsNullOrWhiteSpace($port)) { $port = 1433 }
                Test-SQLConnection -ServerName $server -Port $port
                Read-Host "`nPresiona Enter para continuar"
            }
            "4" {
                $ip = Read-Host "Ingresa la IP remota a permitir (Enter para permitir todas)"
                if ([string]::IsNullOrWhiteSpace($ip)) {
                    New-SQLFirewallRule
                } else {
                    New-SQLFirewallRule -RemoteIP $ip
                }
                Read-Host "`nPresiona Enter para continuar"
            }
            "5" {
                $path = Read-Host "Ingresa la ruta de publicación (Ej: C:\inetpub\wwwroot\EventosESCAT)"
                if (![string]::IsNullOrWhiteSpace($path)) {
                    Publish-APIRegistro -OutputPath $path
                }
                Read-Host "`nPresiona Enter para continuar"
            }
            "6" {
                $path = Read-Host "Ingresa la ruta de la carpeta de logs"
                if (![string]::IsNullOrWhiteSpace($path)) {
                    Get-APILogs -LogPath $path
                }
                Read-Host "`nPresiona Enter para continuar"
            }
            "7" {
                Restart-IISServer
                Read-Host "`nPresiona Enter para continuar"
            }
            "8" {
                Write-Host "`n" -NoNewline
                $jwt = New-JWTSecretKey
                Write-Host "`n" -NoNewline
                $pwd = New-SecurePassword
                
                Write-Host "`n?????????????????????????????????????????????????????????????" -ForegroundColor Green
                Write-Host "?              ??  GUARDA ESTOS VALORES                     ?" -ForegroundColor Green
                Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
                Write-Host "`nJWT Secret Key:" -ForegroundColor Yellow
                Write-Host $jwt -ForegroundColor White
                Write-Host "`nContraseña SQL Server:" -ForegroundColor Yellow
                Write-Host $pwd -ForegroundColor White
                
                Read-Host "`nPresiona Enter para continuar"
            }
            "9" {
                Write-Host "`n? Saliendo..." -ForegroundColor Green
                return
            }
            default {
                Write-Host "`n? Opción inválida" -ForegroundColor Red
                Start-Sleep -Seconds 2
            }
        }
    } while ($true)
}

# Exportar funciones para uso directo
Export-ModuleMember -Function New-JWTSecretKey, New-SecurePassword, Test-SQLConnection, `
    New-SQLFirewallRule, Publish-APIRegistro, Get-APILogs, Restart-IISServer, Start-DeploymentUtilities

# Mensaje de bienvenida
Write-Host "`n?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   API Eventos ESCAT - Utilidades de Despliegue           ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""
Write-Host "Script cargado correctamente." -ForegroundColor Green
Write-Host ""
Write-Host "Funciones disponibles:" -ForegroundColor Yellow
Write-Host "  • Start-DeploymentUtilities    - Menú interactivo" -ForegroundColor White
Write-Host "  • New-JWTSecretKey             - Generar JWT Secret" -ForegroundColor White
Write-Host "  • New-SecurePassword           - Generar contraseña" -ForegroundColor White
Write-Host "  • Test-SQLConnection           - Probar conectividad SQL" -ForegroundColor White
Write-Host "  • New-SQLFirewallRule          - Crear regla de firewall" -ForegroundColor White
Write-Host "  • Publish-APIRegistro          - Publicar aplicación" -ForegroundColor White
Write-Host "  • Get-APILogs                  - Ver logs" -ForegroundColor White
Write-Host "  • Restart-IISServer            - Reiniciar IIS" -ForegroundColor White
Write-Host ""
Write-Host "Para iniciar el menú interactivo, ejecuta:" -ForegroundColor Cyan
Write-Host "  Start-DeploymentUtilities" -ForegroundColor White
Write-Host ""
