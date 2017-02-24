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
using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities
{
    /// <summary>
    /// ランダムな値を格納したバイト配列のデータを実装します。
    /// </summary>
    public class BinaryData
    {
        private byte[] data = null;


        /// <summary>
        /// 作成するバイト配列データの全体の既定のサイズを表します。
        /// 既定のサイズは 1024 バイトです。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const int DefaultDataSize = 1024;


        /// <summary>
        /// バイト配列 1 組の既定のパターンの長さを表します。
        /// 既定の長さは 16 です。
        /// <para>
        /// このフィールドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        public const int DefaultPatternLength = 16;


        /// <summary>
        /// ランダムな値を格納したバイト配列の 1 組のパターンの長さを取得します。
        /// </summary>
        /// <remarks>
        /// この値よりも大きい値がデータ全体のサイズに指定された場合は、この長さのパターンが繰り返し格納されます。
        /// </remarks>
        public int PatternLength { get; protected set; }


        /// <summary>
        /// <see cref="BinaryData"/> クラスの新しいインスタンスを初期化します。 
        /// ランダムな値を格納したバイト配列のパターンを指定されたサイズになるまで繰り返したデータを作成します。
        /// </summary>
        /// <param name="size">
        /// 作成するバイト配列データの全体のサイズを指定します。
        /// 既定のサイズは <see cref="DefaultDataSize"/> バイトです。
        /// </param>
        /// <param name="pattern">
        /// バイト配列 1 組のパターンの長さを指定します。
        /// 既定の長さは <see cref="DefaultPatternLength"/> です。
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">パラメーター size と pattern のいずれか一方、あるいは両方が 0 以下です。</exception>
        /// <exception cref="OutOfMemoryException">指定されたバイト配列データのサイズが大きすぎます。</exception>
        public BinaryData(int size = BinaryData.DefaultDataSize, int pattern = BinaryData.DefaultPatternLength)
        {
            // Validation
            if (size <= 0 || pattern <= 0) { throw new ArgumentOutOfRangeException(); }

            try
            {
                this.data = new byte[size];  // may cause OutOfMemoryException
                this.PatternLength = pattern;

                byte[] block = new byte[size < pattern ? size : pattern];
                new Random().NextBytes(block);

                int count;  // 繰り返し回数
                int mod;    // 余り

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

                // block を this.data に count 回数に分けてコピー
                for (int i = 0; i < count; i++) { block.CopyTo(this.data, block.Length * i); }
                
                // 余りがあれば、count そのままで、続けてコピー
                if (mod != 0)
                {
                    for (int i = 0; i < mod; i++) { this.data[pattern * count + i] = block[i]; }
                }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// 作成したバイト配列データをファイルに書き出します。
        /// </summary>
        /// <param name="filename">保存するファイルのパス</param>
        /// <param name="size">
        /// 作成するバイト配列データのサイズを指定します。
        /// 省略した場合、0 または負の値が指定された場合、あるいは、データのサイズよりも大きい値が指定された場合は、
        /// 作成したバイト配列データ全体がファイルに書き出されます。
        /// </param>
        /// <param name="mode">
        /// ファイルを開く際の <see cref="System.IO.FileMode"/> を指定します。
        /// 既定では <see cref="System.IO.FileMode.Create"/> です。
        /// </param>
        public void ToFile(string filename, long size = 0, FileMode mode = FileMode.Create)
        {
            // Validation
            if (size <= 0 || size > this.data.Length) { size = this.data.LongLength; }

            try
            {
                using (FileStream fs = new FileStream(filename, mode))
                {
                    fs.Write(this.data, 0, (int)size);
                }
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// 作成したバイト配列データを繰り返しファイルに書き出します。
        /// メモリ上に保持できないような容量の大きいサイズのファイルを作成するときに使用します。
        /// <para>
        /// このメソッドは Version 1.4.0.0 で追加されました。
        /// </para>
        /// </summary>
        /// <param name="count">バイト配列データを書き出す繰り返し回数を指定します。</param>
        /// <param name="filename">保存するファイルのパス</param>
        /// <exception cref="ArgumentOutOfRangeException">繰り返し回数が 1024 回以上です。</exception>
        public void ToFile(int count, string filename)
        {
            // Validation
            if (count <= 0 || count > 1024) { throw new ArgumentOutOfRangeException(); }

            try
            {
                this.ToFile(filename);

#if DEBUG
                // Write Debug Console
                DebugInfo.Init();
                Debug.WriteLine("[{0}] Write {1} byte to \"{2}\".", DebugInfo.ToString(), this.GetBytes().Length, filename);
#endif

                for (int i = 1; i < count; i++)
                {
                    this.ToFile(filename, mode: FileMode.Append);

#if DEBUG
                    // Write Debug Console
                    Debug.WriteLine("[{0}] Append {1} byte to \"{2}\" ({3}) ...", DebugInfo.ToString(), this.GetBytes().Length, filename, i);
#endif
                }
            }
            catch (Exception) { throw; }
        }



        /// <summary>
        /// 作成したバイト配列データを取得します。
        /// </summary>
        /// <returns>作成したバイト配列データ</returns>
        public byte[] GetBytes() { return this.data; }
    }
}
