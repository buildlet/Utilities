<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

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

####################################################################################################
Function New-DateString {

    <#
        .SYNOPSIS
            �w�肵�������ɑ΂�����t���A�w�肵�������̕�����Ƃ��Ď擾���܂��B

        .DESCRIPTION
            �w�肵�������ɑ΂�����t�ɑ΂��āA���P�[�� ID (LCID) ����� �W��
			�܂��̓J�X�^���̓��������w�蕶������w�肵�āA������Ƃ��Ď擾���܂��B

        .PARAMETER Date
            �\��������t���w�肵�܂��B
            ����ł́A���̃R�}���h�����s���������ł��B

        .PARAMETER LCID
            ���P�[�� ID (LCID) ���w�肵�܂��B�ȗ������ꍇ�̊���̐ݒ�́A
			���݂̃J���`���[�� LCID �ł��B

        .PARAMETER Format
            �����w�蕶������w�肵�܂��B
            �ȗ������ꍇ�̊���̐ݒ�� 'D' �ł��B

        .INPUTS
            System.DateTime
            �p�C�v���g�p���āADate �p�����[�^�[�� Get-DateString �R�}���h���b�g��
			�n�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            ���t�������Ԃ��܂��B

        .EXAMPLE
            Get-DateString
            �����̓��t�𕶎���Ƃ��Ď擾���܂��B
            �����w�蕶����̓f�t�H���g�� 'D' �Ȃ̂ŁA���{�ł���� 'yyyy�NM��d��' �ɂȂ�܂��B

        .EXAMPLE 
            Get-DateString -Date 2014/4/29 -LCID en-US -Format m
            2014�N4��29�� (0:00) �ɑ΂�����t��������A���P�[�� ID 'en-US' 
			����я����w�蕶���� 'm' �̕�����Ƃ��Ď擾���܂��B

        .LINK
            [MS-LCID] Windows Language Code Identifier (LCID) Reference
            http://msdn.microsoft.com/en-us/library/cc233965.aspx

            ���P�[�� ID (LCID) �̈ꗗ
            http://msdn.microsoft.com/ja-jp/library/cc392381.aspx

            �W���̓��t�Ǝ����̏����w�蕶����
            http://msdn.microsoft.com/ja-jp/library/az4se3k1.aspx

            �J�X�^���̓��t�Ǝ����̏����w�蕶����
            http://msdn.microsoft.com/ja-jp/library/8kb3ddd4.aspx

            DateTime.ToString ���\�b�h (String, IFormatProvider) (System)
            http://msdn.microsoft.com/ja-jp/library/8tfzyc64.aspx

            CultureInfo �R���X�g���N�^�[ (String) (System.Globalization)
            http://msdn.microsoft.com/ja-jp/library/ky2chs3h.aspx

            ISO 639 - Wikipedia
            http://ja.wikipedia.org/wiki/ISO_639
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Position = 0, ValueFromPipeline = $true)]
		[System.DateTime]$Date = (Get-Date),

        [Parameter(Position = 1)]
		[string]$LCID = (Get-Culture).ToString(),

        [Parameter(Position = 2)]
		[string]$Format = 'D'
    )

    Process {
		return ($Date).ToString($Format, (New-Object System.Globalization.CultureInfo($LCID)))
    }
}

####################################################################################################
Function New-Directory {

    <#
        .SYNOPSIS
            �w�肳�ꂽ�p�X�Ƀf�B���N�g�����쐬���܂��B

        .DESCRIPTION
            �w�肳�ꂽ�p�X�Ƀf�B���N�g�����쐬���܂��B
            �w�肳�ꂽ�p�X�Ƀt�@�C���܂��̓f�B���N�g�������ɑ��݂���ꍇ�A����ł̓G���[�ɂȂ�܂��B
			�K�v�ɉ����āAForce �p�����[�^�[�� Clean �p�����[�^�[���w�肵�Ă��������B

        .PARAMETER Path
            �쐬����f�B���N�g���̃p�X���w�肵�܂��B

        .PARAMETER Force
            �f�B���N�g���̍쐬��폜�A����сA�t�@�C���̍폜�������I�ɍs���܂��B
            �������AForce �p�����[�^�[���g�p���Ă��A�R�}���h���b�g�̓Z�L�����e�B�������㏑���ł��܂���B

        .PARAMETER Clean
            �w�肳�ꂽ�p�X�ɁA�������O�̃t�@�C�������ɑ��݂���ꍇ�A���̃t�@�C�����폜���Ă���A
			�f�B���N�g�����쐬���܂��B�w�肳�ꂽ�p�X�ɁA�������O�̃f�B���N�g�������ɑ��݂���ꍇ�A
			�f�B���N�g���Ɋ܂܂��t�@�C���ƃf�B���N�g����S�č폜���܂��B

        .PARAMETER PassThru
            ���ڂ�\���I�u�W�F�N�g���p�C�v���C���ɓn���܂��B
			����ł́A���̃R�}���h���b�g�ɂ��o�͂͂���܂���B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� New-Directory �R�}���h���b�g��
			�n�����Ƃ��ł��܂��B

        .OUTPUTS
            None, System.IO.DirectoryInfo
            PassThru �p�����[�^�[���g�p����ƁASystem.IO.DirectoryInfo �I�u�W�F�N�g�𐶐����܂��B
            ����ȊO�̏ꍇ�A���̃R�}���h���b�g�ɂ��o�͂͂���܂���B

        .EXAMPLE
            New-Directory -Path .\Work -Force
            �J�����g�f�B���N�g���̒����ɂ��� 'Work' �t�H���_�[���쐬���܂��B
            ���Ƀt�H���_�[�܂��̓t�@�C�������݂���ꍇ�́A�������폜���Ă���A�t�H���_�[���쐬���܂��B
    #>

    [CmdletBinding(SupportsShouldProcess = $true)]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Path,

        [Parameter()]
		[switch]$Force,

        [Parameter()]
		[switch]$Clean,

        [Parameter()]
		[switch]$PassThru
    )

    Process {

		# Should Process
		if ($PSCmdlet.ShouldProcess($Path, "�f�B���N�g���̍쐬")) {


			# Delete directory and its content
			# (when "Clean" option is specified, and file or directory already exists).
			if ($Clean -and (Test-Path -Path $Path)) {

				# Directory already exists
				if (Test-Path $Path -PathType Container) {

					# Delete files in the directory
					Get-ChildItem -Path $Path -Force:$Force | % {
						if (Test-Path -Path $_.FullName) { Remove-Item -Path $_.FullName -Recurse -Force:$Force }
					}
				}
			}


			# Create directory
			$output = New-Item -Path $Path -ItemType 'Directory' `
				-Force:$Force `
				-Verbose:($VerbosePreference -ne 'SilentlyContinue')


			# Output
			if ($PassThru) { return $output }
		}
    }
}

####################################################################################################
Function Get-FileVersionInfo {

    <#
        .SYNOPSIS
            �f�B�X�N��̕����t�@�C���̃o�[�W���������擾���܂��B

        .DESCRIPTION
            �w�肵���t�@�C���̃o�[�W�������� System.Diagnostics.FileVersionInfo �Ƃ��Ď擾���܂��B

        .PARAMETER Path
            �t�@�C���o�[�W�������擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.Diagnostics.FileVersionInfo
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� Get-FileVersionInfo 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.Diagnostics.FileVersionInfo
            Get-FileVersionInfo �R�}���h���b�g�� System.Diagnostics.FileVersionInfo ��Ԃ��܂��B

        .EXAMPLE
            Get-FileVersion -Path .\setup.exe
            �J�����g�f�B���N�g���ɂ��� setup.exe �̃o�[�W���������擾���܂��B

        .LINK
            FileVersionInfo �v���p�e�B (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/System.Diagnostics.FileVersionInfo.aspx    
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$Path
    )

    Process { return (Get-Item -Path $Path).VersionInfo }
}

####################################################################################################
Function Get-FileVersion {

    <#
        .SYNOPSIS
            �f�B�X�N��̕����t�@�C���̃t�@�C���o�[�W�������擾���܂��B

        .DESCRIPTION
            �w�肵���t�@�C���̃t�@�C���o�[�W���� (System.Diagnostics.FileVersionInfo.FileVersion) ��
			������Ƃ��Ď擾���܂��B

        .PARAMETER Path
            �t�@�C���o�[�W�������擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� Get-FileVersion 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            Get-FileVersion �R�}���h���b�g�� System.String ��Ԃ��܂��B

        .LINK
            FileVersionInfo.FileVersion �v���p�e�B (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.fileversion.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$Path
    )

    Process {
        return (Get-Item -Path $Path).VersionInfo.FileVersion
    }
}

####################################################################################################
Function Get-ProductVersion {

    <#
        .SYNOPSIS
            �f�B�X�N��̕����t�@�C���̐��i�o�[�W�������擾���܂��B

        .DESCRIPTION
            �w�肵���t�@�C���̐��i�o�[�W���� (System.Diagnostics.FileVersionInfo.ProductVersion) ��
			������Ƃ��Ď擾���܂��B

        .PARAMETER Path
            ���i�o�[�W�������擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� Get-ProductVersion 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            Get-ProductVersion �R�}���h���b�g�� System.String ��Ԃ��܂��B

        .LINK
            FileVersionInfo.ProductVersion �v���p�e�B (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.productversion.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$Path
    )

    Process {
        return (Get-Item -Path $Path).VersionInfo.ProductVersion
    }
}

####################################################################################################
Function Get-ProductName {

    <#
        .SYNOPSIS
            �f�B�X�N��̕����t�@�C���̐��i�����擾���܂��B

        .DESCRIPTION
            �w�肵���t�@�C���̐��i�� (System.Diagnostics.FileVersionInfo) �𕶎���Ƃ��Ď擾���܂��B

        .PARAMETER Path
            ���i�����擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� Get-ProductName 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            Get-ProductName �R�}���h���b�g�� System.String ��Ԃ��܂��B

        .LINK
            FileVersionInfo.ProductName �v���p�e�B (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.productname.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$Path
    )

    Process {
        return (Get-Item -Path $Path).VersionInfo.ProductName
    }
}

####################################################################################################
Function Get-FileDescription {

    <#
        .SYNOPSIS
            �f�B�X�N��̕����t�@�C���̐������擾���܂��B

        .DESCRIPTION
            �w�肵���t�@�C���̐��� (System.Diagnostics.FileVersionInfo.FileDescription) ��
			������Ƃ��Ď擾���܂��B

        .PARAMETER Path
            �t�@�C���̐������擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (Path �p�����[�^�[) �� Get-FileDescription 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            Get-FileDescription �R�}���h���b�g�� System.String ��Ԃ��܂��B

        .LINK
            FileVersionInfo.FileDescription �v���p�e�B (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.filedescription.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$Path
    )

    Process {
        return (Get-Item -Path $Path).VersionInfo.FileDescription
    }
}

####################################################################################################
Function Get-AuthenticodeSignerName {

    <#
        .SYNOPSIS
            �f�W�^�������̏����Җ����擾���܂��B

        .DESCRIPTION
            Get-AuthenticodeSignerName �R�}���h���b�g�� Get-AuthenticodeSignature �R�}���h���b�g��
			�g�p���āA�w�肳�ꂽ�t�@�C���� Authenticode �R�[�h�����̏����Җ����擾���܂��B

        .PARAMETER FilePath
            �f�W�^�������̏����Җ����擾����t�@�C���̃p�X���w�肵�܂��B

        .INPUTS
            System.String
            �p�C�v���g�p���āA�t�@�C���̃p�X (FilePath �p�����[�^�[) �� Get-AuthenticodeSignerName 
			�R�}���h���b�g�ɓn�����Ƃ��ł��܂��B

        .OUTPUTS
            System.String
            Get-AuthenticodeSignerName �R�}���h���b�g�� System.String ��Ԃ��܂��B

        .LINK
            Get-AuthenticodeSignature
            https://technet.microsoft.com/library/36e5e640-2125-476e-98d9-495977315f14.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
        [string]$FilePath
    )

    Process {
        if (($cert = (Get-AuthenticodeSignature -FilePath $FilePath).SignerCertificate) -ne $null) {
            return $cert.Subject.`
				Split(@('O='), [System.StringSplitOptions]::RemoveEmptyEntries)[1].`
				Split(@(', '), [System.StringSplitOptions]::RemoveEmptyEntries)[0]
        }
    }
}

####################################################################################################
Export-ModuleMember -Function *
