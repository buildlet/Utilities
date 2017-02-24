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

using System.Security.Cryptography;
using System.IO;


namespace BUILDLet.Utilities.Cryptography
{
    /// <summary>
    /// 指定されたハッシュ アルゴリズムを使用して、入力データのハッシュ値を計算します。
    /// </summary>
    public class HashCode
    {
        // inner value
        private HashAlgorithm crypto;
        private string hashName;
        private byte[] hash;

        /// <summary>
        /// 既定のハッシュ アルゴリズムを表します。
        /// 既定のハッシュ アルゴリズムは、メッセージ ダイジェスト 5 (MD5) です。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const string DefaultHashAlgorithmName = "MD5";


        /// <summary>
        /// ハッシュ アルゴリズムと対象となるファイルを指定して、<see cref="HashCode"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="path">ハッシュ コードを計算する対象のファイルのパスを指定します。</param>
        /// <param name="hashName">
        /// 使用するハッシュ アルゴリズムを文字列で指定します。
        /// 既定のハッシュ アルゴリズムは <see cref="DefaultHashAlgorithmName"/> です。
        /// </param>
        /// <seealso cref="HashAlgorithm"/>
        /// <seealso cref="HashAlgorithm.Create(string)"/>
        public HashCode(string path, string hashName = HashCode.DefaultHashAlgorithmName)
        {
            // Validation
            if (!File.Exists(path)) { throw new FileNotFoundException(); }

            try
            {
                // set Hash caller (and Hash Algorithm)
                this.HashName = hashName;

                using (FileStream fs = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read))
                {
                    // compute Hash
                    this.hash = crypto.ComputeHash(fs);
                }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// ハッシュ アルゴリズムと対象となるバイト配列を指定して、<see cref="HashCode"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="data">ハッシュ コードを計算する対象の入力データ</param>
        /// <param name="hashName">
        /// 使用するハッシュ アルゴリズムを文字列で指定します。
        /// 既定のハッシュ アルゴリズムは <see cref="DefaultHashAlgorithmName"/> です。
        /// </param>
        /// <seealso cref="HashAlgorithm"/>
        /// <seealso cref="HashAlgorithm.Create(string)"/>
        public HashCode(byte[] data, string hashName = HashCode.DefaultHashAlgorithmName)
        {
            // Validation
            if (data == null) { throw new ArgumentNullException(); }

            try
            {
                // set Hash caller
                this.HashName = hashName;

                // compute Hash
                this.hash = this.crypto.ComputeHash(data);
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// 使用するハッシュ アルゴリズムの名前を取得します。
        /// </summary>
        /// <seealso cref="HashAlgorithm"/>
        /// <seealso cref="HashAlgorithm.Create(string)"/>
        public string HashName
        {
            protected set
            {
                // create Hash algorithm
                this.crypto = HashAlgorithm.Create(this.hashName = value);

                // Validation
                if (this.crypto == null) { throw new ArgumentException(); }
            }
            get { return this.hashName; }
        }


        /// <summary>
        /// 計算されたハッシュ コードの値をバイト配列として取得します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <returns>ハッシュ コードの値</returns>
        public byte[] GetBytes() { return this.hash; }


        /// <summary>
        /// 指定されたハッシュ アルゴリズムで計算したハッシュ値を文字列として取得します。
        /// </summary>
        /// <returns>計算したハッシュ値</returns>
        public override string ToString()
        {
            return BitConverter.ToString(this.hash).Replace("-", string.Empty);
        }
    }
}
