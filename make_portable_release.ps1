param(
    [string]$Configuration = "Release",
    [switch]$Obfuscate = $true,
    [string]$CodeSignThumbprint = ""
)

$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "TESTS.csproj"
$outputDir = Join-Path $PSScriptRoot "bin\$Configuration"
$obfuscatorConfigPath = Join-Path $PSScriptRoot "obfuscar.xml"
$obfuscatedDir = Join-Path $outputDir "obfuscated"
$exeSourcePath = Join-Path $outputDir "TESTS.exe"
$distRoot = Join-Path $PSScriptRoot "dist"
$distDir = Join-Path $distRoot "TM7_Tests_Trainer_portable"

function Find-CodeSigningCertificate([string]$thumbprint)
{
    if ([string]::IsNullOrWhiteSpace($thumbprint)) {
        return $null
    }

    $cleanThumbprint = ($thumbprint -replace "\\s", "").ToUpperInvariant()
    $cert = Get-ChildItem Cert:\CurrentUser\My |
        Where-Object { $_.Thumbprint -eq $cleanThumbprint } |
        Select-Object -First 1

    if ($cert -eq $null) {
        $cert = Get-ChildItem Cert:\LocalMachine\My |
            Where-Object { $_.Thumbprint -eq $cleanThumbprint } |
            Select-Object -First 1
    }

    return $cert
}

Write-Host "Building $Configuration..."
dotnet msbuild $projectPath /t:Rebuild /p:Configuration=$Configuration | Out-Host

if (!(Test-Path $outputDir)) {
    throw "Build output not found: $outputDir"
}

if (!(Test-Path $exeSourcePath)) {
    throw "Main executable not found: $exeSourcePath"
}

if ($Obfuscate) {
    if (!(Test-Path $obfuscatorConfigPath)) {
        throw "Obfuscator config not found: $obfuscatorConfigPath"
    }

    if (Test-Path $obfuscatedDir) {
        Remove-Item $obfuscatedDir -Recurse -Force
    }

    Write-Host "Running Obfuscar..."
    dotnet obfuscar.console $obfuscatorConfigPath | Out-Host

    $obfuscatedExePath = Join-Path $obfuscatedDir "TESTS.exe"
    if (!(Test-Path $obfuscatedExePath)) {
        throw "Obfuscated executable not found: $obfuscatedExePath"
    }

    $exeSourcePath = $obfuscatedExePath
}
else {
    Write-Host "Obfuscation disabled."
}

if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}

New-Item -Path $distDir -ItemType Directory | Out-Null

Copy-Item $exeSourcePath (Join-Path $distDir "TESTS.exe") -Force

$filesToCopy = @(
    "TESTS.exe.config",
    "LiteDB.dll",
    "tests_secure.db"
)

foreach ($file in $filesToCopy) {
    $src = Join-Path $outputDir $file
    if (Test-Path $src) {
        Copy-Item $src $distDir -Force
    }
}

if (![string]::IsNullOrWhiteSpace($CodeSignThumbprint)) {
    $certificate = Find-CodeSigningCertificate $CodeSignThumbprint
    if ($certificate -eq $null) {
        throw "Certificate with thumbprint '$CodeSignThumbprint' was not found in CurrentUser/LocalMachine stores."
    }

    $distExePath = Join-Path $distDir "TESTS.exe"
    Write-Host "Signing executable with certificate $($certificate.Thumbprint)..."
    $signature = Set-AuthenticodeSignature -FilePath $distExePath -Certificate $certificate -HashAlgorithm SHA256
    if ($signature.Status -ne "Valid") {
        throw "Code signing failed. Status: $($signature.Status)"
    }
}
else {
    Write-Host "Code signing skipped (trusted certificate not provided)."
}

$zipPath = Join-Path $distRoot "TM7_Tests_Trainer_portable.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Compress-Archive -Path (Join-Path $distDir "*") -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "Portable folder: $distDir"
Write-Host "Portable zip:    $zipPath"
