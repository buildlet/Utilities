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

using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// INI ファイル (初期化ファイル) の簡易的な読み込みと書き込みをサポートします。
    /// </summary>
    public class PrivateProfile
    {
        /// <summary>
        /// <see cref="PrivateProfile"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected PrivateProfile() { }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を文字列として取得します。
        /// <para>
        /// このメソッドの名前は Version 1.4.0.0 で <c>GetString</c> から <c>GetValue</c> に変更されました。
        /// </para>
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="path">INI ファイルのパス</param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値の文字列を返します。
        /// 該当するセクションとキーの組み合わせが存在しない場合は null を返します。
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は、そのエントリーが '=' まで含んでいるかどうかに
        /// かかわらず、<see cref="System.String.Empty"/> を返します。
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static string GetValue(string section, string key, string path)
        {
            try
            {
                return PrivateProfile.GetValue(section, key, File.ReadLines(path).ToArray());
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を文字列として取得します。
        /// <para>
        /// このメソッドの名前は Version 1.4.0.0 で <c>GetString</c> から <c>GetValue</c> に変更されました。
        /// </para>
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="contents">INI ファイルの内容</param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値の文字列を返します。
        /// 該当するセクションとキーの組み合わせが存在しない場合は null を返します。
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は、そのエントリーが '=' まで含んでいるかどうかに
        /// かかわらず、<see cref="System.String.Empty"/> を返します。
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static string GetValue(string section, string key, string[] contents)
        {
            try
            {
                return PrivateProfile.test_value(section, key, null, ref contents);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションに含まれるキーと値の組み合わせを全て取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <param name="name">セクション</param>
        /// <param name="path">INI ファイルのパス</param>
        /// <returns>
        /// 指定したセクションに対応するキーと値の組み合わせを <see cref="Dictionary{TKey, TValue}"/> として全て取得します。
        /// TKey および TValue は <see cref="String"/> 型です。
        /// <para>
        /// 該当するセクションが存在しない場合は null を返します。
        /// 該当するセクションに、キーと値の組み合わせが存在しない場合は、長さ 0 の <see cref="Dictionary{TKey, TValue}"/> を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static Dictionary<string, string> GetSection(string name, string path)
        {
            try
            {
                return PrivateProfile.GetSection(name, File.ReadLines(path).ToArray());
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションに含まれるキーと値の組み合わせを全て取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <param name="name">セクションの名前</param>
        /// <param name="contents">INI ファイルの内容</param>
        /// <returns>
        /// 指定したセクションに対応するキーと値の組み合わせを <see cref="Dictionary{TKey, TValue}"/> として全て取得します。
        /// TKey および TValue は <see cref="String"/> 型です。
        /// <para>
        /// 該当するセクションが存在しない場合は null を返します。
        /// 該当するセクションに、キーと値の組み合わせが存在しない場合は、長さ 0 の <see cref="Dictionary{TKey, TValue}"/> を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static Dictionary<string, string> GetSection(string name, string[] contents)
        {
            try
            {
                return PrivateProfile.get_section(name, ref contents);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の内容を全て取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <param name="path">INI ファイルのパス</param>
        /// <returns>
        /// 全てのセクションに含まれるキーと値の組み合わせを <see cref="Dictionary{TSection, TDictionary}"/> として全て取得します。
        /// TDictionary は <see cref="Dictionary{TKey, TValue}"/> 型です。
        /// TSection, TKey および TValue は <see cref="String"/> 型です。
        /// <para>
        /// キーと値の組み合わせが存在しないセクションは含まれません。
        /// 有効なセクションが含まれていない場合は null を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static Dictionary<string, Dictionary<string, string>> Read(string path)
        {
            try
            {
                return PrivateProfile.Read(File.ReadLines(path).ToArray());
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の内容を全て取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <param name="contents">INI ファイルの内容</param>
        /// <returns>
        /// 全てのセクションに含まれるキーと値の組み合わせを <see cref="Dictionary{TSection, TDictionary}"/> として全て取得します。
        /// TDictionary は <see cref="Dictionary{TKey, TValue}"/> 型です。
        /// TSection, TKey および TValue は <see cref="String"/> 型です。
        /// <para>
        /// キーと値の組み合わせが存在しないセクションは含まれません。
        /// 有効なセクションが含まれていない場合は null を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static Dictionary<string, Dictionary<string, string>> Read(string[] contents)
        {
            try
            {
                return PrivateProfile.read(ref contents);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
        /// <para>
        /// このメソッドの名前は Version 1.4.0.0 で <c>SetString</c> から <c>SetValue</c> に変更されました。
        /// </para>
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="value">
        /// 値。
        /// <see cref="String.Empty"/> を指定することもできます。この場合は '=' まで書き込まれます。
        /// null が指定されたときは '=' も書き込まれません。
        /// </param>
        /// <param name="path">INI ファイルのパス</param>
        /// <remarks>
        /// <para>
        /// 更新または追加する値は ',' を含むことができます。
        /// </para>
        /// <para>
        /// 指定したセクションが存在しない場合は、INI ファイルの末尾に、そのセクションとエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションにエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションとキーが存在する場合は、該当する値を更新します。値がない場合は、値を追加します。
        /// 当該エントリーの末尾にコメントがある場合、そのコメントは削除されます。
        /// </para>
        /// </remarks>
        public static void SetValue(string section, string key, string value, string path)
        {
            try
            {
                // Read content from ini file
                string[] contents = File.ReadLines(path).ToArray();

                // Update content
                PrivateProfile.SetValue(section, key, value, ref contents);

                // Write content to ini file
                File.WriteAllLines(path, contents);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="value">値。null が指定されたときは '=' も書き込まれません。</param>
        /// <param name="contents">INI ファイルの内容</param>
        /// <remarks>
        /// <para>
        /// 更新または追加する値は ',' を含むことができます。
        /// </para>
        /// <para>
        /// 指定したセクションが存在しない場合は、<paramref name="contents"/> の末尾に、そのセクションとエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションにエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションとキーが存在する場合は、該当する値を更新します。値がない場合は、値を追加します。
        /// 当該エントリーの末尾にコメントがある場合、そのコメントは削除されます。
        /// </para>
        /// </remarks>
        public static void SetValue(string section, string key, string value, ref string[] contents)
        {
            try
            {
                PrivateProfile.test_value(section, key, value, ref contents, true);
            }
            catch (Exception) { throw; }
        }



        // Get All Key Value Pair
        private static Dictionary<string, Dictionary<string, string>> read(ref string[] contents)
        {
            string section_found;
            string key_found;
            string value_found;

            string section_current = null;

            Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> section = null;


#if DEBUG
            DebugInfo.Init();
            Debug.WriteLine("");
            Debug.WriteLine(string.Format("[{0}] Read", DebugInfo.ClassName));
#endif

            for (int i = 0; i < contents.Length; i++)
            {
                // Read line
                PrivateProfile.read_line(i, contents[i], out section_found, out key_found, out value_found);


                // Check SECTION LINE or NOT
                if (section_found != null)
                {
                    // SECTION LINE


                    // NOT 1st SECTION
                    if (section_current != null)
                    {
                        // Append SECTION
                        sections.Add(section_current, section);
                    }

                    // Renew section
                    section = new Dictionary<string, string>();

                    // Set SECTION Name
                    section_current = section_found;
                }
                else
                {
                    // NOT SECTION LINE


                    if (key_found != null)
                    {
                        // Append KEY VALUE Pair
                        section.Add(key_found, value_found);
                    }
                }
            }


            // Append Last SECTION
            if (section.Count != 0 && section_current != null) { sections.Add(section_current, section); }


            // RETURN
            if (sections.Count == 0)
            {
#if DEBUG
                Debug.WriteLine(string.Format(string.Format("[{0}] There is no valid entry.", DebugInfo.ClassName)));
#endif
                return null;
            }
            else
            {
#if DEBUG
                Debug.WriteLine(string.Format("[{0}] There are the following entries.", DebugInfo.ClassName));

                int j = 0;
                foreach (var section_name in sections.Keys)
                {
                    int k = 0;
                    foreach (var key in sections[section_name].Keys)
                    {
                        Debug.WriteLine("[{0}] Section[{1}]=\"{3}\", Key[{2}]=\"{4}\", Value=\"{5}\"", DebugInfo.ClassName, j, k++, section_name, key, sections[section_name][key]);
                    }
                    j++;
                }
                Debug.WriteLine("");
#endif
                return sections;
            }
        }



        // Get Target SECTION
        private static Dictionary<string, string> get_section(string section_name, ref string[] contents)
        {
            string section_found;
            string key_found;
            string value_found;

            bool target_section_already_found = false;

            Dictionary<string, string> pairs = new Dictionary<string, string>();


#if DEBUG
            DebugInfo.Init();
            Debug.WriteLine("");
            Debug.WriteLine("[{0}] Get Section \"{1}\"", DebugInfo.ClassName, section_name);
#endif

            for (int i = 0; i < contents.Length; i++)
            {
                // Read line
                PrivateProfile.read_line(i, contents[i], out section_found, out key_found, out value_found);


                // Check SECTION LINE or NOT
                if (section_found != null)
                {
                    // SECTION LINE


                    if (!target_section_already_found)
                    {
                        // 対象セクションがまだ見つかっていない場合:


                        // Check SECTION Name
                        if (section_found.ToUpper() == section_name.Trim().ToUpper())
                        {
                            // Enter Target SECTION!
                            target_section_already_found = true;
#if DEBUG
                            Debug.WriteLine("[{0}] Line[{1}] Target Section \"{2}\" is found!", DebugInfo.ClassName, i, section_name);
#endif
                            continue;
                        }
                        else
                        {
                            // Enter SECTION, but NOT Target SECTION
#if DEBUG
                            Debug.WriteLine("[{0}] Line[{1}] Section \"{2}\" is found.", DebugInfo.ClassName, i, section_name);
#endif
                            continue;
                        }
                    }
                    else
                    {
                        // 対象セクションが既に見つかっている場合:


                        // Enter Next SECTION of Target SECTION
#if DEBUG
                        Debug.WriteLine("[{0}] Line[{1}] Exit Target Section \"{2}\"...", DebugInfo.ClassName, i, section_name);
#endif
                        break;
                    }
                }
                else
                {
                    // Not SECTION LINE


                    if (target_section_already_found)
                    {
                        // Current SECTION is Target SECTION!
                        if (key_found != null)
                        {
                            // Append KEY VALUE Pair
                            pairs.Add(key_found, value_found);
#if DEBUG
                            Debug.WriteLine("[{0}] Line[{1}] Key Value Pair (Key=\"{3}\", Value=\"{4}\") is found in Section \"{2}\"!", DebugInfo.ClassName, i, section_name, key_found, value_found);
#endif
                        }
                    }
                    else
                    {
                        // Current SECTION is NOT Target SECTION!
                        // nothing to do
                    }
                }
            }


            // RETURN
            if (!target_section_already_found)
            {
                // Target SECTION is not found.
#if DEBUG
                Debug.WriteLine("[{0}] Target Section \"{1}\" could not be found...", DebugInfo.ClassName, section_name);
                Debug.WriteLine("");
#endif
                return null;
            }
            else
            {
                if (pairs.Count == 0)
                {
                    // Target SECTION has no entries.
#if DEBUG
                    Debug.WriteLine("[{0}] Target Section \"{1}\" does not have any Key Value Pair.", DebugInfo.ClassName, section_name);
                    Debug.WriteLine("");
#endif
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("[{0}] Target Section \"{1}\" has the following Key Value Pair.", DebugInfo.ClassName, section_name);

                    int i = 0;
                    foreach (var key in pairs.Keys)
                    {
                        Debug.WriteLine("[{0}] Section=\"{2}\", Key[{1}]=\"{3}\", Value=\"{4}\"", DebugInfo.ClassName, i++, section_name, key, pairs[key]);
                    }
                    Debug.WriteLine("");
#endif
                }
                return pairs;
            }
        }


        // Get or Set Target VALUE
        //
        // [Arguments]
        //   ref string[] contents: content of ini file (contents[i] = 1 line of content)
        //   GET mode: this argument is not updated.
        //   SET mode: this argument represents updated content.
        // 
        // [Return Value]
        //   GET mode: when the target kay value pair is found, VALUE (or string.Empty) is returned.
        //             otherwise, null is returned.
        //   SET mode: null is always returned.
        // 
        // Change method name "read" -> "scan" (Version 1.3.0.0)
        // Change method name "scan" -> "test_value" (Version 1.4.0.0)
        private static string test_value(string section, string key, string value, ref string[] contents, bool set_mode = false)
        {
            string section_found;
            string key_found;
            string value_found;

            bool target_section_already_found = false;


#if DEBUG
            DebugInfo.Init();
            Debug.WriteLine("");
            Debug.WriteLine("[{0}] Test Target Key \"{2}\" in Section \"{1}\".", DebugInfo.ClassName, section, key);
#endif

            // for each line of contents
            for (int i = 0; i < contents.Length; i++)
            {
                // Read line
                PrivateProfile.read_line(i, contents[i], out section_found, out key_found, out value_found);


                // Check SECTION LINE or NOT
                if (section_found != null)
                {
                    // SECTION LINE
                    // 現在のラインで、任意のセクションが検出された場合:


                    if (target_section_already_found)
                    {
                        // 既に、検索対象のセクション内にいる状態で、次のセクションが検出された場合:


                        // Check READ or SET
                        if (!set_mode)
                        {
                            // 読み込みモードの場合:
                            // 検索対象のセクションが終了したので、検出できずに終了。


                            // Appropriate entry is not found.
#if DEBUG
                            Debug.WriteLine("[{0}] Target Key \"{2}\" could not be found in Section \"{1}\"...", DebugInfo.ClassName, section, key);
                            Debug.WriteLine("");
#endif
                            return null;
                        }
                        else
                        {
                            // 書き込みモードの場合:
                            // 現在のセクションの末尾に、新しいエントリーを追加する。


                            List<string> updated_contents = contents.ToList();

                            // Add KEY (and VALUE)
                            if (value == null) { updated_contents.Insert(i, key); }
                            else { updated_contents.Insert(i, key + "=" + value); }

                            // Updated contents
                            contents = updated_contents.ToArray();

#if DEBUG
                            Debug.WriteLine("[{0}] Line[{1}] :\"{2}\" INSERT (Section=\"{3}\")", DebugInfo.ClassName, i, updated_contents[i], section);
                            Debug.WriteLine("");
#endif
                            return null;
                        }
                    }


                    // check SECTION Name
                    if (section_found.ToUpper() == section.Trim().ToUpper())
                    {
                        // Target SECTION is found!
                        target_section_already_found = true;
#if DEBUG
                        Debug.WriteLine("[{0}] Line[{1}] Target Section \"{2}\" is found!", DebugInfo.ClassName, i, section);
#endif
                    }
                }
                else
                {
                    // NOT SECTION LINE
                    // 現在のラインが、セクション名ではない場合:


                    if (target_section_already_found)
                    {
                        // 検索対象のセクション内


                        if (key_found != null)
                        {
                            // 任意のキーが検出された場合:


                            // check KEY
                            if (key_found.ToUpper() == key.Trim().ToUpper())
                            {
                                // Target KEY is found!
#if DEBUG
                                Debug.Write(string.Format("[{0}] Line[{1}] Target Key \"{3}\" in Section \"{2}\" is found!", DebugInfo.ClassName, i, section, key_found));
                                // debug message is NOT terminated.
#endif

                                // Check READ or SET
                                if (!set_mode)
                                {
                                    // 読み込みモードの場合:


                                    // VALUE is found!
#if DEBUG
                                    // Add debug message
                                    Debug.WriteLine(string.Format(" (Value=\"{0}\")", value_found));
                                    Debug.WriteLine("");
#endif
                                    return value_found;
                                }
                                else
                                {
                                    // 書き込みモードの場合:


                                    // Update contents
                                    if (value == null) { contents[i] = key; }
                                    else { contents[i] = key + "=" + value; }

#if DEBUG
                                    // Add debug message
                                    Debug.WriteLine("");
                                    Debug.WriteLine("[{0}] Line[{1}] :\"{2}\" UPDATE (Section=\"{3}\")", DebugInfo.ClassName, i, contents[i], section);
                                    Debug.WriteLine("");
#endif
                                    return null;
                                }
                            }
                            else
                            {
                                // 検出されたキーは、検索対象ではなかった。
                            }
                        }
                        else
                        {
                            // キーは検出されなかった。 (= Blank or Comment Line)
                        }
                    }
                    else
                    {
                        // 検索対象ではないセクション内
                    }
                }
            }


            // Default
            // Check READ or SET
            if (!set_mode)
            {
                // 読み込みモードの場合:
                // 何も見つからなかった。


                // Appropriate entry is not found.
#if DEBUG
                Debug.WriteLine("[{0}] Target Key \"{2}\" in Section \"{1}\" could not be found...", DebugInfo.ClassName, section, key);
                Debug.WriteLine("");
#endif
                return null;
            }
            else
            {
                // 書き込みモードの場合:
                // コンテンツの末尾に、新しいセクションを追加する。


                List<string> updated_contents = contents.ToList();


                // Append NEW SECTION
                // (検索対象のセクションが末尾ではない場合にのみセクションを追加する。)
                if (!target_section_already_found)
                {
                    updated_contents.Add("[" + section + "]");
                }

                // Add KEY (and VALUE)
                if (value == null) { updated_contents.Add(key); }
                else { updated_contents.Add(key + "=" + value); }

                // Updated contents
                contents = updated_contents.ToArray();
#if DEBUG
                Debug.WriteLine("[{0}] Line[{1}] :\"{2}\" APPEND", DebugInfo.ClassName, contents.Length - 2, contents[contents.Length - 2]);
                Debug.WriteLine("[{0}] Line[{1}] :\"{2}\" APPEND", DebugInfo.ClassName, contents.Length - 1, contents[contents.Length - 1]);
                Debug.WriteLine("");
#endif
                return null;
            }
        }



        // Read 1 line of the content of ini file.
        // Change method name "readLine" -> "read_line" (Version 1.4.0.0)
        private static void read_line(int line_number, string content, out string section, out string key, out string value)
        {
            // Set null to (out) section, (out) key and (out) value
            section = null;
            key = null;
            value = null;


#if DEBUG
            Debug.Write(string.Format("[{0}] Line[{1}] :\"{2}\"", DebugInfo.ClassName, line_number, content));
            // debug message is NOT terminated.
#endif

            // Remove comment and Trim end the target
            string target = ((content.Contains(';')) ? (content.Substring(0, content.IndexOf(';'))) : content).TrimEnd(' ', '\t');


            // When line contains only white space.
            if (string.IsNullOrWhiteSpace(target))
            {
#if DEBUG
                // Append debug message
                Debug.WriteLine("  //Blank or Comment Line");
#endif
                return;
            }


            // Check SECTION or NOT
            if (Regex.IsMatch(target, @"\[.+\][\t ]*"))
            {
                // SECTION is found!
                section = target.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

#if DEBUG
                // Append debug message
                Debug.WriteLine(string.Format("  //Section=\"{0}\"", section));
#endif
            }
            else if (!target.StartsWith("["))
            {
                // SECTION is NOT found.


                // Check KEY and VALUE
                if (!target.Contains('='))
                {
                    // NOT containing '=': only KEY (there is NO VALUE)
                    key = target.Trim();
                    value = string.Empty;
                }
                else
                {
                    // containing '=': KEY and VALUE
                    key = target.Substring(0, target.IndexOf('=')).Trim();

                    if (target.Trim().EndsWith("="))
                    {
                        value = string.Empty;
                    }
                    else
                    {
                        value = target.Substring(target.IndexOf('=') + 1).Trim();
                    }
                }
#if DEBUG
                // Append debug message
                Debug.WriteLine("  //Key=\"{0}\", Value=\"{1}\"", key, value);
#endif
            }

            // Default
            return;
        }
    }
}
