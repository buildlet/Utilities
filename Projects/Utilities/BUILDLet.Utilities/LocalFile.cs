﻿/*******************************************************************************
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
********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// ローカルハードディスク上のファイルやフォルダーに関する処理を実装します。
    /// </summary>
    public class LocalFile
    {
        /// <summary>
        /// <see cref="LocalFile"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        protected LocalFile() { }

        /// <summary>
        /// カレントディレクトリを除いたローカルハードディスク上のファイルを検索する既定のサーチパスを表します。
        /// </summary>
        /// <remarks>
        /// 既定では、下記の順番でフォルダーを検索します。
        /// <para>
        ///     <list type="number">
        ///         <value_found><description>マイ ドキュメント フォルダー</description></value_found>
        ///         <value_found><description>Windows フォルダー</description></value_found>
        ///         <value_found><description>System32 フォルダー</description></value_found>
        ///     </list>
        /// </para>
        /// </remarks>
        public static string[] SearchPath
        {
            get
            {
                return new string[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    Environment.GetFolderPath(Environment.SpecialFolder.System)
                };
            }
        }

        /// <summary>
        /// サーチパスを取得します。
        /// </summary>
        /// <returns>サーチパスを取得します。</returns>
        /// <remarks>
        /// 取得されるフォルダーの順番は、カレントディレクトリの次に既定のサーチパス (<see cref="LocalFile.SearchPath"/>) を追加したものになります。
        /// </remarks>
        public static string[] GetSearchPath()
        {
            try { return GetSearchPath(Environment.CurrentDirectory); }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// サーチパスを取得します。
        /// </summary>
        /// <param name="directory">サーチパスの先頭に追加するディレクトリ</param>
        /// <returns>サーチパスを取得します。</returns>
        /// <remarks>
        /// 取得されるフォルダーの順番は、directory パラメーターの次に既定のサーチパス (<see cref="LocalFile.SearchPath"/>) を追加したものになります。
        /// </remarks>
        public static string[] GetSearchPath(string directory)
        {
            try { return GetSearchPath(new string[] { directory }); }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// サーチパスを取得します。
        /// </summary>
        /// <returns>サーチパスを取得します。</returns>
        /// <param name="directries">サーチパスの先頭に追加するディレクトリ</param>
        /// <remarks>
        /// 取得されるフォルダーの順番は、directories パラメーターの次に既定のサーチパス (<see cref="LocalFile.SearchPath"/>) を追加したものになります。
        /// </remarks>
        public static string[] GetSearchPath(string[] directries)
        {
            List<string> folders = new List<string>();

            foreach (var dir in directries)
            {
                if (Directory.Exists(dir))
                {
                    bool found = true;
                    foreach (var search in LocalFile.SearchPath)
                    {
                        if (Path.GetFullPath(dir).Equals(Path.GetFullPath(search), StringComparison.OrdinalIgnoreCase))
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found) { folders.Add(dir); }
                }
            }

            folders.AddRange(LocalFile.SearchPath);

            return folders.ToArray();
        }

        /// <summary>
        /// 指定されたファイルを、カレントディレクトリー、および、既定のサーチパス (<see cref="LocalFile.SearchPath"/>) から検索します。
        /// </summary>
        /// <param name="filename">検索するファイル名を指定します。</param>
        /// <returns>
        /// サーチパスに指定されたファイルが存在する場合は、ファイルのパスを返します。
        /// ファイルが存在しない場合は <see cref="String.Empty"/> を返します。
        /// </returns>
        public static string GetFilePath(string filename)
        {
            try
            {
                return LocalFile.GetFilePath(filename, LocalFile.GetSearchPath());
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// 指定されたファイルを指定したサーチパスから検索します。
        /// </summary>
        /// <param name="filename">検索するファイル名を指定します。</param>
        /// <param name="folders">サーチパスを指定します。</param>
        /// <returns>
        /// サーチパスに指定されたファイルが存在する場合は、ファイルのパスを返します。
        /// ファイルが存在しない場合は <see cref="String.Empty"/> を返します。
        /// </returns>
        public static string GetFilePath(string filename, string[] folders)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(); }


            string path;
            foreach (var folder in folders)
            {
                if (File.Exists(path = Path.Combine(folder, filename))) { return path; }
            }

            // File does not exist.
            return string.Empty;
        }


        /// <summary>
        /// 相対パスを絶対パスに変換します。
        /// </summary>
        /// <param name="path">変換する相対パス</param>
        /// <returns>変換された絶対パス</returns>
        /// <remarks>
        /// 指定されたパスが絶対パスの場合は、指定されたパスをそのまま返します。
        /// 指定されたパスが存在しないなど、変換に失敗した場合は <see cref="String.Empty"/> を返します。
        /// </remarks>
        public static string ConvertPath(string path)
        {
            try
            {
                if (!Path.IsPathRooted(path)) { path = Path.Combine(Environment.CurrentDirectory, path); }

                return Path.GetFullPath(path);
            }
            catch (Exception) { return string.Empty; }
        }

        /// <summary>
        /// 指定されたパス文字列からフォルダー名を取得します。
        /// </summary>
        /// <param name="path">パス文字列</param>
        /// <returns>フォルダー名</returns>
        public static string GetFolderName(string path)
        {
            return path.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
        }
    }
}
