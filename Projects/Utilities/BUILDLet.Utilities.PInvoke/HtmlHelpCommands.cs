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

namespace BUILDLet.Utilities.PInvoke
{
    /// <summary>
    /// HTML Help API がサポートする commands (<c>uCommand</c> パラメーターの値) を実装します。
    /// <para>
    /// この列挙体は Version 1.3.0.0 で追加されました。
    /// </para>
    /// </summary>
    /// <remarks>
    /// 詳細は
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644704.aspx" target="_blank">About Commands (Windows)</a>
    /// を参照してください。
    /// </remarks>
    public enum HtmlHelpCommands : uint
    {
        /// <summary>
        /// Opens a help topic in a specified help window.
        /// </summary>
        HH_DISPLAY_TOPIC = 0x0000,

        /// <summary>
        /// Selects the Contents tab in the Navigation pane of the HTML Help Viewer.
        /// </summary>
        HH_DISPLAY_TOC = 0x0001,

        /// <summary>
        /// Selects the Index tab in the Navigation pane of the HTML Help Viewer and searches for the keyword specified in the <c>dwData</c> parameter.
        /// </summary>
        HH_DISPLAY_INDEX = 0x0002,

        /// <summary>
        /// Selects the Search tab in the Navigation pane of the HTML Help Viewer, but does not actually perform a search.
        /// </summary>
        HH_DISPLAY_SEARCH = 0x0003,

        /// <summary>
        /// Displays a help topic based on a mapped topic ID.
        /// </summary>
        HH_HELP_CONTEXT = 0x000F,

        /// <summary>
        /// Closes all windows opened directly or indirectly by the calling program.
        /// </summary>
        HH_CLOSE_ALL = 0x0012,
        

        // HH_INITIALIZE = 0x001C,
        // HH_UNINITIALIZE = 0x001D
    }
}
