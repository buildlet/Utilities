@"
PakageMaker Module Test Script
Copyright (C) 2015 Daiki Sakamoto
"@


# Test Module
$test_module = 'PackageMaker'


# Setup
. {
    "`n[Setup]"

    # copy DLLs
    $releaseDir = $PSCommandPath | Split-Path -Parent | Join-Path -ChildPath ..\Release
    $lib = (
        'BUILDLet.Utilities.dll',
        'BUILDLet.PowerShell.Utilities.dll'
    ) | % {
        if (-not (Test-Path -Path (Get-Location | Join-Path -ChildPath $_)))
        {
            Copy-Item ($releaseDir | Join-Path -ChildPath $_) . -Force -Verbose
        }
    }


    # set DebugPreference / VerbosePreference
    $DebugPreference = 'Stop'
    $VerbosePreference = 'Continue'

    # import Module
    Import-Module .\Modules\PackageMaker -Debug -Verbose


    # import Pester
    if ((Get-Module -Name Pester) -eq $null) { Import-Module Pester }
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

    $testfile = ".\BUILDLet.Utilities.dll"

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
    Remove-Module -Name $test_module -Verbose
}
