namespace OJWebService.Utils
{
	/// <summary>
	/// 提供字符串排序的扩展方法。
	/// </summary>
	public static class StringCompareExt
	{
		/// <summary>
		/// 比较两个字符串的值。
		/// </summary>
		/// <param name="x">要比较的第一个字符串。</param>
		/// <param name="y">要比较的第二个字符串。</param>
		/// <returns>一个有符号整数，指示 <paramref name="x"/> 与 <paramref name="y"/> 的相对值。</returns>
		public static int NumericCompare(string x, string y)
		{
			for (int ix = 0, iy = 0; ix < x.Length && iy < y.Length; )
			{
				char cx = x[ix], cy = y[iy];
				if (char.IsDigit(cx) && char.IsDigit(cy))
				{
					int sx = ix, sy = iy;
					for (ix++; ix < x.Length; ix++)
					{
						if (!char.IsDigit(x, ix))
						{
							break;
						}
					}
					for (iy++; iy < y.Length; iy++)
					{
						if (!char.IsDigit(y, iy))
						{
							break;
						}
					}
					string nx = x.Substring(sx, ix - sx);
					string ny = y.Substring(sy, iy - sy);
					int cmp = int.Parse(nx) - int.Parse(ny);
					if (cmp != 0)
					{
						return cmp;
					}
					if (nx.Length != ny.Length)
					{
						return string.CompareOrdinal(nx, ny);
					}
				}
				else if (cx != cy)
				{
					return cx - cy;
				}
				else
				{
					ix++;
					iy++;
				}
			}
			return 0;
		}
	}
}