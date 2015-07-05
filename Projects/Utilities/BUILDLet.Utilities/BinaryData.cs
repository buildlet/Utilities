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


namespace BUILDLet.Utilities
{
    /// <summary>
    /// ランダムな値を格納したバイト配列のデータを実装します。
    /// </summary>
    public class BinaryData
    {
        private byte[] data;


        /// <summary>
        /// ランダムな値を格納したバイト配列の 1組のパターンの長さを取得します。
        /// </summary>
        /// <remarks>
        /// この値よりも大きい値がデータ全体のサイズに指定された場合は、この長さのパターンが繰り返し格納されます。
        /// </remarks>
        public int PatternLength { get; protected set; }


        /// <summary>
        /// <see cref="BinaryData"/> クラスの新しいインスタンスを初期化します。 
        /// ランダムな値を格納したバイト配列のパターンを指定されたサイズになるまで繰り返したデータを作成します。
        /// </summary>
        /// <param name="size">作成するバイト配列データの全体のサイズ。既定のサイズは 1024 バイトです。</param>
        /// <param name="pattern">バイト配列 1組のパターンの長さ。既定の長さは 16 です。</param>
        /// <exception cref="ArgumentOutOfRangeException">パラメーター size と pattern のいずれか一方、あるいは両方が 0 以下です。</exception>
        /// <exception cref="OutOfMemoryException">指定されたバイト配列データのサイズが大きすぎます。</exception>
        public BinaryData(int size = 1024, int pattern = 16)
        {
            // Validation
            if (size <= 0 || pattern <= 0) { throw new ArgumentOutOfRangeException(); }

            try
            {
                this.data = new byte[size];  // may cause OutOfMemoryException
                this.PatternLength = pattern;

                byte[] block = new byte[size < pattern ? size : pattern];
                new Random().NextBytes(block);

                int count;
                int mod;

                if (size < pattern)
                {
                    count = 1;
                    mod = 0;
                }
                else
                {
                    count = (int)(size / pattern);
                    mod = size % pattern;
                }

                for (int i = 0; i < count; i++) { block.CopyTo(this.data, i * pattern); }
                if (mod != 0)
                {
                    for (int i = 0; i < mod; i++) { this.data[pattern * count + i] = block[i]; }
                }
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// 作成したバイト配列データをファイルに書き出します。
        /// </summary>
        /// <param name="filename">保存するファイルのパス</param>
        /// <param name="size">
        /// 作成するバイト配列データのサイズを指定します。
        /// 省略した場合、または null が指定された場合は、作成したバイト配列データ全体がファイルに書き出されます。
        /// </param>
        /// <param name="mode">
        /// ファイルを開く際の <see cref="System.IO.FileMode"/> を指定します。
        /// 既定は <see cref="System.IO.FileMode.Create"/> です。
        /// </param>
        public void ToFile(string filename, long? size = null, FileMode mode = FileMode.Create)
        {
            long filesize;
            if (size == null) { filesize = this.data.Length; }
            else { filesize = (long)size; }

            // Validation
            if (filesize <= 0) { throw new ArgumentOutOfRangeException(); }

            try
            {
                using (FileStream fs = new FileStream(filename, mode))
                {
                    long current = 0;
                    int blocksize;

                    while((size - current) > 0)
                    {
                        blocksize = this.data.Length < (filesize - current) ? this.data.Length : (int)(filesize - current);

                        // write to file
                        fs.Write(this.data, 0, blocksize);

                        current += blocksize;
                    }
                }
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// 作成したバイト配列データを取得します。
        /// </summary>
        /// <returns>作成したバイト配列データ</returns>
        public byte[] GetBytes() { return this.data; }
    }
}
