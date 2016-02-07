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

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class TestLogTests
    {
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Log_TimeStampFormat_ExceptionTest1()
        {
            TestLog.WriteLine("TimeStampFormat Exception Test", format: "★");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Log_TimeStampFormat_ExceptionTest2()
        {
            TestLog.WriteLine("TimeStampFormat Exception Test", true, true, format: "★");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Log_Bracket_ExceptionTest()
        {
            TestLog.WriteLine("Bracket Exception Test", true, true, bracket: new char[] { '1', '2', '3' });
        }


        [TestMethod()]
        public void Log_WriteTest()
        {
            TestLog.Clear();
            TestLog.Write("Write Test1");
            TestLog.Write("+Write Test2", false);
            TestLog.Write("+Write Test3");
        }

        [TestMethod()]
        public void Log_WriteLineTest()
        {
            TestLog.Clear();
            TestLog.WriteLine("WriteLine Test");

            TestLog.WriteLine();
            TestLog.WriteLine("WriteLine Test1", false, false);
            TestLog.WriteLine("WriteLine Test2", false, true);
            TestLog.WriteLine("WriteLine Test3", true, false);
            TestLog.WriteLine("WriteLine Test4", true, true);

            TestLog.WriteLine();
            TestLog.WriteLine("WriteLine Test5", true, true, "G");
            TestLog.WriteLine("WriteLine Test6");
            TestLog.WriteLine("WriteLine Test7");
        }


        [TestMethod()]
        public void Log_BracketTest1()
        {
            TestLog.Clear();
            TestLog.WriteLine("Bracket should be changed into '!' and '|'.");
            TestLog.WriteLine("Bracket Test1-1", bracket: new char[] { '!', '|' });
            TestLog.WriteLine("Bracket Test1-2");
        }

        [TestMethod()]
        public void Log_BracketTest2()
        {
            TestLog.Clear();
            TestLog.WriteLine("Bracket should be changed into '★' and '☆'.");
            TestLog.WriteLine("Bracket Test2-1", true, true, bracket: new char[] { '★', '☆' });
            TestLog.WriteLine("Bracket Test2-2");
        }

        [TestMethod()]
        public void Log_TimeStampFormatTest()
        {
            TestLog.Clear();
            TestLog.WriteLine("TimeStampFormat should be changed into \"G\".");
            TestLog.WriteLine("TimeStampFormat Test1", false, true, format: "G");
            TestLog.WriteLine("TimeStampFormat Test2");
        }

        [TestMethod()]
        public void Log_LogOutputStreamTest()
        {
            TestLog.WriteLine("This message should be shown to \"Standard Output Stream\".", stream: TestLogOutputStream.StandardOutput);
            TestLog.WriteLine("This message should be shown to \"Standard Output Stream\". (again)");

            TestLog.WriteLine("This message should be shown to \"Standard Error Output Stream\".", stream: TestLogOutputStream.StandardError);
            TestLog.WriteLine("This message should be shown to \"Standard Error Output Stream\". (again)");

            TestLog.WriteLine("This message should be shown to \"Trace Listner\".", stream: TestLogOutputStream.Trace);
            TestLog.WriteLine("This message should be shown to \"Trace Listner\". (again)");
        }

        [TestMethod()]
        public void Log_StaticPropertiesTest()
        {
            TestLog.Clear();
            TestLog.WriteLine("Default");

            TestLog.WriteLine("TimeStampFormat and Bracket should be changed into \"yyyy/MM/dd\" and { '【', '】' }.");
            TestLog.WriteLine("Method Name and Time Stamp should be shown (without parameters).");
            TestLog.WriteLine("Test 1-0", true, true, "yyyy/MM/dd", new char[] { '【', '】' });

            TestLog.WriteLine("Test 1-1");
            TestLog.WriteLine("Test 1-2");
            TestLog.WriteLine();


            TestLog.Clear();
            TestLog.WriteLine("Default");

            TestLog.WriteLine("TimeStampFormat and Bracket should be changed into \"yyyy/MM/dd\" and { '【', '】' }.");
            TestLog.TimeStampFormat = "yyyy/MM/dd";
            TestLog.Bracket = new char[] { '【', '】' };

            TestLog.WriteLine("Test 2-1", true, true);
            TestLog.WriteLine("Test 2-2", true, true);
            TestLog.WriteLine();


            TestLog.Clear();
            TestLog.WriteLine("Default");

            TestLog.WriteLine("Method Name should not be shown (without parameter).");
            TestLog.MethodName = false;
            TestLog.WriteLine("Time Stamp should be shown (without parameter).");
            TestLog.TimeStamp = true;

            TestLog.WriteLine("Test 3-1");
            TestLog.WriteLine("Test 3-2");
            TestLog.WriteLine();

            TestLog.Clear();
            TestLog.WriteLine("Default");
        }
    }
}
