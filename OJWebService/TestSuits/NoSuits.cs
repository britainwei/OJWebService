using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cyjb;
using OJWebService.Utils;
using System.Text;

namespace OJWebService.TestSuits
{
    /// <summary>
    /// 表示一个 Siemens 测试集。
    /// </summary>
    public sealed class NoSuits : TestSuit
    {

        #region 测试集结构
        /// <summary>
        /// 获取程序跟踪信息的批处理文件，为 scripts\gettraces.sh。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string GetTracesShFileName = @"gettraces.sh";
        /// <summary>
        /// 获取程序跟踪信息的批处理文件，为 scripts\gettraces.bat。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string GetTracesBatFileName = @"gettraces.bat";

        #endregion // 测试集结构

        /// <summary>
        /// 要编译的源文件名称。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string sourceFile;
        /// <summary>
        /// 输入输出测试用例数量。
        /// </summary>
        private int caseCount; 
        /// <summary>
        /// 使用测试集所在的路径初始化类的新实例。
        /// </summary>
        /// <param name="path">测试集所在的路径。</param>
        public NoSuits(string path)
            : base(path)
        {
            this.GenerateOptions.KRStyle = true;
            CheckDirectory(path);
            caseCount = Directory.EnumerateFiles(Path.Combine(this.SuitPath, "inputs")).Count();
        }

        #region 获取文件名

        /// <summary>
        /// 获取要编译的源文件名称。
        /// </summary>
        /// <value>要编译的源文件名称。</value>
        protected override string SourceFile
        {
            get
            {
                return this.sourceFile ??
                    (this.sourceFile = Directory.GetFiles(this.SuitPath, "*.c")[0]);
            }
        }
        /// <summary>
        /// 获取源文件文件名。
        /// </summary>
        /// <param name="version">要获取源文件的错误版本。</param>
        /// <returns>指定错误版本的源文件文件名。</returns>
        protected override string GetFile()
        {
            return this.SourceFile;
        }
        /// <summary>
        /// 获取指定错误版本的插桩后的文件名。
        /// </summary>
        /// <param name="version">要获取插桩后的的文件名的错误版本。</param>
        /// <returns>指定错误版本的插桩后的的文件名。</returns>
        protected override string GetInstrumentedFile()
        {
            return this.GetFile() + ".inst.c";
        }
        /// <summary>
        /// 获取指定错误版本的格式化源文件文件名。
        /// </summary>
        /// <param name="version">要获取格式化源文件的错误版本。</param>
        /// <returns>指定错误版本的格式化源文件文件名。</returns>
        public override string GetFormattedFile()
        { 
            return this.SourceFile + ".formatted.c";
        }
        /// <summary>
        /// 获取指定错误版本的数据文件文件名。
        /// </summary>
        /// <param name="version">要获取数据文件的错误版本。</param>
        /// <returns>指定错误版本的数据文件文件名。</returns>
        public override string GetData()
        {
            return Path.Combine(this.SuitPath, "data.txt");
        }

        #endregion // 获取文件名

        #region 获取程序跟踪信息

        /// <summary>
        /// 执行。
        /// </summary>
        protected override void Execute()
        {
            GenerateBatFile();
            Run(Path.Combine(this.SuitPath, GetTracesBatFileName).Replace('\\', '/'), null);
           // Run(Path.Combine(this.SuitPath, "gettraces.bat"), null);
        }

        /// <summary>
        /// 生成bat可执行脚本。
        /// </summary>
        protected void GenerateBatFile()
        {
            TextWriter writer = File.CreateText(Path.Combine(this.SuitPath, GetTracesBatFileName));
            writer.WriteLine("@echo off");
            String fileName = this.SourceFile.Substring(this.SourceFile.LastIndexOf('\\') + 1);

            for (int i = 0; i < caseCount; i++)
            {
                writer.WriteLine("\"{0}.inst.exe\" < \"inputs/t{1}\" > \"outputs/t{2}\" ", fileName, i, i);
                writer.WriteLine("copy /Y \"{0}.tr\" \"traces/{1}.tr\" > nul", fileName, i);
            }
            writer.Close();
        }

        #endregion // 获取程序跟踪信息

        #region 编译文件

        /// <summary>
        /// 编译指定的源文件。
        /// </summary>
        /// <param name="fileName">要编译的源文件。</param>
        /// <param name="targetFileName">编译的目标文件名。</param>
        protected override void Compile(string fileName, string targetFileName)
        {
            Run("gcc", string.Concat("-w \"", fileName, "\" -o \"", targetFileName, "\""));
        }

        #endregion // 编译文件

        #region 比较执行结果

        /// <summary>
        /// 返回表示测试用例是否成功的列表，按顺序记录了每个测试用例是成功的还是失败的。
        /// </summary>
        /// <param name="version">要判断是否成功的错误版本。</param>
        /// <returns>表示测试用例是否成功的列表，如果成功则为 <c>true</c>；否则为 <c>false</c>。</returns>
        protected override bool[] TestCaseResults()
        {
            bool[] result = new bool[caseCount];
            string correctOutputFile = Path.Combine(this.SuitPath, "correctoutputs" + Path.DirectorySeparatorChar, "t");
            string newOutputFile = Path.Combine(this.SuitPath, @"outputs" + Path.DirectorySeparatorChar, "t");
            
            for (int i = 0; i < caseCount; i++)
            {
                result[i] = CompareFileContent(correctOutputFile + i, newOutputFile + i);
            }
            return result;
        }

        #endregion // 比较执行结果

        #region 辅助函数

        /// <summary>
        /// 检测测试集目录是否包含完整的结构。
        /// </summary>
        /// <param name="path">测试集所在的路径。</param>
        private static void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("找不到源文件目录");
            }
            string tempPath = Path.Combine(path, "inputs" + Path.DirectorySeparatorChar);
            if (!Directory.Exists(tempPath))
            {
                throw new Exception("没有输入");
            }
            tempPath = Path.Combine(path, "correctoutputs" + Path.DirectorySeparatorChar);
            if (!Directory.Exists(tempPath))
            {
                throw new Exception("没有正确输出");
            }
            tempPath = Path.Combine(path, "outputs" + Path.DirectorySeparatorChar);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            tempPath = Path.Combine(path, "traces" + Path.DirectorySeparatorChar);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
        }
        /// <summary>
        /// 运行指定的命令。
        /// </summary>
        /// <param name="command">要运行的命令。</param>
        /// <param name="arguments">命令的的参数。</param>
        private static void Run(string command, string arguments)
        {
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(command) ?? string.Empty,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,                                                                                                          
                RedirectStandardError = false,
                CreateNoWindow = true
            };
            proc.StartInfo = startInfo;
            try
            {
                proc.Start();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine("error in proc start in Nosuits run function info : " + ex.Message);
            }
            proc.WaitForExit();
        }

        #endregion // 辅助函数

    }
}
