# BUILDLet Cleaning Script
@"
BUILDLet Solution Cleaning Script
Copyright (C) 2015 Daiki Sakamoto

"@

$TargetProjects =
    "BUILDLet.Utilities",
    "BUILDLet.Utilities.WPF",
    "BUILDLet.Utilities.WPFTest",
    "BUILDLet.UtilitiesDocumentation",
    "BUILDLet.UtilitiesTest",
    "BUILDLet.WOL",
    "BUILDLet.WOLSetup",
    "BUILDLet.WOLSetupBootstrapper",
    "BUILDLet.PowerShell.Utilities"

$TargetFolders =
    "obj",
    "bin"

$AdditionalFiles =
    "TestResults\*",
    "BUILDLet.WOLSetup\Sources\*",
    "BUILDLet.WOLSetupBootstrapper\Sources\*"

$RemoveFolders = @()


# Add "obj" and "bin" folders to remove queue
$TargetProjects | % {

    $ProjectFolder = Get-Location | Join-Path -ChildPath $_
    if ((Test-Path -Path $ProjectFolder) -and ((Get-Item -Path $ProjectFolder).PSIsContainer))
    {
        $TargetFolders | % {

            if (($target = Join-Path -Path $ProjectFolder -ChildPath $_) | Test-Path)
            {
                $RemoveFolders += $target
            }
        }
    }
}


# Add additional files to remove queue
$AdditionalFiles | % {
    
    if (($target = Get-Location | Join-Path -ChildPath $_) | Test-Path)
    {
        $RemoveFolders += $target
    }
}


# Show continue message
"The following folder(s) and file(s) are removed."
""
$RemoveFolders | % { $_ }
if ($RemoveFolders.Count -eq 0) { "(None)" }
""
"Please hit ENTER key to continue."
Read-Host


# Remove folders and files
$RemoveFolders | % {

    Remove-Item -Path $_ -Recurse -Force -Verbose
}


# Show exit message
""
"Please hit ENTER key to exit."
Read-Host
