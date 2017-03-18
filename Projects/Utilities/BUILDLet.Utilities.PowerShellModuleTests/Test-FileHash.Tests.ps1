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

# Text File
$Script:filename_HelloTXT = 'Hello.txt'
$Script:filepath_HelloTXT = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_HelloTXT

# Source Zip File(s)
$Script:filename_RootDirectoryZip = 'RootDirectory.zip'
$Script:filepath_RootDirectoryZip = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_RootDirectoryZip


# File Test Result
$Script:result


Function assert-FileHash (
	[Parameter(Mandatory = $true)][PSObject]$Expected,
	[Parameter(Mandatory = $true)][PSObject]$Actual,
	[Parameter(Mandatory = $true)][string]$InputMethod) {

	$filepath = $Expected.Path
	$filehash = $Expected.Hash
	$algorithm = $Expected.Algorithm

	It ("(via $InputMethod) Contains '$filepath' (Hash='$filehash' ($algorithm)).") {

		$Script:result = `
			($Actual.Algorithm -eq $Expected.Algorithm) -and `
			($Actual.Hash -eq $Expected.Hash) -and `
			($Actual.Path -eq $Expected.Path)
				
		$Script:result | Should Be $true
	}
}


Function assert-Target (
	[Parameter(Mandatory = $true)]$Expected,
	[Parameter(Mandatory = $true)]$Actual,
	[Parameter(Mandatory = $true)][string]$InputMethod) {


	# Number of Files
	if ($Expected.Count -gt 0) {

		# Target is DIRECTORY

		# Set Number of Files
		$expected_num = $Expected.Count

		# Check Number of Files
		It "(via $InputMethod) Includes $expected_num File(s)." {
			$Actual.Count | Should Not Be 0
			$Actual.Count | Should Be $Expected.Count
		}

		# Check Each File(s)
		$found = 0
		$Actual | % {
			$actual_file_hash = $_

			$Expected | % {
				$expected_file_hash = $_
						
				if ($actual_file_hash.Path -eq $expected_file_hash.Path) {

					# Check File Content
					assert-FileHash -Expected $expected_file_hash -Actual $actual_file_hash -InputMethod $InputMethod

					if ($Script:result -eq $true) { $found++ }
				}
			}
		}

		# Number of Same File(s)
		It "(via $InputMethod) Includes $expected_num Same Files." {
			$found | Should Be $expected_num
		}
	}
	else {

		# Target is FILE

		# Check File Content
		assert-FileHash -Expected $Expected -Actual $Actual -InputMethod $InputMethod
	}
}


Describe "Test-FileHash" {

	$targets = @(
		$Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_HelloTXT
		$Script:dirpath_TestData
	)

	$input_methods = @(
		"Parameter"
		"Pipeline"
	)


	# Getting File Hash (Only) Test
	$targets | % {

		$target = $_
		$target_name = $target | Split-Path -Leaf


		# Expected
		if ($target | Test-Path -PathType Leaf) {

			# FILE

			# Set Context
			$context = "File Hash of '$target_name'"

			# Expected (by Get-FileHash Cmdlet)
			$expected = Get-FileHash -Path $target -Algorithm MD5
		}
		else {

			# DIRECTORY

			# Set Context
			$context = "File Hash of the File(s) in '$target_name' Directory"

			# Expected (by Get-FileHash Cmdlet)
			$expected = @()
			$target | Get-ChildItem -Recurse | % {
				$expected += Get-FileHash -Path $_.FullName -Algorithm MD5
			}
		}


		Context $context {

			$input_methods | % {

				$input_method = $_

				# Actual
				if ($input_method -eq "Parameter") {

					# by Parameter
					$actual = Test-FileHash $target
				}
				else {

					# by Pipeline
					$actual = $target | Test-FileHash
				}


				# Assertion
				assert-Target -Expected $expected -Actual $actual -InputMethod $input_method
			}
		}
	}


	# Hash Algorithm Test
	Context "Hash Algorithm" {
		
		# Algorithm(s)
		@(
			'MD5'
			'SHA1'
			'SHA256'
		) | % {

			# Alrorithm
			$algorithm = $_

			# Expected (by Get-FileHash Cmdlet)
			$expected = (Get-FileHash -Path $Script:filepath_HelloTXT -Algorithm $algorithm).Hash

			It "'$algorithm' is supported. (File Hash of '$Script:filename_HelloTXT' is '$expected'.)" {
				(Test-FileHash $Script:filepath_HelloTXT -Algorithm $algorithm).Hash | Should Be $expected
			}
		}
	}


	$dir_name_base = 'Test-FileHash Test'

	$targets = @(
		# FILE
		@('hello.txt', 'hello.txt'),
		@('hello.txt', 'HELLO.TXT'),
		@('hello.txt', 'hoge.txt'),
		@('hello.txt', 'hello.txt'),

		# DIRECTORY
		@('RootDirectory.zip', 'RootDirectory.zip'),
		@('RootDirectory.zip', 'RootDirectory.zip')
	)


	# Comparison Test
	for ($i = 0; $i -lt 6; $i++) {

		# Set Target Name & Target Directory Name
		$reference_target_name = $targets[$i][0]
		$difference_target_name = $targets[$i][1]
		$reference_dir = Get-Location | Join-Path -ChildPath ("$dir_name_base " + $i.ToString("D2") + " Reference - $reference_target_name")
		$difference_dir = Get-Location | Join-Path -ChildPath ("$dir_name_base " + $i.ToString("D2") + " Difference - $difference_target_name")


		# Create Target Directory (Reference)
		if ($reference_dir | Test-Path -PathType Container) { $reference_dir | Get-ChildItem -Recurse | Remove-Item -Recurse -Force }
		else { New-Item -Path $reference_dir -ItemType Directory }

		# Create Target Directory (Difference)
		if ($difference_dir | Test-Path -PathType Container) { $difference_dir | Get-ChildItem -Recurse | Remove-Item -Recurse -Force }
		else { New-Item -Path $difference_dir -ItemType Directory }


		# Target is File OR Directory
		if ($i -lt 4) {

			# In Case of FILE:

			# Create Target Files (Reference & Difference)
			$content = 'Hello, world.'
			$reference_target_path = $reference_dir | Join-Path -ChildPath $reference_target_name
			$difference_target_path = $difference_dir | Join-Path -ChildPath $difference_target_name
			$content | Out-File -FilePath $reference_target_path -Force
			$content | Out-File -FilePath $difference_target_path -Force


			switch ($i) {

				# 0: SAME FILE
				0 {
					Context "SAME FILE: Reference FILE '$reference_target_name' and Difference FILE '$difference_target_name'" {

						It "Is the Same. (Default: NOT Case Sensitive)" {
							Test-FileHash $reference_target_path $difference_target_path | Should Be $true
						}

						It "Is the Same even though CASE SENSITIVE." {
							Test-FileHash $reference_target_path $difference_target_path -CaseSensitive | Should Be $true
						}
					}
				}

				# 1: SAME FILE But Dirrerent Name in Case Sensitive
				1 {
					Context "SAME FILE: Reference FILE '$reference_target_name' and Difference FILE '$difference_target_name'" {

						It "Is the Same. (Default: NOT Case Sensitive)" {
							Test-FileHash $reference_target_path $difference_target_path | Should Be $true
						}

						It "Is Different in CASE SENSITIVE." {
							Test-FileHash $reference_target_path $difference_target_path -CaseSensitive | Should Be $false
						}
					}
				}

				# 2: SAME FILE But Dirrerent Name
				2 {
					Context "SAME FILE: Reference FILE '$reference_target_name' and Difference FILE '$difference_target_name'" {

						It "Is Different." {
							Test-FileHash $reference_target_path $difference_target_path | Should Be $false
						}
					}
				}

				# 3: DIFFERNET FILE But Same Name
				3 {
					Context "DIFFERENT FILE: Reference FILE '$reference_target_name' and Difference FILE '$difference_target_name'" {

						It "Is Different." {

							# Append File Content
							'Hello, again.' | Out-File -FilePath $difference_target_path -Append -Force

							Test-FileHash $reference_target_path $difference_target_path | Should Be $false
						}
					}
				}
			}
		}
		else {

			# In Case of DIRECTORY:

			# Expand Targets (Reference & Difference)
			Expand-Archive -Path ($Script:dirpath_TestData | Join-Path -ChildPath $reference_target_name) -DestinationPath $reference_dir -Force
			Expand-Archive -Path ($Script:dirpath_TestData | Join-Path -ChildPath $difference_target_name) -DestinationPath $difference_dir -Force

			# Target Directory Name (Reference & Difference)
			$reference_dir_name = $reference_dir | Split-Path -Leaf
			$difference_dir_name = $difference_dir | Split-Path -Leaf


			switch ($i) {

				# 4: SAME DIRECTORY
				4 {
					Context "SAME DIRECTORY: Reference DIRECTORY '$reference_dir_name' and Difference DIRECTORY '$difference_dir_name'" {

						It "Is the Same." {
							Test-FileHash $reference_dir $difference_dir | Should Be $true
						}
					}
				}

				# 5: DIFFERENT DIRECTORY
				5 {
					# Additional File
					$filename_HelloTXT2 = 'hello2.txt'
					$filepath_HelloTXT2 = $difference_dir | Join-Path -ChildPath $filename_HelloTXT2


					Context "DIFFERENT DIRECTORY: Reference DIRECTORY '$reference_dir_name' and Difference DIRECTORY '$difference_dir_name'" {

						It "Is Different." {

							# Append A File
							'Hello, again.' | Out-File -FilePath ($difference_dir | Join-Path -ChildPath $filename_HelloTXT2) -Force

							Test-FileHash $reference_dir $difference_dir | Should Be $false
						}
					}

					Context "File Hash of Common File(s) in Reference DIRECTORY '$reference_dir_name' and Difference DIRECTORY '$difference_dir_name'" {

						# Expected
						$expected = @()
						$reference_dir | Get-ChildItem -Recurse | % { $expected += Get-FileHash -Path $_.FullName -Algorithm MD5 }

						# Actual
						$actual = Test-FileHash $reference_dir $difference_dir -PassThru

						# Assertion
						assert-Target -Expected $expected -Actual $actual -InputMethod 'Parameter'
					}
				}
			}
		}
	}
}
