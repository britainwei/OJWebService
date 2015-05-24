using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OJWebService.TestSuits
{
	/// <summary>
	/// 表示程序执行的跟踪信息。
	/// </summary>
	public sealed class Trace : IEnumerable<int>
	{
		/// <summary>
		/// 跟踪信息的读取器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TextReader reader;
		/// <summary>
		/// 使用是否成功和跟踪信息读取器，初始化 <see cref="Trace"/> 类的新实例。
		/// </summary>
		/// <param name="isSuccessful">此次执行是否成功。</param>
		/// <param name="reader">此次执行的跟踪信息读取器。</param>
		public Trace(bool isSuccessful, TextReader reader)
		{
			this.IsSuccessful = isSuccessful;
			this.reader = reader;
		}
		/// <summary>
		/// 获取此次执行是否成功。
		/// </summary>
		/// <value>如果此次执行是成功的，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsSuccessful { get; private set; }

		#region IEnumerable<int> 成员

		public IEnumerator<int> GetEnumerator()
		{
			// 防止多次枚举。
			if (this.reader == null)
			{
				yield break;
			}
			int value = -1;
			while (true)
			{
				int ich = reader.Read();
				if (ich == -1 || ich == '\n' || ich == '\r')
				{
					if (value != -1)
					{
						yield return value;
					}
					// 移除多余的行。
					while (reader.Peek() == '\n' || reader.Peek() == '\r')
					{
						reader.Read();
					}
					reader = null;
					break;
				}
				if (ich == ',' || ich == ' ')
				{
					if (value != -1)
					{
						yield return value;
						value = -1;
					}
				}
				else if (char.IsDigit((char)ich))
				{
					if (value == -1)
					{
						value = ich - '0';
					}
					else
					{
						value = value * 10 + ich - '0';
					}
				}
			}
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
