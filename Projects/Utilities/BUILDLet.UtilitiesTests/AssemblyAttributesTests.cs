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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Reflection;


namespace BUILDLet.Utilities.Tests
{
    [TestClass]
    public class AssemblyAttributesTests
    {
        [TestMethod()]
        public void AssemblyAttributesTest()
        {
            AssemblyAttributes attr;
            string assemblyName;

            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0:
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        attr = new AssemblyAttributes(assembly);
                        assemblyName = assembly.GetName().Name;
                        break;

                    case 1:
                        attr = new AssemblyAttributes();
                        assemblyName = string.Format("Executing Assembly({0})", Assembly.GetExecutingAssembly().GetName().Name);
                        break;

                    default:
                        attr = null;
                        assemblyName = "ERROR";
                        break;
                }

                TestLog.Clear();
                TestLog.WriteLine();
                TestLog.WriteLine(string.Format("Assembly={0}", assemblyName));
                TestLog.WriteLine(string.Format("AssemblyAttributes.Name=\"{0}\"", attr.Name));
                TestLog.WriteLine(string.Format("AssemblyAttributes.FullName=\"{0}\"", attr.FullName));
                TestLog.WriteLine(string.Format("AssemblyAttributes.Version=\"{0}\"", attr.Version.ToString()));
                TestLog.WriteLine(string.Format("AssemblyAttributes.CultureInfo=\"{0}\"", attr.CultureInfo.ToString()));
                TestLog.WriteLine(string.Format("AssemblyAttributes.CultureName=\"{0}\"", attr.CultureName));
            }
        }
    }
}
