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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Reflection;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.Tests
{
    [TestClass]
    public class AssemblyCustomAttributesTests
    {
        private struct parameter
        {
            public Assembly Assembly;
            public string Title;
            public string Description;
            public string Product;
            public string Company;
            public string Copyright;
        };


        [TestMethod()]
        [TestCategory("MANUAL")]
        public void AssemblyCustomAttributes_Test()
        {
            string this_description = "Test Module for BUILDLet Utilities Class Library";
            string vs_description = "Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration.dll";

            parameter[] parameters =
            {
                new parameter()
                {
                    Assembly = null,
                    Title = this_description,
                    Description = this_description,
                    Product = this_description,
                    Company = "BUILDLet",
                    Copyright = "Copyright © 2014-2017 Daiki Sakamoto"
                },

                new parameter()
                {
                    Assembly = Assembly.GetExecutingAssembly(),
                    Title = this_description,
                    Description = this_description,
                    Product = this_description,
                    Company = "BUILDLet",
                    Copyright = "Copyright © 2014-2017 Daiki Sakamoto"
                },

                new parameter()
                {
                    Assembly = Assembly.GetCallingAssembly(),
                    Title = vs_description,
                    Description = vs_description,
                    Product = "Microsoft® Visual Studio® 2015",
                    Company = "Microsoft Corporation",
                    Copyright = "© Microsoft Corporation. All rights reserved."
                },
            };


            AssemblyCustomAttributes target;
            string assembly_name;

            foreach (var parameter in parameters)
            {
                if (parameter.Assembly == null)
                {
                    target = new AssemblyCustomAttributes();
                    assembly_name = string.Format("\"{0}\" (Executing Assembly)", Assembly.GetExecutingAssembly().GetName().Name);
                }
                else
                {
                    target = new AssemblyCustomAttributes(parameter.Assembly);
                    assembly_name = string.Format("\"{0}\"", parameter.Assembly.GetName().Name);
                }

                // Console Output
                Console.WriteLine("Assembly={0}", assembly_name);
                Console.WriteLine("AssemblyCustomAttributes.Title=\"{0}\"", target.Title);
                Console.WriteLine("AssemblyCustomAttributes.Description=\"{0}\"", target.Description);
                Console.WriteLine("AssemblyCustomAttributes.Company=\"{0}\"", target.Company);
                Console.WriteLine("AssemblyCustomAttributes.Product=\"{0}\"", target.Product);
                Console.WriteLine("AssemblyCustomAttributes.Copyright=\"{0}\"", target.Copyright);
                Console.WriteLine("AssemblyCustomAttributes.Trademark=\"{0}\"", target.Trademark);
                Console.WriteLine("AssemblyCustomAttributes.FileVersion=\"{0}\"", target.FileVersion);
                Console.WriteLine("AssemblyCustomAttributes.ToString()=\"{0}\"", target.ToString());
                Console.WriteLine();

                // Assertion
                Assert.AreEqual(parameter.Title, target.Title);
                Assert.AreEqual(parameter.Description, target.Description);
                Assert.AreEqual(parameter.Company, target.Company);
                Assert.AreEqual(parameter.Product, target.Product);
                Assert.AreEqual(parameter.Copyright, target.Copyright);
            }
        }
    }
}
