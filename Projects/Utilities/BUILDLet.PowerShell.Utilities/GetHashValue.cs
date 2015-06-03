﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.IO;

using BUILDLet.Utilities.Cryptography;


namespace BUILDLet.PowerShell.Utilities
{
    [Cmdlet(VerbsCommon.Get, "HashValue")]
    [CmdletBinding(DefaultParameterSetName = "path")]
    [OutputType(typeof(string), typeof(string[]))]
    public class GetHashValue : PSCmdletExtension
    {
        protected override string Synopsis
        {
            get { return "指定されたハッシュ アルゴリズムを使用して、入力データのハッシュ値を計算します。"; }
        }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "data",
            HelpMessage = "バイト配列のハッシュ値を求める場合に、入力データを指定します。")]
        public byte[] InputObject { get; set; }


        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "path",
            HelpMessage = "ファイルやフォルダーのハッシュ値を求める場合に、入力ファイルのパスを指定します。")]
        public string Path { get; set; }


        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "path", HelpMessage = GetHashValue.helpMessage_Algorithm)]
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "data", HelpMessage = GetHashValue.helpMessage_Algorithm)]
        [PSDefaultValue(Value = "MD5", Help = "既定のアルゴリズムは MD5 です。")]
        public string Algorithm { get; set; }
        private const string helpMessage_Algorithm = "使用するハッシュ アルゴリズムの名前を指定します。既定のアルゴリズムは MD5 です。";


        protected readonly string DefaultAlgorithm = "MD5";

        private List<byte> data = new List<byte>();


        // Pre-Processing Tasks
        protected override void BeginProcessing()
        {
            // Call base method
            base.BeginProcessing();
        }


        // Input Processing Tasks
        protected override void ProcessRecord()
        {
            // Call base method
            base.ProcessRecord();

            // Set default value of "Algorithm" parameter
            if (this.Algorithm == null) { this.Algorithm = this.DefaultAlgorithm; }


            try
            {
                switch (this.ParameterSetName)
                {
                    case "path":
                        string path = this.GetUnresolvedProviderPathFromPSPath(this.Path);

                        if (File.Exists(path))
                        {
                            // File
                            this.WriteVerbose(path + " が見つかりました。");
                            this.WriteObject(new HashCode(path, this.Algorithm).ToString());
                        }
                        else if (Directory.Exists(path))
                        {
                            // Directory
                            this.WriteVerbose(path + " フォルダーを検索します。");
                            this.writeHashValues(path);
                        }
                        else { throw new FileNotFoundException(); }
                        break;


                    case "data":
                        foreach (var datum in this.InputObject) { this.data.Add(datum); }
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
                    case "data":

                        // Process and Output
                        this.WriteObject(new HashCode(this.data.ToArray(), this.Algorithm).ToString());
                        break;

                    default:
                        return;
                }
            }
            catch (Exception e) { throw e; }
        }



        private void writeHashValues(string folderpath)
        {
            // Files
            foreach (var file in Directory.GetFiles(folderpath))
            {
                this.WriteVerbose(file + " ファイルが見つかりました。");
                this.WriteObject(new HashCode(file, this.Algorithm).ToString() + "\t" + file);
            }

            // Directories
            foreach (var folder in Directory.GetDirectories(folderpath))
            {
                this.WriteVerbose(folder + " フォルダーを検索します。");
                this.writeHashValues(folder);
            }
        }
    }
}