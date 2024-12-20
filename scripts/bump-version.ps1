param(
    [switch]$major = $False,
    [switch]$minor = $False,
    [switch]$patch = $False,
    [string]$version,
    [switch]$nogit = $False
)

#Requires -Version 7.4
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Set-Location $PSScriptRoot/..
[Environment]::CurrentDirectory = (Get-Location -PSProvider FileSystem).ProviderPath

if($Null -ne $(git status --untracked-files=no --porcelain=v1)) {
        Write-Error "Git status shows modified files. This script cannot commit a new version while there are uncommitted, modified files."
        return
}

if ($version){
    $major = $False
    $minor = $False
    $patch = $False
}
elseif($major){
    $minor = $False
    $patch = $False
}
elseif($minor){
    $major = $False
    $patch = $False
}
elseif($patch){
    $major = $False
    $minor = $False
}
else{
    Write-Error "Invalid version bump."
    return
}
$csprojPath = "CrewBoom.Mono/CrewBoom.Mono.csproj"

$projxml = [xml](Get-Content -Path $csprojPath)

$oldVersion = $projxml.Project.PropertyGroup[0].Version

$versionArray = $oldVersion.Split(".")

$majorVersion = [int]$versionArray[0]
$minorVersion = [int]$versionArray[1]
$patchVersion = [int]$versionArray[2]

if($major){
    $majorVersion++
    $minorVersion = 0
    $patchVersion = 0
}
elseif($minor){
    $minorVersion++
    $patchVersion = 0
}
else{
    $patchVersion++
}

$newVersion = "$majorVersion.$minorVersion.$patchVersion"

if($version){
    $newVersion = $version
}

Write-Host "Bumping from $oldVersion to $newVersion"

$projxml.Project.PropertyGroup[0].Version = $newVersion
$projxml.Save($csprojPath)

$editorVersionCSPath = "CrewBoom.Editor/Assets/Scripts/Editor/CrewBoomVersion.cs"

$versionString = '
public static class CrewBoomVersion
{
    public const string Version = "'+$newVersion+'";
}
'

Out-File -FilePath $editorVersionCSPath -InputObject $versionString

Write-Host "Bumped all versions!"

if($nogit){
    return;
}

Write-Host "Making Git Tag"

git add $editorVersionCSPath
git add $csprojPath

git commit -m "v$newVersion"
git tag $newVersion

Write-Host -ForegroundColor Green "Don't forget to 'git push --tags'"