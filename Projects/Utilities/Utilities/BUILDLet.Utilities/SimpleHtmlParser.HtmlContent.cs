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
using BUILDLet.Utilities.Properties;


namespace BUILDLet.Utilities
{
    public partial class SimpleHtmlParser
    {
        /// <summary>
        /// HTML コンテンツを文字列として受け取り、以下の処理を実行して、カプセル化します。
        /// <list type="bullet">
        ///     <item>
        ///         <description>コメントを削除する。</description>
        ///     </item>
        ///     <item>
        ///         <description>タブ文字を半角スペースに置換する。</description>
        ///     </item>
        ///     <item>
        ///         <description>改行を削除する。</description>
        ///     </item>
        /// </list>
        /// <para>このクラスは Version 1.4.0.0 で追加されました。</para>
        /// </summary>
        protected class HtmlContent
        {
            // HTML content
            private string content;

            // Indices
            private int endIndex_of_StartTag;
            private int startIndex_of_EndTag;
            private int endIndex_of_EndTag;

            // Content of Start Tag
            private string content_in_StartTag;

            // 1st Element Name
            private string element_name;


            // 1st Element Value (設定要注意)
            private string element_value;

            // 2 番目の要素以降の HTML コンテンツ全体 (設定要注意)
            private string content_remained;

#if DEBUG
            // Debug Print Mode
            private bool debug_print;
#endif


            // Mode
            private bool isStrictMode { get; set; }

            // 最初の要素が空要素かどうか
            private bool isEmpty
            {
                get
                {
                    // "<!" で始まる場合も、空要素として扱うことにする。
                    bool start = this.content.StartsWith("<!");
                    bool end = (this.content.Substring(this.endIndex_of_StartTag - 1, 2) == "/>");

                    // Validation
                    if (start && end)
                    {
                        throw new InvalidDataException(string.Format("{0} {1}",
                            Resources.InvalidHtmlContentErrorMessage,
                            Resources.InvalidHtmlContentErrorMessage_InvalidEmptyElement));
                    }

                    return (start || end);
                }
            }

            // 現在の HTML コンテンツの最初の要素の後に何らかの HTML コンテンツがあるかどうか
            // (現在の HTML コンテンツの最初の要素 = 最後の要素)
            private bool isLast
            {
                get
                {
                    if (this.isEmpty)
                    {
                        // 空要素

                        //       3 = endIndex_of_StartTag
                        //       |
                        // ->|01234|<- 5
                        //    <P/>X
                        // ->|0123|<- 4

                        return !(this.content.Length > this.endIndex_of_StartTag + 1);
                    }
                    else
                    {
                        // 空要素ではない

                        if (this.endIndex_of_EndTag < 0)
                        {
                            // 空要素でもなく、終了タグがない場合は、
                            // 最初の要素の開始タグの後ろに、次の何らかのタグが存在するかどうか
                            return !(this.content.IndexOf('<', this.endIndex_of_StartTag) > 0);
                        }
                        else
                        {
                            //           7 = endIndex_of_EndTag
                            //           |
                            // ->|012345678|<- 9
                            //    <P>A</P>X
                            // ->|01234567|<- 8

                            return !(this.content.Length > this.endIndex_of_EndTag + 1);
                        }
                    }
                }
            }


            /// <summary>
            /// <see cref="SimpleHtmlParser.HtmlContent"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="content">HTML コンテンツ</param>
            /// <param name="strict">
            /// STRICT モードを設定するときに true を指定します。
            /// 既定では true です。
            /// </param>
            /// <param name="initialize">
            /// 初期化する場合に true を指定します。
            /// 既定では true です。
            /// </param>
            public HtmlContent(string content, bool strict = true, bool initialize = true)
            {
                // Validation
                if (string.IsNullOrWhiteSpace(content)) { throw new ArgumentNullException(); }


                // Set HTML content
                this.content = content;

                // Set Mode
                this.isStrictMode = strict;

#if DEBUG
                // Set Debug Print Mode
                this.debug_print = (SimpleHtmlParser.DebugPrintLevel > 1);
#endif

                // Initialize
                if (initialize)
                {
#if DEBUG
                    if (this.debug_print)
                    {
                        DebugInfo.Init();
                        Debug.WriteLine("");
                        Debug.WriteLine("[{0}] Strict Mode={1}", DebugInfo.ClassName, this.isStrictMode);
                    }
#endif

                    // Remove comments
                    int comments = SimpleHtmlParser.RemoveHtmlComment(ref this.content);

                    // Remove Line Braek and Replace Tab into White Space
                    this.content = this.content.Replace('\t', ' ').Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();

#if DEBUG
                    if (this.debug_print)
                    {
                        Debug.WriteLine("[{0}] {1} comment(s) is removed.", DebugInfo.ClassName, comments);
                    }
#endif

                    // Validation of head of HTML content
                    if (!Regex.IsMatch(this.content, @"^(<[A-Za-z!])"))
                    {
                        throw new InvalidDataException(string.Format("{0} {1}",
                            Resources.InvalidHtmlContentErrorMessage,
                            Resources.InvalidHtmlContentErrorMessage_InvalidHtmlStart));
                    }
                }
                else
                {
                    // Validation of HTML content
                    if (!this.content.Contains('<'))
                    {
                        throw new InvalidDataException(string.Format("{0} {1}",
                            Resources.InvalidHtmlContentErrorMessage,
                            Resources.InvalidHtmlContentErrorMessage_NotIncludingAnyTag));
                    }

                    // 初期化しない場合
                    // 最初のタグの前に文字があれば削除する。
                    if ((this.content.Contains('<')) && (!this.content.StartsWith("<")))
                    {
                        this.content = this.content.Substring(this.content.IndexOf('<'));
                    }
                }


                // Indices of HTML Content
                //
                //  0              startIndex_of_EndTag
                //  |              |
                // <P a="123">xxxxx</abc>yyyyy...zzzzz
                //           |          |
                //           |          endIndex_of_EndTag
                //           |
                //           endIndex_of_StartTag


                // Set endIndex_of_StartTag
                this.endIndex_of_StartTag = this.content.IndexOf('>');

                // Validataion of endIndex_of_StartTag (Shortest Case = "<P>")
                //
                // ->|012|<- 3
                //    <P>
                //
                if (this.endIndex_of_StartTag < 2)
                {
                    throw new InvalidDataException(string.Format("{0} {1}",
                        Resources.InvalidHtmlContentErrorMessage,
                        Resources.InvalidHtmlContentErrorMessage_StartTagEmpty));
                }


                // Content of Start Tag
                if (this.isEmpty)
                {
                    int length;

                    if (this.content.StartsWith("<!"))
                    {
                        // ->|012345|<- 6
                        //   |<!xxx>|
                        //  ->|    |<- 4

                        length = this.endIndex_of_StartTag - 1;
                    }
                    else
                    {
                        // ->|012345|<- 6
                        //   |<xxx/>|
                        //  ->|   |<- 3

                        length = this.endIndex_of_StartTag - 2;
                    }

                    // Set content_in_StartTag
                    this.content_in_StartTag = this.content.Substring(1, length).Trim();
                }
                else
                {
                    // ->|01234|<- 5
                    //   |<xxx>|
                    //  ->|   |<- 3

                    // Set content_in_StartTag
                    this.content_in_StartTag = this.content.Substring(1, this.endIndex_of_StartTag - 1).Trim();
                }


                // Set 1st Element Name
                this.element_name = this.content.Split(new char[] { '<', ' ', '>' }, StringSplitOptions.RemoveEmptyEntries)[0];

#if DEBUG
                if (this.debug_print)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("[{0}] 1st Element: this.isEmpty={1}, this.isLast={2}", DebugInfo.ClassName, this.isEmpty, this.isLast);
                    Debug.WriteLine("[{0}] 1st Element: Start Tag=<{1}>", DebugInfo.ClassName, this.content_in_StartTag);
                    Debug.WriteLine("[{0}] 1st Element: Name=\"{1}\"", DebugInfo.ClassName, this.element_name);
                }
#endif


                // これ以降は HTML コンテンツの中身に応じた下記 4 項目の設定
                // (コンストラクターで、最初に設定しておく。)
                //
                //  1. this.element_value
                //  2. this.content_remained
                //
                //  3. this.startIndex_of_EndTag
                //  4. this.endIndex_of_EndTag
                //
                if (this.isEmpty)
                {
                    // 空要素の場合


                    // Set value of 1st Element
                    this.element_value = string.Empty;

                    // 2 番目の要素以降の HTML コンテンツ全体
                    if (this.isLast)
                    {
                        // 最後の要素 (= 最初の要素) の場合

                        // Set content_remained
                        this.content_remained = string.Empty;
                    }
                    else
                    {
                        // 最初の要素の後に何らかの HTML コンテンツがある (最後の要素ではない) 場合は、
                        // 残りの全ての文字列を設定する。

                        // Set content_remained
                        this.content_remained = this.content.Substring(this.endIndex_of_StartTag + 1).Trim();
                    }
                }
                else
                {
                    // 空要素ではない場合


                    // 自分と同じ名前の要素が内部に含まれているかどうかを確認する。
                    //
                    //     this.endIndex_of_StartTag
                    //      |
                    //      |startIndex
                    //      ||
                    //      ||   endIndex (-> this.startIndex_of_EndTag)
                    //      ||    |
                    //    012345678
                    //    <A>x<A>X</A>x</A>..
                    //      |12345|
                    //      |     |
                    //     ->|     |<- endIndex - startIndex
                    //       |12345|

                    // 開始タグ
                    string[] start_tags = { "<" + this.element_name.ToLower() + ">", "<" + this.element_name.ToLower() + " " };

                    // 終了タグ
                    string end_tag = "</" + this.element_name.ToLower() + ">";

                    int startIndex = this.endIndex_of_StartTag + 1;
                    int endIndex = -1;
                    int tag_open = 1;

                    // タグが開いている間
                    while (tag_open > 0)
                    {
                        // 次の検索文字列の終了位置を検索・更新
                        endIndex = this.content.ToLower().IndexOf(end_tag, startIndex);
                        if (endIndex >= 0)
                        {
                            // endIndex までの間に、自分と同じ名前の要素の終了タグが見つかった。
                            tag_open--;

                            // endIndex までの間にある自分と同じ名前の要素の開始タグを検索
                            while (startIndex < endIndex)
                            {
                                // 自分と同じ名前の要素の開始タグの開始位置を検索


                                int[] indices = new int[start_tags.Length];
                                for (int i = 0; i < start_tags.Length; i++)
                                {
                                    indices[i] = this.content.ToLower().IndexOf(start_tags[i], startIndex, endIndex - startIndex);
                                }
                                int index = indices.Max();


                                // endIndex までに、自分と同じ名前の要素の開始タグを見つけられなかったら break
                                if (index < 0) { break; }


                                // 現在までに開いているタグの数を更新
                                tag_open++;

                                // 見つけた開始タグのインデックスを特定
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    if ((indices[i] > 0) && (indices[i] < index)) { index = indices[i]; }
                                }

                                // 次の検索文字列の開始位置を更新
                                startIndex = index + start_tags[0].Length;

#if DEBUG
                                if (this.debug_print)
                                {
                                    Debug.WriteLine("[{0}] 1st Element: Tag Open ({1})", DebugInfo.ClassName, tag_open);
                                }
#endif
                            }

                            // 次の検索文字列の開始位置を更新
                            startIndex = endIndex + end_tag.Length;

                            if (tag_open > 0)
                            {
#if DEBUG
                                if (this.debug_print)
                                {
                                    Debug.WriteLine("[{0}] 1st Element: Tag Closed ({1})", DebugInfo.ClassName, tag_open);
                                }
#endif
                            }
                        }
                        else
                        {
                            // 終了タグが見つからなった。

                            if (this.isStrictMode)
                            {
                                // STRICT Mode
                                throw new InvalidDataException(string.Format("{0} {1} {2}",
                                    Resources.InvalidHtmlContentErrorMessage,
                                    Resources.InvalidHtmlContentErrorMessage_EndTagNotFound,
                                    Resources.InvalidHtmlContentErrorMessage_TryNoStrict));
                            }
                            else
                            {
                                // NOT STRICT Mode
#if DEBUG
                                if (this.debug_print)
                                {
                                    Debug.WriteLine("[{0}] 1st Element: End Tag of <{1}> element is not found.", DebugInfo.ClassName, this.element_name);
                                }
#endif
                                break;
                            }
                        }
                    }


                    // Set startIndex_of_EndTag
                    this.startIndex_of_EndTag = endIndex;

                    // Set endIndex_of_EndTag
                    this.endIndex_of_EndTag = (this.startIndex_of_EndTag > 0) ? this.content.IndexOf('>', this.startIndex_of_EndTag) : -1;


                    if (this.startIndex_of_EndTag > 0)
                    {
                        // 終了タグが見つかっている。


                        // Set value of 1st Element
                        this.element_value = this.content.Substring(this.endIndex_of_StartTag + 1, this.startIndex_of_EndTag - this.endIndex_of_StartTag - 1).Trim();

                        // 2 番目の要素以降の HTML コンテンツ全体
                        if (this.isLast)
                        {
                            // 最後の要素 (= 最初の要素) の場合

                            // Set content_remained
                            this.content_remained = string.Empty;
                        }
                        else
                        {
                            // 最初の要素の後に何らかの HTML コンテンツがある (最後の要素ではない) 場合は、
                            // 残りの全ての文字列を設定する。

                            // Set content_remained
                            this.content_remained = this.content.Substring(this.endIndex_of_EndTag + 1).Trim();
                        }
                    }
                    else
                    {
                        // 終了タグが見つかっていない。


                        if (this.isStrictMode)
                        {
                            // STRICT Mode
                            throw new InvalidDataException(string.Format("{0} {1} {2}",
                                Resources.InvalidHtmlContentErrorMessage,
                                Resources.InvalidHtmlContentErrorMessage_EndTagNotFound,
                                Resources.InvalidHtmlContentErrorMessage_TryNoStrict));
                        }
                        else
                        {
                            // NOT STRICT Mode

                            if (this.isLast)
                            {
                                // 最後の要素 (= 最初の要素)
                                // (空要素でもない)
                                //
                                // <ABC> or <ABC>XYZ

                                if (this.endIndex_of_EndTag < 0)
                                {
                                    // <ABC>XYZ

                                    // Set value of 1st Element
                                    this.element_value = this.content.Substring(this.endIndex_of_StartTag + 1).Trim();
                                }
                                else
                                {
                                    // <ABC>

                                    // Set value of 1st Element
                                    this.element_value = string.Empty;
                                }

                                // Set content_remained
                                this.content_remained = null;
                            }
                            else
                            {
                                // 最初の要素の後に何らかの HTML コンテンツがある。
                                // (最後の要素ではない。)

                                //        endIndex_of_StartTag
                                //        |
                                //        |         nextStartIndex
                                //        |         |
                                //    <...>xxx...yyy<
                                // ->|               |<- nextStartIndex + 1
                                //      ->|          |<- nextStartIndex - endIndex_of_StartTag
                                //      ->|         |<-  nextStartIndex - endIndex_of_StartTag - 1

                                int next_StartIndex = this.content.IndexOf('<', this.endIndex_of_StartTag);

                                // Validation
                                //
                                // <...>xxx...yyy<A
                                //
                                // 次の要素の開始タグの開始位置の次に、少なくとも1文字以上の何らかの文字があるかどうか
                                if (this.content.Length <= next_StartIndex + 1)
                                {
                                    throw new InvalidDataException(string.Format("{0} {1}",
                                        Resources.InvalidHtmlContentErrorMessage,
                                        Resources.InvalidHtmlContentErrorMessage_InvalidHtmlEnd));
                                }


                                // Set value of 1st Element
                                // 最初の要素の値は、次の要素の開始タグの開始位置 ('<') の直前までを設定する。
                                this.element_value = this.content.Substring(this.endIndex_of_StartTag + 1, next_StartIndex - this.endIndex_of_StartTag - 1).Trim();

                                // Set content_remained
                                // 2 番目の要素以降の HTML コンテンツ全体は、残りの全ての文字列を設定する。
                                this.content_remained = this.content.Substring(next_StartIndex).Trim();
                            }
                        }
                    }
                }
#if DEBUG
                if (this.debug_print)
                {
                    int max_length = 100;
                    if (this.element_value.Length > max_length)
                    {
                        Debug.WriteLine("[{0}] 1st Element: Value=\"{1}\" (over than {2})", DebugInfo.ClassName, this.element_value.Substring(0, max_length) + "...", max_length);
                    }
                    else
                    {
                        Debug.WriteLine("[{0}] 1st Element: Value=\"{1}\"", DebugInfo.ClassName, this.element_value);
                    }

                    Debug.WriteLine("[{0}] 1st Element: Start Tag=(0, {1})", DebugInfo.ClassName, this.endIndex_of_StartTag);
                    Debug.WriteLine("[{0}] 1st Element: End Tag=({1}, {2})", DebugInfo.ClassName, this.startIndex_of_EndTag, this.endIndex_of_EndTag);
                    Debug.WriteLine("[{0}] 1st Element: Next={1}null", DebugInfo.ClassName, ((this.content_remained != null) ? "NOT " : ""));
                    Debug.WriteLine("");
                }
#endif
            }


            /// <summary>
            /// HTML コンテンツの最初の要素の名前を取得します。
            /// </summary>
            /// <returns>
            /// HTML コンテンツの最初の要素の名前
            /// </returns>
            public string GetFirstElementName()
            {
                return this.element_name;
            }


            /// <summary>
            /// HTML コンテンツの最初の要素の属性の値を取得します。
            /// </summary>
            /// <param name="name">
            /// 取得する属性の名前。
            /// 属性の値は、ダブルクォーテーション (またはシングルクォーテーション) で括ってください。
            /// </param>
            /// <returns>
            /// 属性が存在しない場合は null を返します。
            /// </returns>
            public string GetFirstElementAttributeValue(string name)
            {
                // Validation
                if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(); }

                string[] brackets = { "\"", "'" };
                foreach (var bracket in brackets)
                {
                    // @" abc=""
                    string search_text = " " + name + "=" + bracket;

                    if (Regex.IsMatch(this.content_in_StartTag, search_text + ".*" + bracket, RegexOptions.IgnoreCase))
                    {
                        // Target Attribute is found!

                        int index = this.content_in_StartTag.ToLower().IndexOf(search_text) + search_text.Length;
                        int length = this.content_in_StartTag.Substring(index).IndexOf(bracket) ;

                        //        index
                        //         |
                        //  01233456
                        // <P abc="123">
                        // <P abc="123" xyz="">
                        //         0123
                        //      ->|   |<- length

                        return this.content_in_StartTag.Substring(index, length);
                    }
                }

                // Default
                // (Target Attribute is NOT found...)
                return null;
            }


            /// <summary>
            /// HTML コンテンツの最初の要素の値を取得します。
            /// </summary>
            /// <returns>
            /// HTML コンテンツの最初の要素の開始タグと、対応する終了タグの間の HTML コンテンツ全体を返します。
            /// 最初の要素に対応する終了タグが見つからないとき、Strict モードの場合は、<see cref="InvalidDataException"/> 例外が発生します。
            /// Strict モードでない場合は、最初のタグの直前までの HTML 全体を返します。
            /// 空要素の場合は <see cref="String.Empty"/> を返します。
            /// </returns>
            public string GetFirstElementValue()
            {
                return this.element_value;
            }


            /// <summary>
            /// HTML コンテンツの 2 番目の要素以降の HTML コンテンツ全体を取得します。
            /// </summary>
            /// <returns>
            /// HTML コンテンツの最初の要素の終了タグから後ろの HTML コンテンツ全体を取得します。
            /// HTML コンテンツの最初の要素が、HTML コンテンツ全体の最終の要素だった場合は、null を返します。
            /// </returns>
            public SimpleHtmlParser.HtmlContent GetNextContents()
            {
                if ((string.IsNullOrEmpty(this.content_remained)) || (!this.content_remained.Contains('<')))
                {
                    return null;
                }
                else
                {
                    return new SimpleHtmlParser.HtmlContent(this.content_remained, this.isStrictMode, false);
                }
            }
        }
    }
}
