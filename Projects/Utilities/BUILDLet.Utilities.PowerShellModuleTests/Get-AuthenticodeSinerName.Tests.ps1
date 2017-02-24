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

# Local Data Directory
$Script:dirpath_Local = '..\..\..\..\..\Local'

# Win32 Test DLL
$Script:filename_Win32DLL = 'Win32Test.dll'
$Script:filepath_Win32DLL = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Win32DLL

# FCIV (for check Digital Signature)
$Script:filename_FCIV = 'fciv.exe'
$Script:filepath_FCIV = $Script:dirpath_Local | Join-Path -ChildPath 'FCIV' | Join-Path -ChildPath $Script:filename_FCIV


Describe "Get-AuthenticodeSignerName" {
	
	Context "Signer Name of '$Script:filename_FCIV'" {
		$expected = "Microsoft Corporation"
		It "Is '$expected'." { Get-AuthenticodeSignerName -FilePath $Script:filepath_FCIV | Should Be $expected }
	}

	Context "Signer Name of '$Script:filename_Win32DLL'" {
		It "Is NULL." { Get-AuthenticodeSignerName -FilePath $Script:filepath_Win32DLL | Should BeNullOrEmpty }
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "Signer Name of Dummy File ('$target': File Not Found)" {
		It "Throws Error (File Not Found)." { { Get-AuthenticodeSignerName -FilePath $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "Signer Name of Directory '$target'" {
		It "Throws Error (Target is Directory)." { { Get-AuthenticodeSignerName -FilePath $target -ErrorAction 'Stop' } | Should Throw }
	}
}
