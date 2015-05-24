using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OJWebService.TestSuits;
using Trace = OJWebService.TestSuits.Trace;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示一幅软件行为图。
	/// </summary>
	public sealed class Graph
	{
		/// <summary>
		/// 图中包含的所有顶点的集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IDictionary<int, Vector> vectors = new Dictionary<int, Vector>();
		/// <summary>
		/// 图中包含的所有边集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ISet<Edge> edges = new HashSet<Edge>();
		/// <summary>
		/// 使用要合并的软件行为图信息初始化 <see cref="Graph"/> 类的新实例。
		/// </summary>
		/// <param name="graphs">要合并的软件行为图。</param>
		public Graph(IEnumerable<Graph> graphs)
		{
			foreach (Edge edge in graphs.SelectMany(graph => graph.edges))
			{
				this.AddEdge(edge.From, edge.To);
			}
		}
		/// <summary>
		/// 使用指定的程序执行跟踪信息和节点信息初始化 <see cref="Graph"/> 类的新实例。
		/// </summary>
		/// <param name="trace">程序执行跟踪信息。</param>
		/// <param name="nodeInfos">节点信息集合。</param>
		public Graph(Trace trace, NodeInfoCollection nodeInfos)
		{
			this.IsSuccessful = trace.IsSuccessful;
			// 添加边。
			int lastIdx = -1;
			foreach (int node in trace)
			{
				if (lastIdx != -1)
				{
					this.AddEdge(lastIdx, node);
				}
				lastIdx = node;
			}
			// 计算边被执行的频率。
			foreach (Vector v in this.Vectors)
			{
				string function = nodeInfos[v.Id].OwnedFunction;
				if (v.OutEdges.Any(e => nodeInfos[e.To].OwnedFunction != function))
				{
					foreach (Edge e in v.OutEdges)
					{
						e.Frequency = 1.0;
					}
				}
				else
				{
					int maxCount = v.OutEdges.Sum(e => e.ExecuteCount);
					if (maxCount > 0)
					{
						foreach (Edge e in v.OutEdges)
						{
							e.Frequency = (double)e.ExecuteCount / maxCount;
						}
					}
				}
			}
		}
		/// <summary>
		/// 获取当前软件行为图是否对应于成功的测试用例。
		/// </summary>
		/// <value>如果当前软件行为图对应于成功的测试用例，则为 <c>true</c>；
		/// 如果对应于失败的测试用例，则为 <c>false</c>。</value>
		public bool IsSuccessful { get; private set; }
		/// <summary>
		/// 获取图中包含的所有顶点的集合。
		/// </summary>
		/// <value>图中包含的所有顶点的集合。</value>
		public ICollection<Vector> Vectors
		{
			get { return this.vectors.Values; }
		}
		/// <summary>
		/// 获取图中包含的所有边的集合。
		/// </summary>
		/// <value>图中包含的所有边的集合。</value>
		public ISet<Edge> Edges
		{
			get { return this.edges; }
		}
		/// <summary>
		/// 返回指定标识符的顶点。
		/// </summary>
		/// <param name="id">要获取的顶点标识符。</param>
		/// <returns>指定标识符的顶点。</returns>
		public Vector GetVector(int id)
		{
			Vector v;
			this.vectors.TryGetValue(id, out v);
			return v;
		}
		/// <summary>
		/// 返回指定起始终止节点的边。
		/// </summary>
		/// <param name="from">边的起始节点。</param>
		/// <param name="to">边的终止节点。</param>
		/// <returns>指定起始终止节点的边。</returns>
		public Edge GetEdge(int from, int to)
		{
			Vector vector;
			if (vectors.TryGetValue(from, out vector))
			{
				return vector.GetEdge(to);
			}
			return null;
		}
		/// <summary>
		/// 向当前软件行为图中添加一条边。
		/// </summary>
		/// <param name="from">边的起始顶点索引。</param>
		/// <param name="to">边的结束顶点索引。</param>
		private void AddEdge(int from, int to)
		{
			Vector fromVector, toVector;
			if (!vectors.TryGetValue(from, out fromVector))
			{
				fromVector = new Vector(from);
				vectors.Add(from, fromVector);
				fromVector.ExecuteCount++;
			}
			if (!vectors.TryGetValue(to, out toVector))
			{
				toVector = new Vector(to);
				vectors.Add(to, toVector);
			}
			Edge edge = fromVector.GetOrAddEdge(to);
			toVector.ExecuteCount++;
			edge.ExecuteCount++;
			edges.Add(edge);
		}
	}
}
