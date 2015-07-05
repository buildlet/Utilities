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
using System.IO;
using System.Diagnostics;

using BUILDLet.Utilities;

namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class LocalFileTests
    {
        [TestMethod()]
        public void LocalFile_SearchPathTest()
        {
            Log log = new Log(false, false, true);

            log.WriteLine("LocalFile.SearchPath={");
            foreach (var dir in LocalFile.SearchPath) { log.WriteLine("\t\"{0}\",", dir); }
            log.WriteLine("}");
        }


        private void test_GetSearchPath(string[] expected, string[] actual, string paramter)
        {
            this.test_GetSearchPath(expected, actual, new string[] { paramter });
        }


        private void test_GetSearchPath(string[] expected, string[] actual, string[] parameters)
        {
            Console.Write("LocalFile.GetSearchPath(");


            // Parameter(s)
            if (parameters.Length == 1 & string.IsNullOrEmpty(parameters[0])) { Console.Write(parameters[0]); }
            else
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Console.Write("\"{0}\"", parameters[i]);
                    if (i < parameters.Length - 1) { Console.Write(", "); }
                }
            }


            Console.Write(")=");


            // Return
            Console.WriteLine("{");
            for (int i = 0; i < actual.Length; i++)
            {
                Console.Write("\t\"{0}\"", actual[i]);
                if (i < actual.Length - 1) { Console.Write(","); }
                Console.WriteLine();
            }
            Console.WriteLine("}");


            // Check Length
            Assert.AreEqual(expected.Length, actual.Length);

            // Check Content
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }


        [TestMethod()]
        public void LocalFile_GetSearchPath_TestEmpty()
        {
            new Log().WriteLine("Empty(Current Directory)");

            List<string> expected = new List<string>();
            expected.Add(Environment.CurrentDirectory);
            expected.AddRange(LocalFile.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), LocalFile.GetSearchPath(), string.Empty);
        }

        [TestMethod()]
        public void LocalFile_GetSearchPath_InvalidDirectoryTest()
        {
            new Log().WriteLine("Invalid Directory");
            string dir = "hoge";

            this.test_GetSearchPath(LocalFile.SearchPath, LocalFile.GetSearchPath(dir), dir);
        }

        [TestMethod()]
        public void LocalFile_GetSearchPath_ValidDirectoryTest()
        {
            new Log().WriteLine("Valid Directory");
            string dir = @"C:\Users";

            List<string> expected = new List<string>();
            expected.Add(dir);
            expected.AddRange(LocalFile.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), LocalFile.GetSearchPath(dir), dir);
        }

        [TestMethod()]
        public void LocalFile_GetSearchPath_AlreadyIncludingDirectoryTest()
        {
            Log log = new Log();

            Dictionary<string, string> dirs = new Dictionary<string, string>();
            dirs.Add("Windows Folder", @"C:\Windows");
            dirs.Add("System32 Folder", @"C:\Windows\System32");

            foreach (var dir in dirs)
            {
                log.WriteLine();
                log.WriteLine(dir.Key);
                this.test_GetSearchPath(LocalFile.SearchPath, LocalFile.GetSearchPath(dir.Value), dir.Value);
            }
        }


        [TestMethod()]
        public void LocalFile_GetSearchPath_ValidDirectoriesTest()
        {
            new Log().WriteLine("Valid Directories");

            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.AddRange(dirs);
            expected.AddRange(LocalFile.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), LocalFile.GetSearchPath(dirs.ToArray()), dirs.ToArray());
        }

        [TestMethod()]
        public void LocalFile_GetSearchPath_ComplexTest()
        {
            new Log().WriteLine("Complex");

            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Windows");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.Add(@"C:\Users");
            expected.Add(@"C:\Program Files");
            expected.AddRange(LocalFile.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), LocalFile.GetSearchPath(dirs.ToArray()), dirs.ToArray());
        }



        private void test_GetFilePath(string filename, string[] folders)
        {
            if (folders == null || folders.Length == 0)
            {
                Console.WriteLine("FileAccess.GetFilePath(\"{0}\")=\"{1}\"", filename, LocalFile.GetFilePath(filename));
            }
            else
            {
                Console.WriteLine("FileAccess.GetFilePath(\"{0}\", {{", filename);

                // 2nd parameter(s)
                for (int i = 0; i < folders.Length; i++)
                {
                    Console.Write("\t\"{0}\"", folders[i]);
                    if (i < folders.Length - 1) { Console.WriteLine(","); }
                    else { Console.WriteLine("\n\t}"); }
                }

                Console.WriteLine(")=\"{0}\"", LocalFile.GetFilePath(filename, folders));
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LocalFile_GetFilePath_EmptyTest()
        {
            new Log().WriteLine("Empty");
            this.test_GetFilePath(null, null);
        }

        [TestMethod()]
        public void LocalFile_GetFilePathTest()
        {
            new Log(false, false, true).WriteLine("Current Directory");
            this.test_GetFilePath("BUILDLet.Utilities.dll", null);
            
            new Log(false, false, true).WriteLine("My Documents");
            this.test_GetFilePath(LocalPath.DummyFileName_in_MyDocuments, null);

            new Log(false, false, true).WriteLine("My Documents");
            this.test_GetFilePath("NOEXIST", null);

            new Log(false, false, true).WriteLine("Windows Folder");
            this.test_GetFilePath("regedit.exe", null);

            new Log(false, false, true).WriteLine("System32 Folder");
            this.test_GetFilePath("calc.exe", null);


            new Log(false, false, true).WriteLine("Current Directory");
            this.test_GetFilePath("BUILDLet.Utilities.dll", null);


            new Log(false, false, true).WriteLine("Program Folder");
            this.test_GetFilePath("wordpad.exe", new string[] { @"C:\Program Files\Windows NT\Accessories" });

            new Log(false, false, true).WriteLine("Program Folder");
            this.test_GetFilePath("wordpad.exe", LocalFile.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));

            new Log(false, false, true).WriteLine("System32 Folder");
            this.test_GetFilePath("shell32.dll", LocalFile.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));
        }


        [TestMethod()]
        public void LocalFile_ConvertPathTest()
        {
            Log log = new Log();

            string path;
            string expected;
            string actual;
            int number_of_testcase = 2;

            for (int i = 0; i < number_of_testcase; i++)
            {
                switch (i)
                {
                    case 0:
                        path = @"C:\FCIV\";
                        expected = path;
                        break;

                    case 1:
#if DEBUG
                        path = @"..\..\bin\Debug";
#else
                        path = @"..\..\bin\Release";
#endif
                        expected = Path.Combine(Environment.CurrentDirectory);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                actual = LocalFile.ConvertPath(path);
                log.WriteLine("LocalFile.ConvertPath({0}) = {1}", path, actual);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void LocalFile_GetFolderNameTest()
        {
            Log log = new Log();

            string path;
            string expected;
            string actual;
            int number_of_testcase = 2;

            for (int i = 0; i < number_of_testcase; i++)
            {
                switch (i)
                {
                    case 0:
                        path = @"C:\FCIV\";
                        expected = "FCIV";
                        break;

                    case 1:
                        path = @"..\..\bin\Debug";
                        expected = "Debug";
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                actual = LocalFile.GetFolderName(path);
                log.WriteLine("LocalFile.ConvertPath({0}) = {1}", path, actual);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
