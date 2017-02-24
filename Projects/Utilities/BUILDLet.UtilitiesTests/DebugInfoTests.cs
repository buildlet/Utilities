/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2016-2017 Daiki Sakamoto

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
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.Diagnostics.Tests
{
    [TestClass()]
    public class DebugInfoTests
    {
        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_DateTimeFormat_NullTest()
        {
            DebugInfo.Init();
            DebugInfo.DateTimeFormat = null;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DebugInfo.DateTimeFormat=null", DebugInfo.ToString());
        }

        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_DateTimeFormat_StringEmptyTest()
        {
            DebugInfo.Init();
            DebugInfo.DateTimeFormat = string.Empty;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DebugInfo.DateTimeFormat=string.Empty", DebugInfo.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void DebugInfo_DateTimeFormat_ExceptionTest()
        {
            string parameter = "0";

            DebugInfo.Init();
            DebugInfo.DateTimeFormat = parameter;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DebugInfo.DateTimeFormat=\"{1}\"", DebugInfo.ToString(), parameter);
        }



        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_GetDateTime_Test()
        {
            string[] parameters =
            {
                "yyyy/MM/dd HH:mm:ss",
                "MMMM dd, yyyy"
            };

            DebugInfo.Init();
            Console.WriteLine("DebugInfo.Init();  //Initialize");
            Console.WriteLine();

            Console.WriteLine("// Default");
            Console.WriteLine("DebugInfo.GetDateTime()=\"{0}\"", DebugInfo.GetDateTime());
            Console.WriteLine("DebugInfo.ToString()=\"{0}\";", DebugInfo.ToString());
            Console.WriteLine();

            foreach (var format in parameters)
            {
                Console.WriteLine("// \"{0}\"", format);
                Console.WriteLine("DebugInfo.GetDateTime(\"{0}\")=\"{1}\"", format, DebugInfo.GetDateTime(format));
                Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //should NOT be \"{1}\".", DebugInfo.ToString(), format);
                Console.WriteLine();
                Console.WriteLine("// DebugInfo.DateTimeFormat=\"{0}\";", (DebugInfo.DateTimeFormat = format));
                Console.WriteLine("DebugInfo.GetDateTime()=\"{0}\"", DebugInfo.GetDateTime());
                Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //should be \"{1}\".", DebugInfo.ToString(), format);
                Console.WriteLine();
            }
        }



        [TestMethod()]
        public void DebugInfo_GetCallerName_Test()
        {
            object[,] parameters =
            {
                // DebugInfoCallerFormat.None,
                { DebugInfoCallerFormat.Name, "DebugInfo_GetCallerName_Test" },
                { DebugInfoCallerFormat.ShortName, "DebugInfoTests.DebugInfo_GetCallerName_Test" },
                { DebugInfoCallerFormat.FullName,"BUILDLet.Utilities.Diagnostics.Tests.DebugInfoTests.DebugInfo_GetCallerName_Test" },
                { DebugInfoCallerFormat.ClassName,"DebugInfoTests" },
                { DebugInfoCallerFormat.FullClassName, "BUILDLet.Utilities.Diagnostics.Tests.DebugInfoTests" }
            };


            DebugInfo.Init();
            Console.WriteLine("DebugInfo.Init();  //Initialize");
            Console.WriteLine();

            // Console Output (0: Default)
            Console.WriteLine("// Default");
            Console.WriteLine("DebugInfo.GetCallerName()=\"{0}\"", DebugInfo.GetCallerName());
            Console.WriteLine("DebugInfo.ToString()=\"{0}\";", DebugInfo.ToString());
            Console.WriteLine();
            // Assertion (0: Default)
            Assert.AreEqual("DebugInfoTests.DebugInfo_GetCallerName_Test", DebugInfo.GetCallerName());

            for (int i = 0; i < parameters.Length / 2; i++)
            {
                DebugInfoCallerFormat format = (DebugInfoCallerFormat)parameters[i, 0];
                string expected = (string)parameters[i, 1];

                // Console Output (1)
                Console.WriteLine();
                Console.WriteLine("// {0}", parameters[i, 0].ToString());
                Console.WriteLine("DebugInfo.GetCallerName({0})=\"{1}\"", format, DebugInfo.GetCallerName(format));
                Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //should NOT be \"{1}\".", DebugInfo.ToString(), format);
                Console.WriteLine();
                // Assertion (1)
                Assert.AreEqual(expected, DebugInfo.GetCallerName(format));

                // Console Output (2)
                Console.WriteLine("// DebugInfo.CallerFormat={0};", (DebugInfo.CallerFormat = format).ToString());
                Console.WriteLine("DebugInfo.GetCallerName()=\"{0}\"", DebugInfo.GetCallerName());
                Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //should be \"{1}\".", DebugInfo.ToString(), format);
                Console.WriteLine();
                // Assertion (2)
                Assert.AreEqual(expected, DebugInfo.GetCallerName());
            }
        }

        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_GetCallerName_SkipFramesTest()
        {
            DebugInfo.Init();
            Console.WriteLine("DebugInfo.Init();  //Initialize");

            for (int i = -10; i < 20; i++)
            {
                Console.Write("DebugInfo.GetCallerName(FullName, {0}) = ", i);

                try
                {
                    Console.WriteLine(DebugInfo.GetCallerName(DebugInfoCallerFormat.FullName, i));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }



        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_Properties_DefaultTest()
        {
            DebugInfo.Init();
            Console.WriteLine("DebugInfo.Init();  //Initialize");
            Console.WriteLine();

            // Console Output
            Console.WriteLine("[{0}] DebugInfo.Date", DebugInfo.Date);
            Console.WriteLine("[{0}] DebugInfo.Time", DebugInfo.Time);
            Console.WriteLine("[{0}] DebugInfo.DateTime", DebugInfo.DateTime);
            Console.WriteLine("[{0}] DebugInfo.Name", DebugInfo.Name);
            Console.WriteLine("[{0}] DebugInfo.ShortName", DebugInfo.ShortName);
            Console.WriteLine("[{0}] DebugInfo.FullName", DebugInfo.FullName);
            Console.WriteLine("[{0}] DebugInfo.ClassName", DebugInfo.ClassName);
            Console.WriteLine("[{0}] DebugInfo.FullClassName", DebugInfo.FullClassName);

            // Assertion (Partial)
            Assert.AreEqual("DebugInfo_Properties_DefaultTest", DebugInfo.Name);
            Assert.AreEqual("DebugInfoTests.DebugInfo_Properties_DefaultTest", DebugInfo.ShortName);
            Assert.AreEqual("BUILDLet.Utilities.Diagnostics.Tests.DebugInfoTests.DebugInfo_Properties_DefaultTest", DebugInfo.FullName);
            Assert.AreEqual("DebugInfoTests", DebugInfo.ClassName);
            Assert.AreEqual("BUILDLet.Utilities.Diagnostics.Tests.DebugInfoTests", DebugInfo.FullClassName);
        }



        [TestMethod()]
        [TestCategory("MANUAL")]
        public void DebugInfo_ToString_LegacyTest()
        {
            // Init
            DebugInfo.Init();
            Console.WriteLine("DebugInfo.Init();  //Initialize");

            // Default
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //Default", DebugInfo.ToString());


            // Thread.Sleep(500); // Wait 500 ms
            // DebugInfo.AppendMargin = true;
            // Console.WriteLine(DebugInfo.ToString() + "AppendMargin=true");


            Thread.Sleep(500); // Wait 500 ms
            DebugInfo.DateTimeFormat = "MMMM dd, yyyy";
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DateTimeFormat=\"MMMM dd, yyyy\"", DebugInfo.ToString());


            Thread.Sleep(500); // Wait 500 ms
            DebugInfo.CallerFormat = DebugInfoCallerFormat.ClassName;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //CallerFormat=ClassName", DebugInfo.ToString());


            // Thread.Sleep(500); // Wait 500 ms
            // DebugInfo.CallerFormat = DebugInfoCallerFormat.None;
            // Console.WriteLine(DebugInfo.ToString() + "CallerFormat=None");


            Thread.Sleep(500); // Wait 500 ms
            DebugInfo.DateTimeFormat = null;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DateTimeFormat=null", DebugInfo.ToString());


            // Init
            DebugInfo.Init();
            Console.WriteLine();
            Console.WriteLine("DebugInfo.Init();  //Initialize");

            // Default
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //Default", DebugInfo.ToString());


            Thread.Sleep(500); // Wait 500 ms
            DebugInfo.DateTimeFormat = string.Empty;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //DateTimeFormat=string.Empty", DebugInfo.ToString());


            Thread.Sleep(500); // Wait 500 ms
            DebugInfo.CallerFormat = DebugInfoCallerFormat.FullName;
            Console.WriteLine("DebugInfo.ToString()=\"{0}\"  //CallerFormat=FullName", DebugInfo.ToString());
        }
    }
}
