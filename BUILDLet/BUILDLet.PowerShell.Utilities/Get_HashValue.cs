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
    [CmdletBinding(DefaultParameterSetName="help")]
    [OutputType(typeof(string))]
    public class Get_HashValue : PSCmdletEx, ICmdletHelper
    {
        public override string Synopsis
        {
            get { return "指定されたハッシュ アルゴリズムを使用して、入力データのハッシュ値を計算します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "data",
            HelpMessage = "バイト配列のハッシュ値を求める場合に、入力データを指定します。")]
        public byte[] InputObject { get; set; }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "path",
            HelpMessage = "ファイルやフォルダーのハッシュ値を求める場合に、入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = false, Position = 1,
            HelpMessage = "使用するハッシュ アルゴリズムの名前を指定します。既定のアルゴリズムは MD5 です。")]
        public string Algorithm { get; set; }


        protected override void ProcessRecord()
        {
            if (Algorithm == null) { this.Algorithm = "MD5"; }

            try
            {
                switch (this.ParameterSetName)
                {
                    case "help":
                        this.WriteObject(this.HelpMessage);
                        break;

                    case "data":
                        this.WriteObject(new HashCode(this.InputObject, this.Algorithm).ToString());
                        break;

                    case "path":
                        string filepath = this.Path;
                        if (!File.Exists(filepath)) { filepath = System.IO.Path.Combine(this.SessionState.Path.CurrentFileSystemLocation.Path, this.Path); }
                        if (!File.Exists(filepath)) { throw new FileNotFoundException(); }

                        this.WriteObject(new HashCode(filepath, this.Algorithm).ToString());
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
