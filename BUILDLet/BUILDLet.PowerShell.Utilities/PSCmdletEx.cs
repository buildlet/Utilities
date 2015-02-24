using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;

namespace BUILDLet.PowerShell.Utilities
{
    [CmdletBinding(DefaultParameterSetName = "help")]
    public abstract class PSCmdletEx : PSCmdlet, ICmdletHelper
    {
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "help", HelpMessage = "このコマンドレットのヘルプを表示します。")]
        public SwitchParameter Help { get; protected set; }

        public abstract string Synopsis { get; }
        public string HelpMessage
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
";
            }
        }
    }
}
