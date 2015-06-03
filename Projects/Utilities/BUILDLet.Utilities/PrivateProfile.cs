﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// INI ファイル (初期化ファイル) の簡易的な読み込みと書き込みをサポートします。
    /// </summary>
    public class PrivateProfile
    {
        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="path">INI ファイルのパス</param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値を返します。
        /// 該当するセクションとキーの組み合わせが存在しない場合は null を返します。
        /// <para>
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は、そのエントリーが '=' まで含んでいるかどうかにかかわらず、<see cref="System.String.Empty"/> を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static string GetString(string section, string key, string path)
        {
            try
            {
                return PrivateProfile.GetString(section, key, File.ReadLines(path).ToArray());
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) から、指定したセクションとキーの組み合わせに対応する値を取得します。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="contents">INI ファイルの内容</param>
        /// <returns>
        /// 指定したセクションとキーの組み合わせに対応する値を返します。
        /// 該当するセクションとキーの組み合わせが存在しない場合は null を返します。
        /// <para>
        /// 該当するセクションとキーの組み合わせに値が存在しない場合は、そのエントリーが '=' まで含んでいるかどうかにかかわらず、<see cref="System.String.Empty"/> を返します。
        /// </para>
        /// </returns>
        /// <remarks>
        /// 行継続文字 (Line Continuetor) としてのバックスラッシュ (\) はサポートしていません。
        /// </remarks>
        public static string GetString(string section, string key, string[] contents)
        {
            try
            {
                return PrivateProfile.read(section, key, null, ref contents, mode.get);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// INI ファイル (初期化ファイル) の指定したセクションとキーの組み合わせに対応する値を更新または追加します。
        /// </summary>
        /// <param name="section">セクション</param>
        /// <param name="key">キー</param>
        /// <param name="value">値。null が指定されたときは '=' も書き込まれません。</param>
        /// <param name="path">INI ファイルのパス</param>
        /// <remarks>
        /// <para>
        /// 更新または追加する値は ',' を含むことができます。
        /// </para>
        /// <para>
        /// 指定したセクションが存在しない場合は、contents の末尾に、そのセクションとエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションにエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションとキーが存在する場合は、該当する値を更新します。値がない場合は、値を追加します。
        /// 当該エントリーの末尾にコメントがある場合、そのコメントは削除されます。
        /// </para>
        /// </remarks>
        public static void SetString(string section, string key, string value, string path)
        {
            try
            {
                // Read
                string[] contents = File.ReadLines(path).ToArray();

                // Update
                PrivateProfile.SetString(section, key, value, ref contents);

                // Write
                File.WriteAllLines(path, contents);
            }
            catch (Exception e) { throw e; }
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
        /// 指定したセクションが存在しない場合は、contents の末尾に、そのセクションとエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションは存在して、キーが存在しない場合は、そのセクションにエントリーを追加します。
        /// </para>
        /// <para>
        /// 指定したセクションとキーが存在する場合は、該当する値を更新します。値がない場合は、値を追加します。
        /// 当該エントリーの末尾にコメントがある場合、そのコメントは削除されます。
        /// </para>
        /// </remarks>
        public static void SetString(string section, string key, string value, ref string[] contents)
        {
            try
            {
                PrivateProfile.read(section, key, value, ref contents, mode.set);
            }
            catch (Exception e) { throw e; }
        }



        private enum mode
        {
            get,
            set
        }


        private static string read(string section, string key, string value, ref string[] contents, mode mode)
        {
#if DEBUG
            Log log = new Log(stream:LogOutputStream.Trace);
            Log log2 = new Log(false, stream: LogOutputStream.Trace);
#endif

            string section_found;
            string key_found;
            string value_found;

            bool section_already_found = false;

            for (int i = 0; i < contents.Length; i++)
            {
                // Read line
                PrivateProfile.readLine(contents[i], out section_found, out key_found, out value_found);


                // Check Section line or NOT
                if (section_found != null)
                {
                    // In case of Section line


                    // NEXT Section
                    if (section_already_found)
                    {
                        if (mode == mode.get)
                        {
                            // Appropriate entry is not found.

                            // Return null (in case of 'GetString()')
                            return null;
                        }
                        if (mode == mode.set)
                        {
                            // Add new entry to this section

                            List<string> after = contents.ToList();

                            // Add Key (and Value)
                            if (string.IsNullOrEmpty(value)) { after.Insert(i, key); }
                            else { after.Insert(i, key + "=" + value); }
#if DEBUG
                            log.WriteLine("Add New Entry ({0}) to Section[{1}].", after[i], section);
#endif
                            // Updated contents
                            contents = after.ToArray();

                            // Return null (always in case of 'SetString()')
                            return null;
                        }
                    }


                    // check the section name
                    if (section_found.ToUpper() == section.Trim().ToUpper())
                    {
                        // SECTION is found!
                        section_already_found = true;
#if DEBUG
                        log.WriteLine("Section [{0}] is found at line ({1})!", section, i);
#endif
                    }
                }
                else if (section_already_found && (key_found != null))
                {
                    // In case of NOT Section line
                    // (check only when the section has been already found.)

                    if (key_found.ToUpper() == key.Trim().ToUpper())
                    {
                        // the KEY name is found!
#if DEBUG
                        log.WriteLine("Key '{0}' is found at line ({1}) !", key, i);
#endif
                        if (mode == mode.get)
                        {
#if DEBUG
                            log.WriteLine("Section=[{0}], Key=\"{1}\", Value=\"{2}\"", section, key, value_found);
#endif
                            // Return the Value (Entries) (in case of 'GetString()')
                            return value_found;
                        }
                        else if (mode == mode.set)
                        {
                            // Update contents
                            if (string.IsNullOrEmpty(value)) { contents[i] = key; }
                            else { contents[i] = key + "=" + value; }
#if DEBUG
                            log.WriteLine("Update the Entry ({0}) in Section[{1}].", contents[i], section);
#endif

                            // Return null (always in case of 'SetString()')
                            return null;
                        }
                        else { throw new ApplicationException(); }
                    }
                }
            }

#if DEBUG
            log.WriteLine("Entry (Section=[{0}], Key=\"{1}\") is not found.", section, key);
#endif

            // Default
            if (mode == mode.get)
            {
                // Return null (in case of 'GetString()')
                return null;
            }
            else if (mode == mode.set)
            {
                // Add new section (in case of 'SetString()')

                List<string> after = contents.ToList();

                // Add Section
                after.Add("[" + section + "]");

                // Add Key (and Value)
                if (string.IsNullOrEmpty(value)) { after.Add(key); }
                else { after.Add(key + "=" + value); }
#if DEBUG
                log.WriteLine("Append New Section [{0}] and New Entry ({1}).", section, after[after.Count - 1]);
#endif

                // Updated contents
                contents = after.ToArray();

                // Return null (always in case of 'SetString()')
                return null;
            }
            else { throw new ApplicationException(); }
        }



        private static void readLine(string line, out string section, out string key, out string value)
        {
            string target;

            // Set null to (out) section, (out) key and (out) value
            section = null;
            key = null;
            value = null;


            // Remove comment
            if (line.Contains(';')) { target = line.Substring(0, line.IndexOf(';')); }
            else { target = line; }

            // Trim end the target
            target = target.TrimEnd(' ', '\t');


            // When line contains only white space.
            if (string.IsNullOrWhiteSpace(target))
            {
#if DEBUG
                Debug.WriteLine("(Blank Line)");
#endif
                return;
            }


            // Check the Section or NOT
            if (Regex.IsMatch(target, @"\[.+\][\t ]*"))
            {
                // In case of Section

                // Set Section name
                section = target.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

#if DEBUG
                Debug.WriteLine(string.Format("Section=[{0}]", section));
#endif
            }
            else if (!target.StartsWith("["))
            {
                // In case of NOT Section

                // Set Key name
                if (!target.Contains('='))
                {
                    // Set Key name
                    // when NOT containing '=' (in case of "xxx" or "xxx    ")
                    key = target.Trim();
                    value = string.Empty;
                }
                else
                {
                    // Set Key name
                    // in case "xxx=xxx" or "xxx="
                    key = target.Substring(0, target.IndexOf('=')).Trim();

                    if (target.Trim().EndsWith("="))
                    {
                        // Set the return value_found to string array including 1 string.Empty
                        // in case of NO value_found after '=' ("xxx=" or "xxx =   ")
                        value = string.Empty;
                    }
                    else
                    {
                        // Set the return value_found
                        // in case of "xxx=xxx" (or "xxx = xxx")
                        value = target.Substring(target.IndexOf('=') + 1).Trim();
                    }
                }

#if DEBUG
                Debug.WriteLine("Key=\"{0}\", Value=\"{1}\"", key, value);
#endif
            }


            // Default
            return;
        }
    }
}