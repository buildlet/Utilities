using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics;
using BUILDLet.Utilities;

namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class FileAccessTests
    {
        [TestMethod()]
        public void FileAccess_SearchPathTest()
        {
            Console.WriteLine("FileAccess.SearchPath={");
            foreach (var dir in FileAccess.SearchPath) { Console.WriteLine("\t\"{0}\",", dir); }
            Console.WriteLine("}");
        }


        private void test_GetSearchPath(string[] expected, string[] actual, string caption, string paramter)
        {
            this.test_GetSearchPath(expected, actual, caption, new string[] { paramter });
        }

        private void test_GetSearchPath(string[] expected, string[] actual, string caption, string[] parameters)
        {
            Console.WriteLine();
            Console.WriteLine("[{0}]", caption);
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
            List<string> expected = new List<string>();
            expected.Add(Environment.CurrentDirectory);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(), "Current Directory", string.Empty);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_InvalidDirectory()
        {
            string dir = "hoge";

            this.test_GetSearchPath(FileAccess.SearchPath, FileAccess.GetSearchPath(dir), "Invalid Directory", dir);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_ValidDirectory()
        {
            string dir = @"C:\Users";

            List<string> expected = new List<string>();
            expected.Add(dir);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dir), "Valid Directory", dir);
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_AlreadyIncludingDirectory()
        {
            Dictionary<string, string> dirs = new Dictionary<string,string>();
            dirs.Add("Windows Folder", @"C:\Windows");
            dirs.Add("System32 Folder", @"C:\Windows\System32");

            foreach (var dir in dirs)
            {
                this.test_GetSearchPath(FileAccess.SearchPath, FileAccess.GetSearchPath(dir.Value), dir.Key, dir.Value);
            }
        }


        [TestMethod()]
        public void FileAccess_GetSearchPathTest_ValidDirectories()
        {
            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.AddRange(dirs);
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dirs.ToArray()), "Valid Directories", dirs.ToArray());
        }

        [TestMethod()]
        public void FileAccess_GetSearchPathTest_Complex()
        {
            List<string> dirs = new List<string>();
            dirs.Add(@"C:\Users");
            dirs.Add(@"C:\Windows");
            dirs.Add(@"C:\Program Files");

            List<string> expected = new List<string>();
            expected.Add(@"C:\Users");
            expected.Add(@"C:\Program Files");
            expected.AddRange(FileAccess.SearchPath);

            this.test_GetSearchPath(expected.ToArray(), FileAccess.GetSearchPath(dirs.ToArray()), "Complex", dirs.ToArray());
        }



        private void test_GetFilePath(string title, string filename)
        {
            this.test_GetFilePath(title, filename, null);
        }

        private void test_GetFilePath(string title, string filename, string[] folders)
        {
            Console.WriteLine();
            Console.WriteLine("[{0}]", title);
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
            this.test_GetFilePath("Empty", null);
        }

        [TestMethod()]
        public void FileAccess_GetFilePathTest()
        {
            this.test_GetFilePath("Current Directory", "BUILDLet.Utilities.dll");
            this.test_GetFilePath("My Documents", "TESTFILE");
            this.test_GetFilePath("My Documents", "NOEXIST");
            this.test_GetFilePath("Windows Folder", "regedit.exe");
            this.test_GetFilePath("System32 Folder", "calc.exe");

            this.test_GetFilePath("Current Directory", "BUILDLet.Utilities.dll");

            this.test_GetFilePath("Program Files", "wordpad.exe", new string[] { @"C:\Program Files\Windows NT\Accessories" });
            this.test_GetFilePath("Program Files", "wordpad.exe", FileAccess.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));
            this.test_GetFilePath("System32 Folder", "shell32.dll", FileAccess.GetSearchPath(@"C:\Program Files\Windows NT\Accessories"));
        }
    }
}
