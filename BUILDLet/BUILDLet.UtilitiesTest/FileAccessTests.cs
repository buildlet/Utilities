using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics;

namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class FileAccessTests
    {
        [TestMethod()]
        public void FileAccess_SearchPathTest()
        {
            Log log = new Log(false, false, true);

            log.WriteLine("FileAccess.SearchPath={");
            foreach (var dir in FileAccess.SearchPath) { log.WriteLine("\t\"{0}\",", dir); }
            log.WriteLine("}");
        }


        private void test_GetSearchPath(string[] expected, string[] actual, string paramter)
        {
            this.test_GetSearchPath(expected, actual, new string[] { paramter });
        }

        private void test_GetSearchPath(string[] expected, string[] actual, string[] parameters)
        {
            Console.Write("FileAccess.GetSearchPath(");


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
        public void FileAccess_GetSearchPathTest_Empty()
        {
            new Log().WriteLine("Empty(Current Directory)");

            List<string> expected = new List<string>();
            expected.Add(Environment.CurrentDirectory);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(), string.Empty);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_InvalidDirectory()
        {
            new Log().WriteLine("Invalid Directory");
            string dir = "hoge";

            this.test_GetSearchPath(FileAccess.SearchPath, FileAccess.GetSearchPath(dir), dir);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_ValidDirectory()
        {
            new Log().WriteLine("Valid Directory");
            string dir = @"C:\Users";

            List<string> expected = new List<string>();
            expected.Add(dir);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dir), dir);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_AlreadyIncludingDirectory()
        {
            Log log = new Log();

            Dictionary<string, string> dirs = new Dictionary<string, string>();
            dirs.Add("Windows Folder", @"C:\Windows");
            dirs.Add("System32 Folder", @"C:\Windows\System32");

            foreach (var dir in dirs)
            {
                log.WriteLine();
                log.WriteLine(dir.Key);
                this.test_GetSearchPath(FileAccess.SearchPath, FileAccess.GetSearchPath(dir.Value), dir.Value);
            }
        }


        [TestMethod()]
        public void FileAccess_GetSearchPathTest_ValidDirectories()
        {
            new Log().WriteLine("Valid Directories");

            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.AddRange(dirs);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dirs.ToArray()), dirs.ToArray());
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_Complex()
        {
            new Log().WriteLine("Complex");

            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Windows");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.Add(@"C:\Users");
            expected.Add(@"C:\Program Files");
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dirs.ToArray()), dirs.ToArray());
        }



        private void test_GetFilePath(string filename, string[] folders)
        {
            if (folders == null || folders.Length == 0)
            {
                Console.WriteLine("FileAccess.GetFilePath(\"{0}\")=\"{1}\"", filename, FileAccess.GetFilePath(filename));
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

                Console.WriteLine(")=\"{0}\"", FileAccess.GetFilePath(filename, folders));
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileAccess_GetFilePathTest_Empty()
        {
            new Log().WriteLine("Empty");
            this.test_GetFilePath(null, null);
        }

        [TestMethod()]
        public void FileAccess_GetFilePathTest()
        {
            Log log = new Log();

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
            this.test_GetFilePath("wordpad.exe", FileAccess.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));

            new Log(false, false, true).WriteLine("System32 Folder");
            this.test_GetFilePath("shell32.dll", FileAccess.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));
        }
    }
}
