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


Describe "New-BinaryFile" {

	$bin_file1 = 'New-BinaryFile_Test1.bin'
	$bin_file2 = 'New-BinaryFile_Test2.bin'
	$bin_file2_size = 1000 * 1000

	# Parameter
	Context "'$bin_file1' in Current Directory" {
		$expected = Get-Location | Join-Path -ChildPath $bin_file1

		# Remove bin file (test1.bin: if exists)
		if (Test-Path -Path $expected) { Remove-Item -Path $expected -Force }

		It "Is created. (by Parameter)" { New-BinaryFile $bin_file1 -PassThru | Should Be $expected }
		It "Exists." { $expected | Should Exist }
	}

	# Pipeline
	Context "'$bin_file2' in Current Directory" {
		$expected = Get-Location | Join-Path -ChildPath $bin_file2

		# Remove bin file (test2.bin: if exists)
		if (Test-Path -Path $expected) { Remove-Item -Path $expected -Force }

		It "Is created. (by Pipeline)" { $bin_file2 | New-BinaryFile -Size $bin_file2_size -PassThru | Should Be $expected }
		It "Exists." { $expected | Should Exist }
	}

	# Size (Default Size)
	Context "File Size of '$bin_file1'" {
		$expected = 1024
		$actual = (Get-Item -Path $bin_file1).Length 
		It "Is $expected bytes. (Default Size)" { $actual | Should Be $expected }
	}

	# Size
	Context "File Size of '$bin_file2'" {
		$expected = $bin_file2_size
		$actual = (Get-Item -Path $bin_file2).Length 
		It "Is $expected bytes." { $actual | Should Be $expected }
	}

	Context "By Multiple File Creation," {

		$count = 5
		$filesize = 1024 * 2

		# Create multiple filename(s)
		0..$count | % { [string[]]$filenames += "New-BinaryFile_TestFile" + [string]::Format("{0:D2}", $_) + ".bin" }

		# Remove bin file(s)
		$filenames | % { if (Test-Path -Path $_) { Remove-Item -Path $_ -Force } }

		# Create file(s)
		$filenames | % { [System.IO.FileInfo[]]$paths += New-BinaryFile -Path $_ -Size $filesize -PassThru }

		# Assertion(s)
		# 0: Exsistence
		# 1: Size
		# 2: Difference
		0..2 | % {

			$test_num = $_
			0..$count | % {

				$file_num = $_
				$filename = $filenames[$file_num]
				$path = $paths[$file_num]

				switch ($test_num) {

					# 0: Exsistence
					0 {
						It "'$filename' is created." { $path | Should Exist }
					}

					# 1: Size
					1 {
						It "Size of '$filename' is $filesize bytes." { (Get-Item -Path $path).Length | Should Be $filesize }
					}

					# 2: Difference
					2 {
						if ($file_num -gt 0) {
							$target_filename = $filenames[$file_num - 1]
							It "'$filename' is different from '$target_filename'." {
								(Get-FileHash -Path $path) | Should Not Be (Get-FileHash -Path $target_filename)
							}
						}
					}
				}
			}
		}
	}
}
