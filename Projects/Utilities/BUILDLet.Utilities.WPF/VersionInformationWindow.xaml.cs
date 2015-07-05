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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Reflection;
using System.IO;
using System.Drawing.Imaging;


namespace BUILDLet.Utilities.WPF
{
    /// <summary>
    /// VersionInformationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionInformationWindow : Window
    {
        /// <summary>
        /// ロゴイメージの幅を表します。
        /// </summary>
        public static int LogoImageWidth
        {
            get { return 64; }
        }


        /// <summary>
        /// ロゴイメージの高さを表します。
        /// </summary>
        public static int LogoImageHeight
        {
            get { return VersionInformationWindow.LogoImageWidth; }
        }


        /// <summary>
        /// <see cref="VersionInformationWindow"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="logo">
        /// ロゴ イメージを指定します。
        /// </param>
        /// <param name="format">
        /// ロゴ イメージのフォーマット指定します。
        /// </param>
        /// <remarks>
        /// <see cref="VersionInformationWindow"/> は、このクラスを初期化したメソッドのコードを格納しているアセンブリの各種情報を格納します。
        /// <see cref="System.Windows.Window"/> と同様に、<see cref="System.Windows.Window.Show"/> または 
        /// <see cref="System.Windows.Window.ShowDialog"/> メソッドで表示できます。
        /// <para>
        /// このメソッドは Version 1.1.2.0 で追加されました。
        /// </para>
        /// </remarks>
        public VersionInformationWindow(System.Drawing.Image logo, ImageFormat format) : this(Assembly.GetCallingAssembly())
        {
            try
            {
                using (System.IO.Stream stream = new MemoryStream())
                {
                    logo.Save(stream, format);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    // Add Logo Image
                    this.addLogoImage(image);
                }
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// <see cref="VersionInformationWindow"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="logo">
        /// ロゴ イメージを指定します。
        /// <para>
        /// 画像が表示されない場合は <see cref="BitmapImage.CacheOption"/> を <see cref="BitmapCacheOption.OnLoad"/> に設定すると、表示される場合があります。
        /// </para>
        /// <para>
        /// 64x64 ピクセルの画像として表示されるので、<see cref="BitmapImage.DecodePixelWidth"/> または 
        /// <see cref="BitmapImage.DecodePixelHeight"/> に 64 を設定することを検討してください。
        /// この値は <see cref="VersionInformationWindow.LogoImageWidth"/> および <see cref="VersionInformationWindow.LogoImageHeight"/> として参照できます。
        /// </para>
        /// </param>
        /// <remarks>
        /// <see cref="VersionInformationWindow"/> は、このクラスを初期化したメソッドのコードを格納しているアセンブリの各種情報を格納します。
        /// <see cref="System.Windows.Window"/> と同様に、<see cref="System.Windows.Window.Show"/> または 
        /// <see cref="System.Windows.Window.ShowDialog"/> メソッドで表示できます。
        /// </remarks>
        public VersionInformationWindow(BitmapImage logo) : this(Assembly.GetCallingAssembly())
        {
            try
            {
                // Add Logo Image
                this.addLogoImage(logo);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// <see cref="VersionInformationWindow"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="assembly">
        /// バージョン情報を表示する <see cref="System.Reflection.Assembly"/> を指定します。
        /// 省略した場合 (および null を指定した場合) は、このクラスを初期化したメソッドのコードを格納しているアセンブリのバージョン情報が指定されます。
        /// このパラメーターは Version 1.1.2.0 で追加されました。
        /// </param>
        /// <remarks>
        /// <see cref="System.Windows.Window"/> と同様に、<see cref="System.Windows.Window.Show"/> または 
        /// <see cref="System.Windows.Window.ShowDialog"/> メソッドで表示できます。
        /// </remarks>
        public VersionInformationWindow(Assembly assembly = null)
        {
            InitializeComponent();

            try
            {
                if (assembly == null) { assembly = Assembly.GetCallingAssembly(); }

                AssemblyCustomAttributes customAttr = new AssemblyCustomAttributes(assembly);
                AssemblyAttributes attr = new AssemblyAttributes(assembly);

                string product = customAttr.AssemblyProductAttribute;
                string version = customAttr.AssemblyFileVersionAttribute;
                string build = attr.Version.ToString();

                this.NameLabel.Content = product;
                this.VersionLabel.Content = string.Format("Version {0} (Build {1})", version, build);

                // Set Logo Image width = 0
                this.LogoImage.Width = 0;
            }
            catch (Exception e) { throw e; }
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }


        private void addLogoImage(BitmapImage logo)
        {
            try
            {
                // If logo has been already initialized, these oprations are not effective...
                // (BeginInit() and EndInit() is already called.)
                logo.DecodePixelWidth = VersionInformationWindow.LogoImageWidth;
                logo.CacheOption = BitmapCacheOption.OnLoad;


                this.LogoImage.Source = logo;
                this.LogoImage.Width = VersionInformationWindow.LogoImageWidth;
                this.LogoImage.Height = VersionInformationWindow.LogoImageHeight;
                this.LogoImage.Margin = new Thickness(15, 2, 10, 10);
            }
            catch (Exception e) { throw e; }
        }
    }
}
