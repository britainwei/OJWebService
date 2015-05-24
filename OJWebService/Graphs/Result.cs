using System;
using Cyjb;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示路径的打分结果。
	/// </summary>
	public sealed class Result : IComparable<Result>
	{
		/// <summary>
		/// 使用结果的路径、得分和超图的最大可能得分初始化 <see cref="Result"/> 类的新实例。
		/// </summary>
		/// <param name="path">结果的路径集合。</param>
		/// <param name="score">结果的得分。</param>
		/// <param name="maxPossibleScore">结果的超图的最大可能得分。</param>
		public Result(ExecutePath path, double score, double maxPossibleScore)
		{
			this.Path = path;
			this.Score = score;
			this.MaxPossibleScore = maxPossibleScore;
		}
		/// <summary>
		/// 获取结果的路径。
		/// </summary>
		/// <value>结果的路径。</value>
		public ExecutePath Path { get; private set; }
		/// <summary>
		/// 获取结果的得分。
		/// </summary>
		/// <value>结果的得分。</value>
		public double Score { get; private set; }
		/// <summary>
		/// 获取结果的超图的最大可能得分。
		/// </summary>
		/// <value>结果的超图的最大可能得分。</value>
		public double MaxPossibleScore { get; private set; }
		/// <summary>
		/// 获取结果的额外数据。
		/// </summary>
		/// <value>结果的额外数据。</value>
		public object Data { get; private set; }
		/// <summary>
		/// 获取或设置结果的文本。
		/// </summary>
		/// <value>结果的文本。</value>
		public string Text { get; set; }

		#region IComparable<Result> 成员

		/// <summary>
		/// 比较当前对象和另一 <see cref="Result"/> 对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的 <see cref="Result"/> 对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。</returns>
		public int CompareTo(Result other)
		{
			// 首先按照得分排序。
			int cmp = DoubleComparer.Default.Compare(this.Score, other.Score);
			if (cmp != 0)
			{
				return -cmp;
			}
			// 得分相同则按照边数排序。
			cmp = this.Path.Edges.Count - other.Path.Edges.Count;
			if (cmp != 0)
			{
				return cmp;
			}
			// 边数相同的，按节点标识符排序。
			int cnt = this.Path.Nodes.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (this.Path.Nodes[i] < other.Path.Nodes[i])
				{
					return -1;
				}
				if (this.Path.Nodes[i] > other.Path.Nodes[i])
				{
					return 1;
				}
			}
			return 0;
		}

		#endregion

		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return this.Score.ToString("0.000") + " " + this.Path;
		}
	}
}
