using Cyjb.Compilers.C;

namespace OJWebService.TestSuits
{
	/// <summary>
	/// 语句级别代码插桩器。
	/// </summary>
	public class StatementInstrumenter : Instrumenter
	{
		/// <summary>
		/// 初始化 <see cref="StatementInstrumenter"/> 类的新实例。
		/// </summary>
		public StatementInstrumenter() { }
		/// <summary>
		/// 获取插桩级别。
		/// </summary>
		/// <value>表示插桩级别的字符串。</value>
		public override string InstrumentLevel { get { return "Statement"; } }
		/// <summary>
		/// 获取插桩级别的文本。
		/// </summary>
		/// <value>表示插桩级别的字符串的文本。</value>
		public override string InstrumentLevelText { get { return "语句级别插桩"; } }

		#region 访问语句

		/// <summary>
		/// 访问指定的 <see cref="BlockStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="BlockStatement"/> 节点。</param>
		protected override void VisitBlockStatement(BlockStatement node)
		{
			int i = 0;
			while (i < node.Count)
			{
				Statement stm = node[i];
				if (SyntaxFacts.IsJumpStatement(stm.Kind))
				{
					// 对于跳转语句，将行号输出在语句的上方。
					node.Insert(i, PrintIdStatement(stm));
					i++;
				}
				this.Visit(stm);
				i++;
				if (stm.Kind == SyntaxKind.ExpressionStatement ||
					SyntaxFacts.IsLabelStatement(stm.Kind) || stm is Declaration)
				{
					// 对于表达式语句和标签语句，将行号输出在语句的下方。
					node.Insert(i, PrintIdStatement(stm));
					i++;
				}
			}
		}
		/// <summary>
		/// 访问指定的 <see cref="DoStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="DoStatement"/> 节点。</param>
		protected override void VisitDoStatement(DoStatement node)
		{
			this.Visit(node.Condition);
			node.Condition = Syntax.ExpressionList(PrintIdExpression(node.Condition), node.Condition);
			this.Visit(node.Statements);
		}
		/// <summary>
		/// 访问指定的 <see cref="ForStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="ForStatement"/> 节点。</param>
		protected override void VisitForStatement(ForStatement node)
		{
			this.Visit(node.Initializer);
			this.Visit(node.Condition);
			this.Visit(node.Incrementor);
			node.Condition = Syntax.ExpressionList(PrintIdExpression(node.Condition), node.Condition);
			this.Visit(node.Statements);
		}
		/// <summary>
		/// 访问指定的 <see cref="IfStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="IfStatement"/> 节点。</param>
		protected override void VisitIfStatement(IfStatement node)
		{
			this.Visit(node.Condition);
			node.Condition = Syntax.ExpressionList(PrintIdExpression(node.Condition), node.Condition);
			this.Visit(node.TrueStatements);
			this.Visit(node.FalseStatements);
		}
		/// <summary>
		/// 访问指定的 <see cref="SwitchStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="SwitchStatement"/> 节点。</param>
		protected override void VisitSwitchStatement(SwitchStatement node)
		{
			this.Visit(node.Condition);
			node.Condition = Syntax.ExpressionList(PrintIdExpression(node.Condition), node.Condition);
			this.Visit(node.Statements);
		}
		/// <summary>
		/// 访问指定的 <see cref="WhileStatement"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="WhileStatement"/> 节点。</param>
		protected override void VisitWhileStatement(WhileStatement node)
		{
			this.Visit(node.Condition);
			node.Condition = Syntax.ExpressionList(PrintIdExpression(node.Condition), node.Condition);
			this.Visit(node.Statements);
		}


		#endregion // 访问语句

	}
}
