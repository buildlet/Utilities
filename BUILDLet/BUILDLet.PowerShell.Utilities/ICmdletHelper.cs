using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUILDLet.PowerShell.Utilities
{
    public interface ICmdletHelper
    {
        string Synopsis { get; }
        string HelpMessage { get; }
    }
}
