using System;
using System.Collections.Generic;

namespace OJWebService.Utils
{
	/// <summary>
	/// 提供对 <see cref="IEnumerable{T}"/> 
	/// 接口的扩展方法。
	/// </summary>
	public static class EnumerableExt
	{
		/// <summary>
		/// 返回序列中满足指定条件的第一个元素的索引。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中返回元素索引的 
		/// <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <param name="startIndex">要搜索的从零开始的索引。</param>
		/// <returns>序列中通过指定谓词函数中的测试的第一个元素的索引。
		/// 如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="source"/> 
		/// 或 <paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int FirstIndex<TSource>(this IEnumerable<TSource> source,
			Func<TSource, bool> predicate, int startIndex)
		{
			int idx = 0;
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			for (; idx < startIndex; idx++)
			{
				if (!enumerator.MoveNext())
				{
					return -1;
				}
			}
			for (;enumerator.MoveNext();idx++)
			{
				if (predicate(enumerator.Current))
				{
					return idx;
				}
			}
			return -1;
		}
		/// <summary>
		/// 返回序列中满足指定条件的最后一个元素的索引。
		/// </summary>
		/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
		/// <param name="source">要从中返回元素索引的 
		/// <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/>。</param>
		/// <param name="predicate">用于测试每个元素是否满足条件的函数。</param>
		/// <param name="startIndex">要搜索的从零开始的索引。</param>
		/// <returns>序列中通过指定谓词函数中的测试的最后一个元素的索引。
		/// 如果没有这样的元素，则返回 <c>-1</c>。</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="source"/> 
		/// 或 <paramref name="predicate"/> 为 <c>null</c>。</exception>
		public static int LastIndex<TSource>(this IEnumerable<TSource> source,
			Func<TSource, bool> predicate, int startIndex)
		{
			int lastIdx = -1;
			int idx = 0;
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					if (idx > startIndex)
					{
						return lastIdx;
					}
					lastIdx = idx;
				}
				idx++;
			}
			return lastIdx;
		}
	}
}
