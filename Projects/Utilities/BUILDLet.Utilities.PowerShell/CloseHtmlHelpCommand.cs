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
    [Cmdlet(VerbsCommon.Close, "HtmlHelp")]
    [OutputType("None")]
    public class CloseHtmlHelpCommand : PSCmdlet    // PSCmdletExtension (NOT Inherited)
    {
        [Parameter(HelpMessage = "このコマンドレットのヘルプを表示します。")]
        public SwitchParameter Help { get; protected set; }


        [Parameter(HelpMessage = "このコマンドレットが含まれるモジュールのバージョンを表示します。")]
        public SwitchParameter Version { get; protected set; }


        private string synopsis = "HtmlHelp API を使用して、開かれた HTML Help ファイルを全て閉じます。";


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();

                if (this.Help)
                {
                    // Show Help
                    this.WriteObject(PSCmdletExtension.GetHelpMessage(this.MyInvocation.InvocationName, this.synopsis));
                }
                else if (this.Version)
                {
                    // Show Version
                    this.WriteObject(PSCmdletExtension.GetCmdletVersion());
                }
                else
                {
                    // Close Html Help File
                    HtmlHelp.Close();
                }
            }
            catch (Exception) { throw; }
        }


        // Post-Processing Tasks
        // protected override void EndProcessing() { }


        // Stop-Processing Tasks
        // protected override void StopProcessing() { }
    }
}
