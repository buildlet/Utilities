/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2016 Daiki Sakamoto

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


namespace BUILDLet.Utilities.PowerShell
{
    [Cmdlet(VerbsCommon.Open, "HtmlHelp")]
    [CmdletBinding(DefaultParameterSetName = "default")]
    [OutputType(typeof(IntPtr))]
    public class OpenHtmlHelp : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "HtmlHelp API を使用して HTML Help ファイルを開きます。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "default",
            HelpMessage = "入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "default",
            HelpMessage = "0 または Help ID を指定してください。")]
        [PSDefaultValue(Value = 0)]
        public int Topic { get; set; }



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
                case "default":

                    // Convert Path
                    string path = this.GetUnresolvedProviderPathFromPSPath(this.Path);

                    // Validate
                    if (!File.Exists(path)) { throw new FileNotFoundException(); }

                    // Verbose Output
                    this.WriteVerbose(string.Format("\"{0}\" (Topic = {1}) を開きます。", path, this.Topic));

                    try
                    {
                        // Open Html Help File
                        IntPtr hwnd = HtmlHelp.Open(path, Topic);

                        // Output
                        this.WriteObject(hwnd);
                    }
                    catch (Exception e) { throw e; }
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
        }
    }
}
