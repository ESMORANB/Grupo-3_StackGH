@echo off
setlocal enabledelayedexpansion

set REPO_URL=https://github.com/ESMORANB/Grupo-3_StackGH.git
set RAMA=main
set CARPETA_PROYECTO=C:\mia_proyecto_I
set CARPETA_REPO=%CARPETA_PROYECTO%\repo
set CARPETA_CSPROJ=%CARPETA_REPO%\ExpedientesAcademicos
set CARPETA_EJECUTABLE=%CARPETA_PROYECTO%\ejecutable

echo ============================================
echo   Despliegue automatizado - Sistema MIA
echo ============================================
echo.

REM 1. Crear la carpeta del proyecto
if not exist "%CARPETA_PROYECTO%" (
    mkdir "%CARPETA_PROYECTO%"
    echo [OK] Carpeta creada: %CARPETA_PROYECTO%
) else (
    echo [INFO] La carpeta ya existe, se reutiliza.
)

REM 2 y 3. Autenticacion y clonacion del repositorio
echo.
echo [1/3] Obteniendo el repositorio...
if exist "%CARPETA_REPO%" (
    echo [INFO] Ya estaba clonado, actualizando con git pull...
    pushd "%CARPETA_REPO%"
    git pull origin %RAMA%
    if errorlevel 1 (
        echo [ERROR] Fallo al actualizar el repositorio.
        popd
        pause
        exit /b 1
    )
    popd
) else (
    git clone --branch %RAMA% %REPO_URL% "%CARPETA_REPO%"
    if errorlevel 1 (
        echo [ERROR] Fallo la clonacion. Revisa tu conexion o si el repo/rama existe.
        pause
        exit /b 1
    )
    echo [OK] Repositorio clonado.
)

REM 4. Compilar con dotnet build
echo.
echo [2/3] Compilando el proyecto...
pushd "%CARPETA_CSPROJ%"
dotnet build --configuration Release
if errorlevel 1 (
    echo [ERROR] Fallo la compilacion. Revisa los errores de arriba.
    popd
    pause
    exit /b 1
)
popd
echo [OK] Compilacion exitosa.

REM 5. Generar y desplegar los ejecutables
echo.
echo [3/3] Desplegando ejecutables...
if not exist "%CARPETA_EJECUTABLE%" mkdir "%CARPETA_EJECUTABLE%"

set ORIGEN=
for /f "delims=" %%d in ('dir /b /ad /o-d "%CARPETA_CSPROJ%\bin\Release" 2^>nul') do (
    if not defined ORIGEN set ORIGEN=%CARPETA_CSPROJ%\bin\Release\%%d
)

if not defined ORIGEN (
    echo [ERROR] No se encontro la carpeta de salida del build.
    pause
    exit /b 1
)

xcopy "!ORIGEN!\*" "%CARPETA_EJECUTABLE%\" /E /Y /I >nul
if errorlevel 1 (
    echo [ERROR] No se pudieron copiar los ejecutables.
    pause
    exit /b 1
)
echo [OK] Ejecutables copiados a: %CARPETA_EJECUTABLE%

echo.
echo ============================================
echo   Despliegue completado
echo   Ejecuta el sistema desde: %CARPETA_EJECUTABLE%
echo ============================================
echo.
echo Iniciando el sistema...
echo.

pushd "%CARPETA_EJECUTABLE%"
ExpedientesAcademicos.exe
popd

echo.
pause
