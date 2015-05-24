using System.Collections.Generic;
using System.Diagnostics;
using Cyjb.Compilers.C;

namespace OJWebService.TestSuits
{
	/// <summary>
	/// 代码插桩器。
	/// </summary>
	public abstract class Instrumenter : SyntaxWalker
	{
		/// <summary>
		/// 跟踪文件的变量名称。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected const string TraceVariableName = "traceFile";
		/// <summary>
		/// 节点信息集合。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private NodeInfoCollection infos;
		/// <summary>
		/// 语法树节点与其标识符的映射表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<SyntaxNode, int> syntaxNodeMapper = new Dictionary<SyntaxNode, int>();
		/// <summary>
		/// 当前正在遍历的函数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string currentFunction;
		/// <summary>
		/// 是否位于函数的首行。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool isFirst;
		/// <summary>
		/// 别初始化 <see cref="Instrumenter"/> 类的新实例。
		/// </summary>
		protected Instrumenter() { }
		/// <summary>
		/// 获取插桩级别。
		/// </summary>
		/// <value>表示插桩级别的字符串。</value>
		public abstract string InstrumentLevel { get; }
		/// <summary>
		/// 获取插桩级别的文本。
		/// </summary>
		/// <value>表示插桩级别的字符串的文本。</value>
		public abstract string InstrumentLevelText { get; }
		/// <summary>
		/// 获取跟踪文件的文件名。
		/// </summary>
		/// <value>跟踪文件的文件名。</value>
		public string TraceFileName { get; private set; }
		/// <summary>
		/// 获取节点信息集合。
		/// </summary>
		/// <value>节点信息的集合。</value>
		public NodeInfoCollection Infos
		{
			get { return this.infos; }
		}
		/// <summary>
		/// 对指定的语法树节点进行插桩。
		/// </summary>
		/// <param name="node">要插桩的语法树节点。</param>
		/// <param name="traceFileName">跟踪文件的文件名。</param>
		public void Instrument(SyntaxNode node, string traceFileName)
		{
			this.TraceFileName = traceFileName;
			this.infos = new NodeInfoCollection();
			this.Visit(node);
		}
		/// <summary>
		/// 访问指定的 <see cref="CompileUnit"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="CompileUnit"/> 节点。</param>
		protected override void VisitCompileUnit(CompileUnit node)
		{
			// 输出文件变量定义。
			node.Declarations.Insert(0, Syntax.Include("stdio.h"));
			node.Declarations.Insert(1, Syntax.VariableDeclaration(TraceVariableName,
				Syntax.TypeName("FILE").MakePointerType()));
			int cnt = node.Declarations.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (node.Declarations[i].Kind == SyntaxKind.FunctionDeclaration)
				{
					this.Visit(node.Declarations[i]);
				}
			}
			// 打开和关闭文件。
			for (int i = 0; i < cnt; i++)
			{
				FunctionDeclaration funcDecl = node.Declarations[i] as FunctionDeclaration;
				if (funcDecl == null)
				{
					continue;
				}
				HandleExit(funcDecl.Statements);
				if (funcDecl.NameText != "main") { continue; }
				funcDecl.Statements.Insert(0, Syntax.ExpressionStatement(
					Syntax.Binary(SyntaxKind.AssignExpression,
						Syntax.Variable(TraceVariableName),
						Syntax.Variable("fopen").Invoke(
							Syntax.Constant("\"" + TraceFileName + "\""),
							Syntax.Constant("\"w\"")))));
				funcDecl.Statements.Add(FileCloseStatement());
			}
		}

		#region 访问声明

		/// <summary>
		/// 访问指定的 <see cref="FunctionDeclaration"/> 节点。
		/// </summary>
		/// <param name="node">要访问的 <see cref="FunctionDeclaration"/> 节点。</param>
		protected override void VisitFunctionDeclaration(FunctionDeclaration node)
		{
			this.currentFunction = node.NameText;
			this.isFirst = true;
			base.VisitFunctionDeclaration(node);
			this.currentFunction = null;
		}

		#endregion // 访问声明

		#region 辅助方法

		/// <summary>
		/// 返回打印 ID 的语句。
		/// </summary>
		/// <param name="node">要打印 ID 的语法树节点。</param>
		/// <returns>打印 ID 的语句。</returns>
		protected Statement PrintIdStatement(SyntaxNode node)
		{
			return PrintIdExpression(node).ToStatement();
		}
		/// <summary>
		/// 返回打印 ID 的表达式。
		/// </summary>
		/// <param name="node">要打印 ID 的语法树节点。</param>
		/// <returns>打印 ID 的表达式。</returns>
		protected Expression PrintIdExpression(SyntaxNode node)
		{
			int id;
			if (!syntaxNodeMapper.TryGetValue(node, out id))
			{
				id = this.infos.Add(new NodeInfo(this.currentFunction, this.isFirst, node.Range.Start.Line));
				this.isFirst = false;
				syntaxNodeMapper.Add(node, id);
			}
			return Syntax.Variable("fprintf").Invoke(
				Syntax.Variable(TraceVariableName), Syntax.Constant("\"" + id + "\\n\""));
		}
		/// <summary>
		/// 返回关闭文件的语句。
		/// </summary>
		/// <returns>关闭文件的语句。</returns>
		private static Statement FileCloseStatement()
		{
			return Syntax.Variable("fclose").Invoke(Syntax.Variable(TraceVariableName)).ToStatement();
		}
		/// <summary>
		/// 处理 exit 语句。
		/// </summary>
		/// <param name="block">要处理的语句块。</param>
		private static void HandleExit(BlockStatement block)
		{
			if (block == null)
			{
				return;
			}
			int cnt = block.Count;
			for (int i = 0; i < cnt; i++)
			{
				BlockStatement subBlock = block[i] as BlockStatement;
				if (subBlock != null)
				{
					HandleExit(subBlock);
				}
				else if (IsExit(block[i]))
				{
					// 在 exit 语句之前插入关闭文件的语句。
					block.Insert(i, FileCloseStatement());
					i++;
				}
			}
		}
		/// <summary>
		/// 判断制定语句是否是退出语句。
		/// </summary>
		/// <param name="stm">要判断的语句。</param>
		/// <returns>如果是退出语句，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		private static bool IsExit(Statement stm)
		{
			ExpressionStatement exp = stm as ExpressionStatement;
			if (exp == null)
			{
				return false;
			}
			InvocationExpression invocation = exp.Expression as InvocationExpression;
			if (invocation == null)
			{
				return false;
			}
			VariableExpression variable = invocation.Target as VariableExpression;
			return variable != null && variable.NameText == "exit";
		}

		#endregion // 辅助方法

	}
}
