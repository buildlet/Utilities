using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.IO;

using BUILDLet.Utilities;


namespace BUILDLet.PowerShell.Utilities
{
    [Cmdlet(VerbsCommon.Get, "PrivateProfileString")]
    [CmdletBinding(DefaultParameterSetName = "path")]
    [OutputType(typeof(string))]
    public class GetPrivateProfileString : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return
  @"INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。
    該当するセクションとキーの組み合わせが存在する場合は、対応する値を返します。
    該当するセクションとキーの組み合わせが存在しない場合は null を返します。
    該当するセクションとキーの組み合わせが存在し、対応する値が存在しない場合は、
　　そのエントリーが '=' まで含んでいるかどうかにかかわらず、String.Empty を返します。
    行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
";
            }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "path",
            HelpMessage = "入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "data",
            HelpMessage = "入力データを指定します。")]
        [AllowEmptyStringAttribute()]
        public string[] InputObject { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "path", HelpMessage = GetPrivateProfileString.helpMessage_Section)]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "data", HelpMessage = GetPrivateProfileString.helpMessage_Section)]
        public string Section { get; set; }
        private const string helpMessage_Section = "エントリーのセクションを指定します。";


        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "path", HelpMessage = GetPrivateProfileString.helpMessage_Key)]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "data", HelpMessage = GetPrivateProfileString.helpMessage_Key)]
        public string Key { get; set; }
        private const string helpMessage_Key = "エントリーのキーを指定します。";


        private List<string> lines = new List<string>();


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
                case "path":

                    // Verbose Output
                    this.WriteVerbose(string.Format(
                        "ファイル '{0}' から、セクション [{1}] とキー <{2}> の組み合わせに対応する値を取得します。",
                        this.Path, this.Section, this.Key));

                    // Process and Output
                    this.WriteObject(PrivateProfile.GetString(this.Section, this.Key, this.GetUnresolvedProviderPathFromPSPath(this.Path)));
                    break;

                case "data":
                    foreach (var line in this.InputObject) { this.lines.Add(line); }
                    break;

                default:
                    return;
            }
        }


        // Post-Processing Tasks
        protected override void EndProcessing()
        {
            // Call base method
            base.EndProcessing();


            try
            {
                switch (this.ParameterSetName)
                {
                    case "data":

                        // Verbose Output
                        this.WriteVerbose(string.Format("セクション [{0}] とキー <{1}> の組み合わせに対応する値を取得します。", this.Section, this.Key));

                        // Process and Output
                        this.WriteObject(PrivateProfile.GetString(this.Section, this.Key, this.lines.ToArray()));
                        break;

                    default:
                        return;
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
