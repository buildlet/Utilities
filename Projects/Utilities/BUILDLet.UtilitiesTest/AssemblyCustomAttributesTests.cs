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
    public class AssemblyCustomAttributesTests
    {
        [TestMethod()]
        public void AssemblyCustomAttributesTest()
        {
            Log log = new Log();

            AssemblyCustomAttributes attr;
            string assemblyName;

            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0:
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        attr = new AssemblyCustomAttributes(assembly);
                        assemblyName = assembly.GetName().Name;
                        break;

                    case 1:
                        attr = new AssemblyCustomAttributes();
                        assemblyName = string.Format("Executing Assembly({0})", Assembly.GetExecutingAssembly().GetName().Name);
                        break;

                    default:
                        attr = null;
                        assemblyName = "ERROR";
                        break;
                }

                log.WriteLine();
                log.WriteLine("Assembly={0}", assemblyName);
                log.WriteLine("AssemblyCustomAttributes.Title=\"{0}\"", attr.AssemblyTitleAttribute);
                log.WriteLine("AssemblyCustomAttributes.Description=\"{0}\"", attr.AssemblyDescriptionAttribute);
                log.WriteLine("AssemblyCustomAttributes.Company=\"{0}\"", attr.AssemblyCompanyAttribute);
                log.WriteLine("AssemblyCustomAttributes.Product=\"{0}\"", attr.AssemblyProductAttribute);
                log.WriteLine("AssemblyCustomAttributes.Copyright=\"{0}\"", attr.AssemblyCopyrightAttribute);
                log.WriteLine("AssemblyCustomAttributes.Trademark=\"{0}\"", attr.AssemblyTrademarkAttribute);
                log.WriteLine("AssemblyCustomAttributes.FileVersion=\"{0}\"", attr.AssemblyFileVersionAttribute);
                log.WriteLine("AssemblyCustomAttributes.ToString()=\"{0}\"", attr.ToString());
            }
        }
    }
}
