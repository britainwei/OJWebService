using System.Collections.Generic;
using System.Diagnostics;
using OJWebService.TestSuits;
using Trace = OJWebService.TestSuits.Trace;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示软件行为图的集合。
	/// </summary>
	public sealed class GraphCollection
	{
		/// <summary>
		/// 软件行为图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Graph[] graphs;
		/// <summary>
		/// 成功的软件行为图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Graph[] successes;
		/// <summary>
		/// 失败的软件行为图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Graph[] fails;
		/// <summary>
		/// 根据指定的程序执行跟踪信息和节点信息初始化 <see cref="GraphCollection"/> 类的新实例。
		/// </summary>
		/// <param name="traces">程序执行跟踪信息。</param>
		/// <param name="infos">节点信息。</param>
		public GraphCollection(TraceSet traces, NodeInfoCollection infos)
		{
			this.graphs = new Graph[traces.Count];
			this.successes = new Graph[traces.SuccessCount];
			this.fails = new Graph[traces.FailCount];
			int idx = 0, successIdx = 0, failIdx = 0;
			foreach (Trace trace in traces)
			{
				this.graphs[idx] = new Graph(trace, infos);
				if (this.graphs[idx].IsSuccessful)
				{
					this.successes[successIdx++] = this.graphs[idx];
				}
				else
				{
					this.fails[failIdx++] = this.graphs[idx];
				}
				idx++;
			}
		}
		/// <summary>
		/// 获取所有软件行为图集合。
		/// </summary>
		/// <value>软件行为图集合。</value>
		public IList<Graph> Graphs
		{
			get { return this.graphs; }
		}
		/// <summary>
		/// 获取成功的软件行为图集合。
		/// </summary>
		/// <value>成功的软件行为图集合。</value>
		public IList<Graph> Successes { get { return this.successes; } }
		/// <summary>
		/// 获取失败的软件行为图集合。
		/// </summary>
		/// <value>失败的软件行为图集合。</value>
		public IList<Graph> Fails { get { return this.fails; } }
	}
}
