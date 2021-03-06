#
# モジュール 'BUILDLet.Utilities.PowerShell' のモジュール マニフェスト
#
# 生成者: Daiki Sakamoto
#
# 生成日: 2015/03/02
#

@{

# このマニフェストに関連付けられているスクリプト モジュール ファイルまたはバイナリ モジュール ファイル。
# RootModule = ''

# このモジュールのバージョン番号です。
ModuleVersion = '2.2.1.0'

# このモジュールを一意に識別するために使用される ID
GUID = '58bbd29a-c79a-48ab-a039-e0b5d07abbda'

# このモジュールの作成者
Author = 'Daiki Sakamoto'

# このモジュールの会社またはベンダー
CompanyName = 'BUILDLet'

# このモジュールの著作権情報
Copyright = '(c) 2015-2017 Daiki Sakamoto. All rights reserved.'

# このモジュールの機能の説明
Description = 'BUILDLet Utility PowerShell Module'

# このモジュールに必要な Windows PowerShell エンジンの最小バージョン
PowerShellVersion = '4.0'

# このモジュールに必要な Windows PowerShell ホストの名前
# PowerShellHostName = ''

# このモジュールに必要な Windows PowerShell ホストの最小バージョン
# PowerShellHostVersion = ''

# このモジュールに必要な Microsoft .NET Framework の最小バージョン
DotNetFrameworkVersion = '4.5'

# このモジュールに必要な共通言語ランタイム (CLR) の最小バージョン
CLRVersion = '4.0'

# このモジュールに必要なプロセッサ アーキテクチャ (なし、X86、Amd64)
# ProcessorArchitecture = ''

# このモジュールをインポートする前にグローバル環境にインポートされている必要があるモジュール
# RequiredModules = @()

# このモジュールをインポートする前に読み込まれている必要があるアセンブリ
RequiredAssemblies = @(
    'BUILDLet.Utilities.PInvoke.dll'
    'BUILDLet.Utilities.dll'
	'Ionic.Zip.dll'
)

# このモジュールをインポートする前に呼び出し元の環境で実行されるスクリプト ファイル (.ps1)。
# ScriptsToProcess = @()

# このモジュールをインポートするときに読み込まれる型ファイル (.ps1xml)
# TypesToProcess = @()

# このモジュールをインポートするときに読み込まれる書式ファイル (.ps1xml)
# FormatsToProcess = @()

# RootModule/ModuleToProcess に指定されているモジュールの入れ子になったモジュールとしてインポートするモジュール
NestedModules = @(
    'BUILDLet.Utilities.PowerShell.psm1'
    'BUILDLet.Utilities.PowerShell.dll'
)

# このモジュールからエクスポートする関数
FunctionsToExport = '*'

# このモジュールからエクスポートするコマンドレット
CmdletsToExport = '*'

# このモジュールからエクスポートする変数
VariablesToExport = '*'

# このモジュールからエクスポートするエイリアス
AliasesToExport = '*'

# このモジュールに同梱されているすべてのモジュールのリスト
# ModuleList = @()

# このモジュールに同梱されているすべてのファイルのリスト
# FileList = @()

# RootModule/ModuleToProcess に指定されているモジュールに渡すプライベート データ
# PrivateData = ''

# このモジュールの HelpInfo URI
# HelpInfoURI = ''

# このモジュールからエクスポートされたコマンドの既定のプレフィックス。既定のプレフィックスをオーバーライドする場合は、Import-Module -Prefix を使用します。
# DefaultCommandPrefix = ''

}
