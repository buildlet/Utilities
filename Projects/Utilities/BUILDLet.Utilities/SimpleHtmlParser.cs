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

using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// 簡易的な HTML パーサーを実装します。
    /// </summary>
    public partial class SimpleHtmlParser
    {
        /// <summary>
        /// <see cref="SimpleHtmlParser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected SimpleHtmlParser()
        {
#if DEBUG
            SimpleHtmlParser.DebugPrintLevel = 2;
#endif
        }


#if DEBUG
        /// <summary>
        /// デバッグ ビルドのとき、トレース リスナーに書き込むデバッグ情報のレベルを指定します。
        /// より詳細なデバッグ メッセージを書き込む場合には 2 以上を指定します。
        /// リリース ビルドでは、このパラメーターは無効です。
        /// <para>
        /// このパラメーターに 2 以上を指定すると、パフォーマンスに大きく影響します。
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// <description>0 を指定すると、GetElements() 自身が出力するデバッグ メッセージのみが書き込まれます。</description>
        /// </item>
        /// <item>
        /// <description>1 を指定すると、GetElements() の内部で再帰的にコールされるメソッドが出力するデバッグ メッセージも書き込まれます。</description>
        /// </item>
        /// <item>
        /// <description>2 を指定すると、SimpleHtmlParser.HtmlContent クラスが出力するデバッグ メッセージも書き込まれます。</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        public static int DebugPrintLevel { get; set; }
#endif

        
        /// <summary>
        /// HTML コンテンツからコメントを削除します。
        /// </summary>
        /// <param name="content">コメントを削除する HTML コンテンツ全体</param>
        /// <returns>削除したコメントの数を返します。</returns>
        /// <remarks>
        /// コメントの開始タグに対応する終了タグが見つからなかった場合は、それ以降の処理は行われません。
        /// </remarks>
        public static int RemoveHtmlComment(ref string content)
        {
            // "<!--" を含んでいる
            // "-->" を含んでいる
            // "-->" の先頭のインデックスが "<!--" の後端のインデックスよりも大きい
            if (content.Contains("<!--")
                && content.Contains("-->")
                && (content.IndexOf("<!--") + "<!--".Length) < content.IndexOf("-->"))
            {
                // comment is found!
                // remove comment of 1st (Update contents)

                string head = content.Substring(0, content.IndexOf("<!--"));
                string tail = content.Substring(content.IndexOf("-->") + "-->".Length);
                content = head + tail;

                // next
                return 1 + SimpleHtmlParser.RemoveHtmlComment(ref content);
            }
            else
            {
                // Default
                return 0;
            }
        }


        /// <summary>
        /// HTML コンテンツから、指定された要素の値を取得します。
        /// </summary>
        /// <param name="content">HTML コンテンツ</param>
        /// <param name="name">取り出す要素の名前</param>
        /// <param name="attributes">
        /// 取り出す要素に属性がある場合に、属性の名前と値を指定することができます。
        /// 属性の名前を指定することで、該当する要素を絞り込むことができる場合などに指定します。
        /// 属性の名前を指定しない場合は null を指定します。
        /// 既定の設定は null です。
        /// <para>このパラメーターは Version 1.4.0.0 で追加されました。</para>
        /// </param>
        /// <param name="strict">
        /// &lt;P&gt; 要素のように、終了タグがない場合がある要素を許可するときに false を指定します。
        /// ただし、false を指定した場合は、取得したい要素の値の内側に、同じ名前の要素が入れ子になっている場合や、
        /// 終了タグのない要素の値は正しく取得できない場合があります。
        /// 既定の設定は true です。
        /// </param>
        /// <returns>
        /// 指定した名前にマッチする要素の値を文字列配列として返します。
        /// 条件に一致する要素がない場合は null を返します。
        /// <para>
        /// 指定した条件 (要素と属性) が入れ子になっている場合は、外側の要素を取り出します。
        /// 必要に応じて、取り出した要素から、再度、目的の要素を取り出してください。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 取り出した要素の値に含まれるタブ文字 ("\t") は空白文字に置き換えられます。
        /// また、改行コード ("\n" および "\r") は削除されます。
        /// </remarks>
        public static string[] GetElements(string content, string name, string[,] attributes = null, bool strict = true)
        {
            // Validataion
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(name)) { throw new ArgumentNullException(); }

            // Validataion (Attribute(s))
            if (attributes != null)
            {
                for (int i = 0; i < attributes.Length / 2; i++)
                {
                    if (string.IsNullOrWhiteSpace(attributes[i, 0])) { throw new ArgumentException(); }
                }
            }

#if DEBUG
            DebugInfo.Init();
            Debug.WriteLine("");
            Debug.WriteLine("[{0}] Debug Print Level={1}", DebugInfo.ShortName, DebugPrintLevel);
            Debug.WriteLine("[{0}] Try to Get Value of Element \"{1}\" (Strict Mode={2})", DebugInfo.ShortName, name, strict);

            if (attributes != null)
            {
                for (int i = 0; i < attributes.Length / 2; i++)
                {
                    Debug.WriteLine("[{0}] Attributes[{1}] \"{2}\"=\"{3}\"", DebugInfo.ShortName, i, attributes[i, 0], attributes[i, 1]);
                }
            }
#endif

            // Get Values of HTML Element
            List<string> values = SimpleHtmlParser.get_element_values(new HtmlContent(content, strict, true), name, attributes, strict, 0, 2);

#if DEBUG
            if (values == null)
            {
                Debug.WriteLine("[{0}] Value of Element \"{1}\" is not found.", DebugInfo.ShortName, name);
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    Debug.WriteLine("[{0}] Value of Element \"{1}\"[{2}]=\"{3}\"", DebugInfo.ShortName, name, i, values[i]);
                }
            }
            Debug.WriteLine("");
#endif
            // RETURN
            return values.ToArray();
        }


        /*
        /// <summary>
        /// HTML コンテンツから、指定された要素の属性の値を取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で非サポートとなりました。
        /// このメソッドをコールすると <see cref="NotSupportedException"/> がスローされます。
        /// </para>
        /// </summary>
        /// <param name="content">HTML コンテンツ</param>
        /// <param name="element">値を取り出す属性がある要素の名前</param>
        /// <param name="name">値を取り出す属性の名前</param>
        /// <returns>指定した要素と属性の名前にマッチする値を文字列配列として返します。</returns>
        public static string[] GetAttributes(string content, string element, string name)
        {
            throw new NotSupportedException();
        }
        */


        // [Parameters]
        //
        // int html_depth:
        //   HTML コンテンツ自体のタグの階層の深さ。
        //
        // int frames:
        //   StackFrame のコンストラクターに指定する skipFrames。
        //   このメソッドを呼び出すときに 2 を指定すれば、呼び出し元のメソッド名がデバッグ メッセージに表示される。
        //
        private static List<string> get_element_values(HtmlContent html_content, string name, string[,] attributes, bool strict, int html_depth, int frames)
        {
            List<string> values = new List<string>();
            string value = html_content.GetFirstElementValue();
            string element_name = html_content.GetFirstElementName();
            bool recurse = false;

#if DEBUG
            if (SimpleHtmlParser.DebugPrintLevel > 0)
            {
                Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Target Element=\"{3}\"", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name, name);
            }
#endif

            // Check 1st Element of HTML Content
            // 最初の要素のチェック
            if (element_name.ToLower() == name.ToLower())
            {
                // 最初の要素の名前が、検索対象だった。

#if DEBUG
                if (SimpleHtmlParser.DebugPrintLevel > 0)
                {
                    Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Target Element \"{3}\" is found!", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name, name);
                }
#endif

                // Check Attribute(s)
                if (attributes != null)
                {
#if DEBUG
                    if (SimpleHtmlParser.DebugPrintLevel > 0)
                    {
                        Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Target Element=\"{3}\", Check Attribute(s)", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name, name);
                    }
#endif

                    // 属性が指定されている場合は、指定された全ての属性をチェックする。
                    for (int i = 0; i < attributes.Length / 2; i++)
                    {
#if DEBUG
                        if (SimpleHtmlParser.DebugPrintLevel > 0)
                        {
                            Debug.Write(string.Format("[{0}] HTML Level={1}, Element=\"{2}\", Check Attribute \"{3}\"=\"{4}\"", DebugInfo.GetCallerName(skipFrames: frames), html_depth, name, attributes[i, 0], attributes[i, 1]));
                            // w/o Line Break
                        }
#endif

                        string attribute_value = html_content.GetFirstElementAttributeValue(attributes[i, 0]);
                        if ((attribute_value != null) && (attribute_value.ToLower() == attributes[i, 1].ToLower()))
                        {
#if DEBUG
                            // add Debug Message
                            if (SimpleHtmlParser.DebugPrintLevel > 0) { Debug.WriteLine(" -> PASS!"); }
#endif
                        }
                        else
                        {
                            // 複数の属性が指定されている場合、ひとつでも条件に一致しない属性があれば、その時点で終了。

                            // Set FLAG
                            recurse = true;
#if DEBUG
                            // add Debug Message
                            if (SimpleHtmlParser.DebugPrintLevel > 0) { Debug.WriteLine(" -> FAIL..."); }
#endif
                            break;
                        }
                    }


                    // All attribute check has been passed!
                    if (!recurse)
                    {
#if DEBUG
                        if (SimpleHtmlParser.DebugPrintLevel > 0)
                        {
                            Debug.WriteLine("[{0}] HTML Level={1}, Element=\"{2}\", Attribute Check is Passed!", DebugInfo.GetCallerName(skipFrames: frames), html_depth, name);
                        }
#endif
                    }
                }
                // Pass Attribute Check


                // Check FLAG
                if (!recurse)
                {
                    // Add value of this element
                    values.Add(value);
#if DEBUG
                    if (SimpleHtmlParser.DebugPrintLevel > 0)
                    {
                        Debug.WriteLine("[{0}] HTML Level={1}, Element=\"{2}\", Value \"{3}\" is found!", DebugInfo.GetCallerName(skipFrames: frames), html_depth, name, value);
                    }
#endif
                }
            }
            else
            {
                // 最初の要素の名前は、検索対象ではなかった。
                recurse = true;
            }
            // Check of 1st Element is finish.


            // 以下のいずれかの場合は、最初の要素の内側の値に対して、再検索
            //
            //  1. 最初の要素の名前が、検索対象ではなかった。
            //  2. 最初の要素の名前は検索対象だったが、少なくとも一つ以上の属性が一致していなかった。
            //
            if ((recurse && !string.IsNullOrWhiteSpace(value)) && (value.Contains('<')))
            {
                try
                {
#if DEBUG
                    if (SimpleHtmlParser.DebugPrintLevel > 0)
                    {
                        Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Check Internal...", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
                    }
#endif

                    // RECURSIVE CALL (for Internal)
                    values = SimpleHtmlParser.get_element_values(new HtmlContent(value, strict, false), name, attributes, strict, html_depth + 1, frames + 1);

#if DEBUG
                    if (SimpleHtmlParser.DebugPrintLevel > 0)
                    {
                        Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Return from Internal", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
                    }
#endif
                }
                catch (Exception) { throw; }
            }


#if DEBUG
            if (SimpleHtmlParser.DebugPrintLevel > 0)
            {
                Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Check NEXT", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
            }
#endif

            // Check NEXT
            HtmlContent next = html_content.GetNextContents();
            if (next == null)
            {
#if DEBUG
                if (SimpleHtmlParser.DebugPrintLevel > 0)
                {
                    Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", NEXT is null.", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
                }
#endif
            }
            else
            {
                try
                {
                    // RECURSIVE CALL (for NEXT)
                    List<string> next_values = SimpleHtmlParser.get_element_values(next, name, attributes, strict, html_depth, frames + 1);

#if DEBUG
                    if (SimpleHtmlParser.DebugPrintLevel > 0)
                    {
                        Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", RETURN from NEXT", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
                    }
#endif

                    if (next_values != null)
                    {
                        // Add value of next element(s)
                        values.AddRange(next_values);
                    }
                }
                catch (Exception) { throw; }
            }


#if DEBUG
            if (SimpleHtmlParser.DebugPrintLevel > 0)
            {
                Debug.WriteLine("[{0}] HTML Level={1}, Current Element=\"{2}\", Exit", DebugInfo.GetCallerName(skipFrames: frames), html_depth, element_name);
            }
#endif

            // RETURN
            if (values != null)
            {
                return values;
            }
            else
            {
                return null;
            }
        }
    }
}
