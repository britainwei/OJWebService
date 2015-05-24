using System;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示图中的边。
	/// </summary>
	public sealed class Edge : IEquatable<Edge>
	{
		/// <summary>
		/// 初始化 <see cref="Edge"/> 类的新实例。
		/// </summary>
		/// <param name="from">边的起始顶点索引。</param>
		/// <param name="to">边的结束顶点索引。</param>
		public Edge(int from, int to)
		{
			this.From = from;
			this.To = to;
			this.ExecuteCount = 0;
			this.SuccessFrequency = this.FailFrequency = double.NaN;
		}
		/// <summary>
		/// 获取边的起始顶点索引。
		/// </summary>
		/// <value>边的起始顶点索引。</value>
		public int From { get; private set; }
		/// <summary>
		/// 获取边的结束顶点索引。
		/// </summary>
		/// <value>边的结束顶点索引。</value>
		public int To { get; private set; }
		/// <summary>
		/// 获取或设置边的执行次数。
		/// </summary>
		/// <value>边的执行次数。</value>
		public int ExecuteCount { get; set; }
		/// <summary>
		/// 获取或设置边的执行频率。
		/// </summary>
		/// <value>边的执行频率。</value>
		public double Frequency { get; set; }
		/// <summary>
		/// 获取或设置边在成功执行中的平均执行频率。
		/// </summary>
		/// <value>边在成功执行中的平均执行频率。</value>
		public double SuccessFrequency { get; set; }
		/// <summary>
		/// 获取或设置边在失败执行中的平均执行频率。
		/// </summary>
		/// <value>边在失败执行中的平均执行频率。</value>
		public double FailFrequency { get; set; }
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Concat("[", this.From, "->", this.To, ",", this.Frequency.ToString("0.000"), "]");
		}
		/// <summary>
		/// 将指定的边编码。
		/// </summary>
		/// <param name="from">边的起始顶点索引。</param>
		/// <param name="to">边的结束顶点索引。</param>
		/// <returns>边的编码。</returns>
		private static int Encode(int from, int to)
		{
			return (from << 16) | to;
		}

		#region IEquatable<Edge> 成员

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Equals(Edge other)
		{
			if (ReferenceEquals(other, this))
			{
				return true;
			}
			if (ReferenceEquals(other, null))
			{
				return false;
			}
			return this.From == other.From && this.To == other.To;
		}

		#endregion // IEquatable<Edge> 成员

		#region object 成员

		/// <summary>
		/// 确定指定的 <see cref="System.Object"/> 是否等于当前的 <see cref="Edge"/>。
		/// </summary>
		/// <param name="obj">与当前的 <see cref="Edge"/> 进行比较的 object。</param>
		/// <returns>如果指定的 <see cref="System.Object"/> 等于当前的 <see cref="Edge"/>，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public override bool Equals(object obj)
		{
			Edge thisObj = obj as Edge;
			return !ReferenceEquals(thisObj, null) && this.Equals(thisObj);
		}
		/// <summary>
		/// 用于 <see cref="Edge"/> 类型的哈希函数。
		/// </summary>
		/// <returns>当前 <see cref="Edge"/> 的哈希代码。</returns>
		public override int GetHashCode()
		{
			return Encode(this.From, this.To);
		}

		#endregion // object 成员

		#region 运算符重载

		/// <summary>
		/// 判断两个 <see cref="Edge"/> 是否相同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Edge"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Edge"/> 对象。</param>
		/// <returns>如果两个 <see cref="Edge"/> 对象相同，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator ==(Edge obj1, Edge obj2)
		{
			if (ReferenceEquals(obj1, obj2))
			{
				return true;
			}
			return !ReferenceEquals(obj1, null) && obj1.Equals(obj2);
		}

		/// <summary>
		/// 判断两个 <see cref="Edge"/> 是否不同。
		/// </summary>
		/// <param name="obj1">要比较的第一个 <see cref="Edge"/> 对象。</param>
		/// <param name="obj2">要比较的第二个 <see cref="Edge"/> 对象。</param>
		/// <returns>如果两个 <see cref="Edge"/> 对象不同，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool operator !=(Edge obj1, Edge obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion // 运算符重载

	}
}
