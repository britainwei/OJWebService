using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cyjb;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 分支限界的图挖掘算法。
	/// </summary>
	public sealed class GraphMining
	{
		/// <summary>
		/// 要进行挖掘的图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly GraphCollection graphs;
		/// <summary>
		/// 路经检查器。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PathChecker checker;
		/// <summary>
		/// 图挖掘的得分函数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ScoreFunction scoreFunction;
		/// <summary>
		/// 图挖掘的结果。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Result> results;
		/// <summary>
		/// 结果个数的限制。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int resultLimit;
		/// <summary>
		/// 图挖掘的搜索空间。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Graph searchSpace;
		/// <summary>
		/// 使用要挖掘的图集合、路径检查器、得分函数和要保留的最大结果数，初始化 <see cref="GraphMining"/> 类的新实例。
		/// </summary>
		/// <param name="graphs">要进行挖掘的图集合。</param>
		/// <param name="checker">路径检查器。</param>
		/// <param name="scoreFunction">图挖掘的得分函数。</param>
		/// <param name="k">要保留的最大结果数。</param>
		public GraphMining(GraphCollection graphs, PathChecker checker, ScoreFunction scoreFunction, int k)
		{
			this.graphs = graphs;
			this.checker = checker;
			this.scoreFunction = scoreFunction;
			this.results = new List<Result>(k);
			this.resultLimit = k;
		}
		/// <summary>
		/// 进行图挖掘，并返回挖掘结果。
		/// </summary>
		/// <returns>图挖掘的结果。</returns>
		public Result[] Mining()
		{
			// 将所有失败的执行路径作为搜索空间。
			this.searchSpace = new Graph(graphs.Fails);
			// 初始化得分函数。
			scoreFunction.Init(this.graphs, this.searchSpace);
			double scoreLimit = 0.0;
			Queue<ExecutePath> queue = new Queue<ExecutePath>();
			foreach (Edge edge in this.searchSpace.Edges)
			{
				queue.Enqueue(new ExecutePath(edge));
			}
			while (queue.Count > 0.0)
			{
				ExecutePath path = queue.Dequeue();
				if (path.Edges.Count > 5)
				{
					continue;
				}
				Result result = scoreFunction.Score(path);
				if (DoubleComparer.Default.Compare(result.Score, 0d) > 0 &&
					DoubleComparer.Default.Compare(result.Score, scoreLimit) >= 0 &&
					this.AddResult(result))
				{
					scoreLimit = this.results.Count >= resultLimit
						? this.results[this.results.Count - 1].Score
						: 0.0;
				}
				if (result.MaxPossibleScore > 0)
				{
					foreach (ExecutePath nextPath in searchSpace.GetVector(path.Nodes[path.Nodes.Count - 1]).OutEdges
						.Where(edge => !path.ContainsNode(edge.To))
						.Select(edge => path.Extend(edge.To))
						.Where(nextPath => checker.Check(nextPath)))
					{
						queue.Enqueue(nextPath);
					}
				}
			}
			return this.results.ToArray();
		}
		/// <summary>
		/// 将指定的结果，添加到结果集合中。
		/// </summary>
		/// <param name="result">要添加的结果。</param>
		/// <returns>指定的结果是否添加到了结果集合中。</returns>
		private bool AddResult(Result result)
		{
			//this.results.Add(result);
			//return true;
			int idx = this.results.BinarySearch(result);
			if (idx < 0)
			{
				idx = ~idx;
			}
			if (idx >= this.resultLimit)
			{
				return false;
			}
			// 在分数大于当前结果的结果集中，如果存在当前结果的子路径或父路径，则不添加当前路径。
			if (this.results.Take(idx)
					.Any(re => result.Path.Edges.IsSupersetOf(re.Path.Edges) ||
						result.Path.Edges.IsSubsetOf(re.Path.Edges)))
			{
				return false;
			}
			// 在分数小于当前结果的结果集中，排除当前结果的子路径或父路径。
			for (int i = this.results.Count - 1; i >= idx; i--)
			{
				if (result.Path.Edges.IsSupersetOf(results[i].Path.Edges) ||
					result.Path.Edges.IsSubsetOf(results[i].Path.Edges))
				{
					this.results.RemoveAt(i);
				}
			}
			// 插入当前路径。
			this.results.Insert(idx, result);
			if (this.results.Count > this.resultLimit)
			{
				this.results.RemoveRange(this.resultLimit, this.results.Count - this.resultLimit);
			}
			return true;
		}
	}
}
