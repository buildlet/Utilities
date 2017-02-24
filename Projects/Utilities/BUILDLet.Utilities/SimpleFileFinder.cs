/*******************************************************************************
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
********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// ローカルドライブ上のファイルを探索する簡易的なメソッドを実装します。
    /// <para>
    /// このクラスは Version 1.4.0.0 で追加されました。
    /// </para>
    /// </summary>
    public class SimpleFileFinder
    {
        private List<string> folders = new List<string>(SimpleFileFinder.DefaultSearchPath.ToList<string>());


        /// <summary>
        /// <see cref="SimpleFileFinder"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        public SimpleFileFinder() { }


        /// <summary>
        /// 既定の探索パスを取得します。
        /// </summary>
        /// <remarks>
        /// 以下の順番で検索します。
        /// <para>
        ///     <list type="number">
        ///         <item>
        ///             <description>現在のフォルダー (カレント ディレクトリ)</description>
        ///         </item>
        ///         <item>
        ///             <description>マイ ドキュメント フォルダー</description>
        ///         </item>
        ///         <item>
        ///             <description>System32 フォルダー</description>
        ///         </item>
        ///         <item>
        ///             <description>Windows フォルダー</description>
        ///         </item>
        ///     </list>
        /// </para>
        /// <para>
        /// Version 1.4.0.0 で、Windows フォルダー よりも System32 フォルダー の優先順位が入れ替わりました。
        /// </para>
        /// </remarks>
        public static string[] DefaultSearchPath
        {
            get
            {
                return new string[] {
                    Environment.CurrentDirectory,
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows)
                };
            }
        }


        /// <summary>
        /// 探索パスを取得します。
        /// </summary>
        public List<string> SearchPath
        {
            get { return this.folders; }
        }


        /// <summary>
        /// 探索パスを文字列配列として取得します。
        /// </summary>
        /// <returns>探索パス</returns>
        public string[] GetSearchPath()
        {
            return this.folders.ToArray();
        }


        /// <summary>
        /// ローカルドライブからファイルを探索します。
        /// </summary>
        /// <param name="filename">探索するファイル名</param>
        /// <returns>
        /// 指定されたファイルが、探索パスのいずれかに存在する場合は、ファイルのパスを返します。
        /// ファイルが存在しない場合は null を返します。
        /// </returns>
        /// <remarks>
        /// ディレクトリーは検出されません。
        /// </remarks>
        public string[] Find(string filename)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(); }

            List<string> files = new List<string>();
            string filepath;

            foreach (var folder in this.folders)
            {
                if (File.Exists(filepath = Path.Combine(folder, filename)) && !Directory.Exists(filepath)) { files.Add(filepath); }
            }

#if DEBUG
            DebugInfo.Init();

            // Debug Print
            Debug.WriteLine("");
            for (int i = 0; i < this.SearchPath.Count; i++)
            {
                Debug.WriteLine("[{0}] SearchPath[{1}]=\"{2}\"", DebugInfo.ShortName, i, this.folders[i]);
            }
            Debug.WriteLine("");

            if (files.Count > 0)
            {
                Debug.WriteLine("[{0}] {1} of \"{2}\" is found in the following path.", DebugInfo.ShortName, files.Count, filename);
                for (int i = 0; i < files.Count; i++)
                {
                    Debug.WriteLine("[{0}] {1}. \"{3}\"", DebugInfo.ShortName, i+1, i, files[i]);
                }
            }
            else
            {
                Debug.WriteLine("[{0}] \"{1}\" is NOT found.", DebugInfo.ShortName, filename);
            }
            Debug.WriteLine("");
#endif

            return (files.Count > 0) ? files.ToArray() : null;
        }
    }
}
