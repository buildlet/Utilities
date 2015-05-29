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
            デジタル署名のタイムスタンプを取得します。

        .DESCRIPTION
            SignTool.exe (署名ツール) を使って、指定されたファイルのデジタル署名のタイムスタンプを文字列として取得します。
            コマンドラインは 'signtool verify /pa /v <filename(s)>' です。

        .PARAMETER FilePath
            タイムスタンプを取得するファイルのパスを指定します。

        .PARAMETER BinPath
            SignTool.exe ファイルへのパスを指定します。
            このパラメーターが省略された場合は $SignToolPath の値が使用されます。
            このパラメーターが省略され、$SignToolPath で示されたパスにも SignTool.exe が存在しない場合はエラーになります。

        .INPUTS
            System.String
            パイプを使用して、FilePath パラメーターを Get-AuthenticodeTimeStamp コマンドレットに渡すことができます。

        .OUTPUTS
            System.String
            デジタル署名のタイムスタンプを文字列として取得します。

        .NOTES
            このコマンドを実行する PC に、あらかじめ SignTool.exe がインストールされている必要があります。
            SignTool.exe は Windows Software Development Kit (Windows SDK) に含まれています。

        .EXAMPLE
            Get-AuthenticodeTimeStamp -FilePath D:\Setup.exe
            'D:\Setup.exe' のデジタル署名のタイムスタンプを取得します。

        .EXAMPLE
            'D:\Setup.exe', 'E:\Setup.exe' | Get-AuthenticodeTimeStamp
            'D:\Setup.exe' および 'E:\Setup.exe' のデジタル署名のタイムスタンプを取得します。
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
            SignTool.exe  (署名ツール) を実行します。

        .DESCRIPTION
            SignTool.exe  (署名ツール) を実行します。
            署名ツールはコマンド ライン ツールで、ファイルにデジタル署名を添付し、ファイルの署名を検証し、ファイルにタイム スタンプを付けます。

        .PARAMETER Command
            4 つのコマンド (catdb、sign、timestamp、または verify) のうちのいずれか 1 つを指定します。  

        .PARAMETER Options
            SignTool.exe へのオプションを指定します。

        .PARAMETER FilePath
            署名するファイルへのパスを指定します。

        .PARAMETER Retry
            SignTool.exe の終了コードが 0 以外だった場合にリトライする回数を指定します。
            既定の設定は 0 回です。

        .PARAMETER Interval
            リトライする間隔を秒数で指定します。既定の設定は 0 秒です。

        .PARAMETER PassThru
            標準出力ストリームへの出力結果を返します。既定では SignTool.exe の終了コードを返します。

        .PARAMETER WhatIf
            コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。

        .PARAMETER BinPath
            SignTool.exe ファイルへのパスを指定します。
            このパラメーターが省略された場合は $SignToolPath の値が使用されます。
            このパラメーターが省略され、$SignToolPath で示されたパスにも SignTool.exe が存在しない場合はエラーになります。

        .INPUTS 
            System.String
            パイプを使用して、FilePath パラメーターを Invoke-SignTool コマンドレットに渡すことができます。

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe の終了コードを返します。
            PassThru オプションが指定されたときは、標準出力ストリームへの出力結果を返します。

        .NOTES
            このコマンドを実行する PC に、あらかじめ SignTool.exe がインストールされている必要があります。
            SignTool.exe は Windows Software Development Kit (Windows SDK) に含まれています。

            既定の設定では、SignTool.exe の標準出力ストリームへの出力結果は標準エラーストリームへリダイレクトされます。
            この出力を抑えたい場合は、標準エラーストリームへの出力を $null へリダイレクト (3> $null) してください。

            タイムスタンプサーバーに $TimeStampServerURL (http://timestamp.verisign.com/scripts/timstamp.dll) を指定することができます。

        .EXAMPLE
            Invoke-SignTool -Command 'sign' -Options '/f C:\PFX\sign.pfx', '/p 12345678', '/t $TimeStampServerURL', '/v' -FilePath 'D:\Setup.exe', 'E:\Setup.exe' -PassThru -Retry 10 -Interval 3
            証明書 C:\PFX\sign.pfx と、パスワード 12345678 を使って 'D:\Setup.exe' および 'E:\Setup.exe' にコード署名をします。
            SignTool.exe の標準出力ストリームへの出力結果はコンソールに出力されます。
            また、タイムスタンプサーバーに $TimeStampServerURL を指定します。署名に失敗した場合は 3 秒間隔で10 回までリトライします。

        .LINK
            SignTool (Windows Drivers)
            https://msdn.microsoft.com/en-us/library/windows/hardware/ff551778.aspx

            SignTool.exe (署名ツール)
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
            ("WhatIf: 次のコマンドラインと同等のプロセスを実行します。" + $verbose_message)
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
            ドライバー パッケージ用のカタログ ファイルを作成します。

        .DESCRIPTION
            Inf2Cat.exe を使って、指定されたドライバー パッケージ用のカタログ ファイルを作成します。

        .PARAMETER PackagePath
            カタログ ファイルを作成するドライバー パッケージの INF ファイルが格納されているディレクトリのパスを指定します。

        .PARAMETER WindowsVersionList
            Inf2Cat.exe に /os: スイッチとともに渡す WindowsVersionList パラメーターを指定します。
            32 ビット、および 64 ビットのドライバー パッケージ用に、それぞれ $Inf2CatWindowsVersionList32 と $Inf2CatWindowsVersionList64 を
            指定することができます。

        .PARAMETER NoCatalogFiles
            Inf2Cat.exe の /nocat スイッチを指定します。

        .PARAMETER PassThru
            標準出力ストリームへの出力結果を返します。既定では SignTool.exe の終了コードを返します。

        .PARAMETER WhatIf
            コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。

        .PARAMETER BinPath
            Inf2Cat.exe ファイルへのパスを指定します。
            このパラメーターが省略された場合は $Inf2CatPath の値が使用されます。
            このパラメーターが省略され、$Inf2CatPath で示されたパスにも Inf2Cat.exe が存在しない場合はエラーになります。

        .INPUTS
            System.String
            パイプを使用して、PackagePath パラメーターを New-CatFile コマンドレットに渡すことができます。

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe の終了コードを返します。
            PassThru オプションが指定されたときは、標準出力ストリームへの出力結果を返します。

        .NOTES
            このコマンドを実行する PC に、あらかじめ Inf2Cat.exe がインストールされている必要があります。
            Inf2Cat.exe は Windows Driver Kit (WDK) に含まれています。

            既定の設定では、SignTool.exe の標準出力ストリームへの出力結果は標準エラーストリームへリダイレクトされます。
            この出力を抑えたい場合は、標準エラーストリームへの出力を $null へリダイレクト (3> $null) してください。

        .EXAMPLE
            New-CatFile -PackagePath 'D:\Drivers\x64' -WindowsVersionList $Inf2CatWindowsVersionList64
            ドライバー パッケージ 'D:\Drivers\x64' に対して未署名のカタログ ファイルを作成します。

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
            ("WhatIf: 次のコマンドラインと同等のプロセスを実行します。" + $verbose_message)
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
            Rock Ridge 属性付きハイブリッド ISO9660 / JOLIET / HFS ファイルシステムイメージを作成します。

        .DESCRIPTION
            Cygwin および genisoimage.exe を使って、ISO イメージ ファイルを作成します。

        .PARAMETER Path
            ISO9660 ファイルシステムにコピーするルートディレクトリのパスを指定します。
            genisoimage.exe の 'pathspec' パラメーターに相当します。

        .PARAMETER DestinationPath
            作成した ISO イメージ ファイルを保存するパスを指定します。
            指定したパスが存在しない場合は、エラーになります。既定の設定は、カレントディレクトリです。

        .PARAMETER FileName
            書き込まれる ISO9660 ファイルシステムイメージのファイル名を指定します。
            省略した場合の既定の設定は、$Path パラメーターに、拡張子 '.iso' を付加したファイル名が設定されます。

        .PARAMETER Options
            genisoimage.exe に渡すオプション パラメーターを指定します。
            ここで指定できるオプションの詳しい説明は、genisoimage あるいは mkisofs コマンドのヘルプを参照してください。

            また、任意のオプションを組み合わせた $GenIsoImageOptions が定義されているので、これを指定することもできます。
            $GenIsoImageOptions でどのようなオプションが指定されているかは、この変数の値を確認してください。

        .PARAMETER PassThru
            genisoimage.exe の標準エラーストリームへの出力結果を標準出力ストリームへリダイレクトします。
            既定では genisoimage.exe の終了コードを返します。

        .PARAMETER Force
            出力先のパスに既にファイルが存在する場合に、そのファイルを上書きします。
            または、出力先のパスに既にディレクトリが存在する場合に、そのディレクトリを削除してから、ISO イメージ ファイルを作成します。
            既定の設定では、出力先のパスに既にファイルまたはディレクトリが存在する場合は、エラーになります。

        .PARAMETER WhatIf
            コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。

        .PARAMETER BinPath
            genisoimage.exe ファイルへのパスを指定します。
            このパラメーターが省略された場合は $GenIsoImagePath の値が使用されます。
            このパラメーターが省略され、$GenIsoImagePath で示されたパスにも genisoimage.exe が存在しない場合はエラーになります。

        .INPUTS
            System.String
            パイプを使用して、Path パラメーターを New-IsoFile コマンドレットに渡すことができます。

        .OUTPUTS
            System.Int32, System.String
            SignTool.exe の終了コードを返します。
            PassThru オプションが指定されたときは、標準出力ストリームへの出力結果を返します。

        .NOTES
            このコマンドを実行する PC に、あらかじめ Cygwin および genisoimage.exe がインストールされている必要があります。

            既定の設定では、genisoimage.exe の標準出力ストリームへの出力結果は標準エラーストリームへリダイレクトされます。
            この出力を抑えたい場合は、標準エラーストリームへの出力を $null へリダイレクト (3> $null) してください。

        .EXAMPLE
            New-IsoFile -Path C:\Input -DestinationPath C:\Release -FileName 'hoge.iso' -Options $GenIsoImageOptions
            'C:\Input' をルートディレクトリとした ISO イメージ ファイルを、出力先のフォルダー 'C:\Release' に作成します。
            ファイル名には 'hoge.iso' で、genisoimage.exe へのオプションには $GenIsoImageOptions を指定しています。

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
            ("WhatIf: 次のコマンドラインと同等のプロセスを実行します。" + $verbose_message)
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
