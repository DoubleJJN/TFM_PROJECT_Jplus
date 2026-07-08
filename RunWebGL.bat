@echo off
setlocal
set "PROJECT_ROOT=%~dp0"
set "SERVER_BAT=%PROJECT_ROOT%server\start-server.bat"

echo ===================================================
echo Iniciando servidor y abriendo WebGL...
echo ===================================================

start "TFM Server" "%SERVER_BAT%"

powershell -NoProfile -ExecutionPolicy Bypass -Command "for ($i = 0; $i -lt 60; $i++) { if (Test-NetConnection -ComputerName localhost -Port 3002 -InformationLevel Quiet) { Start-Process 'http://localhost:3002'; exit } Start-Sleep -Milliseconds 500 }; Start-Process 'http://localhost:3002'"

endlocal