using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Reflection;
using System.IO;

namespace BUILDLet.PowerShell.Utilities
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


        //protected string ConvertPath(string path)
        //{
        //    try { return Path.IsPathRooted(path) ? path : Path.Combine(this.SessionState.Path.CurrentFileSystemLocation.Path, path); }
        //    catch (Exception e) { throw e; }
        //}


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
