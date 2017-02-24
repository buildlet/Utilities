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

using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.IO;


// StackFrame.GetMethod() で、最適化の影響を受けずにコールスタックを維持するための回避策
namespace System.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class DynamicSecurityMethodAttribute : Attribute { }
}


namespace BUILDLet.Utilities.Diagnostics
{
    /// <summary>
    /// ログ出力などに埋め込むためのデバッグ情報の文字列を実装します。
    /// <para>
    /// このクラスは静的クラスです。
    /// 必要に応じて <see cref="DebugInfo.Init"/> メソッドでプロパティの値を初期化してください。
    /// </para>
    /// <para>
    /// このクラスは Version 1.4.0.0 で追加されました。
    /// </para>
    /// </summary>
    /// <remarks>
    /// このクラスはデバッグ ビルドでのみ使用することを推奨します。
    /// <see cref="StackFrame"/> クラスを使用してクラス名やメソッド名の取得しているため、
    /// リリース ビルドでは、コンパイラの最適化のために正しいクラス名やメソッド名が取得できない場合があります。
    /// </remarks>
    public static class DebugInfo
    {
        // inner value
        private static string format;


        /// <summary>
        /// 日付のみを含む既定のフォーマットの書式指定文字列を表します。
        /// 書式指定文字列は "yyyy/MM/dd" です。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const string DefaultDateFormat = "yyyy/MM/dd";


        /// <summary>
        /// 時刻のみを含む既定のフォーマットの書式指定文字列を表します。
        /// 書式指定文字列は "HH:mm:ss" です。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const string DefaultTimeFormat = "HH:mm:ss";


        /// <summary>
        /// 日付と時刻を含む既定のフォーマットの書式指定文字列を表します。
        /// 書式指定文字列は "<see cref="DefaultDateFormat"/> <see cref="DefaultTimeFormat"/>" です。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const string DefaultDateTimeFormat = DebugInfo.DefaultDateFormat + " " + DebugInfo.DefaultTimeFormat;


        /// <summary>
        /// <see cref="DebugInfo"/> クラスの静的コンストラクター
        /// </summary>
        static DebugInfo() { DebugInfo.Init(); }


        /// <summary>
        /// <see cref="DebugInfo"/> クラスの各種静的プロパティを初期化します。
        /// </summary>
        /// <param name="caller">
        /// <see cref="DebugInfo.CallerFormat"/> に設定するクラス名およびメソッド名の形式を指定します。
        /// 既定では <see cref="DebugInfoCallerFormat.ShortName"/> です。
        /// </param>
        /// <param name="time">
        /// <see cref="DebugInfo.DateTimeFormat"/> に設定する時刻のフォーマットの書式指定文字列を指定します。
        /// 既定では <see cref="DefaultTimeFormat"/> です。
        /// </param>
        public static void Init(
            DebugInfoCallerFormat caller = DebugInfoCallerFormat.ShortName,
            string time = DebugInfo.DefaultTimeFormat)
        {
            DebugInfo.CallerFormat = caller;
            DebugInfo.DateTimeFormat = time;
        }


        /// <summary>
        /// 表示する時刻のフォーマットの書式指定文字列を指定します。
        /// </summary>
        public static string DateTimeFormat
        {
            get { return DebugInfo.format; }
            set
            {
                try { System.DateTime.Now.ToString(DebugInfo.format = value); }
                catch (Exception) { throw; }
            }
        }


        /// <summary>
        /// 表示するクラス名およびメソッド名の形式を取得または指定します。
        /// </summary>
        public static DebugInfoCallerFormat CallerFormat { get; set; }


        /// <summary>
        /// 現在時刻の文字列を取得します。
        /// </summary>
        /// <param name="format">
        /// 取得する時刻のフォーマットの書式指定文字列を指定します。
        /// null を指定すると、現在の <see cref="DebugInfo.DateTimeFormat"/> が設定されます。
        /// 既定では null です。
        /// </param>
        /// <returns>現在時刻の文字列</returns>
        public static string GetDateTime(string format = null)
        {
            try
            {
                return System.DateTime.Now.ToString((format ?? DebugInfo.DateTimeFormat), new CultureInfo("en-US"));
            }
            catch (Exception) { throw; }
        }

        
        /// <summary>
        /// 書式のクラス名およびメソッド名を取得します。
        /// </summary>
        /// <param name="format">
        /// 取得する書式のクラス名およびメソッド名のフォーマットを指定します。
        /// null を指定すると、現在の <see cref="DebugInfo.CallerFormat"/> が設定されます。
        /// 既定では null です。
        /// </param>
        /// <param name="skipFrames">
        /// スキップするスタック上のフレーム数。
        /// <para>
        /// 既定のフレーム数は 1 です。
        /// フレーム数 1 を指定することで、このメソッドをコールしたクラス名およびメソッド名を取得できます。
        /// </para>
        /// </param>
        /// <returns>
        /// 書式のクラス名およびメソッド名。
        /// </returns>
        [DynamicSecurityMethod]
        public static string GetCallerName(DebugInfoCallerFormat? format = null, int skipFrames = 1)
        {
            StackFrame sf = new StackFrame(skipFrames);
            string fullname = sf.GetMethod().ReflectedType.FullName;
            string name = sf.GetMethod().ReflectedType.Name;
            string methodName = sf.GetMethod().Name;
            string callerName = string.Empty;

            switch (format ?? DebugInfo.CallerFormat)
            {
                // case DebugInfoCallerFormat.None:
                //     break;

                case DebugInfoCallerFormat.Name:
                    callerName = methodName;
                    break;

                case DebugInfoCallerFormat.ShortName:
                    callerName = name + "." + methodName;
                    break;
                case DebugInfoCallerFormat.FullName:
                    callerName = fullname + "." + methodName;
                    break;

                case DebugInfoCallerFormat.ClassName:
                    callerName = name;
                    break;

                case DebugInfoCallerFormat.FullClassName:
                    callerName = fullname;
                    break;

                default:
                    break;
            }

            return callerName;
        }


        /// <summary>
        /// デバッグ情報として、日付のみを文字列として取得します。
        /// 表示する日付のフォーマットの書式指定文字列は <see cref="DefaultDateFormat"/> です。
        /// </summary>
        public static string Date
        {
            get
            {
                return DebugInfo.GetDateTime(DebugInfo.DefaultDateFormat);
            }
        }


        /// <summary>
        /// デバッグ情報として、時刻のみを文字列として取得します。
        /// 表示する時刻のフォーマットの書式指定文字列は <see cref="DefaultTimeFormat"/> です。
        /// </summary>
        public static string Time
        {
            get
            {
                return DebugInfo.GetDateTime(DebugInfo.DefaultTimeFormat);
            }
        }


        /// <summary>
        /// デバッグ情報として、日付と時刻のみを文字列として取得します。
        /// 表示する日付時刻のフォーマットの書式指定文字列は <see cref="DefaultDateTimeFormat"/> です。
        /// </summary>
        public static string DateTime
        {
            get
            {
                return DebugInfo.GetDateTime(DebugInfo.DefaultDateTimeFormat);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のメソッド名のみを文字列として取得します。
        /// </summary>
        public static string Name
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerFormat.Name, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名とメソッド名のみを文字列として取得します。
        /// </summary>
        public static string ShortName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerFormat.ShortName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のメソッド名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerFormat.FullName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名のみを文字列として取得します。
        /// </summary>
        public static string ClassName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerFormat.ClassName, 2);
            }
        }


        /// <summary>
        /// デバッグ情報として、呼び出し元のクラス名の完全修飾名のみを文字列として取得します。
        /// </summary>
        public static string FullClassName
        {
            get
            {
                return DebugInfo.GetCallerName(DebugInfoCallerFormat.FullClassName, 2);
            }
        }



        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>デバッグ情報の文字列</returns>
        public static new string ToString()
        {
            return DebugInfo.GetDateTime() + ", " + DebugInfo.GetCallerName(null, 2);
        }
    }
}
