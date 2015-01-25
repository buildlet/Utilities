using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Set initial value of separator
            this.separator = this.separators[0];


            // Validation
            bool match = false;
            foreach (var separator in this.separators)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(macAddress + separator, "(([0-9A-Za-z]{2})" + separator + "){6}")) { match = true; }
            }
            if (!match) { throw new ArgumentException(); }

            
            // MAC Address
            var mac = from hex in macAddress.Split(separators) select Convert.ToByte(hex, 16);


            // Hex String
            StringBuilder hexMac = new StringBuilder();
            foreach (byte hex in mac.ToArray()) { hexMac.Append(string.Format("{0:X2}{1}", hex, this.separator)); }
            hexMac.Remove(hexMac.Length - 1, 1);

            this.MacAddress = hexMac.ToString();

#if DEBUG
            Debug.WriteLine("");
            Debug.WriteLine("[MagicPacket]: MAC Address=\"" + this.MacAddress + "\"");
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
                Debug.Write(string.Format("[MagicPacket]: Magic Packet[{0:D2}]=0x", i));
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
    }
}
