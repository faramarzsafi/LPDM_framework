using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace Metaheuristic
{
    public enum AlgorithmType
    {
        AllPermutations,
        Exchange,
        Insertion,
        Reversion,
        Enhanced,
        Enhanced_1,
        Fibonacci,
        Fibonacci_Straight,
        Fibonacci_Straight_2,
        Fibonacci_2Branch,
        Fibonacci_2Branch_Ex1,
        Fibonacci_2Branch_Ex2,
        Fibonacci_Straight_Double,
        Fibonacci_Straight_DoubleX,
        Fibonacci_Dynamic,
    }
    public class Population
    {
        public int counter;
        internal Dictionary<Permutation, int> History = new Dictionary<Permutation, int>();
        private Random random = new Random((int)DateTime.Now.Ticks);
        internal List<int> RetryCounts = new List<int>();
        private PopulationBestHistory bestHistory;

        const int MaxRetryCount = 1000000;
        private Permutation bestPermutation = null;
        public Permutation BestPermutation { get { return bestPermutation; } }
        private Permutation currentPermutation = null;
        public Permutation CurrentPermutation
        {
            get { return currentPermutation; }
            set
            {
                currentPermutation = value;
                if (bestPermutation == null)
                    bestPermutation = currentPermutation;
                else
                    if (bestPermutation.Cost > currentPermutation.Cost)
                    bestPermutation = currentPermutation;
            }
        }
        public int JobsCount { get; set; }
        public double TotalPermutationsCount { get; set; }
        public long TotaIterationCount { get; set; }
        public long TotaElapsedTime { get; set; }
        public long RealGeneratedPermutationsCount { get; set; }
        public long RedundantGeneratedPermutationsCount { get; set; }
        public long TotalGeneratedPermutationsCount { get; set; }
        public int LastPermutationGenerationRetryCount { get; set; }
        public long BestHistoryRefreshCount { get; set; }
        public long BestHistoryCheckCount { get; set; }
        public long BestHistoryExistenceCount { get; set; }
        public AlgorithmType AlgorithmType { get; set; }
        public int LiveTimes { get; set; }
        public double UngeneratedPermutationsCount { get { return TotalPermutationsCount - RealGeneratedPermutationsCount; } }
        public bool Done { get { return RealGeneratedPermutationsCount >= TotalPermutationsCount; } }
        public FinishType FinishType { get; set; }
        public List<Permutation> Permutations { get; set; }
        public Population(Permutation initialPermutation, int liveTimes, AlgorithmType algorithmType)
        {
            LiveTimes = liveTimes;
            CurrentPermutation = initialPermutation;
            JobsCount = Permutation.JobsCount;
            TotalPermutationsCount = Permutation.Factorial(JobsCount);
            TotaElapsedTime = 0;
            RealGeneratedPermutationsCount = 0;
            RedundantGeneratedPermutationsCount = 0;
            TotalGeneratedPermutationsCount = 0;
            FinishType = FinishType.NotYet;
            random = new Random((int)DateTime.Now.Ticks);
            History = new Dictionary<Permutation, int>();
            BestHistoryRefreshCount = 0;
            BestHistoryCheckCount = 0;
            BestHistoryExistenceCount = 0;
            AlgorithmType = algorithmType;
            bestHistory = new PopulationBestHistory(this, LiveTimes);
        }
        public Population(Population another)
        {
            LiveTimes = another.LiveTimes;
            CurrentPermutation = another.CurrentPermutation;
            JobsCount = another.JobsCount;
            TotalPermutationsCount = another.TotalPermutationsCount;
            TotaElapsedTime = another.TotaElapsedTime;
            RealGeneratedPermutationsCount = another.RealGeneratedPermutationsCount;
            RedundantGeneratedPermutationsCount = another.RedundantGeneratedPermutationsCount;
            TotalGeneratedPermutationsCount = another.TotalGeneratedPermutationsCount;
            FinishType = another.FinishType;
            random = null;
            History = null;
            BestHistoryRefreshCount = another.BestHistoryRefreshCount;
            BestHistoryCheckCount = another.BestHistoryCheckCount;
            BestHistoryExistenceCount = another.BestHistoryExistenceCount;
            AlgorithmType = another.AlgorithmType;
            bestHistory = null;
            counter = another.counter;
        }

        public void RefreshHistory()
        {
            bestHistory.Refresh();
        }
        public PopulationBestMember CheckHistory(Permutation permutation)
        {
            return bestHistory.Check(permutation);
        }
    }
    public class PopulationBestMember
    {
        public Permutation Permutation { get; set; }
        public int LiveTimes { get; set; }
        public PopulationBestMember(Permutation permutation, int tabuLiveTimes)
        {
            this.Permutation = permutation;
            LiveTimes = tabuLiveTimes;
        }
        public Permutation Check()
        {
            LiveTimes--;
            if (LiveTimes < 0)
                return this.Permutation;
            else
                return null;
        }
        public override string ToString()
        {
            return Permutation.ToString() + "*" + LiveTimes;
        }
    }
    public class PopulationBestHistory
    {
        private int liveTimes;
        private Population population;
        public PopulationBestHistory(Population population, int liveTimes = 1)
        {
            this.population = population;
            this.liveTimes = liveTimes;
        }
        private Dictionary<Permutation, PopulationBestMember> history = new Dictionary<Permutation, PopulationBestMember>();
        public void Refresh()
        {
            population.BestHistoryRefreshCount++;
            List<Permutation> removedList = new List<Permutation>();
            foreach (PopulationBestMember item in history.Values)
            {
                Permutation p = item.Check();
                if (p != null)
                    removedList.Add(p);
            }
            foreach (Permutation p in removedList)
                history.Remove(p);
        }
        public PopulationBestMember Check(Permutation permutation)
        {
            population.BestHistoryCheckCount++;
            if (history.ContainsKey(permutation))
            {
                population.BestHistoryExistenceCount++;
                return null;
            }
            PopulationBestMember member = new PopulationBestMember(permutation, liveTimes);
            history.Add(permutation, member);
            return member;

        }
    }
    public abstract class Metaheuristic
    {
        protected int liveTimes;
        public AlgorithmType AlgorithmType;
        public Metaheuristic(int liveTimes, AlgorithmType algorithmType)
        {
            this.liveTimes = liveTimes;
            this.AlgorithmType = algorithmType;
        }
        protected virtual List<Permutation> GeneratePopulation(Population r)
        {
            return null;
        }
        protected virtual void InitializePopulation(Population r)
        {

        }
        int Xxx = 0;
        protected virtual void SavePopulation(List<Permutation> permutations)
        {
            //Xxx++;
            //if (Xxx < 6 * 1321) return;
            string filename = string.Format(@"D:\Personal\Master\Results\{1}.csv", "time.ToString(\"yyMMddHHmmss\")", AlgorithmType.ToString());
            using (StreamWriter w = File.AppendText(filename))
            {
                BigInteger permutationRepresentation;
                foreach (Permutation permutation in permutations)
                {
                    w.WriteLine("{0},{1}", permutation.Representation, permutation.Cost);
                }
            }

        }
        protected virtual Permutation FindTheBestInPopulation(Population data)
        {
            return null;
        }

        protected virtual Permutation SelectNewMove(Population data)
        {
            return null;
        }

        protected virtual Permutation EvaluatePopulation(Population data)
        {
            return null;
        }

        protected void UpdateGeneratedPermutations(Population data)
        {
            foreach (Permutation permutation in data.Permutations)
            {
                if (data.History.ContainsKey(permutation))
                {
                    data.History[permutation]++;
                    data.RedundantGeneratedPermutationsCount++;
                }
                else
                {
                    data.History.Add(permutation, 1);
                    data.RealGeneratedPermutationsCount++;
                }
                data.TotalGeneratedPermutationsCount++;
            }
        }
        public virtual IEnumerable<Population> Evaluate(Permutation permutation, long yieldTime, long time, long iterationCount)
        {

            Population population = new Population(permutation, liveTimes, AlgorithmType);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long yieldTimeStep = yieldTime;
            InitializePopulation(population);
            while (true)
            {
                permutation = EvaluatePopulation(population);
                SavePopulation(population.Permutations);
                if (permutation == null)
                {
                    population.FinishType = FinishType.NoNewPermutation;
                    break;
                }
                population.TotaIterationCount++;
                population.TotaElapsedTime = sw.ElapsedMilliseconds;
                if (population.Done)
                {
                    population.FinishType = FinishType.Done;
                    break;
                }
                if (time != 0 && population.TotaElapsedTime >= time * 1000)
                {
                    population.FinishType = FinishType.Time;
                    break;
                }
                if (iterationCount != 0 && population.TotaIterationCount >= iterationCount)
                {
                    population.FinishType = FinishType.Iteration;
                    break;
                }
                if (population.TotaElapsedTime > yieldTimeStep * 1000)
                {
                    yield return population;
                    yieldTimeStep += yieldTime;
                }
            }
            yield return population;
        }
        public static void RunInlineHeader()
        {
            Console.WriteLine("Counter,Search type,n,Permutations#,Redundant#,Total#,Iteration#," +
                              "History check#,History existence#,Elapsed time," +
                              "Initial cost,The best cost");
        }
        static object lockObject = new object();
        static List<Population> Results = new List<Population>();
        static int counter = 0;
        public static Population RunInline(Metaheuristic metaheuristic, Permutation permutation, long yieldTime, long time, long iterationCount)
        {
            Population lastData = null;
            Permutation previousPermutation = permutation;
            foreach (Population data in metaheuristic.Evaluate(permutation, yieldTime, time, iterationCount))
            {
                lock (lockObject)
                {
                    lastData = data;
                    string status = "";
                    switch (data.FinishType)
                    {
                        case FinishType.Done:
                            status = "FINISHED: Done";
                            break;
                        case FinishType.Time:
                            status = "FINISHED: Time Over";
                            break;
                        case FinishType.Iteration:
                            status = "FINISHED: Number of Iterations Exceeded";
                            break;
                        case FinishType.NoNewPermutation:
                            status = "FINISHED: No new permutation";
                            break;
                    }
                    counter++;
                    data.counter = counter;
                    Results.Add(new Population(data));

                    Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                                      data.counter, data.AlgorithmType, data.JobsCount, data.RealGeneratedPermutationsCount, data.RedundantGeneratedPermutationsCount,
                                      data.RealGeneratedPermutationsCount + data.RedundantGeneratedPermutationsCount,
                                      data.TotaIterationCount, data.BestHistoryCheckCount, data.BestHistoryExistenceCount,
                                      data.TotaElapsedTime / 1000, previousPermutation.Cost, data.BestPermutation.Cost, status);
                    previousPermutation = data.BestPermutation;
                    Console.Out.Flush();
                }
            }
            return lastData;

        }
        public static void SaveData(AlgorithmType type, DateTime time, int jobsCount, int tabuLiveTimes, long yieldTime, long maxElapsedTime)
        {
            using (StreamWriter writetext = new StreamWriter(string.Format(@"D:\Personal\Master\Results\{0}-{1}-{2}-{3}-{4}-{5}.csv", time.ToString("yyMMddHHmmss"), type, tabuLiveTimes, jobsCount, yieldTime, maxElapsedTime)))
            {
                writetext.WriteLine("Counter,Search type,n,Effective,Redundant,Total,Iteration#," +
                                    "History check#,History existence#,Elapsed time,The best cost");
                int row = 1;
                for (int i = 0; i < Results.Count; i++)
                {
                    Population data = Results[i];
                    if (data.AlgorithmType == type)
                        writetext.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                          row++, data.AlgorithmType, data.JobsCount, data.RealGeneratedPermutationsCount, data.RedundantGeneratedPermutationsCount,
                                          data.RealGeneratedPermutationsCount + data.RedundantGeneratedPermutationsCount,
                                          data.TotaIterationCount, data.BestHistoryCheckCount, data.BestHistoryExistenceCount,
                                          data.TotaElapsedTime / 1000, data.BestPermutation.Cost);
                }
            }

        }
    }
}
