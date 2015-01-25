using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace BUILDLet.Utilities.Network
{
    /// <summary>
    /// Wake On Lan を実装します。
    /// </summary>
    public class WakeOnLan
    {
        /// <summary>
        /// <see cref="WakeOnLan"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        protected WakeOnLan() { }


        /// <summary>
        /// 指定された MAC アドレスのマジックパケットを送信します。
        /// </summary>
        /// <param name="macAddress">リモートマシンの MAC アドレスの文字列を指定します。</param>
        /// <param name="times">マジックパケットを送信する回数を指定します。省略した場合の既定の回数は 1 回です。</param>
        /// <param name="port">リモートマシンのポート番号を指定します。省略した場合の既定のポート番号は 2304 番です。</param>
        /// <returns>マジックパケットを送信した回数を返します。</returns>
        public static int Send(string macAddress, int times = 1, int port = 2304)
        {
            try
            {
                return WakeOnLan.Send(new MagicPacket(macAddress), times, port);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 指定された MAC アドレスのマジックパケットを送信します。
        /// </summary>
        /// <param name="packet">送信するマジックパケットを指定します。</param>
        /// <param name="times">マジックパケットを送信する回数を指定します。省略した場合の既定の回数は 1 回です。</param>
        /// <param name="port">リモートマシンのポート番号を指定します。省略した場合の既定のポート番号は 2304 番です。</param>
        /// <returns>マジックパケットを送信した回数を返します。</returns>
        public static int Send(MagicPacket packet, int times = 1, int port = 2304)
        {
            try
            {
                UdpClient udp = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, port);

                int bytes = 0;
                int sent = 0;
                for (int i = 0; i < times; i++)
                {
                    bytes = udp.Send(packet.GetBytes(), packet.GetBytes().Length, ep);

#if DEBUG
                    Debug.WriteLine("[WakeOnLan]: Magic Packet (MAC Address=\"{0}\", Port={1}) has been sent! ({2})", packet.MacAddress, port, i + 1);
#endif
                }
                return sent;
            }
            catch (Exception e) { throw e; }
        }
    }
}
