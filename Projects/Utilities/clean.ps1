# BUILDLet Cleaning Script
@"
BUILDLet Solution Cleaning Script
Copyright (C) 2015 Daiki Sakamoto

"@

$Target_Projects = @(
    "BUILDLet.Utilities"
    "BUILDLet.Utilities.WPF"
    "BUILDLet.Utilities.WPFTest"
    "BUILDLet.UtilitiesDocumentation"
    "BUILDLet.UtilitiesTest"
    "BUILDLet.WOL"
    "BUILDLet.WOLSetup"
    "BUILDLet.WOLSetupBootstrapper"
    "BUILDLet.Test.ConsoleApplications\randum300"
    "BUILDLet.Test.ConsoleApplications\stderr300"
    "BUILDLet.Test.ConsoleApplications\stdout300"
    "BUILDLet.Test.ConsoleApplications\exit999"
    "BUILDLet.Utilities.PowerShell"
    "BUILDLet.Utilities.PowerShellTest"
    "BUILDLet.Utilities.PowerShellSetup"
    "BUILDLet.Utilities.PowerShellSetup64"
    "BUILDLet.Utilities.PowerShellSetupBootstrapper"
    "BUILDLet.TestData.ConsoleApplications\stdout300"
    "BUILDLet.TestData.ConsoleApplications\stderr300"
    "BUILDLet.TestData.ConsoleApplications\randum300"
    "BUILDLet.TestData.ConsoleApplications\exit999"
)

$Target_Folders = @(
    "obj"
    "bin"
)

$Additional_Files = @(
    "TestResults\*"
)

$Remove_Folders = @()


# Add "obj" and "bin" folders to remove queue
$Target_Projects | % {

    $project_folder = Get-Location | Join-Path -ChildPath $_
    if ((Test-Path -Path $project_folder) -and ((Get-Item -Path $project_folder).PSIsContainer))
    {
        $Target_Folders | % {

            if (($target = Join-Path -Path $project_folder -ChildPath $_) | Test-Path)
            {
                $Remove_Folders += $target
            }
        }
    }
}


# Add additional files to remove queue
$Additional_Files | % {
    
    if (($target = Get-Location | Join-Path -ChildPath $_) | Test-Path)
    {
        $Remove_Folders += $target
    }
}


# Show continue message
"The following folder(s) and file(s) are removed."
""
$Remove_Folders | % { $_ }
if ($Remove_Folders.Count -eq 0) { "(None)" }
""
"Please hit ENTER key to continue."
Read-Host


# Remove folders and files
$Remove_Folders | % {

    Remove-Item -Path $_ -Recurse -Force -Verbose
}


# Show exit message
""
"Please hit ENTER key to exit."
Read-Host
