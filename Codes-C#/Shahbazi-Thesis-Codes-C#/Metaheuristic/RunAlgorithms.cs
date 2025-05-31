using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Metaheuristic
{
    partial class Program
    {
        static public string SHA1(string txt)
        {
            Byte[] sKeyBytes = ASCIIEncoding.ASCII.GetBytes(txt);
            SHA1 sha = new SHA1CryptoServiceProvider();
            Byte[] hashKeys = sha.ComputeHash(sKeyBytes);
            return ASCIIEncoding.ASCII.GetString(hashKeys);
        }
        static int maxElapsedTime = 10;
        static int tabuLiveTimes = 256;
        static int jobsCount = 8;
        static long yieldTime = 10;
        static int maxIterationCount = 100000;
        static int machinesCount = 10;
        static Permutation jobs;


        public static void RunAlgorithms(string[] args)
        {
            Permutation.JobsCount = jobsCount;
            TabuSearch.RunInlineHeader();
            jobs = Permutation.ReadJobs(@"D:\Personal\Master\THESIS\Tests\TaillardBenchmarks\jobs-01.txt", jobsCount, machinesCount);
            Console.WriteLine(jobs.Representation);
            if (jobs != null)
            {
                //Thread t1 = new Thread(() => { TabuSearch.RunInline(new TS_Exchange(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //Thread t2 = new Thread(() => { TabuSearch.RunInline(new TS_Insertion(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //Thread t3 = new Thread(() => { TabuSearch.RunInline(new TS_Enhanced(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //Thread t4 = new Thread(() => { TabuSearch.RunInline(new TS_Enhanced_1(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                Thread t5 = new Thread(() => { TabuSearch.RunInline(new Fibonacci_Straight_Double(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //Thread t6 = new Thread(() => { Metaheuristic.RunInline(new AllPermutations(5000), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //Thread t7 = new Thread(() => { TabuSearch.RunInline(new Fibonacci_Straight(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                Thread t8 = new Thread(() => { TabuSearch.RunInline(new Fibonacci_Dynamic(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount); });
                //t1.Start();
                //t2.Start();
                //t3.Start();
                //t4.Start();
                t5.Start();
                //t6.Start();
                //t7.Start();
                t8.Start();
                //t1.Join();
                //t2.Join();
                //t3.Join();
                //t4.Join();
                t5.Join();
                //t6.Join();
                //t7.Join();
                t8.Join();
                DateTime now = DateTime.Now;
                //TabuSearch.SaveData(AlgorithmType.Exchange, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                //TabuSearch.SaveData(AlgorithmType.Insertion, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                //TabuSearch.SaveData(AlgorithmType.Enhanced, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                //TabuSearch.SaveData(AlgorithmType.Enhanced_1, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                TabuSearch.SaveData(AlgorithmType.Fibonacci_Straight_Double, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                //TabuSearch.SaveData(AlgorithmType.AllPermutations, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                //TabuSearch.SaveData(AlgorithmType.Fibonacci_Straight, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);
                TabuSearch.SaveData(AlgorithmType.Fibonacci_Dynamic, now, jobsCount, tabuLiveTimes, yieldTime, maxElapsedTime);

            }
            else
                Console.WriteLine("Jobs creation error!");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
        static void Test1()
        {
            TabuSearch.RunInline(new TS_Exchange(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount);
        }
        static void Test2()
        {
            TabuSearch.RunInline(new TS_Insertion(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount);
        }
        static void Test3()
        {
            TabuSearch.RunInline(new TS_Enhanced(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount);
        }
        static void Test4()
        {
            TabuSearch.RunInline(new TS_Enhanced_1(tabuLiveTimes), jobs, yieldTime, maxElapsedTime, maxIterationCount);
        }
    }
}
