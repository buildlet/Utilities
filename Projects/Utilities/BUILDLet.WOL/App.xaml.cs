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
    }
}
