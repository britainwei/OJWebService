using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cyjb;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示 Fisher 的得分函数。
	/// </summary>
	public sealed class FisherScore : ScoreFunction
	{
		/// <summary>
		/// 软件行为图集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GraphCollection graphCollection;
		/// <summary>
		/// 边在成功的软件行为图中出现的频率。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double[] successFrequencies;
		/// <summary>
		/// 边在失败的软件行为图中出现的频率。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double[] failFrequencies;
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
		/// 包含当前路径的成功的软件行为图。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Graph> successGraphs;
		/// <summary>
		/// 包含当前路径的失败的软件行为图。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<Graph> failGraphs;
		/// <summary>
		/// 使用指定的图集合和路径的搜索空间初始化得分函数。
		/// </summary>
		/// <param name="graphs">图集合。</param>
		/// <param name="searchSpace">路径的搜索空间。</param>
		public override void Init(GraphCollection graphs, Graph searchSpace)
		{
			this.graphCollection = graphs;
			this.successCount = graphs.Successes.Count;
			this.failCount = graphs.Fails.Count;
			this.successFrequencies = new double[successCount];
			this.failFrequencies = new double[failCount];
		}
		/// <summary>
		/// 返回指定路径的得分。
		/// </summary>
		/// <param name="path">要计算得分的路径。</param>
		/// <returns>打分的结果。</returns>
		public override Result Score(ExecutePath path)
		{
			Tuple<IList<Graph>, IList<Graph>> data = path.Data as Tuple<IList<Graph>, IList<Graph>>;
			IList<Graph> baseSuccess, baseFail;
			if (data == null)
			{
				baseSuccess = this.graphCollection.Successes;
				baseFail = this.graphCollection.Fails;
			}
			else
			{
				baseSuccess = data.Item1;
				baseFail = data.Item2;
			}
			// 过滤出包含路径的图。
			this.successGraphs = new List<Graph>(baseSuccess.Where(g => g.Edges.IsSupersetOf(path.Edges)));
			this.failGraphs = new List<Graph>(baseFail.Where(g => g.Edges.IsSupersetOf(path.Edges)));
			path.Data = new Tuple<IList<Graph>, IList<Graph>>(this.successGraphs, this.failGraphs);
			if (this.successGraphs.Count == 0 && this.failGraphs.Count == 0)
			{
				return new Result(path, 0D, 0D);
			}
			double score = 0;
			double maxPossableScore = 0;
			foreach (Edge edge in path.Edges)
			{
				double tmpScore, tmpMaxPossableScore;
				CalFisher(edge, out tmpScore, out tmpMaxPossableScore);
				score += tmpScore;
				maxPossableScore += tmpMaxPossableScore;
			}
			score /= path.Edges.Count;
			maxPossableScore /= path.Edges.Count;
			return new Result(path, score, maxPossableScore);
		}
		/// <summary>
		/// 计算指定边的 Fisheries 得分。
		/// </summary>
		/// <param name="edge">要计算得分的边。</param>
		/// <param name="score">边的得分。</param>
		/// <param name="maxPossableScore">边的可能最大得分。</param>
		private void CalFisher(Edge edge, out double score, out double maxPossableScore)
		{
			int from = edge.From;
			int to = edge.To;
			// 边在成功和失败的测试中出现的频率均值。
			int sucCnt = this.successGraphs.Count;
			double successAvg = 0;
			for (int i = 0; i < sucCnt; i++)
			{
				this.successFrequencies[i] = this.successGraphs[i].GetEdge(from, to).Frequency;
				successAvg += this.successFrequencies[i];
			}
			successAvg /= this.successCount;
			int failCnt = this.failGraphs.Count;
			double failAvg = 0;
			for (int i = 0; i < failCnt; i++)
			{
				this.failFrequencies[i] = this.failGraphs[i].GetEdge(from, to).Frequency;
				failAvg += this.failFrequencies[i];
			}
			failAvg /= this.failCount;
			edge.SuccessFrequency = successAvg;
			edge.FailFrequency = failAvg;
			// 边在成功和失败的测试中出现的频率方差。
			double successSig = 0, failSig = 0;
			for (int i = 0; i < sucCnt; i++)
			{
				successSig += (this.successFrequencies[i] - successAvg) * (this.successFrequencies[i] - successAvg);
			}
			successSig += (this.successCount - sucCnt) * successAvg * successAvg;
			successSig /= this.successCount;
			for (int i = 0; i < failCnt; i++)
			{
				failSig += (this.failFrequencies[i] - failAvg) * (this.failFrequencies[i] - failAvg);
			}
			failSig += (this.failCount - failCnt) * failAvg * failAvg;
			failSig /= this.failCount;
			if (DoubleComparer.Default.Compare(successAvg, failAvg) == 0 || successAvg > failAvg)
			{
				score = 0D;
				maxPossableScore = 0D;
			}
			else
			{
				// 计算 Fisher 得分。
				score = (successAvg - failAvg) * (successAvg - failAvg) / (successSig + failSig);
				if (DoubleComparer.Default.Compare(successAvg, 0D) == 0)
				{
					maxPossableScore = failAvg / failSig;
				}
				else if (DoubleComparer.Default.Compare(failAvg, 0D) == 0)
				{
					maxPossableScore = successAvg / successSig;
				}
				else
				{
					maxPossableScore = Math.Max(successAvg / successSig, failAvg / failSig);
				}
			}
		}
	}
}
