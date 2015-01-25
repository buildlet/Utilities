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

namespace BUILDLet.Utilities.WPF
{
    /// <summary>
    /// VersionInformationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionInformationWindow : Window
    {
        /// <summary>
        /// <see cref="VersionInformationWindow"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <remarks>
        ///     <see cref="VersionInformationWindow"/> は、このクラスを初期化したメソッドのコードを格納しているアセンブリの各種情報を格納します。
        ///     <see cref="System.Windows.Window"/> と同様に、<see cref="System.Windows.Window.Show"/> または 
        ///     <see cref="System.Windows.Window.ShowDialog"/> メソッドで表示できます。
        /// </remarks>
        public VersionInformationWindow()
        {
            InitializeComponent();

            AssemblyCustomAttributes customAttr = new AssemblyCustomAttributes(Assembly.GetCallingAssembly());
            AssemblyAttributes attr = new AssemblyAttributes(Assembly.GetCallingAssembly());

            string product = customAttr.AssemblyProductAttribute;
            string version = customAttr.AssemblyFileVersionAttribute;
            string build = attr.Version.ToString();

            this.NameLabel.Content = product;
            this.VersionLabel.Content = string.Format("Version {0} (Build {1})", version, build);
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
    }
}
