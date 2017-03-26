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
using System.Collections;
using System.IO;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "HtmlString", DefaultParameterSetName = "Help")]
    [OutputType(typeof(string), typeof(string[]), ParameterSetName = new string[] { "Path", "LiteralPath", "InputObject" })]
    public class GetHtmlStringCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "入力データから HTML 要素の値を取得します。"; }
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
        [AllowEmptyStringAttribute()]
        public string[] InputObject { get; set; }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 1, HelpMessage = GetHtmlStringCommand.NameHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, HelpMessage = GetHtmlStringCommand.NameHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", Mandatory = true, HelpMessage = GetHtmlStringCommand.NameHelpMessage)]
        public string Name { get; set; }
        protected const string NameHelpMessage = "取得する HTML 要素の名前を指定します。";


        [Parameter(ParameterSetName = "Path", Position = 2, HelpMessage = GetHtmlStringCommand.AttributeHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = GetHtmlStringCommand.AttributeHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = GetHtmlStringCommand.AttributeHelpMessage)]
        public Hashtable Attributes { get; set; }
        protected const string AttributeHelpMessage = "取得する HTML 要素の属性の名前を指定します。";


        [Parameter(ParameterSetName = "Path", HelpMessage = GetHtmlStringCommand.EncodingHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = GetHtmlStringCommand.EncodingHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = GetHtmlStringCommand.EncodingHelpMessage)]
        [PSDefaultValue(Value = typeof(UTF8Encoding), Help = "既定のエンコーディングは " + GetHtmlStringCommand.DefaultEncodingName + " です。")]
        public Encoding Encoding { get; set; }
        protected const string DefaultEncodingName = "UTF-8 (Encoding.UTF8)";
        protected const string EncodingHelpMessage = 
            "HTML コンテンツをファイルから読み込む場合のエンコードを指定します。\n" +
            "既定では " + GetHtmlStringCommand.DefaultEncodingName + " です。";


        [Parameter(ParameterSetName = "Path", HelpMessage = GetHtmlStringCommand.StrictHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = GetHtmlStringCommand.StrictHelpMessage)]
        [Parameter(ParameterSetName = "InputObject", HelpMessage = GetHtmlStringCommand.StrictHelpMessage)]
        public SwitchParameter Strict { get; set; }
        protected const string StrictHelpMessage =
            "通常は、このスイッチをオンにしてください。\n" +
            "このスイッチをオフにすると、<P> 要素のように終了タグのない要素を許可しますが、" +
            "取得したい要素の値の中に入れ子になった要素が存在するときは、入れ子の要素の開始タグを含めたそれ以降の値は取得されません。";


        private StringBuilder lines = new StringBuilder();


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if (this.ParameterSetName == "Path" || this.ParameterSetName == "LiteralPath")
                {
#if DEBUG
                    this.WriteDebug(string.Format("Get-HtmlString: Parameter 'Path'=\"{0}\"", this.Path));
                    this.WriteDebug(string.Format("Get-HtmlString: Parameter 'LiteralPath'=\"{0}\"", this.LiteralPath));
                    // this.WriteDebug(string.Format("Get-HtmlString: Parameter 'InputObject'=\"{0}\"", this.InputObject));
                    this.WriteDebug(string.Format("Get-HtmlString: Parameter 'Encoding'=\"{0}\"", this.Encoding));
                    this.WriteDebug(string.Format("Get-HtmlString: Parameter 'Strict'={0}", this.Strict));
                    this.WriteDebug(string.Format("Get-HtmlString: Parameter 'Name'=\"{0}\"", this.Name));

                    if (this.Attributes != null)
                    {
                        int i = 0;
                        foreach (var attribute_name in this.Attributes.Keys)
                        {
                            this.WriteDebug(string.Format("Get-HtmlString: Parameter 'Attributes'[{0}]={{ \"{1}\"=\"{2}\" }}",
                                i++, attribute_name, this.Attributes[attribute_name]));
                        }
                    }
#endif

                    // Get Source Path
                    string path = this.GetLocation(this.Path, (this.ParameterSetName == "Path"));

                    this.writeHtmlString(File.ReadAllText(path, (this.Encoding ?? Encoding.UTF8)), path);
                }
                else if (this.ParameterSetName == "InputObject")
                {
                    foreach (var line in this.InputObject) { this.lines.Append(line); }
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
                    this.writeHtmlString(lines.ToString());
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


        private void writeHtmlString(string content, string path = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    // For Verbose Output
                    StringBuilder verbose_message = new StringBuilder();
                    if (!string.IsNullOrEmpty(path)) { verbose_message.AppendFormat("ファイル {0} から、", path); }


                    if (this.Attributes == null)
                    {
                        // Verbose Output
                        this.WriteVerbose(verbose_message.AppendFormat("要素 <{0}> の値を検索します。", this.Name).ToString());

                        // Get Value(s) of HTML Element
                        string[] values = SimpleHtmlParser.GetElements(content, this.Name, strict: this.Strict);

                        // Output
                        if (values.Length == 1) { this.WriteObject(values[0]);  }
                        else { this.WriteObject(values); }
                    }
                    else
                    {
                        string[,] attributes = new string[this.Attributes.Count, 2];

                        int i = 0;
                        foreach (var attribute_name in this.Attributes.Keys)
                        {
                            attributes[i, 0] = (string)attribute_name;
                            attributes[i, 1] = (string)this.Attributes[attribute_name];
                            i++;
                        }

                        // Verbose Output
                        this.WriteVerbose(verbose_message.AppendFormat("要素 <{0}> の値を検索します。", this.Name).ToString());
                        verbose_message = new StringBuilder("(");
                        for (int j = 0; j < attributes.Length / 2; j++)
                        {
                            verbose_message.AppendFormat("属性[{0}] '{1}' = '{2}'", j, attributes[j, 0], attributes[j, 1]);

                            if (j < ((attributes.Length / 2) - 1)) { verbose_message.Append(", "); }
                        }
                        this.WriteVerbose(verbose_message.Append(")").ToString());

                        // Get Value(s) of HTML Element
                        string[] values = SimpleHtmlParser.GetElements(content, this.Name, attributes, this.Strict);

                        // Output
                        if (values.Length == 1) { this.WriteObject(values[0]); }
                        else { this.WriteObject(values); }
                    }
                }
            }
            catch (Exception) { throw; }
        }
    }
}
