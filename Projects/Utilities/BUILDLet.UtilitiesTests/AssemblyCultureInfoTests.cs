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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Reflection;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.Tests
{
    [TestClass]
    public class AssemblyCultureInfoTests
    {
        private struct test_parameter
        {
            public Assembly Assembly;
            public string DisplayName;
            public string NativeName;
            public string EnglishName;
            public int LCID;
        };


        [TestMethod()]
        [TestCategory("MANUAL")]
        public void AssemblyCultureInfo_Test()
        {
            string this_display_name = "ロケールに依存しない言語 (ロケールに依存しない国)";
            string this_native_name = "Invariant Language (Invariant Country)";
            string this_english_name = this_native_name;
            int this_lcid = 127;

            test_parameter[] parameters =
            {
                new test_parameter()
                {
                    Assembly = null,
                    DisplayName = this_display_name,
                    NativeName = this_native_name,
                    EnglishName = this_english_name,
                    LCID = this_lcid
                },

                new test_parameter()
                {
                    Assembly = Assembly.GetExecutingAssembly(),
                    DisplayName = this_display_name,
                    NativeName = this_native_name,
                    EnglishName = this_english_name,
                    LCID = this_lcid
                },

                new test_parameter()
                {
                    Assembly = Assembly.GetCallingAssembly(),
                    DisplayName = this_display_name,
                    NativeName = this_native_name,
                    EnglishName = this_english_name,
                    LCID = this_lcid
                }
            };


            AssemblyCultureInfo target;
            string assembly_name;

            foreach (var parameter in parameters)
            {
                if (parameter.Assembly == null)
                {
                    target = new AssemblyCultureInfo();
                    assembly_name = string.Format("\"{0}\" (Executing Assembly)", Assembly.GetExecutingAssembly().GetName().Name);
                }
                else
                {
                    target = new AssemblyCultureInfo(parameter.Assembly);
                    assembly_name = string.Format("\"{0}\"", parameter.Assembly.GetName().Name);
                }

                // Console Output
                Console.WriteLine("Assembly={0}", assembly_name);
                Console.WriteLine("AssemblyCultureInfo.Name=\"{0}\"", target.Name);
                Console.WriteLine("AssemblyCultureInfo.DisplayName=\"{0}\"", target.DisplayName);
                Console.WriteLine("AssemblyCultureInfo.NativeName=\"{0}\"", target.NativeName);
                Console.WriteLine("AssemblyCultureInfo.EnglishName=\"{0}\"", target.EnglishName);
                Console.WriteLine("AssemblyCultureInfo.LCID.ToString()=\"{0}\"", target.LCID.ToString());
                Console.WriteLine();

                // Assertion
                Assert.AreEqual(target.DisplayName, parameter.DisplayName);
                Assert.AreEqual(target.NativeName, parameter.NativeName);
                Assert.AreEqual(target.EnglishName, parameter.EnglishName);
                Assert.AreEqual(target.LCID, parameter.LCID);
            }
        }

    }
}
