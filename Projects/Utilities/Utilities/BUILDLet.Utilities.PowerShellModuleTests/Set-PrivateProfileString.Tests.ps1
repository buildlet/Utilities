<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2016-2017 Daiki Sakamoto

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

# Test INI File(s)
$Script:filename_TestINI = 'Test1.ini'
$Script:filepath_TestINI = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_TestINI


# Parameter for Parameter Test(s)
$Script:test_parameters = @(

    # Section1
    @{ Path = $null; Section = "Section1"; Key = "Key1"; Value = "UPDATE" },

    # Section2 (Update)
    @{ Path = $null; Section = "Section2"; Key = "Key1"; Value = "UPDATE" },
    @{ Path = $null; Section = "Section2"; Key = "Key2"; Value = "Update" },
    @{ Path = $null; Section = "Section2"; Key = "Key3"; Value = "Update" },
    @{ Path = $null; Section = "Section2"; Key = "Key10"; Value = "Update" },

    # Section2 (Append)
    @{ Path = $null; Section = "Section2"; Key = "Key100"; Value = "APPEND" },


    # Section3 (Update)
    @{ Path = $null; Section = "Section3"; Key = "Key1"; Value = "UPDATE" },
    @{ Path = $null; Section = "Section3"; Key = "Key7"; Value = "UPDATE" },
    @{ Path = $null; Section = "Section3"; Key = "Key8"; Value = "UPDATE" },
    @{ Path = $null; Section = "Section3"; Key = "Key3"; Value = "Update1, Update2, Update3" },

    # Section3 (Append)
    @{ Path = $null; Section = "Section3"; Key = "Key100"; Value = "APPEND" },


    # BlankSection (Append)
    @{ Path = $null; Section = "BlankSection"; Key = "Key1"; Value = "NEW" },


    # New Section
    @{ Path = $null; Section = "Section100"; Key = "Key1"; Value = "New" },
    @{ Path = $null; Section = "Section100"; Key = "Key2"; Value = "NEW" },
    @{ Path = $null; Section = "Section100"; Key = "Key3"; Value = "" },
    @{ Path = $null; Section = "Section100"; Key = "Key4"; Value = $null },


    # Section1 (Again)
    @{ Path = $null; Section = "Section1"; Key = "Key1"; Value = "UPDATE1" },
    @{ Path = $null; Section = "Section1"; Key = "Key1"; Value = "UPDATE2" },
    @{ Path = $null; Section = "Section1"; Key = "Key1"; Value = "UPDATE3" },

    # Section2 (Again, Additional: String.EMPTY and NULL Tests)
    @{ Path = $null; Section = "Section2"; Key = "Key5"; Value = "" },
    @{ Path = $null; Section = "Section2"; Key = "Key6"; Value = "" },
    @{ Path = $null; Section = "Section2"; Key = "Key7"; Value = $null },
    @{ Path = $null; Section = "Section2"; Key = "Key8"; Value = "" },
    @{ Path = $null; Section = "Section2"; Key = "Key9"; Value = $null }
)


Describe "Set-PrivateProfileString" {

	# Copy Test INI File(s)
	$test_filename = "copy.ini"
	$test_filepath = (Copy-Item -Path $Script:filepath_TestINI -Destination $test_filename -Force -PassThru).FullName


	# Default Test
	$path = $test_filepath
	$section = "Section1"
	$key = "Key1"
	$value = "Updated"
	$expected = $value
	Context "Value of Profile { Section='$section', Key='$key' } in Test INI File '$test_filename'" {

		It "Is Updated into '$expected'" {

			# Set-PrivateProfileString
			Set-PrivateProfileString $path -Section $section -Key $key -Value $value

			# Get-PrivateProfileString (for Validation)
			Get-PrivateProfileString $path -Section $section -Key $key | Should Be $expected
		}
	}


	# Parameter Test(s)	
	for ($i = 0; $i -lt $Script:test_parameters.Count; $i++) {

		$path = $test_filepath
		$section = $Script:test_parameters[$i].Section
		$key = $Script:test_parameters[$i].Key
		$value = $Script:test_parameters[$i].Value
		$expected = $value

		Context "Value of Profile { Section='$section', Key='$key' } in Test INI File '$test_filename'" {

			# Set-PrivateProfileString
			Set-PrivateProfileString $path -Section $section -Key $key -Value $value

			# Get-PrivateProfileString (for Validation)
			$actual = Get-PrivateProfileString $path -Section $section -Key $key

			if (-not [string]::IsNullOrEmpty($expected)) {

				# Default (Some Value)
				It "Is Updated into '$expected'." { $actual | Should Be $expected }
			}
			else {

				# Empty OR Null
				It "Is `"`" ([string]::Empty), Including '=' OR NOT." { $actual | Should Be "" }
			}
		}
	}
}
