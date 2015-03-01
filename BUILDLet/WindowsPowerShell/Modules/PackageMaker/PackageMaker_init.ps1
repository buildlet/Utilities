# BUILDLet ISO Package Maker Toollkit Initialization Script
# Copyright (C) 2015 Daiki Sakamoto


# Search Path
$Script:assembly = 'BUILDLet.Utilities.dll'
$Script:module   = 'BUILDLet.PowerShell.Utilities.dll'
$Script:assembly_path = $env:ProgramFiles | Join-Path -ChildPath 'BUILDLet' | Join-Path -ChildPath $Script:assembly
$Script:module_path   = $env:ProgramFiles | Join-Path -ChildPath 'WindowsPowerShell\Modules' | Join-Path -ChildPath $Script:assembly


# for debug
if (($DebugPreference -ne 'SilentlyContinue') -and ($VerbosePreference -ne 'SilentlyContinue'))
{
    $Script:assembly_path = Get-Location | Join-Path -ChildPath $Script:assembly
    $Script:module_path   = Get-Location | Join-Path -ChildPath $Script:module
}


# Load Required Assembly
[void][System.Reflection.Assembly]::LoadFrom($Script:assembly_path)

# Import Module
Import-Module -Name $Script:module_path
