BUILDLet Utilities Class Library and Tools
==========================================

Versin 2.2.1.0
--------------

概要
----

BUILDLet Utilities Class Library and Tools には .NET Framework 向けのクラスライブラリと、
そのフロントエンドが含まれます。


BUILDLet WOL
------------

指定した MAC アドレスのマジックパケットを送信するためのフロントエンドです。

空の設定ファイル WOL.conf を次の何れかのフォルダーに手動で作成しておくことで、
送信した MAC アドレスの履歴機能等を使用することができます。

  1. [Current Directory]
  2. [My Documents]\BUILDLet\                    (C:\Users\[User Name]\Documents\BUILDLet\)
  3. [Program Folder]\BUILDLet Utilities\        (C:\Program Files\BUILDLet Utilities\WOL\)
  4. [Program Folder (x86)]\BUILDLet Utilities\  (C:\Program Files (x86)\BUILDLet Utilities\WOL\)

この優先順位でフォルダーを検索します。  
フォルダーのアクセス権に注意してください。  
WOL.ini ファイルは、アンインストール時に自動で削除されないので、手動で削除してください。

BUILDLet WOL の本体は BUILDLet.Utilities.exe です。  
実行するためには、BUILDLet.Utilities.dll が必要です。


インストール方法
----------------

UtilitiesSetup.exe を実行してください。


アンインストール方法
--------------------

コントロールパネルから次のプログラムを選択してアンインストールを実行してください。

  1.  BUILDLet Utilities Class Library and Tools


動作環境
--------

次のソフトウェアがインストールされている必要があります。

  1. Microsoft .NET Framework 4.5

また、次の環境で動作を確認しています。

  1. Windows 10 Pro Version 1607 (x64)
  2. Windows 7 Ultimate Service Pack 1 (x64)


ライセンス
----------

このソフトウェアは MIT ライセンスの下で配布されます。
License.txt を参照してください。


変更履歴
--------

* Version 2.2.1.0 (2017/04/29)  
  BUILDLet Utilities PowerShell Module 2.2.1.0 のための内部リリース

* Version 2.2.0.0 (2017/04/23)  
  BUILDLet WOL を BUILDLet Utilities Class Library and Tools の一部として、再マージしました。  
  設定ファイルの検索パスの優先順位を変更しました。  
  設定ファイルの名前を WOL.conf から WOL.ini に変更しました。  
  設定ファイルのフォーマットを変更し、保存可能な設定値を増やしました。

* WOL Version 2.1.1.0 (April 2, 2017)  
  Base class library BUILDLet.Utilities is updated from Version 1.x to 2.1.x

* WOL Version 1.1.3.0 (July 20, 2015)  
  Minor Update  
  Icon image was a little changed.

* WOL Version 1.1.2.0 (June 15, 2015)  
  Minor Update
  
* WOL Version 1.1.1.0 (June 5, 2015)  
  Minor Update
  
* WOL Version 1.1.0.0 (June 3, 2015)  
  Minor Update  
  The "Launch" button was added to the final step of the installation wizard to launch WOL.exe.

* WOL Version 1.0.8.0 (May 29, 2015)  
  Minor Update
  
* WOL Version 1.0.7.0 (May 24, 2015)  
  Minor Update
  
* WOL Version 1.0.6 (March 1, 2015)  
  Add shourcut in Start Menu, and remove Desktop shortcut.
  
* WOL Version 1.0.5 (February 24, 2015)  
  Change executable file name.
  
* WOL Version 1.0.4 (February 15, 2015)  
  File path of private configuration file in "My Documents" folder was changed.

* WOL Version 1.0.3 (January 29, 2015)  
  MAC address input field was changed into ComboBox from TextBox.
  
* WOL Version 1.0.2 (January 26, 2015)  
  Modify Readme
  
* WOL Version 1.0.1 (January 25, 2015)  
  Modify Readme
  
* WOL Version 1.0.0 (January 25, 2015)  
  1st Release
