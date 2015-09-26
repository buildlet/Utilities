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
using BUILDLet.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void BinaryData_CreateTest()
        {
            int[] sizes =
            {
                100,
                10,
                (500 * (int)Math.Pow(1000, 2)),
                999
            };

            int[] patterns =
            {
                -1,   // to use default
                100,
                -1,   // to use default
                32
            };

            BinaryData bin;
            byte[] data;


            for (int i = 0; i < sizes.Length; i++)
            {
                Log.Clear();
                Log.WriteLine();

                // Sleep (for Randum() function in BinaryData class)
                System.Threading.Thread.Sleep(100);

                // new BinaryData
                if (patterns[i] > 0)
                {
                    bin = new BinaryData(sizes[i], patterns[i]);
                }
                else
                {
                    bin = new BinaryData(sizes[i]);
                }
                Log.WriteLine(string.Format("(size={0},pattern={1})", sizes[i], bin.PatternLength));


                if (sizes[i] > 1000)
                {
                    Log.WriteLine("Size of data is over than 1KB.");
                }
                else
                {
                    data = bin.GetBytes();

                    StringBuilder hex = new StringBuilder();
                    int total = 0;
                    while (total < sizes[i])
                    {
                        hex.Clear();
                        for (int j = 0; j < bin.PatternLength; total++, j++)
                        {
                            if (total < sizes[i]) { hex.Append(string.Format("{0:X2} ", data[total])); }
                        }
                        Log.WriteLine(hex.ToString(), false);
                    }
                }
            }
        }


        [TestMethod()]
        public void BinaryData_ToFileTest()
        {
            BinaryData bin = new BinaryData();
            bin.ToFile(@".\default.bin");
        }



        [TestMethod()]
        public void LONG_BIG_BinaryData_ToFileTest()
        {
            new BinaryData().ToFile(@".\big.bin", 3 * (long)Math.Pow(1000, 3));
        }
    }
}
