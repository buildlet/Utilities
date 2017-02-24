@"
*****************************************
BUILDLet Solution Cleaning Script
Copyright (C) 2015, 2016 Daiki Sakamoto
*****************************************

"@ | Write-Host -ForegroundColor Green


$Targets = @(
    '.\bin\Debug'
    '.\bin\Release'

    '.\*\bin\Debug'
    '.\*\bin\Release'
    '.\*\obj\Debug'
    '.\*\obj\Release'

    '.\TestResults\*'
)


# Show Confirmation Message
$Targets | ? { $_ | Test-Path } | % { $_ | Convert-Path } | % {
    "対象 `"$_`" に対して `"ディレクトリの削除`" を実行します。" | Write-Warning
}


# Wait to Continue
""
"Please Hit ENTER Key to Continue."| Write-Host -ForegroundColor Green
$null = Read-Host


# Remove Item(s)
$Targets | ? { $_ | Test-Path } | % {
    Remove-Item -Path $_ -Recurse -Force -Verbose -WhatIf
}


# Wait to Exit
""
"Please Hit ENTER Key to Exit." | Write-Host -ForegroundColor Green
$null = Read-Host
