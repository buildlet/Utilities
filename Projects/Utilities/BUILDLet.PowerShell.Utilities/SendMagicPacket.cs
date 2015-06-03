using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;

using BUILDLet.Utilities.Network;


namespace BUILDLet.PowerShell.Utilities
{
    [Cmdlet(VerbsCommunications.Send, "MagicPacket")]
    [CmdletBinding(DefaultParameterSetName = "default")]
    [OutputType(typeof(byte[]))]
    public class SendMagicPacket : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "指定された MAC アドレスのマジックパケットを送信します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName="default",
            HelpMessage = "マジックパケットに含まれる MAC アドレスを指定します。")]
        public string MacAddress { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "default",
            HelpMessage = "マジックパケットを送信する回数を指定します。省略した場合の既定の回数は 1 回です。")]
        [PSDefaultValue(Value = 1, Help = "既定の回数は 1 回です。")]
        public int Times { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "リモートマシンのポート番号を指定します。省略した場合の既定のポート番号は 2304 番です。")]
        [PSDefaultValue(Value = 2304, Help = "既定のポート番号は 2304 番です。")]
        public int Port { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default",
            HelpMessage = "生成したマジックパケットを返します。既定ではこのコマンドレットによる出力はありません。")]
        public SwitchParameter PassThru { get; set; }


        protected readonly int DefaultTimes = 1;
        protected readonly int DefaultPort = 2304;


        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();

            // Set default value of parameters
            if (this.Times <= 0) { this.Times = this.DefaultTimes; }
            if (this.Port <= 0) { this.Port = this.DefaultPort; }

            try
            {
                switch (this.ParameterSetName)
                {
                    case "default":
                        // Create Magic Packet
                        MagicPacket packet = new MagicPacket(this.MacAddress);

                        // Send Magic Packet
                        packet.Send(this.Times, this.Port);

                        // Verbose Output
                        WriteVerbose(string.Format(
                            "MAC アドレス \"{0}\" のマジックパケットを、ポート番号 {1} 番に {2} 回送信しました。",
                            this.MacAddress, this.Port, this.Times));

                        // Output Magic Packet (only in case of 'PassThru')
                        if (this.PassThru) { this.WriteObject(packet.GetBytes()); }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
