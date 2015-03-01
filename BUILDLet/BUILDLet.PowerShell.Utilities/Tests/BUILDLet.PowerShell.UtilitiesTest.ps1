@"
BUILDLet.PowerShell.Utilities Test Script
Copyright (C) 2015 Daiki Sakamoto
"@

# Setup
. {
    "`n[Setup]"

    # set Release Directory
    $releaseDir = $PSCommandPath | Split-Path -Parent | Join-Path -ChildPath "..\..\Release"

    # Load Required Assembly
    $lib = "BUILDLet.Utilities.dll"
    [void][System.Reflection.Assembly]::LoadFrom((Join-Path -Path $releaseDir -ChildPath $lib))

    # Import Module
    $module = "BUILDLet.PowerShell.Utilities"
    $releaseDir | Join-Path -ChildPath ($module + ".dll") | Import-Module -Verbose
}


"`n[Test]"

# Test of 'Get-HashValue' Cmdlet
Describe "Get-HashValue" {

	# Help
	Context "Help" {
		It "Should be help message w/o parameter." {
			Get-HashValue | Write-Host
		}

		It "Should be help message w/ 'Help' parameter." {
			Get-HashValue -Help | Write-Host
		}
	}


	# Data

    $testfile = "..\..\Release\WOLSetup.exe"

	Context "File" {
		It "Should Be" {
			Get-HashValue -Path $testfile | Write-Host
		}
	}
}


# Cleaning
. {
    "`n[Cleaning]"

    # Remove Module
    Remove-Module -Name $module -Verbose
}
