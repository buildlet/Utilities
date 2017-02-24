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
using System.Diagnostics;
using System.IO;

using BUILDLet.Utilities.Tests;
using BUILDLet.Utilities.Tests.Properties;
using BUILDLet.Utilities.Diagnostics;

namespace BUILDLet.Utilities.Cryptography.Tests
{
    [TestClass()]
    public class HashCodeTests
    {
        private static readonly string filepath_fciv = Resources.LOCAL_FilePath_FCIV;
        private static readonly string filename_default_bin = "default.bin";
        private static readonly string filename_1GB_bin = "1GB.bin";


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void HashCode_HashName_ExceptionTest()
        {
            byte[] dummy = new byte[] { 0x00, 0xFF };
            new HashCode(dummy, "hoge");
        }


        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void HashCode_FileNotFoundExceptionTest()
        {
            new HashCode(@"dummy");
        }


        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void HashCode_StringEmpty_ExceptionTest()
        {
            new HashCode(string.Empty);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HashCode_Null_ExceptionTest()
        {
            byte[] dummy = null;
            new HashCode(dummy);
        }


        [TestMethod()]
        [TestCategory("LONG")]
        [TestCategory("BIG")]
        public void HashCode_ToString_Test()
        {
            object[,] parameters =
            {
                { HashCodeTests.filename_default_bin, 1024, 1, "MD5" },
                { HashCodeTests.filename_1GB_bin, (int)Math.Pow(1024, 2), 1024, "MD5" }
            };


            DebugInfo.Init();

            for (int i = 0; i < parameters.Length / 4; i++)
            {
                string filename = (string)parameters[i, 0];
                int size = (int)parameters[i, 1];
                int count = (int)parameters[i, 2];
                string hashName = (string)parameters[i, 3];

                string expected;
                string actual;

                DateTime start;
                DateTime end;


                // Console Output
                Console.WriteLine("Taregt File[{0}]=\"{1}\"", i, filename);
                Console.WriteLine();

                // Validation of file existence
                if (!File.Exists(filename))
                {
                    Console.WriteLine("[{0}] \"{1}\" does not exist.", DebugInfo.Time, filename);

                    // Create target file
                    new BinaryData(size).ToFile(count, filename);

                    Console.WriteLine("[{0}] \"{1}\" has been created.", DebugInfo.Time, filename);
                    Console.WriteLine();
                }



                // FCIV
                Process p = Process.Start(filepath_fciv);
                p.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Arguments = string.Format("/c {0} \"{1}\"", filepath_fciv, filename);

                // Run FCIV.exe
                Console.WriteLine("[{0:HH:mm:ss}] FCIV: Start (File=\"{1}\", Size={2}, Hash Name=\"{3}\")", (start = DateTime.Now), filename, size * count, hashName);
                p.Start();
                p.WaitForExit();

                // Store Standard Output
                string fciv_stdout = p.StandardOutput.ReadToEnd();

                // Exit FCIV.exe
                p.Close();
                Console.WriteLine("[{0:HH:mm:ss}] FCIV: End (Elapsed Time={1})", (end = DateTime.Now), end - start);
                Console.WriteLine();

                // extract message of hash value_found (FCIV)
                expected = fciv_stdout.Split(new string[] { "\r\n" }, StringSplitOptions.None)[3].Split(' ')[0].ToUpper();



                // HashCode
                Console.WriteLine("[{0:HH:mm:ss}] HashCode: Start (File=\"{1}\", Size={2}, Hash Name=\"{3}\")", (start = DateTime.Now), filename, size * count, hashName);
                if (string.IsNullOrEmpty(hashName))
                {
                    // Compute Hash
                    actual = new HashCode(filename).ToString();
                }
                else
                {
                    // Compute Hash
                    actual = new HashCode(filename, hashName).ToString();
                }
                Console.WriteLine("[{0:HH:mm:ss}] HashCode: End (Elapsed Time={1})", (end = DateTime.Now), end - start);
                Console.WriteLine();

                
                Console.WriteLine("{0}=Expected (FCIV)", expected);
                Console.WriteLine("{0}=Actual", actual);
                Console.WriteLine();
                Console.WriteLine();


                // Check result
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
