using System.Collections.Generic;
using System.IO;
using Cyjb;

namespace OJWebService.Utils
{
	/// <summary>
	/// 包括了 <see cref="TextReader"/> 类的扩展方法。
	/// </summary>
	public static class TextReaderExt
	{
		/// <summary>
		/// 读取下一行注释数据。
		/// </summary>
		/// <param name="reader">文本读取器。</param>
		/// <returns>下一行非注释数据，如果没有更多数据则为 <c>null</c>。</returns>
		public static string ReadNonCommentLine(this TextReader reader)
		{
			while (true)
			{
				string line = ReadNonEmptyLine(reader);
				if (line == null)
				{
					return null;
				}
				if (line[0] != '#')
				{
					return line;
				}
			}
		}
		/// <summary>
		/// 读取下一行非空数据。
		/// </summary>
		/// <param name="reader">文本读取器。</param>
		/// <returns>下一行非空数据，如果没有更多数据则为 <c>null</c>。</returns>
		public static string ReadNonEmptyLine(this TextReader reader)
		{
			while (true)
			{
				string line = reader.ReadLine();
				if (line == null)
				{
					return null;
				}
				line = line.Trim();
				if (line.Length > 0)
				{
					return line;
				}
			}
		}
		/// <summary>
		/// 读取在方括号内的文本。
		/// </summary>
		/// <param name="reader">文本读取器。</param>
		/// <param name="line">读取的方括号内的文本。</param>
		/// <returns>如果下一行是方括号内的文本，则为 <c>true</c>；否则为 <c>false</c>。
		/// 如果没有更多数据，则也为 <c>false</c>。</returns>
		public static bool ReadBracketedLine(this TextReader reader, out string line)
		{
			while (true)
			{
				line = reader.ReadLine();
				if (line == null)
				{
					return false;
				}
				int left = line.IndexOf('[');
				if (!line.IsWhiteSpace(0, left))
				{
					return false;
				}
				int right = line.LastIndexOf(']');
				if (!line.IsWhiteSpace(right, line.Length - right - 1))
				{
					return false;
				}
				left = line.FirstIndex(ch => !char.IsWhiteSpace(ch), left + 1);
				right = line.LastIndex(ch => !char.IsWhiteSpace(ch), right - 1);
				if (left >= right)
				{
					return false;
				}
				line = line.Substring(left, right - left + 1);
				return true;
			}
		}
		/// <summary>
		/// 从文本读取器中读取一行整数数组。
		/// </summary>
		/// <param name="reader">文本读取器。</param>
		/// <returns>一行整数数组。</returns>
		public static int[] ReadIntArray(this TextReader reader)
		{
			List<int> list = new List<int>();
			int value = -1;
			while (true)
			{
				int ich = reader.Read();
				if (ich == -1 || ich == '\n' || ich == '\r')
				{
					if (value != -1)
					{
						list.Add(value);
					}
					while (reader.Peek() == '\n' || reader.Peek() == '\r')
					{
						reader.Read();
					}
					return list.ToArray();
				}
				if (ich == ',' || ich == ' ')
				{
					if (value != -1)
					{
						list.Add(value);
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
	}
}