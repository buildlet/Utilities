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

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "PrivateProfileString", DefaultParameterSetName = "Help")]
    [OutputType("None", ParameterSetName = new string[] { "Path", "LiteralPath" })]
    [OutputType(typeof(string[]), ParameterSetName = new string[] { "InputObject" })]
    public class SetPrivateProfileStringCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return
  @"INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
    更新または追加する値は ',' を含むことができます。
    指定したセクションが存在しない場合は、contents の末尾に、そのセクションとエントリーを追加します。
    指定したセクションは存在して、キーが存在しない場合は、そのセクションにエントリーを追加します。
    指定したセクションとキーが存在する場合は、該当する値を更新します。値がない場合は、値を追加します。
    当該エントリーの末尾にコメントがある場合、そのコメントは削除されます。
";
            }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "入力ファイルのパスを指定します。")]
        [Alias("PSPath")]
        public string LiteralPath { get; set; }


        [Parameter(ParameterSetName = "InputObject", Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "入力データを指定します。")]
        [AllowEmptyString()]
        public string[] InputObject { get; set; }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 1, HelpMessage = SetPrivateProfileStringCommand.SectionHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, HelpMessage = SetPrivateProfileStringCommand.SectionHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", Mandatory = true, HelpMessage = SetPrivateProfileStringCommand.SectionHelpMessage)]
        public string Section { get; set; }
        protected const string SectionHelpMessage = "更新または追加するエントリーのセクションを指定します。";


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 2, HelpMessage = SetPrivateProfileStringCommand.KeyHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, HelpMessage = SetPrivateProfileStringCommand.KeyHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", Mandatory = true, HelpMessage = SetPrivateProfileStringCommand.KeyHelpMessage)]
        public string Key { get; set; }
        protected const string KeyHelpMessage = "更新または追加するエントリーのキーを指定します。";


        [Parameter(ParameterSetName = "Path", Position = 3, HelpMessage = SetPrivateProfileStringCommand.ValueHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = SetPrivateProfileStringCommand.ValueHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = SetPrivateProfileStringCommand.ValueHelpMessage)]
        [AllowEmptyString()]
        [AllowNull()]
        public string Value { get; set; }
        protected const string ValueHelpMessage = "更新または追加するエントリーの値を指定します。";


        private List<string> lines = new List<string>();


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
                    // Set SOURCE
                    string path = this.GetLocation(this.Path, (this.ParameterSetName == "Path"));

                    // Verbose Output
                    this.WriteVerbose(string.Format(
                        "ファイル {0} に、セクション '{1}' とキー '{2}' の組み合わせ{3}を書き込みます。", path, this.Section, this.Key,
                        (this.Value == null) ? "" : string.Format("に対応した値 '{0}' ", this.Value)));

                    // Process
                    PrivateProfile.SetValue(this.Section, this.Key, this.Value, path);

                    // Output
                    // (None)
                }
                else if (this.ParameterSetName == "InputObject")
                {
                    foreach (var line in this.InputObject) { this.lines.Add(line); }
                }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Post-Processing Tasks
        protected override void EndProcessing()
        {
            try
            {
                // Call base method
                base.EndProcessing();


                if (this.ParameterSetName == "InputObject")
                {
                    string[] contents = this.lines.ToArray();

                    // Verbose Output
                    this.WriteVerbose(string.Format(
                        "セクション '{0}' とキー '{1}' の組み合わせ{2}を書き込みます。", this.Section, this.Key,
                        (this.Value == null) ? "" : string.Format("に対応した値 '{0}' ", this.Value)));

                    // Process (Update or Append)
                    PrivateProfile.SetValue(this.Section, this.Key, this.Value, ref contents);

                    // Output
                    this.WriteObject(contents);
                }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Stop-Processing Tasks
        // protected override void StopProcessing() { }
    }
}
