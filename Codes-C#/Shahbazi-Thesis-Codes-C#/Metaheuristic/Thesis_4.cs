using Metaheuristic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Metaheuristic.Diversity_Old;

namespace Thesis_4
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
            Permutation[] permutations = new Permutation[data.PopulationCount];
            data.MaxDistance = MaxRatioDistance(ref data) / 2;
            //maxDistance = 5;

            for (int i = 0; i < data.PopulationCount; i++)
            {
                while (true)
                {
                    Permutation permutation = GenerateRandomPermutation(data);
                    //Check for duplicate
                    bool isFar = true;
                    double distance;
                    for (int j = 0; j < data.Modality; j++)
                    {
                        distance = permutation.RealDistanceTo(data.DistanceType, data.Optimas[j]);
                        if (distance < data.MaxDistance)
                        {
                            isFar = false;
                            break;
                        }
                    }
                    if (isFar)
                        continue;
                    permutations[i] = permutation;
                    break;
                }
            }
            data.Permutations = permutations;
            return permutations;
        }
        public Permutation[] populationGenerateSingle1(ref Data data)
        {
            Permutation[] permutations = new Permutation[data.PopulationCount];
            data.MaxDistance = MaxRatioDistance(ref data);
            //maxDistance = 5;

            for (int i = 0; i < data.PopulationCount; i++)
            {
                while (true)
                {
                    Permutation permutation = GenerateRandomPermutation(data);
                    //Check for duplicate
                    bool isFar = true;
                    double distance;
                    for (int j = 0; j < data.Modality; j++)
                    {
                        distance = permutation.RealDistanceTo(data.DistanceType, data.Optimas[j]);
                        if (distance < data.MaxDistance)
                        {
                            isFar = false;
                            break;
                        }
                    }
                    if (isFar)
                        continue;
                    permutations[i] = permutation;
                    break;
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
            public Permutation[] permutations;
            public Diversities Diversities;
        }
        public struct DiversityResult
        {
            public Population[] Populations;
            public Permutation[] Optimas;
            public int DiversityRatio;
            public Diversities Diversities;
        }
        public DiversityResult ComputeDiversity(ref Data data)
        {
            DiversityResult result;
            result.Optimas = data.Optimas;
            result.DiversityRatio = data.DiversityRatio;
            result.Populations = new Population[data.RepeatTimes];
            result.Diversities = new Diversities();
            result.Diversities.OurMethod = new double[data.ClustersSizes.Length];

            for (int i = 0; i < data.RepeatTimes; i++)
            {
                populationGenerateSingle(ref data);
                result.Populations[i].permutations = data.Permutations;
                result.Populations[i].Diversities.OurMethod = new double[data.ClustersSizes.Length];
                for (int j = 0; j < data.ClustersSizes.Length; j++)
                {
                    Diversity_Old.Result r = Diversity_Old.OurMethod_Old(data.Permutations, data.ClustersSizes[j]);
                    
                    result.Populations[i].Diversities.OurMethod[j] = r.DivNorm;
                    result.Diversities.OurMethod[j] += result.Populations[i].Diversities.OurMethod[j];

                }
                result.Populations[i].Diversities.Osuna_Enciso_et_al = Diversity_Old.Osuna_Enciso_et_al(data.Permutations);
                result.Populations[i].Diversities.Cheng = Diversity_Old.Cheng(data.Permutations);
                result.Populations[i].Diversities.Salleh_et_al = Diversity_Old.Salleh_et_al(data.Permutations);

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
            return result;
        }

        public static void Run()
        {
            Data data = new Data();
            data.JobsCount = 6;
            Permutation.JobsCount = data.JobsCount;
            data.DiversityRatio = 100;
            data.ReductionRate = 1;
            data.DistanceType = Permutation.DistanceMeasureType.Linear;
            data.Name = "X";
            data.Modality = 1;
            data.PopulationCount = 100;
            data.ClustersSizes = new BigInteger[] {5, 10, 50, 100};
            data.ComputeClustersCounts();
            data.RepeatTimes = 50;

            List<DiversityResult> diversityResults = new List<DiversityResult>();
            Benchmark benchmark = new Benchmark(ref data);
            benchmark.GenerateOptimas(ref data);
            
            Console.Write("Max distance, Ratio,Osuna Enciso et al,Cheng,Salleh et al");
            for (int i = 0; i < data.ClustersSizes.Length; i++)
                Console.Write(",Ours {0}", data.ClustersCounts[i]);
            Console.WriteLine();
            for (; data.DiversityRatio > 0; data.DiversityRatio -= data.ReductionRate)
            {
                DiversityResult result = benchmark.ComputeDiversity(ref data);
                diversityResults.Add(result);

                Console.Write("{0:0.000},{1:0.000},{2:0.000},{3:0.000},{4:0.000}",
                    data.MaxDistance, data.DiversityRatio,
                    result.Diversities.Osuna_Enciso_et_al,
                    result.Diversities.Cheng,
                    result.Diversities.Salleh_et_al);
                for (int i = 0; i < data.ClustersSizes.Length; i++)
                    Console.Write(",{0:0.000}", result.Diversities.OurMethod[i]);
                Console.WriteLine();
            }
            Console.Write("Optimas:");
            for (int i = 0; i < data.Optimas.Length; i++)
            {
                if (i != 0)
                    Console.Write(",");
                Console.Write(" {0}", data.Optimas[i].Representation);
            }
            Console.WriteLine();

        }

    }
}
