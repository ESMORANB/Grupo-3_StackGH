@echo off
setlocal

echo ============================================
echo   Sistema de Expedientes Academicos - MIA
echo ============================================

cd /d "%~dp0"

echo.
echo [1/2] Compilando el proyecto...
dotnet build --configuration Release

if errorlevel 1 (
    echo.
    echo [ERROR] La compilacion fallo. Revisa los errores de arriba.
    pause
    exit /b 1
)

echo.
echo [2/2] Iniciando el sistema...
echo.

dotnet run --configuration Release --no-build

echo.
echo ============================================
echo   El sistema termino su ejecucion.
echo ============================================
pause
