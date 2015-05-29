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
            Log log = new Log();

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

                log.WriteLine();
                log.WriteLine("Assembly={0}", assemblyName);
                log.WriteLine("AssemblyAttributes.Name=\"{0}\"", attr.Name);
                log.WriteLine("AssemblyAttributes.FullName=\"{0}\"", attr.FullName);
                log.WriteLine("AssemblyAttributes.Version=\"{0}\"", attr.Version.ToString());
                log.WriteLine("AssemblyAttributes.CultureInfo=\"{0}\"", attr.CultureInfo.ToString());
                log.WriteLine("AssemblyAttributes.CultureName=\"{0}\"", attr.CultureName);
            }
        }
    }
}
