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
@"
****************************************
 BUILDLet PowerShell Module Test
 Initialize Script Template
                            Version 1.0
 Copyright (C) 2015-2017 Daiki Sakamoto
****************************************
"@ | Write-Host -ForegroundColor Green


# Build Configuration File Path
$BuildConfigFilePath = '..\BUILDLet.Utilities.PowerShell\LastBuild'

# Target Module(s)
[string[]]$TargetModuleName = @(
	'BUILDLet.Utilities.PowerShell'
)

# Required Module(s)
[string[]]$RequiredModulePath = @()



# Initialize Current Location to Project Directory
($project_dir = $PSCommandPath | Split-Path -Parent) | Set-Location
("Change Location to '" + (Get-Location) + "'.") | Write-Host -ForegroundColor Yellow


# Get Build Configuration (& Validation)
if ((($build_config = Get-Content -Path $BuildConfigFilePath) -ne 'Debug') -and ($build_config -ne 'Release')) { throw }

# Set Working Directory
$work_dir = $project_dir | Join-Path -ChildPath 'bin' | Join-Path -ChildPath $build_config

# Create Working Directory (if it does NOT exist)
if (-not (Test-Path -Path $work_dir)) { New-Item -Path $work_dir -ItemType Directory > $null }

# Change Current Location to Working Directory
$work_dir | Set-Location
("Change Location to '" + (Get-Location) + "'.") | Write-Host -ForegroundColor Yellow



# Import Required Module(s)
$RequiredModulePath | % {

	# Import Required Module
	$imported = Import-Module -Name ($project_dir | Join-Path -ChildPath $_ | Resolve-Path) -Force -PassThru

	# Output Message
	if ($imported -ne $null) { "Import '$imported' PowerShell Module." | Write-Host -ForegroundColor Yellow }
}



# Copy & Import Target Module(s)
$TargetModuleName | % {
	
	$target_module = $_

	# Check if the target module has been already imported
	if ((Get-Module -Name $target_module) -eq $null) {

		$target_module_copy_src_path = $project_dir | `
			Join-Path -ChildPath '..\bin\' | `
			Join-Path -ChildPath $build_config | `
			Join-Path -ChildPath 'WindowsPowerShell\Modules\' | `
			Join-Path -ChildPath $target_module

		$target_module_copy_dest_path = $work_dir | `
			Join-Path -ChildPath 'WindowsPowerShell\Modules\'

		$target_module_dir_path = $target_module_copy_dest_path | `
			Join-Path -ChildPath $target_module

		# Create Module Directory (if NOT exists)
		if (-not ($target_module_copy_dest_path | Test-Path)) { New-Item -Path $target_module_copy_dest_path -ItemType Directory > $null }

		# Copy Target Module
		Copy-Item -Path $target_module_copy_src_path -Destination $target_module_copy_dest_path -Recurse -Force

		# Import Target Module
		$imported = Import-Module -Name $target_module_dir_path -Force -PassThru

		# Output Message
		if ($imported -ne $null) { "Import '$imported' PowerShell Module." | Write-Host -ForegroundColor Yellow }
	}
}

# RETURN (Current Directory = Working Directory)
return $work_dir
