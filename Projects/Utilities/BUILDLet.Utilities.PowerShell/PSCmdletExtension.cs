/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015, 2016 Daiki Sakamoto

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
using System.Reflection;
using System.IO;

namespace BUILDLet.Utilities.PowerShell
{
    // [CmdletBinding(DefaultParameterSetName = "help")]
    public abstract class PSCmdletExtension : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "help",
            HelpMessage = "このコマンドレットのヘルプを表示します。")]
        public SwitchParameter Help { get; protected set; }


        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "version",
            HelpMessage = "このコマンドレットが含まれるモジュールのバージョンを表示します。")]
        public SwitchParameter Version { get; protected set; }


        protected abstract string Synopsis { get; }

        
        protected override void ProcessRecord()
        {
            try
            {
                switch (this.ParameterSetName)
                {
                    case "help":
                        this.WriteObject(this.HelpMessage);
                        break;

                    case "version":
                        this.WriteObject(this.CmdletVersion);
                        break;

                    default:
                        break;
                }

                // base.ProcessRecord();
            }
            catch (Exception e) { throw e; }
        }


        protected string CmdletVersion
        {
            get
            {
                return ((AssemblyFileVersionAttribute)Assembly.GetCallingAssembly().GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version;
            }
        }


        protected string HelpMessage
        {
            get
            {
                return
@"
名前
    " + this.MyInvocation.MyCommand.Name + @"

概要
    " + this.Synopsis + @"
    詳細はヘルプを参照してください。

注釈
    ヘルプを参照するには、次のように入力してください。: " + "\"get-help " + this.MyInvocation.InvocationName + "\"" + @"
    技術情報を参照するには、次のように入力してください。: " + "\"get-help " + this.MyInvocation.InvocationName + " -full\"" + @"

";
            }
        }
    }
}
