@"
****************************************
 BUILDLet Solution Cleaning Script
                            Version 2.0
 Copyright (C) 2015-2017 Daiki Sakamoto
****************************************
"@ | Write-Host -ForegroundColor Green


$Targets = @(
#    '.\bin\*'
#    '.\obj\*'

    '.\*\bin'
    '.\*\obj'

    '.\TestResults\*'
)


# Get and Set Current Location
($PSCommandPath | Split-Path -Parent) | Set-Location

# Show Confirmation Message
$list = "削除対象ディレクトリ:`n"
$Targets | ? { $_ | Test-Path } | % { $_ | Convert-Path } | % { $list += "`t$_`n" }
$confirm_yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "削除する"
$confirm_no  = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "削除しない"
$confirmation = [System.Management.Automation.Host.ChoiceDescription[]]($confirm_yes, $confirm_no)
if (($host.ui.PromptForChoice($list, "これらのディレクトリを削除しますか？", $confirmation, 1)) -eq 0) {

	# Remove Item(s)
	$Targets | ? { $_ | Test-Path } | % {
		Remove-Item -Path $_ -Recurse -Force -Verbose #-WhatIf
	}
}

# Wait to Exit
"Press any key to exit." | Write-Host
[void]$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
