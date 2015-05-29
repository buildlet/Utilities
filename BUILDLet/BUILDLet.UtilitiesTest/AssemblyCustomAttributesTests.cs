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
