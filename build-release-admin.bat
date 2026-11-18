@echo off
setlocal

rem Re-launch this script with administrator privileges when needed.
powershell -NoProfile -ExecutionPolicy Bypass -Command "if (([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { exit 0 } else { exit 1 }"
if errorlevel 1 goto :elevate
goto :run

:elevate
echo Requesting administrator privileges...
set "BAT_PATH=%~f0"
powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Start-Process -FilePath $env:BAT_PATH -Verb RunAs -ErrorAction Stop } catch { Write-Host 'Elevation cancelled or failed.'; exit 1 }"
if errorlevel 1 pause
exit /b

:run
cd /d "%~dp0"

echo Running build-release.ps1 as administrator...
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-release.ps1"

if errorlevel 1 (
    echo.
    echo Build failed.
) else (
    echo.
    echo Build completed successfully.
)

pause
