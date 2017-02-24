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
using System.Reflection;
using System.IO;

using BUILDLet.Utilities.Diagnostics;


namespace BUILDLet.Utilities.PowerShell.Commands
{
    public abstract class PSCmdletExtension : PSCmdlet
    {
        public static string GetCmdletVersion()
        {
            return ((AssemblyFileVersionAttribute)Assembly.GetCallingAssembly().GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version;
        }


        public static string GetHelpMessage(string command, string synopsis)
        {
            return
@"
名前
    " + command + @"

概要
    " + synopsis + @"
    詳細はヘルプを参照してください。

注釈
    ヘルプを参照するには、次のように入力してください。: " + "\"get-help " + command + "\"" + @"
    技術情報を参照するには、次のように入力してください。: " + "\"get-help " + command + " -full\"" + @"

";
        }


        protected abstract string Synopsis { get; }


        [Parameter(ParameterSetName = "Help", Position = 0,
            HelpMessage = "このコマンドレットのヘルプを表示します。")]
        public SwitchParameter Help { get; protected set; }


        [Parameter(ParameterSetName = "Version", Mandatory = true, Position = 0,
            HelpMessage = "このコマンドレットが含まれるモジュールのバージョンを表示します。")]
        public SwitchParameter Version { get; protected set; }


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
#if DEBUG
            this.WriteDebug(string.Format("{0}() {{ ParameterSetName=\"{1}\" }} is called.", DebugInfo.ShortName, this.ParameterSetName));
#endif

            // Call base method
            base.BeginProcessing();
        }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
#if DEBUG
            this.WriteDebug(string.Format("{0}() is called.", DebugInfo.ShortName));
#endif

            try
            {
                // Call base method
                base.ProcessRecord();

                switch (this.ParameterSetName)
                {
                    case "Help":
                        this.WriteObject(PSCmdletExtension.GetHelpMessage(this.MyInvocation.InvocationName, this.Synopsis));
                        break;

                    case "Version":
                        this.WriteObject(PSCmdletExtension.GetCmdletVersion());
                        break;

                    default:
                        break;
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
#if DEBUG
            this.WriteDebug(string.Format("{0}() is called.", DebugInfo.ShortName));
#endif

            // Call base method
            base.EndProcessing();
        }

        
        // Stop-Processing Tasks
        protected override void StopProcessing()
        {
#if DEBUG
            this.WriteDebug(string.Format("{0}() is called.", DebugInfo.ShortName));
#endif

            // Call base method
            base.StopProcessing();
        }



        // Write Error
        public void WriteError(Exception e, ErrorCategory errorCategory = ErrorCategory.NotSpecified)
        {
            // Write Error
            base.WriteError(new ErrorRecord(e, e.Message, errorCategory, (object)e));
        }


        // Write PSObject
        public void WritePSObject<T>(List<T> members) where T : PSMemberInfo
        {
            PSObject psobj = new PSObject();
            foreach (var member in members) { psobj.Members.Add(member); }

            // Write PSObject
            this.WriteObject(psobj);
        }


        // Comprete ProgressRecord
        public void CompleteProcessRecord(ProgressRecord progress)
        {
            if (progress != null)
            {
                // Complete the Progress
                progress.PercentComplete = 100;
                progress.RecordType = ProgressRecordType.Completed;

                // Write Progress
                this.WriteProgress(progress);
            }
        }


        // If resolve = true, Get RESOLVED path, otherwise (resolve = false) get UN-RESOLVED path
        // If path = null, Get Current Location
        protected string GetLocation(string path = null, bool resolve = true)
        {
#if DEBUG
            this.WriteDebug(string.Format("{0}(\"{1}\", {2}) is called.", DebugInfo.ShortName, path, resolve));
#endif

            string result;

            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    // Set RETURN value to Current Location
                    result = this.SessionState.Path.CurrentFileSystemLocation.Path;
                }
                else
                {
                    if (resolve)
                    {
                        // Resolve Destination Path
                        ProviderInfo provider;
                        var locations = this.GetResolvedProviderPathFromPSPath(path, out provider);

                        // Validation
                        if (locations.Count != 1) { throw new ArgumentException(); }

                        // Set RETURN value
                        result = locations[0];
                    }
                    else
                    {
                        // Set RETURN value (unresolved path)
                        result = this.GetUnresolvedProviderPathFromPSPath(path);
                    }
                }

#if DEBUG
                this.WriteDebug(string.Format("{0}(\"{1}\", {2}) returns \"{3}\"", DebugInfo.ShortName, path, resolve, result));
#endif
            }
            catch (Exception) { throw; }


            // RETURN
            return result;
        }



        // Get RESOLVED path(s)
        protected string[] GetLocations(string path)
        {
#if DEBUG
            this.WriteDebug(string.Format("{0}(\"{1}\") is called.", DebugInfo.ShortName, path));
#endif

            string[] results;

            try
            {
                // Set RETURN value to Resolved Destination Path
                ProviderInfo provider;
                results = (this.GetResolvedProviderPathFromPSPath(path, out provider)).ToArray();

#if DEBUG
                this.WriteDebug(string.Format("{0}(\"{1}\") returns {{", DebugInfo.ShortName, path));
                for (int i = 0; i < results.Length; i++)
                {
                    string result = string.Format("\t\"{0}\"", results[i]);
                    if (i < results.Length - 1) { this.WriteDebug(result + ","); }
                    else { this.WriteDebug(result); }

                }
                this.WriteDebug("}");
#endif
            }
            catch (Exception) { throw; }


            // RETURN
            return results;
        }
    }
}
