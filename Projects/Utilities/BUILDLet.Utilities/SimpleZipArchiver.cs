/*******************************************************************************
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
using System.IO.Compression;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.Compression
{
    /// <summary>
    /// zip ファイルの作成、および解凍の静的メソッドを提供します。
    /// </summary>
    public class SimpleZipArchiver
    {
        /// <summary>
        /// <see cref="SimpleZipArchiver"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected SimpleZipArchiver() { }


        /// <summary>
        /// 指定したファイルあるいはディレクトリに含まれる全てのファイルを含む zip アーカイブを作成します。 
        /// </summary>
        /// <param name="source">アーカイブするディレクトリまたはファイルのパス</param>
        /// <param name="destination">zip ファイルを作成するフォルダーのパス。指定されたパスが存在しない場合は、エラーになります。</param>
        /// <param name="filename">作成する zip ファイルのファイル名</param>
        /// <param name="overwrite">
        /// 出力先 zip ファイルのパスにフォルダーまたはディレクトリが既に存在する場合、そのファイルまたはディレクトリを削除してから zip ファイルを作成します。
        /// 既定の設定では、出力先のパスにフォルダーまたはディレクトリが既に存在する場合はエラーになります。
        /// </param>
        /// <param name="compressionLevel">
        /// エントリの作成時に速度または圧縮の有効性を強調するかどうかを示す <see cref="System.IO.Compression.CompressionLevel"/> 列挙体の値
        /// </param>
        /// <param name="includeBaseDirectory">
        /// ディレクトリをアーカイブする場合、アーカイブのルートにあるディレクトリ名を含める場合に true を指定してください。
        /// 既定では、アーカイブのルートディレクトリを含みます。
        /// </param>
        /// <returns>作成した zip ファイルのパスを返します。</returns>
        /// <remarks>
        /// filename が指定されていない場合、作成する zip ファイルのファイル名は source から自動的に作成されます。
        /// source がディレクトリの場合は、source に拡張子 ".zip" が付加されたものが zip ファイル名になります。
        /// source がファイルの場合は、source ファイルの拡張子を ".zip" に置き換えたものが zip ファイル名になります。
        /// </remarks>
        public static string Zip(string source, string destination = "", string filename = "", bool overwrite = false,
            CompressionLevel compressionLevel = CompressionLevel.Fastest, bool includeBaseDirectory = true)
        {
            try
            {
                source = LocalFile.ConvertPath(source);

                // Validation (and convert) of source
                if (!Directory.Exists(source) && !File.Exists(source)) { throw new FileNotFoundException(); }

                // Validation of foldername
                if (filename.Contains(Path.DirectorySeparatorChar)) { throw new ArgumentException(); }

                // Set default value_found of destination (Current Directory), Validate or Convert
                if (string.IsNullOrEmpty(destination)) { destination = Environment.CurrentDirectory; }
                else if (!Directory.Exists(destination)) { throw new DirectoryNotFoundException(); }
                else { destination = LocalFile.ConvertPath(destination); }

                // Set default value_found of filename (Zip file name is automatically generated from source.)
                if (string.IsNullOrEmpty(filename))
                {
                    if (Directory.Exists(source)) { filename = LocalFile.GetFolderName(source) + ".zip"; }
                    else if (File.Exists(source)) { filename = Path.ChangeExtension(Path.GetFileName(source), ".zip"); }
                }



                // Set Zip file path
                string zipfile_path = Path.Combine(destination, filename);

                // Validation of Zip file path (check if Original or Renewed Destination File already exists.)
                if (Directory.Exists(zipfile_path))
                {
                    if (overwrite) { Directory.Delete(zipfile_path, true); }
                    else { throw new IOException(); }
                }
                else if (File.Exists(destination))
                {
                    if (overwrite) { File.Delete(zipfile_path); }
                    else { throw new IOException(); }
                }



                // Source (Directory or File)
                if (Directory.Exists(source))
                {
                    // Zip (source is Directory.)
                    ZipFile.CreateFromDirectory(source, zipfile_path, compressionLevel, includeBaseDirectory);
                }
                else if (File.Exists(source))
                {
                    // Zip (source is File.)
                    using (FileStream fs = new FileStream(zipfile_path, FileMode.Create))
                    {
                        using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create))
                        {
                            archive.CreateEntryFromFile(source, System.IO.Path.GetFileName(source), CompressionLevel.Fastest);
                        }
                    }
                }
                else { throw new FileNotFoundException(); }


                // Return Zip file path
                return zipfile_path;
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 指定した zip アーカイブのすべてのファイルをファイル システムのディレクトリに抽出します。
        /// </summary>
        /// <param name="source">抽出するアーカイブのパス</param>
        /// <param name="destination">
        /// 抽出されたファイルを保存するフォルダーを置くディレクトリのパス。指定されたパスが存在しない場合は、エラーになります。
        /// 省略した場合の既定の設定は、カレントディレクトリです。
        /// </param>
        /// <param name="foldername">
        /// 抽出されたファイルを保存するルートディレクトリのフォルダー名。省略した場合の既定の設定では source から自動的に作成されます。
        /// </param>
        /// <param name="overwrite">
        /// 出力先フォルダーのパスにフォルダーまたはディレクトリが既に存在する場合、そのファイルまたはディレクトリを削除してから、ファイルを抽出します。
        /// 既定の設定では、出力先のパスにフォルダーまたはディレクトリが既に存在する場合はエラーになります。
        /// </param>
        /// <returns>抽出されたファイルが保存されているルートディレクトリのパス</returns>
        /// <remarks>
        /// foldername パラメーターが省略された場合、zip ファイルから拡張子を除いたフォルダーが destination ディレクトリに自動的に作成され、
        /// アーカイブから抽出したファイルはその中に保存されます。
        /// また、抽出ファイルを保存するルートディレクトリのパスが既に存在する場合は、エラーになります。
        /// </remarks>
        public static string Unzip(string source, string destination = "", string foldername = "", bool overwrite = false)
        {
            try
            {
                source = LocalFile.ConvertPath(source);

                // Validation (and convert) of source
                if (!File.Exists(source)) { throw new FileNotFoundException(); }

                // Validation of foldername
                if (foldername.Contains(Path.DirectorySeparatorChar)) { throw new ArgumentException(); }

                // Set default value_found of destination (Current Directory), Validate or Convert
                if (string.IsNullOrEmpty(destination)) { destination = Environment.CurrentDirectory; }
                else if (!Directory.Exists(destination)) { throw new DirectoryNotFoundException(); }
                else { destination = LocalFile.ConvertPath(destination); }

                // Set default value_found of folder name to extract Zip file (automatically created from source)
                if (string.IsNullOrEmpty(foldername)) { foldername = Path.GetFileNameWithoutExtension(source); }



                // Set Unzipped folder path
                string outputfolder_path = Path.Combine(destination, foldername);

                // Validation of Unzipped folder path (check if Original or Renewed Destination File already exists.)
                if (Directory.Exists(outputfolder_path))
                {
                    if (overwrite) { Directory.Delete(outputfolder_path, true); }
                    else { throw new IOException(); }
                }
                else if (File.Exists(destination))
                {
                    if (overwrite) { File.Delete(outputfolder_path); }
                    else { throw new IOException(); }
                }



                // Extract (Unzip)
                ZipFile.ExtractToDirectory(source, outputfolder_path);


                // Return UnZipped folder path
                return outputfolder_path;
            }
            catch (Exception e) { throw e; }
        }
    }
}
