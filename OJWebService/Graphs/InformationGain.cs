using System;
using System.Diagnostics;
using System.Linq;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示信息增益的得分函数。
	/// </summary>
	public sealed class InformationGain : ScoreFunction
	{
		/// <summary>
		/// 软件行为图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GraphCollection graphCollection;
		/// <summary>
		/// 全部软件行为图的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int graphCount;
		/// <summary>
		/// 成功的软件行为图的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int successCount;
		/// <summary>
		/// 失败的软件行为图的个数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int failCount;
		/// <summary>
		/// 所有测试用例的熵。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double allEntropy;
		/// <summary>
		/// 使用指定的图集合和路径的搜索空间初始化得分函数。
		/// </summary>
		/// <param name="graphs">图集合。</param>
		/// <param name="searchSpace">路径的搜索空间。</param>
		public override void Init(GraphCollection graphs, Graph searchSpace)
		{
			this.graphCollection = graphs;
			this.graphCount = graphCollection.Graphs.Count;
			this.successCount = graphCollection.Successes.Count;
			this.failCount = graphCollection.Fails.Count;
			this.allEntropy = Entropy(successCount, failCount);
		}
		/// <summary>
		/// 返回指定路径的得分。
		/// </summary>
		/// <param name="path">要计算得分的路径。</param>
		/// <returns>打分的结果。</returns>
		public override Result Score(ExecutePath path)
		{
			int success = this.graphCollection.Successes.Count(g => g.Edges.IsSupersetOf(path.Edges));
			int fail = this.graphCollection.Fails.Count(g => g.Edges.IsSupersetOf(path.Edges));
			double score = CalInformationGain(success, fail);
			double maxPossableScore = Math.Max(CalInformationGain(success, 0), CalInformationGain(0, fail));
			return new Result(path, score, maxPossableScore);
		}
		/// <summary>
		/// 计算信息增益。
		/// </summary>
		/// <param name="success">正例的样例数。</param>
		/// <param name="fail">反例的样例数。</param>
		/// <returns>信息增益的结果。</returns>
		private double CalInformationGain(int success, int fail)
		{
			// 计算信息增益。
			//return Hc - ((success.Count + fail.Count) / all) * Entropy(success.Count, fail.Count);
			return allEntropy - ((double)(success + fail) / graphCount) * Entropy(success, fail) -
				((double)(graphCount - success - fail) / graphCount) * Entropy(successCount - success, failCount - fail);
		}
		/// <summary>
		/// 计算熵。
		/// </summary>
		/// <param name="success">正例的样例数。</param>
		/// <param name="fail">反例的样例数。</param>
		/// <returns>熵值。</returns>
		private static double Entropy(int success, int fail)
		{
			int all = success + fail;
			double hs = success == 0 ? 0D : -((double)success / all) * Math.Log((double)success / all, 2D);
			double hf = fail == 0 ? 0D : -((double)fail / all) * Math.Log((double)fail / all, 2D);
			return hs + hf;
		}
	}
}
