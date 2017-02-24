/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2016-2017 Daiki Sakamoto

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

using System.Management.Automation;
using System.IO;

using BUILDLet.Utilities;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "BinaryFile", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(FileInfo))]
    public class NewBinaryFileCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return @"ランダムな値を格納したバイト配列のパターンを指定されたサイズになるまで繰り返した
    データを格納したバイナリファイルを作成します。";
            }
        }


        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "作成するバイナリファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(ParameterSetName = "Path", HelpMessage = "作成するバイナリファイルのサイズを指定します。")]
        public int Size { get; set; }


        [Parameter(ParameterSetName = "Path", HelpMessage = "バイト配列 1 組のパターンの長さを指定します。")]
        public int PatternLength { get; set; }


        [Parameter(ParameterSetName = "Path", HelpMessage = 
            "作成したバイナリファイルのパスを返します。既定ではこのコマンドレットによる出力はありません。")]
        public SwitchParameter PassThru { get; set; }


        // Pre-Processing Tasks
        // protected override void BeginProcessing() { }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if (this.ParameterSetName == "Path")
                {
                    // Resolve path
                    string path = this.GetLocation(this.Path, false);


                    if (this.ShouldProcess(this.Path, "バイナリファイルの作成"))
                    {
                        // Create binary data
                        BinaryData bin;
                        if ((this.Size > 0) && (this.PatternLength > 0))
                        {
                            bin = new BinaryData(this.Size, this.PatternLength);
                        }
                        else if (this.Size > 0)
                        {
                            bin = new BinaryData(this.Size);
                        }
                        else
                        {
                            bin = new BinaryData();
                        }


                        // Save as file
                        int max_size = (int)Math.Pow(1000, 3);
                        if (this.Size > max_size)
                        {
                            bin.ToFile((int)(this.Size / max_size), path);
                        }
                        else
                        {
                            bin.ToFile(path);
                        }


                        // Output (PassThru)
                        if (this.PassThru)
                        {
                            this.WriteObject(new FileInfo(path));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Post-Processing Tasks
        // protected override void EndProcessing() { }


        // Stop-Processing Tasks
        // protected override void StopProcessing() { }
    }
}
