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

using System.Management.Automation;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;


namespace BUILDLet.Utilities.PowerShell
{
    [Cmdlet(VerbsLifecycle.Invoke, "Process")]
    [CmdletBinding(DefaultParameterSetName = "default")]
    [OutputType(typeof(int), typeof(string[]))]
    public class InvokeProcess : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get
            {
                return
  @"指定されたされたプロセスを開始します。
    既定の設定では、プロセスの終了コードがパイプラインへ出力されます。
    プロセスの標準出力ストリームへの出力は、詳細メッセージ ストリームへリダイレクトされます。
";
            }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName="StartInfo",
            HelpMessage = "プロセスを開始する際の System.Diagnostics.ProcessStartInfo を指定します。")]
        public ProcessStartInfo StartInfo { get; set; }


        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "default",
            HelpMessage = "プロセスで実行されるプログラムファイルのパスを指定します。")]
        public string FilePath { get; set; }


        [Parameter(Mandatory = false, Position = 1, ValueFromPipeline = true, ParameterSetName = "default",
            HelpMessage = "プロセスを開始するときに使用するパラメーターまたはパラメーター値を指定します。")]
        public string[] ArgumentList { get; set; }


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_Retry)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_Retry)]
        [PSDefaultValue(Value = 1, Help = "既定の設定は 0 回です。")]
        public int Retry { get; set; }
        private const string helpMessage_Retry = "プロセスの終了コードが 0 以外だった場合にリトライする回数を指定します。既定の設定は 0 回です。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_Interval)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_Interval)]
        [PSDefaultValue(Value = 1, Help = "既定の設定は 0 秒です。")]
        public int Interval { get; set; }
        private const string helpMessage_Interval = "リトライする間隔を秒数で指定します。既定の設定は 0 秒です。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_OutputEncoding)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_OutputEncoding)]
        public Encoding OutputEncoding { get; set; }
        private const string helpMessage_OutputEncoding = "標準出力ストリームおよび標準エラー ストリームのエンコードを指定します。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_RedirectStandardOutputToWarning)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_RedirectStandardOutputToWarning)]
        public SwitchParameter RedirectStandardOutputToWarning { get; set; }
        private const string helpMessage_RedirectStandardOutputToWarning =
            "プロセスの標準出力ストリームへの出力結果を警告メッセージ ストリームへリダイレクトします。" +
            "PassThru パラメーターと同時に指定されている場合は、PassThru パラメーターが優先されます。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_RedirectStandardErrorToOutput)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_RedirectStandardErrorToOutput)]
        public SwitchParameter RedirectStandardErrorToOutput { get; set; }
        private const string helpMessage_RedirectStandardErrorToOutput = 
            "プロセスの標準エラー ストリームへの出力結果をパイプラインへリダイレクトします。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_RedirectStandardErrorToVerbose)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_RedirectStandardErrorToVerbose)]
        public SwitchParameter RedirectStandardErrorToVerbose { get; set; }
        private const string helpMessage_RedirectStandardErrorToVerbose =
            "プロセスの標準エラー ストリームへの出力結果を詳細メッセージ ストリームへリダイレクトします。" +
            "RedirectStandardErrorToOutput パラメーターが指定されている場合は、RedirectStandardErrorToOutput パラメーターが優先されます。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_PassThru)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_PassThru)]
        public SwitchParameter PassThru { get; set; }
        private const string helpMessage_PassThru = 
            "プロセスの標準出力ストリームへの出力結果を返します。既定ではプロセスの終了コードを返します。";


        [Parameter(Mandatory = false, ParameterSetName = "default", HelpMessage = InvokeProcess.helpMessage_WhatIf)]
        [Parameter(Mandatory = false, ParameterSetName = "StartInfo", HelpMessage = InvokeProcess.helpMessage_WhatIf)]
        public SwitchParameter WhatIf { get; set; }
        private const string helpMessage_WhatIf = 
            "コマンドレットを実行するとどのような結果になるかを表示します。コマンドレットは実行されません。";

        
        protected readonly int DefaultRetryTimes = 0;
        protected readonly int DefaultRetryInterval = 0;

        private List<string> arguments = new List<string>();


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
            // Call base method
            base.BeginProcessing();

            // Set default value of parameters
            if (this.Retry < 0) { this.Retry = this.DefaultRetryTimes; }
            if (this.Interval < 0) { this.Interval = this.DefaultRetryInterval; }
            if (this.OutputEncoding == null) { this.OutputEncoding = Encoding.Default; }


            try 
	        {
                switch (this.ParameterSetName)
                {
                    case "default":

                        // Set this.FilePath
                        this.FilePath = this.GetUnresolvedProviderPathFromPSPath(this.FilePath);
                        break;

                    default:
                        break;
                }
	        }
	        catch (Exception e)	{ throw e; }
        }



        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();


            try
            {
                switch (this.ParameterSetName)
                {
                    case "default":

                        // Store ArgumentList
                        if (this.ArgumentList != null)
                        {
                            foreach (var arg in this.ArgumentList) { this.arguments.Add(arg); }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e) { throw e; }
        }



        // Post-Processing Tasks
        protected override void EndProcessing()
        {
            // Call base method
            base.EndProcessing();


            try
            {
                switch (this.ParameterSetName)
                {
                    case "default":

                        // Set this.StartInfo (ProcessStartInfo)
                        if ((this.ArgumentList == null) || (this.ArgumentList.Length <= 0))
                        {
                            this.StartInfo = new ProcessStartInfo(this.FilePath);
                        }
                        else
                        {
                            StringBuilder argument = new StringBuilder();
                            for (int i = 0; i < this.arguments.Count; i++)
                            {
                                argument.Append(this.arguments[i]);
                                if (i < this.arguments.Count - 1) { argument.Append(' '); }
                            }

                            this.StartInfo = new ProcessStartInfo(this.FilePath, argument.ToString());
                        }
                        break;

                    default:
                        break;
                }



                if (this.ParameterSetName == "default" || this.ParameterSetName == "StartInfo")
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



                        // for Verbose message (1)
                        string psInfo = string.Format("FileNmae='{0}'", p.StartInfo.FileName);
                        if (!string.IsNullOrEmpty(p.StartInfo.Arguments)) { psInfo = string.Format("{0}, Arguments='{1}'", psInfo, p.StartInfo.Arguments); }



                        // for Verbose message (2)
                        List<string> verboseMessage = new List<string>();

                        // for Verbose message (2) (about Standard Output Stream)
                        if (this.PassThru)
                        {
                            verboseMessage.Add(
                                "プロセスの標準出力ストリームへの出力結果が、パイプラインに出力されます。" +
                                "プロセスの終了コードは出力されません。");
                        }
                        else
                        {
                            if (this.RedirectStandardOutputToWarning)
                            {
                                verboseMessage.Add(
                                    "プロセスの終了コードがパイプラインへ出力されます。" +
                                    "プロセスの標準出力ストリームへの出力結果が、標準エラー ストリームにリダイレクトされます。");
                            }
                            else
                            {
                                verboseMessage.Add(
                                    "プロセスの終了コードがパイプラインへ出力されます。" +
                                    "プロセスの標準出力ストリームへの出力結果が、詳細メッセージ ストリームに出力されます。");
                            }
                        }

                        // for Verbose message (2) (about Standard Error Stream)
                        if (this.RedirectStandardErrorToOutput)
                        {
                            verboseMessage.Add(
                                "プロセスの標準エラー ストリームへの出力結果が、標準出力ストリームにリダイレクトされます。");
                        }
                        else if (this.RedirectStandardErrorToVerbose)
                        {
                            verboseMessage.Add(
                                "プロセスの標準エラー ストリームへの出力結果が、詳細メッセージ ストリームにリダイレクトされます。");
                        }


                        // WhatIf
                        if (this.WhatIf)
                        {
                            this.WriteObject(string.Format("WhatIf: プロセス ({0}) を開始します。", psInfo));
                            foreach (var line in verboseMessage) { this.WriteObject("WhatIf: " + line); }
                            return;
                        }
                        else
                        {
                            // Ouput Verbose message (2)
                            foreach (var line in verboseMessage) { this.WriteVerbose(line); }
                        }
                        


                        // Retry loop
                        for (int i = 0; i <= this.Retry; i++)
                        {
                            // Process Start
                            p.Start();

                            // Start reading streams
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();


                            // Output Verbose message (1)
                            this.WriteVerbose(string.Format("({0}) プロセス (PID={1}, {2}) を開始しました。", i, p.Id, psInfo));



                            // Output loop
                            bool exitNext = false;
                            while (true)
                            {
#if DEBUG
                                this.WriteDebug(string.Format("p.HasExited={0}, stdout.IsEmpty={1}, stderr.IsEmpty={2}", 
                                    p.HasExited, stdout.IsEmpty, stderr.IsEmpty));
#endif

                                // Standard Output
                                while (!stdout.IsEmpty)
                                {
                                    string line = string.Empty;
                                    if (stdout.TryDequeue(out line))
                                    {
                                        if (this.PassThru) { this.WriteObject(line); }
                                        else
                                        {
                                            if (this.RedirectStandardOutputToWarning) { this.WriteWarning(line); }
                                            else { this.WriteVerbose(line); }
                                        }
                                    }
                                }

                                // Standard Error
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

                                if (i < this.Retry)
                                {
                                    if (this.Interval > 0) { this.WriteWarning(string.Format("({0}) {1} 秒後にプロセスを再開します。\n", i, this.Interval)); }

                                    // Wait
                                    Thread.Sleep(this.Interval * 1000);
                                }
                            }
                        }



                        // Output (ExitCode)
                        if (!this.PassThru) { this.WriteObject(p.ExitCode); }
                    }
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
