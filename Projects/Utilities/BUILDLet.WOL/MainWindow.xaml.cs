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
using System.Windows.Navigation;
using System.Windows.Shapes;

using BUILDLet.Utilities;
using BUILDLet.Utilities.Network;

namespace BUILDLet.WOL
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // (Send Button) Click Event Handler
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Magic Packet
                MagicPacket packet = new MagicPacket((string)MacAddressComboBox.Text);

                // Send Magic Packet (3 times)
                packet.Send(3);

                // Show message
                MessageBox.Show(string.Format(Properties.Resources.SendMessage, packet.MacAddress), App.Name, MessageBoxButton.OK, MessageBoxImage.Information);

                // Update source file of MAC addresses
                ((DefaultMacAddressList)((App)App.Current).Resources["addressList"]).UpdateSourceFile(this.MacAddressComboBox.Text);

                // Close MainWindow
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // (Window) KeyDown Event Handler
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); }
        }
    }
}
