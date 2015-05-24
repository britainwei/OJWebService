using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cyjb;
using Cyjb.Compilers;
using Cyjb.Compilers.C;
using Cyjb.IO;
using OJWebService.Graphs;

namespace OJWebService.TestSuits
{
    /// <summary>
    /// 表示一个测试集。
    /// </summary>
    public abstract class TestSuit
    {
        /// <summary>
        /// 错误版本跟踪信息的路径，位于 traces\。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected const string TracesPath = @"traces\";
        /// <summary>
        /// 返回指定路径对应的的测试集。
        /// </summary>
        /// <param name="path">要获取测试集的路径。</param>
        /// <returns>指定路径对应的测试集。</returns>
        public static TestSuit GetTestSuit(string path)
        {
            return new NoSuits(path);
        }
        /// <summary>
        /// 分析源代码的选项。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ParseOptions parseOptions = new ParseOptions
        {
            ParseInclude = true,
        };
        /// <summary>
        /// 生成格式化版本的选项。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly GenerateOptions generateOptions = new GenerateOptions
        {
            RecordLocation = true,
            TabSize = 1,
            VerboseParenthesis = true
        };
        /// <summary>
        /// 测试集所在的路径。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string suitPath;
        /// <summary>
        /// 插桩后的可执行文件。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string executableFile;
        /// <summary>
        /// 使用测试集所在的路径初始化 <see cref="TestSuit"/> 类的新实例。
        /// </summary>
        /// <param name="path">测试集所在的路径。</param>
        protected TestSuit(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("无效的测试集路径：" + path);
            }
            this.suitPath = path;
            // 创建目录。
            string tempPath = Path.Combine(path, TracesPath);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(tempPath);
            }
        }
        /// <summary>
        /// 获取测试集所在路径。
        /// </summary>
        /// <value>测试集所在路径。</value>
        protected string SuitPath
        {
            get { return this.suitPath; }
        }
        /// <summary>
        /// 获取生成格式化版本的选项。
        /// </summary>
        /// <value>生成格式化版本的选项。</value>
        protected GenerateOptions GenerateOptions
        {
            get { return this.generateOptions; }
        }

        #region 获取文件名

        /// <summary>
        /// 获取错误版本的插桩后的可执行文件名。
        /// </summary>
        /// <value>错误版本的插桩后的的可执行文件名。</value>
        protected string ExecutableFile
        {
            get
            {
                if (this.executableFile == null)
                {
                    // 将扩展名从 .c 改为 .c.inst.exe。
                    this.executableFile = this.SourceFile + ".inst.exe";
                }
                return this.executableFile;
            }
        }
        /// <summary>
        /// 获取要编译的源文件名称。
        /// </summary>
        /// <value>要编译的源文件名称。</value>
        protected abstract string SourceFile { get; }
        /// <summary>
        /// 获取指定错误版本的源文件文件名。
        /// </summary>
        /// <returns>指定错误版本的源文件文件名。</returns>
        protected abstract string GetFile();
        /// <summary>
        /// 获取指定错误版本的插桩后的文件名。
        /// </summary>
        /// <returns>指定错误版本的插桩后的的文件名。</returns>
        protected abstract string GetInstrumentedFile();
        /// <summary>
        /// 获取指定错误版本的格式化源文件文件名。
        /// </summary>
        /// <returns>指定错误版本的格式化源文件文件名。</returns>
        public abstract string GetFormattedFile();
        /// <summary>
        /// 获取指定错误版本的数据文件文件名。
        /// </summary>
        /// <returns>指定错误版本的数据文件文件名。</returns>
        public abstract string GetData();

        #endregion // 获取文件名

        #region 获取程序跟踪信息

        /// <summary>
        /// 对指定版本进行插桩。
        /// </summary>
        /// <param name="instrumenter">要使用的插桩器。</param>
        public void Instrument(Instrumenter instrumenter)
        {
            CompileUnit unit;
            // 读取格式化版本。
            string fileName = this.GetFormattedFile();
            unit = this.Format();
            if (unit == null)
            {
                return;
             }
            // 对代码进行插桩。
            fileName = this.GetFile();
            instrumenter.Instrument(unit, Path.GetFileName(fileName) + ".tr");
            fileName = this.GetInstrumentedFile();
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                Syntax.Generate(unit, writer, generateOptions);
            }
            // 保存节点信息。
            instrumenter.Infos.Save(this.GetData());
        }
        /// <summary>
        /// 返回指定的错误版本执行跟踪信息。
        /// </summary>
        /// <param name="instrumenter">要使用的插桩器。</param>
        public TraceSet GetTrace(Instrumenter instrumenter)
        {
            this.Instrument(instrumenter);
            // 编译插桩后的程序。
            this.Compile(this.GetInstrumentedFile(), this.ExecutableFile);
            // 获取跟踪信息。
            this.Execute();
            TraceSet traces = GetTrace(instrumenter.InstrumentLevel);
            return traces;
        }
        /// <summary>
        /// 格式化指定的错误版本。
        /// </summary>
        /// <returns>错误版本的编译单元。</returns>
        protected virtual CompileUnit Format()
        {
            string fileName = this.GetFile();
            // 分析源文件。
            parseOptions.SourceFileName = fileName;
            StreamReader reader = new StreamReader(fileName);
            CompileUnit unit = Syntax.Parse(reader, parseOptions);
            reader.Close();
            // 检查分析异常。
            bool hasError = false;
            foreach (Diagnostic diagnostic in unit.Diagnostics.Where(
                diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
            {
                hasError = true;
            }
            if (hasError)
            {
                return null;
            }
            using (StreamWriter writer = new StreamWriter(this.GetFormattedFile()))
            {
                Syntax.Generate(unit, writer, generateOptions);
            }
            return unit;
        }
        /// <summary>
        /// 执行指定的版本。
        /// </summary>
        protected abstract void Execute();
        /// <summary>
        /// 返回指定的错误版本执行跟踪信息。
        /// </summary>
        /// <param name="level">插桩粒度。</param>
        /// <returns>程序执行跟踪信息。</returns>
        private TraceSet GetTrace(string level)
        {
            bool[] results = TestCaseResults();
            int successCount = results.Count(r => r);
            string tracePath = Path.Combine(this.SuitPath, TracesPath);
            string fileName = this.SuitPath + "info.tr";
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("[{0} : {1}]", TraceSet.TestCaseCount, results.Length);
                writer.WriteLine("[{0} : {1}]", TraceSet.SuccessCountSection, successCount);
                writer.WriteLine("[{0} : {1}]", TraceSet.FailCountSection, results.Length - successCount);
                writer.WriteLine("[{0} : {1}]", TraceSet.InstrumentLevelSection, level);
                for (int i = 0; i < results.Length; i++)
                {
                    writer.WriteLine("[{0} : {1}]", TraceSet.TestCaseSection, results[i] ? TraceSet.Success : TraceSet.Fail);
                    using (StreamReader reader = new StreamReader(tracePath + i + ".tr"))
                    {
                        bool first = true;
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                writer.Write(", ");
                            }
                            writer.Write(line);
                        }
                        writer.WriteLine();
                    }
                }
            }
            return new TraceSet(fileName);
        }

        #endregion // 获取程序跟踪信息

        #region 编译文件

        /// <summary>
        /// 编译指定的源文件。
        /// </summary>
        /// <param name="fileName">要编译的源文件。</param>
        /// <param name="targetFileName">编译的目标文件名。</param>
        protected abstract void Compile(string fileName, string targetFileName);

        #endregion // 编译文件

        #region 比较执行结果

        /// <summary>
        /// 按字节块比较的缓冲区长度。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int BufferSize = 0x100;
        /// <summary>
        /// 返回表示测试用例是否成功的列表，按顺序记录了每个测试用例是成功的还是失败的。
        /// </summary>
        /// <returns>表示测试用例是否成功的列表，如果成功则为 <c>true</c>；否则为 <c>false</c>。</returns>
        protected abstract bool[] TestCaseResults();
        /// <summary>
        /// 比较两个文件的内容是否相同的。
        /// </summary>
        /// <param name="firstFile">要比较的第一个文件。</param>
        /// <param name="secondFile">要比较的第二个文件。</param>
        /// <returns>如果两个文件的内容是相同的，则为 <c>true</c>；否则为 <c>false</c>。</returns>
        protected static bool CompareFileContent(string firstFile, string secondFile)
        {
            if (!File.Exists(firstFile))
            {
                return !File.Exists(secondFile);
            }
            if (!File.Exists(secondFile))
            {
                return false;
            }
            using (FileStream firstStream = new FileStream(firstFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream secondStream = new FileStream(secondFile, FileMode.Open, FileAccess.Read))
                {
                    // 按字节块比较的缓冲区。
                    byte[] firstBuffer = new byte[BufferSize];
                    byte[] secondBuffer = new byte[BufferSize];
                    while (true)
                    {
                        int firstLen = firstStream.Read(firstBuffer, 0, BufferSize);
                        int secondLen = secondStream.Read(secondBuffer, 0, BufferSize);
                        if (firstLen != secondLen)
                        {
                            return false;
                        }
                        if (firstLen == 0)
                        {
                            return true;
                        }
                        for (int i = 0; i < firstLen; i++)
                        {
                            if (firstBuffer[i] != secondBuffer[i])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        #endregion // 比较执行结果

    }
}
