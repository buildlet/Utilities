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
    [Cmdlet(VerbsCommon.New, "ZipFile")]
    [CmdletBinding(DefaultParameterSetName = "default")]
    [OutputType(typeof(string))]
    public class NewZipFile : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "zip ファイルを作成します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "default",
            HelpMessage = "zip 圧縮するフォルダーまたはファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "default", HelpMessage =
            "作成された zip ファイルが置かれるディレクトリのパスを指定します。\n" +
            "存在しないパスが指定された場合はエラーになります。既定の設定は、カレントディレクトリです。")]
        public string DestinationPath { get; set; }


        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "default", HelpMessage =
            "作成された zip ファイルのファイル名を指定します。" +
            "省略した場合の既定の設定では FilePath から自動的に作成されます。\n" +
            "その場合、source がファイルの場合は、source の拡張子を '.zip' に置き換えたものが zip ファイル名になります。\n" +
            "あるいは、source がディレクトリの場合は、source に拡張子 '.zip' を付加したものが zip ファイル名になります。")]
        public string FileName { get; set; }


        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "default", HelpMessage =
            "速度または圧縮サイズのどちらを重視するかを示す System.IO.CompressionLevel を指定します。\n" +
            "既定では、速度を重視する System.IO.CompressionLevel.Fastest です。")]
        [ValidateSet("Fastest", "NoCompression", "Optimal")]
        [PSDefaultValue(Value = "Fastest", Help = "既定では、速度を重視する System.IO.CompressionLevel.Fastest です。")]
        public string CompressionLevel { get; set; }


        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "default", HelpMessage = 
            "ディレクトリをアーカイブする場合、アーカイブのルートにあるディレクトリ名を含めない場合に指定します。\n" +
            "既定では、ルートディレクトリを含みます。")]
        public SwitchParameter NotIncludeBaseDirectory { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage =
            "出力先のパスにフォルダーまたはディレクトリが既に存在する場合、そのファイルまたはディレクトリを削除してから zip ファイルを作成します。\n" +
            "既定の設定では、出力先のパスにフォルダーまたはディレクトリが既に存在する場合はエラーになります。")]
        public SwitchParameter Force { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。")]
        public SwitchParameter WhatIf { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "作成された zip ファイルのパスを返します。既定では、このコマンドレットによる出力はありません。")]
        public SwitchParameter PassThru { get; set; }


        protected System.IO.Compression.CompressionLevel DefaultCompressionLevel = System.IO.Compression.CompressionLevel.Fastest;
        
        private System.IO.Compression.CompressionLevel level;


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
                        if (string.IsNullOrEmpty(this.FileName)) { this.FileName = string.Empty; }
                        if (this.CompressionLevel == null)
                        {
                            this.level = this.DefaultCompressionLevel;
                            this.CompressionLevel = this.level.ToString();
                        }
                        else
                        {
                            switch (this.CompressionLevel)
                            {

                                case "Fastest":
                                    this.level = System.IO.Compression.CompressionLevel.Fastest;
                                    break;

                                case "NoCompression":
                                    this.level = System.IO.Compression.CompressionLevel.NoCompression;
                                    break;

                                case "Optimal":
                                    this.level = System.IO.Compression.CompressionLevel.Optimal;
                                    break;

                                default:
                                    throw new ArgumentException();
                            }

                            this.level = System.IO.Compression.CompressionLevel.Fastest;
                        }

                        bool includeBaseDirectory = !this.NotIncludeBaseDirectory.ToBool();
                        bool overwrite = this.Force.ToBool();

                        // Convert path
                        string source = this.GetUnresolvedProviderPathFromPSPath(this.Path);
                        string destination = this.GetUnresolvedProviderPathFromPSPath(this.DestinationPath);

                        // Message
                        string message =
                            string.Format("'{0}' に対してファイルの圧縮を実行し、'{1}' に zip ファイルを作成します。", source, System.IO.Path.Combine(destination, this.FileName)) +
                            string.Format("(IncludeBaeDirectry='{0}', CompressionLevel='{1}')", includeBaseDirectory, this.CompressionLevel);

                        // WhatIf
                        if (this.WhatIf)
                        {
                            WriteObject("WhatIf" + message);
                            return;
                        }

                        // Verbose Output
                        WriteVerbose(message);

                        // Zip
                        string output = SimpleZipArchiver.Zip(source, destination, this.FileName, overwrite, this.level, includeBaseDirectory);

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
