using System;
using System.IO;
using System.Text;

namespace OJWebService.Utils
{
	/// <summary>
	/// 表示 .sh 文件到 .bat 文件的简单转换器。
	/// </summary>
	public static class ShellConverter
	{
		/// <summary>
		/// .sh 文件中的 cp 命令。
		/// </summary>
		private const string CpCommand = "cp $ARISTOTLE_DB_DIR/";
		/// <summary>
		/// 将 .sh 文件的内容，转换为相应的 .bat 文件内容。
		/// </summary>
		/// <param name="fileName">要转换的 .sh 文件名称。</param>
		/// <returns>转换后的 .bat 文件内容。</returns>
		public static string Convert(string fileName)
		{
			string[] lines = File.ReadAllLines(fileName);
			// 将第一行 echo script type 删去。
			lines[0] = "@echo off";
			for (int i = 1; i < lines.Length; i++)
			{
				string line = lines[i];
				if (line.StartsWith(CpCommand, StringComparison.Ordinal))
				{
					// 将 cp 命令替换为 copy 命令。
					lines[i] = "copy /Y " + ProcessPath(line.Substring(CpCommand.Length)) + " >nul";
				}
				else if (!line.StartsWith("echo", StringComparison.Ordinal))
				{
					lines[i] = ProcessPath(ConvertString(line));
				}
			}
			return string.Join(Environment.NewLine, lines);
		}
		/// <summary>
		/// 将指定字符串中类似 ../something 的路径两边添加引号。
		/// </summary>
		/// <param name="str">要处理路径的字符串。</param>
		/// <returns>处理完毕的字符串。</returns>
		private static string ProcessPath(string str)
		{
			StringBuilder text = new StringBuilder();
			bool preIsWhiteSpace = true;
			int start = 0;
			int replaceLen = str.Length - 2;
			for (int i = 0; i < replaceLen; i++)
			{
				// 找到 ../ 起始的路径。
				if (str[i] == '.' && preIsWhiteSpace && str[i + 1] == '.' && str[i + 2] == '/')
				{
					text.Append(str.Substring(start, i - start));
					text.Append('"');
					start = i;
					for (i += 3; i < str.Length; i++)
					{
						if (char.IsWhiteSpace(str, i))
						{
							break;
						}
					}
					text.Append(str.Substring(start, i - start));
					text.Append('"');
					start = i;
					if (i >= str.Length)
					{
						break;
					}
				}
				preIsWhiteSpace = char.IsWhiteSpace(str, i);
			}
			if (start < str.Length)
			{
				text.Append(str.Substring(start));
			}
			return text.ToString();
		}
		/// <summary>
		/// 将指定字符串中类似 'string' 的字符串转换为 windows 形式。
		/// </summary>
		/// <param name="str">要转换的字符串。</param>
		/// <returns>处理完毕的字符串。</returns>
		private static string ConvertString(string str)
		{
			StringBuilder text = new StringBuilder();
			bool inString = false;
			for (int i = 0; i < str.Length; i++)
			{
				switch (str[i])
				{
					case '"':
						if (inString)
						{
							text.Append("\"\"");
						}
						else
						{
							text.Append('"');
						}
						break;
					case '%':
						if (inString)
						{
							text.Append("%%");
						}
						else
						{
							text.Append(str[i]);
						}
						break;
					case '\\':
						if (inString)
						{
							text.Append(str[i]);
						}
						else
						{
							text.Append(str[i + 1]);
							i++;
						}
						break;
					case '\'':
						text.Append('"');
						inString = !inString;
						break;
					default:
						text.Append(str[i]);
						break;
				}
			}
			return text.ToString();
		}
	}
}