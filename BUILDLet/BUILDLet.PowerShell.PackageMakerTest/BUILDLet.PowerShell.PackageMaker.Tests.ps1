@"
BUILDLet PowerShell PackageMaker Toolkit Test Script
Copyright (C) 2015 Daiki Sakamoto

"@

####################################################################################################
##  Settings
####################################################################################################

# Version
$version = '1.0.7.0'

# VerbosePreference
$Script:VerbosePreference = 'SilentlyContinue'
# $Script:VerbosePreference = 'Continue'

# DebugPreference
$Global:DebugPreference = 'SilentlyContinue'
# $Global:DebugPreference = 'Continue'

# ErrorActionPreference
# $ErrorActionPreference = 'Continue'

# Clean
[bool]$clean = $true


# Target Module
$target_module = @{
    'BUILDLet.PowerShell.PackageMaker' = '..\Release\BUILDLet.PowerShell.PackageMaker'
}

# Script Module (to be copied)
$copy_module = @{
}

# Required Module
$required_module = @{
    'BUILDLet.PowerShell.Utilities' = '..\Release\BUILDLet.PowerShell.Utilities'
}

# Test Target Command
$test_target = @{
#
    # Function
    'Get-AuthenticodeTimeStamp' = $true
    'Invoke-SignTool'           = $true
    'New-CatFile'               = $true
    'New-IsoFile'               = $true
#>
}

$load_test_target = @{
#
    'Get-AuthenticodeTimeStamp' = $true
#>
}

# Working Directory
$work_folder = '.\bin\Debug'
$back_path   = '..\..'

# Local Files
$fciv_path        = 'C:\FCIV\fciv.exe'
$signtool_path    = 'C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe'
$inf2cat_path     = 'C:\Program Files (x86)\Windows Kits\8.1\bin\x86\Inf2Cat.exe'
$genisoimage_path = 'C:\cygwin\bin\genisoimage.exe'

####################################################################################################
# Functions only for tests
Function go_to_work   { Set-Location -Path $work_folder }
Function back_to_home { Set-Location -Path $back_path }


####################################################################################################
##  Release
####################################################################################################
'[Release]'
Push-Location ..\BUILDLet.PowerShell.PackageMaker
.\Release.ps1 | Write-Verbose
Pop-Location
'...'
''

####################################################################################################
##  Setup
####################################################################################################
'[Setup]'
. {
    # Import Required Module
    $required_module.Values | % { Import-Module $_ }

    # Copy Module
    $copy_module.Keys | % { Copy-Item -Path $_ -Destination $copy_module[$_] }

    # Import Test Module
    $target_module.Values | % { Import-Module $_ }


    # Reset Working Directory
    if (Test-Path -Path $work_folder -PathType Container) {
        Get-ChildItem -Path $work_folder | % { Remove-Item -Path $_.FullName -Recurse -Force }
    }
    else {
        New-Item -Path $work_folder -ItemType Directory
    }
}
'...'
''

####################################################################################################
##  Tests
####################################################################################################
'[Tests]'

####################################################################################################
# Function Tests

# Get-AuthenticodeTimeStamp
$target = 'Get-AuthenticodeTimeStamp'
if ($test_target[$target]) {

    Describe "$target Function" {

        # Go to working directory
        # go_to_work


	    # Default
	    Context "Default" {

            $expected = @{
                ("$target -FilePath '$fciv_path'") = @('Fri May 14 05:26:01 2004')
                ("$target -FilePath '$fciv_path' -Verbose") = @('Fri May 14 05:26:01 2004')
                ("'$fciv_path', '$fciv_path' | $target") = @('Fri May 14 05:26:01 2004', 'Fri May 14 05:26:01 2004')
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    [string[]]$actual = (Invoke-Expression -Command ($expression = $_))

                    # Assersion
                    $actual.Count | Should be $expected[$expression].Count
                    for ($i = 0; $i -lt $actual.Count; $i++)
                    {
                        ("($i) " + $actual[$i]) | Write-Host
                        $actual[$i] | Should be $expected[$expression][$i]
                    }
                }
            }
	    }


        # Back to original path
        # back_to_home
    }
    ""
}


# Invoke-SignTool
$target = 'Invoke-SignTool'
if ($test_target[$target]) {

    Describe "$target Function" {

        # Go to working directory
        # go_to_work


	    # Default
	    Context "Default" {

            $expression = @(
                "$target -Command 'verify' -Options '/pa' -FilePath '$fciv_path'"
                "$target -Command 'verify' -Options '/pa' -FilePath '$fciv_path' -WhatIf"
                "$target -Command 'verify' -Options '/pa' -FilePath '$fciv_path' -Verbose"
                "$target -Command 'verify' -Options '/pa' -FilePath '$fciv_path' -PassThru"
                "$target -Command 'verify' -Options '/pa' -FilePath '$fciv_path' -PassThru -Verbose"

                "'$fciv_path', '$fciv_path' | $target -Command 'verify' -Options '/pa', '/v' -Verbose"
                "'$fciv_path', '$fciv_path' | $target -Command 'verify' -Options '/pa', '/v' -PassThru -Verbose"

                # "$target -Command 'verify' -Options '/pa', '/v' -FilePath '$fciv_path' -Verbose -Retry 3 -Interval 2"
            )

            $expression | % {
		        It ($_ + ' /') {

                    (Invoke-Expression -Command $_) | % { $_ | Write-Host }
                }
            }
	    }


        # Back to original path
        # back_to_home
    }
    ""
}


# New-CatFile
$target = 'New-CatFile'
if ($test_target[$target]) {

    Describe "$target Function" {

        # Go to working directory
        go_to_work

        # set Package Path
        $source_file = '..\..\..\BUILDLet.PowerShell.PackageMaker\Sample\Sources\Package\DummyPackage.zip'
        $package_path = Expand-ZipFile -Path $source_file -Force -PassThru
        $package_path = $package_path `
            | Join-Path -ChildPath (Split-Path -Path $package_path -Leaf) `
            | Join-Path -ChildPath 'Driver' `
            | Join-Path -ChildPath 'en' `
            | Join-Path -ChildPath 'x86'


	    # Default
	    Context "Default" {

            $expression = @(
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList32' -WhatIf"

                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList32'"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList32' -PassThru"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList32' -Verbose"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList32' -Verbose -PassThru"

                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList64'"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList64' -PassThru"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList64' -Verbose"
                "$target -PackagePath '$package_path' -WindowsVersionList '$Inf2CatWindowsVersionList64' -Verbose -PassThru"
            ) | % {
		        It ($_ + ' /') {

                    (Invoke-Expression -Command $_) | % { $_ | Write-Host }
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# New-IsoFile
$target = 'New-IsoFile'
if ($test_target[$target]) {

    Describe "$target Function" {

        # Go to working directory
        go_to_work

        # Create source file
        $test_dir = 'IsoFileTest'
        $root_dir = Join-Path $test_dir -ChildPath 'RootDirectory'
        Expand-ZipFile -Path ..\..\..\TestData\RootDirectory.zip -DestinationPath . -FolderName $test_dir -Force -PassThru


	    # Default
	    Context "Default" {

            $expression = @(
                "$target -Path $root_dir -DestinationPath $test_dir -FileName '001.iso' -Options `$GenIsoImageOptions -WhatIf"
                "$target -Path $root_dir -DestinationPath $test_dir -FileName '001.iso' -Options `$GenIsoImageOptions -Verbose"
            ) | % {
		        It ($_ + ' /') {

                    (Invoke-Expression -Command $_) | % { $_ | Write-Host }
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# Get-AuthenticodeTimeStamp (Load Test)
$target = 'Get-AuthenticodeTimeStamp'
if ($load_test_target[$target]) {

    Describe "$target Function (Load Test)" {

        # Go to working directory
        # go_to_work


	    # Default
	    Context "Default" {

            $expected = @{
                "$target -FilePath '$fciv_path'" = 'Fri May 14 05:26:01 2004'
            }

            $expected.Keys | % {
		        It (($expression = $_) + ' /') {
                    
                    0..500 | % {
                        "($_) " + ($actual = Invoke-Expression -Command $expression) | Write-Host
                        $actual | Should be $expected[$expression]
                    }
                }
            }
	    }


        # Back to original path
        # back_to_home
    }
    ""
}


# (End of Tests)
'...'
''

####################################################################################################
##  Cleaning
####################################################################################################
'[Cleaning]'
. {
    if ($clean)
    {
        # Remove Test Module
        $target_module.Keys | % { Remove-Module -Name $_ }

        # Remove Required Module
        $required_module.Keys | % { Remove-Module -Name $_ }
    }
    else
    {
        'Cleaning is skipped...' | Write-Verbose
    }
}
'...'
''
