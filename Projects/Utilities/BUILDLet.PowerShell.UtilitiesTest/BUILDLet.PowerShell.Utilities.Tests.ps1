@"
BUILDLet.PowerShell.Utilities Test Script
Copyright (C) 2015 Daiki Sakamoto
"@
""

####################################################################################################
##  Settings
####################################################################################################

# Version
$version = '1.1.1.0'

# VerbosePreference
$Script:VerbosePreference = 'SilentlyContinue'
# $Script:VerbosePreference = 'Continue'

# DebugPreference
$Script:DebugPreference = 'SilentlyContinue'
# $Script:DebugPreference = 'Continue'

# ErrorActionPreference
# $ErrorActionPreference = 'Continue'

# Clean
[bool]$clean = $true


# Target Module
$target_module = @{
    'BUILDLet.PowerShell.Utilities' = '..\..\..\Release\Utilities\BUILDLet.PowerShell.Utilities'
}

# Script Module (to be copied)
$copy_module = @{}

# Required Module
$required_module = @{}

# Test Target Command
$test_target = @{
#
    # Variable
    'VerbosePromptLength' = $true

    # Function
    'New-HR'           = $true
    'New-GUID'         = $true
    'New-DateString'   = $true
    'Reset-Directory'  = $true
    'Get-FileVersionInfo' = $true
    'Get-ProductName'     = $true
    'Get-FileDescription' = $true
    'Get-FileVersion'     = $true
    'Get-ProductVersion'  = $true
    'Get-AuthenticodeSignerName' = $true

    # Cmdlet
    'Get-HashValue'    = $true
    'Send-MagicPacket' = $false  # Wake up PC (WOL)
    'Expand-ZipFile'   = $true
    'New-ZipFile'      = $true
    'Get-HtmlString'   = $true
    'Get-PrivateProfileString' = $true
    'Set-PrivateProfileString' = $true
    'Invoke-Process' = $true
#>
}

$load_test_target = @{
#
    'Invoke-Process1' = $true  # Take a few minutes
    'Invoke-Process2' = $true  # Take a few minutes
#>
}

# Working Directory
$work_folder = '.\bin\Debug'
$back_path   = '..\..'

# Local Files
$win32_test_file = 'Win32Test.dll'
$fciv_path = 'C:\FCIV\fciv.exe'

# Test Data Folder (PathInfo)
$test_data_dir = Resolve-Path -Path '..\..\..\TestData'

####################################################################################################
# Functions only for tests
Function go_to_work   { Set-Location -Path $work_folder }
Function back_to_home { Set-Location -Path $back_path }


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
# Variable Tests

# VerbosePromptLength Variable
$target = 'VerbosePromptLength'
if ($test_target[$target]) {

    Describe "$target Variable" {

	    # Version
	    Context "Default" {
            It ('$' + "$target = $VerbosePromptLength /") {
                
                if ((Get-Host).CurrentCulture.Name -eq 'ja-JP') {
                    $expected = ([System.Text.Encoding]::Unicode.GetByteCount('è⁄ç◊') + ': '.Length)
                }
                else { $expected = 'VERBOSE: '.Length }

                $VerbosePromptLength | Should be $expected
            }
	    }
    }
    ""
}


####################################################################################################
# Function Tests

# New-HR Function
$target = 'New-HR'
if ($test_target[$target]) {

    Describe "$target Function" {

	    Context "Default" {

            $expected = @{
                "$target" = (Invoke-Expression -Command "$target")
                "$target -Character '*' -Length 10" = "**********"
                "$target -Length 20 -Offset 10" = "----------"
                "$target -Offset $VerbosePromptLength" = (Invoke-Expression -Command "$target -Offset 6")
            }

            $expected.Keys | % {
		        It ($_ + ' /') {
                    $expected[$_] | Write-Verbose
                    Invoke-Expression -Command $_ | Should be $expected[$_]
                }
            }
        }
    }
    ""
}


# New-GUID Function
$target = 'New-GUID'
if ($test_target[$target]) {

    Describe "$target Function" {

	    Context "Default" {

            $expression = @(
                "$target"
                "$target -Upper"
            ) | % {
		        It ("GUID = {" + ($expected = Invoke-Expression -Command $_) + "} ('$_') /") { '' }
            }
        }
    }
    ""
}


# New-DateString Function
$target = 'New-DateString'
if ($test_target[$target]) {

    Describe "$target Function" {

	    Context "Default" {

            $expression = @(
                "$target",
                "$target -Date '2015/3/21'"
                "$target -Format 'F'"
                "$target -Format 'g'"
                "$target -Format 'yyyy-MM-dd (dddd) HH:mm:ss' -LCID 'en-US'"
                "$target -LCID 'en-US'"
                "$target -LCID 'ja'"
                "$target -LCID 'fr'"
                "$target -LCID 'it'"
                "$target -LCID 'de'"
                "$target -LCID 'es'"
                "$target -LCID 'zh-CN'"
                "$target -LCID 'zh-TW'"
                "$target -LCID 'ru'"
                "$target -LCID 'pl'"
                "$target -LCID 'tr'"
                "$target -LCID 'ar'"
                "$target -LCID 'ar-AE'"
                "$target -LCID 'ar-SA'"
            ) | % {
		        It ("Date String = '" + (Invoke-Expression -Command $_) + "' ('$_') /") { '' }
            }
        }
    }
    ""
}


# Reset-Directory Function
$target = 'Reset-Directory'
if ($test_target[$target]) {

    Describe "$target Function" {

	    Context "Default" {

            # Go to working directory
            go_to_work

            
            $dirs = @(
                "Reset-Directory (1) TestDirectory"
                "Reset-Directory (2) TestDirectory"
                "Reset-Directory (3) AlreadyExistingDirectory"
                "Reset-Directory (4) AlreadyExistingDirectoryIncludingFiles"
            )

            # Create Directories and Files
            New-Item -Path $dirs[0] -ItemType Directory -Force
            New-Item -Path $dirs[1] -ItemType Directory -Force
            New-Item -Path $dirs[2] -ItemType Directory -Force
            New-Item -Path $dirs[3] -ItemType Directory -Force
            'This is only a test.' | Set-Content -Path ($dirs[3] | Join-Path -ChildPath 'test.txt') -Force
            'This is only a test.' | Set-Content -Path ($dirs[3] | Join-Path -ChildPath 'test2.txt') -Force
            New-Item -Path ($dirs[3] | Join-Path -ChildPath 'NestedFolder') -ItemType Directory -Force
            New-Item -Path ($dirs[3] | Join-Path -ChildPath 'NestedFolder2') -ItemType Directory -Force
            'This is only a test.' | Set-Content -Path ($dirs[3] | Join-Path -ChildPath 'NestedFolder2' | Join-Path -ChildPath 'test2.txt') -Force

            $expression = @(
                "$target -Path '" + $dirs[0] + "'"
                "$target -Path '" + $dirs[1] + "'  -PassThru"
                "$target -Path '" + $dirs[2] + "'  -PassThru"
                "$target -Path '" + $dirs[3] + "'"
            ) | % {
		        It ($_ + " /") {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }


            # Back to original path
            back_to_home
        }
    }
    ""
}


# Get-FileVersionInfo / Get-ProductName / Get-FileDescription / Get-FileVersion / Get-ProductVersion Function
if ($test_target['Get-FileVersionInfo'] -or `
    $test_target['Get-ProductName'] -or `
    $test_target['Get-FileDescription'] -or `
    $test_target['Get-FileVersion'] -or `
    $test_target['Get-ProductVersion']) {


    # Go to working directory
    go_to_work

    # Copy test DLL file
    if (-not (Test-Path -Path $win32_test_file)) { $test_data_dir | Join-Path -ChildPath $win32_test_file | Copy-Item } 


    # [START] FileVersionInfo Functions
    Describe "FileVersionInfo Functions" {

        # Get-FileVersionInfo Function
        if ($test_target[($target = 'Get-FileVersionInfo')]) {

	        Context "$target Function" {

                $expression = "$target -FilePath '$win32_test_file'"

		        It "FileVersionInfo ($expression) /" {
                    "`n" + (Invoke-Expression -Command $expression) | Write-Verbose
                }
            }
            ""
        }


        # Get-ProcuctName Function
        if ($test_target[($target = 'Get-ProductName')]) {

	        Context "$target Function" {
                
                $expression = "$target -FilePath '$win32_test_file'"
                $expected = 'BUILDLet Win32 Test DLL'

		        It ("ProcuctName = '" + ($actual = Invoke-Expression -Command $expression) + "' ($expression) /") {
                    $actual | Should be $expected
                }
            }
            ""
        }


        # Get-FileDescription Function
        if ($test_target[($target = 'Get-FileDescription')]) {

	        Context "$target Function" {
                
                $expression = "$target -FilePath '$win32_test_file'"
                $expected = 'BUILDLet Win32 DLL File for Test of FileVersionInfo'

		        It ("FileDescription = '" + ($actual = Invoke-Expression -Command $expression) + "' ($expression) /") {
                    $actual | Should be $expected
                }
            }
            ""
        }


        # Get-FileVersion Function
        if ($test_target[($target = 'Get-FileVersion')]) {

	        Context "$target Function" {

                $expression = "$target -FilePath '$win32_test_file'"
                $expected = '1.10.100.1000'

		        It ("FileVersion = '" + ($actual = Invoke-Expression -Command $expression) + "' ($expression) /") {
                    $actual | Should be $expected
                }
            }
            ""
        }


        # Get-ProductVersion Function
        if ($test_target[($target = 'Get-ProductVersion')]) {

	        Context "$target Function" {
                
                $expression = "$target -FilePath '$win32_test_file'"
                $expected = '1.2.3.4'

		        It ("ProductVersion = '" + ($actual = Invoke-Expression -Command $expression) + "' ($expression) /") {
                    $actual | Should be $expected
                }
            }
            ""
        }
    }
    # [END] FileVersionInfo Functions


    # Back to original path
    back_to_home
}


# Get-AuthenticodeSinerName Function
$target = 'Get-AuthenticodeSignerName'
if ($test_target[$target]) {

    Describe "$target Function" {

	    Context "Default" {
            
            $expression = "$target -FilePath '$fciv_path'"
            $expected = 'Microsoft Corporation'

		    It ("Authenticode: Issued by '" + ($actual = Invoke-Expression -Command $expression) + "' ($expression) /") {
                $actual | Should be $expected
            }
        }
    }
    ""
}


####################################################################################################
# Cmdlet Tests

# Get-HashValue Cmdlet
$target = 'Get-HashValue'
if ($test_target[$target]) {

    Describe "$target Cmdlet" {

	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            Write-Verbose (
                "Help Message is ...`n" +
                (New-HR) + 
                ($expected = Get-HashValue) +
                (New-HR)
            )
        
            $expression = @(
                "$target",
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Should be $expected
                }
            }
	    }


	    # File
	    Context "File" {

            $test_files = @{
                'Relative Path' = (Join-Path -Path $target_module["BUILDLet.PowerShell.Utilities"] -ChildPath 'BUILDLet.PowerShell.Utilities.dll')
                'Absolute Path' = 'C:\FCIV\fciv.exe'
            }

            $test_files.Keys | % {
            
                $test_file = $test_files[$_]
                "Test File = '$test_file'" | Write-Verbose

                # Actual
                ($actual = Invoke-Expression -Command ($expression = "$target -Path $test_file")) | Write-Verbose

                # Expected (FCIV)
                ($expected = . "$fciv_path" "$test_file") | Write-Verbose
                $expected = $expected[3].Split(' ')[0]

                "Actual:`t" + $actual | Write-Verbose
                "Expected:`t" + $expected | Write-Verbose

		        It "Compare hash value of a file ($_) by FCIV /" { $actual | Should be $expected }
            }
	    }


	    # Folder
	    Context "Folder" {

            $test_folders = @{
                'Relative Path' = $target_module["BUILDLet.PowerShell.Utilities"]
                'Absolute Path' = 'C:\FCIV'
            }

            $test_folders.Keys | % {
            
                $kind_of_path = $_
                $test_folder = $test_folders[$kind_of_path]
                "Test Folder = '$test_folder'" | Write-Verbose

                # Actual
                ([string[]]$actuals = Invoke-Expression -Command ($expression = "$target -Path $test_folder")) | Write-Verbose
                [string[]]$actual = @()
                $actuals | % { $actual += $_.Split("`t")[0] }

                # Expected (FCIV)
                ([string[]]$expecteds = . "$fciv_path" "$test_folder") | Write-Verbose
                [string[]]$expected = @()
                3..($expecteds.Count - 1) | % { $expected += $expecteds[$_].Split(' ')[0] }


                "Actual:`t" | Write-Verbose
                $actual | % { "`t" + $_ | Write-Verbose }

                "Expected:`t" | Write-Verbose
                $expected | % { "`t" + $_ | Write-Verbose }

		        It "Compare hash value of a folder ($kind_of_path) by FCIV /" {
                    0..$actual.Count | % { $actual[$_] | Should be $expected[$_] }
                }
            }
	    }

	    # Data
	    Context "Data" {
            
            # Go to working directory
            go_to_work

            # Test Data
            $test_data = @{
                "test.bin" = [byte[]]@(1, 2, 3)
            }

            # Test Data To File
            $test_data.Keys | % {
                $test_data[$_] | Set-Content -Encoding Byte -Path $_
            }

            # Expected
            $expected = (Get-FileHash -Path "test.bin" -Algorithm MD5).Hash


		    It 'Default /' {
                'Expected : ' + $expected | Write-Verbose
                'Actual   : ' + ($actual = Get-HashValue -InputObject $test_data["test.bin"]) | Write-Verbose
                $actual | Should be $expected
            }

		    It 'Pipeline /' {
                'Expected : ' + $expected | Write-Verbose
                'Actual   : ' + ($actual = $test_data["test.bin"] | Get-HashValue) | Write-Verbose
                $actual | Should be $expected
            }


            # Back to original path
            back_to_home
        }
    }
    ""
}


# Send-MagicPacket Cmdlet
$target = 'Send-MagicPacket'
if ($test_target[$target]) {

    Describe "$target Cmdlet" {
    
	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            $expression = @(
                "$target"
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


	    # Default
	    Context "Default" {
        
            $mac_address = Get-Content -Path '..\..\..\..\BUILDLet\WOL.conf'
            $expression = "$target -MacAddress $mac_address -Times 3 -PassThru"

            It ($expression + ' /') {

                $text = 'Magic Packet = 0x'
                ($packet = Invoke-Expression -Command $expression) | % {
                    $text += [string]::Format('{0:X2}',$_)
                }
                $text | Write-Verbose
            }
	    }
    }
    ""
}


# Expand-ZipFile / New-ZipFile Cmdlet
$target = 'Expand-ZipFile / New-ZipFile'
if ($test_target['Expand-ZipFile'] -or $test_target['New-ZipFile']) {

    Describe "$target Cmdlet" {

        $cmdlets = @(
            'Expand-ZipFile'
            'New-ZipFile'
        )


	    # Version
	    Context "Version" {

            $cmdlets | % {
		        It ('Version = ' + ($actual = (Invoke-Expression -Command "$_ -Version")) + " ($_) /") {
                    $actual | Should be ($expected = $version)
                }
            }
	    }


	    # Help
	    Context "Help" {

            $cmdlets | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Go to working directory
        go_to_work


        # Directories to be created
        $unzipped_dirs = @(
            'Expand-ZipFile (1) Unzipped'
            'Expand-ZipFile (2) Unzipped\NestedFolder'
        )

        # Create Directories
        $unzipped_dirs  | % { New-Item -Path "$_" -ItemType Directory -Force }

        # Add Directories
        $unzipped_dirs += 'Expand-ZipFile (3) Root'
        $unzipped_dirs += 'Expand-ZipFile (4) Root'


	    # Unzip
	    Context "Unzip" {
            
            $zip_files = @(
                (Join-Path -Path $test_data_dir -ChildPath 'RootDirectory.zip')
                (Join-Path -Path $test_data_dir -ChildPath 'hello.zip')
            )

            $expressions = @(
                $cmdlets[0] + " -Path '" + $zip_files[0] + "'"
                $cmdlets[0] + " -Path '" + $zip_files[0] + "' -DestinationPath '" + $unzipped_dirs[0] + "'"
                $cmdlets[0] + " -Path '" + $zip_files[0] + "' -DestinationPath '" + $unzipped_dirs[1] + "'"
                $cmdlets[0] + " -Path '" + $zip_files[0] + "' -FolderName '" + $unzipped_dirs[2] + "'"
                $cmdlets[0] + " -Path '" + $zip_files[1] + "'"
                $cmdlets[0] + " -Path '" + $zip_files[1] + "' -FolderName '" + $unzipped_dirs[3] + "'"
            ) | % {
                It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Directories to be created
        $zip_dirs = @(
            'New-ZipFile (1) Zipped'
            'New-ZipFile (2) FileZipped'
        )

        # Create Directories
        $zip_dirs  | % { New-Item -Path "$_" -ItemType Directory -Force }


	    # Zip
	    Context "Zip" {
            
            $expressions = @(
                $cmdlets[1] + " -Path 'RootDirectory\RootDirectory'"
                $cmdlets[1] + " -Path 'RootDirectory\RootDirectory' -DestinationPath '" + $zip_dirs[0] + "'"
                $cmdlets[1] + " -Path 'RootDirectory\RootDirectory' -FileName '(1) Zipped.zip'"
                $cmdlets[1] + " -Path 'RootDirectory\RootDirectory' -FileName '(2) Root.zip' -NotIncludeBaseDirectory"
                $cmdlets[1] + " -Path 'RootDirectory\RootDirectory' -FileName '(3) Optimal.zip' -CompressionLevel 'Optimal'"
                $cmdlets[1] + " -Path 'hello'"
                $cmdlets[1] + " -Path 'hello' -FileName '(4) Root.zip' -NotIncludeBaseDirectory"
                $cmdlets[1] + " -Path 'hello\hello.txt' -DestinationPath '" + $zip_dirs[1] + "'"
            ) | % {
                It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# Get-HtmlString
$target = 'Get-HtmlString'
if ($test_target[$target]) {


    Describe "$target Cmdlet" {
    
	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            $expression = @(
                "$target"
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Go to working directory
        go_to_work


        # Test Files
        $html_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'W3C_example.html')
            (Join-Path -Path $test_data_dir -ChildPath 'Ç∆ÇŸÇŸ_ÉTÉìÉvÉã.html')
            (Join-Path -Path $test_data_dir -ChildPath 'HTML 4_01 Specification.htm')
            (Join-Path -Path $test_data_dir -ChildPath 'comment.html')
        )


	    # Element
	    Context "Element" {

            $expected = @{
                ("$target -Path '" + $html_files[0] + "' -Name 'TITLE'") = @('My first HTML document')
                ("$target -Path '" + $html_files[0] + "' -Name 'P'") = @('Hello world!')
                ("$target -Path '" + $html_files[1] + "' -Name 'P'") = @('ñ{ï∂')
                ("$target -Path '" + $html_files[2] + "' -Name 'H2' -Encoding ([System.Text.Encoding]::Default) -Strict") = @(
                    'W3C Recommendation 24 December 1999'
                    'Abstract'
                    'Status of this document'
                    '<A name="minitoc">Quick Table of Contents</A>'
                    '<A name="toc">Full Table of Contents</A>'
                )

                ("$target -Path '" + $html_files[3] + "' -Name 'p' -Strict") = @('Hello, world!')
                ("$target -Path '" + $html_files[3] + "' -Name 'p'") = @('Hello, world!')

                ("$target -Path '" + $html_files[3] + "' -Name 'TR' -Strict") = @('<TD>Table</TD>')
                ("$target -Path '" + $html_files[3] + "' -Name 'TR'") = @('')
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    $actual = (Invoke-Expression -Command ($expression = $_))

                    # Assersion
                    $actual.Count | Should be $expected[$expression].Count
                    for ($i = 0; $i -lt $actual.Count; $i++)
                    {
                        ("($i) " + $actual[$i]) | Write-Verbose
                        $actual[$i] | Should be $expected[$expression][$i]
                    }
                }
            }
	    }


	    # Attribute
	    Context "Attribute" {

            $expected = @{
                ("$target -Path '" + $html_files[2] + "' -Name 'OL' -Attribute 'type' -Encoding ([System.Text.Encoding]::Default) -Strict") = `
                    @('"A"', '"A"')
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    $actual = Invoke-Expression -Command ($expression = $_)

                    # Assersion
                    $actual.Count | Should be $expected[$expression].Count
                    for ($i = 0; $i -lt $actual.Count; $i++)
                    {
                        ("($i) " + $actual[$i]) | Write-Verbose
                        $actual[$i] | Should be $expected[$expression][$i]
                    }
                }
            }
	    }


	    # Pipeline
	    Context "Pipeline" {

            $expected = @{
                ("Get-Content '" + $html_files[0] + "' | $target -Name 'TITLE'") = @('My first HTML document')
                ("Get-Content '" + $html_files[2] + "' | $target -Name 'H2' -Strict") = @(
                    'W3C Recommendation 24 December 1999'
                    'Abstract'
                    'Status of this document'
                    '<A name="minitoc">Quick Table of Contents</A>'
                    '<A name="toc">Full Table of Contents</A>'
                )
                ("Get-Content '" + $html_files[2] + "' | $target -Name 'OL' -Attribute 'type'") = @('"A"', '"A"')
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    $actual = Invoke-Expression -Command ($expression = $_)

                    # Assersion
                    $actual.Count | Should be $expected[$expression].Count
                    for ($i = 0; $i -lt $actual.Count; $i++)
                    {
                        ("($i) " + $actual[$i]) | Write-Verbose
                        $actual[$i] | Should be $expected[$expression][$i]
                    }
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# Get-PrivateProfileString
$target = 'Get-PrivateProfileString'
if ($test_target[$target]) {


    Describe "$target Cmdlet" {
    
	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            $expression = @(
                "$target"
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Go to working directory
        go_to_work


        # Test Files
        $ini_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'test.ini')
        )


	    # Default
	    Context "Default" {

            $expected = @{
                ("$target -Path '" + $ini_files[0] + "' -Section 'section1' -Key 'key1'") = 'value1'
                ("$target -Path '" + $ini_files[0] + "' -Section 'SECTION4' -Key 'KEY7'") = @('1', '2', '3')

                ("Get-Content '" + $ini_files[0] + "' | $target -Section 'SECTION2' -Key 'KEY3'") = 'VALUE3'
                ("Get-Content '" + $ini_files[0] + "' | $target -Section 'section4' -Key 'key5'") = @('value5.1', '5.2', '5.3')
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    $actual = (Invoke-Expression -Command ($expression = $_))

                    # Assersion
                    if ($expected[$expression] -is [string]) {
                        $actual | Write-Verbose
                        $actual | Should be $expected[$expression]
                    }
                    else {
                        $actual = $actual.Split(',')
                        $actual.Count | Should be $expected[$expression].Count
                        for ($i = 0; $i -lt $actual.Count; $i++)
                        {
                            ("($i) " + $actual[$i].Trim()) | Write-Verbose
                            $actual[$i].Trim() | Should be $expected[$expression][$i]
                        }
                    }
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# Set-PrivateProfileString
$target = 'Set-PrivateProfileString'
if ($test_target[$target]) {


    Describe "$target Cmdlet" {
    
	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            $expression = @(
                "$target"
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Go to working directory
        go_to_work


        # Test Files
        $ini_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'test.ini')
        )


        # Copy INI file to be overwritten
        if (Test-Path -Path '.\copy.ini') { Remove-Item '.\copy.ini' -Force }
        $ini_files += (Copy-Item -Path $ini_files[0] -Destination '.\copy.ini' -PassThru)


	    # Default
	    Context "Default" {

            $updated = (Get-Content -Path $ini_files[0]) -creplace 'Value1', 'UpdatedValue1'
            $expected = @{
                ("$target -InputObject (Get-Content -Path '" + $ini_files[0] + "') -Section 'Section1' -Key 'Key1' -Value 'UpdatedValue1'") = $updated
                ("Get-Content '" + $ini_files[0] + "' | $target -Section 'Section1' -Key 'Key1' -Value 'UpdatedValue1'") = $updated

                ("$target -Path '" + $ini_files[1] + "' -Section 'Section1' -Key 'Key1' -Value 'UpdatedValue1'") = $null
                ("$target -Path '" + $ini_files[1] + "' -Section 'SECTION2' -Key 'KEY3' -Value 'UpdatedValue3'") = $null
                ("$target -Path '" + $ini_files[1] + "' -Section 'SECTION2' -Key 'Key6' -Value 'AppendValue6'") = $null
                ("$target -Path '" + $ini_files[1] + "' -Section 'Section6' -Key 'Key12' -Value 'NewValue12'") = $null
            }

            $expected.Keys | % {
		        It ($_ + ' /') {

                    # Execution
                    $actual = Invoke-Expression -Command ($expression = $_)

                    # Assersion (partly)
                    if ($expected[$expression] -ne $null)
                    {
                        $actual.Count | Should be $expected[$expression].Count

                        <#
                        "[Expected]:" | Write-Verbose
                        New-HR -Offset $VerbosePromptLength | Write-Verbose
                        for ($i = 0; $i -lt $expected[$expression].Count; $i++)
                        {
                            ("($i) " + $expected[$expression][$i]) | Write-Verbose
                        }
                        Write-Verbose ""
                        #>

                        "[Actual]:" | Write-Verbose
                        New-HR -Offset $VerbosePromptLength | Write-Verbose
                        for ($i = 0; $i -lt $actual.Count; $i++)
                        {
                            ("($i) " + $actual[$i]) | Write-Verbose
                            $actual[$i] | Should be $expected[$expression][$i]
                        }
                        New-HR -Offset $VerbosePromptLength | Write-Verbose
                    }
                }
            }
	    }


        # Back to original path
        back_to_home
    }
    ""
}


# Invoke-Process
$target = 'Invoke-Process'
if ($test_target[$target]) {


    Describe "$target Cmdlet" {
    
	    # Version
	    Context "Version" {
            It ('Version = ' + ($actual = (Invoke-Expression -Command ($expression = "$target -Version"))) + ' /') {
                $actual | Should be ($expected = $version)
            }
	    }


	    # Help
	    Context "Help" {

            $expression = @(
                "$target"
                "$target -Help"
            ) | % {
		        It ($_ + ' /') {
                    Invoke-Expression -Command $_ | Write-Verbose
                }
            }
	    }


        # Go to working directory
        go_to_work


        # Test Files
        $exe_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'exit999.exe')
        )

        # Backup and Set VerbosePreference
        $backup_VerbosePreference = $Script:VerbosePreference
        $Script:VerbosePreference = 'SilentlyContinue'


	    # Default
	    Context "Default" {

            $expression = @(
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -PassThru -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -RedirectStandardOutputToWarning -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -RedirectStandardErrorToOutput -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -RedirectStandardErrorToVerbose -Verbose -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -RedirectStandardErrorToVerbose -Verbose -RedirectStandardErrorToOutput -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -PassThru -RedirectStandardErrorToOutput -WhatIf"
                "$target -FilePath c:\dummy.exe -ArgumentList 1, 2, 3 -PassThru -RedirectStandardOutputToWarning -RedirectStandardErrorToOutput -WhatIf"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"'"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -Verbose"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -PassThru"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -Verbose -PassThru"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -RedirectStandardOutputToWarning"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -RedirectStandardOutputToWarning -Verbose"
                "$target -FilePath '" + $fciv_path + "' -ArgumentList '`"" + $exe_files[0] + "`"' -RedirectStandardOutputToWarning -Verbose -PassThru"
                "$target -FilePath '" + $exe_files[0] + "' -Retry 3 -Interval 2 -Verbose"
            )


            for ($i = 0; $i -lt $expression.Count; $i++)
            {
		        It ($expression[$i] + ' /') {

                    Write-Host
                    New-HR | Write-Host

                    # Invoke
                    Invoke-Expression -Command $expression[$i] | Write-Host
                }
            }
	    }


        # Restore VerbosePreference
        $Script:VerbosePreference = $backup_VerbosePreference

        # Back to original path
        back_to_home
    }
    ""
}


# Invoke-Process (Load Test 1)
$target = 'Invoke-Process'
if ($load_test_target[$target + '1']) {

    Describe "$target Cmdlet (Load Test 1)" {


        # Test Files
        $exe_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'stdout300.exe')
            (Join-Path -Path $test_data_dir -ChildPath 'stderr300.exe')
            (Join-Path -Path $test_data_dir -ChildPath 'randum300.exe')
        )

        # Backup and Set VerbosePreference
        $backup_VerbosePreference = $Script:VerbosePreference
        $Script:VerbosePreference = 'SilentlyContinue'


	    # Default
	    Context "Default" {

            $expression = @(
                ("$target -FilePath '" + $exe_files[0] + "' -PassThru -OutVariable actual")
                ("$target -FilePath '" + $exe_files[1] + "' -PassThru -RedirectStandardErrorToOutput -OutVariable actual")
            )

            for ($i = 0; $i -lt 5; $i++) {

                for ($j = 0; $j -lt $expression.Count; $j++)
                {
		            It ($expression[$j] + ' /') {
                    
                        # Expected
                        [string[]]$expected = @()
                        1..300 | % { $expected += $_ }

                        
                        # Actual
                        $actual = $null
                        Invoke-Expression -Command $expression[$j] | Write-Host


                        # Assersion
                        $actual.Count | Should be $expected.Count
                        for ($k = 0; $k -lt $actual.Count; $k++)
                        {
                            '(' + ($i + 1) + ', ' +  ($k + 1) + ') ' + $actual[$k] | Write-Host
                            $actual[$k] | Should be $expected[$k]
                        }
                    }
                }
            }
        }


        # Restore VerbosePreference
        $Script:VerbosePreference = $backup_VerbosePreference
    }
    ""
}


# Invoke-Process (Load Test 2)
$target = 'Invoke-Process'
if ($load_test_target[$target + '2']) {

    Describe "$target Cmdlet (Load Test 2)" {


        # Test Files
        $exe_files = @(
            (Join-Path -Path $test_data_dir -ChildPath 'stdout300.exe')
            (Join-Path -Path $test_data_dir -ChildPath 'stderr300.exe')
            (Join-Path -Path $test_data_dir -ChildPath 'randum300.exe')
        )

        # Backup and Set VerbosePreference
        $backup_VerbosePreference = $Script:VerbosePreference
        $Script:VerbosePreference = 'SilentlyContinue'


	    # Default
	    Context "Default" {

            $expression = @(
                "$target -FilePath '" + $exe_files[0] + "'"
                "$target -FilePath '" + $exe_files[0] + "' -Verbose"
                "$target -FilePath '" + $exe_files[0] + "' -PassThru"
                "$target -FilePath '" + $exe_files[0] + "' -Verbose -PassThru"
                "$target -FilePath '" + $exe_files[0] + "' -Verbose -PassThru -RedirectStandardOutputToWarning"

                "$target -FilePath '" + $exe_files[1] + "' -Verbose"
                "$target -FilePath '" + $exe_files[1] + "' -Verbose -RedirectStandardErrorToOutput"
                "$target -FilePath '" + $exe_files[1] + "' -Verbose -PassThru"
                "$target -FilePath '" + $exe_files[1] + "' -Verbose -PassThru -RedirectStandardErrorToOutput"
                "$target -FilePath '" + $exe_files[1] + "' -Verbose -PassThru -RedirectStandardErrorToVerbose"

                "$target -FilePath '" + $exe_files[2] + "' -Verbose"
                "$target -FilePath '" + $exe_files[2] + "' -Verbose -RedirectStandardErrorToOutput"
                "$target -FilePath '" + $exe_files[2] + "' -Verbose -PassThru"
                "$target -FilePath '" + $exe_files[2] + "' -Verbose -PassThru -RedirectStandardOutputToWarning"
                "$target -FilePath '" + $exe_files[2] + "' -Verbose -PassThru -RedirectStandardErrorToOutput"
                "$target -FilePath '" + $exe_files[2] + "' -Verbose -PassThru -RedirectStandardErrorToVerbose"
            )


            for ($i = 0; $i -lt $expression.Count; $i++)
            {
		        It ($expression[$i] + ' /') {

                    Write-Host
                    ("[" + [string]::Format("{0:000}", $i) + "] " + $expression[$i]) | Write-Host
                    (' ' * '[000] '.Length) + 'Please wait ' + ($wait_sec = 3) + ' second(s)...' | Write-Host
                    New-HR | Write-Host


                    # Wait
                    sleep $wait_sec

                    # Invoke
                    Invoke-Expression -Command $expression[$i] | Write-Host


                    New-HR | Write-Host
                }
            }
	    }


        # Restore VerbosePreference
        $Script:VerbosePreference = $backup_VerbosePreference
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
