using System.Collections.Generic;
using System.Text;
using Cyjb.Compilers.C;

namespace OJWebService
{
	/// <summary>
	/// 代码插装器。
	/// </summary>
	internal sealed class Graphviz : SyntaxWalker
	{
		private int id = 1;
		StringBuilder text = new StringBuilder();
		Dictionary<SyntaxNode, int> nodeIdx = new Dictionary<SyntaxNode, int>();
		public string GetText()
		{
			text.Insert(0, "digraph G {\n");
			text.AppendLine("}");
			return text.ToString();
		}
		public override void Visit(SyntaxNode node)
		{
			base.Visit(node);
		}
		protected override void DefaultVisit(SyntaxNode node)
		{
			int idx = id++;
			nodeIdx.Add(node, idx);
			int cnt = node.ChildNodes.Count;
			for (int i = 0; i < cnt; i++)
			{
				this.Visit(node.ChildNodes[i]);
				int chidId = nodeIdx[node.ChildNodes[i]];
				text.Append(idx);
				text.Append(" -> ");
				text.Append(chidId);
				text.AppendLine(";");
			}
			text.Append(idx);
			text.Append(" [");
			if (cnt > 0)
			{
				text.Append("shape=box,");
			}
			text.Append("label=\"");
			text.Append(node.Kind.ToString());
			if (node is IdentifierName || node is TypeName)
			{
				text.Append(" ");
				text.Append(node.ToString());
			}
			text.AppendLine("\"];");
		}
	}
}
