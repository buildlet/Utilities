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
. ($PSCommandPath | Split-Path -Parent | Join-Path -ChildPath 'Initialize.ps1') > $null


# TestData Directory
$Script:dirpath_TestData = '..\..\..\TestData'

# Local Data Directory
$Script:dirpath_Local = '..\..\..\..\..\Local'

# HTML Help Test File(s)
$Script:help_files = @(
	'api.chm'
	'hhaxref.chm'
	'htmlhelp.chm'
	'htmlref.chm'
	'Viewhlp.chm'
)

# Text File
$Script:filename_HelloTXT = 'Hello.txt'
$Script:filepath_HelloTXT = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_HelloTXT


Describe "Open-HtmlHelp, Close-HtmlFile" {
	
	Context "Valid HTML Help File" {
            
        $Script:help_files | % {

			$help_file = $_
			$help_filepath = $Script:dirpath_Local | Join-Path -ChildPath 'HTML Help' | Join-Path -ChildPath $help_file

		    It "('$help_file') is open and closed." {

                # Open HTML Help file
                $hwnd = Open-HtmlHelp -Path $help_filepath -PassThru

                # Wait
                $wait_sec = 2
                "`tPlease Wait for $wait_sec Seconds... (hwnd=$hwnd)" | Write-Host -ForegroundColor Yellow
                Start-Sleep -Seconds $wait_sec

				# Assertion
				$hwnd | Should NOT BeNullOrEmpty

                # Cloe HTML Help file
                Close-HtmlHelp
            }
        }
	}

	# Error (File Not Found)
	$target = 'dummy'
	Context "Dummy File ('$target': File Not Found)" {
		It "Throws Error. (File Not Found)." { { Open-HtmlHelp -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Target is Directory)
	$target = $Script:dirpath_TestData
	Context "Directory '$target'" {
		It "Throws Error. (Target is Directory)" { { Open-HtmlHelp -Path $target -ErrorAction 'Stop' } | Should Throw }
	}

	# Error (Invalid HTML Help File)
	Context "'$Script:filename_HelloTXT', which is NOT Valid HTML Help File" {
		It "Returns 0. (Invalid HTML Help File: hwnd=0)." { Open-HtmlHelp -Path $Script:filepath_HelloTXT -PassThru | Should Be 0 }
	}
}
