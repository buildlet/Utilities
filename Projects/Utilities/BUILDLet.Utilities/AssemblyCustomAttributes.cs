using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Globalization;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// アセンブリのカスタム属性を取得します。
    /// </summary>
    public class AssemblyCustomAttributes
    {
        private Assembly assembly;


        /// <summary>
        /// <see cref="AssemblyCustomAttributes"/> クラスの新しいインスタンスを初期化します。 
        /// このメソッドを呼び出したメソッドのコードを格納しているアセンブリが格納されます。
        /// </summary>
        public AssemblyCustomAttributes() : this(Assembly.GetCallingAssembly()) { }

        /// <summary>
        /// <see cref="AssemblyCustomAttributes"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="assembly">カスタム属性を取得を取得するアセンブリを指定します。</param>
        public AssemblyCustomAttributes(Assembly assembly) { this.assembly = assembly; }


        /// <summary>
        /// アセンブリのタイトル (<see cref="System.Reflection.AssemblyTitleAttribute.Title"/>) を取得します。
        /// </summary>
        public string AssemblyTitleAttribute
        {
            get { return ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title; }
        }

        /// <summary>
        /// アセンブリの説明 (<see cref="System.Reflection.AssemblyDescriptionAttribute.Description"/>) を取得します。
        /// </summary>
        public string AssemblyDescriptionAttribute
        {
            get { return ((AssemblyDescriptionAttribute)assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute))).Description; }
        }

        /// <summary>
        /// アセンブリの会社名に関するカスタム属性 (<see cref="System.Reflection.AssemblyCompanyAttribute.Company"/>) を取得します。
        /// </summary>
        public string AssemblyCompanyAttribute
        {
            get { return ((AssemblyCompanyAttribute)assembly.GetCustomAttribute(typeof(AssemblyCompanyAttribute))).Company; }
        }

        /// <summary>
        /// アセンブリの製品名に関するカスタム属性 (<see cref="System.Reflection.AssemblyProductAttribute.Product"/>) を取得します。
        /// </summary>
        public string AssemblyProductAttribute
        {
            get { return ((AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute))).Product; }
        }

        /// <summary>
        /// アセンブリの著作権に関するカスタム属性 (<see cref="System.Reflection.AssemblyCopyrightAttribute.Copyright"/>) を取得します。
        /// </summary>
        public string AssemblyCopyrightAttribute
        {
            get { return ((AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright; }
        }

        /// <summary>
        /// アセンブリの商標に関するカスタム属性 (<see cref="System.Reflection.AssemblyTrademarkAttribute.Trademark"/>) を取得します。
        /// </summary>
        public string AssemblyTrademarkAttribute
        {
            get { return ((AssemblyTrademarkAttribute)assembly.GetCustomAttribute(typeof(AssemblyTrademarkAttribute))).Trademark; }
        }

        /// <summary>
        /// アセンブリの Win32 ファイルバージョン (<see cref="System.Reflection.AssemblyFileVersionAttribute.Version"/>) を取得します。
        /// </summary>
        public string AssemblyFileVersionAttribute
        {
            get { return ((AssemblyFileVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version; }
        }


        /// <summary>
        /// アセンブリに関連付けられた各種情報を文字列として取得します。
        /// </summary>
        /// <returns>指定したアセンブリに関連付けられた各種情報を文字列として取得します。</returns>
        /// <remarks>取得される文字列には次の情報が含まれます。
        /// <para>
        /// <see cref="System.Reflection.AssemblyName.FullName"/> (Name, Version, Culture, PublicKeyToken), 
        /// <see cref="System.Reflection.AssemblyTitleAttribute.Title"/>, <see cref="System.Reflection.AssemblyDescriptionAttribute.Description"/>, 
        /// <see cref="System.Reflection.AssemblyCompanyAttribute.Company"/>, <see cref="System.Reflection.AssemblyProductAttribute.Product"/>, 
        /// <see cref="System.Reflection.AssemblyCopyrightAttribute.Copyright"/>, <see cref="System.Reflection.AssemblyTrademarkAttribute.Trademark"/>, 
        /// <see cref="System.Reflection.AssemblyFileVersionAttribute.Version"/>
        /// </para>
        /// </remarks>
        public override string ToString()
        {
            return assembly.GetName().FullName
                + string.Format(", Title=\"{0}\"", this.AssemblyTitleAttribute)
                + string.Format(", Description=\"{0}\"", this.AssemblyDescriptionAttribute)
                + string.Format(", Company=\"{0}\"", this.AssemblyCompanyAttribute)
                + string.Format(", Product=\"{0}\"", this.AssemblyProductAttribute)
                + string.Format(", Copyright=\"{0}\"", this.AssemblyCopyrightAttribute)
                + string.Format(", Trademark=\"{0}\"", this.AssemblyTrademarkAttribute)
                + string.Format(", FileVersion=\"{0}\"", this.AssemblyFileVersionAttribute);
        }
    }
}
