#####################################################################################################################################################
##
##    BUILDLet.Utilities.PowerShell.psm1
##
#####################################################################################################################################################

#####################################################################################################################################################
##
##  [Variables]
##      $VerbosePromptLength
##
##  [Functions]
##      New-HR
##      New-GUID
##      New-DateString
##      Reset-Directory
##      Get-FileVersionInfo
##      Get-ProductName
##      Get-FileDescription
##      Get-FileVersion
##      Get-ProductVersion
##      Get-AuthenticodeSignerName
##
#####################################################################################################################################################

<###############################################################################
 The MIT License (MIT)

 Copyright (c) 2015 Daiki Sakamoto

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

[int]$VerbosePromptLength = . {
    if ((Get-Host).CurrentCulture.Name -eq 'ja-JP') { return ([System.Text.Encoding]::Unicode.GetByteCount('詳細') + ': '.Length) }
    else { return 'VERBOSE: '.Length }
}

#####################################################################################################################################################
Function New-HR
{
    <#
        .SYNOPSIS
            水平線を出力します。

        .DESCRIPTION
            指定された文字で構成される水平線を出力します。

        .PARAMETER Character
            水平線を構成する System.Char 型の文字を指定します。
            既定では '-' です。

        .PARAMETER Length
            水平線の幅を指定します。
            既定では [コンソールのウィンドウの幅 - 1] です。

        .PARAMETER Offset
            指定した分だけ水平線の幅を短くします。既定では 0 です。
            コンソールウインドウの幅からプロンプト文字列の長さ分だけ短くしたい場合などに指定します。

        .INPUTS
            None
            パイプを使用してこのコマンドレットに入力を渡すことはできません。

        .OUTPUTS
            System.String
            水平線を文字列として取得します。

        .NOTES
            New-HR コマンドレットから取得するのは文字列なので、このコマンドのみを実行した場合、水平線は標準出力に表示されます。

        .EXAMPLE
            New-HR | Write-Host -ForegroundColor Red
            赤い色の水平線を、コンソールに出力します。

        .LINK
            PSHostRawUserInterface.BufferSize Property (System.Management.Automation.Host)
            http://msdn.microsoft.com/en-us/library/system.management.automation.host.pshostrawuserinterface.buffersize.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$false, Position=0)][char]$Character = '-',
        [Parameter(Mandatory=$false, Position=1)][int]$Length = (Get-Host).UI.RawUI.BufferSize.Width - 1,
        [Parameter(Mandatory=$false)][int]$Offset = 0
    )

    Process {
        return ("$Character" * ($Length - $Offset))
    }
}

#####################################################################################################################################################
Function New-GUID
{
    <#
        .SYNOPSIS
            GUID を生成します。

        .DESCRIPTION
            System.Guid.NewGUID メソッドを使って新しい GUID を生成します。

        .PARAMETER Upper
            大文字で GUID を生成します。既定では小文字です。

        .INPUTS
            None
            パイプを使用してこのコマンドレットに入力を渡すことはできません。

        .OUTPUTS
            System.String
            生成した GUID を文字列として返します。

        .EXAMPLE
            $guid = New-GUID
            GUID を生成し、String 型の文字列として変数 guid に格納します。

        .LINK
            Guid.NewGuid メソッド (System)
            http://msdn.microsoft.com/ja-jp/library/system.guid.newguid.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$false)][Switch]$Upper
    )

    Process {

        $guid = [guid]::NewGuid().ToString()

        if ($Upper) { $guid = $guid.ToUpper() }

        return $guid
    }
}

#####################################################################################################################################################
Function New-DateString
{
    <#
        .SYNOPSIS
            指定した時刻に対する日付を、指定した書式の文字列として取得します。

        .DESCRIPTION
            指定した時刻に対する日付に対して、ロケール ID (LCID) および 標準またはカスタムの日時書式指定文字列を指定して、文字列として取得します。

        .PARAMETER Date
            表示する日付を指定します。
            既定では、このコマンドを実行した当日です。

        .PARAMETER LCID
            ロケール ID (LCID) を指定します。省略した場合の既定の設定は、現在のカルチャーの LCID です。

        .PARAMETER Format
            書式指定文字列を指定します。
            省略した場合の既定の設定は 'D' です。

        .INPUTS
            System.DateTime
            パイプを使用して、Date パラメーターを Get-DateString コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            日付文字列を返します。

        .EXAMPLE
            Get-DateString
            今日の日付を文字列として取得します。
            書式指定文字列はデフォルトの 'D' なので、日本であれば 'yyyy年M月d日' になります。

        .EXAMPLE 
            Get-DateString -Date 2014/4/29 -LCID en-US -Format m
            2014年4月29日 (0:00) に対する日付文字列を、ロケール ID 'en-US' および書式指定文字列 'm' の文字列として取得します。

        .LINK
            [MS-LCID] Windows Language Code Identifier (LCID) Reference
            http://msdn.microsoft.com/en-us/library/cc233965.aspx

            ロケール ID (LCID) の一覧
            http://msdn.microsoft.com/ja-jp/library/cc392381.aspx

            標準の日付と時刻の書式指定文字列
            http://msdn.microsoft.com/ja-jp/library/az4se3k1.aspx

            カスタムの日付と時刻の書式指定文字列
            http://msdn.microsoft.com/ja-jp/library/8kb3ddd4.aspx

            DateTime.ToString メソッド (String, IFormatProvider) (System)
            http://msdn.microsoft.com/ja-jp/library/8tfzyc64.aspx

            CultureInfo コンストラクター (String) (System.Globalization)
            http://msdn.microsoft.com/ja-jp/library/ky2chs3h.aspx

            ISO 639 - Wikipedia
            http://ja.wikipedia.org/wiki/ISO_639
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$false, Position=0, ValueFromPipeline=$true)][System.DateTime]$Date = (Get-Date),
        [Parameter(Mandatory=$false, Position=1)][string]$LCID = (Get-Culture).ToString(),
        [Parameter(Mandatory=$false, Position=2)][string]$Format = 'D'
    )

    Process {
        ($Date).ToString($Format, (New-Object System.Globalization.CultureInfo($LCID)))
    }
}

#####################################################################################################################################################
Function Reset-Directory
{
    <#
        .SYNOPSIS
            指定されたパスにディレクトリを作成します。

        .DESCRIPTION
            指定されたパスにディレクトリまたはファイルが既に存在する場合は、それらを削除してからディレクトリを作成します。

        .PARAMETER Path
            作成するディレクトリのパスを指定します。

        .PARAMETER Force
            ディレクトリの作成や削除、および、ファイルの削除を強制的に行います。
            ただし、Force パラメーターを使用しても、コマンドレットはセキュリティ制限を上書きできません。

        .PARAMETER PassThru
            項目を表すオブジェクトをパイプラインに渡します。既定では、このコマンドレットによる出力はありません。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (Path パラメーター) を Reset-Directory コマンドレットに渡すことができます。

        .OUTPUTS
            None, System.IO.DirectoryInfo
            PassThru パラメーターを使用すると、System.IO.DirectoryInfo オブジェクトを生成します。
            それ以外の場合、このコマンドレットによる出力はありません。

        .EXAMPLE
            Reset-Directory -Path .\Work -Force
            カレントディレクトリの直下にある 'Work' フォルダーを作成します。
            既にフォルダーまたはファイルが存在する場合は、それらを削除してから、フォルダーを作成します。
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)][string]$Path,
        [Parameter(Mandatory=$false)][switch]$Force,
        [Parameter(Mandatory=$false)][switch]$PassThru
    )

    Process {

        if (Test-Path -Path $Path) {

            if ((Get-Item -Path $Path) -is [System.IO.DirectoryInfo]) {

                # Directory already exists.

                # Delete files in the directory
                Get-ChildItem -Path $Path | % {
                    if ($Force) { $Path | Join-Path -ChildPath $_ | Remove-Item -Recurse -Force }
                    else { $Path | Join-Path -ChildPath $_ | Remove-Item -Recurse }
                }
            }
            else {

                # File already exists.

                # Delete the file
                if ($Force) { Remove-Item $Path -Force }
                else { Remove-Item $Path }
            }
        }

        # Create directory
        $output = New-Item -Path $Path -ItemType 'Directory' -Force

        # Output
        if ($PassThru) { return $output }
    }
}

#####################################################################################################################################################
Function Get-FileVersionInfo
{
    <#
        .SYNOPSIS
            ディスク上の物理ファイルのバージョン情報を取得します。

        .DESCRIPTION
            指定したファイルのバージョン情報を System.Diagnostics.FileVersionInfo として取得します。

        .PARAMETER FilePath
            ファイルバージョンを取得するファイルのパスを指定します。

        .INPUTS
            System.Diagnostics.FileVersionInfo
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-FileVersionInfo コマンドレットに渡すことができます。

        .OUTPUTS
            System.Diagnostics.FileVersionInfo
            Get-FileVersionInfo コマンドレットは System.Diagnostics.FileVersionInfo を返します。

        .EXAMPLE
            Get-FileVersion -FilePath .\setup.exe
            カレントディレクトリにある setup.exe のバージョン情報を取得します。

        .LINK
            FileVersionInfo プロパティ (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/System.Diagnostics.FileVersionInfo.aspx    
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        return (Get-Item -Path $FilePath).VersionInfo
    }
}

#####################################################################################################################################################
Function Get-ProductName
{
    <#
        .SYNOPSIS
            ディスク上の物理ファイルの製品名を取得します。

        .DESCRIPTION
            指定したファイルの製品名 (System.Diagnostics.FileVersionInfo) を文字列として取得します。

        .PARAMETER FilePath
            製品名を取得するファイルのパスを指定します。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-ProductName コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            Get-ProductName コマンドレットは System.String を返します。

        .LINK
            FileVersionInfo.ProductName プロパティ (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.productname.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        return (Get-Item -Path $FilePath).VersionInfo.ProductName
    }
}

#####################################################################################################################################################
Function Get-FileDescription
{
    <#
        .SYNOPSIS
            ディスク上の物理ファイルの説明を取得します。

        .DESCRIPTION
            指定したファイルの説明 (System.Diagnostics.FileVersionInfo.FileDescription) を文字列として取得します。

        .PARAMETER FilePath
            ファイルの説明を取得するファイルのパスを指定します。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-FileDescription コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            Get-FileDescription コマンドレットは System.String を返します。

        .LINK
            FileVersionInfo.FileDescription プロパティ (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.filedescription.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        return (Get-Item -Path $FilePath).VersionInfo.FileDescription
    }
}


#####################################################################################################################################################
Function Get-FileVersion
{
    <#
        .SYNOPSIS
            ディスク上の物理ファイルのファイルバージョンを取得します。

        .DESCRIPTION
            指定したファイルのファイルバージョン (System.Diagnostics.FileVersionInfo.FileVersion) を文字列として取得します。

        .PARAMETER FilePath
            ファイルバージョンを取得するファイルのパスを指定します。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-FileVersion コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            Get-FileVersion コマンドレットは System.String を返します。

        .LINK
            FileVersionInfo.FileVersion プロパティ (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.fileversion.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        return (Get-Item -Path $FilePath).VersionInfo.FileVersion
    }
}

#####################################################################################################################################################
Function Get-ProductVersion
{
    <#
        .SYNOPSIS
            ディスク上の物理ファイルの製品バージョンを取得します。

        .DESCRIPTION
            指定したファイルの製品バージョン (System.Diagnostics.FileVersionInfo.ProductVersion) を文字列として取得します。

        .PARAMETER FilePath
            製品バージョンを取得するファイルのパスを指定します。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-ProductVersion コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            Get-ProductVersion コマンドレットは System.String を返します。

        .LINK
            FileVersionInfo.ProductVersion プロパティ (System.Diagnostics)
            http://msdn.microsoft.com/ja-jp/library/system.diagnostics.fileversioninfo.productversion.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        return (Get-Item -Path $FilePath).VersionInfo.ProductVersion
    }
}

#####################################################################################################################################################
Function Get-AuthenticodeSignerName
{
    <#
        .SYNOPSIS
            デジタル署名の署名者名を取得します。

        .DESCRIPTION
            Get-AuthenticodeSignerName コマンドレットは Get-AuthenticodeSignature コマンドレットを使用して、
            指定されたファイルの Authenticode コード署名の署名者名を取得します。

        .PARAMETER FilePath
            デジタル署名の署名者名を取得するファイルのパスを指定します。

        .INPUTS
            System.String
            パイプを使用して、ファイルのパス (FilePath パラメーター) を Get-AuthenticodeSignerName コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            Get-AuthenticodeSignerName コマンドレットは System.String を返します。

        .LINK
            Get-AuthenticodeSignature
            https://technet.microsoft.com/library/36e5e640-2125-476e-98d9-495977315f14.aspx
    #>

    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, Position=0, ValueFromPipeline=$true)]
        [ValidateScript ({ 
            if (-not (Test-Path -Path $_ -PathType Leaf)) { throw New-Object System.IO.FileNotFoundException }
            return $true
        })]
        [string]$FilePath
    )

    Process {
        if (($sign = (Get-AuthenticodeSignature -FilePath $FilePath).SignerCertificate) -ne $null) {
            return $sign.Subject.Split(('CN=', ', '), [System.StringSplitOptions]::RemoveEmptyEntries)[0]
        }
    }
}

#####################################################################################################################################################
Export-ModuleMember -Variable *
Export-ModuleMember -Function *
