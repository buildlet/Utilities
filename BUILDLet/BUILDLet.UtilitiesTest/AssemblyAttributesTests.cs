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

                Console.WriteLine();
                Console.WriteLine("[Assembly={0}]", assemblyName);
                Console.WriteLine("AssemblyAttributes.Name=\"{0}\"", attr.Name);
                Console.WriteLine("AssemblyAttributes.FullName=\"{0}\"", attr.FullName);
                Console.WriteLine("AssemblyAttributes.Version=\"{0}\"", attr.Version.ToString());
                Console.WriteLine("AssemblyAttributes.CultureInfo=\"{0}\"", attr.CultureInfo.ToString());
                Console.WriteLine("AssemblyAttributes.CultureName=\"{0}\"", attr.CultureName);
            }
        }
    }
}
