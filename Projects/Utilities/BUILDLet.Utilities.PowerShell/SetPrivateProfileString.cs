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

using System.Management.Automation;
using System.IO;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.PowerShell
{
    [Cmdlet(VerbsCommon.Set, "PrivateProfileString")]
    [CmdletBinding(DefaultParameterSetName = "path")]
    [OutputType("None", ParameterSetName = new string[] { "path" })]
    [OutputType(typeof(string[]), ParameterSetName = new string[] { "data" })]
    public class SetPrivateProfileString : PSCmdletExtension
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


        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "path",
            HelpMessage = "INI ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "data",
            HelpMessage = "入力データを指定します。")]
        [AllowEmptyStringAttribute()]
        public string[] InputObject { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "path", HelpMessage = SetPrivateProfileString.helpMessage_Section)]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "data", HelpMessage = SetPrivateProfileString.helpMessage_Section)]
        public string Section { get; set; }
        private const string helpMessage_Section = "更新または追加するエントリーのセクションを指定します。";


        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "path", HelpMessage = SetPrivateProfileString.helpMessage_Key)]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "data", HelpMessage = SetPrivateProfileString.helpMessage_Key)]
        public string Key { get; set; }
        private const string helpMessage_Key = "更新または追加するエントリーのキーを指定します。";


        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "path", HelpMessage = SetPrivateProfileString.helpMessage_Value)]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "data", HelpMessage = SetPrivateProfileString.helpMessage_Value)]
        [AllowEmptyString()]
        [AllowNull()]
        public string Value { get; set; }
        private const string helpMessage_Value = "更新または追加するエントリーの値を指定します。";


        private List<string> lines = new List<string>();


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
            // Call base method
            base.BeginProcessing();
        }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();


            switch (this.ParameterSetName)
            {
                case "path":

                    // Verbose Output
                    StringBuilder message = new StringBuilder(
                        string.Format("ファイル '{0}' に、セクション [{1}] とキー <{2}> の組み合わせ", this.Path, this.Section, this.Key));
                    if (this.Value != null) { message.AppendFormat("に対応した値 '{0}' ", this.Value); }
                    this.WriteVerbose(message.Append("を書き込みます。").ToString());

                    // Process
                    PrivateProfile.SetString(this.Section, this.Key, this.Value, this.GetUnresolvedProviderPathFromPSPath(this.Path));
                    break;

                case "data":

                    // DEBUG
                    // for (int i = 0; i < this.InputObject.Length; i++) { Console.WriteLine("InputObject[{0}]={1}",i, this.InputObject[i]); }

                    foreach (var line in this.InputObject) { this.lines.Add(line); }
                    break;

                default:
                    return;
            }
        }


        // Post-Processing Tasks
        protected override void EndProcessing()
        {
            // Call base method
            base.EndProcessing();


            try
            {
                switch (this.ParameterSetName)
                {
                    case "data":
                        string[] contents = this.lines.ToArray();

                        // DEBUG
                        // for (int i = 0; i < contents.Length; i++) { Console.WriteLine("contents[{0}]={1}", i, contents[i]); }

                        // Verbose Output
                        StringBuilder message = new StringBuilder(
                            string.Format("セクション [{0}] とキー <{1}> の組み合わせ", this.Section, this.Key));
                        if (this.Value != null) { message.AppendFormat("に対応した値 '{0}' ", this.Value); }
                        this.WriteVerbose(message.Append("を更新または追加します。").ToString());

                        // Process (Update or Append)
                        PrivateProfile.SetString(this.Section, this.Key, this.Value, ref contents);

                        // Output
                        this.WriteObject(contents);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
