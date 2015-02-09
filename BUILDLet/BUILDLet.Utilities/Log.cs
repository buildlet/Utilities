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
    public class Log
    {
        private string caller;
        private TextWriter error;

        private string method
        {
            get { return (this.MethodBracket[0] + this.caller + this.MethodBracket[1]); }

        }

        private string time
        {
            get { return (this.TimeBracket[0] + DateTime.Now.ToString("HH:mm:ss") + this.TimeBracket[1]); }
        }


        /// <summary>
        ///     <see cref="BUILDLet.Utilities.Log"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="methodName">
        ///     ログ出力にメソッド名を含める場合は true を指定します。
        ///     既定の設定は true です。
        /// </param>
        /// <param name="timeStamp">
        ///     ログ出力に時刻を含める場合は true を指定します。
        ///     既定の設定は false です。
        /// </param>
        /// <param name="title">
        ///     このクラスを初期化したしたときに、メソッド名を表示する場合に指定します。
        ///     既定の設定は false です。
        /// </param>
        /// <param name="methodBracket">
        ///     ログ出力のメソッド名を囲む文字を指定します。
        ///     既定では { '[', ']' } です。
        /// </param>
        /// <param name="timeBracket">
        ///     ログ出力の時刻を囲む文字を指定します。
        ///     既定では { '[', ']' } です。
        /// </param>
        /// <param name="caller">
        ///     このクラスの新しいインスタンスを初期化したメソッドの名前が格納されます。
        ///     通常は指定しないでください。
        /// </param>
        /// <param name="stream">
        ///     ログ出力を書き込む出力ストリームを設定します。
        /// </param>
        /// <remarks>
        ///     初期化時に空行1行を既定のログ出力に書き込みます。
        /// </remarks>
        public Log(bool methodName = true, bool timeStamp = false, bool title = false,
            LogOutputStream stream = LogOutputStream.StandardOutput,
            [CallerMemberName] string caller = "", char[] methodBracket = null, char[] timeBracket = null)
        {
            // MethodName
            this.caller = caller;

            // set logging option
            this.MethodName = methodName;
            this.TimeStamp = timeStamp;


            // set bracket of Method
            if (methodBracket == null) { this.MethodBracket = new char[] { '[', ']' }; }
            else
            {
                // Validation
                if (methodBracket.Length != 2) { throw new ArgumentOutOfRangeException(); }

                this.MethodBracket = methodBracket;
            }


            // set bracket of Time
            if (timeBracket == null) { this.TimeBracket = new char[] { '[', ']' }; }
            else
            {
                // Validation
                if (timeBracket.Length != 2) { throw new ArgumentOutOfRangeException(); }

                this.TimeBracket = TimeBracket;
            }


            // set output stream
            this.Outpt = stream;

            // line break
            this.WriteLine();

            // write Title
            if (title) { this.WriteTitle(); }
        }


        /// <summary>
        /// ログ出力のメソッド名を囲む文字を取得します。
        /// </summary>
        public char[] MethodBracket { get; protected set; }

        /// <summary>
        /// ログ出力の時刻を囲む文字を取得します。
        /// </summary>
        public char[] TimeBracket { get; protected set; }


        /// <summary>
        /// ログ出力にメソッド名を含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </summary>
        public bool MethodName { get; set; }

        /// <summary>
        /// ログ出力に時刻を含める場合は true を指定します。
        /// 既定の設定は true です。
        /// </summary>
        public bool TimeStamp { get; set; }

        /// <summary>
        /// ログ出力を書き込む出力ストリームを取得します。
        /// </summary>
        /// <remarks>
        /// 出力ストリームは、<see cref="BUILDLet.Utilities.Log"/> クラスの初期化時に、コンストラクターの引数としてのみ設定することができます。
        /// </remarks>
        public LogOutputStream Outpt { get; protected set; }


        /// <summary>
        /// 指定した書式情報を使用して、指定したオブジェクト配列のテキスト表現をログ出力に書き込み、続けて現在の行終端記号を書き込みます。
        /// <seealso cref="System.Console.WriteLine(String, Object[])"/>
        /// </summary>
        /// <param name="format">
        /// 複合書式指定文字列
        /// <seealso cref="System.Console.WriteLine(String, Object[])"/>
        /// <seealso cref="System.String.Format(String, Object[])"/>
        /// </param>
        /// <param name="args">
        /// format を使用して書き込むオブジェクトの配列
        /// </param>
        public void WriteLine(string format, params object[] args)
        {
            this.Write(format, args);
            this.WriteLine();
        }

        /// <summary>
        /// 指定した書式情報を使用して、指定したオブジェクト配列のテキスト表現をログ出力に書き込みます。
        /// <seealso cref="System.Console.WriteLine(String, Object[])"/>
        /// </summary>
        /// <param name="format">
        /// 複合書式指定文字列
        /// <seealso cref="System.Console.WriteLine(String, Object[])"/>
        /// <seealso cref="System.String.Format(String, Object[])"/>
        /// </param>
        /// <param name="args">
        /// format を使用して書き込むオブジェクトの配列
        /// </param>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(format, args));
        }

        /// <summary>
        /// このクラスの新しいインスタンスを初期化したメソッドの名前をログ出力に書き込み、続けて現在の行終端記号を書き込みます。 
        /// </summary>
        public void WriteTitle()
        {
            this.WriteLine(this.method, false, false);
        }

        /// <summary>
        /// 指定した文字列値をログ出力に書き込み、続けて現在の行終端記号を書き込みます。 
        /// </summary>
        /// <param name="message">書き込む値</param>
        /// <param name="methodName">ログ出力に一時的にメソッド名を含める場合や除去する場合に、true あるいは false を設定します。</param>
        /// <param name="timeStamp">ログ出力に一時的に時刻を含める場合や除去する場合に、true あるいは false を設定します。</param>        
        public void WriteLine(string message, bool? methodName = null, bool? timeStamp = null)
        {
            this.Write(message, methodName, timeStamp);
            this.WriteLine();
        }

        /// <summary>
        /// 現在の行終端記号をログ出力に書き込みます。
        /// </summary>
        public void WriteLine()
        {
            this.Write("\n", false, false);
        }

        /// <summary>
        /// 指定した文字列値をログ出力に書き込みます。
        /// </summary>
        /// <param name="message">書き込む値</param>
        /// <param name="methodName">ログ出力に一時的にメソッド名を含める場合や除去する場合に、true あるいは false を設定します。</param>
        /// <param name="timeStamp">ログ出力に一時的に時刻を含める場合や除去する場合に、true あるいは false を設定します。</param>
        public void Write(string message, bool? methodName = null, bool? timeStamp = null)
        {
            if (methodName == null) { methodName = this.MethodName; }
            if (timeStamp == null) { timeStamp = this.TimeStamp; }

            string separator = (((bool)methodName | (bool)timeStamp) ? ":" : string.Empty);

            string text
                = ((bool)methodName ? this.method : string.Empty)
                + ((bool)timeStamp ? this.time : string.Empty)
                + separator + message;


            // Output to Stream
            switch (this.Outpt)
            {
                case LogOutputStream.StandardOutput:
                    Console.Write(text);
                    break;

                case LogOutputStream.StandardError:
                    if (this.error == null) { this.error = Console.Error; }
                    error.Write(text);
                    break;

                case LogOutputStream.Trace:
                    Trace.Write(text);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
