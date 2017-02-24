/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2016-2017 Daiki Sakamoto

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

using System.IO;
using BUILDLet.Utilities.Diagnostics;
using BUILDLet.Utilities.Tests.Properties;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class SimpleFileFinderTests
    {
        private static readonly string filename_hello = Resources.FileName_Hello;
        private static readonly string filepath_hello = Path.Combine(Resources.DirectoryPath_TestData, filename_hello);



        [TestMethod()]
        public void SimpleFileFinder_DefaultSearchPath_Test()
        {
            // Console Output
            for (int i = 0; i < SimpleFileFinder.DefaultSearchPath.Length; i++)
            {
                Console.WriteLine("SimpleFileFinder.DefaultSearchPath[{0}]=\"{1}\"", i, SimpleFileFinder.DefaultSearchPath[i]);
            }

            // Assertion
            Assert.AreEqual(SimpleFileFinder.DefaultSearchPath.Length, 4);
            Assert.AreEqual(SimpleFileFinder.DefaultSearchPath[0], Environment.CurrentDirectory);
            Assert.AreEqual(SimpleFileFinder.DefaultSearchPath[1], Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            Assert.AreEqual(SimpleFileFinder.DefaultSearchPath[2], Environment.GetFolderPath(Environment.SpecialFolder.System));
            Assert.AreEqual(SimpleFileFinder.DefaultSearchPath[3], Environment.GetFolderPath(Environment.SpecialFolder.Windows));
        }



        [TestMethod()]
        public void SimpleFileFinder_GetSearchPath_Test()
        {
            SimpleFileFinder finder = new SimpleFileFinder();
            string[] folders = finder.GetSearchPath();

            // Console Output
            Console.WriteLine("SimpleFileFinder.GetSearchPath()={");
            for (int i = 0; i < folders.Length; i++) { Console.WriteLine("  Items[{0}]=\"{1}\"", i, folders[i]); }
            Console.WriteLine("}");
        }



        [TestMethod()]
        public void SimpleFileFinder_SearchPath_AddTest()
        {
            object[,] parameters =
            {
                // index, path
                { 1, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Test (Inserted)") },
                { 0, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Test (Inserted)") },
                { -1, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Test (Appended)") },
            };


            for (int i = 0; i < parameters.Length / 2; i++)
            {
                int index = (int)parameters[i, 0];
                string path = (string)parameters[i, 1];

                SimpleFileFinder finder = new SimpleFileFinder();
                if (index < 0)
                {
                    finder.SearchPath.Add(path);
                }
                else
                {
                    finder.SearchPath.Insert(index, path);
                }

                string[] folders = finder.GetSearchPath();


                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Index={1}, Path=\"{2}\" }}", i, index, path);
                Console.WriteLine("SimpleFileFinder.GetSearchPath()={");
                for (int j = 0; j < folders.Length; j++) { Console.WriteLine("  Items[{0}]=\"{1}\"", j, folders[j]); }
                Console.WriteLine("}");
                Console.WriteLine();


                // Assertion
                if (index < 0)
                {
                    Assert.AreEqual(finder.GetSearchPath()[finder.GetSearchPath().Length - 1], path);
                }
                else
                {
                    Assert.AreEqual(finder.GetSearchPath()[index], path);
                }
            }
        }


        [TestMethod()]
        public void SimpleFileFinder_Find_Test()
        {
            string target_dir1 = Path.Combine(Environment.CurrentDirectory, "SimpleFileFinder_TestFolder1");
            string target_dir2 = Path.Combine(Environment.CurrentDirectory, "SimpleFileFinder_TestFolder2");
            string target_file1 = Path.Combine(target_dir1, SimpleFileFinderTests.filename_hello);
            string target_file2 = Path.Combine(target_dir2, SimpleFileFinderTests.filename_hello);

            // Create Directory
            if (!Directory.Exists(target_dir1)) { Directory.CreateDirectory(target_dir1); }
            if (!Directory.Exists(target_dir2)) { Directory.CreateDirectory(target_dir2); }

            // Create Target File
            if (!File.Exists(target_file1)) { File.Copy(SimpleFileFinderTests.filepath_hello, target_file1); }
            if (!File.Exists(target_file2)) { File.Copy(SimpleFileFinderTests.filepath_hello, target_file2); }


            object[,] parameters =
            {
                // { file name, number of file to be found, search path to be added, expected found dir }
                { "BUILDLet.Utilities.dll", 1, null, new string[] { Environment.CurrentDirectory } },
                { "hoge", -1, null, null },
                { Resources.FileName_Hello, 2, new string[] { target_dir1, target_dir2 }, new string[] { target_dir1, target_dir2 } },

                // mignt be according to TEST ENVIRONMENT
                { "calc.exe", 1, null, new string[] { Environment.GetFolderPath(Environment.SpecialFolder.System) } },
                { "notepad.exe", 2, null, new string[] { Environment.GetFolderPath(Environment.SpecialFolder.System), Environment.GetFolderPath(Environment.SpecialFolder.Windows) } }
            };


            for (int i = 0; i < parameters.Length / 4; i++)
            {
                string filename = (string)parameters[i, 0];
                int found = (int)parameters[i, 1];

                SimpleFileFinder finder = new SimpleFileFinder();


                // Add SearchPath
                if (parameters[i, 2] != null)
                {
                    finder.SearchPath.Insert(2, ((string[])parameters[i, 2])[0]);

                    for (int j = 1; j < ((string[])parameters[i, 2]).Length; j++)
                    {
                        finder.SearchPath.Add(((string[])parameters[i, 2])[j]);
                    }
                }


                string[] actual = finder.Find(filename);

                // Console Output
                Console.WriteLine("Parameters[{0}] {{ Target File=\"{1}\", Found (Expected) ={2} }}", i, filename, found);

                if (found > 0)
                {
                    string[] expected = (string[])parameters[i, 3];

                    for (int j = 0; j < actual.Length; j++)
                    {
                        // Console Output
                        Console.WriteLine("Found[{0}]=\"{1}\"", j, actual[j]);

                        // Assertion
                        Assert.AreEqual(Path.Combine(expected[j], filename), actual[j]);
                    }

                    // Assertion
                    Assert.AreEqual(found, actual.Length);
                }
                else
                {
                    Console.WriteLine("Not Found");

                    // Assertion
                    Assert.IsNull(actual);
                }

                Console.WriteLine();
            }
        }
    }
}
