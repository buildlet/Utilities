# BUILDLet ISO Package Maker Toollkit Initialization Script
# Copyright (C) 2015 Daiki Sakamoto

# Load Required Assembly
$lib = "BUILDLet.Utilities.dll"
$lib = $PSCommandPath | Split-Path -Parent | Join-Path -ChildPath "..\..\Libraries" | Join-Path -ChildPath $lib
[void][System.Reflection.Assembly]::LoadFrom($lib)

# Load Module
$module = "BUILDLet.PowerShell.Utilities.dll"
Import-Module -Name $PSCommandPath | Split-Path -Parent | Join-Path -ChildPath $module
