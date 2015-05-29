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
        public void HashCode_HashNameExceptionTest()
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
            Log log = new Log(false, true, true);
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
                log.WriteLine();
                log.WriteLine();
                log.WriteLine("Test File=\"{0}\", Hash Name=\"{1}\"", testFiles[i], hashNames[i]);



                // Output (FCIV: Start)
                log.WriteLine();
                log.WriteLine("FCIV: Start");
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
                log.WriteLine("FCIV: End ({0})", DateTime.Now - start);



                // Output (HashCode.ComputeHash: Start)
                log.WriteLine();
                log.WriteLine("HashCode.ComputeHash: Start");
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
                log.WriteLine("HashCode.ComputeHash: End ({0})", DateTime.Now - start);


                // Output
                log.WriteLine();
                log.WriteLine("(E:Expected (FCIV), A:Actual)");
                log.WriteLine("E:{0}", expected);
                log.WriteLine("A:{0}", actual);


                // Check result
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
