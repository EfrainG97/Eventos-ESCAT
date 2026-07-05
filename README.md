# Eventos-ESCAT: Sistema de Registro de Asistencia

Eventos-ESCAT es una solución de software integral orientada a optimizar el registro de asistencia en eventos universitarios mediante tecnología de códigos QR. El sistema consta de un backend robusto basado en una API RESTful y una aplicación móvil multiplataforma para la captura de datos en tiempo real.

## Arquitectura del Proyecto

El sistema está diseñado bajo una arquitectura cliente-servidor distribuida:

1. **APIRegistro (Backend)**: Desarrollada en ASP.NET Core Web API. Centraliza la lógica de negocio, la validación de seguridad y la persistencia de los datos. Expone endpoints documentados y seguros para el consumo desde clientes externos.
2. **AppRegistro (Frontend Móvil)**: Aplicación multiplataforma construida con el framework .NET MAUI. Implementa el patrón MVVM (Model-View-ViewModel) para lograr una separación clara entre la interfaz de usuario y la lógica de presentación. Actúa como herramienta operativa en campo para el escaneo de los asistentes.

## Tecnologías Utilizadas

* **Framework Principal:** .NET 10.0
* **Backend:** ASP.NET Core Web API
* **Base de Datos y ORM:** SQL Server, Entity Framework Core (Code-First)
* **Seguridad:** JSON Web Tokens (JWT) para autenticación, BCrypt para hashing de contraseñas.
* **Cliente Móvil:** .NET MAUI (iOS, Android, Windows)
* **Escaneo QR:** `ZXing.Net.Maui` para procesamiento de visión e identificación de alumnos.
* **Documentación:** Swagger / OpenAPI

## Características Principales

* **Escaneo y Procesamiento en Tiempo Real:** Integración directa de la cámara del dispositivo móvil para leer identificadores QR, consultando los datos del asistente instantáneamente.
* **Seguridad Avanzada:** Protección de endpoints mediante políticas de autorización (JWT). Almacenamiento local en el dispositivo gestionado con `SecureStorage` para resguardar la sesión operativa.
* **Despliegue Flexible:** Soporte completo para hosting tradicional mediante Internet Information Services (IIS) o contenedores, y generación de binarios empaquetados (`.apk`) firmados para distribución en dispositivos Android.
* **Mantenibilidad:** Arquitectura limpia en la API con separación de responsabilidades (Controllers, Services, DTOs, Data) y uso de Inyección de Dependencias.

## Estructura del Código

```text
Eventos-ESCAT/
├── APIRegistro/              # Backend (API REST)
│   ├── Controllers/          # Exposición de endpoints (Auth, Admin, Alumno)
│   ├── Services/             # Lógica de negocio e interfaces
│   ├── Data/                 # ApplicationDbContext (Entity Framework)
│   ├── Models & DTOs/        # Entidades de dominio y transferencia de datos
│   └── Scripts/              # Utilidades de despliegue y base de datos
│
└── AppRegistro/              # Aplicación Móvil (.NET MAUI)
    ├── Pages/                # Vistas de la aplicación (XAML)
    ├── ViewModels/           # Enlace de datos y lógica de presentación (MVVM)
    └── Services/             # Comunicación HTTP con la API y almacenamiento seguro
```

## Ejecución en Entorno Local

### 1. Preparación de la Base de Datos y Backend
1. Asegurar una instancia local de SQL Server en ejecución.
2. Navegar al directorio de la API y actualizar la configuración de conexión:
   ```bash
   cd APIRegistro
   ```
3. Ejecutar las migraciones pendientes para construir el esquema de datos:
   ```bash
   dotnet ef database update
   ```
4. Iniciar la API:
   ```bash
   dotnet run
   ```

### 2. Ejecución del Cliente Móvil
1. Navegar al directorio de la aplicación MAUI:
   ```bash
   cd AppRegistro
   ```
2. Iniciar la aplicación apuntando al emulador o dispositivo de destino:
   ```bash
   dotnet run -f net10.0-android
   ```
*(Nota: Dentro de la app, es necesario configurar la dirección IP local de la API para establecer la conectividad durante el desarrollo).*
