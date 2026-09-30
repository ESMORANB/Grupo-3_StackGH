# ============================================================
# Script de despliegue - PowerShell (Windows)
# Punto 12 del enunciado: crea la carpeta del proyecto, clona el
# repo, compila con dotnet build y despliega el ejecutable.
#
# Uso:
#   .\deploy.ps1
#   .\deploy.ps1 -RepoUrl "https://gitlab.com/tu-usuario/tu-repo.git" -Rama "dev"
# ============================================================

param(
    [string]$RepoUrl = "https://github.com/ESMORANB/Grupo-3_StackGH.git",
    [string]$Rama = "main",
    [string]$CarpetaProyecto = "C:\mia_proyecto_I"
)

$ErrorActionPreference = "Stop"

function Escribir-Paso($mensaje) {
    Write-Host "`n=== $mensaje ===" -ForegroundColor Cyan
}

# 1. Crear la carpeta en disco C
Escribir-Paso "Creando carpeta del proyecto"
if (Test-Path $CarpetaProyecto) {
    Write-Host "[INFO] La carpeta '$CarpetaProyecto' ya existe, se reutiliza." -ForegroundColor Yellow
} else {
    try {
        New-Item -ItemType Directory -Path $CarpetaProyecto | Out-Null
        Write-Host "[OK] Carpeta creada: $CarpetaProyecto"
    } catch {
        Write-Host "[ERROR] No se pudo crear la carpeta: $_" -ForegroundColor Red
        exit 1
    }
}

# 2 y 3. Autenticacion y clonacion del repositorio
Escribir-Paso "Clonando el repositorio"
$carpetaRepo = Join-Path $CarpetaProyecto "repo"

if (Test-Path $carpetaRepo) {
    Write-Host "[INFO] El repo ya estaba clonado aqui, se actualiza con git pull." -ForegroundColor Yellow
    try {
        Push-Location $carpetaRepo
        git pull origin $Rama
        if ($LASTEXITCODE -ne 0) { throw "git pull devolvio codigo $LASTEXITCODE" }
        Pop-Location
    } catch {
        Write-Host "[ERROR] Fallo al actualizar el repositorio: $_" -ForegroundColor Red
        exit 1
    }
} else {
    try {
        # git clone pedira usuario/token si el repo es privado (autenticacion).
        git clone --branch $Rama $RepoUrl $carpetaRepo
        if ($LASTEXITCODE -ne 0) { throw "git clone devolvio codigo $LASTEXITCODE" }
        Write-Host "[OK] Repositorio clonado."
    } catch {
        Write-Host "[ERROR] Fallo la clonacion (revisa URL, rama o autenticacion): $_" -ForegroundColor Red
        exit 1
    }
}

# 4. Compilar con dotnet build
Escribir-Paso "Compilando el proyecto"
$carpetaCsproj = Join-Path $carpetaRepo "ExpedientesAcademicos"

try {
    Push-Location $carpetaCsproj
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "dotnet build devolvio codigo $LASTEXITCODE" }
    Pop-Location
    Write-Host "[OK] Compilacion exitosa."
} catch {
    Write-Host "[ERROR] Fallo la compilacion: $_" -ForegroundColor Red
    exit 1
}

# 5. Generar y desplegar los ejecutables
Escribir-Paso "Desplegando ejecutables"
$carpetaEjecutable = Join-Path $CarpetaProyecto "ejecutable"

if (-not (Test-Path $carpetaEjecutable)) {
    New-Item -ItemType Directory -Path $carpetaEjecutable | Out-Null
}

try {
    # Busca la carpeta de salida mas reciente dentro de bin\Release
    # (evita depender de saber el nombre exacto del TargetFramework).
    $origenBuild = Get-ChildItem -Path (Join-Path $carpetaCsproj "bin\Release") -Directory |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $origenBuild) { throw "No se encontro ninguna carpeta de salida del build." }

    Copy-Item -Path (Join-Path $origenBuild.FullName "*") -Destination $carpetaEjecutable -Recurse -Force
    Write-Host "[OK] Ejecutables copiados a: $carpetaEjecutable"
} catch {
    Write-Host "[ERROR] No se pudieron copiar los ejecutables: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== Despliegue completado ===" -ForegroundColor Green
Write-Host "Ejecuta el sistema desde: $carpetaEjecutable"
