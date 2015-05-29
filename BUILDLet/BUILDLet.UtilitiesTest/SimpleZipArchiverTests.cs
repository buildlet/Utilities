using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using BUILDLet.Utilities.Compression;
using BUILDLet.Utilities.Tests;


namespace BUILDLet.Utilities.Compression.Tests
{
    [TestClass()]
    public class SimpleZipArchiverTests
    {
        [TestMethod()]
        public void SimpleZipArchiver_ZipUnzipTest()
        {
            // Clean
            foreach (var entry in (Directory.GetFileSystemEntries(Environment.CurrentDirectory, "*", SearchOption.TopDirectoryOnly)))
            {
                if (!entry.Split(Path.DirectorySeparatorChar).Last().Contains("BUILDLet.Utilities"))
                {
                    if (Directory.Exists(entry)) { Directory.Delete(entry, true); }
                    if (File.Exists(entry)) { File.Delete(entry); }
                }
            }


            Log log = new Log();

            string source = string.Empty;
            string destination = string.Empty;
            string output = string.Empty;


            int number_of_testcase = 5;

            string unzipped0, unzipped1, unzipped2;
            string zipped0, zipped1;

            unzipped0 = unzipped1 = unzipped2 = string.Empty;
            zipped0 = zipped1 = string.Empty;

            for (int i = 0; i < number_of_testcase; i++)
            {
                // Unzip (Extract)
                switch (i)
                {
                    case 0:
                        unzipped0 = "RootDirectory";
                        source = Path.Combine(LocalPath.TestDataFolder, "RootDirectory.zip");
                        output = SimpleZipArchiver.Unzip(source);
                        break;

                    case 1:
                        unzipped1 = "(" + i.ToString() + ") UnZippedRootFolder";
                        source = Path.Combine(LocalPath.TestDataFolder, "RootDirectory.zip");
                        output = SimpleZipArchiver.Unzip(source, foldername: unzipped1);
                        break;

                    case 2:
                        unzipped2 = "(" + i.ToString() + ") UnZipped";
                        Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, unzipped2));
                        source = Path.Combine(LocalPath.TestDataFolder, "RootDirectory.zip");
                        output = SimpleZipArchiver.Unzip(source, destination: unzipped2);
                        break;

                    case 3:
                        source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
                        output = SimpleZipArchiver.Unzip(source);
                        break;

                    case 4:
                        source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
                        output = SimpleZipArchiver.Unzip(source, overwrite: true);
                        break;

                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(output))
                {
                    log.WriteLine("({0}) \"{1}\" is unzipped to \"{2}\".", i, source, output);
                }


                // Zip (Archive)
                switch (i)
                {
                    case 0:
                        zipped0 = "RootDirectory";
                        output = SimpleZipArchiver.Zip(source = "RootDirectory");
                        break;

                    case 1:
                        zipped1 = "(" + i.ToString() + ") Zipped";
                        destination = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, zipped1)).FullName;
                        output = SimpleZipArchiver.Zip(source = unzipped0, destination);

                        output = SimpleZipArchiver.Zip(source = unzipped1, filename: ("(" + i.ToString() + ") Root1.zip"), includeBaseDirectory: false);

                        output = SimpleZipArchiver.Zip(source = Path.Combine(unzipped1, "RootDirectory"), filename: ("(" + i.ToString() + ") Root2.zip"));

                        break;

                    case 2:
                        break;

                    case 3:
                        output = SimpleZipArchiver.Zip(source = @"hello\hello.txt");
                        break;

                    case 4:
                        output = SimpleZipArchiver.Zip(source = @"hello\hello.txt", overwrite: true);
                        break;

                    default:
                        break;
                }
                log.WriteLine("({0}) \"{1}\" is zipped to \"{2}\".", i, source, output);
            }
        }



        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SimpleZipArchiver_UnzipExceptionTest1()
        {
            // source does not exist.

            string source = Path.Combine(Environment.CurrentDirectory, "dummy");
            SimpleZipArchiver.Unzip(source);
        }

        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void SimpleZipArchiver_UnzipExceptionTest2()
        {
            // destination directory does not exist.

            string source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
            string destination = "dummy";
            SimpleZipArchiver.Unzip(source, Path.Combine(Environment.CurrentDirectory, destination));
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void SimpleZipArchiver_UnzipExceptionTest3()
        {
            // output folder path to be unzipped already exists. (Directory)

            string source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
            string destination = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName;
            SimpleZipArchiver.Unzip(source, destination, "bin");
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void SimpleZipArchiver_UnzipExceptionTest4()
        {
            // output folder path to be unzipped already exists. (File)

            string source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
            string destination = "BUILDLet.UtilitiesTest.dll";
            SimpleZipArchiver.Unzip(source, Environment.CurrentDirectory, destination);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SimpleZipArchiver_UnzipExceptionTest5()
        {
            // folder name includes directory separator character.

            string source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
            SimpleZipArchiver.Unzip(source, foldername: @".\dummy\folder");
        }



        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SimpleZipArchiver_ZipExceptionTest1()
        {
            // source does not exist.
            SimpleZipArchiver.Zip(Path.Combine(Environment.CurrentDirectory, "dummy"));
        }

        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void SimpleZipArchiver_ZipExceptionTest2()
        {
            // destination directory does not exist.
            SimpleZipArchiver.Zip(LocalPath.TestDataFolder, Path.Combine(Environment.CurrentDirectory, "dummy"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void SimpleZipArchiver_ZipExceptionTest3()
        {
            // output zip file path already exists. (Directory)
            string destination = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName;
            SimpleZipArchiver.Zip(LocalPath.TestDataFolder, destination, "bin");
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException))]
        public void SimpleZipArchiver_ZipExceptionTest4()
        {
            // output zip file path already exists. (File)
            SimpleZipArchiver.Zip(LocalPath.TestDataFolder, Environment.CurrentDirectory, "BUILDLet.UtilitiesTest.dll");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SimpleZipArchiver_ZipExceptionTest5()
        {
            // folder name includes directory separator character.
            string source = Path.Combine(LocalPath.TestDataFolder, "hello.zip");
            SimpleZipArchiver.Zip(source, filename: @".\dummy\folder");
        }
    }
}
