@"
BUILDLet PowerShell PackageMaker Toolkit Sample Script
Copyright (C) 2015 Daiki Sakamoto
"@

# Script Name
"[init.ps1] Version " + ($version = '1.0.7.0')


####################################################################################################
##  Settings
####################################################################################################
# Verbose
$VerbosePreference = 'SilentlyContinue'

# Debug
$DebugPreference = 'SilentlyContinue'

# Error Action
$ErrorActionPreference = 'Stop'


####################################################################################################
##  Definition
####################################################################################################
# Required Modules
$required_modules = @(
    'BUILDLet.PowerShell.Utilities'
    'BUILDLet.PowerShell.PackageMaker'
)

# INI File
$ini_file = '.\sample.ini'

# Setting Items in INI File
$Global:Settings = @{

    # [Source]
    'Source' = @{
        'Package' = $null
        'README'  = $null
        'AUTORUN' = $null
        'Icon'    = $null
        'ReleaseNotes' = $null
    }

    # [Destination]
    'Destination' = @{
        'WorkingDirectory' = $null
        'ReleaseDirectory' = $null
        'ReleaseNotes'     = $null
        'ISO' = $null
    }

    # [Project]
    'Project' = @{
        'Name'          = $null
        'Version'       = $null
        'DirectoryName' = $null
        'DriverRoot'    = $null
        'Models'        = $null
    }

    # [Model]
    'Model' = @{
        'MODEL1' = $null
        'MODEL2' = $null
        'MODEL3' = $null
        'MODEL4' = $null
        'MODEL5' = $null
        'MODEL6' = $null
        'MODEL7' = $null
        'MODEL8' = $null
        'MODEL9' = $null
    }

    # [Language]
    'Language' = @{
        'en' = $null
        'ja' = $null
    }

    # [Readme\LCID]
    'Readme\LCID' = @{
        'en' = $null
        'ja' = $null
    }
}


####################################################################################################
##  Initialization
####################################################################################################
# Import the required modules
""
$required_modules | % {
    if ((Get-Module | Select-String ($module = $_)) -eq $null) {
        "Importing module '$module'..."
        Import-Module $module
    }
    else {
        "Required module '$module' is already imported."
    }
}


# Create Search Key (Pairs of Section and Key) for reading INI file
$search_keys = @{}
$Settings.Keys | % {
    $section = $_
    $search_keys[$section] = @()
    $Settings[$section].Keys | % { $search_keys[$section] += $_ }
}


# Read INI File
""
"Reading INI File '$ini_file'..."
$search_keys.Keys | % {
    $section = $_
    $search_keys[$section] | % {
        $key = $_

        # Read Setting Value
        $Settings[$section][$key] = Get-PrivateProfileString -Path $ini_file -Section $section -Key $key

        # Output
        "[$section] $key=" + $Settings[$section][$key]
    }
}


# Getting Project Information
""
"[Project Information]"
$Global:Project_Name     = $Settings['Project']['Name']
$Global:Project_Version  = $Settings['Project']['Version']
$Global:Project_Dir_Name = $Settings['Project']['DirectoryName']
"Target: $Project_Name Version $Project_Version"
"(Directory Name='$Project_Dir_Name')"


# Getting Model Information
""
"[Model Information]"
"Model=" + ($models = $Settings['Project']['Models'])
""
$Global:Driver_Dirs = New-Object -TypeName 'string[]' -ArgumentList $models.Count
$Global:Model_Names = New-Object -TypeName 'string[]' -ArgumentList $models.Count

[string[]]$models = $models.Split(',').Trim()
for ($i = 0; $i -lt $models.Count; $i++) {
    
    $Model_Names[$i] = $Settings['Model'][$models[$i]].Split(',')[0].Trim()
    $Driver_Dirs[$i] = $Settings['Model'][$models[$i]].Split(',')[1].Trim()

    # Output
    "[$models[$i]] '" + $Model_Names[$i] + "'='" + $Driver_Dirs[$i] + "'"
}


# Initialize Working Directory and Release Directory
""
$Global:Work_Dir = $Settings['Destination']['WorkingDirectory']
"Renewing Working Directory '$Work_Dir'..."
Reset-Directory -Path $Work_Dir -Force

$Global:Release_Dir = $Settings['Destination']['ReleaseDirectory']
"Renewing Release Directory '$Release_Dir'..."
Reset-Directory -Path $Release_Dir -Force
""
""
