using Metaheuristic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Metaheuristic.Diversity_Old;

namespace Thesis_9
{
    internal class Benchmark
    {
        public string Name;
        public int Modality;
        public Permutation[] Optimas;

        List<Diversity> Diversities = new List<Diversity>();
        public List<Permutation[]> PermutationsList = new List<Permutation[]>();
        List<Dictionary<string, double>> DiversitiesValues = new List<Dictionary<string, double>>();
        List<double> MaxDistances = new List<double>();
        List<int> DiversityRatios = new List<int>();
        List<Dictionary<string, long>> ElapsedTimes = new List<Dictionary<string, long>>();

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
        private int diversityRatio;
        public Benchmark(string name, int jobsCount, int populationCount, int modality=1, Permutation.DistanceMeasureType distanceType = Permutation.DistanceMeasureType.Linear)
        {
            JobsCount = jobsCount;
            Permutation.JobsCount = jobsCount;
            Name = name;
            PopulationCount = populationCount;
            Modality = modality;
            DistanceType = distanceType;
            diversityRatio = 100;
            RepeatTimes = 50;
            ReductionRate = 1;
            switch (DistanceType)
            {
                case Permutation.DistanceMeasureType.Linear:
                    SpaceMax = Factoradic.Factorial[JobsCount];
                    SpaceMaxDistance = (double)SpaceMax; break;
                case Permutation.DistanceMeasureType.L1norm:
                    SpaceMaxDistance = ((double)JobsCount * JobsCount); break;
                case Permutation.DistanceMeasureType.L2norm:
                    SpaceMaxDistance = Math.Sqrt(JobsCount * JobsCount * JobsCount); break;
            }

        }
        private Random random = new Random();
        public Permutation GenerateRandomPermutation()
        {
            int[] jobs = new int[JobsCount];
            for (int i = 0; i < JobsCount; i++)
            {
                int x;
                while (true)
                {
                    bool duplicate = false;
                    x = random.Next(JobsCount);
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
        public Permutation[] GenerateOptimas(params BigInteger[] optimas)
        {
            Permutation[] permutations = new Permutation[Modality];
            for (int i = 0; i < Modality; i++)
            {
                if (optimas != null && optimas.Length > i)
                {
                    permutations[i] = new Permutation(optimas[i]);
                    continue;
                }
                while (true)
                {
                    Permutation permutation = GenerateRandomPermutation();
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
            }
            Optimas = permutations;
            return permutations;
        }
        public Permutation[] populationGenerateSingle()
        {
            Permutation[] permutations = new Permutation[PopulationCount * Optimas.Length];
            for (int i = 0; i < Optimas.Length; i++)
            {
                Permutation optima = Optimas[i];
                double optimaRightMargin = (double)(SpaceMax - optima.Representation) * diversityRatio / 100;
                double optimaLeftMargin = (double)(optima.Representation) * diversityRatio / 100;
                for (int j = 0; j < PopulationCount; j++)
                {
                    while (true)
                    {
                        Permutation permutation = GenerateRandomPermutation();
                        double distance;
                        distance = permutation.RealDistanceTo(DistanceType, optima, true);
                        if (distance == 0 || distance > 0 && distance < optimaRightMargin || distance < 0 && -distance < optimaLeftMargin)
                        {
                            permutations[i * PopulationCount + j] = permutation;
                            break;
                        }
                    }
                }
            }
            return permutations;
        }

        public void ComputeDiversity()
        {
            Dictionary<string, double> DiversityValues = new Dictionary<string, double>();
            Dictionary<string, long> elapsedTimes = new Dictionary<string, long>();
            for (int i = 0; i < Diversities.Count; i++)
            {
                DiversityValues[Diversities[i].Name] = 0;
                elapsedTimes[Diversities[i].Name] = 0;
            }

            Permutation[] permutations = null;

            var stopwatch = new Stopwatch();
            for (int i = 0; i < RepeatTimes; i++)
            {
                permutations = populationGenerateSingle();
                for (int j = 0; j < Diversities.Count; j++)
                {
                    stopwatch.Restart();
                    DiversityValues[Diversities[j].Name] += Diversities[j].Compute(permutations);
                    stopwatch.Stop();
                    elapsedTimes[Diversities[j].Name] += stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency; 
                }
            }

            for (int i = 0; i < Diversities.Count; i++)
            {
                DiversityValues[Diversities[i].Name] /= RepeatTimes;
                elapsedTimes[Diversities[i].Name] /= RepeatTimes;
            }

            DiversitiesValues.Add(DiversityValues);
            ElapsedTimes.Add(elapsedTimes);
            PermutationsList.Add(permutations);
            MaxDistances.Add(SpaceMaxDistance * diversityRatio / 100);
            DiversityRatios.Add(diversityRatio);
        }
        string ResultBasePath = "";
        string ResultPath = "";
        string ResultFileName = "";
        public bool InitResultPath(string resultBasePath)
        {
            ResultBasePath = resultBasePath;
            if (!System.IO.Directory.Exists(ResultBasePath))
                return false;

            string rootPath = string.Format("{0}\\{1}-{2}!", ResultBasePath, Name, JobsCount);
            System.IO.Directory.CreateDirectory(rootPath);
            ResultFileName = string.Format("{0}, {1}-{2}!, Mod {3}, PpO {4}, α {5}",
                DateTime.Now.ToString("yyMMddHHmmss.fff"), Name, JobsCount, Modality, PopulationCount, OurMethodAlpha * 100);
            string path = string.Format("{0}\\{1}", rootPath, ResultFileName);
            System.IO.Directory.CreateDirectory(path);
            File.Copy(ResultBasePath + "\\Template.xlsx", path + "\\" + ResultFileName + ".xlsx");
            ResultPath = path;
            return true;
        }
        public void Print(Permutation[] permutations, int diversityRatio)
        {
            String filename = String.Format("{0}\\{1}%.csv", ResultPath, diversityRatio);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write("{0}%", diversityRatio);
                for (int i = 0; i < Optimas.Length; i++)
                    writetext.Write(",Optima {0}, Optima [{1}]", i + 1, i + 1);
                writetext.WriteLine();
                writetext.Write(" ");
                for (int i = 0; i < Optimas.Length; i++)
                    writetext.Write(",{0},[{1}]", Optimas[i].Representation, Optimas[i].ToArrayString(false, " "));
                writetext.WriteLine("\n------------------------------------------------");
                writetext.Write("#");
                for (int i = 0; i < Optimas.Length; i++)
                    writetext.Write(",Optima {0}, Optima [{1}]", i + 1, i + 1);
                writetext.WriteLine();
                for (int i = 0; i < PopulationCount; i++)
                {
                    int counter = 0;
                    if (PopulationCount < 100)
                        counter = i * (100 / PopulationCount) + 1;
                    else
                        counter = i % 100 + 1;
                    writetext.Write(counter);
                    for (int j = 0; j < Optimas.Length; j++)
                    {
                        Permutation p = permutations[i + j * PopulationCount];
                        writetext.Write(",{0},[{1}]", p.Representation, p.ToArrayString(false, " "));
                    }
                    writetext.WriteLine();
                }
            }
        }
        public void PrintDiversity()
        {
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

                writetext.Write("Max distance, Ratio");
                for (int i = 0; i < Diversities.Count; i++)
                    writetext.Write(",{0}", Diversities[i].Name);
                writetext.WriteLine();

                for (int i = 0; i < DiversitiesValues.Count; i++)
                {
                    writetext.Write("{0:0.000},{1:0.000}", MaxDistances[i], DiversityRatios[i]);
                    for (int j = 0; j < Diversities.Count; j++)
                        writetext.Write(",{0:0.000}", DiversitiesValues[i][Diversities[j].Name]);
                    writetext.WriteLine();
                }
            }

        }
        public void PrintTime()
        {
            string filename = string.Format("{0}\\time.csv", ResultPath, ResultFileName);
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

                writetext.Write("Max distance, Ratio");
                for (int i = 0; i < Diversities.Count; i++)
                    writetext.Write(",{0}", Diversities[i].Name);
                writetext.WriteLine();

                for (int i = 0; i < DiversitiesValues.Count; i++)
                {
                    writetext.Write("{0:0.000},{1:0.000}", MaxDistances[i], DiversityRatios[i]);
                    for (int j = 0; j < Diversities.Count; j++)
                        writetext.Write(",{0:0.000}", ElapsedTimes[i][Diversities[j].Name]);
                    writetext.WriteLine();
                }
            }

        }
        public void Print(int[] selectedDiversityRatios)
        {
            for (int i = 0; i < selectedDiversityRatios.Length; i++)
            {
                var divRatioIndex = DiversityRatios.IndexOf(selectedDiversityRatios[i]);
                if (divRatioIndex != -1)
                    Print(PermutationsList[divRatioIndex], selectedDiversityRatios[i]);
            }
            PrintDiversity();
            PrintTime();
        }
        public static void Run()
        {
            Benchmark benchmark = new Benchmark("T9", 6, 100);
            benchmark.Modality = 4;
            benchmark.OurMethodAlpha = 0.9;
            //Diversities
            benchmark.Diversities.Add(new Osuna_Enciso_et_al());
            benchmark.Diversities.Add(new Cheng());
            benchmark.Diversities.Add(new Salleh_et_al());
            benchmark.Diversities.Add(new Zhao_et_al());
            benchmark.Diversities.Add(new Zhu_et_al());
            benchmark.Diversities.Add(new Wineberg_Oppacher());
            benchmark.Diversities.Add(new OurMethod_FreeClusterSize(6, benchmark.OurMethodAlpha));
            benchmark.Diversities.Add(new OurMethod_FreeClusterSize(12, benchmark.OurMethodAlpha));
            benchmark.Diversities.Add(new OurMethod_FreeClusterSize(24, benchmark.OurMethodAlpha));
            benchmark.Diversities.Add(new OurMethod_FreeClusterSize(48, benchmark.OurMethodAlpha));

            //Diversities
            //benchmark.GenerateOptimas();
            //benchmark.GenerateOptimas(10, 20, 700, 710);
            benchmark.GenerateOptimas(102, 121, 315, 640);
            //benchmark.GenerateOptimas(100,1500,2000,40000);

            if (!benchmark.InitResultPath("D:\\Personal\\Master\\THESIS\\Tests"))
            {
                Console.WriteLine("Can not initialize result path.");
                return;
            }
            Console.Write("Optimas:");
            for (int i = 0; i < benchmark.Optimas.Length; i++)
            {
                if (i != 0)
                    Console.Write(",");
                Console.Write(" {0}", benchmark.Optimas[i].Representation);
            }
            Console.WriteLine();

            Console.WriteLine("Starting...");
            for (; benchmark.diversityRatio > 0; benchmark.diversityRatio -= benchmark.ReductionRate)
            {
                benchmark.ComputeDiversity();
                Console.Write("\r{0}%", 101 - benchmark.diversityRatio);
            }
            Console.WriteLine("\rFinished.");
            benchmark.Print(new int[] { 100, 70, 40, 10, 2 });
        }

    }
}
