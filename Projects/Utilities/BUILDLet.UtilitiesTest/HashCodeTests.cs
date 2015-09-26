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
using System.Diagnostics;

using BUILDLet.Utilities.Tests;


namespace BUILDLet.Utilities.Cryptography.Tests
{
    [TestClass()]
    public class HashCodeTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void HashCode_HashName_ExceptionTest()
        {
            byte[] dummy = new byte[] { 0x00, 0xFF };
            new HashCode(dummy, "hoge");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void HashCode_MD5_ExceptionTest1()
        {
            new HashCode(string.Empty);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HashCode_MD5_ExceptionTest2()
        {
            byte[] dummy = null;
            new HashCode(dummy);
        }

        [TestMethod()]
        public void LONG_BIG_HashCode_ToStringTest()
        {
            DateTime start;

            string fciv_path = LocalPath.FCIV;
            string fciv_stdout;

            string expected;
            string actual;


            string[] testFiles = 
            {
                "default.bin",
                "big.bin"
            };

            string[] hashNames =
            {
                "MD5",
                "MD5"
            };


            // Validation of file existence
            foreach (var file in testFiles)
            {
                if (!System.IO.File.Exists(file))
                {
                    switch (file)
                    {
                        case "default.bin":
                            new BinaryData().ToFile(file);
                            break;

                        case "big.bin":
                            new BinaryData().ToFile(file, 3 * (long)Math.Pow(1000, 3));
                            break;

                        default:
                            Assert.Inconclusive("Test File\"{0}\" is not founds.", file);
                            break;
                    }
                }
            }


            // Main
            for (int i = 0; i < testFiles.Length; i++)
            {
                // Output
                Log.Clear();
                Log.WriteLine(string.Format("Test File=\"{0}\", Hash Name=\"{1}\"", testFiles[i], hashNames[i]));



                // Output (FCIV: Start)
                Log.WriteLine();
                Log.WriteLine("FCIV: Start", false, true);
                start = DateTime.Now;

                // FCIV
                Process p = Process.Start(fciv_path);
                p.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Arguments = string.Format("/c {0} \"{1}\"", fciv_path, testFiles[i]);

                // Run FCIV.exe
                p.Start();

                // store stdard output
                fciv_stdout = p.StandardOutput.ReadToEnd();

                p.WaitForExit();
                p.Close();

                // extract message of hash value_found (FCIV)
                expected = fciv_stdout.Split(new string[] { "\r\n" }, StringSplitOptions.None)[3].Split(' ')[0].ToUpper();

                // Output (FCIV: End)
                Log.WriteLine(string.Format("FCIV: End ({0})", DateTime.Now - start));



                // Output (HashCode.ComputeHash: Start)
                Log.WriteLine();
                Log.WriteLine("HashCode.ComputeHash: Start");
                start = DateTime.Now;

                // compute Hash
                if (string.IsNullOrEmpty(hashNames[i]))
                {
                    actual = new HashCode(testFiles[i]).ToString();
                }
                else
                {
                    actual = new HashCode(testFiles[i], hashNames[i]).ToString();
                }

                // Output (HashCode.ComputeHash: End)
                Log.WriteLine(string.Format("HashCode.ComputeHash: End ({0})", DateTime.Now - start));


                // Output
                Log.WriteLine();
                Log.WriteLine(string.Format("(E:Expected (FCIV), A:Actual)"), false, false);
                Log.WriteLine(string.Format("E:{0}", expected));
                Log.WriteLine(string.Format("A:{0}", actual));
                Log.WriteLine();
                Log.WriteLine();


                // Check result
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
