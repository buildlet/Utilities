/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015, 2016 Daiki Sakamoto

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
using System.IO;


namespace BUILDLet.Utilities.PInvoke
{
    /// <summary>
    /// HTML Help API を実装します。
    /// <para>
    /// このクラスは Version 1.3.0.0 で追加されました。
    /// </para>
    /// </summary>
    public class HtmlHelp
    {
        /// <summary>
        /// HtmlHelp API を実装します。
        /// </summary>
        /// <param name="hwndCaller">
        /// Specifies the handle (<c>hwnd</c>) of the window calling <c>HtmlHelp()</c>.
        /// The help window is owned by this window.
        /// </param>
        /// <param name="pszFile">
        /// Depending on the <c>uCommand</c> value, specifies the file path to either a compiled help (.chm) file,
        /// or a topic file within a specified help file.
        /// <para>
        /// A window type name can also be specified, preceded with a greater-than (>) character.
        /// If the specified command does not require a file, this value may be NULL.
        /// </para>
        /// </param>
        /// <param name="uCommand">
        /// Specifies the <c>command</c> (<see cref="HtmlHelpCommands"/>) to complete.
        /// </param>
        /// <param name="dwData">
        /// Specifies any data that may be required, based on the value of the <c>uCommand</c> parameter.
        /// </param>
        /// <returns>
        /// Depending on the specified <c>uCommand</c> and the result, <c>HtmlHelp()</c> returns one or both of the following:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The handle (<c>hwnd</c>) of the help window.</description>
        ///     </item>
        ///     <item>
        ///         <description>NULL. In some cases, NULL indicates failure; in other cases, NULL indicates that the help window has not yet been created.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        /// オリジナルの <c>HtmlHelp()</c> API の定義は次に示す通りです。
        /// <code language="CPP" title="C++">
        ///     HWND HtmlHelp(
        ///       HWND    hwndCaller,
        ///       LPCSTR  pszFile,
        ///       UINT    uCommand,
        ///       DWORD   dwData) ;
        /// </code>
        /// </para>
        /// 
        /// <para>
        /// 詳細は
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms670172.aspx" target="_blank">About the HTML Help API Function (Windows)</a>
        /// を参照してください。
        /// </para>
        /// 
        /// <para>
        /// <span style="font-weight:bold">Note</span>
        /// When using the HTML Help API, set the stack size of the hosting executable to at least 100k.
        /// If the defined stack size is too small, then the thread created to run HTML Help will also be created with this stack size, 
        /// and failure could result. Optionally, you can remove /STACK from the link command line, 
        /// and remove any STACK setting in the executable's DEF file (default stack size is 1MB in this case).
        /// You can also you can set the stack size using the /Fnumber compiler command (the compiler will pass this to the linker as /STACK).
        /// </para>
        /// 
        /// </remarks>
        [DllImport("hhctrl.ocx", EntryPoint="HtmlHelp")]
        public static extern IntPtr Invoke(IntPtr hwndCaller, string pszFile, uint uCommand, int dwData);


        /// <summary>
        /// HtmlHelp API を使用して HTML Help ファイルを開きます。
        /// <para>
        /// Opens a help topic in a specified help window. 
        /// </para>
        /// <para>
        /// If a window type is not specified, a default window type is used.
        /// If the window type or default window type is open, the help topic replaces the current topic in the window.
        /// </para>
        /// </summary>
        /// <param name="filename">
        /// Specifies a compiled help (.chm) file, or a specific topic within a compiled help file.
        /// To specify a defined window type, insert a greater-than (>) character followed by the name of the window type.
        /// </param>
        /// <param name="topic">
        /// 0 または Help ID を指定してください。
        /// <para>
        /// Specifies NULL or a pointer to a topic within a compiled help file.
        /// </para>
        /// </param>
        /// <returns>
        /// The handle (<c>hwnd</c>) of the help window.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        /// 次に示すコードと同様の処理を実装します。
        /// <code language="CPP" title="C++">
        ///     HWND hwnd =
        ///        HtmlHelp(
        ///                  GetDesktopWindow(),
        ///                  "c:\\help.chm::/intro.htm>mainwin",
        ///                  HH_DISPLAY_TOPIC,
        ///                  NULL) ;
        /// </code>
        /// </para>
        /// 
        /// <para>
        /// 詳細は
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms670084.aspx" target="_blank">HH_DISPLAY_TOPIC command (Windows)</a>
        /// を参照してください。
        /// </para>
        /// 
        /// </remarks>
        public static IntPtr Open(string filename, int topic = 0)
        {
            // Validation
            if (!File.Exists(filename)) { throw new FileNotFoundException(); }

            try
            {
                return HtmlHelp.Invoke(Win32.GetDesktopWindow(), filename, (uint)HtmlHelpCommands.HH_DISPLAY_TOPIC, topic);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// HtmlHelp API を使用して、開かれている HTML Help ファイルを全て閉じます。
        /// <para>
        /// Closes all windows opened directly or indirectly by the calling program.
        /// </para>
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        /// 次に示すコードと同様の処理を実装します。
        /// <code language="CPP" title="C++">
        ///     HWND hwnd =
        ///        HtmlHelp(
        ///                  NULL,
        ///                  NULL,
        ///                  HH_CLOSE_ALL,
        ///                  0) ;
        /// </code>
        /// </para>
        /// 
        /// <para>
        /// 詳細は
        /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms670079.aspx" target="_blank">HH_CLOSE_ALL command (Windows)</a>
        /// を参照してください。
        /// </para>
        /// 
        /// </remarks>
        public static void Close()
        {
            try
            {
                HtmlHelp.Invoke(IntPtr.Zero, null, (uint)HtmlHelpCommands.HH_CLOSE_ALL, 0);
            }
            catch (Exception e) { throw e; }
        }
    }
}
