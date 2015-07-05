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

using System.IO;
using System.Diagnostics;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// 簡易的な HTML パーサーを実装します。
    /// </summary>
    public class SimpleHtmlParser
    {
        /// <summary>
        /// <see cref="SimpleHtmlParser"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected SimpleHtmlParser() { }


        /// <summary>
        /// HTML コンテンツから、指定された要素の値を取得します。
        /// </summary>
        /// <param name="content">HTML コンテンツ</param>
        /// <param name="name">取り出す要素の名前</param>
        /// <param name="strict">
        /// &lt;P&gt; 要素のように終了タグのない要素を許可するときに、false を指定します。
        /// ただし、そのとき、取得したい要素の値の中に入れ子になった要素が存在するときは、
        /// 入れ子の要素の開始タグを含めたそれ以降の値は取得されません。
        /// 既定の設定は true です。
        /// </param>
        /// <returns>指定した名前にマッチする要素の値を文字列配列として返します。</returns>
        public static string[] GetElements(string content, string name, bool strict = true)
        {
            // Validataions
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(name)) { throw new ArgumentNullException(); }


            // Remove comments
            int comments = SimpleHtmlParser.RemoveHtmlComment(ref content);
#if DEBUG
            Debug.WriteLine("Number of removed comment is {0}.", comments);
#endif

            // Remove Line Braek and Replace Tab into White Space
            SimpleHtmlParser.removeLineBreak(ref content);



            List<string> founds = new List<string>();
            StringBuilder value = new StringBuilder();

            string tag = name.Trim().ToLower();
            string[] fragments = content.Split('<');

            for (int i = 0; i < fragments.Length; i++)
			{
                // Skip comment
                //SimpleHtmlParser.skipComment(ref fragments, ref i);


                // Tag name of the element is found!
                if ((fragments[i].ToLower().Contains(tag)) && (SimpleHtmlParser.getFirstText(fragments[i]).ToLower() == tag))
                {
                    if (fragments[i].Trim().EndsWith("/>"))
                    {
                        // In case of "<name />" or "<name ... />" (has No value_found)
                        break;
                    }
                    else
                    {
                        // In case of "<name..."

                        // Clear value_found
                        value.Clear();

                        // Check if 1 character exists at least.
                        if (fragments[i].Trim().IndexOf('>') < fragments[i].Trim().Length - 1)
                        {
                            // Append the value_found of start (in case of "<name>..." or "<name ...>...")
                            value.Append(fragments[i].Substring(fragments[i].IndexOf('>') + 1).Trim());
                        }


                        // for "strict" mode
                        if (strict)
                        {
                            // Check if correspoidnig END tag exists or not.
                            bool found = false;
                            for (int j = i + 1; j < fragments.Length; j++)
                            {
                                // Corresponding eEND tag is found! (in case of "</name>")
                                if (SimpleHtmlParser.getFirstText(fragments[j]).ToLower() == ("/" + tag))
                                {
                                    found = true;
                                    break;
                                }

                                // Append the value_found
                                value.Append("<" + fragments[j].Trim());

                                // Forward current position
                                i = j;
                            }
                            if (!found) { throw new InvalidDataException(); }
                        }


                        // Add value_found of 
                        // "<name>..." (not strict) or
                        // "<name>...</name>" (strict)
                        founds.Add(value.ToString());
                    }
                }
			}


             // Remove comment part
             //for (int i = 0; i < founds.Count; i++)
             //{
             //    founds[i] = SimpleHtmlParser.removeCommentedText(founds[i]);
             //}


            // Return
            if (founds.Count > 0) { return founds.ToArray(); }
            else { return null; }
        }

        
        /// <summary>
        /// HTML コンテンツから、指定された要素の属性の値を取得します。
        /// </summary>
        /// <param name="content">HTML コンテンツ</param>
        /// <param name="name">値を取り出す属性がある要素の名前</param>
        /// <param name="attribute">値を取り出す属性の名前</param>
        /// <returns>指定した要素と属性の名前にマッチする値を文字列配列として返します。</returns>
        public static string[] GetAttributes(string content, string name, string attribute)
        {
            // Validataions
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(attribute)) { throw new ArgumentNullException(); }


            // Remove comments
            int comments = SimpleHtmlParser.RemoveHtmlComment(ref content);
#if DEBUG
            Debug.WriteLine("Number of removed comment is {0}.", comments);
#endif

            // Remove Line Braek and Replace Tab into White Space
            SimpleHtmlParser.removeLineBreak(ref content);



            List<string> founds = new List<string>();

            string tag = name.Trim().ToLower();
            string attributeName = attribute.Trim().ToLower();
            string[] fragments = content.Split('<');

            for (int i = 0; i < fragments.Length; i++)
            {
                // Skip comment
                //SimpleHtmlParser.skipComment(ref fragments, ref i);


                // Tag name of the element is found!
                if ((fragments[i].ToLower().Contains(tag)) && (SimpleHtmlParser.getFirstText(fragments[i]).ToLower() == tag))
                {
                    // Remove string after '>' (including '>')
                    fragments[i] = fragments[i].Substring(0, fragments[i].IndexOf('>'));

                    if ((fragments[i].Substring(tag.Length).ToLower().Contains(attributeName + "=")) &&
                        (fragments[i].Length >= (tag + " " + attributeName + "=X").Length))
                    {
                        // Check contents of ('<') "xxx xxx=X..." ('>')
                        foreach (var attr in fragments[i].Substring(tag.Length).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            // Attribute is found!
                            if (attr.Trim().ToLower().StartsWith(attributeName + "="))
                            {
                                if (attr.Length >= (attributeName + "=X").Length)
                                {
                                    // Add value_found of attribute
                                    founds.Add(attr.Substring((attributeName + "=").Length).Trim());
                                }
                            }
                        }
                    }
                }
            }


            // Remove comment part
            //for (int i = 0; i < founds.Count; i++)
            //{
            //    founds[i] = SimpleHtmlParser.removeCommentedText(founds[i]);
            //}


            // Return
            if (founds.Count > 0) { return founds.ToArray(); }
            else { return null; }
        }


        /// <summary>
        /// HTML コンテンツからコメントを削除します。
        /// </summary>
        /// <param name="content">コメントを削除する HTML コンテンツ</param>
        /// <returns>削除したコメントの数を返します。</returns>
        /// <remarks>
        /// コメントの開始タグに対応する終了タグが見つからなかった場合は、それ以降の処理は行われません。
        /// </remarks>
        public static int RemoveHtmlComment(ref string content)
        {
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

            // Default
            return 0;
        }


        // get head portion of text
        private static string getFirstText(string fragment)
        {
            return fragment.Split(new char[] { ' ', '>' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }


        // Remove Line Braek and Replace Tab into White Space
        private static void removeLineBreak(ref string content)
        {
            content = content.Replace('\t', ' ').Replace("\n", string.Empty).Replace("\r", string.Empty);
        }


        // Skip comment
        //private static void skipComment(ref string[] fragments, ref int index)
        //{
        //    if (fragments[index].StartsWith("!--"))
        //    {
        //        for (int i = index; i < fragments.Length; i++, index++)
        //        {
        //            if (fragments[i].Contains("-->") && (fragments[i].IndexOf("-->") < (fragments[i].Length - "-->".Length)))
        //            {
        //                fragments[i] = fragments[i].Substring(fragments[i].IndexOf("-->") + "-->".Length);
        //                break;
        //            }
        //        }
        //    }
        //}


        // Remove comment part
        //private static string removeCommentedText(string text)
        //{
        //    string head = string.Empty;
        //    string tail = string.Empty;

        //    if (text.Contains("<!--") && text.Contains("-->") && (text.IndexOf("<--") < text.IndexOf("-->")))
        //    {
        //        if (text.IndexOf("<!--") > 0) { head = text.Substring(0, text.IndexOf("<!--")); }
        //        if ((text.IndexOf("-->") + "-->".Length) < text.Length) { tail = text.Substring(text.IndexOf("-->") + "-->".Length); }
        //        return SimpleHtmlParser.removeCommentedText(head + tail);
        //    }
        //    else { return text; }
        //}
    }
}
