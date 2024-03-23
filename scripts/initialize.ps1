#Requires -Version 7.4
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Set-Location $PSScriptRoot/..
[Environment]::CurrentDirectory = (Get-Location -PSProvider FileSystem).ProviderPath

dotnet build -c Release