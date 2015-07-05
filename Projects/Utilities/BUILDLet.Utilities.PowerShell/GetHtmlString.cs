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
    [Cmdlet(VerbsCommon.Get, "HtmlString")]
    [CmdletBinding(DefaultParameterSetName = "path")]
    [OutputType(typeof(string), typeof(string[]))]
    public class GetHtmlString : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "入力データから HTML 要素またはその属性の値を取得します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "path",
            HelpMessage = "入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "data",
            HelpMessage = "入力データを指定します。")]
        [AllowEmptyStringAttribute()]
        public string[] InputObject { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "path", HelpMessage = GetHtmlString.helpMessage_Name)]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "data", HelpMessage = GetHtmlString.helpMessage_Name)]
        public string Name { get; set; }
        private const string helpMessage_Name = "取得する HTML 要素の名前を指定します。";


        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "path", HelpMessage = GetHtmlString.helpMessage_Attribute)]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "data", HelpMessage = GetHtmlString.helpMessage_Attribute)]
        public string Attribute { get; set; }
        private const string helpMessage_Attribute = "属性を取得する場合、取得する HTML 要素の属性の名前を指定します。";


        [Parameter(Mandatory = false, ParameterSetName = "path",
            HelpMessage = "HTML コンテンツをファイルから読み込む場合のエンコードを指定します。")]
        [PSDefaultValue(Value = typeof(UTF8Encoding), Help = "既定では UTF-8 (Encoding.UTF8) です。")]
        public Encoding Encoding { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "path", HelpMessage = GetHtmlString.helpMessage_Strict)]
        [Parameter(Mandatory = false, ParameterSetName = "data", HelpMessage = GetHtmlString.helpMessage_Strict)]
        public SwitchParameter Strict { get; set; }
        private const string helpMessage_Strict =
            "通常は、このスイッチをオンにしてください。\n" +
            "このスイッチをオフにすると、<P> 要素のように終了タグのない要素を許可しますが、" +
            "取得したい要素の値の中に入れ子になった要素が存在するときは、入れ子の要素の開始タグを含めたそれ以降の値は取得されません。";


        private StringBuilder lines = new StringBuilder();


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
            // Call base method
            base.BeginProcessing();

            // Set default value of parameters
            if (this.Encoding == null) { this.Encoding = Encoding.UTF8; }
        }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();


            switch (this.ParameterSetName)
            {
                case "path":
                    this.innerProcess(File.ReadAllText(this.GetUnresolvedProviderPathFromPSPath(this.Path), this.Encoding));
                    break;

                case "data":
                    foreach (var line in this.InputObject) { this.lines.Append(line); }
                    break;

                default:
                    break;
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
                        this.innerProcess(lines.ToString());
                        break;

                    default:
                        return;
                }
            }
            catch (Exception e) { throw e; }
        }


        private void innerProcess(string content)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    // For Verbose Output
                    StringBuilder message = new StringBuilder();
                    if (this.ParameterSetName == "path") { message.AppendFormat("ファイル '{0}' から、", this.Path); }


                    if (string.IsNullOrEmpty(this.Attribute))
                    {
                        // Verbose Output
                        this.WriteVerbose(message.AppendFormat("要素 <{0}> の値を検索します。", this.Name).ToString());

                        // Process and Output
                        this.WriteObject(SimpleHtmlParser.GetElements(content, this.Name, this.Strict.ToBool()));
                    }
                    else
                    {
                        // Verbose Output
                        this.WriteVerbose(message.AppendFormat("要素 <{0}> の属性 '{1}' の値を検索します。", this.Name, this.Attribute).ToString());

                        // Process and Output
                        this.WriteObject(SimpleHtmlParser.GetAttributes(content, this.Name, this.Attribute));
                    }
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
