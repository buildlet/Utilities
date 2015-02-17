using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.IO;

using BUILDLet.Utilities.Cryptography;

namespace BUILDLet.PowerShell.Utilities
{
    [Cmdlet(VerbsCommon.Get, "HashValue")]
    [CmdletBinding()]
    public class Get_HashValue : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "path")]
        public string Path { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "data")]
        public byte[] InputObject { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public string Algorithm { get; set; }

        protected override void ProcessRecord()
        {
            if (Algorithm == null) { this.Algorithm = "MD5"; }

            try
            {
                switch (this.ParameterSetName)
                {
                    case "path":
                        string filepath = this.Path;
                        if (!File.Exists(filepath)) { filepath = System.IO.Path.Combine(this.SessionState.Path.CurrentFileSystemLocation.Path, this.Path); }
                        if (!File.Exists(filepath)) { throw new FileNotFoundException(); }

                        this.WriteObject(new HashCode(filepath, this.Algorithm).ToString());
                        break;

                    case "data":
                        this.WriteObject(new HashCode(this.InputObject, this.Algorithm).ToString());
                        break;

                    default:
                        break;
                }

                // base.ProcessRecord();
            }
            catch (Exception e) { throw e; }
        }
    }
}
