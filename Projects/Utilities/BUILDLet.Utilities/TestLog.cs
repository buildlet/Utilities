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

using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// 標準出力ストリーム、標準エラー出力ストリーム、または、トレース リスナーへのログ出力を実装します。
    /// </summary>
    /// <remarks>
    /// このクラスの想定される適用範囲を明示するために、Version 1.3.0.0 で、<c>Log</c> クラスから <c>TestLog</c> クラスへ名前を変更しました。
    /// </remarks>
    public static class TestLog
    {
        private static TextWriter error;
        private static string format;
        private static char[] bracket;

        private static void init()
        {
            TestLog.error = null;
            TestLog.format = "HH:mm:ss";
            TestLog.bracket = new char[] {'[', ']'};

            TestLog.MethodName = true;
            TestLog.TimeStamp = false;
            TestLog.OutputStream = TestLogOutputStream.StandardOutput;
        }


        static TestLog() { TestLog.init(); }


        /// <summary>
        /// ログ出力にメソッド名を含めるかどうかを取得または設定します。
        /// ログ出力にメソッド名を含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </summary>
        public static bool MethodName { get; set; }

        /// <summary>
        /// ログ出力に時刻を含めるかどうかを取得または設定します。
        /// ログ出力に時刻を含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </summary>
        public static bool TimeStamp { get; set; }

        /// <summary>
        /// ログ出力を書き込む出力ストリームを取得または設定します。
        /// 既定の設定は <see cref="BUILDLet.Utilities.TestLogOutputStream.StandardOutput"/> です。
        /// </summary>
        public static TestLogOutputStream OutputStream { get; set; }

        /// <summary>
        /// ログに出力される時刻のフォーマットの書式指定文字列を取得または設定します。
        /// 既定では "HH:mm:ss" です。
        /// </summary>
        public static string TimeStampFormat
        {
            get { return TestLog.format; }
            set
            {
                // Validate
                try { DateTime.Now.ToString(value); }
                catch (Exception e) { throw e; }

                TestLog.format = value;
            }
        }

        /// <summary>
        /// ログに出力される時刻やメソッド名を囲む文字の既定の設定を取得します。
        /// 既定では { '[', ']' } です。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Char 配列の長さが 2 ではありません。</exception>
        public static char[] Bracket
        {
            get { return TestLog.bracket; }
            set
            {
                // Validation
                if (value.Length != 2) { throw new ArgumentOutOfRangeException(); }

                TestLog.bracket = value;
            }
        }


        /// <summary>
        /// <see cref="TestLog"/> クラスの状態を初期値に戻します。
        /// </summary>
        public static void Clear()
        {
            TestLog.init();
        }


        /// <summary>
        /// 指定した文字列値をログ出力に書き込み、続けて行終端記号を書き込みます。
        /// </summary>
        /// <param name="message">書き込む値</param>
        /// <param name="method">
        /// このメソッドの呼び出し元のメソッドの名前を出力に含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </param>
        /// <param name="time">
        /// 時刻をログ出力に含める場合は true を指定します。
        /// 既定の設定は false です。
        /// </param>
        /// <param name="format">
        /// ログに出力される時刻のフォーマットの書式指定文字列を設定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.TimeStampFormat"/> です。
        /// </param>
        /// <param name="bracket">
        /// ログに出力される時刻やメソッド名を囲む文字を指定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.Bracket"/> です。
        /// </param>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.OutputStream"/> です。
        /// </param>
        /// <param name="caller">
        /// このクラスの新しいインスタンスを初期化したメソッドの名前が格納されます。
        /// 通常は指定しないでください。
        /// </param>
        public static void WriteLine(string message, bool? method = null, bool? time = null,
            string format = null, char[] bracket = null, TestLogOutputStream? stream = null, [CallerMemberName] string caller = "")
        {
            TestLog.Write(message + "\n", method, time, format, bracket, stream, caller);
        }


        /// <summary>
        /// 指定した文字列値をログ出力に書き込みます。 
        /// </summary>
        /// <param name="message">書き込む値</param>
        /// <param name="method">
        /// このメソッドの呼び出し元のメソッドの名前を出力に含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </param>
        /// <param name="time">
        /// 時刻をログ出力に含める場合は true を指定します。
        /// 既定の設定は false です。
        /// </param>
        /// <param name="format">
        /// ログに出力される時刻のフォーマットの書式指定文字列を設定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.TimeStampFormat"/> です。
        /// </param>
        /// <param name="bracket">
        /// ログに出力される時刻やメソッド名を囲む文字を指定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.Bracket"/> です。
        /// </param>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.OutputStream"/> です。
        /// </param>
        /// <param name="caller">
        /// このクラスの新しいインスタンスを初期化したメソッドの名前が格納されます。
        /// 通常は指定しないでください。
        /// </param>
        public static void Write(string message, bool? method = null, bool? time = null, 
            string format = null, char[] bracket = null, TestLogOutputStream? stream = null, [CallerMemberName] string caller = "")
        {
            try
            {
                if (method != null) { TestLog.MethodName = (bool)method; }
                if (time != null) { TestLog.TimeStamp = (bool)time; }
                if (format != null) { TestLog.TimeStampFormat = format; }
                if (bracket != null) { TestLog.Bracket = bracket; }
                if (stream != null) { TestLog.OutputStream = (TestLogOutputStream)stream; }

                string text
                    = ((TestLog.MethodName || TestLog.TimeStamp) ? TestLog.Bracket[0].ToString() : string.Empty)  // "[" or ""
                    + (TestLog.TimeStamp ? DateTime.Now.ToString(TestLog.TimeStampFormat) : string.Empty)     // Time Stamp or ""
                    + ((TestLog.MethodName && TestLog.TimeStamp) ? ": " : string.Empty)                       // ": " or ""
                    + (TestLog.MethodName ? caller : string.Empty)                                        // Method Name
                    + ((TestLog.MethodName || TestLog.TimeStamp) ? (TestLog.Bracket[1] + " ") : string.Empty)     // "] " or ""
                    + message;


                // Output to Stream
                TestLog.write(text);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 行終端記号をログ出力に書き込みます。 
        /// </summary>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.TestLog.OutputStream"/> です。
        /// </param>
        public static void WriteLine(TestLogOutputStream? stream = null)
        {
            if (stream != null) { TestLog.OutputStream = (TestLogOutputStream)stream; }

            TestLog.write("\n");
        }



        private static void write(string text)
        {
            try
            {
                switch (TestLog.OutputStream)
                {
                    case TestLogOutputStream.StandardOutput:
                        Console.Write(text);
                        break;

                    case TestLogOutputStream.StandardError:
                        if (TestLog.error == null) { TestLog.error = Console.Error; }
                        error.Write(text);
                        break;

                    case TestLogOutputStream.Trace:
                        Trace.Write(text);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
