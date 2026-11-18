$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "src\OvertimeTimer.App\OvertimeTimer.App.csproj"
$dotnet = "C:\Users\Admin\.dotnet\dotnet.exe"

& $dotnet publish $project -c Release -r win-x64 -p:PublishProfile=SingleFile

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

$output = Join-Path $PSScriptRoot "src\OvertimeTimer.App\bin\Release\net10.0-windows\win-x64\publish"
Write-Host ""
Write-Host "Publish output: $output"
Get-ChildItem $output | Select-Object Name, Length
