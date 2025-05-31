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

namespace Thesis_8
{
    internal class Data
    {
        public string Name;
        public int Modality;
        public Permutation[] Optimas;
        public Permutation[] Permutations;
        public int DiversityRatio;
        public double MaxDistance;
        public BigInteger SpaceMax;
        public double SpaceMaxDistance;
        public int JobsCount;
        public int RepeatTimes;
        public int ReductionRate;
        public Permutation.DistanceMeasureType DistanceType;
        public long[] ClustersCounts;
        public BigInteger[] ClustersSizes;
        public int PopulationCount;
        public double OurMethodAlpha;
        public Data()
        {
            OurMethodAlpha = 1;
        }
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
                    data.SpaceMax = Factoradic.Factorial[data.JobsCount];
                    data.SpaceMaxDistance = (double)data.SpaceMax; break;
                case Permutation.DistanceMeasureType.L1norm:
                    data.SpaceMaxDistance = ((double)data.JobsCount * data.JobsCount); break;
                case Permutation.DistanceMeasureType.L2norm:
                    data.SpaceMaxDistance = Math.Sqrt(data.JobsCount * data.JobsCount * data.JobsCount); break;
            }

        }
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
            {
                Permutation optima = data.Optimas[i];
                double optimaRightMargin = (double)(data.SpaceMax - optima.Representation) * data.DiversityRatio / 100;
                double optimaLeftMargin = (double)(optima.Representation) * data.DiversityRatio / 100;
                for (int j = 0; j < data.PopulationCount; j++)
                {
                    while (true)
                    {
                        Permutation permutation = GenerateRandomPermutation(data);
                        double distance;
                        distance = permutation.RealDistanceTo(data.DistanceType, optima, true);
                        if (distance == 0 || distance > 0 && distance < optimaRightMargin || distance < 0 && -distance < optimaLeftMargin)
                        {
                            permutations[i * data.PopulationCount + j] = permutation;
                            break;
                        }
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
            public double Zhao_et_al;
            public double Edge_Distances_Div;
            public double Wineberg_Oppacher;
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
                    Result r = OurMethod(data.Permutations, data.ClustersSizes[j], 1);

                    result.Populations[i].Diversities.OurMethod[j] = r.DivNorm;
                    result.Diversities.OurMethod[j] += result.Populations[i].Diversities.OurMethod[j];

                }
                result.Populations[i].Diversities.Osuna_Enciso_et_al = Osuna_Enciso_et_al(data.Permutations);
                result.Diversities.Osuna_Enciso_et_al += result.Populations[i].Diversities.Osuna_Enciso_et_al;

                result.Populations[i].Diversities.Cheng = Cheng(data.Permutations);
                result.Diversities.Cheng += result.Populations[i].Diversities.Cheng;

                result.Populations[i].Diversities.Salleh_et_al = Salleh_et_al(data.Permutations);
                result.Diversities.Salleh_et_al += result.Populations[i].Diversities.Salleh_et_al;

                result.Populations[i].Diversities.Zhao_et_al = Zhao_et_al(data.Permutations);
                result.Diversities.Zhao_et_al += result.Populations[i].Diversities.Zhao_et_al;

                result.Populations[i].Diversities.Edge_Distances_Div = Edge_Distances_Div(data.Permutations);
                result.Diversities.Edge_Distances_Div += result.Populations[i].Diversities.Edge_Distances_Div;

                result.Populations[i].Diversities.Wineberg_Oppacher = Wineberg_Oppacher(data.Permutations);
                result.Diversities.Wineberg_Oppacher += result.Populations[i].Diversities.Wineberg_Oppacher;
            }
            for (int j = 0; j < data.ClustersSizes.Length; j++)
            {
                result.Diversities.OurMethod[j] /= data.RepeatTimes;
            }
            result.Diversities.Osuna_Enciso_et_al /= data.RepeatTimes;
            result.Diversities.Cheng /= data.RepeatTimes;
            result.Diversities.Salleh_et_al /= data.RepeatTimes;
            result.Diversities.Zhao_et_al /= data.RepeatTimes;
            result.Diversities.Edge_Distances_Div /= data.RepeatTimes;
            result.Diversities.Wineberg_Oppacher /= data.RepeatTimes;
            result.MaxDistance = data.MaxDistance;
            return result;
        }
        static string ResultBasePath = "D:\\Personal\\Master\\THESIS\\Tests";
        static string ResultPath = "";
        static string ResultFileName = "";
        public static bool InitResultPath(Data data)
        {
            if (!System.IO.Directory.Exists(ResultBasePath))
                return false;
            
            string rootPath = string.Format("{0}\\{1}-{2}!", ResultBasePath, data.Name, data.JobsCount);
            System.IO.Directory.CreateDirectory(rootPath);
            ResultFileName = string.Format("{0}, {1}-{2}!, Mod {3}, PpO {4}, α {5}",
                DateTime.Now.ToString("yyMMddHHmmss.fff"), data.Name, data.JobsCount, data.Modality, data.PopulationCount, data.OurMethodAlpha * 100);
            string path = string.Format("{0}\\{1}", rootPath, ResultFileName);
            System.IO.Directory.CreateDirectory(path);
            File.Copy(ResultBasePath + "\\Template.xlsx", path + "\\" + ResultFileName + ".xlsx");
            ResultPath = path;
            return true;
        }
        public static void Print(DiversityResult result)
        {
            String filename = String.Format("{0}\\{1}%.csv", ResultPath, result.DiversityRatio);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write("{0}%", result.DiversityRatio);
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
                    int counter = 0;
                    if (result.PopulationCount < 100)
                        counter = i * (100 / result.PopulationCount) + 1;
                    else
                        counter = i % 100 + 1;
                    writetext.Write(counter);
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
            int Modality = 0;
            int PopulationCount = 0;

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
                    Modality = result.Modality;
                    PopulationCount = result.PopulationCount;
                }
            }

            string filename = string.Format("{0}\\{1}.csv", ResultPath, ResultFileName);
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

                writetext.Write("Max distance, Ratio,Osuna Enciso et al,Cheng,Salleh et al, Zhao et al, Edge_Distances_Div, Wineberg_Oppacher");
                for (int i = 0; i < ClustersSizes.Length; i++)
                    writetext.Write(",Ours {0}", ClustersCounts[i]);
                writetext.WriteLine();

                for (int i = 0; i < diversityResults.Count; i++)
                {
                    DiversityResult result = diversityResults[i];
                    writetext.Write("{0:0.000},{1:0.000},{2:0.000},{3:0.000},{4:0.000},{5:0.000},{6:0.000},{7:0.000}",
                        result.MaxDistance, result.DiversityRatio,
                        result.Diversities.Osuna_Enciso_et_al,
                        result.Diversities.Cheng,
                        result.Diversities.Salleh_et_al,
                        result.Diversities.Zhao_et_al,
                        result.Diversities.Edge_Distances_Div,
                        result.Diversities.Wineberg_Oppacher
                        );
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
            data.Name = "T8";
            data.Modality = 4;
            data.PopulationCount = 100;
            data.OurMethodAlpha = 0.9;
            //data.ClustersSizes = new BigInteger[] { 144, 720, 5040 };
            data.ClustersSizes = new BigInteger[] { 6, 30, 120 };
            //data.ClustersSizes = new BigInteger[] { 5, 20, 40, 60, 80 };
            //data.ClustersSizes = new BigInteger[] { 200, 500, 1000, 4032, 10000 };
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
            //data.Optimas[0].Representation = 347;
            //data.Optimas[0].Representation = 10;
            //data.Optimas[1].Representation = 20;
            //data.Optimas[2].Representation = 700;
            //data.Optimas[3].Representation = 710;

            //data.Optimas[0].Representation = 1000;
            //data.Optimas[1].Representation = 1500;
            //data.Optimas[2].Representation = 2000;
            //data.Optimas[3].Representation = 40000;

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
                Console.Write("\r{0}%", 101 - data.DiversityRatio);
            }
            Console.WriteLine("\rFinished.");
            Print(diversityResults, new int[] { 100, 70, 40, 10, 2 });
        }

    }
}
