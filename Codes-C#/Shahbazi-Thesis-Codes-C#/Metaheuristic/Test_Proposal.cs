using System;
using System.Numerics;

namespace Metaheuristic
{

    partial class Program
    {
        static int[] sample1 = new int[] { 9, 1, 99, 2, 33, 53, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 8, 23, 23, 99, 7, 6, 33, 11, 9, 61, 56, 24, 6, 4, 3, 13, 42, 39, 61, 63, 4, 2, 7, 32, 64, 97, 52, 1, 54, 4, 9, 11 };
        static int[] sample2 = new int[] { 91, 97, 99, 2, 33, 55, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 88, 23, 23, 99, 88, 83, 33, 11, 82, 61, 55, 24, 111, 112, 117, 13, 42, 39, 61, 63, 71, 73, 115, 32, 65, 97, 52, 55, 54, 103, 9, 11 };
        static int[] sample3 = new int[] { 91, 97, 99, 41, 33, 55, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 88, 23, 23, 118, 88, 83, 33, 11, 82, 61, 55, 24, 111, 112, 117, 13, 42, 41, 74, 79, 71, 73, 115, 32, 65, 1, 109, 101, 108, 103, 9, 11 };
        static int[] sample4 = new int[] { 32, 35, 39, 31, 32, 37, 36, 31, 33, 36, 32, 39, 38, 32, 31, 36, 39, 35, 35, 31, 39, 34, 33, 38, 37, 31, 33, 35, 36, 39, 38, 32, 31, 33, 35, 35, 37, 34, 32, 35, 36, 39, 31, 38, 39, 31, 34, 35, 36, 37 };
        static int[] sample6 = new int[] { 9, 1, 99, 2, 33, 53, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 8, 23, 23, 99, 7, 6, 33, 11, 9, 61, 56, 24, 6, 4, 3, 13, 42, 39, 61, 63, 4, 2, 7, 32, 64, 97, 52, 1, 54, 4, 9, 11, 91, 97, 99, 2, 33, 55, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 88, 23, 23, 99, 88, 83, 33, 11, 82, 61, 55, 24, 111, 112, 117, 13, 42, 39, 61, 63, 71, 73, 115, 32, 65, 97, 52, 55, 54, 103, 9, 11, 91, 97, 99, 41, 33, 55, 54, 31, 91, 7, 22, 2, 61, 68, 6, 47, 53, 12, 88, 23, 23, 118, 88, 83, 33, 11, 82, 61, 55, 24, 111, 112, 117, 13, 42, 41, 74, 79, 71, 73, 115, 32, 65, 1, 109, 101, 108, 103, 9, 11, 32, 35, 39, 31, 32, 37, 36, 31, 33, 36, 32, 39, 38, 32, 31, 36, 39, 35, 35, 31, 39, 34, 33, 38, 37, 31, 33, 35, 36, 39, 38, 32, 31, 33, 35, 35, 37, 34, 32, 35, 36, 39, 31, 38, 39, 31, 34, 35, 36, 37 };
        static int[] sample7 = new int[] { 9};
        public static void Test_Proposal()
        {
            TestSample(sample7);
            //TestSample(sample2);
        }

        public static void TestSample(int[] sample)
        {
            Permutation.JobsCount = 5;
            Permutation[] permutations = new Permutation[sample.Length];
            BigInteger b;
            for (int i = 0; i < sample.Length; i++)
            {
                b = sample[i];
                permutations[i] = b.ToPermutation();
            }
            int ClustersCount = 12;
            Diversity_Old.Result result = Diversity_Old.OurMethod_Old(permutations, ClustersCount);
            Console.WriteLine("\nOur Diversity\n");
            Console.WriteLine("j,Pj,Divj,Dj,DNj,DivLj");
            for (int i = 0;i < ClustersCount; i++)
            {
                Console.WriteLine("{0},{1},{2},{3},{4},{5}", 
                    i+1, result.Clusters[i], result.Clusters[i]/result.PMax, result.ClusterDistances[i], result.ClusterDistances[i]/result.DMax, result.ClusterDiversities[i]);
            }
            Console.WriteLine("\nc,Pmax,Dmax,DivL,DivLmax,Xpl%,Xpt%\n{0},{1},{2},{3},{4},{5},{6}\n",
                ClustersCount, result.PMax, result.DMax, result.DivTotal, result.DivMax, result.DivTotal/result.DivMax*100, (result.DivMax-result.DivTotal)/ result.DivMax * 100);
            Console.WriteLine("Our Method,Osuna_Enciso_et_al,Cheng,Salleh_et_al\n{0},{1},{2},{3}", 
                result.DivNorm,
                Diversity_Old.Osuna_Enciso_et_al(permutations),
                Diversity_Old.Cheng(permutations),
                Diversity_Old.Salleh_et_al(permutations));

        }
    }
}
