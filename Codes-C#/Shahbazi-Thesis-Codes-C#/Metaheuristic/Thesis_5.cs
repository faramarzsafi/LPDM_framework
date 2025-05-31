using Metaheuristic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Metaheuristic.Diversity_Old;

namespace Thesis_5
{
    internal class Data
    {
        public string Name;
        public int Modality;
        public Permutation[] Optimas;
        public Permutation[] Permutations;
        public int DiversityRatio;
        public double MaxDistance;
        public double SpaceMaxDistance;
        public int JobsCount;
        public int RepeatTimes;
        public int ReductionRate;
        public Permutation.DistanceMeasureType DistanceType;
        public long[] ClustersCounts;
        public BigInteger[] ClustersSizes;
        public int PopulationCount;
        public void ComputeClustersCounts()
        {
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClustersCounts = new long[ClustersSizes.Length];
            for (int i = 0; i < ClustersSizes.Length; i++)
            {
                ClustersCounts[i] = (long)(Max / ClustersSizes[i]);
                if (Max % ClustersSizes[i] != 0)
                    ClustersCounts[i]++;
            }
        }
    }
    internal class Benchmark
    {
        Data data;
        public Benchmark(ref Data data)
        {
            Permutation.JobsCount = data.JobsCount;
            switch (data.DistanceType)
            {
                case Permutation.DistanceMeasureType.Linear:
                    data.SpaceMaxDistance = (double)Factoradic.Factorial[data.JobsCount]; break;
                case Permutation.DistanceMeasureType.L1norm:
                    data.SpaceMaxDistance = ((double)data.JobsCount * data.JobsCount); break;
                case Permutation.DistanceMeasureType.L2norm:
                    data.SpaceMaxDistance = Math.Sqrt(data.JobsCount * data.JobsCount * data.JobsCount); break;
            }

        }
        //public void PopulationToPermutation()
        //{
        //    Permutations = new Permutation[Population.Length];
        //    BigInteger b;
        //    for (int i = 0; i < Population.Length; i++)
        //    {
        //        b = Population[i];
        //        Permutations[i] = b.ToPermutation();
        //    }
        //}
        //public void PermutationToPopulation()
        //{
        //    Population = new BigInteger[Permutations.Length];
        //    for (int i = 0; i < Population.Length; i++)
        //    {
        //        Population[i] = Permutations[i].Representation;
        //    }
        //}
        public void GeneratePopulation(ref Data data)
        {
            GenerateOptimas(ref data);
            populationGenerateSingle(ref data);
        }
        private Random random = new Random();
        public Permutation GenerateRandomPermutation(Data data)
        {
            int[] jobs = new int[data.JobsCount];
            for (int i = 0; i < data.JobsCount; i++)
            {
                int x;
                while (true)
                {
                    bool duplicate = false;
                    x = random.Next(data.JobsCount);
                    for (int j = 0; j < i; j++)
                        if (x == jobs[j])
                        {
                            duplicate = true;
                            break;
                        }
                    if (!duplicate) break;
                }
                jobs[i] = x;
            }
            Permutation permutation = new Permutation(jobs);
            return permutation;
        }
        public Permutation[] GenerateOptimas(ref Data data)
        {
            Permutation[] permutations = new Permutation[data.Modality];
            for (int i = 0; i < data.Modality; i++)
                while (true)
                {
                    Permutation permutation = GenerateRandomPermutation(data);
                    bool alreadyExists = false;
                    for (int j = 0; j < i; j++)
                        if (permutation == permutations[j])
                        {
                            alreadyExists = true;
                            break;
                        }
                    if (alreadyExists) continue;
                    permutations[i] = permutation;
                    break;
                }
            data.Optimas = permutations;
            return permutations;
        }
        public double MaxRatioDistance(ref Data data)
        {
            return (data.SpaceMaxDistance * data.DiversityRatio / 100);
        }
        public Permutation[] populationGenerateSingle(ref Data data)
        {
            Permutation[] permutations = new Permutation[data.PopulationCount * data.Optimas.Length];
            data.MaxDistance = MaxRatioDistance(ref data);
            for (int i = 0; i < data.Optimas.Length; i++)
                for (int j = 0; j < data.PopulationCount; j++)
                {
                    while (true)
                    {
                        Permutation permutation = GenerateRandomPermutation(data);
                        double distance;
                        distance = permutation.RealDistanceTo(data.DistanceType, data.Optimas[i]);
                        if (distance < data.MaxDistance)
                        {
                            permutations[i * data.PopulationCount + j] = permutation;
                            break;
                        }
                    }
                }
            data.Permutations = permutations;
            return permutations;
        }
        public struct Diversities
        {
            public long[] OurMethodClustersCount;
            public double[] OurMethod;
            public double Osuna_Enciso_et_al;
            public double Cheng;
            public double Salleh_et_al;
        }
        public struct Population
        {
            public Permutation[] Permutations;
            public Diversities Diversities;
        }
        public struct DiversityResult
        {
            public Population[] Populations;
            public Permutation[] Optimas;
            public int DiversityRatio;
            public Diversities Diversities;
            public double MaxDistance;
            public int PopulationCount;
            public int Modality;
            public string Name;
            public BigInteger[] ClustersSizes;
            public long[] ClustersCounts;


        }
        public DiversityResult ComputeDiversity(ref Data data)
        {
            DiversityResult result;
            result.Optimas = data.Optimas;
            result.DiversityRatio = data.DiversityRatio;
            result.Populations = new Population[data.RepeatTimes];
            result.Diversities = new Diversities();
            result.Diversities.OurMethod = new double[data.ClustersSizes.Length];
            result.PopulationCount = data.PopulationCount;
            result.Modality = data.Modality;
            result.Name = data.Name;
            result.ClustersCounts = data.ClustersCounts;
            result.ClustersSizes = data.ClustersSizes;

            for (int i = 0; i < data.RepeatTimes; i++)
            {
                populationGenerateSingle(ref data);
                result.Populations[i].Permutations = data.Permutations;
                result.Populations[i].Diversities.OurMethod = new double[data.ClustersSizes.Length];
                for (int j = 0; j < data.ClustersSizes.Length; j++)
                {
                    Result r = OurMethod_Old(data.Permutations, data.ClustersSizes[j]);

                    result.Populations[i].Diversities.OurMethod[j] = r.DivNorm;
                    result.Diversities.OurMethod[j] += result.Populations[i].Diversities.OurMethod[j];

                }
                result.Populations[i].Diversities.Osuna_Enciso_et_al = Osuna_Enciso_et_al(data.Permutations);
                result.Populations[i].Diversities.Cheng = Cheng(data.Permutations);
                result.Populations[i].Diversities.Salleh_et_al = Salleh_et_al(data.Permutations);

                result.Diversities.Osuna_Enciso_et_al += result.Populations[i].Diversities.Osuna_Enciso_et_al;
                result.Diversities.Cheng += result.Populations[i].Diversities.Cheng;
                result.Diversities.Salleh_et_al += result.Populations[i].Diversities.Salleh_et_al;
            }
            for (int j = 0; j < data.ClustersSizes.Length; j++)
            {
                result.Diversities.OurMethod[j] /= data.RepeatTimes;
            }
            result.Diversities.Osuna_Enciso_et_al /= data.RepeatTimes;
            result.Diversities.Cheng /= data.RepeatTimes;
            result.Diversities.Salleh_et_al /= data.RepeatTimes;
            result.MaxDistance = data.MaxDistance;
            return result;
        }
        static string ResultBasePath = "D:\\Personal\\Master\\THESIS\\Tests";
        static string ResultPath = "";
        public static bool InitResultPath(Data data)
        {
            if (!System.IO.Directory.Exists(ResultBasePath))
                return false;
            System.IO.Directory.CreateDirectory(ResultBasePath + "\\" + data.Name);
            string path = string.Format("{0}\\{1}\\{2}", ResultBasePath, data.Name, DateTime.Now.ToString("yyMMddHHmmss.fff"));
            System.IO.Directory.CreateDirectory(path);
            ResultPath = path;
            return true;
        }
        public static void Print(DiversityResult result)
        {
            String filename = String.Format("{0}\\{1}-{2} modes-{3}%.csv", ResultPath, result.Name, result.Modality, result.DiversityRatio);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write(" ");
                for (int i = 0; i < result.Optimas.Length; i++)
                    writetext.Write(",Optima {0}, Optima [{1}]", i + 1, i + 1);
                writetext.WriteLine();
                writetext.Write(" ");
                for (int i = 0; i < result.Optimas.Length; i++)
                    writetext.Write(",{0},[{1}]", result.Optimas[i].Representation, result.Optimas[i].ToArrayString(false, " "));
                writetext.WriteLine("\n------------------------------------------------");
                writetext.Write("#");
                for (int i = 0; i < result.Optimas.Length; i++)
                    writetext.Write(",Optima {0}, Optima [{1}]", i + 1, i + 1);
                writetext.WriteLine();
                for (int i = 0; i < result.PopulationCount; i++)
                {
                    writetext.Write(i+1);
                    for (int j = 0; j < result.Optimas.Length; j++)
                    {
                        Permutation p = result.Populations[0].Permutations[i + j * result.PopulationCount];
                        writetext.Write(",{0},[{1}]", p.Representation, p.ToArrayString(false, " "));
                    }
                    writetext.WriteLine();
                }
            }
        }
        public static void Print(List<DiversityResult> diversityResults, int[] diversityRatios)
        {
            string Name = "";
            Permutation[] Optimas = null;
            BigInteger[] ClustersSizes = null;
            long[] ClustersCounts = null;

            for (int i = 0; i < diversityResults.Count; i++)
            {
                DiversityResult result = diversityResults[i];
                if (diversityRatios.Contains(result.DiversityRatio))
                    Print(result);
                if (i == 0)
                {
                    Name = result.Name;
                    Optimas = result.Optimas;
                    ClustersSizes = result.ClustersSizes;
                    ClustersCounts = result.ClustersCounts;
                }
            }

            String filename = String.Format("{0}\\{1}.csv", ResultPath, Name);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write("Optimas,");
                for (int i = 0; i < Optimas.Length; i++)
                {
                    if (i != 0)
                        writetext.Write(",");
                    writetext.Write(" {0}", Optimas[i].Representation);
                }
                writetext.WriteLine();

                writetext.Write("Max distance, Ratio,Osuna Enciso et al,Cheng,Salleh et al");
                for (int i = 0; i < ClustersSizes.Length; i++)
                    writetext.Write(",Ours {0}", ClustersCounts[i]);
                writetext.WriteLine();

                for (int i = 0; i < diversityResults.Count; i++)
                {
                    DiversityResult result = diversityResults[i];
                    writetext.Write("{0:0.000},{1:0.000},{2:0.000},{3:0.000},{4:0.000}",
                        result.MaxDistance, result.DiversityRatio,
                        result.Diversities.Osuna_Enciso_et_al,
                        result.Diversities.Cheng,
                        result.Diversities.Salleh_et_al);
                    for (int j = 0; j < result.ClustersSizes.Length; j++)
                        writetext.Write(",{0:0.000}", result.Diversities.OurMethod[j]);
                    writetext.WriteLine();
                }
            }
        }
        public static void Run()
        {
            Data data = new Data();
            data.JobsCount = 6;
            Permutation.JobsCount = data.JobsCount;
            data.DiversityRatio = 100;
            data.ReductionRate = 1;
            data.DistanceType = Permutation.DistanceMeasureType.Linear;
            data.Name = "XXXD";
            data.Modality = 10;
            data.PopulationCount = 100;
            data.ClustersSizes = new BigInteger[] { 5, 20, 40, 60, 80 };
            data.ComputeClustersCounts();
            data.RepeatTimes = 50;

            if (!InitResultPath(data))
            {
                Console.WriteLine("Can not initialize result path.");
                return;
            }
            List<DiversityResult> diversityResults = new List<DiversityResult>();
            Benchmark benchmark = new Benchmark(ref data);
            benchmark.GenerateOptimas(ref data);
            Console.Write("Optimas:");
            for (int i = 0; i < data.Optimas.Length; i++)
            {
                if (i != 0)
                    Console.Write(",");
                Console.Write(" {0}", data.Optimas[i].Representation);
            }
            Console.WriteLine();

            Console.WriteLine("Starting...");
            for (; data.DiversityRatio > 0; data.DiversityRatio -= data.ReductionRate)
            {
                DiversityResult result = benchmark.ComputeDiversity(ref data);
                diversityResults.Add(result);
                Console.Write("\r{0}%", 101-data.DiversityRatio);
            }
            Console.WriteLine("\rFinished.");
            Print(diversityResults , new int[] { 100, 70, 40, 2});
        }

    }
}
