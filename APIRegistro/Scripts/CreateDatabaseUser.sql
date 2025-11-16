-- ============================================
-- Script para crear usuario de SQL Server
-- con acceso remoto para IIS
-- Base de Datos: EventosESCAT
-- ============================================

USE [master]
GO

-- 1. Crear el login de SQL Server (autenticación SQL)
-- IMPORTANTE: Cambia 'TuContraseñaSegura123!' por una contraseña fuerte
IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = 'EventosESCATUser')
BEGIN
    CREATE LOGIN [EventosESCATUser] 
    WITH PASSWORD = N'TuContraseñaSegura123!',
         DEFAULT_DATABASE = [EventosESCAT],
         CHECK_EXPIRATION = OFF,
         CHECK_POLICY = ON
    
    PRINT 'Login EventosESCATUser creado exitosamente'
END
ELSE
BEGIN
    PRINT 'El login EventosESCATUser ya existe'
END
GO

-- 2. Cambiar a la base de datos EventosESCAT
USE [EventosESCAT]
GO

-- 3. Crear el usuario de base de datos asociado al login
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'EventosESCATUser')
BEGIN
    CREATE USER [EventosESCATUser] FOR LOGIN [EventosESCATUser]
    PRINT 'Usuario EventosESCATUser creado en la base de datos'
END
ELSE
BEGIN
    PRINT 'El usuario EventosESCATUser ya existe en la base de datos'
END
GO

-- 4. Asignar permisos al usuario
-- Rol db_datareader: permite leer todos los datos
ALTER ROLE [db_datareader] ADD MEMBER [EventosESCATUser]
GO

-- Rol db_datawriter: permite insertar, actualizar y eliminar datos
ALTER ROLE [db_datawriter] ADD MEMBER [EventosESCATUser]
GO

-- Permisos adicionales para ejecutar stored procedures (opcional)
GRANT EXECUTE TO [EventosESCATUser]
GO

PRINT 'Permisos asignados correctamente al usuario EventosESCATUser'
GO

-- ============================================
-- Configuración para acceso remoto
-- ============================================

-- 5. Habilitar autenticación de SQL Server (requiere reiniciar SQL Server)
-- Ejecuta esto en SQL Server Management Studio o desde SQL Server Configuration Manager:
/*
    EXEC xp_instance_regwrite 
        N'HKEY_LOCAL_MACHINE', 
        N'Software\Microsoft\MSSQLServer\MSSQLServer',
        N'LoginMode', 
        REG_DWORD, 
        2  -- 1 = Windows Authentication, 2 = Mixed Mode
    GO
*/

-- Nota: Después de ejecutar el comando anterior, debes REINICIAR el servicio de SQL Server

-- ============================================
-- Verificar la configuración
-- ============================================

-- Verificar que el login existe
SELECT 
    'Login' as Tipo,
    name as Nombre,
    create_date as FechaCreacion,
    CASE WHEN is_disabled = 0 THEN 'Activo' ELSE 'Deshabilitado' END as Estado
FROM sys.sql_logins 
WHERE name = 'EventosESCATUser'
GO

-- Verificar que el usuario existe en la base de datos
USE [EventosESCAT]
GO

SELECT 
    'Usuario DB' as Tipo,
    name as Nombre,
    create_date as FechaCreacion,
    type_desc as TipoUsuario
FROM sys.database_principals 
WHERE name = 'EventosESCATUser'
GO

-- Verificar roles asignados
SELECT 
    r.name as Rol,
    m.name as Usuario
FROM sys.database_role_members rm
INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
INNER JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
WHERE m.name = 'EventosESCATUser'
GO

PRINT '============================================'
PRINT 'Script completado exitosamente'
PRINT '============================================'
PRINT ''
PRINT 'PASOS ADICIONALES PARA HABILITAR ACCESO REMOTO:'
PRINT '1. Habilitar autenticación mixta en SQL Server (Windows + SQL Server)'
PRINT '2. Reiniciar el servicio de SQL Server'
PRINT '3. Configurar el Firewall de Windows para permitir el puerto 1433'
PRINT '4. Habilitar TCP/IP en SQL Server Configuration Manager'
PRINT '5. Actualizar la cadena de conexión en appsettings.json'
PRINT ''
PRINT 'Cadena de conexión para IIS:'
PRINT 'Server=TU_SERVIDOR;Database=EventosESCAT;User Id=EventosESCATUser;Password=TuContraseñaSegura123!;TrustServerCertificate=True;'
GO
