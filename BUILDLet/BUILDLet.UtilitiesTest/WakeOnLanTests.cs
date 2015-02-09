using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Windows.Forms;
using System.Reflection;

using System.Diagnostics;

namespace BUILDLet.Utilities.Network.Tests
{
    [TestClass]
    public class WakeOnLanTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WakeOnLan_SendTest_Exeption()
        {
            WakeOnLan.Send("00:00:00:00:00:00", 1, -1);
        }


        [TestMethod()]
        public void WakeOnLan_SendTest()
        {
            Log log = new Log();
            int times = 0;
            int port = 0;

            foreach (var mac in TestData.DummyMacAddresses)
            {
                log.WriteLine();
                log.WriteLine("MAC Address Text=\"{0}\"", mac);

                MagicPacket packet = new MagicPacket(mac);

                log.WriteLine("WakeOnLan.Send(packet)");
                WakeOnLan.Send(packet);

                log.WriteLine("WakeOnLan.Send(packet, {0})", times = 0);
                WakeOnLan.Send(packet, times);

                log.WriteLine("WakeOnLan.Send(packet, {0})", times = 5);
                WakeOnLan.Send(packet, times);

                log.WriteLine("WakeOnLan.Send(packet, {0}, {1})", times = 3, port = 8080);
                WakeOnLan.Send(packet, times, port);
            }
        }


        [TestMethod()]
        public void WakeOnLan_SendTest2()
        {
            Log log = new Log();

            WakeOnLan.Send("00:00:00:00:00:00", -1);
            log.WriteLine("Magic Packet should not be sent.");

            WakeOnLan.Send("FF:FF:FF:FF:FF:FF", -1);
            log.WriteLine("Magic Packet should not be sent.");
        }


        [TestMethod()]
        public void GUI_WakeOnLan_SendTest()
        {
            // for MessageBox
            string caption = new AssemblyCustomAttributes().AssemblyTitleAttribute;
            string message = string.Empty;


            // for file of MAC Address
            string filename = TestData.WakeOnLan_ConfigFileName;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);


            // Check file path
            if (!File.Exists(path)) { Assert.Inconclusive("\"{0}\" file does not exists.", filename); }


            // Read MAC Address from file
            string mac = (new StreamReader(path)).ReadLine().Substring(0, 17);


            // Show confirmation message
            message = "Are you sure you want to send the magic packet?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);


            // Abort
            if (result != DialogResult.Yes) { Assert.Inconclusive("Sending the Magic Packet was canceled!"); }


            // Send Magic Packet
            WakeOnLan.Send(mac);


            // Show message
            message = "Has Remote Computer been waken up?";
            result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            Assert.AreEqual(DialogResult.Yes, result);
        }
    }
}
