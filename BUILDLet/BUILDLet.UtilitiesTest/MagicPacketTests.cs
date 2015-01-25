using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics;

namespace BUILDLet.Utilities.Network.Tests
{
    [TestClass()]
    public class MagicPacketTests
    {
        private string getExpectedMacAddress(string macAddress)
        {
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                hex.Append(macAddress[(i + 1) * 3 - 3]);
                hex.Append(macAddress[(i + 1) * 3 - 2]);
            }
            return hex.ToString();
        }

        private string getExpectedMagicPacket(string macAddress)
        {
            StringBuilder expected = new StringBuilder();

            string mac = this.getExpectedMacAddress(macAddress);

            for (int i = 0; i < 6; i++) { expected.Append("FF"); }
            for (int i = 0; i < 16; i++) { expected.Append(mac); }

            return expected.ToString();
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MagicPacketTest_RegularExpression_NG1()
        {
            new MagicPacket("abc");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MagicPacketTest_RegularExpression_NG2()
        {
            new MagicPacket("012345ABCDEF");                
        }

        [TestMethod()]
        public void MagicPacket_GetBytesTest()
        {
            foreach (var mac in NetworkTestData.MacAddresses)
            {
                string expected = this.getExpectedMagicPacket(mac);
                string actual = BitConverter.ToString((new MagicPacket(mac)).GetBytes()).Replace("-", "");

                Console.WriteLine();
                Console.WriteLine("Expected Text=\"{0}\"", expected);
                Console.WriteLine("Actual Text=\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MagicPacket_MacAddressTest()
        {
            foreach (var mac in NetworkTestData.MacAddresses)
            {
                string expected = mac.Replace('-', ':');
                string actual = (new MagicPacket(mac)).MacAddress;

                Console.WriteLine();
                Console.WriteLine("Expected Text=\"{0}\"", expected);
                Console.WriteLine("MagicPacket.MacAddress=\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MagicPacket_SeparatorTest()
        {
            foreach (var mac in NetworkTestData.MacAddresses)
            {
                char separator = '@';
                string expected = mac.Replace('-', separator).Replace(':', separator);

                MagicPacket packet = new MagicPacket(mac);
                packet.Separator = separator;
                string actual = packet.MacAddress;

                Console.WriteLine();
                Console.WriteLine("Expected Text=\"{0}\"", expected);
                Console.WriteLine("MagicPacket.MacAddress=\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
