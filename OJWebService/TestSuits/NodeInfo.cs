namespace OJWebService.TestSuits
{
	/// <summary>
	/// 程序跟踪信息节点的信息。
	/// </summary>
	public sealed class NodeInfo
	{
		/// <summary>
		/// 使用程序跟踪信息节点的信息初始化 <see cref="NodeInfo"/> 类的新实例。
		/// </summary>
		/// <param name="owner">节点所属的函数。</param>
		/// <param name="isFirst">节点是否是所属的函数的第一个节点。</param>
		/// <param name="line">节点所在的行。</param>
		public NodeInfo(string owner, bool isFirst, int line)
		{
			this.OwnedFunction = owner;
			this.IsFirstNode = isFirst;
			this.Line = line;
		}
		/// <summary>
		/// 获取节点所属的函数。
		/// </summary>
		/// <value>节点所属的函数。</value>
		public string OwnedFunction { get; private set; }
		/// <summary>
		/// 获取节点是否是所属的函数的第一个节点。
		/// </summary>
		/// <value>如果节点时所属函数的第一个节点，则为 <c>true</c>；否则为 <c>false</c>。</value>
		public bool IsFirstNode { get; private set; }
		/// <summary>
		/// 获取节点所在的行。
		/// </summary>
		/// <value>节点所在的行。</value>
		public int Line { get; private set; }
	}
}
