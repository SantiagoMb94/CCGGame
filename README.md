# CCGGame

Juego de Cartas Coleccionables (CCG) construido en .NET MAUI con arquitectura MVVM, servicios modularizados y vistas modernas para construir mazos y jugar duelos.

## Características principales
- Modelos enriquecidos: cartas con rareza, coste, habilidades; mazos con validación (30-40 cartas, 1 legendaria máx., 3 copias resto); jugadores con mano, campo, energía y turnos.
- Servicios: `CardDataService` con 30 cartas base; `DeckService` para validar, barajar y generar mazos iniciales; `DuelService` con flujo de duelo, eventos y resolución de ataques.
- MVVM completo: `MainMenuViewModel`, `DeckBuilderViewModel`, `DuelViewModel` con comandos y notificaciones de cambio.
- UI: páginas `MainPage` (menú), `DeckBuilderPage` y `DuelPage` con estilos y componentes listos para escritorio/móvil.
- Estilos: `CardStyles.xaml` con temas por rareza y acciones de carta.

## Arquitectura / Estructura
- `Models/`: `Card`, `Deck`, `Player`
- `Services/`: `CardDataService`, `DeckService`, `DuelService`
- `ViewModels/`: VM para menú, constructor de mazos y duelo
- `Views/`: `MainPage`, `DeckBuilderPage`, `DuelPage`
- `Resources/Styles/`: colores, estilos generales y estilos de carta
- `MauiProgram.cs`: registro de servicios, viewmodels y vistas (DI)

## Requisitos
- .NET 8 SDK
- Workload de .NET MAUI (Windows para ejecutar target win10)
  - Verificar: `dotnet workload list`
  - Instalar (si falta): `dotnet workload install maui`
- Opcional para Android/iOS: SDKs y emuladores/dispositivos correspondientes.

## Preparación
```bash
dotnet restore
```

## Ejecutar en Windows
```bash
# Construir
dotnet build -f net8.0-windows10.0.19041.0

# Ejecutar (usa el target Windows)
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## Ejecutar en Android (opcional)
Requiere SDK Android configurado y emulador/dispositivo:
```bash
dotnet build -f net8.0-android
dotnet build -t:Run -f net8.0-android
```

## Funcionalidades clave
- Construcción de mazos con búsqueda y filtros (tipo, rareza) y validación en tiempo real.
- Creación de mazo inicial desde el servicio de cartas.
- Duelo con turnos, energía creciente, juego de cartas al campo y combate carta vs carta o directo al jugador.
- Log de eventos del juego.

## Notas de implementación
- Datos de cartas en `Services/CardDataService`.
- Navegación y DI configurados en `MauiProgram.cs`.
- Estilos de cartas en `Resources/Styles/CardStyles.xaml`.
