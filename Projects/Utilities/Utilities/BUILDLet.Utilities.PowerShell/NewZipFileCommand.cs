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

using System.Management.Automation;
using System.IO;

using Ionic.Zip;
using Ionic.Zlib;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "ZipFile", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(FileInfo), ParameterSetName = new string[] { "Path", "LiteralPath" })]
    public class NewZipFileCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return @"zip ファイルを作成します。
    (Version 1.4.0.0 から、DotNetZip ライブラリを使用してファイルを作成します。)";
            }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "zip 圧縮するフォルダーまたはファイルのパスを指定します。")]
        [Alias("SourcePath")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "zip 圧縮するフォルダーまたはファイルのパスを指定します。")]
        [Alias("PSPath")]
        public string LiteralPath { get; set; }


        [Parameter(ParameterSetName = "Path", Position = 1, ValueFromPipelineByPropertyName = true, HelpMessage = NewZipFileCommand.DestinationPathHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = true, HelpMessage = NewZipFileCommand.DestinationPathHelpMessage)]
        public string DestinationPath { get; set; }
        protected const string DestinationPathHelpMessage =
            "作成する zip ファイルのパスを指定します。\n" +
            "または、作成する zip ファイルを保存するディレクトリのパスを指定します。\n" +
            "\n" +
            "このパラメーターを省略した場合の既定の設定では、カレントディレクトリに既定のファイル名の zip ファイルを作成します。\n" +
            "\n" +
            "既に存在するファイルのパスが指定された場合は、既定ではエラーになります。\n" +
            "既に存在するファイルのパスが Force パラメーターとともに指定された場合は、指定されたファイルの削除を試みます。\n" +
            "\n" +
            "既に存在するディレクトリのパスが指定された場合は、指定されたディレクトリに、既定のファイル名の zip ファイルを作成します。\n" +
            "\n" +
            "既に存在するディレクトリのパスが指定されて、そのディレクトリに、既定のファイル名のファイルまたはディレクトリが既に存在する\n" +
            "場合は、既定ではエラーになります。\n" +
            "既に存在するディレクトリのパスが Force パラメーターとともに指定されて、そのディレクトリに、\n" +
            "既定のファイル名のファイルまたはディレクトリが既に存在する場合は、そのファイルまたはディレクトリの削除を試みます。\n" +
            "\n" +
            "存在しないディレクトリが含まれるパスが指定された場合は、エラーになります。\n" +
            "\n" +
            "source がファイルで、そのファイルに拡張子がある場合は source の拡張子を '.zip' に置き換えたものが既定のファイル名になります。\n" +
            "source がファイルで、そのファイルに拡張子がない場合、あるいは source がディレクトリの場合は、source に拡張子 '.zip' を付加\n" +
            "したものが既定のファイル名になります。";


        [Parameter(ParameterSetName = "Path", HelpMessage = NewZipFileCommand.ForceHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = NewZipFileCommand.ForceHelpMessage)]
        [Alias("Overwrite")]
        public SwitchParameter Force { get; set; }
        protected const string ForceHelpMessage =
            "作成する zip ファイルのパスに、既にファイルまたはディレクトリが存在する場合に、そのファイルまたはディレクトリの削除を試みます。\n" +
            "既定では、エラーになります。";


        [Parameter(ParameterSetName = "Path", HelpMessage = NewZipFileCommand.PassThruHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = NewZipFileCommand.PassThruHelpMessage)]
        public SwitchParameter PassThru { get; set; }
        protected const string PassThruHelpMessage = "作成した zip ファイルのパスを返します。既定ではこのコマンドレットによる出力はありません。";


        [Parameter(ParameterSetName = "Path", HelpMessage = NewZipFileCommand.SilentHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = NewZipFileCommand.SilentHelpMessage)]
        public SwitchParameter Silent { get; set; }
        protected const string SilentHelpMessage = "進行状況バーを表示しません。";


        [Parameter(ParameterSetName = "Path", HelpMessage = NewZipFileCommand.PasswordHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = NewZipFileCommand.PasswordHelpMessage)]
        public string Password { get; set; }
        protected const string PasswordHelpMessage = "パスワードを指定します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = NewZipFileCommand.EncodingHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = NewZipFileCommand.EncodingHelpMessage)]
        public Encoding Encoding { get; set; }
        protected const string EncodingHelpMessage =
            "エンコーディングを指定します。既定のエンコーディングは System.Text.Encoding.Default です。";


        // ProgressRecord
        private ProgressRecord mainProgress = null;


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if ((this.ParameterSetName == "Path") || (this.ParameterSetName == "LiteralPath"))
                {
#if DEBUG
                    this.WriteDebug(string.Format("New-ZipFile: Parameter 'Path'=\"{0}\"", this.Path));
                    this.WriteDebug(string.Format("New-ZipFile: Parameter 'LiteralPath'=\"{0}\"", this.LiteralPath));
                    this.WriteDebug(string.Format("New-ZipFile: Parameter 'DestinationPath'=\"{0}\"", this.DestinationPath));
#endif


                    // Set SOURCE
                    string src = this.GetLocation(this.Path, (this.ParameterSetName == "Path"));
#if DEBUG
                    this.WriteDebug(string.Format("New-ZipFile: Set Source Path=\"{0}\"", src));
#endif

                    // Set DESTINATION
                    string dest;
                    if (string.IsNullOrWhiteSpace(this.DestinationPath))
                    {
                        // Destination = CURRENT DIRECTORY (Temporary)
                        dest = this.GetLocation();
                    }
                    else
                    {
                        // Destination = Directory OR File Path (Temporary)
                        dest = this.GetLocation(this.DestinationPath, false);
                    }
#if DEBUG
                    this.WriteDebug(string.Format("New-ZipFile: Set Destination Path=\"{0}\"", dest));
#endif

                    // Update DESTINATION
                    if (Directory.Exists(dest))
                    {
                        // EXISTING DIRECTORY Path is specified.

                        // Update Destination Path (Add SOURCE File Name + Extension (".zip" or ".zip.zip")
                        dest = this.updateDestinationPath(src, dest);
#if DEBUG
                        this.WriteDebug(string.Format("New-ZipFile: Update Destination Path=\"{0}\"", dest));
#endif
                    }


                    // Should Process
                    if (this.ShouldProcess(src, string.Format("ファイルを圧縮して {0} に保存", dest)))
                    {
                        // Validation (SOURCE)
                        if (!File.Exists(src) && !Directory.Exists(src)) { throw new ItemNotFoundException(); }


                        // Validate (DESTINATION)
                        if (File.Exists(dest))
                        {
                            // EXISTING FILE Path is specified.

                            if (this.Force) { File.Delete(dest); }
                            else { throw new IOException(); }
                        }
                        else if (Directory.Exists(dest))
                        {
                            // Target Directory, which Name is same as Default File Name, already exists in EXISTING DIRECTORY.
                            // (if EXISTING DIRECTORY Path is specified, it is alrady updated.)

                            if (this.Force) { Directory.Delete(dest); }
                            else { throw new IOException(); }
                        }
                        else if (!Directory.Exists(Directory.GetParent(dest).FullName))
                        {
                            // PARENT DIRECTORY of Destination does NOT exist.
                            throw new DirectoryNotFoundException();
                        }
                        else if (dest.Contains(System.IO.Path.GetInvalidPathChars().ToString()))
                        {
                            // Destination contains Invalid Path Character.
                            throw new ArgumentException();
                        }


                        if (!this.Silent)
                        {
                            mainProgress = new ProgressRecord(0, string.Format("Expand-ZipFile: \"{0}\" を圧縮しています。", src), "準備中...");
                        }


                        // Inittialize Count(s)
                        int entryCount = 0;
                        int eventCount = 0;
                        int intervalCount = 150;


                        // using ZipFile
                        using (ZipFile zip = new ZipFile(dest, (this.Encoding ?? UnicodeEncoding.Default)))
                        {
                            // Extract Progress Event Handler
                            zip.SaveProgress += (object sender, SaveProgressEventArgs e) =>
                            {
#if DEBUG
                                // for Debug
                                this.WriteDebug(string.Format("e.EventType={0}", e.EventType));
                                this.WriteDebug(string.Format("e.ArchiveName={0}", e.ArchiveName));
                                this.WriteDebug(string.Format("e.BytesTransferred={0}", e.BytesTransferred));
                                this.WriteDebug(string.Format("e.TotalBytesToTransfer={0}", e.TotalBytesToTransfer));
                                this.WriteDebug(string.Format("e.EntriesTotal={0}", e.EntriesTotal));
                                this.WriteDebug(string.Format("e.CurrentEntry={0}", e.CurrentEntry));
                                this.WriteDebug(string.Format("e.EntriesExtracted={0}", e.EntriesSaved));
                                this.WriteDebug("-----");
#endif

                                if (!this.Silent)
                                {
                                    if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
                                    {
                                        // Increment count of Entry
                                        entryCount++;

                                        mainProgress.Activity = (zip.Count > 1) ?
                                        string.Format("Expand-ZipFile: \"{0}\" ({1} / {2}) を圧縮しています。", src, entryCount, zip.Count) :
                                        string.Format("Expand-ZipFile: \"{0}\" を圧縮しています。", src);

                                        mainProgress.StatusDescription = string.Format("\"{0}\" を圧縮中...", e.CurrentEntry.FileName);
                                        mainProgress.PercentComplete = 100 * (entryCount - 1) / zip.Count;

                                        // Write Progress
                                        this.WriteProgress(mainProgress);
                                    }

                                    if ((e.EventType == ZipProgressEventType.Saving_EntryBytesRead) && (e.TotalBytesToTransfer != 0))
                                    {
                                        if (++eventCount > intervalCount)
                                        {
                                            mainProgress.StatusDescription = string.Format("\"{0}\" ({1} / {2} バイト) を圧縮中...", e.CurrentEntry.FileName, e.BytesTransferred, e.TotalBytesToTransfer);
                                            mainProgress.PercentComplete = ((100 * (entryCount - 1)) + (int)(100 * e.BytesTransferred / e.TotalBytesToTransfer)) / zip.Count;

                                            // Write Progress
                                            this.WriteProgress(mainProgress);

                                            // Reset event count
                                            eventCount = 0;
                                        }
                                    }

                                    if (e.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
                                    {
                                        if (entryCount >= zip.Count)
                                        {
                                            // Complete Main Progress
                                            this.CompleteProcessRecord(mainProgress);
                                        }
                                    }
                                }
                            };


                            // Set password
                            if (!string.IsNullOrEmpty(this.Password)) { zip.Password = this.Password; }

                            // Set ZIP64 extension
                            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

                            // Workaround of DotNetZip bug
                            zip.ParallelDeflateThreshold = -1;


                            // Zip (Compress)
                            if (File.Exists(src))
                            {
                                // File
                                zip.AddFile(src, string.Empty);
                            }
                            else if (Directory.Exists(src))
                            {
                                // Directory
                                zip.AddDirectory(src, new DirectoryInfo(src).Name);
                            }
                            else { throw new Exception(); }

                            // Save as file
                            zip.Save();
                        }


                        // Complete Main Progress
                        if (!this.Silent) { this.CompleteProcessRecord(mainProgress); }


                        // Output (PassThru)
                        if (this.PassThru)
                        {
                            this.WriteObject(new FileInfo(dest));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Post-Processing Tasks
        // protected override void EndProcessing() { }


        // Stop-Processing Tasks
        protected override void StopProcessing()
        {
            // Call base method
            base.StopProcessing();


            // Complete Main Progress
            if (!this.Silent) { this.CompleteProcessRecord(mainProgress); }
        }


        // Destination + Source + Extension (.zip)
        private string updateDestinationPath(string source, string destination)
        {
            string filename = string.Empty;

            if (File.Exists(source))
            {
                filename = System.IO.Path.GetFileNameWithoutExtension(source) + ((System.IO.Path.GetExtension(source) == ".zip") ? ".zip.zip" : ".zip");
            }
            else if (Directory.Exists(source))
            {
                filename = source.Split(System.IO.Path.DirectorySeparatorChar).Last().TrimEnd(new char[] { System.IO.Path.DirectorySeparatorChar }) + ".zip";
            }

            return System.IO.Path.Combine(destination, filename);
        }
    }
}
