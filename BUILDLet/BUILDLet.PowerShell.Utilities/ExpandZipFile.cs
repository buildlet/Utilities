using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.IO;
using System.IO.Compression;

using BUILDLet.Utilities.Compression;


namespace BUILDLet.PowerShell.Utilities
{
    [Cmdlet(VerbsData.Expand, "ZipFile")]
    [CmdletBinding(DefaultParameterSetName = "default")]
    [OutputType(typeof(string))]
    public class ExpandZipFile : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "zip ファイルを解凍します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "default",
            HelpMessage = "解凍する zip ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "default", HelpMessage =
            "解凍したファイルを保存するルートディレクトリを置くディレクトリのパスを指定します。\n" +
            "存在しないパスが指定された場合はエラーになります。既定の設定は、カレントディレクトリです。")]
        public string DestinationPath { get; set; }


        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "default", HelpMessage = 
            "解凍したファイルを保存するルートディレクトリのフォルダー名を指定します。\n" +
            "既定の設定は source から拡張子を除いた名前です。")]
        public string FolderName { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = 
            "出力先のパスにフォルダーまたはディレクトリが既に存在する場合、そのファイルまたはディレクトリを削除してから zip ファイルを解凍します。\n" +
            "既定の設定では、出力先のパスにフォルダーまたはディレクトリが既に存在する場合はエラーになります。")]
        public SwitchParameter Force { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。")]
        public SwitchParameter WhatIf { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "解凍されたファイルのパスを返します。既定では、このコマンドレットによる出力はありません。")]
        public SwitchParameter PassThru { get; set; }

        
        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();

            try
            {
                switch (this.ParameterSetName)
                {
                    case "default":

                        // Set default value of parameters
                        if (string.IsNullOrEmpty(this.DestinationPath)) { this.DestinationPath = string.Empty; }
                        if (string.IsNullOrEmpty(this.FolderName)) { this.FolderName = string.Empty; }
                        bool overwrite = this.Force.ToBool();

                        // Convert path
                        string source = this.GetUnresolvedProviderPathFromPSPath(this.Path);
                        string destination = this.GetUnresolvedProviderPathFromPSPath(this.DestinationPath);

                        // Message
                        string message = string.Format("'{0}' に対してファイルの解凍を実行し、'{1}' に保存します。", source, System.IO.Path.Combine(destination, this.FolderName));

                        // WhatIf
                        if (this.WhatIf)
                        {
                            WriteObject("WhatIf: " + message);
                            return;
                        }

                        // Verbose Output
                        WriteVerbose(message);

                        // UnZip
                        string output = SimpleZipArchiver.Unzip(source, destination, this.FolderName, overwrite);

                        // Output (PassThru)
                        if (this.PassThru) { WriteObject(output); }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
