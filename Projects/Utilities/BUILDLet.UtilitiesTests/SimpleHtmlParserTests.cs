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
using System.Runtime.CompilerServices;

using BUILDLet.Utilities;
using BUILDLet.Utilities.Diagnostics;
using BUILDLet.Utilities.Tests.Properties;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class SimpleHtmlParserTests
    {
        private static readonly string filepath_HTML_4_01_Specification = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_4_01_Specification);
        private static readonly string filepath_HTML_W3C_Example = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_W3C_Example);
        private static readonly string filepath_HTML_ToHoHo = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_ToHoHo_Sample);

        private static readonly string filepath_HTML_TestFile1 = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_TestFile1);
        private static readonly string filepath_HTML_TestFile2_STRICT = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_TestFile2_STRICT);

        private static readonly string filepath_HTML_CommentTest = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_CommentTest);
        private static readonly string filepath_HTML_TreeTest = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_TreeTest);
        private static readonly string filepath_HTML_TestFile_AttributeTest = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_AttributeTest);



        [TestMethod()]
        public void SimpleHtmlParser_RemoveHtmlComment_Test()
        {
            object[,] parameters =
            {
                { SimpleHtmlParserTests.filepath_HTML_CommentTest, Encoding.Default, 7 },
                { SimpleHtmlParserTests.filepath_HTML_4_01_Specification, Encoding.GetEncoding("ISO-8859-1"), 7 }
            };


            DebugInfo.Init();

            for (int i = 0; i < parameters.Length / 3; i++)
            {
                string filename = (string)parameters[i, 0];
                Encoding encoding = (Encoding)parameters[i, 1];
                int expected = (int)parameters[i, 2];

                string content = File.ReadAllText(filename, encoding);

                // Set Output Encoding
                Console.OutputEncoding = encoding;

                // Console Output
                Console.WriteLine("Target File[{0}]=\"{1}\"", i, filename);
                Console.WriteLine("Content of \"{0}\" is the following.", filename);
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine(content);
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine();

                // Test
                int actual = SimpleHtmlParser.RemoveHtmlComment(ref content);

                // Console Output
                Console.WriteLine("SimpleHtmlParser.RemoveHtmlComment()={0}", actual);
                Console.WriteLine("Content of \"{0}\" is the following.", filename);
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine(content);
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();

                // Assertion
                Assert.AreEqual(expected, actual);
            }
        }



        [TestMethod()]
        public void SimpleHtmlParser_DebugPrintLevel_Test()
        {
#if DEBUG
            for (int i = 0; i < 3; i++)
            {
                SimpleHtmlParser.DebugPrintLevel = i;

                Debug.WriteLine("");
                Debug.WriteLine("########################################");
                Debug.WriteLine(string.Format("SimpleHtmlParser.DebugPrintLevel={0}", SimpleHtmlParser.DebugPrintLevel));
                Debug.WriteLine("########################################");

                string content = File.ReadAllText(SimpleHtmlParserTests.filepath_HTML_W3C_Example, Encoding.Default);
                SimpleHtmlParser.GetElements(content, "P", null, false);
            }
#endif
        }



        [TestMethod()]
        public void SimpleHtmlParser_GetElements_Test()
        {
#if DEBUG
            // Set Debug Print Flag
            SimpleHtmlParser.DebugPrintLevel = 2;
#endif

            object[,] parameters =
            {
                // HTML File Name, Encoding, STRICT Mode, Element Name, Attributes, Expected

                // W3C Example (NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_W3C_Example, Encoding.Default, false, "TITLE", null, new string[] { "My first HTML document" } },
                { SimpleHtmlParserTests.filepath_HTML_W3C_Example, Encoding.Default, false, "P", null, new string[] { "Hello world!" } },

                // W3C Example (STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_W3C_Example, Encoding.Default, true, "P", null, null },

                // ToHoHo (Only NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_ToHoHo, Encoding.UTF8, false, "P", null, new string[] { "本文" } },

                // HTML 4.01 Specification (Only NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_4_01_Specification, Encoding.GetEncoding("ISO-8859-1"), false, "H2", null,
                    new string[]
                    {
                        "W3C Recommendation 24 December 1999",
                        "Abstract",
                        "Status of this document",
                        "<A name=\"minitoc\">Quick Table of Contents</A>",
                        "<A name=\"toc\">Full Table of Contents</A>"
                    }
                },

                // Comment Test File (STRICT and NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_CommentTest, Encoding.UTF8, true, "p", null, new string[] { "Hello, world!" } },
                { SimpleHtmlParserTests.filepath_HTML_CommentTest, Encoding.UTF8, false, "p", null, new string[] { "Hello, world!" } },
                { SimpleHtmlParserTests.filepath_HTML_CommentTest, Encoding.UTF8, true, "TR", null, new string[] { "<TD>Table</TD>" } },
                { SimpleHtmlParserTests.filepath_HTML_CommentTest, Encoding.UTF8, false, "TR", null, new string[] { "<TD>Table</TD>" } },

                // Tree (STRICT and NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.UTF8, true, "P", null, new string[] { "P Test" } },
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.UTF8, false, "P", null, new string[] { "P Test" } },


                // Attribute Test (STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_TestFile2_STRICT, Encoding.Default, true, "P",
                    new string[,]
                    {
                        { "class", "ver" }
                    },
                    new string[] { "Version 1.00" }
                },
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.UTF8, true, "P", 
                    new string[,]
                    {
                        { "class", "test" }
                    },
                    new string[] { "P Test" }
                },


                // Attribute Test (NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_TestFile1, Encoding.Default, false, "P",
                    new string[,]
                    {
                        { "class", "ver" }
                    },
                    new string[] { "Version 1.00" }
                },
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.UTF8, false, "P", 
                    new string[,]
                    {
                        {"class", "test"}
                    },
                    new string[] { "P Test" }
                },


                // Multiple Attributes Test (STRICT and NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.Default, true, "A",
                    new string[,]
                    {
                        { "class", "test" },
                        { "href", "http://www.dummy.local" }
                    },
                    new string[] { "Multiple Attributes Test" }
                },
                { SimpleHtmlParserTests.filepath_HTML_TreeTest, Encoding.Default, false, "A",
                    new string[,]
                    {
                        { "class", "test" },
                        { "href", "http://www.dummy.local" }
                    },
                    new string[] { "Multiple Attributes Test" }
                },


                // HTML 4.01 Specification (Only NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_4_01_Specification, Encoding.GetEncoding("ISO-8859-1"), false, "A",
                    new string[,]
                    {
                        { "class", "informref" },
                        { "href", "http://www.w3.org/TR/html4/references.html#ref-RFC1866" },
                        { "rel", "biblioentry" }
                    },
                    new string[] { "[RFC1866]" }
                },

                // Complex Test (HTML 4.01 Specification / Only NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_4_01_Specification, Encoding.GetEncoding("ISO-8859-1"), false, "A",
                    new string[,]
                    {
                        { "href", "http://www.w3.org/TR" },
                    },
                    new string[]
                    {
                        "W3C Technical Reports",
                        "http://www.w3.org/TR"
                    }
                },

                // Attribute Test (STRICT / and NOT STRICT Mode)
                { SimpleHtmlParserTests.filepath_HTML_TestFile_AttributeTest, Encoding.Default, true, "P",
                    new string[,]
                    {
                        { "attrib1", "test1" },
                        { "attrib2", "test2" },
                        { "attrib3", "test3" }
                    },
                    new string[] { "P1", "P2", "P3" }
                },
                { SimpleHtmlParserTests.filepath_HTML_TestFile_AttributeTest, Encoding.Default, false, "P",
                    new string[,]
                    {
                        { "attrib1", "test1" },
                        { "attrib2", "test2" },
                        { "attrib3", "test3" }
                    },
                    new string[] { "P1", "P2", "P3" }
                },
            };


            DebugInfo.Init();

            for (int i = 0; i < parameters.Length / 6; i++)
            {
                string filename =(string)parameters[i, 0];
                Encoding encoding = (Encoding)parameters[i, 1];
                bool strict_mode = (bool)parameters[i, 2];
                string element_name = (string)parameters[i, 3];

                string html_content = File.ReadAllText(filename, encoding);

                // Attribute(s)
                string[,] attributes = (string[,])parameters[i, 4];

                // Expected and Actual
                string[] expected = (string[])parameters[i, 5];
                string[] actual = null;


                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Target File=\"{1}\" }}", i, filename);

                try
                {
                    // Console Output
                    Console.Write("[{0}] SimpleHtmlParser.GetElements(name={1}", DebugInfo.Time, element_name);
                    if (attributes == null)
                    {
                        Console.Write(", attributes=null");
                    }
                    else
                    {
                        for (int j = 0; j < attributes.Length / 2; j++)
                        {
                            Console.WriteLine(",");
                            Console.Write("  attributes[{0}] \"{1}\"=\"{2}\"", j, attributes[j, 0], attributes[j, 1]);
                        }
                    }
                    Console.WriteLine(", strict={0})", strict_mode);


                    // Test
                    actual = SimpleHtmlParser.GetElements(html_content, element_name, attributes, strict_mode);


                    // Assertion (Length)
                    Assert.AreEqual(expected.Length, actual.Length);

                    for (int j = 0; j < expected.Length; j++)
                    {
                        // Console Output
                        Console.WriteLine("[{0}] Found[{1}]=\"{2}\"", DebugInfo.Time, j, actual[j]);

                        // Assertion
                        Assert.AreEqual(expected[j], actual[j]);
                    }
                }
                catch (InvalidDataException e)
                {
                    // Console Output
                    Console.WriteLine("[{0}] InvalidDataException (\"{1}\") was thrown.", DebugInfo.Time, e.Message);

                    // Assertion (Exception)
                    if (expected != null) { Assert.Fail(e.Message); }
                }

                Console.WriteLine();
            }
        }
    }
}
