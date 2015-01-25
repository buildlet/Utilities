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
            InitializeComponent();

            // Get default MAC Address
            string mac = ((App)App.Current).GetDefaultMacAddress();

            // Set default MAC Address
            if (!string.IsNullOrEmpty(mac)) { this.MacAddressTextBox.Text = mac; }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Magic Packet
                MagicPacket packet = new MagicPacket(MacAddressTextBox.Text);

                // Send Magic Packet
                WakeOnLan.Send(packet, 3);

                // Show Message
                MessageBox.Show(string.Format(Properties.Resources.SendMessage, packet.MacAddress), App.Name, MessageBoxButton.OK, MessageBoxImage.Information);

                // Close MainWindow
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); }
        }
    }
}
