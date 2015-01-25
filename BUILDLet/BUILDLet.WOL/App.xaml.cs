using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.IO;

using BUILDLet.Utilities;

namespace BUILDLet.WOL
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static string Name
        {
            get { return new AssemblyCustomAttributes().AssemblyProductAttribute; }
        }

        public string GetDefaultMacAddress()
        {
            // Get File path
            string filepath = Utilities.FileAccess.GetFilePath(BUILDLet.WOL.Properties.Resources.ConfigurationFileName);

            // File is not found.
            if (string.IsNullOrEmpty(filepath)) { return string.Empty; }

            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    string line = sr.ReadLine();

                    // Return
                    if (line.Length > 17) { return line.Substring(0, 17); }
                    else { return line; }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }
    }
}
