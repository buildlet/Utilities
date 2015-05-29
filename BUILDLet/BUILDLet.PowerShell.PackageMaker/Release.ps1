@"
BUILDLet.PowerShell.PackageMaker Release Script
Copyright (C) 2015 Daiki Sakamoto
"@

$module = 'BUILDLet.PowerShell.PackageMaker'
$files = @(
    'BUILDLet.PowerShell.PackageMaker.psd1'
    'BUILDLet.PowerShell.PackageMaker.psm1'
)

# Directory
$directory = Join-Path -Path '..\Release' -ChildPath $module
if (-not (Test-Path -Path $directory)) { New-Item -Path $directory -ItemType Directory }
''
# File Copy
"[RELEASE] $module -> " + (Get-Item -Path $directory).FullName
$files | % { Copy-Item -Path $_ -Destination $directory -Force }
