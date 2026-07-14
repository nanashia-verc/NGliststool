Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$solution = Join-Path $PSScriptRoot '..\NgProductManager.sln'
& dotnet build $solution
