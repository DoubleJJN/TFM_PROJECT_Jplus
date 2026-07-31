@echo off
echo Liberando puerto 3002...
for /f "tokens=5" %%a in ('netstat -aon ^| find ":3002" ^| find "LISTENING"') do taskkill /f /pid %%a >nul 2>&1
timeout /t 1 /nobreak >nul
cd /d "%~dp0"
echo ===================================================
echo Iniciando servidor TFM Game...
echo ===================================================
node server.js
pause
