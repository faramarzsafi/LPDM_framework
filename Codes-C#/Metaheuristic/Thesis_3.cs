using Metaheuristic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Thesis_3
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
        public long ClustersCount;
        public BigInteger ClustersSize;
        public int PopulationCount;
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
            data.MaxDistance = MaxRatioDistance(ref data)/2;
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
            public double OurMethod;
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
            result.DiversityRatio = data.DiversityRatio;
            result.Optimas = GenerateOptimas(ref data);
            result.Populations = new Population[data.RepeatTimes];
            result.Diversities = new Diversities();
            for (int i = 0; i < data.RepeatTimes; i++)
            {
                populationGenerateSingle(ref data);
                result.Populations[i].permutations = data.Permutations;
                Diversity_Old.Result r = Diversity_Old.OurMethod_Old(data.Permutations, data.ClustersSize);
                result.Populations[i].Diversities.OurMethod = r.DivNorm;
                result.Populations[i].Diversities.Osuna_Enciso_et_al = Diversity_Old.Osuna_Enciso_et_al(data.Permutations);
                result.Populations[i].Diversities.Cheng = Diversity_Old.Cheng(data.Permutations);
                result.Populations[i].Diversities.Salleh_et_al = Diversity_Old.Salleh_et_al(data.Permutations);

                result.Diversities.OurMethod += result.Populations[i].Diversities.OurMethod;
                result.Diversities.Osuna_Enciso_et_al += result.Populations[i].Diversities.Osuna_Enciso_et_al;
                result.Diversities.Cheng += result.Populations[i].Diversities.Cheng;
                result.Diversities.Salleh_et_al += result.Populations[i].Diversities.Salleh_et_al;
            }
            result.Diversities.OurMethod /= data.RepeatTimes;
            result.Diversities.Osuna_Enciso_et_al /= data.RepeatTimes;
            result.Diversities.Cheng /= data.RepeatTimes;
            result.Diversities.Salleh_et_al /= data.RepeatTimes;
            return result;
            //for (int i = 0; i < data.PopulationCount; i++)
            //    Console.WriteLine("{0}", data.Permutations[i].Representation);

            //Console.WriteLine("Optima: {0}, Space Max distance: {1}, Max distance: {2}", data.Optimas[0].Representation, data.SpaceMaxDistance, data.MaxDistance);
            //Diversity.Result result = Diversity.OurMethod(data.Permutations, data.ClustersSize);
            //Console.WriteLine("Pop, Our Method,Osuna_Enciso_et_al,Cheng,Salleh_et_al\n{0},{1:0.000},{2:0.000},{3:0.000},{4:0.000}",
            //    data.Name,
            //    result.DivNorm,
            //    Diversity.Osuna_Enciso_et_al(data.Permutations),
            //    Diversity.Cheng(data.Permutations),
            //    Diversity.Salleh_et_al(data.Permutations));
        }

        public static void Run()
        {
            Data data = new Data();
            data.DiversityRatio = 100;
            data.ReductionRate = 1;
            data.DistanceType = Permutation.DistanceMeasureType.Linear;
            data.Name = "X";
            data.JobsCount = 6;
            data.Modality = 1;
            data.PopulationCount = 100;
            data.ClustersSize = 60;
            data.RepeatTimes = 50;

            List<DiversityResult> diversityResults = new List<DiversityResult>();
            Benchmark benchmark = new Benchmark(ref data);
            Console.WriteLine("Optima, Max distance, Ratio, Our Method,Osuna_Enciso_et_al,Cheng,Salleh_et_al");
            for (; data.DiversityRatio > 0; data.DiversityRatio -= data.ReductionRate)
            {
                DiversityResult result = benchmark.ComputeDiversity(ref data);
                diversityResults.Add(result);

                Console.WriteLine("{0:0.000},{1:0.000},{2:0.000},{3:0.000},{4:0.000},{5:0.000},{6:0.000}",
                    data.Optimas[0].Representation, data.MaxDistance, data.DiversityRatio,
                    result.Diversities.OurMethod,
                    result.Diversities.Osuna_Enciso_et_al,
                    result.Diversities.Cheng,
                    result.Diversities.Salleh_et_al);
            }
        }

    }
}
