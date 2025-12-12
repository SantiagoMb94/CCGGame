# CCGGame

Juego de Cartas Coleccionables (CCG) construido en .NET MAUI con arquitectura MVVM, servicios modularizados y vistas modernas para construir mazos y jugar duelos.

## 🎮 Características principales

- **Modelos enriquecidos**: 
  - Cartas con rareza, coste, habilidades (Taunt, DivineShield, etc.)
  - Mazos con validación (30-40 cartas, máximo 1 legendaria, máximo 3 copias del resto)
  - Jugadores con mano, campo, energía, salud y sistema de turnos

- **Servicios modulares**:
  - `CardDataService`: Gestión de cartas base (30+ cartas)
  - `CardArtService`: Gestión de imágenes y arte de cartas
  - `DeckService`: Validación, barajado y generación de mazos iniciales
  - `DuelService`: Flujo completo de duelo, eventos, resolución de ataques y mecánicas especiales

- **Arquitectura MVVM completa**:
  - `MainMenuViewModel`: Navegación y menú principal
  - `DeckBuilderViewModel`: Construcción y gestión de mazos
  - `DuelViewModel`: Lógica de juego y control de duelos

- **UI moderna**:
  - `MainMenu`: Página de inicio con navegación
  - `DeckBuilderPage`: Constructor de mazos con búsqueda y filtros
  - `DuelPage`: Interfaz de duelo con tablero, mano y controles

- **Estilos y recursos**:
  - Temas por rareza de cartas
  - Converters personalizados para UI
  - Diseño responsive para escritorio y móvil

## 📁 Arquitectura / Estructura

```
CCGGame/
├── Models/              # Modelos de dominio
│   ├── Card.cs         # Modelo de carta con propiedades y habilidades
│   ├── Deck.cs         # Modelo de mazo con validación
│   └── Player.cs       # Modelo de jugador con estado del juego
│
├── Services/           # Servicios de lógica de negocio
│   ├── ICardArtService.cs
│   ├── ICardDataService.cs
│   ├── IDeckService.cs
│   ├── IDuelService.cs
│   ├── CardArtService.cs
│   ├── CardDataService.cs
│   ├── DeckService.cs
│   └── DuelService.cs
│
├── ViewModels/         # ViewModels MVVM
│   ├── MainMenuViewModel.cs
│   ├── DeckBuilderViewModel.cs
│   └── DuelViewModel.cs
│
├── Views/              # Vistas XAML
│   ├── MainMenu.xaml
│   ├── DeckBuilderPage.xaml
│   └── DuelPage.xaml
│
├── Converters/         # Converters para binding
│   ├── HealthToProgressConverter.cs
│   ├── ManaToProgressConverter.cs
│   ├── CrystalFillConverter.cs
│   ├── PlayableToColorConverter.cs
│   └── TypeToIconConverter.cs
│
├── Resources/
│   ├── Styles/         # Estilos y recursos XAML
│   │   ├── Colors.xaml
│   │   ├── Styles.xaml
│   │   ├── CardStyles.xaml
│   │   └── SharedStyles.xaml
│   ├── Images/
│   ├── Fonts/
│   └── AppIcon/
│
├── Tests/              # Proyecto de tests
│   └── CCGGame.Tests/
│       ├── RuleTests.cs
│       └── CCGGame.Tests.csproj
│
├── MauiProgram.cs      # Configuración DI y servicios
├── build-and-run.ps1   # Script de build para Windows
└── build-and-run.sh    # Script de build para Linux/Mac
```

## 🔧 Requisitos

- **.NET 8 SDK** o superior
- **Workload de .NET MAUI**:
  ```bash
  # Verificar workloads instalados
  dotnet workload list
  
  # Instalar MAUI workload (si falta)
  dotnet workload install maui
  ```
- **Para Windows**: Windows 10 SDK (10.0.17763.0 o superior)
- **Opcional para Android/iOS**: SDKs correspondientes y emuladores/dispositivos

## 🚀 Preparación

```bash
# Restaurar paquetes NuGet
dotnet restore
```

## 💻 Ejecutar en Windows

### Opción 1: Usando el script de PowerShell (Recomendado)

```powershell
# Compilar y ejecutar
./build-and-run.ps1

# Compilar, publicar (self-contained) y ejecutar
./build-and-run.ps1 -Publish
```

### Opción 2: Usando comandos dotnet directamente

```bash
# Compilar
dotnet build -f net8.0-windows10.0.19041.0

# Ejecutar
dotnet build -t:Run -f net8.0-windows10.0.19041.0

# Publicar (self-contained)
dotnet publish -f net8.0-windows10.0.19041.0 -c Release `
    -p:SelfContained=true `
    -p:WindowsAppSDKSelfContained=true
```

## 📱 Ejecutar en otras plataformas (Opcional)

### Android
Requiere SDK Android configurado y emulador/dispositivo:
```bash
dotnet build -f net8.0-android
dotnet build -t:Run -f net8.0-android
```

### iOS / Mac Catalyst
Requiere macOS y Xcode:
```bash
dotnet build -f net8.0-ios
dotnet build -f net8.0-maccatalyst
```

## 🎯 Funcionalidades clave

### Construcción de Mazos
- Búsqueda y filtros por tipo y rareza
- Validación en tiempo real (30-40 cartas, límites de copias)
- Vista previa de estadísticas del mazo
- Creación de mazo inicial desde el servicio

### Sistema de Duelo
- Turnos alternados con energía creciente
- Juego de cartas al campo
- Combate carta vs carta o directo al jugador
- Mecánicas especiales:
  - **Taunt**: Obliga a atacar cartas con esta habilidad primero
  - **DivineShield**: Bloquea el primer daño recibido
  - **Fatigue**: Daño creciente cuando el mazo se agota
- Log de eventos del juego en tiempo real

## 🧪 Tests

El proyecto incluye tests unitarios usando xUnit:

```bash
# Ejecutar tests
dotnet test
```

## 📝 Notas de implementación

- **Datos de cartas**: Definidos en `Services/CardDataService.cs`
- **Inyección de dependencias**: Configurada en `MauiProgram.cs`
- **Navegación**: Implementada mediante Shell navigation
- **Estilos de cartas**: Definidos en `Resources/Styles/SharedStyles.xaml`
- **Converters**: Utilizados para transformar datos en la UI (salud, maná, etc.)

## 🛠️ Desarrollo

### Estructura de servicios
Todos los servicios implementan interfaces para facilitar testing y mantenimiento:
- `ICardDataService` → `CardDataService`
- `ICardArtService` → `CardArtService`
- `IDeckService` → `DeckService`
- `IDuelService` → `DuelService`

### Patrones utilizados
- **MVVM**: Separación clara entre vista, lógica y modelo
- **Dependency Injection**: Servicios registrados en `MauiProgram.cs`
- **Command Pattern**: Comandos para acciones de UI
- **Observer Pattern**: Notificaciones de cambio de propiedades

## 📄 Licencia

Este proyecto es un ejemplo educativo de implementación de un juego de cartas usando .NET MAUI.
