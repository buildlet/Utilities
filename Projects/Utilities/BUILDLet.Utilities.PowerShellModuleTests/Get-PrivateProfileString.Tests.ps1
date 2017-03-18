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
. ($PSCommandPath | Split-Path -Parent | Join-Path -ChildPath 'Initialize.ps1') > $null


# TestData Directory
$Script:dirpath_TestData = '..\..\..\TestData'

# Test INI File(s)
$Script:filename_TestINI = 'Test1.ini'
$Script:filepath_TestINI = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_TestINI


# Expected Profiles
$Script:expected_profiles = @(

    # Section1
    @{ Path = $Script:filepath_TestINI; Section = "Section1"; Key = "Key1"; Expected = "Value1" },

    # Section2
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key1"; Expected = "Value1" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key2"; Expected = "Value2" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key3"; Expected = "Value3" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key4"; Expected = "Value4" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key5"; Expected = "Value5" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key6"; Expected = "Value6" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key7"; Expected = "Value7" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key8"; Expected = "Value8" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key9"; Expected = "Value9" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key10"; Expected = "Value10" },
    @{ Path = $Script:filepath_TestINI; Section = "Section2"; Key = "Key11"; Expected = "Value11" },

    # Section3
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key1"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key2"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key3"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key4"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key5"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key6"; Expected = $null },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key7"; Expected = "Value0,0.5,1.0,1.5,1.2,2" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key8"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key9"; Expected = "1, 2, 3" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key10"; Expected = "" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key11"; Expected = $null },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key12"; Expected = "Value12" },
    @{ Path = $Script:filepath_TestINI; Section = "Section3"; Key = "Key13"; Expected = $null },

    # Section4
    @{ Path = $Script:filepath_TestINI; Section = "Section4"; Key = "Key1"; Expected = ",,X" },

    # Blank Section
    @{ Path = $Script:filepath_TestINI; Section = "BlankSection"; Key = "dummy"; Expected = $null },

    # Invalid Section
    @{ Path = $Script:filepath_TestINI; Section = "dummy"; Key = "dummy"; Expected = $null }
)


# Expected Sections
$Script:expected_sections = @(
	@{ Name = "Section1"; KeyValuePairs = @( "Key1=Value1" ) },
	@{ Name = "Section2"; KeyValuePairs = @(
		"Key1=Value1",
		"Key2=Value2",
		"Key3=Value3",
		"Key4=VALUE4",
		"Key5=value5",
		"Key6=Value6",
		"Key7=Value7",
		"Key8=Value8",
		"Key9=Value9",
		"Key10=Value10",
		"Key11=Value11" ) },
	@{ Name = "Section3"; KeyValuePairs = @(
		"Key1=",
		"Key2=",
		"Key3=",
		"Key4=",
		"Key5=",
		"Key7=Value0,0.5,1.0,1.5,1.2,2",
		"Key8=",
		"Key9=1, 2, 3",
		"Key10=",
		"Key12=Value12" ) },
	@{ Name = "Section4"; KeyValuePairs = @( "Key1=,,X" ) },
	@{ Name = "BlankSection"; KeyValuePairs = @() },
	@{ Name = "InvalidSection"; KeyValuePairs = $null }
)


# Key Value Pair Test(s)
Describe "Get-PrivateProfileString -Section -Key" {

	# Default Test (Key Value Pair)
	$filename = $Script:filename_TestINI
	$path = $Script:filepath_TestINI
	$section = "Section1"
	$key = "Key1"
	$expected = "Value1"
	Context "Key Value Pair { Section='$section', Key='$key' } in Test INI File '$filename'" {
		It "Is '$expected'." { Get-PrivateProfileString $path -Section $section -Key $key | Should Be $expected }
	}


	# Parameter Test(s)	
	for ($i = 0; $i -lt $Script:expected_profiles.Count; $i++) {

		$path = $Script:expected_profiles[$i].Path
		$filename = $path | Split-Path -Leaf

		$section = $Script:expected_profiles[$i].Section
		$key = $Script:expected_profiles[$i].Key
		$expected = $Script:expected_profiles[$i].Expected

		Context "($i) Key Value Pair { Section='$section', Key='$key' } in Test INI File '$filename'" {

			# Get-PrivateProfileString (Key Value Pair)
			$actual = Get-PrivateProfileString $path -Section $section -Key $key

			if (-not [string]::IsNullOrEmpty($expected)) {

				# Default (Some Value)
				It "Is '$expected'." { $actual | Should Be $expected }
			}
			elseif ($expected -eq [string]::Empty) {

				# Empty
				It "Is `"`" ([string]::Empty)." { $actual | Should Be ([string]::Empty) }
			}
			elseif ($expected -eq $null) {

				# NOT Found (Null)
				It "Is NOT Found" { $actual | Should Be $null }
			}
			else { throw }
		}
	}
}


# Section Test(s)
Describe "Get-PrivateProfileString -Section" {

	# Default Test (Section)
	$filename = $Script:filename_TestINI
	$path = $Script:filepath_TestINI
	$section = "Section1"
	$expected = @{ Section = $section; Key = "Key1"; Value = "Value1" }
	$expected_section = $expected.Section
	$expected_key = $expected.Key
	$expected_value = $expected.Value
	Context "Section '$section' in Test INI File '$filename'" {

		# Get-PrivateProfileString (Section)
		$actual = Get-PrivateProfileString $path -Section $section

		It "Contains Profile { Section='$expected_section', Key='$expected_key', Value='$expected_value' }." {

			$actual.Count | Should NOT BeGreaterThan 1
			$actual[0].Section | Should Be $expected_section
			$actual[0].Key | Should Be $expected_key
			$actual[0].Value | Should Be $expected_value
		}
	}

	
	# Paramater Test(s)
	for ($i = 0; $i -lt $Script:expected_sections.Count; $i++) {

		$filename = $Script:filename_TestINI
		$path = $Script:filepath_TestINI

		$section = $Script:expected_sections[$i].Name
		$expected_key_value_pairs = $Script:expected_sections[$i].KeyValuePairs
		$expected_number_of_profiles = $expected_key_value_pairs.Count

		Context "($i) Section '$section' in Test INI File '$filename'" {

			# Get-PrivateProfileString (Section)
			$actual = Get-PrivateProfileString $path -Section $section

			if (($expected_key_value_pairs -eq $null) -or ($expected_number_of_profiles -eq 0)) {

				# NOT Found OR Found But No Entries
				It "Is NOT Found, OR NOT Including Any Profiles." { $actual | Should Be $null }				
			}
			elseif ($expected_number_of_profiles -eq 1) {

				# Found (=1)
				It "Includes Only 1 Profile." { $actual.Count | Should NOT BeGreaterThan 0 }

				$key = ([string]$expected_key_value_pairs[0]).Split('=')[0]
				$value = ([string]$expected_key_value_pairs[0]).Split('=')[1]

				It "Contains Profile { Section='$section', Key='$key', Value='$value' }." {
					$actual[0].Section | Should Be $section
					$actual[0].Key | Should Be $key
					$actual[0].Value | Should Be $value
				}
			}
			else {

				# Found (>1)
				It "Includes $expected_number_of_profiles Profile(s)." { $actual.Count | Should Be $expected_number_of_profiles }

				for ($j = 0; $j -lt $expected_key_value_pairs.Count; $j++) {
					
					$key = ([string]$expected_key_value_pairs[$j]).Split('=')[0]
					$value = ([string]$expected_key_value_pairs[$j]).Split('=')[1]

					$found = $false
					$actual | % {
						
						# Target Profile is Found.
						if (($_.Section -eq $section) -and ($_.Key -eq $key) -and ($_.Value -eq $value)) { $found = $true }
					}

					It "Contains Profile[$j] { Section='$section', Key='$key', Value='$value' }." { $found | Should Be $true }
				}
			}
		}
	}
}


# Read Test(s)
Describe "Get-PrivateProfileString (Read)" {

	$filename = $Script:filename_TestINI
	$path = $Script:filepath_TestINI

	$expected_number_of_profiles = 0
	$Script:expected_profiles | ? { $_.Expected -ne $null } | % { $expected_number_of_profiles++ }

	Context "Test INI File '$filename'" {

		# Get-PrivateProfileString (Read)
		$actual = Get-PrivateProfileString $path

		# Count Check
		It "Includes $expected_number_of_profiles Profile(s)." { $actual.Count | Should Be $expected_number_of_profiles }

		$i = 0
		$Script:expected_profiles | ? { $_.Expected -ne $null } | % {
					
			$section = $_.Section
			$key = $_.Key
			$value = $_.Expected

			$found = $false
			$actual | % {
						
				# Target Profile is Found.
				if (($_.Section -eq $section) -and ($_.Key -eq $key) -and ($_.Value -eq $value)) { $found = $true }
			}

			It "Contains Profile[$i] { Section='$section', Key='$key', Value='$value' }." { $found | Should Be $true }
			$i++
		}
	}
}
