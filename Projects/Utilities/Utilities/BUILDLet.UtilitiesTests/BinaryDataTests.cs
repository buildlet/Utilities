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

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class BinaryDataTests
    {
        [TestMethod()]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void BinaryData_Create_OutOfMemoryExceptionTest()
        {
            new BinaryData(2 * (int)Math.Pow(1000, 3));
        }


        [TestMethod()]
        public void BinaryData_GetBytesTest()
        {
            int[,] parameters =
            {
                // {size, pattern}
                {-1, -1},
                {100, -1},
                {-1, 1024},
                {-1, 10},
                {10, 100},
                {(500 * (int)Math.Pow(1000, 2)), -1},
                {999, 32}
            };


            BinaryData bin;
            byte[] data;

            for (int i = 0; i < parameters.Length / 2; i++)
            {
                int size = parameters[i, 0];
                int pattern = parameters[i, 1];


                // Sleep (for Randum() function in BinaryData class)
                System.Threading.Thread.Sleep(100);


                // new BinaryData
                if ((size < 0) && (pattern < 0))
                {
                    // default
                    bin = new BinaryData();
                }
                else if (pattern < 0)
                {
                    bin = new BinaryData(size);
                }
                else if (size < 0)
                {
                    bin = new BinaryData(pattern:pattern);
                }
                else
                {
                    bin = new BinaryData(size, pattern);
                }


                // Console Output
                Console.WriteLine("Paramaters[{0}] {{ Size={1}, Pattern={2} }}", i, size, pattern);
                Console.WriteLine("BinaryData.GetBytes().Length={0}", bin.GetBytes().Length);
                Console.WriteLine("BinaryData.PatternLength={0}", bin.PatternLength);
                Console.WriteLine();

                if (size > 1024)
                {
                    Console.WriteLine("Size of data is over than 1KB.");
                }
                else
                {
                    data = bin.GetBytes();

                    StringBuilder hex = new StringBuilder();
                    int total = 0;
                    int line = 0;
                    while (total < data.Length)
                    {
                        hex.Clear();
                        for (int j = 0; j < bin.PatternLength; total++, j++)
                        {
                            if (total < data.Length)
                            {
                                hex.Append(string.Format("{0:X2} ", data[total]));
                            }
                        }
                        Console.WriteLine("[{0:D4}] {1}", line++, hex.ToString());
                    }
                }
                Console.WriteLine();
                Console.WriteLine();


                // Assertion
                Assert.AreEqual(((size > 0) ? size : new BinaryData().GetBytes().Length), bin.GetBytes().Length);
                Assert.AreEqual(((pattern > 0) ? pattern : new BinaryData().PatternLength), bin.PatternLength);
            }
        }


        [TestMethod()]
        [TestCategory("LONG")]
        [TestCategory("BIG")]
        public void BinaryData_ToFile_Test()
        {
            object[,] parameters =
            {
                // filepath, size, count
                {@".\default.bin", -1, -1},
                {@".\100bytes.bin", 100, -1},
                {@".\1GB.bin", (int)Math.Pow(1024, 2), 1024}
            };


            BinaryData bin;

            for (int i = 0; i < parameters.Length / 3; i++)
            {
                string filepath = (string)parameters[i, 0];
                int size = (int)parameters[i, 1];
                int count = (int)parameters[i, 2];


                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Path=\"{1}\", Size={2}, Count={3} }}", i, filepath, size, count);


                // Create new file
                if (size <= 0 && count <= 0)
                {
                    bin = new BinaryData();
                    bin.ToFile(filepath);
                }
                else if (count <= 0)
                {
                    bin = new BinaryData();
                    bin.ToFile(filepath, size);
                }
                else if (size <= 0)
                {
                    bin = new BinaryData();
                    bin.ToFile(count, filepath);
                }
                else
                {
                    bin = new BinaryData(size);
                    bin.ToFile(count, filepath);
                }

                long filesize = new FileInfo(filepath).Length;

                // Console Output
                Console.WriteLine("BinaryData.GetBytes().Length={0}", bin.GetBytes().Length);
                Console.WriteLine("BinaryData.PatternLength={0}", bin.PatternLength);
                Console.WriteLine("File Size={0}", filesize);
                Console.WriteLine();

                // Check File Size
                Assert.AreEqual((size > 0 ? size : bin.GetBytes().Length) * (count > 0 ? count : 1), filesize);
            }
        }
    }
}
