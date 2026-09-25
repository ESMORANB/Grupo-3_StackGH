#!/usr/bin/env bash
# ============================================================
# Script de despliegue - Bash (Linux/Mac)
# Punto 12 del enunciado. Nota: "disco C" es especifico de Windows,
# aqui se usa $HOME/mia_proyecto_I como equivalente portable.
#
# Uso:
#   ./deploy.sh
#   ./deploy.sh "https://gitlab.com/tu-usuario/tu-repo.git" "dev"
# ============================================================

set -e

REPO_URL="${1:-https://github.com/ESMORANB/Grupo-3_StackGH.git}"
RAMA="${2:-main}"
CARPETA_PROYECTO="$HOME/mia_proyecto_I"
CARPETA_REPO="$CARPETA_PROYECTO/repo"
CARPETA_CSPROJ="$CARPETA_REPO/ExpedientesAcademicos"
CARPETA_EJECUTABLE="$CARPETA_PROYECTO/ejecutable"

paso() { echo -e "\n=== $1 ==="; }

# 1. Crear carpeta del proyecto
paso "Creando carpeta del proyecto"
if [ -d "$CARPETA_PROYECTO" ]; then
    echo "[INFO] La carpeta '$CARPETA_PROYECTO' ya existe, se reutiliza."
else
    mkdir -p "$CARPETA_PROYECTO" || { echo "[ERROR] No se pudo crear la carpeta."; exit 1; }
    echo "[OK] Carpeta creada: $CARPETA_PROYECTO"
fi

# 2 y 3. Autenticacion y clonacion
paso "Clonando el repositorio"
if [ -d "$CARPETA_REPO" ]; then
    echo "[INFO] El repo ya estaba clonado aqui, se actualiza con git pull."
    (cd "$CARPETA_REPO" && git pull origin "$RAMA") || { echo "[ERROR] Fallo al actualizar el repositorio."; exit 1; }
else
    # git pedira usuario/token si el repo es privado (autenticacion).
    git clone --branch "$RAMA" "$REPO_URL" "$CARPETA_REPO" || { echo "[ERROR] Fallo la clonacion (revisa URL, rama o autenticacion)."; exit 1; }
    echo "[OK] Repositorio clonado."
fi

# 4. Compilar
paso "Compilando el proyecto"
if ! (cd "$CARPETA_CSPROJ" && dotnet build --configuration Release); then
    echo "[ERROR] Fallo la compilacion."
    echo "[AVISO] Si el .csproj tiene <TargetFramework>net8.0-windows</TargetFramework>,"
    echo "        el proyecto es exclusivo de Windows y NO compila en Linux/Mac."
    echo "        Debe cambiarse a net8.0 (sin -windows) para que sea portable."
    exit 1
fi
echo "[OK] Compilacion exitosa."

# 5. Desplegar ejecutables
paso "Desplegando ejecutables"
mkdir -p "$CARPETA_EJECUTABLE"

ORIGEN_BUILD=$(find "$CARPETA_CSPROJ/bin/Release" -mindepth 1 -maxdepth 1 -type d 2>/dev/null | sort | tail -n 1)
if [ -z "$ORIGEN_BUILD" ]; then
    echo "[ERROR] No se encontro la carpeta de salida del build."
    exit 1
fi

cp -R "$ORIGEN_BUILD/." "$CARPETA_EJECUTABLE/" || { echo "[ERROR] No se pudieron copiar los ejecutables."; exit 1; }
echo "[OK] Ejecutables copiados a: $CARPETA_EJECUTABLE"

echo -e "\n=== Despliegue completado ==="
echo "Ejecuta el sistema desde: $CARPETA_EJECUTABLE"
