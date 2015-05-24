using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cyjb;
using Cyjb.Compilers.C;
using Cyjb.IO;
using OJWebService.Graphs;
using OJWebService.TestSuits;

namespace OJWebService
{
	/// <summary>
	/// 主函数。
	/// </summary>
    public partial class MainFunction
    {
        /// <summary>
        /// 测试集。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TestSuit testSuit;

        public MainFunction()
        {
            if (this.testSuit == null)
            {
                this.testSuit = TestSuit.GetTestSuit(testsuitPath);
            }
        }

        public void run()
        {
            // 构造图
            this.buildGraph();
            // 挖掘
            this.IGMining();
        }

        #region 错误定位

        /// <summary>
        /// 测试路径。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string testsuitPath = @"\source\";
        /// <summary>
        /// 节点信息集合。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NodeInfoCollection nodeInfos;
        /// <summary>
        /// 计时器。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Stopwatch timeWatch = new Stopwatch();
        /// <summary>
        /// 软件行为图集合。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GraphCollection graphs;
        /// <summary>
        /// 挖掘结果。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Result[] results;
        /// <summary>
        /// Tarantula 的计算结果。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double[] tarantulaList;
        /// <summary>
        /// 信息增益的挖掘结果。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Result[] informationGainResults;
        /// <summary>
        /// 构造行为图。
        /// </summary>
        private void buildGraph()
        {
            // 读取程序跟踪信息。
            TraceSet traces = this.testSuit.GetTrace(new StatementInstrumenter());
            // 读取节点信息。
            this.nodeInfos = new NodeInfoCollection(this.testSuit.GetData());
            // 构造软件行为图。
            timeWatch.Restart();
            this.graphs = new GraphCollection(traces, this.nodeInfos);
            timeWatch.Stop();
            // 统计 tarantulaList 和 ochiaiList 的值。
            Statistic();
            this.informationGainResults = null;
        }
        /// <summary>
        /// 挖掘信息增益错误签名。
        /// </summary>
        private void IGMining()
        {
            if (this.informationGainResults == null)
            {
                this.Mining(new InformationGain());
                this.informationGainResults = this.results;
            }
            this.results = this.informationGainResults;
            ShowResult();
        }
        /// <summary>
        /// 使用指定的得分函数进行错误签名挖掘。
        /// </summary>
        /// <param name="score">使用的得分函数。</param>
        private void Mining(ScoreFunction score)
        {
            // k 不确定 britain
            int k = 100;
            GraphMining mining = new GraphMining(this.graphs, new PathChecker(this.nodeInfos), score, k);
            timeWatch.Restart();
            this.results = mining.Mining();
            timeWatch.Stop();
        }
        /// <summary>
        /// 显示挖掘结果。
        /// </summary>
        private void ShowResult()
        {
            String[] codes = File.ReadAllLines(this.testSuit.GetFormattedFile());
            StringBuilder text = new StringBuilder();
            StreamWriter sw = new StreamWriter(@"\source\result.txt", false);
            for (int i = 0; i < this.results.Length; i++)
            {
                text.Clear();
                Result result = this.results[i];
                text.Append(i + 1);
                text.Append("  ");
                text.Append(result.Score.ToString("0.000"));
                text.Append("  ");
                int cnt = result.Path.Nodes.Count;
                for (int j = 0; j < cnt; j++)
                {
                    if (j > 0)
                    {
                        text.Append("    ->    ");
                    }
                    text.Append(codes[this.nodeInfos[result.Path.Nodes[j]].Line - 1].Trim());
                }
                result.Text = text.ToString();

                sw.WriteLine(text);

            }
            sw.Close();
        }
        /// <summary>
        /// 统计每个节点的 tarantula 得分。
        /// </summary>
        private void Statistic()
        {
            int nodeCnt = this.nodeInfos.Count;
            int[] successes = new int[nodeCnt];
            int[] fails = new int[nodeCnt];
            int allSuccessed = this.graphs.Successes.Count;
            // 计算每个节点在成功的执行中出现的次数。
            for (int i = 0; i < allSuccessed; i++)
            {
                foreach (Vector v in this.graphs.Successes[i].Vectors)
                {
                    successes[v.Id] += v.ExecuteCount;
                }
            }
            int allFailed = this.graphs.Fails.Count;
            // 计算每个节点在失败的执行中出现的次数。
            for (int i = 0; i < allFailed; i++)
            {
                foreach (Vector v in this.graphs.Fails[i].Vectors)
                {
                    fails[v.Id] += v.ExecuteCount;
                }
            }
            this.tarantulaList = new double[nodeCnt];
            for (int i = 0; i < nodeCnt; i++)
            {
                tarantulaList[i] = ((double)fails[i] / allFailed) /
                    ((double)successes[i] / allSuccessed + (double)fails[i] / allFailed);
            }
            this.OutTarantulaRankFile();
        }

        /// <summary>
        /// 将tarantulaList 输出到文件中
        /// </summary>
        private void OutTarantulaRankFile()
        {
            // 得到每行的最大的分。
            Tuple<double, int>[] ranks = tarantulaList
                .Select((value, index) => new Tuple<double, int>(value, this.nodeInfos[index].Line))
                .Where(tuple => tuple.Item1 >= 0)
                .OrderByDescending(tuple => tuple.Item1).ToArray();
            // 将排序结果输出到文件里           
            StreamWriter sw = new StreamWriter(@"\source\TarantulaScoreRank.txt", false);
            int len = ranks.Length;
            sw.WriteLine("序号\t行号\t\t权重");
            for (int i = 0; i < len; i++)
            {
                sw.WriteLine("{0}\t{1}\t{2}", i + 1, ranks[i].Item2, ranks[i].Item1);
            }
            sw.Close();
        }

        #endregion // 错误定位
    }
}
