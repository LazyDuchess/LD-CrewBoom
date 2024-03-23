#Requires -Version 7.4
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Set-Location $PSScriptRoot/..
[Environment]::CurrentDirectory = (Get-Location -PSProvider FileSystem).ProviderPath

$csprojPath = "CrewBoom.Mono/CrewBoom.Mono.csproj"
$projxml = [xml](Get-Content -Path $csprojPath)
$version = $projxml.Project.PropertyGroup.Version

function EnsureDir($path) {
    if(!(Test-Path $path)) { New-Item -Type Directory $path > $null }
}

function Clean() {
    if(Test-Path Build) {
        Remove-Item -Recurse Build
    }
}

function CreateZip($zipPath) {
    if(Test-Path $zipPath) { Remove-Item $zipPath }
    $zip = [System.IO.Compression.ZipFile]::Open($zipPath, 'Create')
    return $zip
}

function ExtractZip($zipPath){
    $targetPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($zipPath),[System.IO.Path]::GetFileNameWithoutExtension($zipPath))
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $targetPath)
}

function AddToZip($zip, $path, $pathInZip=$path) {
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $path, $pathInZip) > $Null
}

$projectZipPath = "Build/CrewBoom.Editor-$version.zip"

function CreateProjectZip(){
    $zip = CreateZip $projectZipPath

    Push-Location "CrewBoom.Editor"
    Get-ChildItem -Recurse './' -Exclude *.csproj,*.sln,.gitignore,.vsconfig,*.vs* | ForEach-Object {
        $path = ($_ | Resolve-Path -Relative).Replace('.\', '')
        $doZip = $true
        if($_.FullName -like "*CrewBoom.Editor\Library\*"){
            $doZip = $false
        }
        if($_.FullName -like "*CrewBoom.Editor\UserSettings\*"){
            $doZip = $false
        }
        if($_.FullName -like "*CrewBoom.Editor\obj\*"){
            $doZip = $false
        }
        if($_.FullName -like "*CrewBoom.Editor\Logs\*"){
            $doZip = $false
        }
        if($_.FullName -like "*CrewBoom.Editor\CharacterBundles\*"){
            $doZip = $false
        }
		if($_.FullName -like "*CrewBoom.Editor\Temp\*"){
            $doZip = $false
        }
        if(Test-Path -Path $_.FullName -PathType leaf){
            if($doZip){
			    AddToZip $zip $_.FullName $path
            }
		}
    }
    Pop-Location
    $zip.Dispose()
}

Clean
EnsureDir "Build"
dotnet build -c Release
CreateProjectZip