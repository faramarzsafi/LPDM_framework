using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Metaheuristic;

namespace Thesis_1
{
    internal class Data
    {
        public string Name;
        public int Modality;
        public Permutation[] Optimas;
        public Permutation[] Permutations;
        public BigInteger[] Population;
        public int DiversityRatio;
        public BigInteger MaxDistance;
        public int JobsCount;
        public int IterationCount;
        public int ReductionRate;
        public Permutation.DistanceMeasureType DistanceType;
        int ClustersCount;
    }
    internal class Benchmark
    {
        int ClustersCount;
        Permutation[] Permutations;
        BigInteger[] Population;
        string Name;
        int Modality;
        int PopulationCount;
        int IterationCount;
        int ReductionRate;
        int DiversityRatio;
        BigInteger MaxDistance;
        int JobsCount;

        Permutation.DistanceMeasureType DistanceType;
        public Benchmark(string name, int modality, int populationCount)
        {
            JobsCount = 6;
            Permutation.JobsCount = JobsCount;
            ClustersCount = 12;
            IterationCount = 51;
            ReductionRate = 2;
            DiversityRatio = 30;
            DistanceType = Permutation.DistanceMeasureType.L1norm;
            Name = name;
            Modality = modality;
            PopulationCount = populationCount;
            MaxDistance = Factoradic.Factorial[JobsCount];
        }
        public void PopulationToPermutation()
        {
            Permutations = new Permutation[Population.Length];
            BigInteger b;
            for (int i = 0; i < Population.Length; i++)
            {
                b = Population[i];
                Permutations[i] = b.ToPermutation();
            }
        }
        public void PermutationToPopulation()
        {
            Population = new BigInteger[Permutations.Length];
            for (int i = 0; i < Population.Length; i++)
            {
                Population[i] = Permutations[i].Representation;
            }
        }
        public void GeneratePopulation(ref Data data)
        {
            GenerateOptimas(ref data);
            populationGenerateSingle(ref data);
        }
        private Random random = new Random();
        public Permutation GenerateRandomPermutation()
        {
            int[] jobs=new int[JobsCount];
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
        public Permutation[] GenerateOptimas(ref Data data)
        {
            Permutation[] permutations = new Permutation[data.Modality];
            for (int i = 0; i < data.Modality; i++)
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
            data.Optimas = permutations;
            return permutations;
        }
        public BigInteger MaxRatioDistance(ref Data data)
        {
            switch (DistanceType)
            {
                case Permutation.DistanceMeasureType.Linear:
                    return (BigInteger)((double)MaxDistance * data.DiversityRatio / 100);
                case Permutation.DistanceMeasureType.L1norm:
                    return (BigInteger)((double)JobsCount * JobsCount * data.DiversityRatio / 100);
                case Permutation.DistanceMeasureType.L2norm:
                    return (BigInteger)(Math.Sqrt(JobsCount * JobsCount * JobsCount) * data.DiversityRatio / 100);
            }
            return 0;
        }
        public Permutation[] populationGenerateSingle(ref Data data)
        {
            Permutation[] permutations = new Permutation[PopulationCount];
            BigInteger maxDistance = MaxRatioDistance(ref data);
            //maxDistance = 5;

            for (int i = 0; i < PopulationCount; i++)
            {
                while (true)
                {
                    Permutation permutation = GenerateRandomPermutation();
                    //Check for duplicate
                    bool isFar = true;
                    BigInteger distance;
                    for (int j = 0; j < Modality; j++)
                    {
                        distance = permutation.DistanceTo(DistanceType, data.Optimas[j]);
                        if (permutation.DistanceTo(DistanceType, data.Optimas[j]) < maxDistance)
                        {
                            isFar = false;
                            break;
                        }
                    }
                    if (isFar)
                        continue;               
                    //if (permutation.Representation - )
                    permutations[i] = permutation;
                    break;
                }
            }
            data.Permutations = permutations;
            return permutations;
        }
        public void Compare()
        {
            Data data = new Data();
            data.Modality = 1;
            data.DiversityRatio = 20;

            GeneratePopulation(ref data);
            for (int i = 0; i < PopulationCount; i++)
                Console.WriteLine("{0}", data.Permutations[i].Representation);

            Console.WriteLine("Optima:, {0}", data.Optimas[0].Representation);
            Diversity_Old.Result result = Diversity_Old.OurMethod_Old(data.Permutations, ClustersCount);
            Console.WriteLine("Pop, Our Method,Osuna_Enciso_et_al,Cheng,Salleh_et_al\n{0},{1:0.000},{2:0.000},{3:0.000},{4:0.000}",
                Name,
                result.DivNorm,
                Diversity_Old.Osuna_Enciso_et_al(data.Permutations),
                Diversity_Old.Cheng(data.Permutations),
                Diversity_Old.Salleh_et_al(data.Permutations));
        }

        public static void Run()
        {
            Benchmark benchmark=new Benchmark("Simple", 1, 100);
            benchmark.Compare();
        }

    }
}
