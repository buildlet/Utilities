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

# Source Zip File(s)
$Script:filename_HelloZip = 'hello.zip'
$Script:filepath_HelloZip = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_HelloZip
$Script:filename_RootDirectoryZip = 'RootDirectory.zip'
$Script:filepath_RootDirectoryZip = $Script:dirpath_TestData | Join-Path -ChildPath $Script:filename_RootDirectoryZip

# Original Zip File(s)
$Script:original_zip_files = @(
	$Script:filepath_HelloZip
	$Script:filepath_RootDirectoryZip
)


# Create (if does NOT exist) OR Clean (if exists) Directory
Function new-TestDirectory ([Parameter(Mandatory = $true)][string]$Path) {

	if (-not ($Path | Test-Path)) { New-Item -Path $Path -ItemType Directory > $null }
	else { $Path | Get-ChildItem -Recurse -Force | Remove-Item -Recurse -Force }
}


# Initialize Reference(s)
Function set-Reference ([Parameter(Mandatory = $true)][string[]]$SourceZipFiles) {

	if ($Script:references -eq $null) {

		# for Assertion
		$Script:references = @()


		for ($i = 0; $i -lt $SourceZipFiles.Count; $i++) {

			# Source Zip File
			$copy_zip_src_filepath = $SourceZipFiles[$i]
			$copy_zip_src_filename = $copy_zip_src_filepath | Split-Path -Leaf

			# Copy Destination Directory
			$copy_zip_dest_dir_name = "Zip Source File $i ($copy_zip_src_filename)"
			$copy_zip_dest_dir_path = Get-Location | Join-Path -ChildPath $copy_zip_dest_dir_name
			new-TestDirectory -Path $copy_zip_dest_dir_path

			# Copy Source Zip File to Destination Directory
			$source_zip_filepath = Copy-Item -Path $copy_zip_src_filepath -Destination $copy_zip_dest_dir_path -PassThru -Force
			$source_zip_filename = $source_zip_filepath | Split-Path -Leaf


			# Rererence Directory
			$reference_dir_name = "Zip Expanded Reference $i ($source_zip_filename)"
			$reference_dir_path = Get-Location | Join-Path -ChildPath $reference_dir_name
			new-TestDirectory -Path $reference_dir_path

			# Expand Zip File -> Rererence Directory (by 'Expand-Archive' Cmdlet)
			Expand-Archive -Path $source_zip_filepath -DestinationPath $reference_dir_path


			# Get File Hash(es)
			$reference_file_hash = @()
			$reference_dir_path | Get-ChildItem -File -Recurse | % { $reference_file_hash += Get-FileHash -Path $_.FullName -Algorithm MD5 }


			# Validation of Root
			if (($reference_dir_path | Get-ChildItem).Count -gt 1) { throw }

			# Set Reference
			$Script:references += @{
				FileHash = $reference_file_hash
				SourceZipFileName = $source_zip_filename
				SourceZipFilePath = $source_zip_filepath
				DirectoryPath = $reference_dir_path
				DirectoryName = $reference_dir_name
				RootPath = ($reference_dir_path | Get-ChildItem).FullName
				RootName = $reference_dir_path | Get-ChildItem | Split-Path -Leaf
			}
		}
	}
}


# Check File Hash(es) in Target Directory by 'Get-FileHash'
Function test-DirectoryFileHash (
	[Parameter(Mandatory = $true)]$Expected_FileHash,
	[Parameter(Mandatory = $true)][string]$Expected_Prefix,
	[Parameter(Mandatory = $true)]$Actual_FileHash,
	[Parameter(Mandatory = $true)][string]$Actual_Prefix
) {

	# Check Number of Files
	if ($Actual_FileHash.Count -ne $Expected_FileHash.Count) { throw }


	# Sort File Hash(es)
	$expected_file_hash = $Expected_FileHash | Sort-Object -Property "Path"
	$actual_file_hash = $Actual_FileHash | Sort-Object -Property "Path"
			
	# Set Prefix Path
	$expected_prefix = $Expected_Prefix
	$actual_prefix = $Actual_Prefix

	# Remove '\' from Prefix Path
	if ($expected_prefix[-1] -ne '\') { $expected_prefix += '\' }
	if ($actual_prefix[-1] -ne '\') { $actual_prefix += '\' }


	# Check File Hash(es)
	for ($i = 0; $i -lt $expected_file_hash.Count; $i++) {

		$expected_path = ([string]$expected_file_hash[$i].Path).Substring($expected_prefix.Length)
		$actual_path = ([string]$actual_file_hash[$i].Path).Substring($actual_prefix.Length)

		$expected_hash = $expected_file_hash[$i].Hash
		$actual_hash = $actual_file_hash[$i].Hash

		if (($actual_path -eq $expected_path) -and ($actual_hash -eq $expected_hash)) {
			$expected_path | Write-Output
		}
		else {
			$null | Write-Output
		}
	}
}


# Test 'Expand-ZipFile' Cmdlet
Function test-ExpandZipFile (
	[Parameter()][int]$Index,
	[Parameter()][string]$Context,
	[Parameter(Mandatory = $true)]$Reference,
	[Parameter(Mandatory = $true)][string]$SourcePath,
	[Parameter()][string]$DestinationPath,
	[Parameter()][switch]$PassThru,
	[Parameter()][string]$Password
) {
	# for Assertion
	$Script:actual = @()


	# Source Zip File
	$source_zip_filename = $SourcePath | Split-Path -Leaf
	$source_zip_filepath = $SourcePath | Resolve-Path

	# Expected Destination Directory
	if (-not [string]::IsNullOrEmpty($DestinationPath)) {
		$expected_dest_dir_path = $DestinationPath | Convert-Path
	}
	else {
		$expected_dest_dir_path = (Get-Location).Path
	}
	$expected_dest_dir_name = $expected_dest_dir_path | Split-Path -Leaf


	# Source Zip File is NOT including only a File (May be including Directory)
	$include_files = ($Reference.FileHash.Count -gt 1)

	# Expected Root
	$expected_root_name = $Reference.RootName
	$expected_root_path = $expected_dest_dir_path | Join-Path -ChildPath $expected_root_name


	# Index (of Context)
	if ($Index -lt 0) { [string]$context_Index = "" }
	else { [string]$context_Index = "($Index) " }

	# Context
	if (-not [string]::IsNullOrEmpty($Context)) { $Context = " ($Context)" }


	# Context: Source Zip File
	Context ($context_Index + "Source Zip File '$source_zip_filename'" + $Context) {

		# Password
		if ([string]::IsNullOrEmpty($Password)) { $with_Password = "" }
		else { $with_Password = " with Password '$Password'" }

		# Expand Zip File
		It "Is Expanded$with_Password into '$expected_root_name' in '$expected_dest_dir_name' Directory." {

			if ([string]::IsNullOrEmpty($DestinationPath)) {

				if ([string]::IsNullOrEmpty($Password)) {

					# Destination=No, Password=No, PassThru=No/Yes
					if ($PassThru) { $Script:actual += Expand-ZipFile $SourcePath -PassThru }
					else           { $Script:actual += Expand-ZipFile $SourcePath }
				}
				else {

					# Destination=No, Password=Yes, PassThru=No/Yes
					if ($PassThru) { $Script:actual += Expand-ZipFile $SourcePath -Password $Password -PassThru }
					else           { $Script:actual += Expand-ZipFile $SourcePath -Password $Password }
				}
			}
			else {
				if ([string]::IsNullOrEmpty($Password)) {

					# Destination=Yes, Password=No, PassThru=No/Yes
					if ($PassThru) { $Script:actual += Expand-ZipFile $SourcePath $DestinationPath -PassThru }
					else           { $Script:actual += Expand-ZipFile $SourcePath $DestinationPath }
				}
				else {

					# Destination=Yes, Password=Yes, PassThru=No/Yes
					if ($PassThru) { $Script:actual += Expand-ZipFile $SourcePath $DestinationPath -Password $Password -PassThru }
					else           { $Script:actual += Expand-ZipFile $SourcePath $DestinationPath -Password $Password }
				}
			}


			# Output is NOT Array OR NOT PassThru
			if ((-not $include_files) -or (-not $PassThru)) {

				# Check Only Existence
				$Script:actual | Should Exist
			}
		}
	

		# Expanded into a File OR Directory
		if (-not $include_files) {

			# Expanded into a FILE
			It "Is Archive of a FILE." { $Script:actual | Test-Path -PathType Leaf }


			# Get Hash (Expected & Actual)
			$expected_hash = $Reference.FileHash.Hash
			$actual_hash = (Get-FileHash -Path $Script:actual -Algorithm MD5).Hash

			# Check File Hash
			It "Contains a Valid File '$expected_root_name'." {

				# Check Path
				$Script:actual | Should Be $expected_root_path

				# Check File Hash
				$actual_hash | Should Be $expected_hash
			}
		}
		else {

			# Expanded into DIRECTORY
			It "Is Archive of DIRECTORY." { $expected_root_path | Test-Path -PathType Container }


			# Get File Hash(es)
			$actual_file_hash = @()
			if ($PassThru) {
				$Script:actual | % { $actual_file_hash += Get-FileHash -Path $_.FullName -Algorithm MD5 }
			}
			else {
				$Script:actual | Get-ChildItem -File -Recurse | % { $actual_file_hash += Get-FileHash -Path $_.FullName -Algorithm MD5 }
			}


			# Expected Number of Expanded Zip File(s)
			$expected_number_of_files = $Reference.FileHash.Count

			# Check Number of Files
			It "Includes $expected_number_of_files File(s)." { $actual_file_hash.Count | Should Be $expected_number_of_files }


			# Check File Hash(es) in Target Directory
			test-DirectoryFileHash `
				-Expected_FileHash $Reference.FileHash `
				-Expected_Prefix $Reference.DirectoryPath `
				-Actual_FileHash $actual_file_hash `
				-Actual_Prefix $expected_dest_dir_path | % {
				
				It "Contains a Valid File '$_'." { $_ | Should NOT BeNullOrEmpty }
			}
		}
	}
}


# Test 'New-ZipFile' Cmdlet
Function test-NewZipFile (
	[Parameter()][int]$Index,
	[Parameter()][string]$Context,
	[Parameter(Mandatory = $true)]$Reference,
	[Parameter(Mandatory = $true)][string]$Expected_ZipFileName,
	[Parameter(Mandatory = $true)][string]$Expected_DestinationDirectoryPath,
	[Parameter(Mandatory = $true)][string]$SourcePath,
	[Parameter()][string]$DestinationPath,
	[Parameter()][switch]$PassThru,
	[Parameter()][string]$Password,
	[Parameter(Mandatory = $true)][string]$ExpandDirectoryPath
) {
	# for Assertion
	$Script:actual = $null


	# Source (File OR Directory) Name
	$source_name = $SourcePath | Split-Path -Leaf

	# Source Type (File OR Directory) 
	if ($SourcePath | Resolve-Path | Test-Path -PathType Leaf) {
		$source_type = "File"
	}
	elseif ($SourcePath | Resolve-Path | Test-Path -PathType Container) {
		$source_type = "Directory"
	}
	else { throw }


	# Expected Destination Directory
	$expected_new_dest_dir_path = $Expected_DestinationDirectoryPath | Convert-Path
	$expected_new_dest_dir_name = $expected_new_dest_dir_path | Split-Path -Leaf

	# Expected Destination Zip File
	$expected_new_zip_filename = $Expected_ZipFileName
	$expected_new_zip_filepath = $expected_new_dest_dir_path | Join-Path -ChildPath $expected_new_zip_filename


	# Delete File (if exists)
	if ($expected_new_zip_filepath | Test-Path) { Remove-Item $expected_new_zip_filepath -Force }


	# Index (of Context)
	if ($Index -lt 0) { $context_Index = "" }
	else { $context_Index = "($Index) " }

	# Context
	if (-not [string]::IsNullOrEmpty($Context)) { $Context = " ($Context)" }


	# Context: Source (File OR Directory)
	Context ($context_Index + "Source $source_type '$source_name'" + $Context) {

		# New Zip File
		It "Is Compressed into '$expected_new_zip_filename' in '$expected_new_dest_dir_name' Directory." {

			if ([string]::IsNullOrEmpty($DestinationPath)) {

				if ([string]::IsNullOrEmpty($Password)) {

					# Destination=No, Password=No, PassThru=No/Yes
					if ($PassThru) { $Script:actual = New-ZipFile $SourcePath -PassThru }
					else           { $Script:actual = New-ZipFile $SourcePath }
				}
				else {

					# Destination=No, Password=Yes, PassThru=No/Yes
					if ($PassThru) { $Script:actual = New-ZipFile $SourcePath -Password $Password -PassThru }
					else           { $Script:actual = New-ZipFile $SourcePath -Password $Password }
				}
			}
			else {
				if ([string]::IsNullOrEmpty($Password)) {

					# Destination=Yes, Password=No, PassThru=No/Yes
					if ($PassThru) { $Script:actual = New-ZipFile $SourcePath $DestinationPath -PassThru }
					else           { $Script:actual = New-ZipFile $SourcePath $DestinationPath }
				}
				else {

					# Destination=Yes, Password=Yes, PassThru=No/Yes
					if ($PassThru) { $Script:actual = New-ZipFile $SourcePath $DestinationPath -Password $Password -PassThru }
					else           { $Script:actual = New-ZipFile $SourcePath $DestinationPath -Password $Password }
				}
			}


			# Check Output (PassThru=OFF/ON)
			if (-not $PassThru) {

				# PassThru=OFF
				$Script:actual | Should BeNullOrEmpty
			}
			else {

				# PassThru=ON
				$Script:actual | Should Be $expected_new_zip_filepath
			}


			# Check Existence
			$expected_new_zip_filepath | Should Exist
		}
	}


	# Create Validation Workspace Directory
	new-TestDirectory -Path $ExpandDirectoryPath


	# Expand New Created Zip File for Validation
	test-ExpandZipFile `
		-Index $Index `
		-Context "Created by 'New-ZipFile'" `
		-Reference $Reference `
		-SourcePath $expected_new_zip_filepath `
		-DestinationPath $ExpandDirectoryPath `
		-PassThru `
		-Password $Password
}


Describe "Expand-ZipFile" {

	# Initialize Rerence(s)
	set-Reference -SourceZipFiles $Script:original_zip_files


	# Default Test
	for ($i = 0; $i -lt $Script:references.Count; $i++) {

		# Test Expand-ZipFile
		# (Expand to Current Directory)
		test-ExpandZipFile `
			-Index -1 `
			-Context "Default: Only with 'SourcePath' Parameter w/o Parameter Name" `
			-Reference $Script:references[$i] `
			-SourcePath $Script:references[$i].SourceZipFilePath
	}


	# Parameter Tests
	for ($i = 0; $i -lt $Script:references.Count; $i++) {

		# $j=00: DestinationPath="."
		# $j=01: DestinationPath=Directory Name
		# $j=02: DestinationPath=Relative Path
		# $j=03: DestinationPath=Absolute Path
		# $j=04: PassThru=OFF (DestinationPath=Directory Name)
		# $j=05: PassThru=ON  (DestinationPath=Directory Name)
		# $j=06: Force=OFF -> Error     (DestinationPath=Directory Name)
		# $j=07: Force=ON  -> NOT Error (DestinationPath=Directory Name)
		for ($j = 0; $j -lt 8; $j++) {

			# Set Destination Directory Name
			switch ($j) {
				0 { $dir_name = "" }
				1 { $dir_name = "DestinationPath=Directory Name" }
				2 { $dir_name = "DestinationPath=Relative Path" }
				3 { $dir_name = "DestinationPath=Absolute Path" }
				4 { $dir_name = "PassThru=OFF" }
				5 { $dir_name = "PassThru=ON" }
				6 { $dir_name = "Force=OFF" }
				7 { $dir_name = "Force=ON" }
				default { throw }
			}

			# Create Destination Directory
			# ($j=01..07)
			if ($j -gt 0) {
				$expand_zip_dest_dir_name = "Expand-ZipFile Test $i-" + $j.ToString("D2") + " - " + $dir_name + " (" + $Script:references[$i].SourceZipFileName + ")"
				$expand_zip_dest_dir_path = Get-Location | Join-Path -ChildPath $expand_zip_dest_dir_name
				new-TestDirectory -Path $expand_zip_dest_dir_path
			}


			# 'DestinationPath' Parameter
			switch ($j) {
				0 { $dest_path = "." }
				1 { $dest_path = "$expand_zip_dest_dir_name" }
				2 { $dest_path = ".\$expand_zip_dest_dir_name" }
				3 { $dest_path = "$expand_zip_dest_dir_path" }
				default { $dest_path = "$expand_zip_dest_dir_name" }
			}

			# Context
			switch ($j) {
				0 { $context = "'DestinationPath'=Current Directory: '$dest_path'" }
				1 { $context = "'DestinationPath'=Directory Name: '$dest_path'" }
				2 { $context = "'DestinationPath'=Relative Path: '$dest_path'" }
				3 { $context = "'DestinationPath'=Absolute Path: '$dest_path'" }
				4 { $context = "'PassThru'=OFF" }
				5 { $context = "'PassThru'=ON" }
				6 { $context = "'Force'=OFF: NOT Overwirte" }
				7 { $context = "'Force'=ON: Overwirtten" }
				default { throw }
			}

			# Expand Zip File
			if ($j -lt 6) {

				# Test Expand-ZipFile
				test-ExpandZipFile `
					-Index $j `
					-Context $context `
					-Reference $Script:references[$i] `
					-SourcePath $Script:references[$i].SourceZipFilePath `
					-DestinationPath $dest_path `
					-PassThru:($j -eq 5)
			}
			else {

				# Expand Source Zip File by 'Expand-Archive'
				Expand-Archive -Path $Script:references[$i].SourceZipFilePath -DestinationPath $expand_zip_dest_dir_path
			
				# Get Target File
				$test_filepath = ($expand_zip_dest_dir_path | Get-ChildItem | Get-ChildItem -File)[0].FullName
				$test_filename = $test_filepath | Split-Path -Leaf

				# Source Zip File Name for Context
				$source_zip_filename = $Script:references[$i].SourceZipFileName


				# Check ONLY a Test File
				Context "($j) Test File '$test_filename' included in Source Zip File '$source_zip_filename' ($context)" {

					# Get Reference Hash of Target File
					$hash_before = (Get-FileHash -Path $test_filepath).Hash


					# Change a File
					'HOGE' | Out-File -FilePath $test_filepath -Encoding ascii -Append


					# Expand Source Zip File by 'Expand-ZipFile'
					if ($j -eq 6) {

						# Expand-ZipFile (w/o 'Force' Parameter) into the Same Directory
						Expand-ZipFile $Script:references[$i].SourceZipFilePath $expand_zip_dest_dir_path
					}
					else {

						# Expand-ZipFile WITH 'Force' Parameter into the Same Directory
						Expand-ZipFile $Script:references[$i].SourceZipFilePath $expand_zip_dest_dir_path -Force
					}

					# Get Actual Hash of Target File
					$hash_after = (Get-FileHash -Path $test_filepath).Hash


					# Check only a File (Restriction)
					if ($j -eq 6) {
						It "Is Expanded in Directory '$expand_zip_dest_dir_name', and Different from Source File." {
							$hash_after | Should NOT Be $hash_before
						}
					}
					else {
						It "Is Expanded in Directory '$expand_zip_dest_dir_name', and Same As Source File." {
							$hash_after | Should Be $hash_before
						}
					}
				}
			}
		}
	}


	# Additional Tests for 'SuppressOutput' Parameter
	Context "($j) Output of Cmdlet with 'SuppressOutput' Parameter" {

		for ($i = 0; $i -lt $Script:references.Count; $i++) {

			$filename = $Script:references[$i].SourceZipFileName
			$filepath = $Script:references[$i].SourceZipFilePath

			It "Is Nothing. ('$filename')" { Expand-ZipFile -Path $filepath -Force -SuppressOutput | Should BeNullOrEmpty }
		}
	}

	$j++
	Context "($j) FullName of Output of Cmdlet with both 'SuppressOutput' and 'PassThru' Parameter" {

		for ($i = 0; $i -lt $Script:references.Count; $i++) {

			$filename = $Script:references[$i].SourceZipFileName
			$filepath = $Script:references[$i].SourceZipFilePath

			It "Is Same as the Output with 'PassThru' Parameter. ('$filename')" {
				$expected = Expand-ZipFile -Path $filepath -Force -PassThru
				$actual = Expand-ZipFile -Path $filepath -Force -SuppressOutput -PassThru

				# Check Number of Output
				$actual.Count | Should Be $expected.Count

				# Check Each of Content (FullName)
				0..($expected.Count - 1) | % { $actual[$_].FullName -eq $expected[$_].FullName }
			}
		}
	}
}


Describe "New-ZipFile" {

	# Initialize Rerence(s)
	set-Reference -SourceZipFiles $Script:original_zip_files


	# Default Test
	for ($i = 0; $i -lt $Script:references.Count; $i++) {

		# Validation Workspace Directory
		$expand_zip_dest_dir_name = "New-ZipFile Test $i-00 - Validation Workspace (" + $Script:references[$i].SourceZipFileName + ")"
		$expand_zip_dest_dir_path = Get-Location | Join-Path -ChildPath $expand_zip_dest_dir_name

		# Test New-ZipFile
		# (Create New Zip File in Current Directory)
		test-NewZipFile `
			-Index -1 `
			-Context "Default: Only with 'SourcePath' Parameter w/o Parameter Name" `
			-Reference $Script:references[$i] `
			-Expected_ZipFileName $Script:references[$i].SourceZipFileName `
			-Expected_DestinationDirectoryPath (Get-Location).Path `
			-SourcePath $Script:references[$i].RootPath `
			-ExpandDirectoryPath $expand_zip_dest_dir_path
	}
	

	# Parameter Tests
	for ($i = 0; $i -lt $Script:references.Count; $i++) {

		# $j=00: DestinationPath="."
		# $j=01: DestinationPath=Existing Directory Name
		# $j=02: DestinationPath=Existing Directory Path (Relative)
		# $j=03: DestinationPath=Existing Directory Path (Absolute)
		#
		# $j=04: DestinationPath=Zip File Name
		# $j=05: DestinationPath=Zip File Path (Relative)
		# $j=06: DestinationPath=Zip File Path (Absolute)
		#
		# $j=07: PassThru=OFF (DestinationPath=Directory Name)
		# $j=08: PassThru=ON  (DestinationPath=Directory Name)
		#
		# $j=09: with Password (DestinationPath=Directory Name)
		#
		# $j=10: Force (DestinationPath=Directory Name)
		for ($j = 0; $j -lt 11; $j++) {

			# Set Destination Directory Name
			switch ($j) {
				0 { $dir_name = "" }
				1 { $dir_name = "DestinationPath=Existing Directory Name" }
				2 { $dir_name = "DestinationPath=Existing Directory Relative Path" }
				3 { $dir_name = "DestinationPath=Existing Directory Absolute Path" }
				4 { $dir_name = "DestinationPath=New Zip File Name" }
				5 { $dir_name = "DestinationPath=New Zip File Relative Path" }
				6 { $dir_name = "DestinationPath=New Zip File Absolute Path" }
				7 { $dir_name = "PassThru=OFF" }
				8 { $dir_name = "PassThru=ON" }
				9 { $dir_name = "with Password" }
				10 { $dir_name = "with Force Parameter" }
				default { throw }
			}

			# Destination Directory
			if (($j -eq 0) -or ($j -eq 4)) {
				# Current Directory
				# ($j=00)
				$new_dest_dir_path = (Get-Location).Path
				$new_dest_dir_name = $new_dest_dir_path | Split-Path -Leaf
			}
			else {
				# Create Destination Directory
				# ($j=01..12)
				$new_dest_dir_name = "New-ZipFile Test $i-" + $j.ToString("D2") + " - " + $dir_name + " (" + $Script:references[$i].SourceZipFileName + ")"
				$new_dest_dir_path = Get-Location | Join-Path -ChildPath $new_dest_dir_name
				new-TestDirectory -Path $new_dest_dir_path
			}


			# Expected New Zip File
			if (($j -ge 4) -and ($j -le 6)) {
				$expected_new_zip_filename = [System.IO.Path]::GetFileNameWithoutExtension($Script:references[$i].SourceZipFileName) + $j.ToString("D2") + ".zip"
			}
			else {
				$expected_new_zip_filename = $Script:references[$i].SourceZipFileName
			}
			$expected_new_zip_filepath = $new_dest_dir_path | Join-Path -ChildPath $expected_new_zip_filename


			# 'DestinationPath' Parameter
			switch ($j) {
				0 { $dest_path = "." }
				1 { $dest_path = "$new_dest_dir_name" }
				2 { $dest_path = ".\$new_dest_dir_name" }
				3 { $dest_path = "$new_dest_dir_path" }
				4 { $dest_path = $expected_new_zip_filename }
				5 { $dest_path = ".\$new_dest_dir_name\$expected_new_zip_filename" }
				6 { $dest_path = "$expected_new_zip_filepath" }
				default { $dest_path = "$new_dest_dir_name" }
			}

			# Password
			if ($j -eq 9) { $password = "12345" }
			else { $password = [string]::Empty }

			# Context
			switch ($j) {
				0 { $context = "'DestinationPath'=Current Directory: '$dest_path'" }
				1 { $context = "'DestinationPath'=Directory Name: '$dest_path'" }
				2 { $context = "'DestinationPath'=Directory Relative Path: '$dest_path'" }
				3 { $context = "'DestinationPath'=Directory Absolute Path: '$dest_path'" }
				4 { $context = "'DestinationPath'=New Zip File Name: '$dest_path'" }
				5 { $context = "'DestinationPath'=New Zip File Relative Path: '$dest_path'" }
				6 { $context = "'DestinationPath'=New Zip File Absolute Path: '$dest_path'" }
				7 { $context = "'PassThru'=OFF" }
				8 { $context = "'PassThru'=ON" }
				9 { $context = "Password='$password'" }
				10 { $context = "with 'Force' Parameter" }
				default { throw }
			}

			# Validation Workspace Directory (Only Preparing Name and Path)
			$expand_zip_dest_dir_name = "New-ZipFile Test $i-" + $j.ToString("D2") + " - Validation Workspace (" + $expected_new_zip_filename + ")"
			$expand_zip_dest_dir_path = Get-Location | Join-Path -ChildPath $expand_zip_dest_dir_name


			if ($j -lt 10) {

				# Test New-ZipFile
				test-NewZipFile `
					-Index $j `
					-Context $context `
					-Reference $Script:references[$i] `
					-Expected_ZipFileName $expected_new_zip_filename `
					-Expected_DestinationDirectoryPath $new_dest_dir_path `
					-SourcePath $Script:references[$i].RootPath `
					-DestinationPath $dest_path `
					-PassThru:($j -eq 8) `
					-Password $password `
					-ExpandDirectoryPath $expand_zip_dest_dir_path


				# Invalid Password Check
				if ($j -eq 9) {

					# Expand with INVALID Password
					Context "($j) New Zip File '$expected_new_zip_filename' with Password '$password'" {
						
						# Invalid Password
						$invalid_password = "ABC"

						# Create Validation Workspace Directory
						new-TestDirectory -Path ($expand_zip_dest_dir_path = "$expand_zip_dest_dir_path - Invalid Password")

						It "Could NOT Expanded with INVALID Password '$invalid_password'." {
							
							# 'Expand-ZipFile' with INVALID Password (with 'Silent' Parameter, because Progress Bar does NOT disappear).
							{ Expand-ZipFile $expected_new_zip_filepath $expand_zip_dest_dir_path -Password $invalid_password -Silent -ErrorAction 'Stop' } | Should Throw
						}
					}
				}
			}
			else {

				# Create Dummy File
				($content = 'HOGE') | Out-File -FilePath $expected_new_zip_filepath


				# 'Force' Parameter Check
				Context "($j) Existing File '$expected_new_zip_filename'" {

					It "Could NOT Be Overwritten by 'New-ZipFile' w/o 'Force' Parameter." {

						# New-ZipFile w/o 'Force' Parameter
						{ New-ZipFile $Script:references[$i].RootPath $new_dest_dir_path -Silent -ErrorAction 'Stop' } | Should Throw
					}


					It "Is Overwritten by 'New-ZipFile' with 'Force' Parameter." {

						# New-ZipFile with 'Force' Parameter
						New-ZipFile $Script:references[$i].RootPath $new_dest_dir_path -Force

						Get-Content -Path $expected_new_zip_filepath -TotalCount 1 | Should NOT Be $content
					}
				}


				# Create Validation Workspace Directory
				new-TestDirectory -Path $expand_zip_dest_dir_path


				# Expand New Created Zip File for Validation
				test-ExpandZipFile `
					-Index $j `
					-Context "Created by 'New-ZipFile' with 'Force' Parameter" `
					-Reference $Script:references[$i] `
					-SourcePath $expected_new_zip_filepath `
					-DestinationPath $expand_zip_dest_dir_path `
					-PassThru
			}
		}
	}
}
