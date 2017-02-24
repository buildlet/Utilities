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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using BUILDLet.Utilities.Diagnostics;
using BUILDLet.Utilities.Tests;
using BUILDLet.Utilities.Tests.Properties;


namespace BUILDLet.Utilities.Network.Tests
{
    [TestClass()]
    public class MagicPacketTests
    {
        // private value
        private string[] dummy_mac_addresses = {
            "00-00-00-00-00-00",
            "FF-FF-FF-FF-FF-FF",
            "00:00:00:00:00:00",
            "FF:FF:FF:FF:FF:FF",
            "01:23:45:AB:CD:EF",
            "AB:CD:EF:01:23:45"
        };


        // private method
        private string get_MagicPacketString(string macAddress)
        {
            // create MAC Address string
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < 6; i++) { hex.Append(macAddress.Substring(3 * i, 2)); }
            string body = hex.ToString();

            // create Magic Packet string
            StringBuilder packet = new StringBuilder();
            for (int i = 0; i < 6; i++) { packet.Append("FF"); }
            for (int i = 0; i < 16; i++) { packet.Append(body); }

            return packet.ToString();
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MagicPacket_MacAddress_RegularExpression_ExceptionTest1()
        {
            new MagicPacket("abc");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MagicPacket_MacAddress_RegularExpression_ExceptionTest2()
        {
            new MagicPacket("012345ABCDEF");                
        }


        [TestMethod()]
        public void MagicPacket_MacAddress_Test()
        {
            for (int i = 0; i < dummy_mac_addresses.Length; i++)
            {
                string expected = dummy_mac_addresses[i].Replace('-', ':');
                string actual = (new MagicPacket(dummy_mac_addresses[i])).MacAddress;

                Console.WriteLine("MAC Address({0})=\"{1}\"", i, dummy_mac_addresses[i]);
                Console.WriteLine("\"{0}\"=Expected", expected);
                Console.WriteLine("\"{0}\"=Actual", actual);
                Console.WriteLine();

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void MagicPacket_GetBytes_Test()
        {
            for (int i = 0; i < dummy_mac_addresses.Length; i++)
            {
                string expected = this.get_MagicPacketString(dummy_mac_addresses[i]);
                string actual = BitConverter.ToString((new MagicPacket(dummy_mac_addresses[i])).GetBytes()).Replace("-", "");

                Console.WriteLine("MAC Address({0})=\"{1}\"", i, dummy_mac_addresses[i]);
                Console.WriteLine("\"{0}\"=Expected", expected);
                Console.WriteLine("\"{0}\"=Actual", actual);
                Console.WriteLine();

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void MagicPacket_Separator_Test()
        {
            for (int i = 0; i < dummy_mac_addresses.Length; i++)
            {
                char separator = '@';
                string expected = dummy_mac_addresses[i].Replace('-', separator).Replace(':', separator);

                MagicPacket packet = new MagicPacket(dummy_mac_addresses[i]);
                packet.Separator = separator;
                string actual = packet.MacAddress;

                Console.WriteLine("MAC Address({0})=\"{1}\"", i, dummy_mac_addresses[i]);
                Console.WriteLine("\"{0}\"=Expected", expected);
                Console.WriteLine("\"{0}\"=Actual", actual);
                Console.WriteLine();

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MagicPacket_Send_ExeptionTest()
        {
            new MagicPacket("00:00:00:00:00:00").Send(1, -1);
        }


        [TestMethod()]
        public void MagicPacket_Send_Test()
        {
            DebugInfo.Init();

            int times = 0;
            int port = 0;

            foreach (var mac in this.dummy_mac_addresses)
            {
                MagicPacket packet = new MagicPacket(mac);

                packet.Send();
                Console.WriteLine("[{0}] MagicPacket.Send();  //MAC Address=\"{1}\"", DebugInfo.Time, mac);

                packet.Send(times = 0);
                Console.WriteLine("[{0}] MagicPacket.Send({2});  //MAC Address=\"{1}\"", DebugInfo.Time, mac, times);

                packet.Send(times = 5);
                Console.WriteLine("[{0}] MagicPacket.Send({2});  //MAC Address=\"{1}\"", DebugInfo.Time, mac, times);

                packet.Send(times = 3, port = 8080);
                Console.WriteLine("[{0}] MagicPacket.Send({2}, {3});  //MAC Address=\"{1}\"", DebugInfo.Time, mac, times, port);

                Console.WriteLine();
            }
        }


        [TestMethod()]
        public void MagicPacket_Send_NotSendTest()
        {
            DebugInfo.Init();

            new MagicPacket("00:00:00:00:00:00").Send(-1);
            Console.WriteLine("[{0}] Magic Packet should NOT be sent.", DebugInfo.Time);

            new MagicPacket("FF:FF:FF:FF:FF:FF").Send(-1);
            Console.WriteLine("[{0}] Magic Packet should NOT be sent.", DebugInfo.Time);
        }


        [TestMethod()]
        [TestCategory("GUI")]
        [TestCategory("MANUAL")]
        public void MagicPacket_WakeOnLanTest()
        {
            string caption = new AssemblyCustomAttributes().Title;
            string message = string.Empty;
            string filepath = Resources.LOCAL_FilePath_MacAddressTestFile;

            // Check file existence
            if (!File.Exists(filepath)) { Assert.Inconclusive("\"{0}\" file does not exists.", filepath); }


            // Read MAC Address from file
            string mac = (new StreamReader(filepath)).ReadLine().Substring(0, 17);


            // Show confirmation message
            message = "Are you sure you want to send the magic packet?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            // Abort
            if (result != DialogResult.Yes) { Assert.Inconclusive("Sending the Magic Packet was canceled!"); }


            // Send Magic Packet
            new MagicPacket(mac).Send();


            // Show message
            message = "Has Remote Computer been waken up?";
            result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);

            // Canceled
            if (result == DialogResult.Cancel) { Assert.Inconclusive("Test was canceled!"); }

            Assert.AreEqual(DialogResult.Yes, result);
        }
    }
}
