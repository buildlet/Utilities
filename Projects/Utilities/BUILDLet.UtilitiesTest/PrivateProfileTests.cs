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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class PrivateProfileTests
    {
        private struct testcase
        {
            public string filepath;
            public string section;
            public string key;
            public string value;
            public string expected;
        }


        [TestMethod()]
        public void PrivateProfile_GetStringTest()
        {
            testcase[] testcases = new testcase[6];

            for (int i = 0; i < testcases.Length; i++)
			{
                testcases[i].filepath = Path.Combine(LocalPath.TestDataFolder, "test.ini");
                testcases[i].value = null;

                switch (i)
                {
                    case 0:
                        testcases[i].section = "Section2";
                        testcases[i].key = "Key4";
                        testcases[i].expected = "Value4";
                        break;

                    case 1:
                        testcases[i].section = "Section2";
                        testcases[i].key = "Key3";
                        testcases[i].expected = "Value3";
                        break;

                    case 2:
                        testcases[i].section = "Section4";
                        testcases[i].key = "Key5";
                        testcases[i].expected = "Value5.1,5.2,5.3";
                        break;

                    case 3:
                        testcases[i].section = "Section4";
                        testcases[i].key = "Key7";
                        testcases[i].expected = "1, 2, 3";
                        break;

                    case 4:
                        testcases[i].section = "Section4";
                        testcases[i].key = "Key8";
                        testcases[i].expected = string.Empty;
                        break;

                    case 5:
                        testcases[i].section = "Section5";
                        testcases[i].key = "Key11";
                        testcases[i].expected = ",,X";
                        break;

                    default:
                        break;
                }

                // print Trace message
                Log.WriteLine(stream: LogOutputStream.Trace);
                Log.WriteLine(string.Format("Check Key name \"{0}\" of Section [{1}] ({2}).",
                    testcases[i].key, testcases[i].section, testcases[i].filepath), stream: LogOutputStream.Trace);

                // Test
                string actual = PrivateProfile.GetString(testcases[i].section, testcases[i].key, testcases[i].filepath);
                
                // Assertion
                Assert.AreEqual(testcases[i].expected, actual);
            }
        }

        [TestMethod()]
        public void PrivateProfile_GetString_NotFoundTest()
        {
            string[] contents = File.ReadAllLines(Path.Combine(LocalPath.TestDataFolder, "test.ini")).ToArray();
            Assert.IsNull(PrivateProfile.GetString("dummy", "dummy", contents));
        }

        [TestMethod()]
        public void PrivateProfile_GetString_NoValueTest()
        {
            string[] contents = File.ReadAllLines(Path.Combine(LocalPath.TestDataFolder, "test.ini")).ToArray();
            Assert.AreEqual(string.Empty, PrivateProfile.GetString("Section4", "Key9", contents));
        }



        [TestMethod()]
        public void PrivateProfile_SetStringTest()
        {
            testcase[] testcases = new testcase[4];

            for (int i = 0; i < testcases.Length; i++)
            {
                testcases[i].filepath = Path.Combine(LocalPath.TestDataFolder, "test.ini");

                switch (i)
                {
                    case 0:
                        testcases[i].section = "Section2";
                        testcases[i].key = "Key4";
                        testcases[i].value = "UpdatedValue";
                        testcases[i].expected = "UpdatedValue";
                        break;

                    case 1:
                        testcases[i].section = "Section3";
                        testcases[i].key = "Key100";
                        testcases[i].value = "AddedValue";
                        testcases[i].expected = "AddedValue";
                        break;

                    case 2:
                        testcases[i].section = "Section100";
                        testcases[i].key = "Key1";
                        testcases[i].value = "AppendedValue";
                        testcases[i].expected = "AppendedValue";
                        break;

                    case 3:
                        testcases[i].section = "Section3";
                        testcases[i].key = "Key101";
                        testcases[i].value = "1,2,3";
                        testcases[i].expected = "1,2,3";
                        break;

                    default:
                        break;
                }

                // print Trace message
                Log.WriteLine(stream: LogOutputStream.Trace);
                Log.WriteLine(string.Format("Check Key name \"{0}\" of Section [{1}] ({2}).",
                    testcases[i].key, testcases[i].section, testcases[i].filepath), stream: LogOutputStream.Trace);

                // Test
                string[] contents = File.ReadLines(testcases[i].filepath).ToArray();
                PrivateProfile.SetString(testcases[i].section, testcases[i].key, testcases[i].value, ref contents);

                // get updated value
                string actual = PrivateProfile.GetString(testcases[i].section, testcases[i].key, contents);

                // Assertion
                Assert.AreEqual(testcases[i].expected, actual);
            }
        }
        
        [TestMethod()]
        public void PrivateProfile_SetStringTest2()
        {
            testcase[] testcases = new testcase[3];

            // Renew (Delete & Copy) Test File
            string inifile = "copy.ini";
            if (File.Exists(inifile)) { File.Delete(inifile); }
            File.Copy(Path.Combine(LocalPath.TestDataFolder, "test.ini"), inifile);

            for (int i = 0; i < testcases.Length; i++)
            {
                testcases[i].filepath = inifile;

                switch (i)
                {
                    case 0:
                        testcases[i].section = "Section2";
                        testcases[i].key = "Key4";
                        testcases[i].value = "UpdatedValue";
                        testcases[i].expected = "UpdatedValue";
                        break;

                    case 1:
                        testcases[i].section = "Section100";
                        testcases[i].key = "Key1";
                        testcases[i].value = "AppendedValue";
                        testcases[i].expected = "AppendedValue";
                        break;

                    case 2:
                        testcases[i].section = "Section4";
                        testcases[i].key = "Key10";
                        testcases[i].value = "AddedValue";
                        testcases[i].expected = "AddedValue";
                        break;

                    default:
                        break;
                }
                
                // print Trace message
                Log.WriteLine(stream: LogOutputStream.Trace);
                Log.WriteLine(string.Format("Check Key name \"{0}\" of Section [{1}] ({2}).",
                    testcases[i].key, testcases[i].section, testcases[i].filepath), stream: LogOutputStream.Trace);

                // Test
                PrivateProfile.SetString(testcases[i].section, testcases[i].key, testcases[i].value, testcases[i].filepath);

                // get updated value
                string actual = PrivateProfile.GetString(testcases[i].section, testcases[i].key, testcases[i].filepath);

                // Assertion
                Assert.AreEqual(testcases[i].expected, actual);
            }
        }

    }
}
