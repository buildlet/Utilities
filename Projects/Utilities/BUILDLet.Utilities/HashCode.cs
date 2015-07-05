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

using System.Security.Cryptography;
using System.IO;


namespace BUILDLet.Utilities.Cryptography
{
    /// <summary>
    /// 指定されたハッシュ アルゴリズムを使用して、入力データのハッシュ値を計算します。
    /// </summary>
    public class HashCode
    {
        private string name;
        private HashAlgorithm crypto;


        /// <summary>
        /// 計算されたハッシュ コードの値を取得します。
        /// </summary>
        public byte[] Hash { get; protected set; }


        /// <summary>
        /// 使用するハッシュ アルゴリズムの名前を取得します。
        /// <seealso cref="HashAlgorithm"/>
        /// <seealso cref="HashAlgorithm.Create(string)"/>
        /// </summary>
        public string HashName
        {
            protected set
            {
                // create Hash algorithm
                this.crypto = HashAlgorithm.Create(this.name = value);

                // Validation
                if (this.crypto == null) { throw new ArgumentException(); }
            }
            get { return this.name; }
        }
        

        /// <summary>
        /// ハッシュ アルゴリズムと対象となるファイルを指定して、<see cref="HashCode"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="path">ハッシュ コードを計算する対象のファイルのパスを指定します。</param>
        /// <param name="hashName">
        ///     使用するハッシュ アルゴリズムを文字列で指定します。
        ///     既定のハッシュ アルゴリズムは、メッセージ ダイジェスト 5 (MD5) です。
        ///     <seealso cref="HashAlgorithm"/>
        ///     <seealso cref="HashAlgorithm.Create(string)"/>
        /// </param>
        public HashCode(string path, string hashName = "MD5")
        {
            try
            {
                // set Hash caller (and Hash Algorithm)
                this.HashName = hashName;

                // compute Hash
                using (FileStream fs = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read))
                {
                    this.Hash = crypto.ComputeHash(fs);
                }
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// ハッシュ アルゴリズムと対象となるバイト配列を指定して、<see cref="HashCode"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="data">ハッシュ コードを計算する対象の入力データ</param>
        /// <param name="hashName">
        ///     使用するハッシュ アルゴリズムを文字列で指定します。
        ///     既定のハッシュ アルゴリズムは、メッセージ ダイジェスト 5 (MD5) です。
        ///     <seealso cref="HashAlgorithm"/>
        ///     <seealso cref="HashAlgorithm.Create(string)"/>
        /// </param>
        public HashCode(byte[] data, string hashName = "MD5")
        {
            // Validation
            if (data == null) { throw new ArgumentNullException(); }

            try
            {
                // set Hash caller
                this.HashName = hashName;

                // compute Hash
                this.Hash = this.crypto.ComputeHash(data);
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 指定されたハッシュ アルゴリズムで計算したハッシュ値を文字列として取得します。
        /// </summary>
        /// <returns>計算したハッシュ値</returns>
        public override string ToString()
        {
            return BitConverter.ToString(this.Hash).Replace("-", string.Empty);
        }
    }
}
