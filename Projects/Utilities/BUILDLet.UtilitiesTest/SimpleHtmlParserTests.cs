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

            Log log = new Log(false);

            Debug.Write("[" + testcases[0].filename + "]: ");
            string content = File.ReadAllText(Path.Combine(LocalPath.TestDataFolder, testcases[0].filename), testcases[0].encoding);
            string[] actual = SimpleHtmlParser.GetAttributes(content, testcases[0].tag, testcases[0].attr);

            // Output
            log.WriteLine("[{0}] {1} of \"{2}\" attribute of <{3}> element is found.",
                testcases[0].filename, actual.Length, testcases[0].attr, testcases[0].tag);
            for (int i = 0; i < actual.Length; i++) { log.WriteLine("({0}) \"{1}\"", i, actual[i]); }

        }


        private void innerTest_SimpleHtmlParser(testcase[] testcases)
        {
            Log log = new Log();

            foreach (var testcase in testcases)
            {
                string filepath = Path.Combine(LocalPath.TestDataFolder, testcase.filename);
                string[] actual;

                if (string.IsNullOrEmpty(testcase.attr))
                {
                    Debug.Write("[" + testcase.filename + "]: ");
                    actual = SimpleHtmlParser.GetElements(File.ReadAllText(filepath, testcase.encoding), testcase.tag, testcase.strict);

                    // Length should be the same.
                    log.WriteLine("({0}) {1} of <{2}> is found.", testcase.filename, actual.Length, testcase.tag);
                    Assert.AreEqual(testcase.expected.Length, actual.Length);

                    for (int i = 0; i < testcase.expected.Length; i++)
                    {
                        log.WriteLine("({0}) Value({1}) of <{2}> is \"{3}\".", testcase.filename, i, testcase.tag, actual[i]);
                        Assert.AreEqual(testcase.expected[i], actual[i]);
                    }
                }
                else
                {
                    Debug.Write("[" + testcase.filename + "]: ");
                    actual = SimpleHtmlParser.GetAttributes(File.ReadAllText(filepath, testcase.encoding), testcase.tag, testcase.attr);

                    // Length should be the same.
                    log.WriteLine("({0}) {1} of \"{2}\" attribute of <{3}> element is found.", testcase.filename, actual.Length, testcase.attr, testcase.tag);
                    Assert.AreEqual(testcase.expected.Length, actual.Length);

                    for (int i = 0; i < testcase.expected.Length; i++)
                    {
                        log.WriteLine("({0}) Value({1}) of \"{2}\" attribute of <{3}> is \"{4}\".", testcase.filename, i, testcase.attr, testcase.tag, actual[i]);
                        Assert.AreEqual(testcase.expected[i], actual[i]);
                    }
                }
                log.WriteLine();
            }
        }

        [TestMethod()]
        public void SimpleHtmlParser_RemoveHtmlCommentTest()
        {
            Log log = new Log();
            string content = File.ReadAllText(Path.Combine(LocalPath.TestDataFolder, "comment.html"));

            log.WriteLine("Before\n" + content);
            Assert.AreEqual(7, SimpleHtmlParser.RemoveHtmlComment(ref content));
            log.WriteLine("After\n" + content);
        }
    }
}
