<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

 Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
################################################################################>

#
# This is a PowerShell Unit Test file.
# You need a unit test framework such as Pester to run PowerShell Unit tests. 
# You can download Pester from http://go.microsoft.com/fwlink/?LinkID=534084
#

# Initialize
. ($PSCommandPath | Split-Path -Parent | Join-Path -ChildPath 'Initialize.ps1')


# TestData Directory
$Script:dirpath_TestData = '..\..\..\TestData'

# Win32 Test DLL
$Script:filename_Original_Win32TestDLL = 'BUILDLet.Win32TestData.Win32TestDLL.dll'
$Script:filepath_Original_Win32TestDLL = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Original_Win32TestDLL

$Script:filename_Target_Win32TestDLL = 'Win32Test.dll'
$Script:filepath_Target_Win32TestDLL = Get-Location | Join-Path -ChildPath $Script:filename_Target_Win32TestDLL

$Script:Win32DLL_ProductName = 'BUILDLet Win32 Test DLL'
$Script:Win32DLL_FileDescription = 'This is FileDescription of BUILDLet.Win32TestData.Win32TestDLL.dll'
$Script:Win32DLL_FileVersion = '1.23.456.7890'
$Script:Win32DLL_ProductVersion = '1.4.0.0'


# Copy Test DLL
if (-not ($Script:filepath_Target_Win32TestDLL | Test-Path)) {
	Copy-Item -Path $Script:filepath_Original_Win32TestDLL -Destination $Script:filepath_Target_Win32TestDLL -Force
}


Describe "Get-FileVersionInfo" {

	# Execution
	[System.Diagnostics.FileVersionInfo]$version_info = Get-FileVersionInfo -Path $Script:filepath_Target_Win32TestDLL

	# ProductName
	Context "ProductName of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_ProductName'." { $version_info.ProductName | Should Be $Script:Win32DLL_ProductName }
	}

	# FileDescription
	Context "FileDescription of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_FileDescription'." { $version_info.FileDescription | Should Be $Script:Win32DLL_FileDescription }
	}

	# FileVersion
	Context "FileVersion of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_FileVersion'." { $version_info.FileVersion | Should Be $Script:Win32DLL_FileVersion }
	}

	# ProductVersion
	Context "ProductVersion of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_ProductVersion'." { $version_info.ProductVersion | Should Be $Script:Win32DLL_ProductVersion }
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "FileVersionInfo of Dummy File ('$target': File Not Found)" {
		It "Throws Error (File Not Found)." { { Get-FileVersionInfo -Path 'dummy' -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "FileVersionInfo of Directory '$target'" {
		It "Throws Error (Target is Directory)." { { Get-FileVersionInfo -Path 'C:\Windows' -ErrorAction 'Stop' } | Should Throw }
	}
}


Describe "Get-FileVersion" {
	
	# FileVersion
	Context "FileVersion of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_FileVersion'." {
			Get-FileVersion -Path $Script:filepath_Target_Win32TestDLL | Should Be $Script:Win32DLL_FileVersion
		}
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "FileVersion of Dummy File ('$target': File Not Found)" {
		It "Throws Error. (File Not Found)." { { Get-FileVersion -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "FileVersion of Directory '$target'" {
		It "Throws Error. (Target is Directory)" { { Get-FileVersion -Path $target -ErrorAction 'Stop' } | Should Throw }
	}
}


Describe "Get-ProductVersion" {
	
	# ProductVersion
	Context "ProductVersion of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_ProductVersion'." {
			Get-ProductVersion -Path $Script:filepath_Target_Win32TestDLL | Should Be $Script:Win32DLL_ProductVersion
		}
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "ProductVersion of Dummy File ('$target': File Not Found)" {
		It "Throws Error. (File Not Found)." { { Get-ProductVersion -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "ProductVersion of Directory '$target'" {
		It "Throws Error. (Target is Directory)" { { Get-ProductVersion -Path $target -ErrorAction 'Stop' } | Should Throw }
	}
}


Describe "Get-FileDescription" {
	
	# FileDescription
	Context "FileDescription of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_FileDescription'." {
			Get-FileDescription -Path $Script:filepath_Target_Win32TestDLL | Should Be $Script:Win32DLL_FileDescription
		}
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "FileDescription of Dummy File ('$target': File Not Found)" {
		It "Throws Error. (File Not Found)." { { Get-FileDescription -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "FileDescription of Directory '$target'" {
		It "Throws Error. (Target is Directory)" { { Get-FileDescription -Path $target -ErrorAction 'Stop' } | Should Throw }
	}
}


Describe "Get-ProductName" {
	
	# ProductName
	Context "ProductName of '$Script:filename_Target_Win32TestDLL'" {
		It "Is '$Script:Win32DLL_ProductName'." {
			Get-ProductName -Path $Script:filepath_Target_Win32TestDLL | Should Be $Script:Win32DLL_ProductName
		}
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "ProductName of Dummy File ('$target': File Not Found)" {
		It "Throws Error. (File Not Found)." { { Get-ProductName -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "ProductName of Directory '$target'" {
		It "Throws Error. (Target is Directory)" { { Get-ProductName -Path $target -ErrorAction 'Stop' } | Should Throw }
	}
}
