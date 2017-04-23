/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

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

using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;

using BUILDLet.Utilities;
using BUILDLet.Utilities.Network;


namespace BUILDLet.Utilities.WOL
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // Send Message
        private readonly string sendMessage = Properties.Resources.SendMessage;

        private readonly string settingFileName = Properties.Resources.SettingFileName;
        private readonly string personalSettingFileDirName = Properties.Resources.SettingFilePersonalFolderName;
        private readonly string defaultUtilitiesDirName = Properties.Resources.UtilitiesFolderName;

        private readonly string ini_WOL_Section = Properties.Resources.INI_WOL_Section;
        private readonly string ini_CloseAfterSend_Key = Properties.Resources.INI_CloseAfterSend_Key;
        private readonly string ini_ShowDialogAfterSend_Key = Properties.Resources.INI_ShowDialogAfterSend_Key;
        private readonly string ini_Histories_Key = Properties.Resources.INI_Histories_Key;
        private readonly string ini_SendCount_Key = Properties.Resources.INI_SendCount_Key;
        private readonly string ini_MacAddressList_Key = Properties.Resources.INI_MacAddressList_Key;

        private readonly string defaultShowDialogAfterSendText = Properties.Resources.DefaultShowDialogAfterSend;
        private readonly string defaultCloseAfterSendText = Properties.Resources.DefaultCloseAfterSend;

        private readonly string defaultHistoriesText = Properties.Resources.DefaultHistories;
        private readonly string defaultSendCountText = Properties.Resources.DefaultSendCount;

        private readonly int maxHistories = int.Parse(Properties.Resources.MaxHistories);
        private readonly int maxSendCount = int.Parse(Properties.Resources.MaxSendCount);

        private int histries = int.Parse(Properties.Resources.DefaultHistories);
        private int send_count = int.Parse(Properties.Resources.DefaultSendCount);


        // Setting File Path
        protected string SettingFilePath;

        protected bool ShowDialogAfterSend { get; set; } = (int.Parse(Properties.Resources.DefaultShowDialogAfterSend) > 0 ? true : false);
        protected bool CloseAfterSend { get; set; } = (int.Parse(Properties.Resources.DefaultCloseAfterSend) > 0 ? true : false);

        protected int SendCount
        {
            get { return this.send_count; }
            set
            {
                try
                {
                    if (value <= this.maxSendCount) { this.send_count = value; }
                }
                catch (Exception) { throw; }
            }
        }

        protected int Histories
        {
            get { return this.histries; }
            set
            {
                try
                {
                    if (value <= this.maxHistories) { this.histries = value; }
                }
                catch (Exception) { throw; }
            }
        }


        // MAC Address List
        public static ObservableCollection<string> MacAddressList { get; set; }


        public MainWindow()
        {
            try
            {
                // Load MAC Address List
                // (Should be called before InitializeComponent())
                this.loadDefaualtSettings();

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


                // Send Magic Packet
                packet.Send(this.SendCount);


                if (this.ShowDialogAfterSend)
                {
                    // Show message
                    MessageBox.Show(string.Format(this.sendMessage, packet.MacAddress, this.SendCount), App.Name, MessageBoxButton.OK, MessageBoxImage.Information);
                }


                // Save Updated MAC Address List
                this.saveUpdatedSettings();


                if (this.CloseAfterSend)
                {
                    // Close MainWindow
                    this.Close();
                }
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


        // Load Default Settings (including MAC Address List)
        private void loadDefaualtSettings()
        {
            MainWindow.MacAddressList = new ObservableCollection<string>();

            try
            {
                SimpleFileFinder finder = new SimpleFileFinder();

                // Clear & Set Search Path
                finder.SearchPath.Clear();
                finder.SearchPath.AddRange(new List<string> {
                    Environment.CurrentDirectory,
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), this.personalSettingFileDirName),
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), this.defaultUtilitiesDirName),
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), this.defaultUtilitiesDirName)
                });

                // Find File Path
                string[] files = finder.Find(this.settingFileName);

                if (files != null)
                {
                    // Set File Path
                    this.SettingFilePath = files[0];

                    // Set ShowDialogAfterSend
                    this.ShowDialogAfterSend = (int.Parse(PrivateProfile.GetValue(this.ini_WOL_Section, this.ini_ShowDialogAfterSend_Key, this.SettingFilePath) ?? this.defaultShowDialogAfterSendText) > 0 ? true : false);

                    // Set CloseAfterSend
                    this.CloseAfterSend = (int.Parse(PrivateProfile.GetValue(this.ini_WOL_Section, this.ini_CloseAfterSend_Key, this.SettingFilePath) ?? this.defaultCloseAfterSendText) > 0 ? true : false);

                    // Set SendCount
                    this.SendCount = int.Parse(PrivateProfile.GetValue(this.ini_WOL_Section, this.ini_SendCount_Key, this.SettingFilePath) ?? this.defaultSendCountText);

                    // Set HistroyCount
                    this.Histories = int.Parse(PrivateProfile.GetValue(this.ini_WOL_Section, this.ini_Histories_Key, this.SettingFilePath) ?? this.defaultHistoriesText);

                    // Get Default Mac Addresses
                    string[] addresses = PrivateProfile.GetValue(this.ini_WOL_Section, this.ini_MacAddressList_Key, this.SettingFilePath).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Set Dafault MAC Address List
                    for (int i = 0; i < ((addresses.Length < this.Histories) ? addresses.Length : this.Histories); i++)
                    {
                        MainWindow.MacAddressList.Add(addresses[i].PadLeft(("FF:FF:FF:FF:FF:FF").Length));
                    }
                }
            }
            catch (Exception)
            {
                // Do Nothing
                // MessageBox.Show(string.Format("Initialization Failed! ({0})", e.Message), App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Save Updateed Settings (including MAC Address List)
        private void saveUpdatedSettings()
        {
            try
            {
                // Write Current Settings to Setting File
                if (!string.IsNullOrEmpty(this.SettingFilePath))
                {
                    // Set ShowDialogAfterSend
                    PrivateProfile.SetValue(this.ini_WOL_Section, this.ini_ShowDialogAfterSend_Key, (this.ShowDialogAfterSend ? "1" : "0"), this.SettingFilePath);

                    // Write CloseAfterSend
                    PrivateProfile.SetValue(this.ini_WOL_Section, this.ini_CloseAfterSend_Key, (this.CloseAfterSend ? "1" : "0"), this.SettingFilePath);

                    // Write SendCount
                    PrivateProfile.SetValue(this.ini_WOL_Section, this.ini_SendCount_Key, this.SendCount.ToString(), this.SettingFilePath);

                    // Write HistoryCount
                    PrivateProfile.SetValue(this.ini_WOL_Section, this.ini_Histories_Key, this.Histories.ToString(), this.SettingFilePath);

                    // Write Updated Mac Address List
                    PrivateProfile.SetValue(this.ini_WOL_Section, this.ini_MacAddressList_Key, this.getUpdatedMacAddressListAsString(), this.SettingFilePath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Get Updated MacAddressList As String
        private string getUpdatedMacAddressListAsString()
        {
            // Update MAC Address List
            this.updateMacAddressList();

            return this.getMacAddressListAsString();
        }


        // Get MacAddressList (ObservableCollection) As String
        private string getMacAddressListAsString()
        {
            StringBuilder list = new StringBuilder();
            int count = (MainWindow.MacAddressList.Count < this.Histories) ? MainWindow.MacAddressList.Count : this.Histories;

            for (int i = 0; i < count; i++)
            {
                if (i > 0) { list.Append(','); }

                list.Append(MainWindow.MacAddressList[i]);
            }

            return list.ToString();
        }


        // Update MAC Address List
        private void updateMacAddressList()
        {
            try
            {
                string current = this.MacAddressComboBox.Text;
                int index = MainWindow.MacAddressList.IndexOf(current);

                // Remove current item, if selected MAC Address is already included.
                if (index >= 0) { MainWindow.MacAddressList.RemoveAt(index); }

                // Insert selected MAC Address
                MainWindow.MacAddressList.Insert(0, current);

                // Remove last item, if over
                if (MainWindow.MacAddressList.Count > this.Histories)
                {
                    MainWindow.MacAddressList.RemoveAt(MainWindow.MacAddressList.Count - 1);
                }

                // Update Current Selection of ComboBox
                this.MacAddressComboBox.Text = MainWindow.MacAddressList[0];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
