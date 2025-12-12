param(
    [string]$Framework = "net8.0-windows10.0.19041.0",
    [switch]$Publish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Info($msg) { Write-Host "[INFO] $msg" -ForegroundColor Cyan }
function Write-Err($msg)  { Write-Host "[ERROR] $msg" -ForegroundColor Red }

try {
    $root = Split-Path -Parent $MyInvocation.MyCommand.Path
    Set-Location $root

    # Asegurar dotnet en PATH si está en la ruta típica
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        $dotnetPath = "C:\Program Files\dotnet"
        if (Test-Path "$dotnetPath\dotnet.exe") {
            $env:PATH = "$dotnetPath;$env:PATH"
        }
    }

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "dotnet no está disponible en PATH. Instala .NET 8 SDK y workload MAUI."
    }

    Write-Info "Restaurando paquetes..."
    dotnet restore

    if ($Publish) {
        Write-Info "Publicando self-contained ($Framework)..."
        dotnet publish -f $Framework -c Release `
            -p:SelfContained=true `
            -p:WindowsAppSDKSelfContained=true `
            -p:PublishSingleFile=false `
            -p:PublishReadyToRun=false

        $publishPath = Join-Path $root "bin/Release/$Framework/win10-x64/publish/CCGGame.exe"
        if (-not (Test-Path $publishPath)) {
            throw "No se encontró el ejecutable publicado en $publishPath"
        }

        Write-Info "Ejecutando publicado ($publishPath)..."
        & $publishPath
    }
    else {
        Write-Info "Compilando ($Framework)..."
        dotnet build -f $Framework -p:SelfContained=true -p:WindowsAppSDKSelfContained=true

        Write-Info "Ejecutando ($Framework)..."
        dotnet build -t:Run -f $Framework -p:SelfContained=true -p:WindowsAppSDKSelfContained=true
    }
}
catch {
    Write-Err $_
    exit 1
}

