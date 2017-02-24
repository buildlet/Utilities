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

using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using BUILDLet.Utilities.PInvoke;
using BUILDLet.Utilities.PInvokeTests.Properties;


namespace BUILDLet.Utilities.PInvoke.Tests
{
    [TestClass()]
    public class HtmlHelpTests
    {
        [TestMethod()]
        [TestCategory("GUI")]
        [TestCategory("MANUAL")]
        public void HtmlHelp_Invoke_Test()
        {
            int waitSec = 3;
            string helpFileName = Path.GetFileName(Resources.FileName_ChmHelp_UtilitiesHelpFile);
            IntPtr handle = IntPtr.Zero;

            // handle = (IntPtr)Process.GetCurrentProcess().Handle;
            handle = Window.GetDesktopWindow();


            // Copy Help File
            File.Copy(Path.Combine(Resources.FolderPath_UtilitiesHelpFile, helpFileName), helpFileName, true);

            // Show message
            string caption = "HtmlHelp Test";
            string message = string.Format("Please do not touch the Help window to be shown, and wait {0} seconds.", waitSec);
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


            // Open Help File
            HtmlHelp.Invoke(handle, helpFileName, (uint)HtmlHelpCommands.HH_DISPLAY_TOPIC, 0);

            // Wait
            Thread.Sleep(waitSec * 1000);

            // Close Help File
            HtmlHelp.Invoke(IntPtr.Zero, null, (uint)HtmlHelpCommands.HH_CLOSE_ALL, 0);

            // Wait
            Thread.Sleep(1500);


            // Show confirmation message
            message = "HTML Help has been shown, and closed automtically?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            Assert.AreEqual(DialogResult.Yes, result);
        }


        [TestMethod()]
        [TestCategory("GUI")]
        [TestCategory("MANUAL")]
        public void HtmlHelp_Open_Close_Test()
        {
            // Open Help File
            HtmlHelp.Open(Path.Combine(Resources.FolderPath_UtilitiesHelpFile, Resources.FileName_ChmHelp_UtilitiesHelpFile));

            // Wait
            Thread.Sleep(3000);

            // Close Help File
            HtmlHelp.Close();
        }


        [TestMethod()]
        [TestCategory("GUI")]
        [TestCategory("MANUAL")]
        public void HtmlHelp_Open_Close_ContinuousTest()
        {
            int waitSec = 2;
            string helpFileDir = Resources.FolderPath_ChmHelpFile;
            string[] helpFiles =
            {
                Resources.FileName_ChmHelp_TestFile1,
                Resources.FileName_ChmHelp_TestFile2,
                Resources.FileName_ChmHelp_TestFile3,
                Resources.FileName_ChmHelp_TestFile4,
                Resources.FileName_ChmHelp_TestFile5
            };
            IntPtr handle = Window.GetDesktopWindow();


            // Show message
            string caption = "HtmlHelp Test";
            string message = string.Format("Please do not touch the Help window to be shown, and count it.", waitSec);
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


            foreach (var helpFileName in helpFiles)
            {
                // Copy Help File
                File.Copy(Path.Combine(helpFileDir, helpFileName), helpFileName, true);

                // Open Help File
                // HtmlHelp.Invoke(handle, helpFileName, (uint)HtmlHelpCommands.HH_DISPLAY_TOPIC, 0);
                HtmlHelp.Open(helpFileName);

                // Wait
                Thread.Sleep(waitSec * 1000);

                // Close Help File
                // HtmlHelp.Invoke(IntPtr.Zero, null, (uint)HtmlHelpCommands.HH_CLOSE_ALL, 0);
                HtmlHelp.Close();
            }


            // Wait
            Thread.Sleep(1000);

            // Show confirmation message
            message = string.Format("HTML Help has been shown {0} times, and closed automtically?", helpFiles.Length);
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            Assert.AreEqual(DialogResult.Yes, result);
        }
    }
}
