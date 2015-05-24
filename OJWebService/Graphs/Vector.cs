using System.Collections.Generic;
using System.Diagnostics;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示图中的顶点。
	/// </summary>
	public sealed class Vector
	{
		/// <summary>
		/// 当前顶点的的出边集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<int, Edge> outEdges = new Dictionary<int, Edge>();
		/// <summary>
		/// 使用顶点的索引初始化 <see cref="Vector"/> 类的新实例。
		/// </summary>
		/// <param name="id">顶点的索引。</param>
		public Vector(int id)
		{
			this.Id = id;
			this.ExecuteCount = 0;
		}
		/// <summary>
		/// 获取顶点的索引。
		/// </summary>
		/// <value>顶点的索引。</value>
		public int Id { get; private set; }
		/// <summary>
		/// 获取或设置顶点的执行次数。
		/// </summary>
		/// <value>顶点的执行次数。</value>
		public int ExecuteCount { get; set; }
		/// <summary>
		/// 获取当前顶点的出边集合。
		/// </summary>
		/// <value>当前顶点的出边集合。</value>
		public IEnumerable<Edge> OutEdges
		{
			get { return this.outEdges.Values; }
		}
		/// <summary>
		/// 获取从当前顶点到指定顶点的边。
		/// </summary>
		/// <returns>从当前顶点到指定顶点的边。</returns>
		public Edge GetEdge(int to)
		{
			Edge edge;
			this.outEdges.TryGetValue(to, out edge);
			return edge;
		}
		/// <summary>
		/// 获取或添加从当前顶点到指定顶点的边。
		/// </summary>
		/// <param name="to">边的结束顶点索引。</param>
		/// <returns>从当前顶点到指定顶点的边。</returns>
		public Edge GetOrAddEdge(int to)
		{
			Edge edge;
			if (!this.outEdges.TryGetValue(to, out edge))
			{
				edge = new Edge(this.Id, to);
				this.outEdges.Add(to, edge);
			}
			return edge;
		}
	}
}
