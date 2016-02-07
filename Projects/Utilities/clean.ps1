# BUILDLet Cleaning Script
@"
BUILDLet Solution Cleaning Script
Copyright (C) 2015 Daiki Sakamoto

"@

$Target_Projects = @(
    "BUILDLet.Utilities"
    "BUILDLet.UtilitiesHelpDocumentation"
    "BUILDLet.UtilitiesTests"

    "BUILDLet.Utilities.PInvoke"
    "BUILDLet.Utilities.PInvokeTests"

    "BUILDLet.Utilities.WPF"
    "BUILDLet.Utilities.WPFTests"

    "BUILDLet.WOL"
    "BUILDLet.WOLSetup"
    "BUILDLet.WOLSetupBootstrapper"

    "BUILDLet.Utilities.PowerShell"
    "BUILDLet.Utilities.PowerShellTests"
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

$Remove_Objects = @()


# Add "obj" and "bin" folders to remove queue
$Target_Projects | % {

    if (($project_folder = Get-Location | Join-Path -ChildPath $_) | Test-Path -PathType Container)
    {
        $Target_Folders | % {
            
            if (($target = Join-Path -Path $project_folder -ChildPath $_) | Test-Path -PathType Container)
            {
                $Remove_Objects += $target
            }
        }
    }
}


# Add additional files to remove queue
$Additional_Files | % {
    
    if (($target = Get-Location | Join-Path -ChildPath $_) | Test-Path)
    {
        $Remove_Objects += $target
    }
}


# Show continue message
"The following folder(s) and file(s) are removed."
""
$Remove_Objects
if ($Remove_Objects.Count -eq 0) { "(None)" }
""
"Please hit ENTER key to continue."
Read-Host


# Remove folders and files
$Remove_Objects | % {

    Remove-Item -Path $_ -Recurse -Force -Verbose
}


# Show exit message
""
"Please hit ENTER key to exit."
Read-Host
