using System;

namespace BUILDLet.Utilities
{
    /// <summary>
    /// <see cref="BUILDLet.Utilities.Log"/> クラスの出力ストリームを表します。
    /// </summary>
    public enum LogOutputStream
    {
        /// <summary>
        /// 標準出力ストリーム
        /// </summary>
        StandardOutput,

        /// <summary>
        /// 標準エラー出力ストリーム
        /// </summary>
        StandardError,

        /// <summary>
        /// 既定のトレース リスナー
        /// </summary>
        Trace
    }
}
