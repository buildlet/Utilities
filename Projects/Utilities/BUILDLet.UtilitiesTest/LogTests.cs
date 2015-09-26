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
    public class LogTests
    {
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Log_TimeStampFormat_ExceptionTest1()
        {
            Log.WriteLine("TimeStampFormat Exception Test", format: "★");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Log_TimeStampFormat_ExceptionTest2()
        {
            Log.WriteLine("TimeStampFormat Exception Test", true, true, format: "★");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Log_Bracket_ExceptionTest()
        {
            Log.WriteLine("Bracket Exception Test", true, true, bracket: new char[] { '1', '2', '3' });
        }


        [TestMethod()]
        public void Log_WriteTest()
        {
            Log.Clear();
            Log.Write("Write Test1");
            Log.Write("+Write Test2", false);
            Log.Write("+Write Test3");
        }

        [TestMethod()]
        public void Log_WriteLineTest()
        {
            Log.Clear();
            Log.WriteLine("WriteLine Test");

            Log.WriteLine();
            Log.WriteLine("WriteLine Test1", false, false);
            Log.WriteLine("WriteLine Test2", false, true);
            Log.WriteLine("WriteLine Test3", true, false);
            Log.WriteLine("WriteLine Test4", true, true);

            Log.WriteLine();
            Log.WriteLine("WriteLine Test5", true, true, "G");
            Log.WriteLine("WriteLine Test6");
            Log.WriteLine("WriteLine Test7");
        }


        [TestMethod()]
        public void Log_BracketTest1()
        {
            Log.Clear();
            Log.WriteLine("Bracket should be changed into '!' and '|'.");
            Log.WriteLine("Bracket Test1-1", bracket: new char[] { '!', '|' });
            Log.WriteLine("Bracket Test1-2");
        }

        [TestMethod()]
        public void Log_BracketTest2()
        {
            Log.Clear();
            Log.WriteLine("Bracket should be changed into '★' and '☆'.");
            Log.WriteLine("Bracket Test2-1", true, true, bracket: new char[] { '★', '☆' });
            Log.WriteLine("Bracket Test2-2");
        }

        [TestMethod()]
        public void Log_TimeStampFormatTest()
        {
            Log.Clear();
            Log.WriteLine("TimeStampFormat should be changed into \"G\".");
            Log.WriteLine("TimeStampFormat Test1", false, true, format: "G");
            Log.WriteLine("TimeStampFormat Test2");
        }

        [TestMethod()]
        public void Log_LogOutputStreamTest()
        {
            Log.WriteLine("This message should be shown to \"Standard Output Stream\".", stream: LogOutputStream.StandardOutput);
            Log.WriteLine("This message should be shown to \"Standard Output Stream\". (again)");

            Log.WriteLine("This message should be shown to \"Standard Error Output Stream\".", stream: LogOutputStream.StandardError);
            Log.WriteLine("This message should be shown to \"Standard Error Output Stream\". (again)");

            Log.WriteLine("This message should be shown to \"Trace Listner\".", stream: LogOutputStream.Trace);
            Log.WriteLine("This message should be shown to \"Trace Listner\". (again)");
        }

        [TestMethod()]
        public void Log_StaticPropertiesTest()
        {
            Log.Clear();
            Log.WriteLine("Default");

            Log.WriteLine("TimeStampFormat and Bracket should be changed into \"yyyy/MM/dd\" and { '【', '】' }.");
            Log.WriteLine("Method Name and Time Stamp should be shown (without parameters).");
            Log.WriteLine("Test 1-0", true, true, "yyyy/MM/dd", new char[] { '【', '】' });

            Log.WriteLine("Test 1-1");
            Log.WriteLine("Test 1-2");
            Log.WriteLine();


            Log.Clear();
            Log.WriteLine("Default");

            Log.WriteLine("TimeStampFormat and Bracket should be changed into \"yyyy/MM/dd\" and { '【', '】' }.");
            Log.TimeStampFormat = "yyyy/MM/dd";
            Log.Bracket = new char[] { '【', '】' };

            Log.WriteLine("Test 2-1", true, true);
            Log.WriteLine("Test 2-2", true, true);
            Log.WriteLine();


            Log.Clear();
            Log.WriteLine("Default");

            Log.WriteLine("Method Name should not be shown (without parameter).");
            Log.MethodName = false;
            Log.WriteLine("Time Stamp should be shown (without parameter).");
            Log.TimeStamp = true;

            Log.WriteLine("Test 3-1");
            Log.WriteLine("Test 3-2");
            Log.WriteLine();

            Log.Clear();
            Log.WriteLine("Default");
        }
    }
}
