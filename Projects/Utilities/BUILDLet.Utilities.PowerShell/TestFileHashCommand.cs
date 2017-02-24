/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2016-2017 Daiki Sakamoto

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

using BUILDLet.Utilities.Cryptography;
using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsDiagnostic.Test, "FileHash", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(bool), typeof(PSCustomObject), typeof(PSCustomObject[]), ParameterSetName = new string[] { "Path", "LiteralPath" })]
    public class TestFileHashCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return @"指定されたパスをルートとする相対パスが同じファイルのハッシュ値を比較することによって、
    ファイル内容が同一かどうかを確認します。指定されたパスに含まれる全てのファイルの内容が同じ場合に true を返します。
    Difference が指定されなかった場合は、Reference に含まれるファイルのハッシュ値を取得します。";
            }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "比較の参照として使用されるパスを指定します。")]
        [Alias("ReferencePath")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "比較の参照として使用されるパスを指定します。")]
        [Alias("PSPath")]
        public string LiteralPath { get; set; }


        [Parameter(ParameterSetName = "Path", Position = 1, ValueFromPipelineByPropertyName = true, HelpMessage = TestFileHashCommand.DifferencePathHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = true, HelpMessage = TestFileHashCommand.DifferencePathHelpMessage)]
        public string DifferencePath { get; set; }
        protected const string DifferencePathHelpMessage = "Reference パスと比較するパスを指定します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = TestFileHashCommand.AlgorithmHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = TestFileHashCommand.AlgorithmHelpMessage)]
        [PSDefaultValue(Value = TestFileHashCommand.DefaultAlgorithmName, Help = TestFileHashCommand.DefaultAlgorithmHelpMessage)]
        public string Algorithm { get; set; }
        protected const string DefaultAlgorithmName = HashCode.DefaultHashAlgorithmName;
        protected const string DefaultAlgorithmHelpMessage = "既定のアルゴリズムは " + TestFileHashCommand.DefaultAlgorithmName + " です。";
        protected const string AlgorithmHelpMessage = "ファイルを比較するときのハッシュ アルゴリズムを指定します。" + TestFileHashCommand.DefaultAlgorithmHelpMessage;


        [Parameter(ParameterSetName = "Path", HelpMessage = TestFileHashCommand.CaseSensitiveHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = TestFileHashCommand.CaseSensitiveHelpMessage)]
        public SwitchParameter CaseSensitive { get; set; }
        protected const string CaseSensitiveHelpMessage = "ファイル パスを比較するときに、大文字と小文字を区別します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = TestFileHashCommand.FileNameHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = TestFileHashCommand.FileNameHelpMessage)]
        public SwitchParameter PassThru { get; set; }
        protected const string FileNameHelpMessage = "Reference と Difference の共通部分を PSCustomObject として出力します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = TestFileHashCommand.SilentHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = TestFileHashCommand.SilentHelpMessage)]
        public SwitchParameter Silent { get; set; }
        protected const string SilentHelpMessage = "進行状況バーを表示しません。";


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // ProgressRecord
        private ProgressRecord progress = null;

        
        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();

                if ((this.ParameterSetName == "Path") || (this.ParameterSetName == "LiteralPath"))
                {
                    // Set Should Process Message
                    string target;
                    string action;
                    if (string.IsNullOrEmpty(this.DifferencePath))
                    {
                        target = (this.GetLocation(this.Path, (this.ParameterSetName == "Path")));
                        action = "ハッシュ値の計算";
                    }
                    else
                    {
                        target =
                            "Reference: " + (this.GetLocation(this.Path, (this.ParameterSetName == "Path"))) + " と " +
                            "Difference: " + (this.GetLocation(this.DifferencePath, (this.ParameterSetName == "Path")));
                        action = "ファイルの比較";
                    }


                    // Should Process
                    if (this.ShouldProcess(target, action))
                    {
                        // Update Hash Algorithm
                        if (string.IsNullOrEmpty(this.Algorithm)) { this.Algorithm = TestFileHashCommand.DefaultAlgorithmName; }
#if DEBUG
                        // Output Debug Message
                        this.WriteDebug(string.Format("Test-FileHash: Hash Algorithm=\"{0}\"", this.Algorithm));
#endif


                        // Prepare container of File Hash(es)
                        //   0: Relative Path
                        //   1: Path (Full Path)
                        //   2: Hash (File Hash)
                        List<string[]> references = new List<string[]>();
                        List<string[]> differences = new List<string[]>();
                        List<string[]> intersections = new List<string[]>();


                        // 0: ReferencePath
                        // 1: DifferencePath
                        for (int i = 0; i < (string.IsNullOrEmpty(this.DifferencePath) ? 1 : 2); i++)
                        {
                            // Set container of File Hash
                            List<string[]> hashes = (i == 0 ? references : differences);

                            // Set Target (Refference OR Difference), Path (itself) & Prefix (File OR Directory) Path
                            target = ((i == 0) ? "Reference" : "Difference");
                            string path = (this.GetLocation(((i == 0) ? this.Path : this.DifferencePath), (this.ParameterSetName == "Path")));
                            string prefix = this.getPrefixPath(path);
#if DEBUG
                            // Output Debug Message
                            this.WriteDebug(string.Format("Test-FileHash: {0}Path=\"{1}\"", target, path));
                            this.WriteDebug(string.Format("Test-FileHash: Prefix Path of {0}Path=\"{1}\"", target, prefix));
#endif

                            // Prepare Progress
                            int files = 0;
                            if (!this.Silent)
                            {
                                this.progress = new ProgressRecord(0, string.Format("Test-FileHash: {0} のハッシュ値を計算しています", path), "準備中...");
                                files = (File.Exists(path) ? 1 : Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length);
                            }

                            // Get Hash Value(s) of tergets
                            this.getHashValues(path, ref hashes, target, prefix, files);


                            if (Directory.Exists(prefix))
                            {
                                // Verbose Output
                                this.WriteVerbose(string.Format("Test-FileHash: {0} フォルダーに {1} のファイルが見つかりました。", path, hashes.Count));
                            }
#if DEBUG
                            this.WriteDebug(string.Format("Test-FileHash: {0} File(s) in {1}Path", hashes.Count, target));
#endif

                            // Comparison
                            if (i >= 1)
                            {
                                var query =
                                    from reference in references
                                    join difference in differences on reference[0] equals difference[0]
                                    where reference[2] == difference[2]
                                    select reference;

                                foreach (var item in query)
                                {
                                    intersections.Add(item);
#if DEBUG
                                    this.WriteDebug(string.Format("Test-FileHash:[{0}] Relative Path \"{1}\" is common.", intersections.Count - 1, item[0]));
#endif
                                }

                                // Verbose Output
                                this.WriteVerbose(string.Format("Test-FileHash: {0} のファイルが同じファイルです。", intersections.Count));
#if DEBUG
                                this.WriteDebug(string.Format("Test-FileHash: {0} Same File(s)", intersections.Count));
#endif
                            }
                        }


                        // Complete Main Progress
                        if (!this.Silent) { this.CompleteProcessRecord(progress); }

                        
                        // Output
                        if (string.IsNullOrEmpty(this.DifferencePath))
                        {
                            foreach (var item in references)
                            {
                                this.writeHashValue(item[1], item[2]);
                            }
                        }
                        else
                        {
                            if (this.PassThru)
                            {
                                foreach (var item in intersections)
                                {
                                    this.writeHashValue(item[1], item[2]);
                                }
                            }
                            else
                            {
                                this.WriteObject((intersections.Count == references.Count) && (intersections.Count == differences.Count));
                            }
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
            if (!this.Silent) { this.CompleteProcessRecord(progress); }
        }


        // Get Prefix (FILE or DIRECTORY) Path without the Ending Character "\"
        private string getPrefixPath(string path)
        {
            // Remove "\" appended the end of path, if exists
            return ((path.Last() != System.IO.Path.DirectorySeparatorChar) ? path : path.TrimEnd(new char[] { System.IO.Path.DirectorySeparatorChar }));
        }


        // Get Hash Value(s)
        private void getHashValues(string path, ref List<string[]> hashes, string target, string prefix, int files)
        {
            if (File.Exists(path))
            {
                // FILE

                // Verbose Output
                this.WriteVerbose(string.Format("Test-FileHash ({0}): ファイル {1} が見つかりました。", target, path));

                // Get Relative Path
                string relative_path = (
                    File.Exists(prefix) ?
                    path.Substring(Directory.GetParent(prefix).FullName.Length + 1) :
                    path.Substring(prefix.Length + 1)
                    );


                if (!this.Silent)
                {
                    // Progress Record
                    this.progress.StatusDescription = string.Format("ファイル {0} ({1} / {2}) のハッシュ値を計算中...",
                        System.IO.Path.GetFileName(path), hashes.Count + 1, files);
                    this.progress.PercentComplete = (100 * hashes.Count) / files;

                    // Write Progress
                    this.WriteProgress(this.progress);
                }


                // Get File Hash
                string hash = new HashCode(path, this.Algorithm).ToString();


                // NOT Case Sensitive
                if (!this.CaseSensitive) { relative_path = relative_path.ToUpper(); }


                // Add File Path and Hash Value
                hashes.Add(new string[] { relative_path, path, hash });

#if DEBUG
                this.WriteDebug(string.Format("{0}({1}):[{2}] Add Relative Path \"{3}\".", DebugInfo.ShortName, target, hashes.Count - 1, relative_path));
#endif
            }
            else if (Directory.Exists(path))
            {
                // DIRECTORY

                // Verbose Output
                this.WriteVerbose(string.Format("Test-FileHash ({0}): フォルダー {1} を検索します。", target, path));

                // Search Child Entries (File OR Directory)
                foreach (var child in Directory.GetFileSystemEntries(path))
                {
                    this.getHashValues(child, ref hashes, target, prefix, files);
                }
            }
            else { throw new Exception(); }
        }


        // Write Hash Value
        private void writeHashValue(string path, string hash)
        {
            this.WritePSObject(new List<PSNoteProperty> {
                            new PSNoteProperty("Algorithm", this.Algorithm),
                            new PSNoteProperty("Hash", hash),
                            new PSNoteProperty("Path", path)
                        });
        }
    }
}
