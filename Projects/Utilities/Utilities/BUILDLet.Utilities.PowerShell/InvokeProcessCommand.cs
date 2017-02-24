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

using System.Management.Automation;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Invoke, "Process", DefaultParameterSetName = "Help", SupportsShouldProcess = true)]
    [OutputType(typeof(int), typeof(string[]), ParameterSetName = new string[] { "FilePath", "StartInfo" })]
    public class InvokeProcessCommand : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return
  @"指定されたされたプロセスを開始します。
    既定では、プロセスの終了コードがパイプラインへ出力されます。
    既定では、プロセスの標準出力ストリームへの出力は、詳細メッセージ ストリームへリダイレクトされます。
    既定では、プロセスの標準エラー ストリームへの出力は、警告メッセージ ストリームへリダイレクトされます。

    プロセスの終了コードは、以下の優先度でリダイレクトされます。
      1. なし                              : PassThru パラメーター, RedirectStandardOutputToWarning パラメーター
      2. 標準出力ストリーム (パイプライン) : なし (Default)

    プロセスの標準出力ストリームへの出力は、以下の優先度でリダイレクトされます。
      1. 警告メッセージ ストリーム         : RedirectStandardOutputToWarning パラメーター
      2. 標準出力ストリーム (パイプライン) : PassThru パラメーター
      3. 詳細メッセージ ストリーム         : なし (Default)

    プロセスの標準エラー ストリームへの出力は、以下の優先度でリダイレクトされます。
      1. 標準出力ストリーム (パイプライン) : RedirectStandardErrorToOutput パラメーター
      2. 詳細メッセージ ストリーム         : RedirectStandardErrorToVerbose パラメーター
      3. 警告メッセージ ストリーム         : なし (Default)

";
            }
        }


        [Parameter(ParameterSetName = "FilePath", Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true,
            HelpMessage = "プロセスで実行されるプログラムファイルのパスを指定します。")]
        public string FilePath { get; set; }


        [Parameter(ParameterSetName = "FilePath", Position = 1, ValueFromPipeline = true,
            HelpMessage = "プロセスを開始するときに使用するパラメーターまたはパラメーター値を指定します。")]
        public string[] ArgumentList { get; set; }


        [Parameter(ParameterSetName = "StartInfo", Mandatory = true, Position = 0, ValueFromPipeline = true,
            HelpMessage = "プロセスを開始する際の System.Diagnostics.ProcessStartInfo を指定します。")]
        public ProcessStartInfo StartInfo { get; set; }


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.RetryCountHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.RetryCountHelpMessage)]
        [PSDefaultValue(Value = InvokeProcessCommand.DefaultRetryCount, Help = InvokeProcessCommand.DefaultRetryCountHelpMessage)]
        public int RetryCount { get; set; }
        protected const int DefaultRetryCount = 0;
        protected const string RetryCountHelpMessage = "プロセスの終了コードが 0 以外だった場合にリトライする回数を指定します。" + InvokeProcessCommand.DefaultRetryCountHelpMessage;
        protected const string DefaultRetryCountHelpMessage = "既定のリトライ回数は 0 回です。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.RetryIntervalelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.RetryIntervalelpMessage)]
        [PSDefaultValue(Value = InvokeProcessCommand.DefaultRetryInterval, Help = InvokeProcessCommand.DefaultRetryIntervalHelpMessage)]
        public int RetryInterval { get; set; }
        protected const int DefaultRetryInterval = 0;
        protected const string RetryIntervalelpMessage = "リトライする間隔を秒数で指定します。" + InvokeProcessCommand.DefaultRetryIntervalHelpMessage;
        protected const string DefaultRetryIntervalHelpMessage = "既定のリトライ間隔は 0 秒です。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.OutputEncodingHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.OutputEncodingHelpMessage)]
        public Encoding OutputEncoding { get; set; }
        protected const string OutputEncodingHelpMessage =
            "標準出力ストリームおよび標準エラー ストリームのエンコードを指定します。\n" +
            "既定のエンコーディングは System.Text.Encoding.Default です。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.RedirectStandardOutputToWarningHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.RedirectStandardOutputToWarningHelpMessage)]
        public SwitchParameter RedirectStandardOutputToWarning { get; set; }
        protected const string RedirectStandardOutputToWarningHelpMessage =
            "プロセスの標準出力ストリームへの出力結果を警告メッセージ ストリームへリダイレクトします。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.RedirectStandardErrorToOutputHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.RedirectStandardErrorToOutputHelpMessage)]
        public SwitchParameter RedirectStandardErrorToOutput { get; set; }
        protected const string RedirectStandardErrorToOutputHelpMessage = 
            "プロセスの標準エラー ストリームへの出力結果をパイプラインへリダイレクトします。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.RedirectStandardErrorToVerboseHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.RedirectStandardErrorToVerboseHelpMessage)]
        public SwitchParameter RedirectStandardErrorToVerbose { get; set; }
        protected const string RedirectStandardErrorToVerboseHelpMessage =
            "プロセスの標準エラー ストリームへの出力結果を詳細メッセージ ストリームへリダイレクトします。\n" +
            "RedirectStandardErrorToOutput パラメーターが指定されている場合は、RedirectStandardErrorToOutput パラメーターが優先されます。";


        [Parameter(ParameterSetName = "FilePath", HelpMessage = InvokeProcessCommand.PassThruHelpMessage)]
        [Parameter(ParameterSetName = "StartInfo", HelpMessage = InvokeProcessCommand.PassThruHelpMessage)]
        public SwitchParameter PassThru { get; set; }
        private const string PassThruHelpMessage = 
            "プロセスの標準出力ストリームへの出力結果を返します。既定ではプロセスの終了コードを返します。";


        private List<string> arguments = new List<string>();


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
            try
            {
                // Call base method
                base.BeginProcessing();


                // Set default value of parameters
                if (this.RetryCount < 0) { this.RetryCount = InvokeProcessCommand.DefaultRetryCount; }
                if (this.RetryInterval < 0) { this.RetryInterval = InvokeProcessCommand.DefaultRetryInterval; }
                if (this.OutputEncoding == null) { this.OutputEncoding = Encoding.Default; }
            }
            catch (Exception e)
            {
                // Write Error
                this.WriteError(e);
            }
        }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            try
            {
                // Call base method
                base.ProcessRecord();


                if (this.ParameterSetName == "FilePath")
                {
                    // Store ArgumentList
                    if (this.ArgumentList != null)
                    {
                        foreach (var argument in this.ArgumentList) { this.arguments.Add(argument); }
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
        protected override void EndProcessing()
        {
            try
            {
                // Call base method
                base.EndProcessing();


                // Set this.StartInfo (ProcessStartInfo)
                if (this.ParameterSetName == "FilePath")
                {
                    if ((this.ArgumentList == null) || (this.ArgumentList.Length <= 0))
                    {
                        this.StartInfo = new ProcessStartInfo(this.GetLocation(this.FilePath));
                    }
                    else
                    {
                        StringBuilder argument = new StringBuilder();
                        for (int i = 0; i < this.arguments.Count; i++)
                        {
                            argument.Append(this.arguments[i]);
                            if (i < this.arguments.Count - 1) { argument.Append(' '); }
                        }

                        this.StartInfo = new ProcessStartInfo(this.GetLocation(this.FilePath), argument.ToString());
                    }
                }


                if (this.ParameterSetName == "FilePath" || this.ParameterSetName == "StartInfo")
                {
                    // for Verbose Message
                    StringBuilder psInfo = new StringBuilder(string.Format("FileNmae='{0}'", StartInfo.FileName));
                    if (!string.IsNullOrEmpty(StartInfo.Arguments)) { psInfo.AppendFormat(", Arguments='{0}'", StartInfo.Arguments); }


                    // for Verbose Message about Standard Output Stream and Standard Error Stream
                    List<string> verbose_message = new List<string>();

                    // Exit Code ('PassThuru' Parameter)
                    if (this.PassThru) { verbose_message.Add("プロセスの終了コードは出力されません。"); }
                    else { verbose_message.Add("プロセスの終了コードがパイプラインへ出力されます。"); }

                    // Standard Output Stream ('RedirectStandardOutputToWarning' Parameter + 'PassThuru' Parameter)
                    if (this.RedirectStandardOutputToWarning)
                    {
                        verbose_message.Add("プロセスの標準出力ストリームへの出力結果は、警告メッセージ ストリームにリダイレクトされます。");
                    }
                    else
                    {
                        if (this.PassThru)
                        {
                            verbose_message.Add("プロセスの標準出力ストリームへの出力結果は、パイプラインにリダイレクトされます。");
                        }
                        else
                        {
                            verbose_message.Add("プロセスの標準出力ストリームへの出力結果は、詳細メッセージ ストリームに出力されます。");
                        }
                    }

                    // Standard Error Stream ('RedirectStandardErrorToOutput' Parameter)
                    if (this.RedirectStandardErrorToOutput)
                    {
                        verbose_message.Add("プロセスの標準エラー ストリームへの出力結果は、標準出力ストリームにリダイレクトされます。");
                    }
                    else if (this.RedirectStandardErrorToVerbose)
                    {
                        verbose_message.Add("プロセスの標準エラー ストリームへの出力結果は、詳細メッセージ ストリームにリダイレクトされます。");
                    }
                    else
                    {
                        verbose_message.Add("プロセスの標準エラー ストリームへの出力結果は、警告メッセージ ストリームに出力されます。");
                    }

                    // Verbose Ouput (about Standard Output Stream and Standard Error Stream)
                    foreach (var line in verbose_message) { this.WriteVerbose(line); }


                    // Should Process
                    if (this.ShouldProcess(string.Format("プロセス ({0})", psInfo), "プロセスの起動"))
                    {
                        // new Process
                        using (Process p = new Process())
                        {
                            // Set Process.StartInfo
                            p.StartInfo = this.StartInfo;
                            p.StartInfo.WorkingDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.StandardOutputEncoding = this.OutputEncoding;
                            p.StartInfo.StandardErrorEncoding = this.OutputEncoding;


                            // new ConcurrentQueue and Register Event for Standard Output Stream
                            ConcurrentQueue<string> stdout = new ConcurrentQueue<string>();
                            p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                            {
                                if (e.Data != null) { stdout.Enqueue(e.Data); }
                            };

                            // new ConcurrentQueue and Register Event for Standard Error Stream
                            ConcurrentQueue<string> stderr = new ConcurrentQueue<string>();
                            p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                            {
                                if (e.Data != null) { stderr.Enqueue(e.Data); }
                            };


                            // Retry loop
                            for (int i = 0; i <= this.RetryCount; i++)
                            {
                                // Process Start
                                p.Start();

                                // Start reading streams
                                p.BeginOutputReadLine();
                                p.BeginErrorReadLine();

                                // Update psInfo
                                psInfo.Insert(0, string.Format("PID={0}, ", p.Id));

                                // Verbose Output
                                this.WriteVerbose(string.Format("({0}) プロセス ({1}) を開始しました。", i, psInfo));


                                // Output loop
                                bool exitNext = false;
                                while (true)
                                {
#if DEBUG
                                    this.WriteDebug(string.Format("p.HasExited={0}, stdout.IsEmpty={1}, stderr.IsEmpty={2}",
                                        p.HasExited, stdout.IsEmpty, stderr.IsEmpty));
#endif

                                    // STANDARD OUTPUT STREAM
                                    //   1. Warning Output             : by 'RedirectStandardOutputToWarning' Parameter
                                    //   2. Standard Output (Pipeline) : by 'PassThru' Parameter
                                    //   3. Verbose Output             : (Default)
                                    while (!stdout.IsEmpty)
                                    {
                                        string line = string.Empty;
                                        if (stdout.TryDequeue(out line))
                                        {
                                            if (this.RedirectStandardOutputToWarning) { this.WriteWarning(line); }
                                            else if (this.PassThru) { this.WriteObject(line); }
                                            else { this.WriteVerbose(line); }
                                        }
                                    }

                                    // STANDARD ERROR STREAM
                                    //   1. Standard Output (Pipeline) : by 'RedirectStandardErrorToOutput' Parameter
                                    //   2. Verbose Output             : by 'RedirectStandardErrorToVerbose' Parameter
                                    //   3. Warning Output             : (Default)
                                    while (!stderr.IsEmpty)
                                    {
                                        string line = string.Empty;
                                        if (stderr.TryDequeue(out line))
                                        {
                                            if (this.RedirectStandardErrorToOutput) { this.WriteObject(line); }
                                            else if (this.RedirectStandardErrorToVerbose) { this.WriteVerbose(line); }
                                            else { this.WriteWarning(line); }
                                        }
                                    }


                                    // Interrupt
                                    Thread.Sleep(0);


                                    // Exit Condition
                                    if (p.HasExited && stdout.IsEmpty && stderr.IsEmpty)
                                    {
                                        // Exit (Break)
                                        if (exitNext) { break; }

                                        // Wait Process for Exit
                                        // (to exhaust the remaining data of Standard Output/Error Stream)
                                        p.WaitForExit();

                                        exitNext = true;
                                    }
                                }


                                // Stop reading streams
                                p.CancelOutputRead();
                                p.CancelErrorRead();


                                // Check ExitCode
                                if (p.ExitCode == 0) { break; }
                                else
                                {
                                    this.WriteWarning(string.Format("({0}) プロセス (PID={1}) は終了コード [0x{2:X}] ({2}) で終了しました。", i, p.Id, p.ExitCode));

                                    if (i < this.RetryCount)
                                    {
                                        if (this.RetryInterval > 0) { this.WriteWarning(string.Format("({0}) {1} 秒後にプロセスを再開します。", i, this.RetryInterval)); }

                                        // Wait
                                        Thread.Sleep(this.RetryInterval * 1000);
                                    }
                                }
                            }


                            // Output (ExitCode)
                            //   1. None                       : 'PassThru' Parameter
                            //   2. Warning Output             : 'RedirectStandardOutputToWarning' Parameter
                            //   3. Standard Output (Pipeline) : (Default)
                            if (!this.PassThru)
                            {
                                this.WriteObject(p.ExitCode);
                            }
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


        // Stop-Processing Tasks
        // protected override void StopProcessing() { }
    }
}
