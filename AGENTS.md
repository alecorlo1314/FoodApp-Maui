# Contexto del Proyecto
Este proyecto es una aplicación móvil de entrega de comida desarrollada en .NET MAUI utilizando Clean Architecture, MVVM (CommunityToolkit.Mvvm), MediatR para CQRS, y Supabase como backend.

## 1. Arquitectura del Proyecto (Clean Architecture)
El código debe estar estrictamente separado en 4 capas:
- **Domain**: Contiene las entidades puras, interfaces de repositorios (`IUserRepository`, etc.) y objetos de resultado (`Result<T>`). NO debe tener referencias a UI, librerías externas de bases de datos o frameworks.
- **Application**: Contiene los casos de uso organizados mediante MediatR (Queries y Commands), manejadores (`Handlers`), y validaciones de negocio.
- **Infrastructure**: Contiene la implementación de los repositorios, modelos de Supabase mapeados con atributos (`[Table]`, `[Column]`), servicios externos (Stripe, GPS/Geocoding), y acceso a datos.
- **Presentation**: Contiene las vistas (`ContentPage`, XAML) y los ViewModels. Los ViewModels se comunican únicamente con `IMediator` y no deben realizar consultas directas a bases de datos ni lógica de infraestructura.

## 2. Estándares de Código y Patrones
- **MVVM**: Utiliza obligatoriamente los atributos de **CommunityToolkit.Mvvm**:
  - `[ObservableProperty]` para propiedades que notifican cambios a la vista.
  - `[RelayCommand]` para los comandos asíncronos o síncronos.
- **Inyección de Dependencias (DI)**: Todos los servicios, repositorios y manejadores deben registrarse correctamente en `MauiProgram.cs` utilizando los ciclos de vida adecuados (`AddTransient`, `AddScoped`, `AddSingleton`).
- **Manejo de Errores**: Utiliza la clase genérica `Result<T>` o `Result` para retornar éxitos o fallos desde Application/Infrastructure hacia los ViewModels, evitando propagar excepciones no controladas a la UI.

## 3. Limitaciones y Reglas Técnicas
- **No acoplar UI con Base de Datos**: Está prohibido colocar lógica de Supabase u obtención directa de datos dentro de los ViewModels. Siempre debe hacerse a través de MediatR (Queries/Commands).
- **Pasarelas de Pago**: Las transacciones de pago deben pasar por `IPaymentGatewayService` (Stripe) utilizando tokens seguros o las configuraciones de prueba permitidas.
- **Mapas y Ubicación**: Para la geolocalización, usa las APIs nativas de MAUI Essentials (`Geolocation` y `Geocoding`). Para mapas en vistas secundarias, apóyate en `WebView` con Leaflet o controles compatibles de Syncfusion.
- **UI & Diseño**: Las vistas deben construirse en XAML siguiendo una estética moderna, limpia, usando componentes de Syncfusion (`SfMaskedEntry`, `SfBottomSheet`) cuando se requiera y respetando la paleta de colores corporativa (Naranja `#FF7622`, fondos oscuros `#121223` o blancos limpios según la vista).

## Regla de Cancelación Asíncrona (CancellationToken)
- **Uso obligatorio de CancellationToken**: Todos los `IRequestHandler` de MediatR deben aceptar y propagar el parámetro `CancellationToken cancellationToken` en sus métodos `Handle`.
- **Propagación en infraestructura**: El token debe pasarse a todas las operaciones asíncronas subyacentes (consultas a Supabase, llamadas HTTP con HttpClient, lecturas de archivos, etc.) para evitar procesos huérfanos si la vista cambia o se cancela la acción.