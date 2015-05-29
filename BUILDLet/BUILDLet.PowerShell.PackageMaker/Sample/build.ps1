@"
BUILDLet PowerShell PackageMaker Toolkit Sample Script
Copyright (C) 2015 Daiki Sakamoto
"@

# Script Name
"[build.ps1] Version " + ($version = '1.0.7.0')


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


# Extract Package into Working Directory
""
$package_src = $Settings['Source']['Package']
"Extracting Package '$package_src'..."
$package_root = Expand-ZipFile -Path $package_src -DestinationPath $Work_Dir -Force -PassThru

#Validate
if (-not (Test-Path -Path $package_root)) { throw New-Object System.Management.Automation.JobFailedException }
"Package has been successfully extracted to '$package_root'."


# Update package root path
$package_root = Join-Path -Path $package_root -ChildPath (Split-Path -Path $package_root -Leaf)

#Validate
if (-not (Test-Path -Path $package_root)) { throw New-Object System.Management.Automation.JobFailedException }
"Package root path is updated to '$package_root'."



# Renew CAT File(s)
""
"Renewing CAT File(s)..."

# for Models
for ($i = 0; $i -lt $Driver_Dirs.Count; $i++) {

    ""
    ""
    "[" + $Model_Names[$i] + "]"

    # Set Driver Root Directory
    $driver_dir = $Driver_Dirs[$i]

    # for Directories
    $package_root | Join-Path -ChildPath $driver_dir | Join-Path -ChildPath '*\*' | Convert-Path | % {

        # x86 or x64
        ""
        $package_path = $_
        switch ($package_path.Split([System.IO.Path]::DirectorySeparatorChar)[-1])
        {
            # Renew CAT File(s)
            'x86' { $result = (New-CatFile -PackagePath $package_path -WindowsVersionList $Inf2CatWindowsVersionList32) }
            'x64' { $result = (New-CatFile -PackagePath $package_path -WindowsVersionList $Inf2CatWindowsVersionList64) }
        }

        # Validate
        if ($result -ne 0) { throw New-Object System.Management.Automation.ApplicationFailedException }
        "CAT File '$package_path' is successfully renewed."
    }
}
""


# Copy Readme File(s)
""
$readme_src = $Settings['Source']['README']
$readme_dir = Copy-Item -Path $readme_src -Destination $package_root -Force -PassThru
Get-ChildItem -Path $readme_src | Copy-Item -Destination $readme_dir -Recurse -Force
"Readme File(s) '$readme_dir' have been copied."


# Update Readme File(s)
$readme_dir | Join-Path -ChildPath '*\*' | Convert-Path | % {

    $readme_file = $_

    # Set Readme language
    $readme_lang = $Settings['Readme\LCID'][$readme_file.Split([System.IO.Path]::DirectorySeparatorChar)[-2]]

    # Validate
    if ($readme_lang -eq $null) { throw New-Object System.Management.Automation.JobFailedException }

    # Update Content of Readme File
    (Get-Content -Path $readme_file -Encoding UTF8) `
        -replace '__DATE__', (New-DateString -LCID $readme_lang) `
        -replace '__VERSION__', $version `
        | Out-File -FilePath $readme_file -Force
    "Readme File ($readme_lang) '$readme_file' has been updated."
}



# Copy AUTORUN.INF and Icon File
""
$autorun_file = Copy-Item -Path $Settings['Source']['AUTORUN'] -Destination $package_root -PassThru -Force
"AUTORUN.INF has been copied to '$autorun_file'."

$icon_file = Copy-Item -Path $Settings['Source']['Icon'] -Destination $package_root -PassThru -Force
"Icon File has been copied to '$icon_file'."



# Interrupt (before Moving)
[System.Threading.Thread]::Sleep(0)

# Move Package Root into Release Directory
""
$project_root = Move-Item -Path $package_root -Destination (Join-Path -Path $Release_Dir -ChildPath $Project_Dir_Name) -Force -PassThru
"Package root directory is moved from"
"'$package_root' (Source) to"
"'$project_root' (Destination)."



# Create ISO Image File
""
$iso_root = $project_root
$iso_filename = $Settings['Destination']['ISO']
$result = (New-IsoFile -Path $iso_root -DestinationPath $Release_Dir -FileName $iso_filename -Options $GenIsoImageOptions)

# Validate
if ($result -ne 0) { throw New-Object System.Management.Automation.ApplicationFailedException }
"ISO Image File '$iso_filename' is generated successfully."



# Get End Time and Elapsed Time
""
$end = Get-Date
$elapsed = New-TimeSpan -Start $start -End $end
"[END] " + $end.ToString("yyyy/MM/dd hh:mm:ss") + " (Elapsed Time: $elapsed)"
""
""
