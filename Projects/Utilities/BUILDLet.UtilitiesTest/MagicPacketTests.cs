﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using BUILDLet.Utilities.Tests;


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
        public void MagicPacket_RegularExpression_ExceptionTest1()
        {
            new MagicPacket("abc");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MagicPacket_RegularExpression_ExceptionTest2()
        {
            new MagicPacket("012345ABCDEF");                
        }

        [TestMethod()]
        public void MagicPacket_GetBytesTest()
        {
            new Log().WriteLine("(E:Expected, A:Actual)");

            foreach (var mac in TestData.DummyMacAddresses)
            {
                string expected = this.getExpectedMagicPacket(mac);
                string actual = BitConverter.ToString((new MagicPacket(mac)).GetBytes()).Replace("-", "");

                Console.WriteLine();
                Console.WriteLine("E:\"{0}\"", expected);
                Console.WriteLine("A:\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MagicPacket_MacAddressTest()
        {
            new Log().WriteLine("(E:Expected, A:Actual=MagicPacket.MacAddress)");

            foreach (var mac in TestData.DummyMacAddresses)
            {
                string expected = mac.Replace('-', ':');
                string actual = (new MagicPacket(mac)).MacAddress;

                Console.WriteLine();
                Console.WriteLine("E:\"{0}\"", expected);
                Console.WriteLine("A:\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void MagicPacket_SeparatorTest()
        {
            new Log().WriteLine("(E:Expected, A:Actual=MagicPacket.MacAddress)");

            foreach (var mac in TestData.DummyMacAddresses)
            {
                char separator = '@';
                string expected = mac.Replace('-', separator).Replace(':', separator);

                MagicPacket packet = new MagicPacket(mac);
                packet.Separator = separator;
                string actual = packet.MacAddress;

                Console.WriteLine();
                Console.WriteLine("E:\"{0}\"", expected);
                Console.WriteLine("A:\"{0}\"", actual);

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MagicPacketTest_Send_ExeptionTest()
        {
            new MagicPacket("00:00:00:00:00:00").Send(1, -1);
        }


        [TestMethod()]
        public void MagicPacket_SendTest()
        {
            Log log = new Log();
            int times = 0;
            int port = 0;

            foreach (var mac in TestData.DummyMacAddresses)
            {
                log.WriteLine();
                log.WriteLine("MAC Address Text=\"{0}\"", mac);

                MagicPacket packet = new MagicPacket(mac);

                log.WriteLine("MagicPacket.Send()");
                packet.Send();

                log.WriteLine("MagicPacket.Send({0})", times = 0);
                packet.Send(times);

                log.WriteLine("MagicPacket.Send({0})", times = 5);
                packet.Send(times);

                log.WriteLine("MagicPacket.Send({0}, {1})", times = 3, port = 8080);
                packet.Send(times, port);
            }
        }


        [TestMethod()]
        public void MagicPacket_SendTest2()
        {
            Log log = new Log();

            new MagicPacket("00:00:00:00:00:00").Send(-1);
            log.WriteLine("Magic Packet should not be sent.");

            new MagicPacket("FF:FF:FF:FF:FF:FF").Send(-1);
            log.WriteLine("Magic Packet should not be sent.");
        }


        [TestMethod()]
        public void GUI_WakeOnLan_SendTest()
        {
            // for MessageBox
            string caption = new AssemblyCustomAttributes().AssemblyTitleAttribute;
            string message = string.Empty;


            // for file of MAC Address
            string filename = LocalPath.WakeOnLan_ConfigFilePath_from_MyDocuments;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);


            // Check file path
            if (!File.Exists(path)) { Assert.Inconclusive("\"{0}\" file does not exists.", filename); }


            // Read MAC Address from file
            string mac = (new StreamReader(path)).ReadLine().Substring(0, 17);


            // Show confirmation message
            message = "Are you sure you want to send the magic packet?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);


            // Abort
            if (result != DialogResult.Yes) { Assert.Inconclusive("Sending the Magic Packet was canceled!"); }


            // Send Magic Packet
            new MagicPacket(mac).Send();


            // Show message
            message = "Has Remote Computer been waken up?";
            result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            Assert.AreEqual(DialogResult.Yes, result);
        }
    }
}