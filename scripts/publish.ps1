Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot '..\src\NgProductManager\NgProductManager.csproj'
$output = Join-Path $PSScriptRoot '..\publish'

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}

New-Item -ItemType Directory -Path $output -Force | Out-Null
& dotnet publish $project -c Release -o $output
