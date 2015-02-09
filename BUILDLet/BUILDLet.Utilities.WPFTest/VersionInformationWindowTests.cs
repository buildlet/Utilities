using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Windows.Media.Imaging;

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


        [TestMethod()]
        public void GUI_VersionInformationWindowTest_with_Image()
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(@"..\..\Resources\BUILDLet_White_64x64.png", UriKind.Relative);
            logo.CacheOption = BitmapCacheOption.OnLoad;
            logo.EndInit();

            Assert.IsTrue((bool)(new VersionInformationWindow(logo)).ShowDialog());
        }
    }
}
