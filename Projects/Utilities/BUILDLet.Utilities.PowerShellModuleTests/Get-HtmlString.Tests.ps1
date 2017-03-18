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

# Test HTML File(s)
$Script:filename_Test1 = 'Test1.html'
$Script:filepath_Test1 = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Test1
$Script:filename_Test2 = 'Test2_STRICT.html'
$Script:filepath_Test2 = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Test2
$Script:filename_Attribute = 'Attributes.html'
$Script:filepath_Attribute = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Attribute
$Script:filename_W3C_example = 'W3C_example.html'
$Script:filepath_W3C_example = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_W3C_example
$Script:filename_HTML4_01_Spec = 'HTML 4_01 Specification.htm'
$Script:filepath_HTML4_01_Spec = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_HTML4_01_Spec
$Script:filename_Tohoho = 'www.tohoho-web.com.how2.html'
$Script:filepath_Tohoho = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_Tohoho


Describe "Get-HtmlString" {

	# for Default Test(s)
	$filename = $Script:filename_Test1
	$path = $Script:filepath_Test1

	# Default Test (only Element Name)
	$name = "H1"
	$expected = "This is Headline Level 1"
	Context "HTML Element <$name> in HTML File '$filename'" {
		It "Is '$expected'." { Get-HtmlString $path -Name $name | Should Be $expected }
	}


	# Default Test (Element Name & 1 Attribute)
	$element_name = "P"
	$attribute1_name = "class"
	$attribute1_value = "ver"
	$attributes = @{ $attribute1_name = $attribute1_value }
	$expected = "Version 1.00"
	Context "HTML Element <$element_name> (Attribute '$attribute1_name'='$attribute1_value') in HTML Test File '$filename'" {

		It "Is '$expected'. (w/o 'STRICT' Mode)" {
			Get-HtmlString $path -Name $element_name -Attributes $attributes | Should Be $expected
		}

		It "Can NOT be retrieved with 'STRICT' Mode." {
			{ Get-HtmlString $path -Name $element_name -Attributes $attributes -Strict -ErrorAction Stop } | Should Throw
		}
	}


	# 'Strict' Parameter Test(s)
	$filename1 = $Script:filename_Test1
	$filepath1 = $Script:filepath_Test1
	$filename2 = $Script:filename_Test2
	$filepath2 = $Script:filepath_Test2
	$element_name = "P"
	$attribute1_name = "class"
	$attribute1_value = "ver"
	$attributes = @{ $attribute1_name = $attribute1_value }
	$expected = "Version 1.00"
	Context "HTML Element <$element_name> (Attribute '$attribute1_name'='$attribute1_value')" {

		It "(='$expected') Can be retrieved from HTML Test File '$filename1' w/o 'STRICT' Mode." {
			Get-HtmlString $filepath1 -Name $element_name -Attributes $attributes | Should Be $expected
		}

		It "(='$expected') Can NOT be retrieved from HTML Test File '$filename1' with 'STRICT' Mode." {
			{ Get-HtmlString $filepath1 -Name $element_name -Attributes $attributes -Strict -ErrorAction Stop } | Should Throw
		}

		It "(='$expected') Can be retrieved from Both HTML Test File '$filename1' and '$filename2' w/o 'STRICT' Mode." {
			Get-HtmlString $filepath1 -Name $element_name -Attributes $attributes | Should Be $expected
			Get-HtmlString $filepath2 -Name $element_name -Attributes $attributes -Strict | Should Be $expected
		}
	}


	# Multi-Value Retrieve Test(s)
	$filename = $Script:filename_Attribute
	$path = $Script:filepath_Attribute
	$name = "P"
	$expected = @( "P1", "P2", "P3" )

	0..1 | % {
		$context = "HTML Element <$name>"

		switch ($_) {

			0 {
				$attributes = @{ "attrib1" = "test1"; "attrib2" = "test2"; "attrib3" = "test3" }

				$context += " (Attribute"
				$attributes.Keys | % { $context += (" '$_'='" + $attributes[$_] + "'") }
				$context += " )"
			}

			1 {
				$attribute_name = "attrib2"
				$attribute_value = "test2"
				$attributes = @{ $attribute_name = $attribute_value }

				$context += " (Specifying ONLY 1 Attribute '$attribute_name'='$attribute_value')"
			}
		}

		$context += " in HTML Test File '$filename'"

		Context $context {

			# Get-HtmlString
			$actual = Get-HtmlString $path -Name $name -Attributes $attributes

			$expected | % {
				$expected_value = $_
				It "'$expected_value' Can Be Found." {

					$found = $false
					$actual | % {
						$actual_value = $_
						if ($actual_value -eq $expected_value) { $found = $true }
					}

					# Assertion
					$found | Should Be $true
				}
			}
		}
	}
}
