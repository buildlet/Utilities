﻿/*******************************************************************************
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
    /// アセンブリの情報を取得します。
    /// </summary>
    public class AssemblyAttributes
    {
        private Assembly assembly;


        /// <summary>
        /// <see cref="AssemblyAttributes"/> クラスの新しいインスタンスを初期化します。 
        /// このメソッドを呼び出したメソッドのコードを格納しているアセンブリが格納されます。
        /// </summary>
        public AssemblyAttributes() : this(Assembly.GetCallingAssembly()) { }

        /// <summary>
        /// <see cref="AssemblyAttributes"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="assembly">情報を取得するアセンブリを指定します。</param>
        public AssemblyAttributes(Assembly assembly) { this.assembly = assembly; }

        
        /// <summary>
        /// アセンブリの簡易名 (<see cref="System.Reflection.AssemblyName.Name"/>) を取得します。
        /// </summary>
        public string Name
        {
            get { return assembly.GetName().Name; }
        }

        /// <summary>
        /// アセンブリの完全名 (<see cref="System.Reflection.AssemblyName.FullName"/>) を取得します。
        /// </summary>
        public string FullName
        {
            get { return assembly.GetName().FullName; }
        }

        /// <summary>
        /// アセンブリのバージョン (<see cref="System.Reflection.AssemblyName.Version"/>) を取得します。
        /// </summary>
        public Version Version
        {
            get { return assembly.GetName().Version; }
        }

        /// <summary>
        /// アセンブリに関連付けられたカルチャ (<see cref="System.Reflection.AssemblyName.CultureInfo"/>) を取得します。
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return assembly.GetName().CultureInfo; }
        }

        /// <summary>
        /// アセンブリに関連付けられたカルチャの名前 (<see cref="System.Reflection.AssemblyName.CultureName"/>) を取得します。
        /// </summary>
        public string CultureName
        {
            get { return assembly.GetName().CultureName; }
        }
    }
}
