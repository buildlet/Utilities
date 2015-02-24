
# Test of 'Get-HashValue' Cmdlet
Describe "Get-HashValue" {

	# Import 'BUILDLet.PowerShell.Utilities.dll' Module
	$PSCommandPath | Split-Path -Parent | Join-Path -ChildPath 'Modules\PackageMaker' | Import-Module -Verbose

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
	Context "Data" {
		It "Should Be" {
			Get-HashValue -Path .\Sources\BUILDLet.PowerShell.Utilities.dll | Write-Host
		}
	}
}