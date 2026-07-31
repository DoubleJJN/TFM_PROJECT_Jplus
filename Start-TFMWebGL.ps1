<# 
.SYNOPSIS
    Script de inicio automatico para TFM Game WebGL
    Verifica Node.js, instala si falta, ejecuta npm install y lanza el servidor
#>

param(
    [switch]$ForceReinstall
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ServerDir = Join-Path $ProjectRoot "server"
$NodeVersion = "20.18.0"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "TFM Game - Inicio automatico WebGL" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

function Test-NodeJS {
    try {
        $nodeVersion = node --version 2>$null
        if ($nodeVersion) {
            Write-Host "Node.js detectado: $nodeVersion" -ForegroundColor Green
            return $true
        }
    } catch {}
    return $false
}

function Install-NodeJS {
    Write-Host "Instalando Node.js $NodeVersion..." -ForegroundColor Yellow
    
    if (Get-Command winget -ErrorAction SilentlyContinue) {
        Write-Host "   Usando winget..." -ForegroundColor Gray
        winget install --id OpenJS.NodeJS --version $NodeVersion --accept-source-agreements --accept-package-agreements
    } elseif (Get-Command choco -ErrorAction SilentlyContinue) {
        Write-Host "   Usando Chocolatey..." -ForegroundColor Gray
        choco install nodejs --version=$NodeVersion -y
    } else {
        Write-Host "   Descargando instalador oficial..." -ForegroundColor Gray
        $url = "https://nodejs.org/dist/v$NodeVersion/node-v$NodeVersion-x64.msi"
        $installer = Join-Path $env:TEMP "node-v$NodeVersion-x64.msi"
        Invoke-WebRequest -Uri $url -OutFile $installer -UseBasicParsing
        Write-Host "   Ejecutando instalador (requiere admin)..." -ForegroundColor Gray
        Start-Process msiexec.exe -ArgumentList "/i `"$installer`" /quiet /norestart" -Wait -Verb RunAs
        Remove-Item $installer -ErrorAction SilentlyContinue
    }
    
    $env:PATH = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
    
    if (Test-NodeJS) {
        Write-Host "Node.js instalado correctamente" -ForegroundColor Green
        return $true
    } else {
        Write-Host "Error instalando Node.js" -ForegroundColor Red
        return $false
    }
}

function Run-NpmInstall {
    Write-Host "Ejecutando npm install en $ServerDir..." -ForegroundColor Yellow
    
    if (-not (Test-Path (Join-Path $ServerDir "package.json"))) {
        Write-Host "No se encuentra package.json en $ServerDir" -ForegroundColor Red
        return $false
    }
    
    Push-Location $ServerDir
    try {
        if ($ForceReinstall -or -not (Test-Path "node_modules")) {
            Write-Host "   Instalando dependencias..." -ForegroundColor Gray
            npm install 2>&1 | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
        } else {
            Write-Host "   Dependencias ya instaladas (usa -ForceReinstall para forzar)" -ForegroundColor Gray
        }
        Write-Host "npm install completado" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "Error en npm install: $_" -ForegroundColor Red
        return $false
    } finally {
        Pop-Location
    }
}

function Free-Port3002 {
    Write-Host "Liberando puerto 3002..." -ForegroundColor Yellow
    $process = Get-NetTCPConnection -LocalPort 3002 -State Listen -ErrorAction SilentlyContinue
    if ($process) {
        $pid = $process.OwningProcess
        Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
        Write-Host "   Puerto 3002 liberado (PID: $pid)" -ForegroundColor Gray
    } else {
        Write-Host "   Puerto 3002 ya libre" -ForegroundColor Gray
    }
    Start-Sleep -Seconds 1
}

function Start-Server {
    Write-Host "Iniciando servidor..." -ForegroundColor Yellow
    $serverBat = Join-Path $ServerDir "start-server.bat"
    if (-not (Test-Path $serverBat)) {
        Write-Host "No se encuentra start-server.bat" -ForegroundColor Red
        return $false
    }
    
    $process = Start-Process cmd.exe -ArgumentList "/c `"$serverBat`"" -WindowStyle Normal -PassThru
    Write-Host "Servidor iniciado (PID: $($process.Id))" -ForegroundColor Green
    return $true
}

function Wait-And-OpenBrowser {
    Write-Host "Esperando a que el servidor responda en puerto 3002..." -ForegroundColor Yellow
    
    for ($i = 0; $i -lt 60; $i++) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:3002" -Method Head -TimeoutSec 2 -ErrorAction Stop -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-Host "Servidor respondiendo correctamente" -ForegroundColor Green
                Write-Host "Abriendo navegador en http://localhost:3002" -ForegroundColor Cyan
                Start-Process "http://localhost:3002"
                return $true
            }
        } catch {}
        Write-Host "   Esperando... ($i/60)" -ForegroundColor Gray
        Start-Sleep -Milliseconds 500
    }
    
    Write-Host "Timeout: el servidor no respondio en 30 segundos" -ForegroundColor Yellow
    Write-Host "   Intenta abrir manualmente: http://localhost:3002" -ForegroundColor Gray
    return $false
}

try {
    if (-not (Test-NodeJS) -or $ForceReinstall) {
        if (-not (Install-NodeJS)) {
            throw "No se pudo instalar Node.js"
        }
    }
    
    if (-not (Run-NpmInstall)) {
        throw "npm install fallo"
    }
    
    Free-Port3002
    
    if (-not (Start-Server)) {
        throw "No se pudo iniciar el servidor"
    }
    
    Wait-And-OpenBrowser
    
    Write-Host "`n==========================================" -ForegroundColor Cyan
    Write-Host "TFM Game WebGL listo para jugar!" -ForegroundColor Green
    Write-Host "==========================================`n" -ForegroundColor Cyan
    
} catch {
    Write-Host "`nERROR: $_" -ForegroundColor Red
    Write-Host "`nPresiona cualquier tecla para salir..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "`nPresiona cualquier tecla para cerrar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")