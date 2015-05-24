namespace OJWebService.Utils
{
	/// <summary>
	/// 提供字符串的扩展方法。
	/// </summary>
	public static class StringExt
	{
		/// <summary>
		/// 返回当前字符串的指定子串是否全部是空白。
		/// </summary>
		/// <param name="str">要判断的字符串。</param>
		/// <param name="start">子串的起始索引。</param>
		/// <param name="length">子串的长度。</param>
		/// <returns>如果指定子串只包含空白，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsWhiteSpace(this string str, int start, int length)
		{
			for (int idx = start, i = 0; i < length; start++, i++)
			{
				if (!char.IsWhiteSpace(str, idx))
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// 分析指定的整数数组字符串。
		/// </summary>
		/// <param name="str">要分析的整数数组字符串。</param>
		/// <returns>得到的结果整数数组。</returns>
		public static int[] ParseIntArray(string str)
		{
			string[] valueStrings = str.Split(',');
			int[] values = new int[valueStrings.Length];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = int.Parse(valueStrings[i]);
			}
			return values;
		}
	}
}
