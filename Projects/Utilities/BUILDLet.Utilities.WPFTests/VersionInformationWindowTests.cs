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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;


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
        public void GUI_VersionInformationWindowTest2()
        {
            Assert.IsTrue((bool)(new VersionInformationWindow(Assembly.GetCallingAssembly())).ShowDialog());
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


        [TestMethod()]
        public void GUI_VersionInformationWindowTest_with_Image2()
        {
            Bitmap logo = new Bitmap(@"..\..\Resources\BUILDLet_White_64x64.png");
            Assert.IsTrue((bool)(new VersionInformationWindow(logo, logo.RawFormat)).ShowDialog());
        }
    }
}
