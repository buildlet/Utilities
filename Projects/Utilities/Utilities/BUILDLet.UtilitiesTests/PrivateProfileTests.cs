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
using System.IO;
using System.Diagnostics;

using BUILDLet.Utilities;
using BUILDLet.Utilities.Diagnostics;
using BUILDLet.Utilities.Tests.Properties;


namespace BUILDLet.Utilities.Tests
{
    [TestClass()]
    public class PrivateProfileTests
    {
        private static readonly string filepath_ini_testfile1 = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_INI_TestFile1);


        private struct parameter
        {
            public string filepath;

            public string section;
            public string key;
            public string value_expected;

            public parameter(string filepath, string section, string key, string value_expected)
            {
                this.filepath = filepath ?? PrivateProfileTests.filepath_ini_testfile1;

                this.section = section;
                this.key = key;
                this.value_expected = value_expected;
            }
        }



        [TestMethod()]
        public void PrivateProfile_GetValue_TestFile1Test()
        {
            parameter[] parameters =
            {
                // Section1
                new parameter(null, "Section1", "Key1", "Value1"),

                // Section2
                new parameter(null, "Section2", "Key1", "Value1"),
                new parameter(null, "Section2", "Key2", "Value2"),
                new parameter(null, "Section2", "Key3", "Value3"),
                new parameter(null, "Section2", "Key4", "Value4"),
                new parameter(null, "Section2", "Key5", "Value5"),
                new parameter(null, "Section2", "Key6", "Value6"),
                new parameter(null, "Section2", "Key7", "Value7"),
                new parameter(null, "Section2", "Key8", "Value8"),
                new parameter(null, "Section2", "Key9", "Value9"),
                new parameter(null, "Section2", "Key10", "Value10"),
                new parameter(null, "Section2", "Key11", "Value11"),

                // Section3
                new parameter(null, "Section3", "Key1", string.Empty),
                new parameter(null, "Section3", "Key2", string.Empty),
                new parameter(null, "Section3", "Key3", string.Empty),
                new parameter(null, "Section3", "Key4", string.Empty),
                new parameter(null, "Section3", "Key5", string.Empty),
                new parameter(null, "Section3", "Key6", null),
                new parameter(null, "Section3", "Key7", "Value0,0.5,1.0,1.5,1.2,2"),
                new parameter(null, "Section3", "Key8", string.Empty),
                new parameter(null, "Section3", "Key9", "1, 2, 3"),
                new parameter(null, "Section3", "Key10", string.Empty),
                new parameter(null, "Section3", "Key11", null),
                new parameter(null, "Section3", "Key12", "Value12"),
                new parameter(null, "Section3", "Key13", null),

                // Section4
                new parameter(null, "Section4", "Key1", ",,X"),

                // Blank Section
                new parameter(null, "BlankSection", "dummy", null),

                // Invalid Section
                new parameter(null, "dummy", "dummy", null)
            };


            // Console Output
            // Console.WriteLine("Target File=\"{0}\"", parameters[0].filepath);

            for (int i = 0; i < parameters.Length; i++)
            {
                string filepath = parameters[i].filepath;

                string section = parameters[i].section;
                string key = parameters[i].key;

                string expected = parameters[i].value_expected;
                string actual = PrivateProfile.GetValue(section, key, filepath);


                // Console Output (To be continued...)
                Console.Write("PrivateProfile.GetValue(section=\"{1}\", key=\"{2}\", path=\"{3}\")", i, section, key, filepath);


                if (expected == null)
                {
                    // NOT Found
                    Console.WriteLine("\t = OK : Target should NOT be found.");
                    Assert.IsNull(actual);
                }
                else
                {
                    if (actual != null)
                    {
                        // Expected
                        Console.WriteLine("\t = OK : Value \"{0}\" is found.", actual);
                    }
                    else
                    {
                        // Unexpected
                        Console.WriteLine("\t = NG : (Unexpected) Target Key \"{1}\" is NOT found in Section \"{0}\".", section, key);
                    }

                    // Assertion
                    Assert.AreEqual(expected.ToUpper(), actual.ToUpper());
                }
            }
        }



        [TestMethod()]
        public void PrivateProfile_SetValue_TestFile1Test()
        {
            parameter[] parameters =
            {
                // Section1
                new parameter(null, "Section1", "Key1", "UPDATE"),


                // Section2 (Update)
                new parameter(null, "Section2", "Key1", "UPDATE"),
                new parameter(null, "Section2", "Key2", "Update"),
                new parameter(null, "Section2", "Key3", "Update"),
                new parameter(null, "Section2", "Key10", "Update"),
                // Section2 (Append)
                new parameter(null, "Section2", "Key100", "APPEND"),


                // Section3 (Update)
                new parameter(null, "Section3", "Key1", "UPDATE"),
                new parameter(null, "Section3", "Key7", "UPDATE"),
                new parameter(null, "Section3", "Key8", "UPDATE"),
                new parameter(null, "Section3", "Key3", "Update1, Update2, Update3"),
                // Section3 (Append)
                new parameter(null, "Section3", "Key100", "APPEND"),


                // BlankSection (Append)
                new parameter(null, "BlankSection", "Key1", "NEW"),


                // New Section
                new parameter(null, "Section100", "Key1", "New"),
                new parameter(null, "Section100", "Key2", "NEW"),
                new parameter(null, "Section100", "Key3", ""),
                new parameter(null, "Section100", "Key4", null),


                // Section1 (Again)
                new parameter(null, "Section1", "Key1", "UPDATE1"),
                new parameter(null, "Section1", "Key1", "UPDATE2"),
                new parameter(null, "Section1", "Key1", "UPDATE3"),

                // Section2 (Again, Additional: String.EMPTY and NULL Tests)
                new parameter(null, "Section2", "Key5", ""),
                new parameter(null, "Section2", "Key6", ""),
                new parameter(null, "Section2", "Key7", null),
                new parameter(null, "Section2", "Key8", ""),
                new parameter(null, "Section2", "Key9", null)
            };


            string source_filepath = parameters[0].filepath;
            string update_file = "copy.ini";


            DebugInfo.Init();

            // Renew (Delete & Copy) Test INI File
            if (File.Exists(update_file)) { File.Delete(update_file); }
            File.Copy(source_filepath, update_file);
            Console.WriteLine("[{0}] \"{1}\" is copied to \"{2}\".", DebugInfo.Time, source_filepath, update_file);
            Console.WriteLine();


            for (int i = 0; i < 2; i++)
            {
                // 0: Read and Update buffer (string[])
                // 1: Read from file and Update it directly

                string filepath = (i == 0 ? source_filepath : update_file);
                string[] contents = File.ReadLines(filepath).ToArray();


                // Console Output
                Console.WriteLine("Target File=\"{0}\"", filepath);

                for (int j = 0; j < parameters.Length; j++)
                {
                    string section = parameters[j].section;
                    string key = parameters[j].key;
                    string value = parameters[j].value_expected;

                    string actual = null;


                    switch (i)
                    {
                        case 0:
                            // BUFFER

                            // Set Target Value
                            PrivateProfile.SetValue(section, key, value, ref contents);

                            // Console Output (To be continued...)
                            Console.Write("PrivateProfile.SetValue(section=\"{1}\", key=\"{2}\", value=\"{3}\", ref string[] contents)", j, section, key, value);

                            // Get Value to Verify
                            actual = PrivateProfile.GetValue(section, key, contents);
                            break;

                        case 1:
                            // FILE

                            // Set Target Value
                            PrivateProfile.SetValue(section, key, value, update_file);

                            // Console Output (To be continued...)
                            Console.Write("PrivateProfile.SetValue(section=\"{1}\", key=\"{2}\", value=\"{3}\", path=\"{4}\")", j, section, key, value, update_file);

                            // Get Value to Verify
                            actual = PrivateProfile.GetValue(section, key, update_file);
                            break;

                        default:
                            Assert.Inconclusive();
                            break;
                    }

                    // Assertion
                    if (value != null)
                    {
                        Assert.AreEqual(value.ToUpper(), actual.ToUpper());
                    }
                    else
                    {
                        Assert.AreEqual(string.Empty, actual.ToUpper());
                    }

                    // Console Output (append message)
                    Console.WriteLine(" = OK!");
                }
                Console.WriteLine();
            }
        }



        [TestMethod()]
        public void PrivateProfile_GetSection_TestFile1Test()
        {
            string source_filepath = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_INI_TestFile1);

            object[,] parameters =
            {
                { true, new string[] { source_filepath, "Section1" } },
                { true, new string[] { source_filepath, "Section2" } },
                { true, new string[] { source_filepath, "Section3" } },
                { true, new string[] { source_filepath, "Section4" } },
                { true, new string[] { source_filepath, "BlankSection" } },
                { false, new string[] { source_filepath, "NotFoundSection" } }
            };

            for (int i = 0; i < parameters.Length / 2; i++)
            {
                bool found = (bool)parameters[i, 0];
                string filename = ((string[])parameters[i, 1])[0];
                string section_name = ((string[])parameters[i, 1])[1];

                Dictionary<string, string> sections = PrivateProfile.GetSection(section_name, filename);


                // Console Output
                Console.WriteLine("PrivateProfile.GetSection(name=\"{1}\", path=\"{2}\")", i, section_name, filename);

                if (!found)
                {
                    // Console Output
                    Console.WriteLine("Section \"{0}\" should NOT be fouond.", section_name);

                    // Assertion
                    Assert.IsNull(sections);
                }
                else
                {
                    if (sections.Count == 0)
                    {
                        // Console Output
                        Console.WriteLine("Section \"{0}\" is found, but has no entries.", section_name);
                    }
                    else
                    {
                        // Console Output
                        Console.WriteLine("Section \"{0}\" is found.", section_name);
                        int j = 0;
                        foreach (var key in sections.Keys)
                        {
                            Console.WriteLine("Items[{0}] Key=\"{1}\", Value=\"{2}\"", j++, key, sections[key]);
                        }
                    }
                }
                Console.WriteLine();
            }
        }



        [TestMethod()]
        public void PrivateProfile_Read_TestFile1Test()
        {
            string filename = Path.Combine(Resources.DirectoryPath_TestData, Resources.FileName_INI_TestFile1);

            Dictionary<string, Dictionary<string, string>> sections = PrivateProfile.Read(filename);

            Console.WriteLine("PrivateProfile.Read(\"{0}\")", filename);

            int i = 0;
            foreach (var section_name in sections.Keys)
            {
                Console.WriteLine("Sections[{0}]=\"{1}\"", i, section_name);

                int j = 0;
                foreach (var key in sections[section_name].Keys)
                {
                    Console.WriteLine("Items[{0}] Key=\"{1}\", Value=\"{2}\"", j++, key, sections[section_name][key]);
                }

                Console.WriteLine();
                i++;
            }
        }

    }
}
