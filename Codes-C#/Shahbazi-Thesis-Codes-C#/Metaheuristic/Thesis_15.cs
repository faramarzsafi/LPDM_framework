using Metaheuristic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Metaheuristic.Diversity_Old;

namespace Thesis_15
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
        public int OutlierPercent;
        public Benchmark(string name, int jobsCount, int populationCount, int modality = 1, Permutation.DistanceMeasureType distanceType = Permutation.DistanceMeasureType.Linear)
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
            OutlierPercent = 0;
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
        public static BigInteger NextBigInteger(Random random, BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;

            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                R = new BigInteger(bytes);
            } while (R >= N);

            return R;
        }
        public Permutation GenerateRandomPermutation(BigInteger optima, BigInteger marginLeft, BigInteger marginRight)
        {
            BigInteger distance = 0;
            BigInteger p = 0;
            while (distance == 0)
            {
                distance = NextBigInteger(random, marginLeft + marginRight);
                if (distance < marginLeft)
                    p = optima - distance;
                else
                    p = optima + distance - marginLeft;

            }
            Permutation permutation = new Permutation(p);
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
                BigInteger optimaRightMargin = (SpaceMax - optima.Representation) * diversityRatio / 100;
                BigInteger optimaLeftMargin = (optima.Representation) * diversityRatio / 100;
                int inliersCount = PopulationCount - PopulationCount * OutlierPercent / 100;

                for (int j = 0; j < PopulationCount; j++)
                {
                    Permutation permutation = GenerateRandomPermutation(optima.Representation, optimaLeftMargin, optimaRightMargin);
                    permutations[i * PopulationCount + j] = permutation;
                }
                //double dOptimaRightMargin = (double)optimaRightMargin;
                //double dOptimaLeftMargin = (double)optimaLeftMargin;
                //for (int j = 0; j < PopulationCount; j++)
                //{
                //    Permutation permutation = GenerateRandomPermutation(optima.Representation, optimaLeftMargin, optimaRightMargin);
                //    double distance;
                //    distance = permutation.RealDistanceTo(DistanceType, optima, true);
                //    if (j < inliersCount)
                //    {
                //        if (distance == 0 || distance > 0 && distance < dOptimaRightMargin || distance < 0 && -distance < dOptimaLeftMargin)
                //        {
                //            permutations[i * PopulationCount + j] = permutation;
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        if (distance > 0 && distance >= dOptimaRightMargin || distance < 0 && -distance >= dOptimaLeftMargin)
                //        {
                //            permutations[i * PopulationCount + j] = permutation;
                //            break;
                //        }
                //    }
                //}
            }
            return permutations;
        }

        public Permutation[] populationGenerateSingle1()
        {
            Permutation[] permutations = new Permutation[PopulationCount * Optimas.Length];
            for (int i = 0; i < Optimas.Length; i++)
            {
                Permutation optima = Optimas[i];
                double optimaRightMargin1 = (double)(SpaceMax - optima.Representation) * diversityRatio / 100;
                double optimaLeftMargin1 = (double)(optima.Representation) * diversityRatio / 100;
                int inliersCount = PopulationCount - PopulationCount * OutlierPercent / 100;

                for (int j = 0; j < PopulationCount; j++)
                {
                    while (true)
                    {
                        Permutation permutation = GenerateRandomPermutation();
                        double distance;
                        distance = permutation.RealDistanceTo(DistanceType, optima, true);
                        if (j < inliersCount)
                        {
                            if (distance == 0 || distance > 0 && distance < optimaRightMargin1 || distance < 0 && -distance < optimaLeftMargin1)
                            {
                                permutations[i * PopulationCount + j] = permutation;
                                break;
                            }
                        }
                        else
                        {
                            if (distance > 0 && distance >= optimaRightMargin1 || distance < 0 && -distance >= optimaLeftMargin1)
                            {
                                permutations[i * PopulationCount + j] = permutation;
                                break;
                            }
                        }
                    }
                }
            }
            return permutations;
        }
        public void RunDiversityCompute(int index, Permutation[] permutations, Dictionary<string, double> DiversityValues, Dictionary<string, long> elapsedTimes)
        {
            if (index == 16)
                Console.WriteLine("16");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            DiversityValues[Diversities[index].Name] += Diversities[index].Compute(permutations);
            stopwatch.Stop();
            elapsedTimes[Diversities[index].Name] += stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
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
            var lockObj = new object();
            for (int i = 0; i < RepeatTimes; i++)
            {
                permutations = populationGenerateSingle();
                Task[] tasks = new Task[Diversities.Count];
                ComputeResult[] computeResults = new ComputeResult[Diversities.Count];
                for (int j = 0; j < Diversities.Count; j++)
                {
                    var diversity = Diversities[j];
                    tasks[j] = new Task(() => diversity.AsyncCompute(permutations));
                }
                for (int j = 0; j < Diversities.Count; j++)
                    tasks[j].Start();
                Task.WaitAll(tasks);
                for (int j = 0; j < Diversities.Count; j++)
                {
                    DiversityValues[Diversities[j].Name] += Diversities[j].AsyncComputeResult.diversity;
                    elapsedTimes[Diversities[j].Name] += Diversities[j].AsyncComputeResult.elapsedtime;
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
        public bool InitResultPath()
        {
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
            string filename = string.Format("{0}\\data-{1}.csv", ResultPath, ResultFileName);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write("Optimas,");
                for (int i = 0; i < Optimas.Length; i++)
                {
                    if (i != 0)
                        writetext.Write(",");
                    string jobs = "";
                    for (int j = 0; j < Optimas[i].Jobs.Length; j++)
                    {
                        if (j != 0)
                            jobs += " ";
                        jobs += String.Format("{0}", Optimas[i].Jobs[j]);
                    }
                    writetext.Write(" {0}({1})", Optimas[i].Representation, jobs);
                }
                writetext.WriteLine();

                writetext.Write("H-Axis, Max distance, Ratio");
                for (int i = 0; i < Diversities.Count; i++)
                    writetext.Write(",{0}", Diversities[i].Name);
                writetext.WriteLine();

                for (int i = 0; i < DiversitiesValues.Count; i++)
                {
                    writetext.Write("{0},{1},{2}", DiversitiesValues.Count - i, MaxDistances[i], DiversityRatios[i]);
                    for (int j = 0; j < Diversities.Count; j++)
                        writetext.Write(",{0}", DiversitiesValues[i][Diversities[j].Name]);
                    writetext.WriteLine();
                }
            }

        }
        public void PrintTime()
        {
            string filename = string.Format("{0}\\time-{1}.csv", ResultPath, ResultFileName);
            using (StreamWriter writetext = new StreamWriter(filename))
            {
                writetext.Write("Optimas,");
                for (int i = 0; i < Optimas.Length; i++)
                {
                    if (i != 0)
                        writetext.Write(",");
                    string jobs = "";
                    for (int j = 0; j < Optimas[i].Jobs.Length; j++)
                    {
                        if (j != 0)
                            jobs += " ";
                        jobs += String.Format("{0}", Optimas[i].Jobs[j]);
                    }
                    writetext.Write(" {0}({1})", Optimas[i].Representation, jobs);
                }
                writetext.WriteLine();

                writetext.Write("H-Axis, Max distance, Ratio");
                for (int i = 0; i < Diversities.Count; i++)
                    writetext.Write(",{0}", Diversities[i].Name);
                writetext.WriteLine();

                for (int i = 0; i < DiversitiesValues.Count; i++)
                {
                    writetext.Write("{0},{1},{2}", DiversitiesValues.Count - i, MaxDistances[i], DiversityRatios[i]);
                    for (int j = 0; j < Diversities.Count; j++)
                        writetext.Write(",{0}", ElapsedTimes[i][Diversities[j].Name]);
                    writetext.WriteLine();
                }
            }

        }
        public void Print(int[] selectedDiversityRatios)
        {
            if (selectedDiversityRatios != null)
                for (int i = 0; i < selectedDiversityRatios.Length; i++)
                {
                    var divRatioIndex = DiversityRatios.IndexOf(selectedDiversityRatios[i]);
                    if (divRatioIndex != -1)
                        Print(PermutationsList[divRatioIndex], selectedDiversityRatios[i]);
                }
            PrintDiversity();
            PrintTime();
        }
        public Dictionary<string, string> ReadParams(string[] args)
        {
            if (args == null)
                return null;

            var parameters = new Dictionary<string, string>();
            if (args.Length > 0)
            {
                foreach (string argument in args)
                {
                    int idx = argument.IndexOf('=');

                    if (idx == -1)
                        continue;
                    string key = argument.Substring(0, idx);
                    string value = argument.Substring(idx + 1);
                    parameters[key] = value;
                }
            }
            return parameters;
        }
        public string SetParams(Dictionary<string, string> parameters)
        {
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                switch (key.ToUpper().Trim())
                {
                    case "NAME":
                        if (value == "")
                            return "Invalid Name";
                        Name = value;
                        break;
                    case "PERMUTATIONLENGTH":
                        if (!int.TryParse(value, out JobsCount))
                            return "Invalid PermutationLength";
                        Permutation.JobsCount = JobsCount;
                        break;

                    case "POPULATIONCOUNT":
                        if (!int.TryParse(value, out PopulationCount))
                            return "Invalid PopulationCount";
                        break;
                    case "MODALITY":
                        if (!int.TryParse(value, out Modality))
                            return "Invalid Modality";
                        break;
                    case "OUTLIERPERCENT":
                        if (!int.TryParse(value, out OutlierPercent))
                            return "Invalid OutlierPercent";
                        break;
                    case "WORKINGFOLDER":
                        ResultBasePath = value;
                        break;
                }
            }
            return null;
        }
        public static void Run(string[] args)
        {

            Benchmark benchmark = new Benchmark("T15", 7, 100);
            Dictionary<string, string> parameters = benchmark.ReadParams(args);
            string err = benchmark.SetParams(parameters);
            if (err != null)
            {
                Console.WriteLine("ERROR: {0}", err);
                return;
            }
            //benchmark.RepeatTimes = 1;
            benchmark.Modality = 4;
            benchmark.OurMethodAlpha = 1;
            benchmark.OutlierPercent = 0;
            //Diversities
            benchmark.Diversities.Add(new OurMethod());

            benchmark.Diversities.Add(new Dummy());
            benchmark.Diversities.Add(new Dummy());
            benchmark.Diversities.Add(new Dummy());
            benchmark.Diversities.Add(new Dummy());

            //benchmark.Diversities.Add(new OurMethod_FreeClusterSize(240, benchmark.OurMethodAlpha));
            //benchmark.Diversities.Add(new OurMethod_FreeClusterSize(120, benchmark.OurMethodAlpha));
            //benchmark.Diversities.Add(new OurMethod_FreeClusterSize(60, benchmark.OurMethodAlpha));
            //benchmark.Diversities.Add(new OurMethod_FreeClusterSize(15, benchmark.OurMethodAlpha));
            benchmark.Diversities.Add(new Ursem());//DM1
            benchmark.Diversities.Add(new Wineberg_Oppacher());//DM2
            benchmark.Diversities.Add(new Zhu_et_al());//DM3
            benchmark.Diversities.Add(new Shi());//DM4
            benchmark.Diversities.Add(new Wang_et_al());//DM5
            benchmark.Diversities.Add(new Li_et_al());//DM6
            benchmark.Diversities.Add(new Cheng());//DM7
            benchmark.Diversities.Add(new Tilahun());//DM8
            benchmark.Diversities.Add(new Salleh_et_al());//DM9
            benchmark.Diversities.Add(new Zhao_et_al());//DM10
            benchmark.Diversities.Add(new Osuna_Enciso_et_al());//DM11
            //benchmark.Diversities.Add(new Dummy());
            //benchmark.Diversities.Add(new Dummy());
            //benchmark.Diversities.Add(new Dummy());
            //benchmark.Diversities.Add(new Dummy());

            ////Diversities
            benchmark.GenerateOptimas();
            //benchmark.GenerateOptimas(111);
            //benchmark.GenerateOptimas(10, 20, 700, 710);
            //benchmark.GenerateOptimas(102, 121, 315, 640);
            //benchmark.GenerateOptimas(100,1500,2000,40000);
            //benchmark.GenerateOptimas(127, 143, 633, 674);
            //benchmark.GenerateOptimas(10, 32, 671, 699);
            //benchmark.GenerateOptimas(10, 243, 476, 710);
            //benchmark.GenerateOptimas(718, 719);
            //benchmark.GenerateOptimas(506,3025, 1680, 2147);//T2: 506(0 5 2 1 4 3 6), 3025(4 1 2 0 3 6 5), 1680(2 3 0 1 4 5 6), 2147(2 6 5 1 4 3 0)
            //benchmark.GenerateOptimas(10,20,5020,5030);     //T3: 10(0 1 2 4 6 3 5), 20(0 1 2 6 4 3 5), 5020(6 5 4 0 3 1 2), 5030(6 5 4 2 1 0 3)
            benchmark.GenerateOptimas(1660, 1670, 1680, 1690);//T4: 1660(2 1 6 0 5 3 4), 1670(2 1 6 4 3 0 5), 1680(2 3 0 1 4 5 6), 1690(2 3 0 4 6 1 5)
            benchmark.ResultBasePath = "D:\\Personal\\Master\\THESIS\\Tests";

            if (!benchmark.InitResultPath())
            {
                Console.WriteLine("Can not initialize result path.");
                return;
            }
            Console.Write("Optimas:");
            for (int i = 0; i < benchmark.Optimas.Length; i++)
            {
                if (i != 0)
                    Console.Write(",");
                string jobs = "";
                for (int j = 0; j < benchmark.Optimas[i].Jobs.Length; j++)
                {
                    if (j != 0)
                        jobs += ",";
                    jobs += String.Format("{0}", benchmark.Optimas[i].Jobs[j]);
                }

                Console.Write(" {0}({1})", benchmark.Optimas[i].Representation, jobs);
            }
            Console.WriteLine();

            Console.WriteLine("Starting...");
            for (; benchmark.diversityRatio > 0; benchmark.diversityRatio -= benchmark.ReductionRate)
            {
                benchmark.ComputeDiversity();
                Console.Write("\r{0}%", 101 - benchmark.diversityRatio);
            }
            Console.WriteLine("\rFinished.");
            benchmark.Print(null);
            //benchmark.Print(new int[] { 100, 80, 60, 55, 50, 45, 40, 10, 2 });
            //benchmark.Print(new int[] { 100, 99, 98, 97, 96, 95, 70, 40, 10, 2 });
            //benchmark.Print(new int[] { 100, 70, 40, 10, 2 });
        }

    }
}
