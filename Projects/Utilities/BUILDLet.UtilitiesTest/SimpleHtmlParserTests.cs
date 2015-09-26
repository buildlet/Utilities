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
using System.Runtime.CompilerServices;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class SimpleHtmlParserTests
    {
        private struct testcase
        {
            public string filename;
            public Encoding encoding;
            public bool strict;
            public string tag;
            public string attr;
            public string[] expected;
        }


        [TestMethod()]
        public void SimpleHtmlParser_GetElementsTest()
        {
            testcase[] testcases = new testcase[8];

            for (int i = 0; i < testcases.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        // W3C Example (1)
                        testcases[i].filename = "W3C_example.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = false;
                        testcases[i].tag = "TITLE";
                        testcases[i].expected = new string[] { "My first HTML document" };
                        break;

                    case 1:
                        // W3C Example (2)
                        testcases[i].filename = "W3C_example.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = false;
                        testcases[i].tag = "P";
                        testcases[i].expected = new string[] { "Hello world!" };
                        break;

                    case 2:
                        // とほほ HTMLの究極のサンプル
                        testcases[i].filename = "とほほ_サンプル.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = true;
                        testcases[i].tag = "P";
                        testcases[i].expected = new string[] { "本文" };
                        break;

                    case 3:
                        // HTML 4.01 Specification (W3C Recommendation)
                        testcases[i].filename = "HTML 4_01 Specification.htm";
                        testcases[i].encoding = Encoding.Default;
                        testcases[i].strict = true;
                        testcases[i].tag = "H2";
                        testcases[i].expected = new string[] {
                            "W3C Recommendation 24 December 1999",
                            "Abstract",
                            "Status of this document",
                            "<A name=\"minitoc\">Quick Table of Contents</A>",
                            "<A name=\"toc\">Full Table of Contents</A>"
                        };
                        break;

                    case 4:
                        // Comment Test (1)
                        testcases[i].filename = "comment.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = true;
                        testcases[i].tag = "p";
                        testcases[i].expected = new string[] { "Hello, world!" };
                        break;

                    case 5:
                        // Comment Test (2)
                        testcases[i].filename = "comment.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = false;
                        testcases[i].tag = "p";
                        testcases[i].expected = new string[] { "Hello, world!" };
                        break;

                    case 6:
                        // Comment Test (1)
                        testcases[i].filename = "comment.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = true;
                        testcases[i].tag = "TR";
                        testcases[i].expected = new string[] { "<TD>Table</TD>" };
                        break;

                    case 7:
                        // Comment Test (2)
                        testcases[i].filename = "comment.html";
                        testcases[i].encoding = Encoding.UTF8;
                        testcases[i].strict = false;
                        testcases[i].tag = "TR";
                        testcases[i].expected = new string[] { "" };
                        break;

                    default:
                        break;
                }
            }

            // Test
            this.innerTest_SimpleHtmlParser(testcases);
        }


        [TestMethod()]
        public void SimpleHtmlParser_GetAttributesTest()
        {
            testcase[] testcases = new testcase[1];

            // HTML 4.01 Specification (W3C Recommendation)
            testcases[0].filename = "HTML 4_01 Specification.htm";
            testcases[0].encoding = Encoding.Default;
            testcases[0].strict = true;
            testcases[0].tag = "OL";
            testcases[0].attr = "type";
            testcases[0].expected = new string[] {
                "\"A\"",
                "\"A\""
            };

            // Test
            this.innerTest_SimpleHtmlParser(testcases);
        }


        [TestMethod()]
        public void SimpleHtmlParser_GetAttributesTest2()
        {
            testcase[] testcases = new testcase[1];

            // HTML 4.01 Specification (W3C Recommendation)
            testcases[0].filename = "HTML 4_01 Specification.htm";
            testcases[0].encoding = Encoding.Default;
            testcases[0].strict = true;
            testcases[0].tag = "A";
            testcases[0].attr = "href";
            testcases[0].expected = null;

            // Initialize Log
            Log.Clear();
            Log.OutputStream = LogOutputStream.Trace;
            Log.WriteLine(string.Format("Test File is \"{0}\".", testcases[0].filename), true);

            string content = File.ReadAllText(Path.Combine(LocalPath.TestDataFolder, testcases[0].filename), testcases[0].encoding);
            string[] actual = SimpleHtmlParser.GetAttributes(content, testcases[0].tag, testcases[0].attr);

            // Output
            Log.WriteLine(string.Format("{0} of \"{1}\" attribute of <{2}> element is found.", actual.Length, testcases[0].attr, testcases[0].tag));
            for (int i = 0; i < actual.Length; i++) { Log.WriteLine(string.Format("({0}) \"{1}\"", i, actual[i])); }
        }


        private void innerTest_SimpleHtmlParser(testcase[] testcases, [CallerMemberName] string caller = "")
        {
            // Initialize Log
            Log.Clear();
            Log.MethodName = false;
            Log.OutputStream = LogOutputStream.Trace;

            foreach (var testcase in testcases)
            {
                string filepath = Path.Combine(LocalPath.TestDataFolder, testcase.filename);
                string[] actual;


                Log.WriteLine(string.Format("[{0}] Test File is \"{1}\".", caller, testcase.filename));
                if (string.IsNullOrEmpty(testcase.attr))
                {
                    actual = SimpleHtmlParser.GetElements(File.ReadAllText(filepath, testcase.encoding), testcase.tag, testcase.strict);

                    // Length should be the same.
                    Log.WriteLine(string.Format("[{0}] {1} of <{2}> is found.", caller, actual.Length, testcase.tag));
                    Assert.AreEqual(testcase.expected.Length, actual.Length);

                    for (int i = 0; i < testcase.expected.Length; i++)
                    {
                        Log.WriteLine(string.Format("[{0}] Value({1}) of <{2}> is \"{3}\".", caller, i, testcase.tag, actual[i]));
                        Assert.AreEqual(testcase.expected[i], actual[i]);
                    }
                }
                else
                {
                    actual = SimpleHtmlParser.GetAttributes(File.ReadAllText(filepath, testcase.encoding), testcase.tag, testcase.attr);

                    // Length should be the same.
                    Log.WriteLine(string.Format("[{0}] {1} of \"{2}\" attribute of <{3}> element is found.", caller, actual.Length, testcase.attr, testcase.tag));
                    Assert.AreEqual(testcase.expected.Length, actual.Length);

                    for (int i = 0; i < testcase.expected.Length; i++)
                    {
                        Log.WriteLine(string.Format("[{0}] Value({1}) of \"{2}\" attribute of <{3}> is \"{4}\".", caller, i, testcase.attr, testcase.tag, actual[i]));
                        Assert.AreEqual(testcase.expected[i], actual[i]);
                    }
                }
                Log.WriteLine();
            }
        }

        [TestMethod()]
        public void SimpleHtmlParser_RemoveHtmlCommentTest()
        {
            string content = File.ReadAllText(Path.Combine(LocalPath.TestDataFolder, "comment.html"));

            Log.Clear();
            Log.WriteLine("Before\n" + content);
            Assert.AreEqual(7, SimpleHtmlParser.RemoveHtmlComment(ref content));
            Log.WriteLine("After\n" + content);
        }
    }
}
