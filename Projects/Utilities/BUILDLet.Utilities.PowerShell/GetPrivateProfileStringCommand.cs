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
    [Cmdlet(VerbsCommon.Get, "PrivateProfileString", DefaultParameterSetName = "Help")]
    [OutputType(typeof(string), typeof(PSCustomObject[]), ParameterSetName = new string[] { "Path", "LiteralPath", "InputObject" })]
    public class GetPrivateProfileStringCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return
  @"INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。
    該当するセクションとキーの組み合わせが存在する場合は、対応する値を返します。
    該当するセクションとキーの組み合わせが存在しない場合は null を返します。
    該当するセクションとキーの組み合わせが存在し、対応する値が存在しない場合は、
　　そのエントリーが '=' まで含んでいるかどうかにかかわらず、String.Empty を返します。
    行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
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


        [Parameter(ParameterSetName = "Path", Position = 1, HelpMessage = GetPrivateProfileStringCommand.SectionHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = GetPrivateProfileStringCommand.SectionHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = GetPrivateProfileStringCommand.SectionHelpMessage)]
        public string Section { get; set; }
        protected const string SectionHelpMessage =
            "取得する値のセクションを指定します。\n" +
            "Section パラメーターを省略した場合は、Source に含まれる全ての値を取得します。\n" +
            "該当するセクションが存在しない場合、および、該当するセクションが存在して、\n" +
            "そのセクションにエントリーが存在しない場合の出力はありません。";


        [Parameter(ParameterSetName = "Path", Position = 2, HelpMessage = GetPrivateProfileStringCommand.KeyHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = GetPrivateProfileStringCommand.KeyHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = GetPrivateProfileStringCommand.KeyHelpMessage)]
        public string Key { get; set; }
        protected const string KeyHelpMessage =
            "取得する値のキーを指定します。\n" +
            "Key パラメーターを省略した場合は、当該セクションに含まれる全ての値を取得します。";


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

                    // Get and Write Private Profile
                    this.writePrivateProfile(File.ReadAllLines(path), this.Section, this.Key, 
                        path.Split(new char[] { System.IO.Path.DirectorySeparatorChar }).Last());
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
                    // Get and Write Private Profile
                    this.writePrivateProfile(this.lines.ToArray(), this.Section, this.Key);
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


        private void writePrivateProfile(string[] contents, string section, string key, string filepath = "")
        {
            // for Verbose Message
            string from_file = string.IsNullOrEmpty(filepath) ? "" : string.Format("ファイル {0} から、", filepath);


            if (string.IsNullOrEmpty(this.Section))
            {
                // Read ALL

                // Verbose Output
                this.WriteVerbose(string.Format("{0}全ての値を取得します。", from_file));
                
                Dictionary<string, Dictionary<string, string>> profiles = PrivateProfile.Read(contents);

                if (profiles != null)
                {
                    foreach (var section_name in profiles.Keys)
                    {
                        foreach (var profile in profiles[section_name])
                        {
                            // Write Output
                            this.WritePSObject(new List<PSNoteProperty> {
                            new PSNoteProperty("Section", section_name),
                            new PSNoteProperty("Key", profile.Key),
                            new PSNoteProperty("Value", profile.Value)
                        });
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(this.Key))
            {
                // Read SECTION

                // Verbose Output
                this.WriteVerbose(string.Format( "{0}セクション '{1}' に対応する値を全て取得します。", from_file, this.Section));

                Dictionary<string, string> profiles = PrivateProfile.GetSection(this.Section, contents);

                if (profiles != null)
                {
                    foreach (var profile in profiles)
                    {
                        // Write Output
                        this.WritePSObject(new List<PSNoteProperty> {
                            new PSNoteProperty("Section", this.Section),
                            new PSNoteProperty("Key", profile.Key),
                            new PSNoteProperty("Value", profile.Value)
                        });
                    }
                }
            }
            else
            {
                // Read a Profile

                // Verbose Output
                this.WriteVerbose(string.Format("{0}セクション '{1}' とキー '{2}' の組み合わせに対応する値を取得します。", from_file, this.Section, this.Key));

                // Write Output
                this.WriteObject(PrivateProfile.GetValue(this.Section, this.Key, contents));
            }
        }
    }
}
