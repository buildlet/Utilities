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

using System.Collections.ObjectModel;
using System.IO;

namespace BUILDLet.WOL
{
    public class DefaultMacAddressList : ObservableCollection<string>
    {
        private int macLength = 17;
        private int maxHist = int.Parse(Properties.Resources.HistoryCount);
        
        public DefaultMacAddressList()
        {
            // Get default MAC Address
            string[] addresses = this.getDefaultMacAddresses();


            if (addresses != null)
            {
                foreach (var addr in addresses)
                {
                    this.Add(addr);
                }
            }
        }


        public string SourceFilePath { get; protected set; }


        public void UpdateSourceFile(string address)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.SourceFilePath))
                {
                    // Update MAC address list
                    this.updateMacAddresses(address);


                    // Write to configuration file
                    using (StreamWriter sw = new StreamWriter(this.SourceFilePath))
                    {
                        foreach (var item in this)
                        {
                            sw.WriteLine(item);
                        }
                    }
                }
            }
            catch (Exception e) { throw e; }
        }


        private void updateMacAddresses(string address)
        {
            if (this.IndexOf(address) < 0)
            {
                // Add current item
                this.Insert(0, address);

                // Remove last item
                if (this.Count > this.maxHist) { this.RemoveItem(this.Count - 1); }
            }
        }


        private string[] getDefaultMacAddresses()
        {
            // Set search path of configuration file
            string[] searchPath =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Properties.Resources.ConfigurationFileFolder),
                Environment.CurrentDirectory
            };

            // Get file path
            this.SourceFilePath = Utilities.LocalFile.GetFilePath(Properties.Resources.ConfigurationFileName, searchPath);

            // File is not found.
            if (string.IsNullOrEmpty(this.SourceFilePath)) { return null; }


            try
            {
                using (StreamReader sr = new StreamReader(this.SourceFilePath))
                {
                    int lines = 0;
                    string line;
                    List<string> addresses = new List<string>();


                    while ((line = sr.ReadLine()) != null)
                    {
                        // Maximum count
                        if (++lines > this.maxHist) { break; }


                        if (line.Length > this.macLength) { addresses.Add(line.Substring(0, this.macLength)); }
                        else { addresses.Add(line); }
                    }


                    // Return
                    if (addresses.Count == 0) { return null; }
                    else { return addresses.ToArray(); }
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
