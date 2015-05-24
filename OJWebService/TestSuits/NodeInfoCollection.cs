using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Cyjb;

namespace OJWebService.TestSuits
{
	/// <summary>
	/// 程序跟踪信息节点的信息集合。
	/// </summary>
	public sealed class NodeInfoCollection
	{
		/// <summary>
		/// 节点信息列表。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<NodeInfo> infos = new List<NodeInfo>();
		/// <summary>
		/// 可执行语句的行数。
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int executableStatementCount = -1;
		/// <summary>
		/// 初始化 <see cref="NodeInfoCollection"/> 类的新实例。
		/// </summary>
		public NodeInfoCollection() { }
		/// <summary>
		/// 使用指定的节点信息文件初始化 <see cref="NodeInfoCollection"/> 类的新实例。
		/// </summary>
		/// <param name="fileName">保存节点信息的文件。</param>
		public NodeInfoCollection(string fileName)
		{
			ExceptionHelper.CheckArgumentNull(fileName, "fileName");
			using (StreamReader reader = new StreamReader(fileName))
			{
				while (true)
				{
					string line = reader.ReadLine();
					if (line == null)
					{
						break;
					}
					string[] strs = line.Split();
					this.infos.Add(new NodeInfo(strs[0], bool.Parse(strs[1]), int.Parse(strs[2])));
				}
			}
		}
		/// <summary>
		/// 获取指定节点的信息。
		/// </summary>
		/// <param name="id">要获取信息的节点标识符。</param>
		/// <returns>指定节点的信息。</returns>
		public NodeInfo this[int id]
		{
			get { return this.infos[id]; }
		}
		/// <summary>
		/// 获取包含的节点的数量。
		/// </summary>
		/// <value>包含的节点的数量。</value>
		public int Count
		{
			get { return this.infos.Count; }
		}
		/// <summary>
		/// 获取可执行语句的行数。
		/// </summary>
		/// <value>可执行语句的行数。</value>
		public int ExecutableStatementCount
		{
			get
			{
				if (this.executableStatementCount == -1)
				{
					this.executableStatementCount = this.infos.Select(info => info.Line).Distinct().Count();
				}
				return this.executableStatementCount;
			}
		}
		/// <summary>
		/// 添加一个新的节点信息。
		/// </summary>
		/// <param name="info">要添加的节点信息。</param>
		/// <returns>新节点的标识符。</returns>
		public int Add(NodeInfo info)
		{
			int idx = this.infos.Count;
			this.infos.Add(info);
			return idx;
		}
		/// <summary>
		/// 返回与相应行号对应的节点标识符列表。
		/// </summary>
		/// <param name="line">要获取节点标识符的行。</param>
		/// <returns>与相应行号对应的节点标识符列表。</returns>
		public int[] GetIds(int line)
		{
			List<int> ids = new List<int>();
			int cnt = this.infos.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (this.infos[i].Line == line)
				{
					ids.Add(i);
				}
			}
			return ids.ToArray();
		}
		/// <summary>
		/// 将节点信息写入到指定文件中。
		/// </summary>
		/// <param name="fileName">要写入节点信息的文件。</param>
		public void Save(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName))
			{
				int cnt = this.infos.Count;
				for (int i = 0; i < cnt; i++)
				{
					NodeInfo info = this.infos[i];
					writer.WriteLine("{0} {1} {2}", info.OwnedFunction, info.IsFirstNode, info.Line);
				}
			}
		}
	}
}
