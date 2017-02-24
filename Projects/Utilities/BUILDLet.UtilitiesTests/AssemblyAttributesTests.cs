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
    public class AssemblyAttributesTests
    {
        private struct parameter
        {
            public Assembly Assembly;
            public string AssemblyName;
        };


        [TestMethod()]
        [TestCategory("MANUAL")]
        public void AssemblyAttributes_Test()
        {
            parameter[] parameters =
            {
                new parameter() { Assembly = null, AssemblyName = "BUILDLet.UtilitiesTests" },
                new parameter() { Assembly = Assembly.GetExecutingAssembly(), AssemblyName = "BUILDLet.UtilitiesTests" },
                new parameter() { Assembly = Assembly.GetCallingAssembly(), AssemblyName = "Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration" }
            };

            AssemblyAttributes target;
            string asssembly_name;

            foreach (var parameter in parameters)
            {
                if (parameter.Assembly == null)
                {
                    target = new AssemblyAttributes();
                    asssembly_name = string.Format("\"{0}\" (Executing Assembly)", Assembly.GetExecutingAssembly().GetName().Name);
                }
                else
                {
                    target = new AssemblyAttributes(parameter.Assembly);
                    asssembly_name = string.Format("\"{0}\"", parameter.Assembly.GetName().Name);
                }

                // Console Output
                Console.WriteLine("Assembly={0}", asssembly_name);
                Console.WriteLine("AssemblyAttributes.Name=\"{0}\"", target.Name);
                Console.WriteLine("AssemblyAttributes.FullName=\"{0}\"", target.FullName);
                Console.WriteLine("AssemblyAttributes.Version.ToString()=\"{0}\"", target.Version.ToString());
                Console.WriteLine("AssemblyAttributes.CultureInfo.ToString()=\"{0}\"", target.CultureInfo.ToString());
                Console.WriteLine("AssemblyAttributes.CultureName=\"{0}\"", target.CultureName);
                Console.WriteLine();

                // Assertion (only Name)
                Assert.AreEqual(parameter.AssemblyName, target.Name);
            }
        }
    }
}
