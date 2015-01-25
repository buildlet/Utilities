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
    public class FileAccess
    {
        /// <summary>
        /// <see cref="FileAccess"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        protected FileAccess() { }

        /// <summary>
        /// カレントディレクトリを除いたローカルハードディスク上のファイルを検索する既定のサーチパスを表します。
        /// </summary>
        /// <remarks>
        /// 既定では、下記の順番でフォルダーを検索します。
        /// <para>
        ///     <list type="number">
        ///         <item><description>マイ ドキュメント フォルダー</description></item>
        ///         <item><description>Windows フォルダー</description></item>
        ///         <item><description>System32 フォルダー</description></item>
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
        /// 取得されるフォルダーの順番は、カレントディレクトリの次に既定のサーチパス (<see cref="FileAccess.SearchPath"/>) を追加したものになります。
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
        /// 取得されるフォルダーの順番は、directory パラメーターの次に既定のサーチパス (<see cref="FileAccess.SearchPath"/>) を追加したものになります。
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
        /// 取得されるフォルダーの順番は、directories パラメーターの次に既定のサーチパス (<see cref="FileAccess.SearchPath"/>) を追加したものになります。
        /// </remarks>
        public static string[] GetSearchPath(string[] directries)
        {
            List<string> folders = new List<string>();

            foreach (var dir in directries)
            {
                if (Directory.Exists(dir))
                {
                    bool found = true;
                    foreach (var search in FileAccess.SearchPath)
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

            folders.AddRange(FileAccess.SearchPath);

            return folders.ToArray();
        }

        /// <summary>
        /// 指定されたファイルを、カレントディレクトリー、および、既定のサーチパス (<see cref="FileAccess.SearchPath"/>) から検索します。
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
                return FileAccess.GetFilePath(filename, FileAccess.GetSearchPath());
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
    }
}
