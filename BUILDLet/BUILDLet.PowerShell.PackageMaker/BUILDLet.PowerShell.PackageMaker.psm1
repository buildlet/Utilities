#####################################################################################################################################################
##
##    BUILDLet.PowerShell.PackageMaker.psm1
##
#####################################################################################################################################################

#####################################################################################################################################################
##
##  [Variables]
##      $SignToolPath
##
##  [Functions]
##      Get-AuthenticodeTimeStamp
##      Invoke-SignTool
##      New-CatFile
##      New-IsoFile
##

#####################################################################################################################################################
[string]$SignToolPath    = 'C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe'
[string]$Inf2CatPath     = 'C:\Program Files (x86)\Windows Kits\8.1\bin\x86\Inf2Cat.exe'
[string]$GenIsoImagePath = 'C:\Cygwin\bin\genisoimage.exe'

[string]$TimeStampServerURL = 'http://timestamp.verisign.com/scripts/timstamp.dll'

[string]$Inf2CatWindowsVersionList32 = 'Vista_X86,7_X86,8_X86,6_3_X86,Server2008_X86'
[string]$Inf2CatWindowsVersionList64 = 'Vista_X64,7_X64,8_X64,6_3_X64,Server2008_X64,Server2008R2_X64,Server8_X64,Server6_3_X64'

[string[]]$GenIsoImageOptions = @(
    '-input-charset utf-8'   # Same as New-IsoImageFile function of PackageBuilder
    '-output-charset utf-8'  # Same as New-IsoImageFile function of PackageBuilder
    '-rational-rock'         # Same as New-IsoImageFile function of PackageBuilder
    '-joliet'                # Same as New-IsoImageFile function of PackageBuilder
    '-joliet-long'           # Same as New-IsoImageFile function of PackageBuilder
    '-jcharset utf-8'        # Same as New-IsoImageFile function of PackageBuilder
    '-pad'                   # Add (PackageMaker V1.0.7.0)
)

#####################################################################################################################################################
Function Get-AuthenticodeTimeStamp
{
    <#
        .SYNOPSIS
            �f�W�^�������̃^�C���X�^���v���擾���܂��B

        .DESCRIPTION
            SignTool.exe (�����c�[��) ���g���āA�w�肳�ꂽ�t�@�C���̃f�W�^�������̃^�C���X�^���v�𕶎���Ƃ��Ď擾���܂��B
            �R�}���h���C���� 'signtool verify /pa /v <filename(s)>' �ł��B

        .PARAMETER FilePath
            �^�C���X�^���v���擾����t�@�C���̃p�X���w�肵�܂��B

        .PARAMETER BinPath
            SignTool.exe �t�@�C���ւ̃p�X���w�肵�܂��B
            ���̃p�����[�^�[���ȗ����ꂽ�ꍇ�� $SignToolPath �̒l���g�p����܂��B
            ���̃p�����[�^�[���ȗ�����A$SignToolPath �Ŏ����ꂽ�p�X�ɂ� SignTool.exe �����݂��Ȃ��ꍇ�̓G���[�ɂȂ�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āAFilePath �p�����[�^�[�� Get-AuthenticodeTimeStamp �R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            �f�W�^�������̃^�C���X�^���v�𕶎���Ƃ��Ď擾���܂��B

        .NOTES
            ���̃R�}���h�����s���� PC �ɁA���炩���� SignTool.exe ���C���X�g�[������Ă���K�v������܂��B
            SignTool.exe �� Windows Software Development Kit (Windows SDK) �Ɋ܂܂�Ă��܂��B

        .EXAMPLE
            Get-AuthenticodeTimeStamp -FilePath D:\Setup.exe
            'D:\Setup.exe' �̃f�W�^�������̃^�C���X�^���v���擾���܂��B

        .EXAMPLE
            'D:\Setup.exe', 'E:\Setup.exe' | Get-AuthenticodeTimeStamp
            'D:\Setup.exe' ����� 'E:\Setup.exe' �̃f�W�^�������̃^�C���X�^���v���擾���܂��B
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)][string[]]$FilePath,
        [Parameter()][string]$BinPath
    )


    # Pre-Processing Tasks
    Begin {
        [string[]]$paths = @()

        # Set default value of parameters
        if (-not $BinPath) { $BinPath = $SignToolPath }
    }


    # Input Processing Tasks
    Process {
        $FilePath | ? { Test-Path -Path $_ -PathType Leaf } | % { $paths += $_ }
    }


    # Post-Processing Tasks
    End {

        # Invoke 'SignTool.exe' Process
        ($output = Invoke-SignTool -Command verify -Options '/pa','/v' -FilePath $paths -BinPath $BinPath -PassThru) | Write-Verbose

        # Validation (for Debug)
        if ($output -eq $null) { throw New-Object System.Management.Automation.ApplicationFailedException }


        [string[]]$result = @()
        $output | Select-String -Pattern ($pattern = 'The signature is timestamped: ') | % { $result += ([string]$_).Substring($pattern.Length) }

        # Validation (for Debug)
        $result | % {
            if ([string]::IsNullOrEmpty($_)) { throw New-Object System.Management.Automation.JobFailedException }
        }

        return $result
    }
}

#####################################################################################################################################################
Function Invoke-SignTool
{
    <#
        .SYNOPSIS
            SignTool.exe  (�����c�[��) �����s���܂��B

        .DESCRIPTION
            SignTool.exe  (�����c�[��) �����s���܂��B
            �����c�[���̓R�}���h ���C�� �c�[���ŁA�t�@�C���Ƀf�W�^��������Y�t���A�t�@�C���̏��������؂��A�t�@�C���Ƀ^�C�� �X�^���v��t���܂��B

        .PARAMETER Command
            4 �̃R�}���h (catdb�Asign�Atimestamp�A�܂��� verify) �̂����̂����ꂩ 1 ���w�肵�܂��B  

        .PARAMETER Options
            SignTool.exe �ւ̃I�v�V�������w�肵�܂��B

        .PARAMETER FilePath
            ��������t�@�C���ւ̃p�X���w�肵�܂��B

        .PARAMETER Retry
            SignTool.exe �̏I���R�[�h�� 0 �ȊO�������ꍇ�Ƀ��g���C����񐔂��w�肵�܂��B
            ����̐ݒ�� 0 ��ł��B

        .PARAMETER Interval
            ���g���C����Ԋu��b���Ŏw�肵�܂��B����̐ݒ�� 0 �b�ł��B

        .PARAMETER PassThru
            �W���o�̓X�g���[���ւ̏o�͌��ʂ�Ԃ��܂��B����ł� SignTool.exe �̏I���R�[�h��Ԃ��܂��B

        .PARAMETER WhatIf
            �R�}���h���b�g�����s����Ƃǂ̂悤�Ȍ��ʂɂȂ邩��\�����܂��B�R�}���h���b�g�͎��s����܂���B

        .PARAMETER BinPath
            SignTool.exe �t�@�C���ւ̃p�X���w�肵�܂��B
            ���̃p�����[�^�[���ȗ����ꂽ�ꍇ�� $SignToolPath �̒l���g�p����܂��B
            ���̃p�����[�^�[���ȗ�����A$SignToolPath �Ŏ����ꂽ�p�X�ɂ� SignTool.exe �����݂��Ȃ��ꍇ�̓G���[�ɂȂ�܂��B

        .INPUTS 
            System.String
            �p�C�v���g�p���āAFilePath �p�����[�^�[�� Invoke-SignTool �R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe �̏I���R�[�h��Ԃ��܂��B
            PassThru �I�v�V�������w�肳�ꂽ�Ƃ��́A�W���o�̓X�g���[���ւ̏o�͌��ʂ�Ԃ��܂��B

        .NOTES
            ���̃R�}���h�����s���� PC �ɁA���炩���� SignTool.exe ���C���X�g�[������Ă���K�v������܂��B
            SignTool.exe �� Windows Software Development Kit (Windows SDK) �Ɋ܂܂�Ă��܂��B

            ����̐ݒ�ł́ASignTool.exe �̕W���o�̓X�g���[���ւ̏o�͌��ʂ͕W���G���[�X�g���[���փ��_�C���N�g����܂��B
            ���̏o�͂�}�������ꍇ�́A�W���G���[�X�g���[���ւ̏o�͂� $null �փ��_�C���N�g (3> $null) ���Ă��������B

            �^�C���X�^���v�T�[�o�[�� $TimeStampServerURL (http://timestamp.verisign.com/scripts/timstamp.dll) ���w�肷�邱�Ƃ��ł��܂��B

        .EXAMPLE
            Invoke-SignTool -Command 'sign' -Options '/f C:\PFX\sign.pfx', '/p 12345678', '/t $TimeStampServerURL', '/v' -FilePath 'D:\Setup.exe', 'E:\Setup.exe' -PassThru -Retry 10 -Interval 3
            �ؖ��� C:\PFX\sign.pfx �ƁA�p�X���[�h 12345678 ���g���� 'D:\Setup.exe' ����� 'E:\Setup.exe' �ɃR�[�h���������܂��B
            SignTool.exe �̕W���o�̓X�g���[���ւ̏o�͌��ʂ̓R���\�[���ɏo�͂���܂��B
            �܂��A�^�C���X�^���v�T�[�o�[�� $TimeStampServerURL ���w�肵�܂��B�����Ɏ��s�����ꍇ�� 3 �b�Ԋu��10 ��܂Ń��g���C���܂��B

        .LINK
            SignTool (Windows Drivers)
            https://msdn.microsoft.com/en-us/library/windows/hardware/ff551778.aspx

            SignTool.exe (�����c�[��)
            https://msdn.microsoft.com/ja-jp/library/8s9b9yaz.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0)]
        [ValidateSet('catdb', 'sign', 'timestamp', 'verify')]
        [string]$Command,
        
        [Parameter(Mandatory=$true, Position=1)]
        [string[]]$Options,
        
        [Parameter(Mandatory=$true, Position=2, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            $_ | % {
                if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            }
            return $true
        })]
        [string[]]$FilePath,

        [Parameter()][int]$Retry = 0,
        [Parameter()][int]$Interval = 0,
        [Parameter()][switch]$PassThru,
        [Parameter()][switch]$WhatIf,

        [Parameter()]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf) `
                -or ((Split-Path $_ -Leaf).ToUpper() -ne 'SIGNTOOL.EXE')) {
                throw New-Object System.IO.FileNotFoundException -ArgumentList "'SignTool.exe' is not found."
            }
            return $true
        })]
        [string]$BinPath
    )


    # Pre-Processing Tasks
    Begin {

        [string[]]$paths = @()


        # Set Path to SignTool.exe
        if ($BinPath) { $signtool_path = $BinPath }
        else {
            if (-not (Test-Path -Path $SignToolPath -PathType Leaf) `
                -or ((Split-Path $SignToolPath -Leaf).ToUpper() -ne 'SIGNTOOL.EXE')) {
                throw New-Object System.IO.FileNotFoundException -ArgumentList `
                    ("'SignTool.exe' is not found. Please check the " + '$SignToolPath' + " variable ('$SignToolPath').")
            }
            else { $signtool_path = $SignToolPath }
        }
    }


    # Input Processing Tasks
    Process {
        $FilePath | ? { Test-Path -Path $_ -PathType Leaf } | % { $paths += $_ }
    }


    # Post-Processing Tasks
    End {

        # Validation
        if ($paths -eq $null) { throw New-Object System.IO.FileNotFoundException }

        # Construct <filename(s)> argument for signtool.exe
        [string]$filenames = [string]::Empty
        $paths | % { $filenames += ('"' + $_ + '" ') }
        [void]$filenames.Trim()

        # Construct ArgumentList
        [string[]]$arguments = @()
        $arguments += $Command
        $Options | % { $arguments += $_ }
        $arguments += $filenames



        # Construct Versbose Message
        $verbose_message = ('"' + $signtool_path + '"')
        $arguments | % { $verbose_message += (' ' + $_) }

        # WhatIf
        if ($WhatIf) {
            ("WhatIf: ���̃R�}���h���C���Ɠ����̃v���Z�X�����s���܂��B" + $verbose_message)
            return
        }

        # Verbose Output
        $verbose_message | Write-Verbose



        # Invoke 'SignTool.exe' Process
        if ($PassThru)
        {
            Invoke-Process -FilePath $signtool_path -ArgumentList $arguments -Retry $Retry -Interval $Interval -PassThru 4> $null
        }
        else
        {
            Invoke-Process -FilePath $signtool_path -ArgumentList $arguments -Retry $Retry -Interval $Interval -RedirectStandardOutputToWarning 4> $null
        }
    }
}

#####################################################################################################################################################
Function New-CatFile
{
    <#
        .SYNOPSIS
            �h���C�o�[ �p�b�P�[�W�p�̃J�^���O �t�@�C�����쐬���܂��B

        .DESCRIPTION
            Inf2Cat.exe ���g���āA�w�肳�ꂽ�h���C�o�[ �p�b�P�[�W�p�̃J�^���O �t�@�C�����쐬���܂��B

        .PARAMETER PackagePath
            �J�^���O �t�@�C�����쐬����h���C�o�[ �p�b�P�[�W�� INF �t�@�C�����i�[����Ă���f�B���N�g���̃p�X���w�肵�܂��B

        .PARAMETER WindowsVersionList
            Inf2Cat.exe �� /os: �X�C�b�`�ƂƂ��ɓn�� WindowsVersionList �p�����[�^�[���w�肵�܂��B
            32 �r�b�g�A����� 64 �r�b�g�̃h���C�o�[ �p�b�P�[�W�p�ɁA���ꂼ�� $Inf2CatWindowsVersionList32 �� $Inf2CatWindowsVersionList64 ��
            �w�肷�邱�Ƃ��ł��܂��B

        .PARAMETER NoCatalogFiles
            Inf2Cat.exe �� /nocat �X�C�b�`���w�肵�܂��B

        .PARAMETER PassThru
            �W���o�̓X�g���[���ւ̏o�͌��ʂ�Ԃ��܂��B����ł� SignTool.exe �̏I���R�[�h��Ԃ��܂��B

        .PARAMETER WhatIf
            �R�}���h���b�g�����s����Ƃǂ̂悤�Ȍ��ʂɂȂ邩��\�����܂��B�R�}���h���b�g�͎��s����܂���B

        .PARAMETER BinPath
            Inf2Cat.exe �t�@�C���ւ̃p�X���w�肵�܂��B
            ���̃p�����[�^�[���ȗ����ꂽ�ꍇ�� $Inf2CatPath �̒l���g�p����܂��B
            ���̃p�����[�^�[���ȗ�����A$Inf2CatPath �Ŏ����ꂽ�p�X�ɂ� Inf2Cat.exe �����݂��Ȃ��ꍇ�̓G���[�ɂȂ�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āAPackagePath �p�����[�^�[�� New-CatFile �R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe �̏I���R�[�h��Ԃ��܂��B
            PassThru �I�v�V�������w�肳�ꂽ�Ƃ��́A�W���o�̓X�g���[���ւ̏o�͌��ʂ�Ԃ��܂��B

        .NOTES
            ���̃R�}���h�����s���� PC �ɁA���炩���� Inf2Cat.exe ���C���X�g�[������Ă���K�v������܂��B
            Inf2Cat.exe �� Windows Driver Kit (WDK) �Ɋ܂܂�Ă��܂��B

            ����̐ݒ�ł́ASignTool.exe �̕W���o�̓X�g���[���ւ̏o�͌��ʂ͕W���G���[�X�g���[���փ��_�C���N�g����܂��B
            ���̏o�͂�}�������ꍇ�́A�W���G���[�X�g���[���ւ̏o�͂� $null �փ��_�C���N�g (3> $null) ���Ă��������B

        .EXAMPLE
            New-CatFile -PackagePath 'D:\Drivers\x64' -WindowsVersionList $Inf2CatWindowsVersionList64
            �h���C�o�[ �p�b�P�[�W 'D:\Drivers\x64' �ɑ΂��Ė������̃J�^���O �t�@�C�����쐬���܂��B

        .LINK
            Inf2Cat (Windows Drivers)
            https://msdn.microsoft.com/en-us/library/windows/hardware/ff547089.aspx
    #>


    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Container)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$PackagePath,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$WindowsVersionList,

        [Parameter()][switch]$NoCatalogFiles,

        [Parameter()][switch]$PassThru,
        [Parameter()][switch]$WhatIf,

        [Parameter()]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf) `
                -or ((Split-Path $_ -Leaf).ToUpper() -ne 'INF2CAT.EXE')) {
                throw New-Object System.IO.FileNotFoundException -ArgumentList "'Inf2Cat.exe' is not found."
            }
            return $true
        })]
        [string]$BinPath
    )


    Process {

        # Set Path to Inf2Cat.exe
        if ($BinPath) { $inf2cat_path = $BinPath }
        else {
            if (-not (Test-Path -Path $Inf2CatPath -PathType Leaf) `
                -or ((Split-Path $Inf2CatPath -Leaf).ToUpper() -ne 'INF2CAT.EXE')) {
                throw New-Object System.IO.FileNotFoundException -ArgumentList `
                    ("'Inf2Cat.exe' is not found. Please check the " + '$Inf2CatPath' + " variable ('$Inf2CatPath').")
            }
            else { $inf2cat_path = $Inf2CatPath }
        }

        # Construct arguments for 'Inf2Cat.exe'
        [string[]]$arguments = @()
        $arguments += "/driver:`"$PackagePath`""
        $arguments += "/os:$WindowsVersionList"
        if ($NoCatalogFiles) { $arguments += '/nocat' }
        if ($VerbosePreference -ne 'SilentlyContinue') { $arguments += '/verbose' }



        # Construct Versbose message
        $verbose_message = ('"' + $inf2cat_path + '"')
        $arguments | % { $verbose_message += (' ' + $_) }

        # WhatIf
        if ($WhatIf) {
            ("WhatIf: ���̃R�}���h���C���Ɠ����̃v���Z�X�����s���܂��B" + $verbose_message)
            return
        }

        # Verbose Output
        $verbose_message | Write-Verbose



        # Invoke 'Inf2Cat.exe' Process
        if ($PassThru) {
            Invoke-Process -FilePath $inf2cat_path -ArgumentList $arguments -PassThru 4> $null
        }
        else {
            Invoke-Process -FilePath $inf2cat_path -ArgumentList $arguments -RedirectStandardOutputToWarning 4> $null
        }
    }
}

#####################################################################################################################################################
Function New-IsoFile
{
    <#
        .SYNOPSIS
            Rock Ridge �����t���n�C�u���b�h ISO9660 / JOLIET / HFS �t�@�C���V�X�e���C���[�W���쐬���܂��B

        .DESCRIPTION
            Cygwin ����� genisoimage.exe ���g���āAISO �C���[�W �t�@�C�����쐬���܂��B

        .PARAMETER Path
            ISO9660 �t�@�C���V�X�e���ɃR�s�[���郋�[�g�f�B���N�g���̃p�X���w�肵�܂��B
            genisoimage.exe �� 'pathspec' �p�����[�^�[�ɑ������܂��B

        .PARAMETER DestinationPath
            �쐬���� ISO �C���[�W �t�@�C����ۑ�����p�X���w�肵�܂��B
            �w�肵���p�X�����݂��Ȃ��ꍇ�́A�G���[�ɂȂ�܂��B����̐ݒ�́A�J�����g�f�B���N�g���ł��B

        .PARAMETER FileName
            �������܂�� ISO9660 �t�@�C���V�X�e���C���[�W�̃t�@�C�������w�肵�܂��B
            �ȗ������ꍇ�̊���̐ݒ�́A$Path �p�����[�^�[�ɁA�g���q '.iso' ��t�������t�@�C�������ݒ肳��܂��B

        .PARAMETER Options
            genisoimage.exe �ɓn���I�v�V���� �p�����[�^�[���w�肵�܂��B
            �����Ŏw��ł���I�v�V�����̏ڂ��������́Agenisoimage ���邢�� mkisofs �R�}���h�̃w���v���Q�Ƃ��Ă��������B

            �܂��A�C�ӂ̃I�v�V������g�ݍ��킹�� $GenIsoImageOptions ����`����Ă���̂ŁA������w�肷�邱�Ƃ��ł��܂��B
            $GenIsoImageOptions �łǂ̂悤�ȃI�v�V�������w�肳��Ă��邩�́A���̕ϐ��̒l���m�F���Ă��������B

        .PARAMETER PassThru
            genisoimage.exe �̕W���G���[�X�g���[���ւ̏o�͌��ʂ�W���o�̓X�g���[���փ��_�C���N�g���܂��B
            ����ł� genisoimage.exe �̏I���R�[�h��Ԃ��܂��B

        .PARAMETER Force
            �o�͐�̃p�X�Ɋ��Ƀt�@�C�������݂���ꍇ�ɁA���̃t�@�C�����㏑�����܂��B
            �܂��́A�o�͐�̃p�X�Ɋ��Ƀf�B���N�g�������݂���ꍇ�ɁA���̃f�B���N�g�����폜���Ă���AISO �C���[�W �t�@�C�����쐬���܂��B
            ����̐ݒ�ł́A�o�͐�̃p�X�Ɋ��Ƀt�@�C���܂��̓f�B���N�g�������݂���ꍇ�́A�G���[�ɂȂ�܂��B

        .PARAMETER WhatIf
            �R�}���h���b�g�����s����Ƃǂ̂悤�Ȍ��ʂɂȂ邩��\�����܂��B�R�}���h���b�g�͎��s����܂���B

        .PARAMETER BinPath
            genisoimage.exe �t�@�C���ւ̃p�X���w�肵�܂��B
            ���̃p�����[�^�[���ȗ����ꂽ�ꍇ�� $GenIsoImagePath �̒l���g�p����܂��B
            ���̃p�����[�^�[���ȗ�����A$GenIsoImagePath �Ŏ����ꂽ�p�X�ɂ� genisoimage.exe �����݂��Ȃ��ꍇ�̓G���[�ɂȂ�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āAPath �p�����[�^�[�� New-IsoFile �R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe �̏I���R�[�h��Ԃ��܂��B
            PassThru �I�v�V�������w�肳�ꂽ�Ƃ��́A�W���o�̓X�g���[���ւ̏o�͌��ʂ�Ԃ��܂��B

        .NOTES
            ���̃R�}���h�����s���� PC �ɁA���炩���� Cygwin ����� genisoimage.exe ���C���X�g�[������Ă���K�v������܂��B

            ����̐ݒ�ł́Agenisoimage.exe �̕W���o�̓X�g���[���ւ̏o�͌��ʂ͕W���G���[�X�g���[���փ��_�C���N�g����܂��B
            ���̏o�͂�}�������ꍇ�́A�W���G���[�X�g���[���ւ̏o�͂� $null �փ��_�C���N�g (3> $null) ���Ă��������B

        .EXAMPLE
            New-IsoFile -Path C:\Input -DestinationPath C:\Release -FileName 'hoge.iso' -Options $GenIsoImageOptions
            'C:\Input' �����[�g�f�B���N�g���Ƃ��� ISO �C���[�W �t�@�C�����A�o�͐�̃t�H���_�[ 'C:\Release' �ɍ쐬���܂��B
            �t�@�C�����ɂ� 'hoge.iso' �ŁAgenisoimage.exe �ւ̃I�v�V�����ɂ� $GenIsoImageOptions ���w�肵�Ă��܂��B

        .LINK
            Cygwin
            http://www.cygwin.com/
    #>

    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Container)) { throw New-Object System.IO.DirectoryNotFoundException }
            return $true
        })]
        [string]$Path,

        [Parameter(Position=1)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_)) { throw New-Object System.IO.DirectoryNotFoundException }
            return $true
        })]
        [string]$DestinationPath,

        [Parameter(Position=2)][string]$FileName,
        [Parameter()][string[]]$Options,
        [Parameter()][string]$BinPath,
        [Parameter()][switch]$PassThru,
        [Parameter()][switch]$Force,
        [Parameter()][switch]$WhatIf
    )


    # Input Processing Tasks
    Process {

        # Set default value of parameters
        if (-not $DestinationPath) { $DestinationPath = Get-Location }
        if (-not $FileName) { $FileName = (Split-Path -Path $Path -Leaf) + '.iso' }
        if (-not $BinPath) { $BinPath = $GenIsoImagePath }


        # Validation ($target_path)
        if (Test-Path -Path ($output_path = Join-Path -Path $DestinationPath -ChildPath $FileName)) {
            if (-not $Force) { throw New-Object System.IO.IOException }
            else { Remove-Item $output_path -Force -Recurse }
        }

        # Validation ($BinPath)
        if (-not (Test-Path -Path $BinPath -PathType Leaf) -or ((Split-Path $BinPath -Leaf).ToUpper() -ne 'GENISOIMAGE.EXE')) {
            throw New-Object System.IO.FileNotFoundException -ArgumentList (
                "'genisoimage.exe' is not found." +
                " Check the parameter '`$BinPath' ('$BinPath'), or the variable '`$GenIsoImagePath' ('$GenIsoImagePath').")
        }



        # Construct ArgumentList
        [string[]]$arguments = @()
        $Options | ? { -not [string]::IsNullOrEmpty($_) } | % { $arguments += $_ }
        $arguments += ('-o "' + $output_path + '"')
        $arguments += ('"' + $Path + '"')



        # Construct Versbose Message
        $verbose_message = ('"' + $BinPath + '"')
        $arguments | % { $verbose_message += (' ' + $_) }

        # WhatIf
        if ($WhatIf) {
            ("WhatIf: ���̃R�}���h���C���Ɠ����̃v���Z�X�����s���܂��B" + $verbose_message)
            return
        }

        # Verbose Output
        $verbose_message | Write-Verbose



        # Invoke 'genisoimage.exe' Process
        if ($PassThru)
        {
            Invoke-Process -FilePath $BinPath -ArgumentList $arguments -PassThru -RedirectStandardErrorToOutput 4> $null
        }
        else
        {
            Invoke-Process -FilePath $BinPath -ArgumentList $arguments 4> $null
        }
    }
}

#####################################################################################################################################################
Export-ModuleMember -Variable *
Export-ModuleMember -Function *
