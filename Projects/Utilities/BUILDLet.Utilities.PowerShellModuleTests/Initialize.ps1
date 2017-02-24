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

# Build Configuration File Path
$Script:build_config_file_path = '..\BUILDLet.Utilities.PowerShell\LastBuild'

# Target Module(s)
[string[]]$Global:TargetModule = @(
	'BUILDLet.Utilities.PowerShell'
)

# Required Module(s)
[string[]]$Global:RequiredModule = @()

# Wroking Directory
[string]$Global:WorkingDirectory = [string]::Empty



# Get Project Directory, and Set Current Location to it
($project_dir = $PSCommandPath | Split-Path -Parent) | Set-Location

# Get Build Configuration (& Validation)
if ((($build_config = Get-Content -Path $build_config_file_path) -ne 'Debug') -and ($build_config -ne 'Release')) { throw }

# Set Working Directory
$Global:WorkingDirectory = $project_dir | Join-Path -ChildPath 'bin' | Join-Path -ChildPath $build_config

# Create Working Directory (if it does NOT exist)
if (-not (Test-Path -Path $Global:WorkingDirectory)) { New-Item -Path $Global:WorkingDirectory -ItemType Directory > $null }

# Change Current Location to Working Directory
Set-Location -Path $Global:WorkingDirectory



# Import Required Module(s)
$Global:RequiredModule | % {

	# Import Required Module
	$imported = Import-Module -Name ($project_dir | Join-Path -ChildPath $_ | Resolve-Path) -Force -PassThru

	# Output Message
	if ($imported -ne $null) { "'$imported' is imported." | Write-Host -ForegroundColor Yellow }
}



# Copy & Import Target Module(s)
$Global:TargetModule | % {
	
	$target_module = $_

	# Check if the target module has been already imported
	if ((Get-Module -Name $target_module) -eq $null) {

		$target_module_copy_src_path = $project_dir | `
			Join-Path -ChildPath '..\bin\' | `
			Join-Path -ChildPath $build_config | `
			Join-Path -ChildPath 'WindowsPowerShell\Modules\' | `
			Join-Path -ChildPath $target_module

		$target_module_copy_dest_path = $Global:WorkingDirectory | `
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
		if ($imported -ne $null) { "'$imported' is imported." | Write-Host -ForegroundColor Yellow }
	}
}
