<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2017 Daiki Sakamoto

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

# Win32 Test DLL
$Script:filename_GetContentBlockTest_TXT = 'GetContentBlockTest.txt'
$Script:filepath_GetContentBlockTest_TXT = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_GetContentBlockTest_TXT


Describe "Get-ContentBlock" {

	# Test File Path
	$filepath = $Script:filepath_GetContentBlockTest_TXT
	$filename = $Script:filename_GetContentBlockTest_TXT


	# StartPattern / EndPattern
	Context "Start Pattern and End Pattern" {

		It "Is Specified, then Appropriate Content is Exported." {

			# Expected
			$expected = @'
<!-- __START__ -->
Line 5
Line 6
Line 7
<!-- __END__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__START__.*-->' -EndPattern '<!--.*__END__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# EndPattern
	Context "Only End Pattern" {

		It "Is Specified, then Appropriate Content is Exported." {

			# Expected
			$expected = @'
Line 1
Line 2
Line 3
<!-- __START__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -EndPattern '<!--.*__START__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# StartPattern
	Context "Only Start Pattern" {

		It "Is Specified, then Appropriate Content is Exported." {

			# Expected
			$expected = @'
<!-- __LAST__ -->
Line 40
Line 41
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__LAST__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# 'ExcludeStartLine' & 'ExcludeEndLine' Parameter
	Context "'ExcludeStartLine' and 'ExcludeEndLine' Parameter" {

		It "Is Specified (with 'StartPattern' and 'EndPattern' Parameter), then Appropriate Content is Exported." {

			# Expected
			$expected = @'
Line 5
Line 6
Line 7
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__START__.*-->' -EndPattern '<!--.*__END__.*-->' -ExcludeStartLine -ExcludeEndLine

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# Stagged Section Test 1
	Context "Stagged Section (1)" {

		It "Is Exported Correctlly." {

			# Expected
			$expected = @'
<div class="test1"> <!-- __TEST1__ -->
Line 12
Line 13
<div class="test2"> <!-- __TEST2__ -->
Line 15
Line 16
</div> <!-- /TEST1__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__TEST1__.*-->' -EndPattern '<!--.*/TEST1__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# Stagged Section Test 2
	Context "Stagged Section (2)" {

		It "Is Exported Correctlly." {

			# Expected
			$expected = @'
<div class="test2"> <!-- __TEST2__ -->
Line 15
Line 16
</div> <!-- /TEST1__ -->
Line 18
Line 19
</div> <!-- /TEST2__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__TEST2__.*-->' -EndPattern '<!--.*/TEST2__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# Nested Section Test 1
	Context "Nested Section (1: Outer)" {

		It "Is Exported Correctlly." {

			# Expected
			$expected = @'
<div class="test3"> <!-- __TEST3__ -->
Line 27
Line 28
<div class="test4"> <!-- __TEST4__ -->
Line 30
Line 31
</div> <!-- /TEST4__ -->
Line 33
Line 34
</div> <!-- /TEST3__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__TEST3__.*-->' -EndPattern '<!--.*/TEST3__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}


	# Nested Section Test 2
	Context "Nested Section (2: Inner)" {

		It "Is Exported Correctlly." {

			# Expected
			$expected = @'
<div class="test4"> <!-- __TEST4__ -->
Line 30
Line 31
</div> <!-- /TEST4__ -->
'@.Split("`r`n", [System.StringSplitOptions]::RemoveEmptyEntries)

			# Actual
			$actual = Get-ContentBlock -Path $filepath -StartPattern '<!--.*__TEST4__.*-->' -EndPattern '<!--.*/TEST4__.*-->'

			# Check Number of Lines
			$actual.Count | Should Be $expected.Count

			# Check Content of Each Line
			0..($expected.Count - 1) | % { $actual[$_] | Should Be $expected[$_] }
		}
	}
}
