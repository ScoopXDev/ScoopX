#Requires -Version 5.1
$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $Root

$PublishOut = Join-Path $Root "artifacts\publish\win-x64"
$Iss = Join-Path $Root "installer\ScoopX.iss"
$SetupOut = Join-Path $Root "artifacts\ScoopX-Setup-x64.exe"

Write-Host "==> Publishing ScoopX (unpackaged, win-x64, self-contained)..."
if (Test-Path $PublishOut) { Remove-Item $PublishOut -Recurse -Force }
dotnet publish (Join-Path $Root "ScoopX.csproj") `
  -c Release `
  -p:Platform=x64 `
  -p:WindowsPackageType=None `
  -p:WindowsAppSDKSelfContained=true `
  -p:SelfContained=true `
  -p:RuntimeIdentifier=win-x64 `
  -o $PublishOut
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
if (-not (Test-Path (Join-Path $PublishOut "ScoopX.exe"))) {
  throw "ScoopX.exe missing under $PublishOut"
}

$iscc = @(
  "D:\Program Files\Inno Setup 7\ISCC.exe",
  "${env:LocalAppData}\Programs\Inno Setup 7\ISCC.exe",
  "${env:ProgramFiles}\Inno Setup 7\ISCC.exe",
  "${env:ProgramFiles(x86)}\Inno Setup 7\ISCC.exe",
  "${env:LocalAppData}\Programs\Inno Setup 6\ISCC.exe",
  "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
  "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $iscc) {
  throw "Inno Setup (ISCC.exe) not found. Install from https://jrsoftware.org/isinfo.php"
}

Write-Host "==> Building installer with $iscc ..."
& $iscc $Iss
if ($LASTEXITCODE -ne 0) { throw "ISCC failed" }
if (-not (Test-Path $SetupOut)) { throw "Expected $SetupOut" }

Write-Host "OK: $SetupOut"
