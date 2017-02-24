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
    public class SimpleHtmlParser_HtmlContentTests : SimpleHtmlParser
    {
        private static readonly string filepath_invalid_Hello = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_Hello);
        private static readonly string filepath_HTML_W3C_Example = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_HTML_W3C_Example);



        [TestMethod()]
        [ExpectedException(typeof(InvalidDataException))]
        public void SimpleHtmlParser_HtmlContent_InvalidDataExceptionTest()
        {
            new HtmlContent(File.ReadAllText(filepath_invalid_Hello));
        }



        [TestMethod()]
        public void SimpleHtmlParser_HtmlContent_FirstElement_Test()
        {
            object[,] parameters =
            {
                // STRICT Mode, HTML Content, Expected 1st Element Name, Expected 1st Element Value
                { false, File.ReadAllText(filepath_HTML_W3C_Example), "!DOCTYPE", string.Empty },
                { true,
@"<H1>Headline Level 1</H1>
<H2>Headline Level 2</H2>
<H3>Headline Level 3</H3>
<H4>Headline Level 4</H4>", "H1", "Headline Level 1" },
                { true,
@"<BODY>
    <DIV>
        <H1>Title</H1>
        <P>Test</P>
    </DIV>
</BODY>", "BODY", "<DIV>        <H1>Title</H1>        <P>Test</P>    </DIV>" },
                { false,
@"<BODY>
    <DIV>
        <H1>Title</H1>
        <P>Test
    </DIV>
</BODY>", "BODY", "<DIV>        <H1>Title</H1>        <P>Test    </DIV>" }
            };


            for (int i = 0; i < parameters.Length / 4; i++)
            {
                bool strict_mode = (bool)parameters[i, 0];
                string content = (string)parameters[i, 1];
                string expected_name = (string)parameters[i, 2];
                string expected_value = (string)parameters[i, 3];

                HtmlContent html_content = new HtmlContent(content, strict_mode);

                // Test
                string first_element_name = html_content.GetFirstElementName();
                string first_element_value = html_content.GetFirstElementValue(); //.Replace(" ", string.Empty);

                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Strict Mode={1} }}", i, strict_mode);
                Console.WriteLine("HtmlContent.GetFirstElementName()=\"{0}\"", first_element_name);
                Console.WriteLine("HtmlContent.GetFirstElementValue()=\"{0}\"", first_element_value);
                Console.WriteLine();

                // Assertion
                Assert.AreEqual(expected_name, first_element_name);
                Assert.AreEqual(expected_value, first_element_value);
            }
        }



        [TestMethod()]
        public void SimpleHtmlParser_HtmlContent_GetFirstElementAttributeValue_Test()
        {
            object[,] parameters =
            {
                // STRICT Mode, HTML Content, 1st Element Name, Attribute Name, Expected Attribute Value
                { true, "<P class=\"ver\">Version 1.00</P>", "class", "ver" },
                { false, "<P class=\"ver\">Version 1.00</P>", "class", "ver" },
                { false, "<P class=\"ver\">Version 1.00", "class", "ver" },
                { true, "<P class=\"ver\">Version 1.00</P></DUMMY>", "class", "ver" }
            };


            for (int i = 0; i < parameters.Length / 4; i++)
            {
                bool strict_mode = (bool)parameters[i, 0];
                string content = (string)parameters[i, 1];
                string attribute_name = (string)parameters[i, 2];
                string expected = (string)parameters[i, 3];

                HtmlContent html_content = new HtmlContent(content, strict_mode);

                // Test
                string actual = html_content.GetFirstElementAttributeValue(attribute_name);

                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Strict Mode={1} }}", i, strict_mode);
                Console.WriteLine("HtmlContent.GetFirstElementAttributeValue(\"{0}\")=\"{1}\"", attribute_name, actual);
                Console.WriteLine();

                // Assertion
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
