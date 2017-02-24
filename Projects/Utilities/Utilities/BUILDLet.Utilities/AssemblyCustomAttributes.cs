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
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyTitleAttribute</c> から <c>Title</c> に変更されました。
        /// </para>
        /// </summary>
        public string Title
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
                return ((attribute != null) ? ((AssemblyTitleAttribute)attribute).Title : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの説明 (<see cref="System.Reflection.AssemblyDescriptionAttribute.Description"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyDescriptionAttribute</c> から <c>Description</c> に変更されました。
        /// </para>
        /// </summary>
        public string Description
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute));
                return ((attribute != null) ? ((AssemblyDescriptionAttribute)attribute).Description : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの会社名に関するカスタム属性 (<see cref="System.Reflection.AssemblyCompanyAttribute.Company"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyCompanyAttribute</c> から <c>Company</c> に変更されました。
        /// </para>
        /// </summary>
        public string Company
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyCompanyAttribute));
                return ((attribute != null) ? ((AssemblyCompanyAttribute)attribute).Company : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの製品名に関するカスタム属性 (<see cref="System.Reflection.AssemblyProductAttribute.Product"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyProductAttribute</c> から <c>Product</c> に変更されました。
        /// </para>
        /// </summary>
        public string Product
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyProductAttribute));
                return ((attribute != null) ? ((AssemblyProductAttribute)attribute).Product : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの著作権に関するカスタム属性 (<see cref="System.Reflection.AssemblyCopyrightAttribute.Copyright"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyCopyrightAttribute</c> から <c>Copyright</c> に変更されました。
        /// </para>
        /// </summary>
        public string Copyright
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
                return ((attribute != null) ? ((AssemblyCopyrightAttribute)attribute).Copyright : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの商標に関するカスタム属性 (<see cref="System.Reflection.AssemblyTrademarkAttribute.Trademark"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyTrademarkAttribute</c> から <c>Trademark</c> に変更されました。
        /// </para>
        /// </summary>
        public string Trademark
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyTrademarkAttribute));
                return ((attribute != null) ? ((AssemblyTrademarkAttribute)attribute).Trademark : string.Empty);
            }
        }

        /// <summary>
        /// アセンブリの Win32 ファイルバージョン (<see cref="System.Reflection.AssemblyFileVersionAttribute.Version"/>) を取得します。
        /// <para>
        /// このプロパティの名前は Version 1.4.0.0 で <c>AssemblyFileVersionAttribute</c> から <c>FileVersion</c> に変更されました。
        /// </para>
        /// </summary>
        public string FileVersion
        {
            get
            {
                Attribute attribute = assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
                return ((attribute != null) ? ((AssemblyFileVersionAttribute)attribute).Version : string.Empty);
            }
        }


        /// <summary>
        /// アセンブリに関連付けられた各種情報を文字列として取得します。
        /// </summary>
        /// <returns>
        /// 指定したアセンブリに関連付けられた各種情報を文字列として取得します。
        /// </returns>
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
                + string.Format(", Title=\"{0}\"", this.Title)
                + string.Format(", Description=\"{0}\"", this.Description)
                + string.Format(", Company=\"{0}\"", this.Company)
                + string.Format(", Product=\"{0}\"", this.Product)
                + string.Format(", Copyright=\"{0}\"", this.Copyright)
                + string.Format(", Trademark=\"{0}\"", this.Trademark)
                + string.Format(", FileVersion=\"{0}\"", this.FileVersion);
        }
    }
}
