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
@"
****************************************
 BUILDLet PowerShell Module
 Deploy Script Template
                            Version 1.0
 Copyright (C) 2015-2017 Daiki Sakamoto
****************************************
"@ | Write-Host -ForegroundColor Green


# Target PowerShell Module Name
$ModuleName = 'BUILDLet.PackageMaker.PowerShell'

# File List of Target PowerShell Module
$ModuleFiles = @(
	'BUILDLet.PackageMaker.PowerShell.psd1'
	'BUILDLet.PackageMaker.PowerShell.psm1'
)


# Copy Module File(s)
@('Debug', 'Release') | % {

	$build_config = $_
	$dest_dir_path = $PSCommandPath | Split-Path -Parent `
		| Join-Path -ChildPath '..\bin' `
		| Join-Path -ChildPath $build_config `
		| Join-Path -ChildPath 'WindowsPowerShell\Modules' `
		| Join-Path -ChildPath $ModuleName

	# Create Target Directory (if NOT Exists)
	if (-not ($dest_dir_path | Test-Path)) { New-Item -Path $dest_dir_path -ItemType Directory -Force > $null }

	# Copy File(s)
	$ModuleFiles | % {

		$src_filepath = $PSCommandPath | Split-Path -Parent | Join-Path -ChildPath $_

		# Copy File
		Copy-Item -Path $src_filepath -Destination $dest_dir_path -Force
	}

	"Deploy '$ModuleName' PowerShell Module -> '$dest_dir_path'." | Write-Host -ForegroundColor Yellow
}
