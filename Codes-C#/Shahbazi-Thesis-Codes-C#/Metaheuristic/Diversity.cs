using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Diagnostics;
using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace Metaheuristic
{
    internal class DiversityData
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
        public DiversityData()
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
    public struct ComputeResult
    {
        public double diversity;
        public long elapsedtime;
    }
    public abstract class Diversity
    {
        protected double div;
        public double Div { get { return div; } }
        protected string name;
        public virtual string Name { get { return name; } set { name = value; } }
        public abstract double Compute(Permutation[] permutations);
        public ComputeResult AsyncComputeResult = new ComputeResult();
        public void AsyncCompute(Permutation[] permutations)
        {
            AsyncComputeResult = new ComputeResult();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            AsyncComputeResult.diversity = Compute(permutations);
            stopwatch.Stop();
            AsyncComputeResult.elapsedtime = stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
        }

    }
    //public class OurMethod : Diversity
    //{
    //    public long ClusterCount;
    //    public double DivNorm;
    //    public double DivTotal;
    //    public double DivMax;
    //    public double DMax;
    //    public double PMax;
    //    public double[] ClusterDiversities;
    //    public double[] ClusterDistances;
    //    Dictionary<int[], double> Clusters;
    //    public double Alpha;
    //    public override string Name { get { return string.Format("Ours-{0}", ClusterCount); } }
    //    public OurMethod()
    //    {
    //        name = "Our Method";
    //        Clusters = new Dictionary<int[], double>();
    //        for (int i = 0; i < Permutation.JobsCount; i++)
    //            for (int j = 0; j < Permutation.JobsCount - 1; j++)
    //                Clusters[new int[] { i, j }] = 0;
    //        ClusterCount = Clusters.Count();
    //        BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
    //        ClusterSize = Max / ClusterCount;
    //        if (Max % ClusterCount != 0)
    //            ClusterCount++;
    //        Alpha = 1;
    //    }
    //    public override double Compute(Permutation[] permutations)
    //    {
    //        Stopwatch s = new Stopwatch();
    //        s.Start();
    //        DivNorm = 0;
    //        DivTotal = 0;
    //        DivMax = 0;
    //        DMax = 0;
    //        PMax = 0;
    //        ClusterDiversities = new double[ClusterCount];
    //        ClusterDistances = new double[ClusterCount];
    //        Clusters.Clear();
    //        for (int i = 0; i < permutations.Length; i++)
    //        {
    //            Clusters[new int[] { permutations[i].Jobs[0], permutations[i].Jobs[1] }]++;
    //        }
    //        for (int i = 0; i < ClusterCount; i++)
    //            if (PMax < Clusters[i]) PMax = Clusters[i];
    //        int[] M = new int[ClusterCount];
    //        int m = 0;
    //        for (int i = 0; i < ClusterCount; i++)
    //        {
    //            if (Clusters[i] >= PMax * Alpha)
    //            {
    //                M[m] = i;
    //                m++;
    //            }
    //        }
    //        for (int i = 0; i < ClusterCount; i++)
    //        {
    //            int d = 0;
    //            for (int j = 0; j < m; j++)
    //                d += Math.Abs(i - M[j]);
    //            ClusterDistances[i] = d;
    //            if (DMax < d) DMax = d;
    //        }
    //        for (int i = 0; i < ClusterCount; i++)
    //        {
    //            ClusterDiversities[i] = Clusters[i] / PMax * (1 + ClusterDistances[i] / DMax);
    //            if (DivMax < ClusterDiversities[i]) DivMax = ClusterDiversities[i];
    //            DivTotal += ClusterDiversities[i] / (double)ClusterCount;
    //        }

    //        DivNorm = DivTotal / ClusterCount;
    //        s.Stop();
    //        long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
    //        return DivNorm;
    //    }
    //}
    public class Dummy : Diversity
    {
        public static int dummyCount = 0;
        public Dummy()
        {
            dummyCount++;
            name = String.Format("Dummy{0}", dummyCount);
        }
        public override double Compute(Permutation[] permutations)
        {
            return 0;
        }
    }
    public class DesiredDiversity : Diversity
    {
        public DesiredDiversity(int jobsCount, BigInteger spaceMax, int populationCount, int diversityRatio, int modality, Permutation[] optimas)
        {
            name = "Desired Diversity";
            JobsCount = jobsCount;
            PopulationCount = populationCount;
            DiversityRatio = diversityRatio;
            Modality = modality;
            Optimas = optimas;
            SpaceMax = spaceMax;
        }

        public int JobsCount { get; }
        public int PopulationCount { get; }
        public int DiversityRatio { get; }
        public int Modality { get; }
        public Permutation[] Optimas { get; }
        public Permutation[] Population { get; }
        public BigInteger SpaceMax { get; }

        public Permutation[] generateSingleFlatPopulation()
        {
            Permutation[] permutations = new Permutation[PopulationCount * Optimas.Length];
            for (int i = 0; i < Optimas.Length; i++)
            {
                Permutation optima = Optimas[i];
                BigInteger optimaRightMargin = (SpaceMax - optima.Representation) * DiversityRatio / 100;
                BigInteger optimaLeftMargin = (optima.Representation) * DiversityRatio / 100;
                BigInteger flatDistance = (optimaRightMargin - optimaLeftMargin) / PopulationCount;
                BigInteger individual = optimaLeftMargin;
                for (int j = 0; j < PopulationCount; j++)
                {
                    individual += flatDistance;
                    Permutation permutation = new Permutation(individual);
                    permutations[i * PopulationCount + j] = permutation;
                }

            }
            return permutations;
        }

        public override double Compute(Permutation[] permutations)
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
            div = (double)DivTot;
            return div;
        }
    }
    public class OurMethod : Diversity
    {
        public long ClusterCount;
        public double DivNorm;
        public double DivTotal;
        public double DivMax;
        public double DMax;
        public double PMax;
        public double[] ClusterDiversities;
        public double[] ClusterDistances;
        public double[] Clusters;
        Dictionary<long, long> ClusterLocation;
        public double Alpha;
        private bool nameIncludesClusterCount = false;
        public override string Name { 
            get 
            { 
                if (nameIncludesClusterCount)
                    return string.Format("Ours-{0}", ClusterCount);
                return name;
            }
        }
        public OurMethod(bool nameIncludesClusterCount = false)
        {
            this.nameIncludesClusterCount = nameIncludesClusterCount;
            name = "Ours";
            ClusterLocation = new Dictionary<long, long>();
            ClusterCount = 0;
            for (int i = 0; i < Permutation.JobsCount; i++)
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    if (i == j) continue;
                    ClusterLocation[i * Permutation.JobsCount + j] = ClusterCount;
                    ClusterCount++;
                }
            Alpha = 1;
        }
        public override double Compute(Permutation[] permutations)
        {
            DivNorm = 0;
            DivTotal = 0;
            DivMax = 0;
            DMax = 0;
            PMax = 0;
            ClusterDiversities = new double[ClusterCount];
            ClusterDistances = new double[ClusterCount];
            Clusters = new double[ClusterCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                Clusters[ClusterLocation[permutations[i].Jobs[0] * Permutation.JobsCount + permutations[i].Jobs[1]]]++;
            }
            for (int i = 0; i < ClusterCount; i++)
                if (PMax < Clusters[i]) PMax = Clusters[i];
            int[] M = new int[ClusterCount];
            int m = 0;
            for (int i = 0; i < ClusterCount; i++)
            {
                if (Clusters[i] >= PMax * Alpha)
                {
                    M[m] = i;
                    m++;
                }
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                int d = 0;
                for (int j = 0; j < m; j++)
                    d += Math.Abs(i - M[j]);
                ClusterDistances[i] = d;
                if (DMax < d) DMax = d;
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                ClusterDiversities[i] = Clusters[i] / PMax * (1 + ClusterDistances[i] / DMax);
                if (DivMax < ClusterDiversities[i]) DivMax = ClusterDiversities[i];
                DivTotal += ClusterDiversities[i] / (double)ClusterCount;
            }

            DivNorm = DivTotal / ClusterCount;
            return DivNorm;
        }
    }
    public class OurMethod2 : Diversity
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
        public double Alpha;
        public override string Name { get { return string.Format("Ours-{0}", ClusterCount); } }
        public OurMethod2()
        {
            name = "Our Method";
            ClusterCount = Permutation.JobsCount;
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClusterSize = Max / ClusterCount;
            if (Max % ClusterCount != 0)
                ClusterCount++;
            Alpha = 1;
        }
        public OurMethod2(long clusterCount, double alpha)
        {
            name = "Our Method";
            ClusterCount = clusterCount;
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClusterSize = Max / ClusterCount;
            if (Max % ClusterCount != 0)
                ClusterCount++;
            Alpha = alpha;
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            DivNorm = 0;
            DivTotal = 0;
            DivMax = 0;
            DMax = 0;
            PMax = 0;
            ClusterDiversities = new double[ClusterCount];
            ClusterDistances = new double[ClusterCount];
            Clusters = new double[ClusterCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                Clusters[permutations[i].Jobs[0]]++;
            }
            for (int i = 0; i < ClusterCount; i++)
                if (PMax < Clusters[i]) PMax = Clusters[i];
            int[] M = new int[ClusterCount];
            int m = 0;
            for (int i = 0; i < ClusterCount; i++)
            {
                if (Clusters[i] >= PMax * Alpha)
                {
                    M[m] = i;
                    m++;
                }
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                int d = 0;
                for (int j = 0; j < m; j++)
                    d += Math.Abs(i - M[j]);
                ClusterDistances[i] = d;
                if (DMax < d) DMax = d;
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                ClusterDiversities[i] = Clusters[i] / PMax * (1 + ClusterDistances[i] / DMax);
                if (DivMax < ClusterDiversities[i]) DivMax = ClusterDiversities[i];
                DivTotal += ClusterDiversities[i] / (double)ClusterCount;
            }

            DivNorm = DivTotal / ClusterCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return DivNorm;
        }
    }
    public class OurMethod_FreeClusterSize : Diversity
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
        public double Alpha;
        public override string Name { get
            {
                if (name == "Ours")
                    return string.Format("Ours-{0}", ClusterCount);
                return name;
            } 
        }
        public OurMethod_FreeClusterSize()
        {
            name = "Ours";
            ClusterCount = Permutation.JobsCount;
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClusterSize = Max / ClusterCount;
            if (Max % ClusterCount != 0)
                ClusterCount++;
            Alpha = 1;
        }
        public OurMethod_FreeClusterSize(long clusterCount, double alpha)
        {
            this.name = "Ours";
            ClusterCount = clusterCount;
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClusterSize = Max / ClusterCount;
            if (Max % ClusterCount != 0)
                ClusterCount++;
            Alpha = alpha;
        }
        public OurMethod_FreeClusterSize(long clusterCount, string name = "Ours")
        {
            this.name = name;
            ClusterCount = clusterCount;
            BigInteger Max = Factoradic.Factorial[Permutation.JobsCount];
            ClusterSize = Max / ClusterCount;
            if (Max % ClusterCount != 0)
                ClusterCount++;
            Alpha = 1;
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            DivNorm = 0;
            DivTotal = 0;
            DivMax = 0;
            DMax = 0;
            PMax = 0;
            ClusterDiversities = new double[ClusterCount];
            ClusterDistances = new double[ClusterCount];
            Clusters = new double[ClusterCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                BigInteger p = permutations[i].Representation;
                long loc = (long)(p / ClusterSize);
                if (loc >= ClusterCount)
                    loc = ClusterCount - 1;
                Clusters[loc]++;
            }
            for (int i = 0; i < ClusterCount; i++)
                if (PMax < Clusters[i]) PMax = Clusters[i];
            int[] M = new int[ClusterCount];
            int m = 0;
            for (int i = 0; i < ClusterCount; i++)
            {
                if (Clusters[i] >= PMax * Alpha)
                {
                    M[m] = i;
                    m++;
                }
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                int d = 0;
                for (int j = 0; j < m; j++)
                    d += Math.Abs(i - M[j]);
                ClusterDistances[i] = d;
                if (DMax < d) DMax = d;
            }
            for (int i = 0; i < ClusterCount; i++)
            {
                ClusterDiversities[i] = Clusters[i] / PMax * (1 + ClusterDistances[i] / DMax);
                if (DivMax < ClusterDiversities[i]) DivMax = ClusterDiversities[i];
                DivTotal += ClusterDiversities[i] / (double)ClusterCount;
            }

            DivNorm = DivTotal / ClusterCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return DivNorm;
        }
    }
    public class Ursem : Diversity//DM1[34]
    {
        public Ursem()
        {
            name = "Ursem";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            double L = Math.Sqrt(Permutation.JobsCount * Permutation.JobsCount * Permutation.JobsCount);
            double[] Sm = new double[Permutation.JobsCount];
            
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    Sm[j] += (double)jobs[j];
                }
            }
            for (int j = 0; j < Permutation.JobsCount; j++)
            {
                Sm[j] /= permutations.Length;
            }
            double DivTot = 0;
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                double t = 0;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                   t += ((double)jobs[j] - Sm[j]) * ((double)jobs[j] - Sm[j]);
                }
                DivTot += Math.Sqrt(t);
            }

            double div = DivTot / L / (double)Permutation.JobsCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Wineberg_Oppacher : Diversity//DM2[35]
    {
        public Wineberg_Oppacher()
        {
            name = "Wineberg and Oppacher";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            double sum = 0;
            for (int i = 0; i < permutations.Length; i++)
                for (int j = 0; j < i - 1; j++)
                    sum += permutations[i].RealDistanceTo(Permutation.DistanceMeasureType.Hamming, permutations[j]);
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return sum;
        }
    }
    public class Zhu_et_al : Diversity//DM3[36]
    {
        public Zhu_et_al()
        {
            name = "Zhu et al.";
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
                return String.Format("{0},{1}", a, b);
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
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            double sum = 0;
            for (int i = 0; i < permutations.Length - 1; i++)
            {
                for (int j = i + 1; j < permutations.Length; j++)
                {
                    int[] A = permutations[i].Jobs;
                    int[] B = permutations[j].Jobs;
                    Edge[] AEdges = new Edge[Permutation.JobsCount - 1];
                    Edge[] BEdges = new Edge[Permutation.JobsCount - 1];
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

            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return sum;
        }
    }
    public class Shi : Diversity//DM4[37]
    {
        public Shi()
        {
            name = "Shi";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            double DivTot = 0;
            for (int i = 0; i < permutations.Length; i++)
            {
                for (int j = i+1; j < permutations.Length; j++)
                {
                    int[] Xi = permutations[i].Jobs;
                    int[] Xj = permutations[j].Jobs;
                    double sum = 0;
                    for (int k = 0; k < Permutation.JobsCount; k++)
                    {
                        sum += (Xi[k] - Xj[k]) * (Xi[k] - Xj[k]);
                    }
                    DivTot += Math.Sqrt(sum);
                }
            }
            double L = Math.Sqrt(Permutation.JobsCount * Permutation.JobsCount * Permutation.JobsCount);

            double div = 2 * DivTot / (double)(permutations.Length * (permutations.Length - 1) * L);
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Shi_Dv : Diversity//DM4[37]
    {
        public long ClusterCount;
        public double[] ClusterDiversities;
        public double[] ClusterDistances;
        public double[] Clusters;
        Dictionary<long, long> ClusterLocation;
        public Shi_Dv()
        {
            name = "Shi";
            ClusterLocation = new Dictionary<long, long>();
            ClusterCount = 0;
            for (int i = 0; i < Permutation.JobsCount; i++)
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    if (i == j) continue;
                    ClusterLocation[i * Permutation.JobsCount + j] = ClusterCount;
                    ClusterCount++;
                }
            Clusters = new double[ClusterCount];
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < permutations.Length; i++)
            {
                Clusters[ClusterLocation[permutations[i].Jobs[0] * Permutation.JobsCount + permutations[i].Jobs[1]]]++;
            }
            double n_m = (double)permutations.Length / ClusterCount;
            double DivTot = 0;
            foreach (double n_i in Clusters)
            {

                DivTot += (double)((n_i - n_m) * (n_i - n_m));
            }
            double div = DivTot / ClusterCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Wang_et_al : Diversity//DM5[38]
    {
        public Wang_et_al()
        {
            name = "Wang et al.";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            double DivTot = 0;
            for (int i = 0; i < permutations.Length-1; i++)
            {
                for (int j = i+1; j < permutations.Length; j++)
                {
                    int[] Xi = permutations[i].Jobs;
                    int[] Xj = permutations[j].Jobs;
                    double sign = 0;
                    for (int k = 0; k < Permutation.JobsCount; k++)
                    {
                        if (Xi[k] != Xj[k])
                            sign++;
                    }
                    DivTot += sign;
                }
            }

            double div = 2 * DivTot / (permutations.Length * (permutations.Length - 1) * (double)Permutation.JobsCount);
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Li_et_al : Ursem//DM6[39]
    {
        public Li_et_al()
        {
            name = "Li et al.";
        }
        public override double Compute(Permutation[] permutations)
        {
            return base.Compute(permutations);
            Stopwatch s = new Stopwatch();
            s.Start();
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

            double div = DivTot / DivMax / (double)Permutation.JobsCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Cheng : Diversity//DM7[40]
    {
        public Cheng()
        {
            name = "Cheng";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
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

            double div = DivTot / DivMax / (double)Permutation.JobsCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Tilahun : Diversity//DM8[41]
    {
        public Tilahun()
        {
            name = "Tilahun";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            double[] Xc = new double[Permutation.JobsCount];
            for (int i = 0; i < permutations.Length; i++)
            {
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    Xc[j] += (double)jobs[j] / (double)permutations.Length;
                }
            }

            double DivTot = 0;
            for (int i = 0; i < permutations.Length; i++)
            {
                double sum = 0;
                int[] jobs = permutations[i].Jobs;
                for (int j = 0; j < Permutation.JobsCount; j++)
                {
                    sum += ((double)jobs[j] - Xc[j]) * ((double)jobs[j] - Xc[j]);
                }
                DivTot += Math.Sqrt(sum);
            }
            double L = Math.Sqrt(Permutation.JobsCount * Permutation.JobsCount * Permutation.JobsCount);
            double div = DivTot / (L * (double)Permutation.JobsCount);
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Salleh_et_al : Diversity//DM9[15]
    {
        public Salleh_et_al()
        {
            name = "Salleh et al.";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
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

            double div = DivTot / DivMax / (double)Permutation.JobsCount;
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Zhao_et_al : Diversity//DM10[42]
    {
        public Zhao_et_al()
        {
            name = "Zhao et al.";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
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
            div = (double)DivTot;
            s.Stop();
            long t = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    public class Osuna_Enciso_et_al : Diversity//DM11[43]
    {
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
        public Osuna_Enciso_et_al()
        {
            name = "Osuna Enciso et al.";
        }
        public override double Compute(Permutation[] permutations)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
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

            double div = (double)Math.Sqrt(Math.Sqrt(Vpop) / Math.Sqrt(Vlim));
            s.Stop();
            long a = s.ElapsedTicks * 1000000 / Stopwatch.Frequency;
            return div;
        }
    }
    internal class Diversity_1
    {


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
             
             */
            return 0;
        }

    }
}
