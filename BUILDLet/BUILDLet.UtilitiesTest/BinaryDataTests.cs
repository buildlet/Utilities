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
            int[] sizes = { 100, 10, (500 * (int)Math.Pow(1000, 2)), 999 };
            int[] patterns = { -1, 100, -1, 32 };

            Log log = new Log(false, title: true);

            BinaryData bin;
            byte[] data;


            for (int i = 0; i < sizes.Length; i++)
            {
                log.WriteLine();

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
                log.WriteLine("(size={0},pattern={1})", sizes[i], bin.PatternLength);


                if (sizes[i] > 1000)
                {
                    log.WriteLine("Size of data is over than 1KB.");
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
                        log.WriteLine(hex.ToString());
                    }
                }
            }
        }


        [TestMethod()]
        public void BinaryData_ToFileTest_DEFAULT()
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
