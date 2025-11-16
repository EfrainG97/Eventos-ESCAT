# EventosESCAT - Aplicación de Registro de Eventos

Aplicación móvil desarrollada con .NET MAUI para el registro de eventos de alumnos de ESCAT mediante escaneo de códigos QR.

## 📱 Descripción General

**EventosESCAT** es una aplicación móvil multiplataforma que permite a los administradores registrar la asistencia de alumnos a eventos mediante el escaneo de códigos QR. La aplicación se conecta a una API REST backend para autenticación y gestión de datos.

### Características Principales

- ✅ **Autenticación JWT**: Login seguro con tokens JWT
- ✅ **Escaneo de Códigos QR**: Lectura de códigos QR para identificar alumnos
- ✅ **Registro de Eventos**: Actualización de información de alumnos en tiempo real
- ✅ **Almacenamiento Seguro**: Guardado seguro de credenciales y tokens
- ✅ **Configuración Flexible**: Configuración de IP y puerto del servidor API

## 🏗️ Arquitectura del Proyecto

El proyecto está dividido en dos componentes principales:

```
EventosESCAT/
├── AppRegistro/          # Aplicación móvil .NET MAUI
│   ├── Pages/           # Páginas de la aplicación
│   │   ├── LoginPage.xaml
│   │   ├── QRScannerPage.xaml
│   │   └── MainPage.xaml
│   ├── Services/        # Servicios de la aplicación
│   │   ├── ApiService.cs
│   │   └── SecureStorageService.cs
│   ├── Models/          # Modelos de datos
│   └── Platforms/       # Configuraciones específicas por plataforma
│       ├── Android/
│       └── iOS/
│
└── APIRegistro/         # API REST Backend
    ├── Controllers/     # Controladores de la API
    ├── Services/        # Servicios de negocio
    ├── Data/            # Contexto de base de datos
    └── Models/          # Modelos de datos
```

## 📋 Requisitos Previos

### Para Desarrollo

- **.NET 10.0 SDK** o superior
- **Visual Studio 2022** (con carga de trabajo de desarrollo móvil) o **Visual Studio Code**
- **Android SDK** (para desarrollo Android)
- **Java JDK 17** o superior
- **Android Emulator** o dispositivo físico con Android 5.0 (API 21) o superior

### Para Publicación Android

- **Java JDK 17** o superior (para generar keystore)
- **Android SDK** con herramientas de compilación
- **Keytool** (incluido con JDK)

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd EventosESCAT
```

### 2. Restaurar Paquetes NuGet

```bash
dotnet restore
```

### 3. Configurar la API Backend

Antes de usar la aplicación móvil, asegúrate de que la API backend esté configurada y ejecutándose. Consulta la documentación en `APIRegistro/README.md` para más detalles.

### 4. Configurar Permisos de Android

La aplicación requiere los siguientes permisos en Android:

- **Cámara**: Para escanear códigos QR
- **Internet**: Para conectarse a la API backend

Estos permisos ya están configurados en `AppRegistro/Platforms/Android/AndroidManifest.xml`.

## 🔐 Creación del Keystore para Android

Para publicar la aplicación en Google Play Store o distribuirla como APK firmado, necesitas crear un keystore (archivo de certificado).

### Paso 1: Verificar que Java JDK esté instalado

Abre CMD (Símbolo del sistema) y verifica la instalación:

```cmd
java -version
```

Si no está instalado, descarga e instala [Java JDK 17](https://www.oracle.com/java/technologies/downloads/#java17) o superior.

### Paso 2: Navegar a la carpeta del proyecto

```cmd
cd C:\Users\Efra\source\repos\EventosESCAT\AppRegistro
```

### Paso 3: Crear el Keystore

Ejecuta el siguiente comando para crear el keystore:

```cmd
keytool -genkeypair -v -storetype PKCS12 -keystore eventos-escat.keystore -alias eventos-escat -keyalg RSA -keysize 2048 -validity 10000 -storepass 123456 -keypass 123456
```

**Parámetros explicados:**

- `-genkeypair`: Genera un par de claves (pública y privada)
- `-v`: Modo verbose (muestra información detallada)
- `-storetype PKCS12`: Formato del keystore (PKCS12 es el estándar)
- `-keystore eventos-escat.keystore`: Nombre del archivo keystore
- `-alias eventos-escat`: Alias de la clave (nombre identificador)
- `-keyalg RSA`: Algoritmo de cifrado (RSA)
- `-keysize 2048`: Tamaño de la clave (2048 bits es seguro)
- `-validity 10000`: Validez del certificado en días (aproximadamente 27 años)
- `-storepass 123456`: Contraseña del keystore
- `-keypass 123456`: Contraseña de la clave privada

**⚠️ IMPORTANTE DE SEGURIDAD:**

1. **Cambia las contraseñas** (`123456`) por contraseñas seguras antes de crear el keystore en producción
2. **Guarda el keystore en un lugar seguro** - Si lo pierdes, no podrás actualizar tu aplicación
3. **No compartas el keystore** - Es tu certificado de identidad para Google Play

### Paso 4: Completar la información del certificado

Durante la creación, te pedirá información adicional:

```
¿Cuál es su nombre y apellido?
  [Unknown]: ESCAT Eventos

¿Cuál es el nombre de la unidad organizativa?
  [Unknown]: Escuela Superior de Cómputo

¿Cuál es el nombre de su organización?
  [Unknown]: ESCAT

¿Cuál es el nombre de su ciudad o localidad?
  [Unknown]: Ciudad de México

¿Cuál es el nombre de su estado o provincia?
  [Unknown]: CDMX

¿Cuál es el código de país de dos letras para esta unidad?
  [Unknown]: MX
```

Presiona Enter para aceptar los valores predeterminados o ingresa tus propios valores.

### Paso 5: Verificar que el keystore se creó correctamente

```cmd
dir eventos-escat.keystore
```

Deberías ver el archivo `eventos-escat.keystore` en la carpeta `AppRegistro`.

### Información del Keystore Creado

- **Archivo**: `eventos-escat.keystore`
- **Alias**: `eventos-escat`
- **Contraseña del Keystore**: `123456` (cambiar en producción)
- **Contraseña de la Clave**: `123456` (cambiar en producción)

## 📦 Publicación de la Aplicación Android

### Publicar APK Firmado

Para publicar la aplicación como APK firmado, usa el siguiente comando desde CMD:

```cmd
cd C:\Users\Efra\source\repos\EventosESCAT\AppRegistro

dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk /p:AndroidKeyStore=true /p:AndroidSigningKeyStore=eventos-escat.keystore /p:AndroidSigningKeyAlias=eventos-escat /p:AndroidSigningKeyPass=123456 /p:AndroidSigningStorePass=123456
```

**Parámetros explicados:**

- `-f net10.0-android`: Framework objetivo (.NET 10.0 para Android)
- `-c Release`: Configuración de compilación (Release)
- `/p:AndroidPackageFormat=apk`: Formato del paquete (APK)
- `/p:AndroidKeyStore=true`: Habilitar firma con keystore
- `/p:AndroidSigningKeyStore=eventos-escat.keystore`: Ruta al archivo keystore
- `/p:AndroidSigningKeyAlias=eventos-escat`: Alias de la clave
- `/p:AndroidSigningKeyPass=123456`: Contraseña de la clave privada
- `/p:AndroidSigningStorePass=123456`: Contraseña del keystore

### Ubicación del APK Generado

Después de la publicación, el APK firmado se encontrará en:

```
AppRegistro\bin\Release\net10.0-android\com.escat.appregistro-Signed.apk
```

### Publicar AAB (Android App Bundle) para Google Play

Si deseas publicar en Google Play Store, usa el formato AAB:

```cmd
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=aab /p:AndroidKeyStore=true /p:AndroidSigningKeyStore=eventos-escat.keystore /p:AndroidSigningKeyAlias=eventos-escat /p:AndroidSigningKeyPass=123456 /p:AndroidSigningStorePass=123456
```

El archivo AAB se generará en:

```
AppRegistro\bin\Release\net10.0-android\com.escat.appregistro-Signed.aab
```

## 🧪 Ejecutar la Aplicación en Modo Debug

### Desde Visual Studio

1. Abre el proyecto `EventosESCAT.slnx` en Visual Studio 2022
2. Selecciona el proyecto `AppRegistro`
3. Selecciona un emulador Android o dispositivo físico
4. Presiona F5 o clic en "Iniciar"

### Desde Línea de Comandos

```cmd
cd C:\Users\Efra\source\repos\EventosESCAT\AppRegistro

dotnet build -f net10.0-android

dotnet run -f net10.0-android
```

## 📱 Uso de la Aplicación

### 1. Inicio de Sesión

1. Al abrir la aplicación, verás la pantalla de login
2. Ingresa la **IP del servidor** donde está desplegada la API
3. Ingresa el **puerto** del servidor (ej: 443 para HTTPS, 80 para HTTP)
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

## 🔧 Configuración Avanzada

### Cambiar la URL Base de la API

La URL de la API se configura en la pantalla de login y se guarda de forma segura en el dispositivo. Para cambiar la configuración:

1. Cierra sesión de la aplicación
2. Ingresa la nueva IP y puerto en la pantalla de login

### Configurar Certificados SSL

Si tu API usa HTTPS con certificados autofirmados, es posible que necesites configurar la validación de certificados. Consulta la documentación de .NET MAUI para más detalles.

## 📚 Dependencias Principales

### Paquetes NuGet Utilizados

- **Microsoft.Maui.Controls** - Framework MAUI
- **Microsoft.Maui.Essentials** - Funcionalidades esenciales de MAUI
- **ZXing.Net.Maui** (0.6.0) - Biblioteca para escaneo de códigos QR
- **ZXing.Net.Maui.Controls** (0.6.0) - Controles UI para escaneo QR

## 🐛 Solución de Problemas

### Error: "No se puede encontrar el keystore"

**Solución**: Asegúrate de que el archivo `eventos-escat.keystore` esté en la carpeta `AppRegistro` y que la ruta en el comando de publicación sea correcta.

### Error: "Contraseña incorrecta del keystore"

**Solución**: Verifica que las contraseñas en el comando de publicación coincidan con las usadas al crear el keystore.

### Error: "No se puede conectar a la API"

**Solución**: 
1. Verifica que la API esté ejecutándose
2. Verifica que la IP y puerto sean correctos
3. Verifica que el dispositivo/emulador tenga acceso a la red
4. Si usas HTTPS, verifica que el certificado sea válido

### Error: "Permiso de cámara denegado"

**Solución**: 
1. Ve a Configuración del dispositivo > Aplicaciones > AppRegistro > Permisos
2. Habilita el permiso de Cámara

## 📝 Notas Importantes

- **Keystore**: Guarda el archivo `eventos-escat.keystore` en un lugar seguro. Si lo pierdes, no podrás actualizar tu aplicación en Google Play Store.
- **Contraseñas**: En producción, usa contraseñas seguras para el keystore (mínimo 12 caracteres, con mayúsculas, minúsculas, números y símbolos).
- **Versión de la App**: La versión se configura en `AppRegistro.csproj` en las propiedades `ApplicationDisplayVersion` y `ApplicationVersion`.
- **Package Name**: El identificador de la aplicación es `com.escat.appregistro` (configurado en `ApplicationId`).

## 🔗 Enlaces Útiles

- [Documentación de .NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [Documentación de ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui)
- [Guía de publicación en Google Play](https://developer.android.com/studio/publish)
- [Documentación de la API Backend](./APIRegistro/README.md)

## 📄 Licencia

[Especificar licencia si aplica]

## 👥 Contribuidores

[Especificar contribuidores si aplica]

---

**Última actualización**: 2025

