#!/usr/bin/env bash
# Compila y corre el sistema localmente (Linux/Mac).
# Dale permiso de ejecucion una vez: chmod +x Ejecutar.sh

set -e
cd "$(dirname "$0")"

echo "============================================"
echo "  Sistema de Expedientes Academicos - MIA"
echo "============================================"

echo
echo "[1/2] Compilando el proyecto..."
if ! dotnet build --configuration Release; then
    echo
    echo "[ERROR] La compilacion fallo. Revisa los errores de arriba."
    read -p "Presiona Enter para salir..."
    exit 1
fi

echo
echo "[2/2] Iniciando el sistema..."
echo
dotnet run --configuration Release --no-build

echo
echo "============================================"
echo "  El sistema termino su ejecucion."
echo "============================================"
read -p "Presiona Enter para salir..."
