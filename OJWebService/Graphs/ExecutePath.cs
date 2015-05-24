using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示搜索过程的路径。
	/// </summary>
	public sealed class ExecutePath
	{
		/// <summary>
		/// 路径的节点列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int[] nodes;
		/// <summary>
		/// 初始化 <see cref="ExecutePath"/> 类的新实例。
		/// </summary>
		private ExecutePath() { }
		/// <summary>
		/// 使用指定的边初始化 <see cref="ExecutePath"/> 类的新实例。
		/// </summary>
		/// <param name="edge">路径包含的边。</param>
		public ExecutePath(Edge edge)
		{
			this.Edges = new HashSet<Edge> { new Edge(edge.From, edge.To) };
			this.nodes = new[] { edge.From, edge.To };
			this.Data = null;
		}
		/// <summary>
		/// 获取路径包含的边。
		/// </summary>
		/// <value>路径包含的边。</value>
		public ISet<Edge> Edges { get; private set; }
		/// <summary>
		/// 获取路径包含的节点。
		/// </summary>
		/// <value>路径包含的节点。</value>
		public IList<int> Nodes { get { return this.nodes; } }
		/// <summary>
		/// 获取或设置路径的额外数据。
		/// </summary>
		/// <value>路径的额外数据。</value>
		public object Data { get; set; }
		/// <summary>
		/// 返回当前路径中是否包含指定标识符的节点。
		/// </summary>
		/// <param name="id">要判断是否包含的节点标识符。</param>
		/// <returns>如果当前路径中包含指定标识符的节点，则为 <c>true</c>；
		/// 否则为 <c>false</c>。</returns>
		public bool ContainsNode(int id)
		{
			return this.nodes.Any(node => node == id);
		}
		/// <summary>
		/// 将当前路径扩展指定的节点，并返回新的路径。
		/// </summary>
		/// <param name="node">要扩展的节点。</param>
		/// <returns></returns>
		public ExecutePath Extend(int node)
		{
			ExecutePath path = new ExecutePath
			{
				Edges = new HashSet<Edge>(this.Edges.Select(e => new Edge(e.From, e.To))),
				nodes = new int[this.nodes.Length + 1],
				Data = this.Data
			};
			path.Edges.Add(new Edge(this.nodes[this.nodes.Length - 1], node));
			this.nodes.CopyTo(path.nodes, 0);
			path.nodes[path.nodes.Length - 1] = node;
			return path;
		}
		/// <summary>
		/// 返回当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式。</returns>
		public override string ToString()
		{
			return string.Join("->", this.nodes);
		}
	}
}
