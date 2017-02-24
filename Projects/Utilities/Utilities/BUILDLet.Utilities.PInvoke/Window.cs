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

using System.Runtime.InteropServices;


namespace BUILDLet.Utilities.PInvoke
{
    /// <summary>
    /// Win32 API の ウィンドウの関数の一部を実装します。
    /// <para>
    /// このクラスは Version 1.3.0.0 で追加されました。
    /// </para>
    /// <para>
    /// このクラスの名前は Version 1.4.0.0 で、<c>Win32</c> から <c>Window</c> に変更されました。
    /// </para>
    /// </summary>
    public class Window
    {
        /// <summary>
        /// <c>GetDesktopWindow()</c> API を実装します。
        /// </summary>
        /// <returns>
        /// デスクトップ ウィンドウのハンドルを返します。
        /// </returns>
        /// <remarks>
        /// デスクトップ ウィンドウのハンドルを取得します。
        /// デスクトップ ウィンドウはスクリーン全体を覆っており、この上にアイコンやウィンドウなどが描画されます。
        /// <para>
        /// オリジナルの <c>GetDesktopWindow()</c> API の定義は次に示す通りです。
        /// <code language="CPP" title="C++">
        ///     HWND WINAPI GetDesktopWindow(void);
        /// </code>
        /// </para>
        /// <para>
        /// 詳細は
        /// <a href="https://msdn.microsoft.com/ja-jp/library/cc364616.aspx" target="_blank">GetDesktopWindow 関数</a>
        /// または
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633504.aspx" target="_blank">GetDesktopWindow function (Windows)</a>
        /// を参照してください。
        /// </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
    }
}
