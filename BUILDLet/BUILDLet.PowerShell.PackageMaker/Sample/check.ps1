@"
BUILDLet PowerShell PackageMaker Toolkit Sample Script
Copyright (C) 2015 Daiki Sakamoto
"@

# Script Name
"[check.ps1] Version " + ($version = '1.0.7.0')


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
# (None)


####################################################################################################
##  Initialization
####################################################################################################
# (None)


####################################################################################################
##  Main
####################################################################################################
# Gettinig Start Time
"[START] " + ($start = (Get-Date)).ToString("yyyy/MM/dd hh:mm:ss")


# Getting Version Information
""
"Getting Version Information from INF Files..."
$project_root = Join-Path -Path $Release_Dir -ChildPath $Project_Dir_Name

# for Models
$version_models = @()
for ($i = 0; $i -lt $Driver_Dirs.Count; $i++) {

    $model = $Model_Names[$i]
    $driver_dir = $Driver_Dirs[$i]
    $version_languages = @()

    # for Language Directories
    $project_root | Join-Path -ChildPath $driver_dir | Get-ChildItem -Directory | % {
        
        $lang_dir = $_.FullName
        $version_drivers = @()

        # Set and Validate Language
        $lang = $Settings['Language'][$lang_dir.Split([System.IO.Path]::DirectorySeparatorChar)[-1]]
        if ($lang -eq $null) { throw New-Object System.Management.Automation.JobFailedException }


        # for each INF File
        $lang_dir | Join-Path -ChildPath '*\*' | Convert-Path | ? { $_ -like '*.inf' } | % {
    
            $inf_file = $_

            # Set and Validate Architecture (x86 / x64)
            $os = $inf_file.Split([System.IO.Path]::DirectorySeparatorChar)[-2]
            if ($os -eq $null) { throw New-Object System.Management.Automation.JobFailedException }


            # Get and Validate CAT File Name
            $cat_file = ($_ | Split-Path -Parent | Join-Path -ChildPath (Get-PrivateProfileString -Path $inf_file -Section 'Version' -Key 'CatalogFile'))
            if (-not (Test-Path -Path $cat_file)) { throw New-Object System.Management.Automation.JobFailedException }

            # Getting Authenticode Information
            $signature = Get-AuthenticodeSignerName -FilePath $cat_file
            if ($signature -eq $null) {
                $signature = 'Not Signed'
            }
            else {
                $signature += "<br />(" + (Get-AuthenticodeTimeStamp -FilePath $cat_file) + ")"
            }


            # Append Version Information (per Driver)
            $version_drivers += @{
                'Printer'   = Get-PrivateProfileString -Path $inf_file -Section 'Strings' -Key 'PRINTER'
                'OS'        = $os
                'Date'      = (Get-PrivateProfileString -Path $inf_file -Section 'Version' -Key 'DriverVer').Split(',')[0].Trim()
                'Version'   = (Get-PrivateProfileString -Path $inf_file -Section 'Version' -Key 'DriverVer').Split(',')[1].Trim()
                'INF'       = $inf_file
                'CAT'       = $cat_file
                'Signature' = $signature
            }
        }

        # Append Version Information (per Language)
        $version_languages += @{
            'Language' = $lang
            'Drivers'  = $version_drivers
        }
    }

    # Append Version Information (per Model)
    $version_models += @{
        'Model'     = $model
        'Languages' = $version_languages
    }
}

# Output
$version_models | % {
    ""
    "[" + $_['Model'] + "]"
    $_['Languages'] | % {
        ""
        $_['Language'] + ":"
        $_['Drivers'] | % {

            $_['Printer'] + " (" + $_['OS'] + "/" + $_['INF'].Split('\')[-1] + ") : Version=" + $_['Version'] + ", Date=" + $_['Date']
        }
    }
}



# Getting Release Nots related information from INI File
$release_notes_src = $Settings['Source']['ReleaseNotes']
$release_notes_name = $Settings['Destination']['ReleaseNotes']

# Read HTML Release Notes Template
""
"Reading HTML Release Notes Template '$release_notes_src'..."

[string[]]$html_template = @()
[string[]]$html_template_model = @()
[string[]]$html_template_lang = @()
[string[]]$html_template_driver = @()

Get-Content -Path $release_notes_src | % {

    $html_line = ([string]$_).TrimStart('#')

    if ($_[0] -eq '#') {
        if ($_[1] -eq '#') {
            if ($_[2] -eq '#') { $html_template_driver += $html_line }
            else { $html_template_lang += $html_line}
        }
        else { $html_template_model += $html_line }
    }
    else { $html_template += $html_line }
}



# Generating HTML Version Information

# Container of Driver Versions (per Models)
[string[]]$html_models = @()

# for Models
$version_models | % {

    # Set Model    
    $model = $_['Model']

    [string[]]$html_model = @()
    [string[]]$html_languages = @()

    # Replacing MODEL
    $html_template_model | % { $html_model += ($_ -replace '__MODEL__', $model) }


    # for Languages
    $_['Languages'] | % {
        
        # Set Language
        $lang = $_['Language']

        [string[]]$html_lang = @()
        [string[]]$html_drivers = @()

        # Replacing LANGUAGE
        $html_template_lang | % { $html_lang += ($_ -replace '__LANGUAGE__', $lang) }


        # for Drivers
        $_['Drivers'] | % {

            # Append and Replace Driver Versions
            $html_drivers += $html_template_driver `
                -replace '__DRIVER__', $_['Printer'].Trim('"') `
                -replace '__OS__', $_['OS'] `
                -replace '__VERSION__', $_['Version'] `
                -replace '__DATE__', $_['Date'] `
                -replace '__SIGNATURE__', $_['Signature']
        }


        # Insert Driver Versions (per Languages)
        $html_languages += $html_lang -replace "__DRIVERVERSIONS__", $html_drivers
    }

    # Append Driver Versions (per Models)
    $html_models += $html_languages
}



# Write HTML Release Notes
""
"Writing HTML Release Notes '$release_notes_name'..."
$html_template `
    -replace '__DATE__', (New-DateString -LCID 'en-US') `
    -replace '__PROJECT__', $Project_Name `
    -replace '__PROJECTVERSION__', $Project_Version `
    -replace '__VERSIONS__', $html_models `
    | Out-File -FilePath (Join-Path -Path $Release_Dir -ChildPath $release_notes_name) -Force



# Get End Time and Elapsed Time
""
$end = Get-Date
$elapsed = New-TimeSpan -Start $start -End $end
"[END] " + $end.ToString("yyyy/MM/dd hh:mm:ss") + " (Elapsed Time: $elapsed)"
""
""
