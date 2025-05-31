using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Metaheuristic
{
    internal class Diversity_Old
    {
        public struct Result
        {
            public BigInteger ClusterSize;
            public long ClusterCount;
            public double DivNorm;
            public double DivTotal;
            public double DivMax;
            public double DMax;
            public double PMax;
            public double[] ClusterDiversities;
            public double[] ClusterDistances;
            public double[] Clusters;
            public Result(BigInteger clusterSize, long clusterCount)
            {
                ClusterSize = clusterSize;
                ClusterCount = clusterCount;    
                DivNorm = 0;
                DivTotal = 0;
                DivMax = 0;
                DMax = 0;
                PMax = 0;
                ClusterDiversities = new double[clusterCount];
                ClusterDistances = new double[clusterCount];
                Clusters = new double[clusterCount];
            }
        }
        public static Result OurMethod_Old(Permutation[] permutations, BigInteger clusterSize)
        {
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            long clusterCount = (long)(Max / clusterSize);
            if (Max % clusterSize != 0)
                clusterCount++;
            Result result = new Result(clusterSize, clusterCount);
            for (int i = 0; i < permutations.Length; i++)
            {
                BigInteger p = permutations[i].Representation;
                long loc = (long)(p / clusterSize);
                result.Clusters[loc]++;
            }
            for (int i = 0; i < clusterCount; i++)
                if (result.PMax < result.Clusters[i]) result.PMax = result.Clusters[i];
            int[] M = new int[clusterCount];
            int m = 0;
            for (int i = 0; i < clusterCount; i++)
            {
                if (result.Clusters[i] == result.PMax)
                {
                    M[m] = i;
                    m++;
                }
            }
            for (int i = 0; i < clusterCount; i++)
            {
                int d = 0;
                for (int j = 0; j < m; j++)
                    d += Math.Abs(i - M[j]);
                result.ClusterDistances[i] = d;
                if (result.DMax < d) result.DMax = d;
            }
            for (int i = 0; i < clusterCount; i++)
            {
                result.ClusterDiversities[i] = result.Clusters[i] / result.PMax * (1 + result.ClusterDistances[i] / result.DMax);
                if (result.DivMax < result.ClusterDiversities[i]) result.DivMax = result.ClusterDiversities[i];
                result.DivTotal += result.ClusterDiversities[i] / (double)clusterCount;
            }

            result.DivNorm = result.DivTotal / result.DivMax;
            return result;
        }

        public static Result OurMethod(Permutation[] permutations, BigInteger clusterSize, double alpha)
        {
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            long clusterCount = (long)(Max / clusterSize);
            if (Max % clusterSize != 0)
                clusterCount++;
            Result result = new Result(clusterSize, clusterCount);
            for (int i = 0; i < permutations.Length; i++)
            {
                BigInteger p = permutations[i].Representation;
                long loc = (long)(p / clusterSize);
                result.Clusters[loc]++;
            }
            for (int i = 0; i < clusterCount; i++)
                if (result.PMax < result.Clusters[i]) result.PMax = result.Clusters[i];
            int[] M = new int[clusterCount];
            int m = 0;
            for (int i = 0; i < clusterCount; i++)
            {
                if (result.Clusters[i] >= result.PMax * alpha)
                {
                    M[m] = i;
                    m++;
                }
            }
            for (int i = 0; i < clusterCount; i++)
            {
                int d = 0;
                for (int j = 0; j < m; j++)
                    d += Math.Abs(i - M[j]);
                result.ClusterDistances[i] = d;
                if (result.DMax < d) result.DMax = d;
            }
            for (int i = 0; i < clusterCount; i++)
            {
                result.ClusterDiversities[i] = result.Clusters[i] / result.PMax * (1 + result.ClusterDistances[i] / result.DMax);
                if (result.DivMax < result.ClusterDiversities[i]) result.DivMax = result.ClusterDiversities[i];
                result.DivTotal += result.ClusterDiversities[i] / (double)clusterCount;
            }

            result.DivNorm = result.DivTotal / result.DivMax;
            return result;
        }
        class PopLim
        {
            public double max;
            public double min;
            public PopLim()
            {
                max = -1;
                min = Permutation.JobsCount + 1;
            }
        }
        public static double Osuna_Enciso_et_al(Permutation[] permutations)
        {
            double Vlim = 1;
            for (int i = 0; i < Permutation.JobsCount; i++)
            {
                Vlim *= Permutation.JobsCount;
            }
            PopLim[] popLim = new PopLim[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    if (popLim[j] == null) popLim[j] = new PopLim();
                    if (popLim[j].max < jobs[j]) popLim[j].max = jobs[j];
                    if (popLim[j].min > jobs[j]) popLim[j].min = jobs[j];
                }
            }
            double Vpop = 1;
            for (int j = 0; j < Permutation.JobsCount; j++)
            {
                double lim = popLim[j].max - popLim[j].min;
                double q = (lim + 1) / 4;
                Vpop *= 2 * q;
            }

            return (double)Math.Sqrt(Math.Sqrt(Vpop) / Math.Sqrt(Vlim));
        }
        public static double Salleh_et_al(Permutation[] permutations)
        {
            double[] X = new double[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    X[j] += (double)jobs[j] / (double)permutations.Length;
                }
            }
            double[] Median = new double[Permutation.JobsCount];
            double[] Div = new double[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    Div[j] += (double)Math.Abs(jobs[j] - (int)X[j]) / (double)permutations.Length;
                }
            }
            double DivTot = 0;
            double DivMax = 0;
            for (int j = 0; j < Permutation.JobsCount; j++)
            {
                DivTot += Div[j];
                if (DivMax < Div[j]) DivMax = Div[j];
            }

            return DivTot / DivMax / (double)Permutation.JobsCount;
        }

        public static double Cheng(Permutation[] permutations)
        {
            double[] X = new double[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    X[j] += (double)jobs[j] / (double)permutations.Length;
                }
            }

            double[] Div = new double[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    Div[j] += Math.Abs((double)jobs[j] - X[j]) / (double)permutations.Length;
                }
            }
            double DivTot = 0;
            double DivMax = 0;
            for (int j = 0; j < Permutation.JobsCount; j++)
            {
                DivTot += Div[j];
                if (DivMax < Div[j]) DivMax = Div[j];
            }

            return DivTot / DivMax / (double)Permutation.JobsCount;
        }
        public static double Zhao_et_al(Permutation[] permutations)
        {
            BigInteger DivTot = 0;
            for (int i = 0; i < permutations.Length - 1; i++)
            {
                for (int j = i + 1; j < permutations.Length; j++)
                {
                    BigInteger a = permutations[i].Representation;
                    BigInteger b = permutations[j].Representation;
                    if (a > b)
                        DivTot += a - b;
                    else
                        DivTot += b - a;
                }
            }

            return (double)DivTot;
        }
        struct Edge
        {
            public int a;
            public int b;
            public Edge(int a, int b)
            {
                this.a = a;
                this.b = b;
            }
            public override string ToString()
            {
                return String.Format("{0},{1}",a,b);
            }
            public static bool operator ==(Edge e1, Edge e2)
            {
                if (e1.a == e2.a && e1.b == e2.b)
                    return true;
                return false;
            }

            public static bool operator !=(Edge e1, Edge e2)
            {
                return !(e1 == e2);
            }

        }
        public static double Edge_Distances_Div(Permutation[] permutations)
        {
            double sum = 0;
            for (int i = 0; i < permutations.Length - 1; i++)
            {
                for (int j = i + 1; j < permutations.Length; j++)
                {
                    int[] A = permutations[i].Jobs;
                    int[] B = permutations[j].Jobs;
                    Edge[] AEdges = new Edge[Permutation.JobsCount-1];
                    Edge[] BEdges = new Edge[Permutation.JobsCount-1];
                    for (int k = 0; k < Permutation.JobsCount - 1; k++)
                    {
                        AEdges[k] = new Edge(A[k], A[k + 1]);
                        BEdges[k] = new Edge(B[k], B[k + 1]);
                    }
                    for (int k = 0; k < Permutation.JobsCount - 1; k++)
                    {
                        bool foundEdge = false;
                        for (int l = 0; l < Permutation.JobsCount - 1; l++)
                            if (AEdges[k] == BEdges[l])
                            {
                                foundEdge = true;
                                break;
                            }
                        if (!foundEdge)
                            sum++;
                    }
                }
            }

            return sum;
        }
        public static double Wineberg_Oppacher(Permutation[] permutations)
        {
            double sum = 0;
            for (int i = 0; i < permutations.Length; i++)
                for (int j = 0; j < i-1; j++)
                    sum += permutations[i].RealDistanceTo(Permutation.DistanceMeasureType.Hamming, permutations[j]);
            return sum;
        }

        public static double Gaudesi_et_al(Permutation[] permutations)
        {
            /*
             تفاوت متقارن دو جایگشت همواره صفر است. 
             */
            return 0;
        }
        public static double Shi(Permutation[] permutations)
        {
            /*
             این روش مختص الگوریتم 
            Brain Storm Optimization 
            بوده به کلاسترینگ این الگوریتم وابسته است.
             */
            return 0;
        }
        public static double Tilahun(Permutation[] permutations)
        {
            /*
             این روش مختص الگوریتم 
            Prey-Predator 
            بوده به و به میانگین راه حل ها نیاز دارد.
            برخلاف روش هایی که میانگین راه حل ها را بر روی هر بعد مورد استفاده قرار میدهند، در مورد جایگشت ها قابل استفاده نیست.
             */
            return 0;
        }
        public static double Gabor_Belzner(Permutation[] permutations)
        {
            /*
             این روش مختص الگوریتم 
            Prey-Predator 
            بوده به و به میانگین راه حل ها نیاز دارد.
            برخلاف روش هایی که میانگین راه حل ها را بر روی هر بعد مورد استفاده قرار میدهند، در مورد جایگشت ها قابل استفاده نیست.
             */
            return 0;
        }

    }
}
