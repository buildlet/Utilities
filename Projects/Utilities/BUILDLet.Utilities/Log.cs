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
    public static class Log
    {
        private static TextWriter error;
        private static string format;
        private static char[] bracket;

        private static void init()
        {
            Log.error = null;
            Log.format = "HH:mm:ss";
            Log.bracket = new char[] {'[', ']'};

            Log.MethodName = true;
            Log.TimeStamp = false;
            Log.OutputStream = LogOutputStream.StandardOutput;
        }


        static Log() { Log.init(); }


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
        /// 既定の設定は <see cref="BUILDLet.Utilities.LogOutputStream.StandardOutput"/> です。
        /// </summary>
        public static LogOutputStream OutputStream { get; set; }

        /// <summary>
        /// ログに出力される時刻のフォーマットの書式指定文字列を取得または設定します。
        /// 既定では "HH:mm:ss" です。
        /// </summary>
        public static string TimeStampFormat
        {
            get { return Log.format; }
            set
            {
                // Validate
                try { DateTime.Now.ToString(value); }
                catch (Exception e) { throw e; }

                Log.format = value;
            }
        }

        /// <summary>
        /// ログに出力される時刻やメソッド名を囲む文字の既定の設定を取得します。
        /// 既定では { '[', ']' } です。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Char 配列の長さが 2 ではありません。</exception>
        public static char[] Bracket
        {
            get { return Log.bracket; }
            set
            {
                // Validation
                if (value.Length != 2) { throw new ArgumentOutOfRangeException(); }

                Log.bracket = value;
            }
        }


        /// <summary>
        /// <see cref="Log"/> クラスの状態を初期値に戻します。
        /// </summary>
        public static void Clear()
        {
            Log.init();
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
        /// 既定では <see cref="BUILDLet.Utilities.Log.TimeStampFormat"/> です。
        /// </param>
        /// <param name="bracket">
        /// ログに出力される時刻やメソッド名を囲む文字を指定します。
        /// 既定では <see cref="BUILDLet.Utilities.Log.Bracket"/> です。
        /// </param>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.Log.OutputStream"/> です。
        /// </param>
        /// <param name="caller">
        /// このクラスの新しいインスタンスを初期化したメソッドの名前が格納されます。
        /// 通常は指定しないでください。
        /// </param>
        public static void WriteLine(string message, bool? method = null, bool? time = null,
            string format = null, char[] bracket = null, LogOutputStream? stream = null, [CallerMemberName] string caller = "")
        {
            Log.Write(message + "\n", method, time, format, bracket, stream, caller);
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
        /// 既定では <see cref="BUILDLet.Utilities.Log.TimeStampFormat"/> です。
        /// </param>
        /// <param name="bracket">
        /// ログに出力される時刻やメソッド名を囲む文字を指定します。
        /// 既定では <see cref="BUILDLet.Utilities.Log.Bracket"/> です。
        /// </param>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.Log.OutputStream"/> です。
        /// </param>
        /// <param name="caller">
        /// このクラスの新しいインスタンスを初期化したメソッドの名前が格納されます。
        /// 通常は指定しないでください。
        /// </param>
        public static void Write(string message, bool? method = null, bool? time = null, 
            string format = null, char[] bracket = null, LogOutputStream? stream = null, [CallerMemberName] string caller = "")
        {
            try
            {
                if (method != null) { Log.MethodName = (bool)method; }
                if (time != null) { Log.TimeStamp = (bool)time; }
                if (format != null) { Log.TimeStampFormat = format; }
                if (bracket != null) { Log.Bracket = bracket; }
                if (stream != null) { Log.OutputStream = (LogOutputStream)stream; }

                string text
                    = ((Log.MethodName || Log.TimeStamp) ? Log.Bracket[0].ToString() : string.Empty)  // "[" or ""
                    + (Log.TimeStamp ? DateTime.Now.ToString(Log.TimeStampFormat) : string.Empty)     // Time Stamp or ""
                    + ((Log.MethodName && Log.TimeStamp) ? ": " : string.Empty)                       // ": " or ""
                    + (Log.MethodName ? caller : string.Empty)                                        // Method Name
                    + ((Log.MethodName || Log.TimeStamp) ? (Log.Bracket[1] + " ") : string.Empty)     // "] " or ""
                    + message;


                // Output to Stream
                Log.write(text);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 行終端記号をログ出力に書き込みます。 
        /// </summary>
        /// <param name="stream">
        /// ログ出力を書き込む出力ストリームを設定します。
        /// 既定では <see cref="BUILDLet.Utilities.Log.OutputStream"/> です。
        /// </param>
        public static void WriteLine(LogOutputStream? stream = null)
        {
            if (stream != null) { Log.OutputStream = (LogOutputStream)stream; }

            Log.write("\n");
        }



        private static void write(string text)
        {
            try
            {
                switch (Log.OutputStream)
                {
                    case LogOutputStream.StandardOutput:
                        Console.Write(text);
                        break;

                    case LogOutputStream.StandardError:
                        if (Log.error == null) { Log.error = Console.Error; }
                        error.Write(text);
                        break;

                    case LogOutputStream.Trace:
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
