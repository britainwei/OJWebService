namespace OJWebService.Graphs
{
	/// <summary>
	/// 表示基于图挖掘的错误定位方法的得分函数。
	/// </summary>
	public abstract class ScoreFunction
	{
		/// <summary>
		/// 使用指定的图集合和路径的搜索空间初始化得分函数。
		/// </summary>
		/// <param name="graphs">图集合。</param>
		/// <param name="searchSpace">路径的搜索空间。</param>
		public abstract void Init(GraphCollection graphs, Graph searchSpace);
		/// <summary>
		/// 返回指定路径的得分。
		/// </summary>
		/// <param name="path">要计算得分的路径。</param>
		/// <returns>打分的结果。</returns>
		public abstract Result Score(ExecutePath path);
	}
}
