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

using BUILDLet.Utilities.PInvoke;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Open, "HtmlHelp", DefaultParameterSetName = "Help")]
    [OutputType(typeof(IntPtr), ParameterSetName = new string[] { "Path", "LiteralPath" })]
    public class OpenHtmlHelpCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "HtmlHelp API を使用して HTML Help ファイルを開きます。"; }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "LiteralPath", Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "入力ファイルのパスを指定します。")]
        [Alias("PSPath")]
        public string LiteralPath { get; set; }


        [Parameter(ParameterSetName = "Path",  Position = 1, HelpMessage = OpenHtmlHelpCommand.TopicHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = OpenHtmlHelpCommand.TopicHelpMessage)]
        [PSDefaultValue(Value = 0)]
        public int Topic { get; set; }
        protected const string TopicHelpMessage = "0 または Help ID を指定してください。";


        [Parameter(ParameterSetName = "Path", HelpMessage = OpenHtmlHelpCommand.PassThruHelpMessage)]
        [Parameter(ParameterSetName = "LiteralPath", HelpMessage = OpenHtmlHelpCommand.PassThruHelpMessage)]
        public SwitchParameter PassThru { get; set; }
        protected const string PassThruHelpMessage =
            "ヘルプ ウィンドウのハンドル (hwnd) をパイプラインに渡します。\n" +
            "ただし、ヘルプのオープンに失敗した場合や、まだヘルプが開かれていない場合は 0 を返します。\n" +
            "既定では、このコマンドレットによる出力はありません。";


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if (this.ParameterSetName == "Path")
                {
                    // Get RESOLVED path
                    string[] locations = this.GetLocations(this.Path);

                    // Validate
                    if (locations.Length <= 0) { throw new FileNotFoundException(); }

                    foreach (var path in locations)
                    {
                        // Open Html Help File
                        this.openHtmlHelpFile(path);
                    }
                }
                else if (this.ParameterSetName == "LiteralPath")
                {
                    // Open Html Help File
                    this.openHtmlHelpFile(this.GetLocation(this.LiteralPath, false));
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
        // protected override void StopProcessing() { }


        private void openHtmlHelpFile(string path)
        {
            // Validate
            if (!File.Exists(path)) { throw new FileNotFoundException(); }

            // Verbose Output
            this.WriteVerbose(string.Format("\"{0}\" (Topic = {1}) を開きます。", path, this.Topic));

            // Open Html Help File
            IntPtr hwnd = HtmlHelp.Open(path, Topic);

            // Output (PassThru)
            if (this.PassThru) { this.WriteObject(hwnd); }
        }
    }
}
