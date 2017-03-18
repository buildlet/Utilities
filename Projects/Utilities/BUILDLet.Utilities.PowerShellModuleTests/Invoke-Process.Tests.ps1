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

# Local Data Directory
$Script:dirpath_Local = '..\..\..\..\..\Local'

# Test Tools Directory
$Script:dirpath_TestTools = '..\..\..\..\TestTools\bin\Debug'

# Test File(s) (Executables)
$Script:filename_FCIV_EXE = 'fciv.exe'
$Script:filepath_FCIV_EXE = $Script:dirpath_Local | Join-Path -ChildPath 'FCIV' | Join-Path -ChildPath $Script:filename_FCIV_EXE

# Test Tools (Executables: Console Application)
$Script:filename_Exit999_EXE = 'Exit999.exe'
$Script:filename_Stdout300_EXE = 'Stdout300.exe'
$Script:filename_Stderr300_EXE = 'Stderr300.exe'
$Script:filename_Randum300_EXE = 'Randum300.exe'
$Script:filepath_Exit999_EXE = $Script:dirpath_TestTools | Join-Path -ChildPath $Script:filename_Exit999_EXE
$Script:filepath_Stdout300_EXE = $Script:dirpath_TestTools | Join-Path -ChildPath $Script:filename_Stdout300_EXE
$Script:filepath_Stderr300_EXE = $Script:dirpath_TestTools | Join-Path -ChildPath $Script:filename_Stderr300_EXE
$Script:filepath_Randum300_EXE = $Script:dirpath_TestTools | Join-Path -ChildPath $Script:filename_Randum300_EXE


Describe "Invoke-Process" {

	# Default Test (by 'FCIV.exe')
	$filename = $Script:filename_FCIV_EXE
	$filepath = $Script:filepath_FCIV_EXE
	Context "'$filename' (FCIV: File Checksum Integrity Verifier)" {

		$argument_valid_dir = "WindowsPowerShell\Modules\BUILDLet.Utilities.PowerShell\"
		$argument_blank_dir = "WindowsPowerShell\"
		$expected_code0 = 0
		$expected_code1 = 1
		$expected_output_headerOnly = @( "//", "// File Checksum Integrity Verifier version 2.05.", "//" )


		# Return Code (= 0)
		It "Is Invoked. (Return Code = $expected_code0)" { Invoke-Process $filepath $argument_valid_dir | Should Be $expected_code0 }

		# Return Code (!= 0)
		It "Is Invoked. (Return Code = $expected_code1)" { Invoke-Process $filepath | Should Be $expected_code1 }

		# 'PassThru' Parameter (Redirect RETURN CODE to Standard Output Stream)
		It "Is Invoked. (VERBOSE Output is Redirected to STANDARD OUTPUT STREAM by 'PassThru' Parameter)" {

			$actual = Invoke-Process $filepath $argument_blank_dir -PassThru

			# Check Line Number
			$actual.Count | Should Be $expected_output_headerOnly.Count

			# Check Each Value
			0..($expected_output_headerOnly.Count - 1) | % { $actual[$_] | Should Be $expected_output_headerOnly[$_] }
		}

		# 'RedirectStandardOutputToWarning' Parameter (Redirect Standard Output Stream to WARNING Output)
		It "Is Invoked. (VERBOSE Output is Redirected to WARNING Message Stream by 'RedirectStandardOutputToWarning' Parameter)" {

			$actual = Invoke-Process $filepath $argument_blank_dir -RedirectStandardOutputToWarning -WarningVariable warnings

			# Check Return Code
			$actual | Should Be $expected_code0

			# Check Line Number
			$warnings.Count | Should Be $expected_output_headerOnly.Count

			# Check Each Value
			0..($expected_output_headerOnly.Count - 1) | % { $warnings[$_] | Should Be $expected_output_headerOnly[$_] }
		}
	}


	# Default Test (Check RETURN CODE using 'Exit999.exe' (1))
	Context "'$Script:filename_Exit999_EXE' (1)" {

		$expected = 0x999
		$expected_HexString = $expected.ToString("X4")
		It "Returns Code $expected (0x$expected_HexString)." { Invoke-Process $Script:filepath_Exit999_EXE | Should Be $expected }
	}


	# STANDARD OUTPUT STREAM (& Load) Test (using 'Stdout300.exe')
	Context "'$Script:filename_Stdout300_EXE'" {

		$expected_code0 = 0


		# Return Code
		It "Returns Code $expected_code0." { Invoke-Process $Script:filepath_Stdout300_EXE | Should Be $expected_code0 }


		# Omit Default (STANDARD OUTPUT STREAM is redirected to VERBOSE Output)


		# 'PassThru' Parameter (STANDARD OUTPUT STREAM is redirected to STANDARD Output)
		It "Outputs continuous value to STANDARD OUTPUT STREAM, and it is redirected to PIPELINE (STANDARD Output) by 'PassThru' Parameter." {

			$expected = 1
			Invoke-Process $Script:filepath_Stdout300_EXE -PassThru | % { $_ | Should Be ($expected++) }
		}


		# 'RedirectStandardOutputToWarning' Parameter (STANDARD OUTPUT STREAM is redirected to VERBOSE Output)
		It "Outputs continuous value to STANDARD OUTPUT STREAM, and it is redirected to WARNING Output by 'RedirectStandardOutputToWarning' Parameter." {

			$actual = Invoke-Process $Script:filepath_Stdout300_EXE -RedirectStandardOutputToWarning -WarningVariable warnings -WarningAction SilentlyContinue

			# Check Return Code
			$actual | Should Be $expected_code0

			# Check Line Number			
			$warnings.Count | Should Be 300

			# Check Each Value
			$expected = 1
			$warnings | % { [int]::Parse($_) | Should Be ($expected++) }
		}
	}


	# STANDARD ERROR STREAM (& Load) Test (using 'Stderr300.exe')
	Context "'$Script:filename_Stderr300_EXE'" {

		$expected_code0 = 0


		# Return Code
		It "Returns Code $expected_code0." { Invoke-Process $Script:filepath_Stderr300_EXE -WarningAction SilentlyContinue | Should Be $expected_code0 }


		# Default (STANDARD ERROR STREAM is redirected to WARNING Output)
		It "Outputs continuous value to STANDARD ERROR STREAM, and it is redirected to WARNING Output. (Default)" {

			$actual = Invoke-Process $Script:filepath_Stderr300_EXE -WarningVariable warnings -WarningAction SilentlyContinue

			# Check Return Code
			$actual | Should Be $expected_code0

			# Check Line Number			
			$warnings.Count | Should Be 300

			# Check Each Value
			$expected = 1
			$warnings | % { [int]::Parse($_) | Should Be ($expected++) }
		}


		# 'RedirectStandardErrorToOutput' Parameter (STANDARD ERROR STREAM is redirected to STANDARD Output)
		It "Outputs continuous value to STANDARD ERROR STREAM, and it is redirected to PIPELINE (STANDARD Output) by 'RedirectStandardErrorToOutput' Parameter." {

			$expected = 1
			Invoke-Process $Script:filepath_Stderr300_EXE -PassThru -RedirectStandardErrorToOutput | % { $_ | Should Be ($expected++) }
		}


		# Omit 'RedirectStandardErrorToVerbose' Parameter Check (STANDARD ERROR STREAM is redirected to VERBOSE Output)
	}


	# Load Test (using 'Randum300.exe')
	Context "'$Script:filename_Randum300_EXE'" {

		$expected_code0 = 0


		# Return Code
		It "Returns Code $expected_code0." { Invoke-Process $Script:filepath_Randum300_EXE -WarningAction SilentlyContinue | Should Be $expected_code0 }


		# 'PassThru' Parameter Test (VERBOSE Output is redirected to PIPELINE)
		It "Outputs continuous value to PIPELINE (VERBOSE Output Redirected from STANDARD OUTPUT STREAM by 'PassThru' Parameter) and WARNING Output." {

			$stdout = Invoke-Process $Script:filepath_Randum300_EXE -PassThru -WarningVariable warnings -WarningAction SilentlyContinue

			# Check Return Code
			# (None)

			# Check Line Number			
			$stdout.Count + $warnings.Count | Should Be 300

			# Check Each Value
			$index_stdout = 0
			$index_warning = 0
			$expected = 0
			for ($i = 0; $i -lt 300; $i++) {
				$expected++
				if ($stdout[$index_stdout] -eq $expected) { $index_stdout++ }
				elseif ([int]::Parse($warnings[$index_warning]) -eq $expected) { $index_warning++ }
				else { throw ("Line[$i]: Pipeline = " + $stdout[$index_stdout] + " / " + "Warning Output = " + [int]::Parse($warnings[$index_warning])) }
			}
		}


		# 'PassThru' parameter Test (VERBOSE Output -> PIPELINE) and 'RedirectStandardErrorToOutput' parameter (WARNING Output -> PIPELINE)
		It "Outputs continuous value to PIPELINE (VERBOSE: 'PassThru' Parameter & WARNING: 'RedirectStandardErrorToOutput' Parameter)." {

			$stdout = Invoke-Process $Script:filepath_Randum300_EXE -PassThru -RedirectStandardErrorToOutput

			# Check Return Code
			# (None)

			# Check Line Number			
			$stdout.Count | Should Be 300

			# (Sequential order is NOT assured.)

			$expected = 0
			1..300 | % { $expected += $_ }

			$actual = 0
			0..299 | % { $actual += $stdout[$_] }

			$actual | Should Be $expected
		}
	}


	# 'RetryCount' Parameter Test (using 'Exit999.exe' (2))
	Context "'$Script:filename_Exit999_EXE' (2)" {

		$expected_code = 0x999
		$expected_code_HexString = $expected_code.ToString("X4")
		$retry_count = 5
		$retry_interval = 2

		It "Returns Code $expected_code (0x$expected_code_HexString) $retry_count times." {

			Invoke-Process $Script:filepath_Exit999_EXE -RetryCount $retry_count -RetryInterval $retry_interval | % { $_ | Should Be $expected_code }
		}
	}
}
