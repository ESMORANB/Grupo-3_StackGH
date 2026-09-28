@echo off
echo ============================================
echo   Despliegue automatizado - Sistema MIA
echo ============================================
echo.

powershell.exe -ExecutionPolicy Bypass -File "%~dp0deploy.ps1"

echo.
pause
