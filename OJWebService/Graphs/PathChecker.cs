using System.Collections.Generic;
using System.Diagnostics;
using OJWebService.TestSuits;

namespace OJWebService.Graphs
{
	/// <summary>
	/// 检查路径是否合法的类。
	/// </summary>
	public sealed class PathChecker
	{
		/// <summary>
		/// 节点的信息集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NodeInfoCollection infos;
		/// <summary>
		/// 函数名称的列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Stack<string> functionStack = new Stack<string>();
		/// <summary>
		/// 使用节点的信息集合初始化 <see cref="PathChecker"/> 类的新实例。
		/// </summary>
		/// <param name="infos">节点的信息集合。</param>
		public PathChecker(NodeInfoCollection infos)
		{
			this.infos = infos;
		}
		/// <summary>
		/// 检查指定的路径是否合法。
		/// </summary>
		/// <param name="path">要检查的路径。</param>
		/// <returns>如果路径合法，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public bool Check(ExecutePath path)
		{
			int cnt = path.Nodes.Count;
			NodeInfo lastNode = infos[path.Nodes[0]];
			functionStack.Clear();
			for (int i = 1; i < cnt; i++)
			{
				NodeInfo currentNode = infos[path.Nodes[i]];
				// 转移边。
				if (lastNode.OwnedFunction == currentNode.OwnedFunction)
				{
					continue;
				}
				if (currentNode.IsFirstNode)
				{
					// 调用边。
					functionStack.Push(lastNode.OwnedFunction);
				}
				else if (functionStack.Count > 0 && currentNode.OwnedFunction != functionStack.Pop())
				{
					// 返回边。
					return false;
				}
				lastNode = currentNode;
			}
			return true;
		}
	}
}
