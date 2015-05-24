using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cyjb;
using OJWebService.Utils;

namespace OJWebService.TestSuits
{
	/// <summary>
	/// 表示程序的跟踪信息集。遍历跟踪信息的方式和顺序不能改变，因为是直接从文件中读取的。
	/// </summary>
	public class TraceSet : IEnumerable<Trace>
	{

		#region 字符串常量

		/// <summary>
		/// 测试用例数目信息的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string TestCaseCount = "TestCaseCount";
		/// <summary>
		/// 成功测试用例数目信息的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string SuccessCountSection = "SuccessCount";
		/// <summary>
		/// 失败测试用例数目信息的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string FailCountSection = "FailCount";
		/// <summary>
		/// 插桩粒度信息的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string InstrumentLevelSection = "InstrumentLevel";
		/// <summary>
		/// 测试用例信息的名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string TestCaseSection = "TestCase";
		/// <summary>
		/// 表示成功的测试用例的字符串。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string Success = "success";
		/// <summary>
		/// 表示失败的测试用例的字符串。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public const string Fail = "fail";

		#endregion // 字符串常量

		/// <summary>
		/// 跟踪信息的读取器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TextReader reader;
		/// <summary>
		/// 使用缓存的程序跟踪信息文件初始化 <see cref="TraceSet"/> 的新实例。
		/// </summary>
		/// <param name="fileName">缓存的程序跟踪信息文件名。</param>
		public TraceSet(string fileName)
		{
			ExceptionHelper.CheckArgumentNull(fileName, "fileName");
			this.reader = new StreamReader(fileName);
			// 解析四个配置节。
			string line;
			if (!reader.ReadBracketedLine(out line))
			{
				throw new Exception("执行路径缓存文件格式错误！");
			}
			ParseSection(line);
			if (!reader.ReadBracketedLine(out line))
			{
				throw new Exception("执行路径缓存文件格式错误！");
			}
			ParseSection(line);
			if (!reader.ReadBracketedLine(out line))
			{
				throw new Exception("执行路径缓存文件格式错误！");
			}
			ParseSection(line);
			if (!reader.ReadBracketedLine(out line))
			{
				throw new Exception("执行路径缓存文件格式错误！");
			}
			ParseSection(line);
		}
		/// <summary>
		/// 解析表示配置的行。
		/// </summary>
		/// <param name="line">表示配置的行。</param>
		private void ParseSection(string line)
		{
			int idx = line.IndexOf(':');
			string value = line.Substring(idx + 1).TrimStart();
			switch (line.Substring(0, idx).TrimEnd())
			{
				case TestCaseCount:
					this.Count = int.Parse(value);
					break;
				case SuccessCountSection:
					this.SuccessCount = int.Parse(value);
					break;
				case FailCountSection:
					this.FailCount = int.Parse(value);
					break;
				case InstrumentLevelSection:
					this.InstrumentLevel = value;
					break;
			}
		}
		/// <summary>
		/// 获取包含的跟踪信息的数量。
		/// </summary>
		/// <value>包含的跟踪信息的数量。</value>
		public int Count { get; private set; }
		/// <summary>
		/// 获取包含的成功跟踪信息的数量。
		/// </summary>
		/// <value>包含的成功跟踪信息的数量。</value>
		public int SuccessCount { get; private set; }
		/// <summary>
		/// 获取包含的失败跟踪信息的数量。
		/// </summary>
		/// <value>包含的失败跟踪信息的数量。</value>
		public int FailCount { get; private set; }
		/// <summary>
		/// 获取程序执行的插桩粒度。
		/// </summary>
		/// <value>程序的插桩粒度。</value>
		public string InstrumentLevel { get; private set; }

		#region IEnumerable<Trace> 成员

		public IEnumerator<Trace> GetEnumerator()
		{
			if (this.reader == null)
			{
				yield break;
			}
			for (int i = 0; i < this.Count; i++)
			{
				string line;
				if (!this.reader.ReadBracketedLine(out line))
				{
					throw new Exception("执行路径缓存文件格式错误！");
				}
				int idx = line.IndexOf(':');
				string value = line.Substring(idx + 1).TrimStart();
				yield return new Trace(value == Success, this.reader);
			}
			this.reader.Close();
			this.reader = null;
		}

		#endregion

		#region IEnumerable 成员

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
