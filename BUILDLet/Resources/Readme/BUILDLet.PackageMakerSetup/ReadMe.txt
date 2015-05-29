BUILDLet PackageMaker Toolkit  Version 1.0.7.0
==============================================

概要
----

ソフトウェアパッケージとそのISOイメージファイルを作成する PowerShell スクリプトを
作成するための PowerShell コマンド群です。
2つの PowerShell モジュールとソフトウェアパッケージ作成のためのサンプルが含まれます。

  1. BUILDLet PowerShell Utility Module      (BUILDLet.PowerShell.Utilities)
  2. BUILDLet PowerShell PackageMaker Module (BUILDLet.PowerShell.PackageMaker)


インストール方法
----------------

PackageMakerSetup.exe を実行してください。

インストールウィザードの終了画面で Launch ボタンをクリックすると、
この Readme が表示されます。


アンインストール方法
--------------------

コントロールパネルから下記のプログラムを選択してアンインストールを実行してください。

  1. BUILDLet PackageMaker Toolkit
  2. BUILDLet PowerShell Utility Module      (BUILDLet.PowerShell.Utilities)
  3. BUILDLet PowerShell PackageMaker Module (BUILDLet.PowerShell.PackageMaker)


動作環境
--------

BUILDLet.PowerShell.Utilities を実行するためには、下記のソフトウェアがインストール
されている必要があります。

  1. Windows Management Framework 4.0 (Windows PowerShell 4.0)
  2. Microsoft .NET Framework 4.5


BUILDLet.PowerShell.PackageMaker の全ての機能を使用するためには、
実行環境に下記のソフトウェアがインストールされている必要があります。
( [] で記載してあるプログラムが必要です。 )

  1. Windows Software Development Kit (SDK) for Windows 8.1  [SignTool.exe]
  2. Windows Driver Kit (WDK) for Windows 8.1  [Inf2Cat.exe]
  3. Cygwin  [genisoimage.exe]


Windows 7 Ultimate x64 で動作を確認しています。


使用準備
--------

下記の PowerShell コマンドを実行してモジュールをインポートしてください。

    Import-Module BUILDLet.PowerShell.PackageMaker

PowerShell モジュールは、32ビットOSの場合は %ProgramFiles%\WindowsPowerShell\Modules 
に保存されます。64ビットOSの場合は %ProgramFiles%\WindowsPowerShell\Modules および 
%ProgramFiles(x86)%\WindowsPowerShell\Modules にインストールされます。
これらのパスは $env:PSModulePath に含まれているので、上記のコマンドを入力するだけで
モジュールがインポートできす。インポートできないときは下記のコマンドを実行して 
BUILDLet.PowerShell.Utilities および BUILDLet.PowerShell.PackageMaker が表示される
ことを確認してください。

    Get-Module -ListAvailable


BUILDLet.PowerShell.PackageMaker をインポートするためには、BUILDLet.PowerShell.Utilities 
が事前にインポートされている必要があるため、BUILDLet.PowerShell.PackageMaker を
インポートすると BUILDLet.PowerShell.Utilities も自動的にインポートされます。


使用方法
--------

BUILDLet PackageMaker Toolkit には、下記の PowerShell コマンド (Function および Cmdlet) 、
変数、および、サンプルスクリプトを含むソフトウェアパッケージのサンプルが含まれます。


コマンド (Function または Cmdlet)
---------------------------------

BUILDLet.PowerShell.Utilities をインポートすると、
以下のコマンド (Function または Cmdlet) がインポートされます。
詳細は各コマンドのヘルプを参照してください。
( Cmdlet は、コマンドのみを入力するとコマンドの概要が表示されます。 )

* Expand-ZipFile (Cmdlet)
  zip ファイルを解凍します。

* New-ZipFile (Cmdlet)
  zip ファイルを作成します。

* Get-HashValue (Cmdlet)
  指定されたハッシュ アルゴリズムを使用して、入力データのハッシュ値を計算します。

* Get-HtmlString (Cmdlet)
  入力データから HTML 要素またはその属性の値を取得します。

* Get-PrivateProfileString (Cmdlet)
  INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。

* Set-PrivateProfileString (Cmdlet)
  INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。

* Invoke-Process (Cmdlet)
  指定されたされたプロセスを開始します。

* Send-MagicPacket (Cmdlet)
  指定された MAC アドレスのマジックパケットを送信します。

* Get-AuthenticodeSignerName (Function)
  デジタル署名の署名者名を取得します。

* Get-FileDescription (Function)
  ディスク上の物理ファイルの説明を取得します。

* Get-FileVersion (Function)
  ディスク上の物理ファイルのファイルバージョンを取得します。

* Get-FileVersionInfo (Function)
  ディスク上の物理ファイルのバージョン情報を取得します。

* Get-ProductName (Function)
  ディスク上の物理ファイルの製品名を取得します。

* Get-ProductVersion (Function)
  ディスク上の物理ファイルの製品バージョンを取得します。

* New-DateString (Function)
  指定した時刻に対する日付を、指定した書式の文字列として取得します。

* New-GUID (Function)
  GUID を生成します。

* New-HR (Function)
  水平線を出力します。

* Reset-Directory (Function)
  指定されたパスにディレクトリを作成します。


BUILDLet.PowerShell.PackageMaker をインポートすると、以下のコマンド (Function) がインポートされます。
詳細は各コマンドのヘルプを参照してください。

* Get-AuthenticodeTimeStamp (Function)
  デジタル署名のタイムスタンプを取得します。

* Invoke-SignTool (Function)
  SignTool.exe  (署名ツール) を実行します。

* New-CatFile (Function)
  ドライバー パッケージ用のカタログ ファイルを作成します。

* New-IsoFile (Function)
  Rock Ridge 属性付きハイブリッド ISO9660 / JOLIET / HFS ファイルシステムイメージを作成します。


変数
----

BUILDLet.PowerShell.Utilities をインポートすると、以下の変数がインポートされます。

* VerbosePromptLength 
  詳細メッセージの各行の接頭文字列長
  ( 日本語環境であれば、"詳細: " なので 6 です。 )


BUILDLet.PowerShell.PackageMaker をインポートすると、以下の変数がインポートされます。

* GenIsoImageOptions
  デフォルトで用意されている genisoimage.exe のオプションパラメーターを格納した文字列配列

* GenIsoImagePath
  デフォルトで用意されている genisoimage.exe のファイルパス

* Inf2CatPath
  デフォルトで用意されている Inf2Cat.exe のファイルパス

* Inf2CatWindowsVersionList32
  32 ビット OS 用にデフォルトで用意されている Inf2Cat.exe のオプションパラメーター文字列

* Inf2CatWindowsVersionList64
  64 ビット OS 用にデフォルトで用意されている Inf2Cat.exe のオプションパラメーター文字列

* SignToolPath
  デフォルトで用意されている SignTool.exe のファイルパス

* TimeStampServerURL
  デフォルトで用意されているタイムスタンプサーバーの URL


サンプルについて
----------------

PowerShell サンプルスクリプトを含むソフトウェアパッケージ作成のサンプルは、既定の設定では
32ビットOSの場合は %ProgramFiles%\BUILDLet PackageMaker Toolkit、
64ビットOSの場合は %ProgramFiles(x86)%\BUILDLet PackageMaker Toolkit にインストールされています。


ライセンス
----------
このソフトウェアは MTI ライセンスの下で配布されます。
LICENCE.txt を参照してください。


変更履歴
--------
* 2015/05/27    Version 1.0.7.0    初版
