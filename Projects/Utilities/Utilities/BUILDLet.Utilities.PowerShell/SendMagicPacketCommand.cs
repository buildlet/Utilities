/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

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

using BUILDLet.Utilities.Network;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommunications.Send, "MagicPacket", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(byte[]))]
    public class SendMagicPacketCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "指定された MAC アドレスのマジックパケットを送信します。"; }
        }


        [Parameter(ParameterSetName = "MacAddress", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "マジックパケットに含まれる MAC アドレスを指定します。")]
        public string MacAddress { get; set; }


        [Parameter(ParameterSetName = "MacAddress", ValueFromPipelineByPropertyName = true,
            HelpMessage = "マジックパケットを送信する回数を指定します。" + SendMagicPacketCommand.DefaultCountHelpMessage)]
        [PSDefaultValue(Value = SendMagicPacketCommand.DefaultCount, Help = SendMagicPacketCommand.DefaultCountHelpMessage)]
        public int Count { get; set; }
        protected const int DefaultCount = 1;
        protected const string DefaultCountHelpMessage = "既定の送信回数は 1 回です。";


        [Parameter(ParameterSetName = "MacAddress", ValueFromPipelineByPropertyName = true,
            HelpMessage = "リモートマシンのポート番号を指定します。" + SendMagicPacketCommand.DefaultPortHelpMessage)]
        [PSDefaultValue(Value = SendMagicPacketCommand.DefaultPort, Help = SendMagicPacketCommand.DefaultPortHelpMessage)]
        public int Port { get; set; }
        protected const int DefaultPort = 2304;
        protected const string DefaultPortHelpMessage = "既定のポート番号は 2304 番です。";


        [Parameter(ParameterSetName = "MacAddress",
            HelpMessage = "生成したマジックパケットを返します。既定ではこのコマンドレットによる出力はありません。")]
        public SwitchParameter PassThru { get; set; }


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if (this.ParameterSetName == "MacAddress")
                {
                    // Set default value of parameters
                    if (this.Count <= 0) { this.Count = SendMagicPacketCommand.DefaultCount; }
                    if (this.Port <= 0) { this.Port = SendMagicPacketCommand.DefaultPort; }


                    if (this.ShouldProcess(
                        string.Format("MAC アドレス {0} のマジックパケット", this.MacAddress),
                        string.Format("ポート番号 {0} 番に {1} 回の送信", this.Port, this.Count)))
                    {
                        // Create Magic Packet
                        MagicPacket packet = new MagicPacket(this.MacAddress);

                        // Send Magic Packet
                        packet.Send(this.Count, this.Port);

                        // Output Magic Packet (only in case of 'PassThru')
                        if (this.PassThru) { this.WriteObject(packet.GetBytes()); }
                    }
                }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Post-Processing Tasks
        // protected override void EndProcessing() { }


        // Stop-Processing Tasks
        // protected override void StopProcessing() { }
    }
}
