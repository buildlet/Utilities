using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BUILDLet.Utilities.WPF.Tests
{
    [TestClass()]
    public class VersionInformationWindowTests
    {
        [TestMethod()]
        public void GUI_VersionInformationWindowTest()
        {
            Assert.IsTrue((bool)(new VersionInformationWindow()).ShowDialog());
        }
    }
}
