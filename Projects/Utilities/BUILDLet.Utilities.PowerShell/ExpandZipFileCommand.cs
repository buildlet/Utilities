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
    [Cmdlet(VerbsData.Expand, "ZipFile", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(FileInfo), typeof(DirectoryInfo), ParameterSetName = new string[] { "Path", "LiteralPath" })]
    public class ExpandZipFileCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return @"zip ファイルを展開します。
    (Version 1.4.0.0 から、DotNetZip ライブラリを使用してファイルを展開します。)";
            }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "展開する zip ファイルのパスを指定します。")]
        [Alias("SourcePath")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "展開する zip ファイルのパスを指定します。")]
        [Alias("PSPath")]
        public string LiteralPath { get; set; }


        [Parameter(ParameterSetName = "Path", Position = 1, ValueFromPipelineByPropertyName = true, HelpMessage = ExpandZipFileCommand.DestinationPathHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = true, HelpMessage = ExpandZipFileCommand.DestinationPathHelpMessage)]
        public string DestinationPath { get; set; }
        protected const string DestinationPathHelpMessage =
            "展開されたファイルを保存するディレクトリのパスを指定します。存在しないパスが指定されたり、\n" +
            "同じ名前のファイルが存在する場合は、エラーになります。(コマンドレットはディレクトリを作成しません。)\n" +
            "既定の設定は、カレントディレクトリです。";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.ForceHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.ForceHelpMessage)]
        [Alias("Overwrite")]
        public SwitchParameter Force { get; set; }
        protected const string ForceHelpMessage =
            "ファイルの展開先に同じ名前のファイルやディレクトリが既に存在していた場合に、そのファイルを上書きします。\n" +
            "既定の設定では、上書きしません。\n" +
            "(このパラメーターを指定すると、Ionic.Zip.ZipEntry.Extract メソッドに指定する extractExistingFile パラメーターに\n" +
            " Ionic.Zip.ExtractExistingFileAction.OverwriteSilently が指定されます。\n" +
            " 既定では Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite です。)";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.PassThruHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.PassThruHelpMessage)]
        public SwitchParameter PassThru { get; set; }
        protected const string PassThruHelpMessage =
            "展開した全ての Ionic.Zip.ZipEntry を出力します。\n" +
            "既定では、ルート エントリー (ファイルまたはディレクトリ) のみを出力します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.SilentHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.SilentHelpMessage)]
        public SwitchParameter Silent { get; set; }
        protected const string SilentHelpMessage = "進行状況バーを表示しません。";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.PasswordHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.PasswordHelpMessage)]
        public string Password { get; set; }
        protected const string PasswordHelpMessage = "パスワードを指定します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.EncodingHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.EncodingHelpMessage)]
        public Encoding Encoding { get; set; }
        protected const string EncodingHelpMessage =
            "エンコーディングを指定します。既定のエンコーディングは System.Text.UnicodeEncoding.Default です。";


        [Parameter(ParameterSetName = "Path", HelpMessage = ExpandZipFileCommand.SuppressOutputHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = ExpandZipFileCommand.SuppressOutputHelpMessage)]
        public SwitchParameter SuppressOutput { get; set; }
        protected const string SuppressOutputHelpMessage =
            "このコマンドレットの出力を抑制します。\n" +
            "Source に含まれるエントリーが多く、パフォーマンスに影響がある場合に、このパラメーターを指定してください。\n" +
            "ただし、PassThru パラメーターと同時に指定された場合は、PassThru パラメーターが優先されます。";


        // ProgressRecord
        private ProgressRecord mainProgress = null;
        // private ProgressRecord subProgress;


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
                    this.WriteDebug(string.Format("Expand-ZipFile: Parameter 'Path'=\"{0}\"", this.Path));
                    this.WriteDebug(string.Format("Expand-ZipFile: Parameter 'LiteralPath'=\"{0}\"", this.LiteralPath));
                    this.WriteDebug(string.Format("Expand-ZipFile: Parameter 'DestinationPath=\"{0}\"", this.DestinationPath));
#endif

                    // Set SOURCE
                    string src = this.GetLocation(this.Path, (this.ParameterSetName == "Path"));
#if DEBUG
                    this.WriteDebug(string.Format("Expand-ZipFile: Set Source Path=\"{0}\"", src));
#endif

                    // Set DESTINATION
                    string dest = this.GetLocation(this.DestinationPath);
#if DEBUG
                    this.WriteDebug(string.Format("Expand-ZipFile: Set Destination Path=\"{0}\"", dest));
#endif


                    // Should Process
                    if (this.ShouldProcess(src, string.Format("ファイルを展開して {0} に保存", dest)))
                    {
                        // Validation (for Source)
                        if (!File.Exists(src)) { throw new FileNotFoundException(); }

                        // Validation (for Destination)
                        if (!Directory.Exists(dest))
                        {
                            if (File.Exists(dest))
                            {
                                // FILE already exists.
                                throw new IOException();
                            }
                            else
                            {
                                // DIRECTORY does NOT exist.
                                throw new DirectoryNotFoundException();
                            }
                            // Cmdlet does NOT create the target directory.
                        }


                        if (!this.Silent)
                        {
                            mainProgress = new ProgressRecord(0, string.Format("Expand-ZipFile: \"{0}\" を展開しています。", src), "準備中...");
                            // subProgress = new ProgressRecord(1, "準備中...", "準備中...");
                            // subProgress.ParentActivityId = 0;
                        }


                        // Inittialize Count(s)
                        int entryCount = 0;
                        int eventCount = 0;
                        int intervalCount = 150;


                        // using ZipFile
                        using (ZipFile zip = ZipFile.Read(src, new ReadOptions() { Encoding = (this.Encoding ?? UnicodeEncoding.Default) }))
                        {
                            // Root Entries for Output
                            List<string> roots = new List<string>();


                            // Extract Progress Event Handler
                            zip.ExtractProgress += (object sender, ExtractProgressEventArgs e) =>
                            {
#if DEBUG
                                // for Debug
                                this.WriteDebug(string.Format("e.EventType={0}", e.EventType));
                                this.WriteDebug(string.Format("e.ArchiveName={0}", e.ArchiveName));
                                this.WriteDebug(string.Format("e.BytesTransferred={0}", e.BytesTransferred));
                                this.WriteDebug(string.Format("e.TotalBytesToTransfer={0}", e.TotalBytesToTransfer));
                                this.WriteDebug(string.Format("e.EntriesTotal={0}", e.EntriesTotal));
                                this.WriteDebug(string.Format("e.CurrentEntry={0}", e.CurrentEntry));
                                this.WriteDebug(string.Format("e.EntriesExtracted={0}", e.EntriesExtracted));
                                this.WriteDebug("-----");
#endif

                                // NOT Silent
                                if (!this.Silent)
                                {
                                    if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
                                    {
                                        // Increment count of Entry
                                        entryCount++;

                                        mainProgress.Activity = (zip.Count > 1) ?
                                        string.Format("Expand-ZipFile: \"{0}\" ({1} / {2}) を展開しています。", src, entryCount, zip.Count) :
                                        string.Format("Expand-ZipFile: \"{0}\" を展開しています。", src);

                                        mainProgress.StatusDescription = string.Format("\"{0}\" を展開中...", e.CurrentEntry.FileName);
                                        mainProgress.PercentComplete = 100 * (entryCount - 1) / zip.Count;

                                        // Write Progress
                                        this.WriteProgress(mainProgress);
                                    }

                                    if ((e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten) && (e.TotalBytesToTransfer != 0))
                                    {
                                        // Increment event count
                                        if (++eventCount > intervalCount)
                                        {
                                            mainProgress.StatusDescription = string.Format("\"{0}\" ({1} / {2} バイト) を展開中...", e.CurrentEntry.FileName, e.BytesTransferred, e.TotalBytesToTransfer);
                                            mainProgress.PercentComplete = ((100 * (entryCount - 1)) + (int)(100 * e.BytesTransferred / e.TotalBytesToTransfer)) / zip.Count;

                                            // Write Progress
                                            this.WriteProgress(mainProgress);

                                            // Sub Progress
                                            // subProgress.Activity = string.Format("'{0}' を展開しています", e.CurrentEntry);
                                            // subProgress.StatusDescription = string.Format("{0} / {1} バイトを展開中...", e.BytesTransferred, e.TotalBytesToTransfer);
                                            // subProgress.PercentComplete = (int)(100 * e.BytesTransferred / e.TotalBytesToTransfer);
                                            // this.WriteProgress(subProgress);

                                            // Reset event count
                                            eventCount = 0;
                                        }
                                    }

                                    if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                                    {
                                        if (entryCount >= zip.Count)
                                        {
                                            // Complete Sub Progress
                                            // this.completeProcess(subProgress);

                                            // Complete Main Progress
                                            this.CompleteProcessRecord(mainProgress);
                                        }
                                    }
                                }


                                // PassThru (or NOT)
                                if (this.PassThru)
                                {
                                    // PassThru

                                    if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
                                    {
                                        string filename = e.CurrentEntry.FileName.Replace('/', System.IO.Path.DirectorySeparatorChar);

                                        // Output
                                        if (e.CurrentEntry.IsDirectory)
                                        {
                                            // Directory
                                            this.WriteObject(new DirectoryInfo(System.IO.Path.Combine(e.ExtractLocation, filename)));
                                        }
                                        else
                                        {
                                            // File
                                            this.WriteObject(new FileInfo(System.IO.Path.Combine(e.ExtractLocation, filename)));
                                        }
                                    }
                                }
                                else
                                {
                                    // NOT PassThru

                                    // NOT PassThru && NOT SuppressOutput
                                    if (!this.SuppressOutput)
                                    {
                                        string[] separeted = e.CurrentEntry.FileName.Split(new char[] { '/' });
                                        string root = separeted[0];

                                        if (!roots.Contains(root))
                                        {
                                            // Add File Name of Root Entry 
                                            roots.Add(root);

                                            // Output
                                            if (e.CurrentEntry.IsDirectory || (separeted.Length > 1))
                                            {
                                                // DIRECTORY
                                                this.WriteObject(new DirectoryInfo(System.IO.Path.Combine(dest, root)));
                                            }
                                            else
                                            {
                                                // FILE
                                                this.WriteObject(new FileInfo(System.IO.Path.Combine(dest, root)));
                                            }
                                        }
                                    }
                                }
                            };


                            // Set FileAction (Overwirte or NOT)
                            ExtractExistingFileAction fileAction = (this.Force ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.DoNotOverwrite);


                            // UnZip (Extract)
                            foreach (var entry in zip)
                            {
                                // Extract each Zip entries
                                if (string.IsNullOrEmpty(this.Password))
                                {
                                    entry.Extract(dest, fileAction);
                                }
                                else
                                {
                                    entry.ExtractWithPassword(dest, fileAction, this.Password);
                                }
                            }
                        }


                        if (!this.Silent)
                        {
                            // Complete Sub Progress
                            // this.completeProcess(subProgress);

                            // Complete Main Progress
                            this.CompleteProcessRecord(mainProgress);
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


            if (!this.Silent)
            {
                // Complete Sub Progress
                // this.completeProcess(subProgress);

                // Complete Main Progress
                this.CompleteProcessRecord(mainProgress);
            }
        }
    }
}
