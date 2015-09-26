/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015 Daiki Sakamoto

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

using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;


namespace BUILDLet.Utilities.Network
{
    /// <summary>
    /// マジックパケットを実装します。
    /// </summary>
    public class MagicPacket
    {
        private byte[] data = new byte[6 * (1 + 16)];

        private char separator;
        private char[] separators = { ':', '-' };


        /// <summary>
        /// 保持している MAC アドレスを16進文字列として取得します。
        /// </summary>
        public string MacAddress { get; protected set; }


        /// <summary>
        /// MAC アドレスの既定の区切り文字を取得、または設定します。
        /// 既定では ':' です。
        /// </summary>
        public char Separator
        {
            get { return this.separator; }
            set
            {
                // Update MAC address hex string
                this.MacAddress = this.MacAddress.Replace(this.separator, value);

                this.separator = value;
            }
        }

        
        /// <summary>
        /// <see cref="MagicPacket"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="macAddress">MAC アドレスの文字列を指定します。</param>
        public MagicPacket(string macAddress)
        {
            // Set initial value_found of separator
            this.separator = this.separators[0];


            // Validation
            bool match = false;
            foreach (var separator in this.separators)
            {
                if (Regex.IsMatch(macAddress + separator, "(([0-9A-Za-z]{2})" + separator + "){6}")) { match = true; }
            }
            if (!match) { throw new ArgumentException(); }

            
            // MAC Address
            var mac = from hex in macAddress.Split(separators) select Convert.ToByte(hex, 16);


            // Hex String
            StringBuilder hexMAC = new StringBuilder();
            foreach (byte hex in mac.ToArray()) { hexMAC.Append(string.Format("{0:X2}{1}", hex, this.separator)); }
            hexMAC.Remove(hexMAC.Length - 1, 1);

            this.MacAddress = hexMAC.ToString();

#if DEBUG
            Debug.WriteLine("");
            Debug.WriteLine(string.Format("[{0}] MAC Address=\"{1}\"", typeof(MagicPacket).Name, this.MacAddress));
#endif


            // Magic Packet
            byte[] packet = new byte[6 * (1 + 16)];
            // (Header) 0xFF * 6
            (new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).CopyTo(packet, 0);
            // (Body) MAC Address * 16
            for (int i = 0; i < 16; i++) { mac.ToArray().CopyTo(packet, 6 + (i * 6)); }

            this.data = packet;

#if DEBUG
            for (int i = 0; i < (1 + 16); i++)
            {
                Debug.Write(string.Format("[{0}] Magic Packet[{1:D2}]=0x", typeof(MagicPacket).Name, i));
                for (int j = 0; j < 6; j++) { Debug.Write(string.Format("{0:X2}", this.data[(i * 6) + j])); }
                Debug.WriteLine("");
            }
#endif
        }


        /// <summary>
        /// マジックパケットをバイト配列として取得します。
        /// </summary>
        /// <returns>マジックパケットのバイト配列</returns>
        public byte[] GetBytes() { return this.data; }


        /// <summary>
        /// マジックパケットを送信します。
        /// </summary>
        /// <param name="times">マジックパケットを送信する回数を指定します。省略した場合の既定の回数は 1 回です。</param>
        /// <param name="port">リモートマシンのポート番号を指定します。省略した場合の既定のポート番号は 2304 番です。</param>
        /// <returns>マジックパケットを送信した回数を返します。</returns>
        public int Send(int times = 1, int port = 2304)
        {
            try
            {
                UdpClient udp = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, port);
                int bytes = 0;
                int sent = 0;

                for (int i = 0; i < times; i++)
                {
                    bytes = udp.Send(this.data, this.data.Length, ep);
                    sent++;

#if DEBUG
                    Debug.WriteLine(
                        "[{0}] Magic Packet (MAC Address=\"{1}\", Port={2}) has been sent! ({3})", 
                        typeof(MagicPacket).Name, this.MacAddress, port, i + 1);
#endif
                }
                return sent;
            }
            catch (Exception e) { throw e; }
        }

    }
}
