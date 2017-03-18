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


# Directory Number for 'New-Directory' Function Test
$Script:dir_number = 0


Function get-TargetDirPath (
	[Parameter(Mandatory = $false)][int]$Count = -1,
	[Parameter(Mandatory = $true)][string]$FolderName,
	[Parameter(Mandatory = $true)][switch]$Clean = $false) {

	# Get Folder Path
	if ($Count -ge 0) {
		$test_path = Get-Location | Join-Path -ChildPath ("New-Directory Test " + $Count.ToString("D2") + " - $FolderName")
	}
	else {
		$test_path = Get-Location | Join-Path -ChildPath $FolderName
	}

	# Cleaning
	if ($Clean -and ($test_path | Test-Path)) { $test_path | Remove-Item -Force -Recurse }

	# RETURN
	return $test_path
}


Describe "New-Directory" {

	# Folder NAME
	$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'Name' -Clean
	$dir_name = $dir_path | Split-Path -Leaf
	Context "Directory Name: '$dir_name'" {

		It "Is specified, Target Directory is created." {
			New-Directory -Path $dir_name -PassThru | Test-Path -PathType Container | Should Be $true
		}

		It "Is specified, The Created Directory '$dir_name' contains NOTHING." {
			$dir_path | Get-ChildItem | Should BeNullOrEmpty
		}
	}


	# FULL PATH
	$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'Path (FullName)' -Clean
	$dir_name = $dir_path | Split-Path -Leaf
	Context "Directory Path (FullName): '$dir_path'" {

		It "Is specified, Target Directory is created." {
			New-Directory -Path $dir_path -PassThru | Test-Path -PathType Container | Should Be $true
		}

		It "Is specified, The Created Directory '$dir_name' contains NOTHING." {
			$dir_path | Get-ChildItem | Should BeNullOrEmpty
		}
	}


	Context "Error" {

		# Error: Directory Already Exists
		It "(Directory Already Exists) is thrown, Because The Directory '$dir_name' Already exists." {
			{ New-Directory -Path $dir_path -ErrorAction Stop } | Should Throw
		}


		# Error: File Already Exists
		"Hello, world." | Out-File -FilePath ($filename = "New-Directory_ErrorTest.txt")
		It "(File Already Exists) is thrown, Because The File '$filename' Already exists." {
			{ New-Directory -Path $filename -ErrorAction Stop } | Should Throw
		}
	}


	Context "'PassThru' Parameter" {

		# PassThru = ON
		$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'PassThru=ON' -Clean
		$dir_name = $dir_path | Split-Path -Leaf
		It "Is specified, FULL PATH of the Created Directory '$dir_name' is output." { (New-Directory -Path $dir_name -PassThru).FullName | Should Be $dir_path }
		It "Is specified, Directory '$dir_name' is created." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, The Created Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		# PassThru = OFF
		$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'PassThru=OFF' -Clean
		$dir_name = $dir_path | Split-Path -Leaf
		It "Is NOT specified, NOTHING is output." { New-Directory -Path $dir_name | Should BeNullOrEmpty }
		It "Is NOT specified, Anyway, Directory '$dir_name' is created." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is NOT specified, Anyway, The Created Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }
	}


	Context "'Force' Parameter" {
		
		$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'FORCE' -Clean
		$dir_name = $dir_path | Split-Path -Leaf


		$keyword = 'NEWLY'
		It "Is specified, FULL PATH of The Created Directory '$dir_name' is output." {
			(New-Directory -Path $dir_name -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified, Directory '$dir_name' is $keyword created." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, the Created Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		$keyword1 = 'EXISTING'
		$keyword2 = 'EMPTY'
		It "Is specified, FULL PATH of $keyword1 and $keyword2 Directory '$dir_name' is output." {
			(New-Directory -Path $dir_name -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified, Directory '$dir_name' Still Exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		# Create Test File in the Target Directory
		$hello_filename = "Hello.txt"
		$hello_filepath = $dir_path | Join-Path -ChildPath $hello_filename
		"Hello, world." | Out-File -FilePath $hello_filepath


		$keyword1 = 'EXISTING'
		$keyword2 = 'Containing 1 File'
		It "Is specified, FULL PATH of $keyword1 Directory '$dir_name' $keyword2 is output." {
			(New-Directory -Path $dir_name -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified, Directory '$dir_name' Still exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, File '$hello_filename' in Directory '$dir_name' Still exists." { $hello_filepath | Test-Path -PathType Leaf | Should Be $true }


		# Create Test Files in Sub Directory of the Target Directory
		$target_dir_path = @()
		$target_filepath = @()
		$dir_num = 4
		0..($dir_num - 1) | % {
			$target_dir_path += (New-Item -Path ($dir_path | Join-Path -ChildPath "Sub Directory $_") -ItemType Directory).FullName
			$target_filepath += ($hello_filepath | Copy-Item -Destination ($target_dir_path[-1] | Join-Path -ChildPath "Hello$_.txt") -PassThru).FullName
		}
		$target_dir_path += (New-Item -Path ($target_dir_path[-1] | Join-Path -ChildPath "Sub Sub Directory 101") -ItemType Directory).FullName
		$target_filepath += ($hello_filepath | Copy-Item -Destination ($target_dir_path[-1] | Join-Path -ChildPath "Hello101.txt") -PassThru).FullName


		$keyword1 = 'EXISTING'
		$keyword2 = 'Containing Directories and Files'
		It "Is specified, FULL PATH of $keyword1 Directory '$dir_name' $keyword2 is output." {
			(New-Directory -Path $dir_name -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified, Directory '$dir_name' Still exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, File '$hello_filename' (Created in the Previous Test Case) in Directory '$dir_name' Still exists." {
			$hello_filepath | Test-Path -PathType Leaf | Should Be $true
		}
		0..($dir_num - 1) | % {
			$dir_name = $target_dir_path[$_] | Split-Path -Leaf
			$filename = $target_filepath[$_] | Split-Path -Leaf
			It "Is specified, File '$filename' in Directory '$dir_name' Still exists." {
				$target_filepath[$_] | Test-Path -PathType Leaf | Should Be $true
			}
		}
		$dir_name = $target_dir_path[-1] | Split-Path -Leaf
		$filename = $target_filepath[-1] | Split-Path -Leaf
		It "Is specified, File '$filename' in Directory '$dir_name' Still exists." {
			$target_filepath[-1] | Test-Path -PathType Leaf | Should Be $true
		}
	}


	Context "'Clean' Parameter" {

		$dir_path = get-TargetDirPath -Count ($Script:dir_number++) -FolderName 'CLEAN' -Clean
		$dir_name = $dir_path | Split-Path -Leaf


		$keyword = 'NEWLY'
		It "Is specified, FULL PATH of The Created Directory '$dir_name' is output." {
			(New-Directory -Path $dir_name -Clean -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified, Directory '$dir_name' is $keyword Created." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified, The Created Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		$keyword1 = 'EXISTING'
		$keyword2 = 'EMPTY'
		It "Is specified with 'Force' Parameter, FULL PATH of $keyword1 and $keyword2 Directory '$dir_name' is output." {
			(New-Directory -Path $dir_name -Clean -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified with 'Force' Parameter, Directory '$dir_name' Still exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified with 'Force' Parameter, Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		# Create Test File in the Target Directory
		$hello_filename = "Hello.txt"
		$hello_filepath = $dir_path | Join-Path -ChildPath $hello_filename
		"Hello, world." | Out-File -FilePath $hello_filepath


		$keyword1 = 'EXISTING'
		$keyword2 = 'Containing 1 File'
		It "Is specified with 'Force' Parameter, FULL PATH of $keyword1 Directory '$dir_name' $keyword2 is output." {
			(New-Directory -Path $dir_name -Clean -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified with 'Force' Parameter, Directory '$dir_name' Still exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified with 'Force' Parameter, Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }


		# Create Test Files in Sub Directory of the Target Directory
		$target_dir_path = @()
		$target_filepath = @()
		$dir_num = 4
		0..($dir_num - 1) | % {
			$target_dir_path += (New-Item -Path ($dir_path | Join-Path -ChildPath "Sub Directory $_") -ItemType Directory).FullName
			$target_filepath += $target_dir_path[-1] | Join-Path -ChildPath "Hello$_.txt"
			"Hello, world." | Out-File -FilePath $target_filepath[-1]
		}
		$target_dir_path += (New-Item -Path ($target_dir_path[-1] | Join-Path -ChildPath "Sub Sub Directory 101") -ItemType Directory).FullName
		$target_filepath += $target_dir_path[-1] | Join-Path -ChildPath "Hello101.txt"
		"Hello, world." | Out-File -FilePath $target_filepath[-1]


		$keyword1 = 'EXISTING'
		$keyword2 = 'Containing Directories and Files'
		It "Is specified with 'Force' Parameter, FULL PATH of $keyword1 Directory '$dir_name' $keyword2 is output." {
			(New-Directory -Path $dir_name -Clean -Force -PassThru).FullName | Should Be $dir_path
		}
		It "Is specified with 'Force' Parameter, Directory '$dir_name' Still exists." { $dir_path | Test-Path -PathType Container | Should Be $true }
		It "Is specified with 'Force' Parameter, Directory '$dir_name' contains NOTHING." { $dir_path | Get-ChildItem | Should BeNullOrEmpty }
	}
}
