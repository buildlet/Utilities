﻿/*******************************************************************************
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

namespace BUILDLet.Utilities
{
    /// <summary>
    /// <see cref="BUILDLet.Utilities.TestLog"/> クラスの出力ストリームを表します。
    /// </summary>
    /// <remarks>
    /// Version 1.3.0.0 で、<c>Log</c> クラスを <c>TestLog</c> クラスへ名前を変更したことにともない、
    /// 同じく Version 1.3.0.0 で、<c>LogOutputStream</c> クラスから <c>TestLogOutputStream</c> クラスへ名前を変更しました。。
    /// </remarks>
    public enum TestLogOutputStream
    {
        /// <summary>
        /// 標準出力ストリーム
        /// </summary>
        StandardOutput,

        /// <summary>
        /// 標準エラー出力ストリーム
        /// </summary>
        StandardError,

        /// <summary>
        /// 既定のトレース リスナー
        /// </summary>
        Trace
    }
}