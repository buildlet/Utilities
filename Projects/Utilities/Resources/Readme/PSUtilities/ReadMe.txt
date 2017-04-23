BUILDLet Utilities PowerShell Module
====================================

Version 2.2.0.0
---------------

概要
----

BUILDLet Utilities PowerShell Module は、さまざまなユーティリティーを含む PowerShell モジュールです。


インストール方法
----------------

PSUtilitiesSetup.exe を実行してください。


アンインストール方法
--------------------

コントロールパネルから次のプログラムを選択してアンインストールを実行してください。

  1. BUILDLet Utilities PowerShell Module


動作環境
--------

次のソフトウェアがインストールされている必要があります。

  1. Windows Management Framework 4.0 (Windows PowerShell 4.0)
  2. Microsoft .NET Framework 4.5


Windows 10 Pro Version 1607 (x64) および Windows 7 Ultimate Service Pack 1 (x64) で動作を確認しています。


使用準備
--------

次の PowerShell コマンドを実行してモジュールをインポートしてください。

    Import-Module BUILDLet.Utilities.PowerShell

BUILDLet.Utilities.PowerShell は、
32ビットOSの場合は %ProgramFiles%\WindowsPowerShell\Modules にインストールされます。
64ビットOSの場合は %ProgramFiles%\WindowsPowerShell\Modules および 
%ProgramFiles(x86)%\WindowsPowerShell\Modules の両方にインストールされます。
これらのパスは $env:PSModulePath に含まれているので、このコマンドを入力すれば、
モジュールをインポートすることができます。

インポートできないときは、次のコマンドを実行して BUILDLet.Utilities.PowerShell が
表示されることを確認してください。

    Get-Module -ListAvailable


使用方法
--------

BUILDLet.Utilities.PowerShell をインポートすると、次のコマンド (Function または Cmdlet) 
がインポートされます。詳細は各コマンドのヘルプを参照してください。  
一部のコマンドは、コマンドのみを入力するとコマンドの概要が表示されます。

  1. Expand-ZipFile (Cmdlet)  
     zip ファイルを解凍します。

  2. New-ZipFile (Cmdlet)  
     zip ファイルを作成します。

  3. New-BinaryFile (Cmdlet)  
     ランダムな値のバイト配列を格納したバイナリファイルを作成します。

  4. Test-FileHash (Cmdlet)  
     ファイルのハッシュ値を比較することによって、ファイル内容が同一かどうかを確認します。

  5. Get-HtmlString (Cmdlet)  
     入力データから HTML 要素またはその属性の値を取得します。

  6. Get-PrivateProfileString (Cmdlet)  
     INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。

  7. Set-PrivateProfileString (Cmdlet)  
     INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。

  8. Invoke-Process (Cmdlet)  
     指定されたされたプロセスを開始します。

  9. Send-MagicPacket (Cmdlet)  
     指定された MAC アドレスのマジックパケットを送信します。

  10. Open-HtmlHelp (Cmdlet)  
     HTML ヘルプ ファイルを開きます。

  11. Close-HtmlHelp (Cmdlet)  
     開いている HTML ヘルプ ファイルを全て閉じます。

  12. Get-AuthenticodeSignerName (Function)  
     デジタル署名の署名者名を取得します。

  13. Get-FileDescription (Function)  
     ディスク上の物理ファイルの説明を取得します。

  14. Get-FileVersion (Function)  
     ディスク上の物理ファイルのファイルバージョンを取得します。

  15. Get-FileVersionInfo (Function)  
     ディスク上の物理ファイルのバージョン情報を取得します。

  16. Get-ProductName (Function)  
     ディスク上の物理ファイルの製品名を取得します。

  17. Get-ProductVersion (Function)  
     ディスク上の物理ファイルの製品バージョンを取得します。

  18. New-DateString (Function)  
     指定した時刻に対する日付を、指定した書式の文字列として取得します。

  19. New-Directory (Function)  
     指定されたパスにディレクトリを作成します。

  20. Get-ContentBlock (Function)  
     指定した検索パターンを含む開始行と終了行の間に含まれるテキスト ブロックを取得します。


ライセンス
----------

このソフトウェアは MIT ライセンスの下で配布されます。
License.txt を参照してください。


DotNetZip Library は Microsoft Public License (Ms-PL) の下で配布されます。  
DotNetZip Library のライセンスについては、下記 URL を参照してください。  
http://dotnetzip.codeplex.com/license

DotNetZip Library については、下記の URL を参照してください。  
http://dotnetzip.codeplex.com/

Microsoft Public License (Ms-PL) については、下記 URL を参照してください。  
https://msdn.microsoft.com/ja-jp/library/gg592960.aspx


変更履歴
--------

* Version 2.2.0.0 (2017/04/23)  
  ディレクトリのエントリーを含まない Source に対する Expand-ZipFile Function 
  の出力が正しくない不具合を修正しました。  
  また、Expand-ZipFile Function の既定の出力をルート エントリーとし、
  SuppressOutput パラメーターを追加しました。 

* Version 2.1.1.0 (2017/04/02)  
  (BUILDLet WOL Version 2.1.1.0 のための内部リリース)  

* Version 2.1.0.0 (2017/03/26)  
  (BUILDLet PackageMaker Tool Kit Version 2.1.0.0 のためのリリース)  
  Get-ContentBlock Function を追加しました。  
  Get-AuthenticodeSignerName Function の処理を修正しました。  
  Get-HtmlString Function の戻り値の Count が 1 の場合は、String 型を返すように処理を変更しました。

* Version 2.0.1.0 (2017/03/18)  
  (BUILDLet PackageMaker Tool Kit Version 2.0.1.0 のためのリリース)  
  New-Directory Function の再帰的削除に関する処理を変更しました。

* Version 2.0.0.0 (2017/01/08)  
  全ての Cmdlet および Function を刷新しました。
