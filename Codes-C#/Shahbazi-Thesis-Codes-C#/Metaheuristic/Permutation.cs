using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Metaheuristic
{
    public static class FixedSizedQueue
    {
        public static void FixedEnqueue<T>(this ConcurrentQueue<T> queue, T obj)
        {
            queue.Enqueue(obj);

            while (queue.Count > Permutation.MachinesCount)
            {
                T outObj;
                queue.TryDequeue(out outObj);
            }
        }
    }
    public partial class Permutation : IEquatable<Permutation>, IComparable<Permutation>
    {
        public static int[,] Costs;
        public static int[] InitialPermutation;
        public static int[] PermutationMapping;
        public static int JobsCount;
        public static int MachinesCount;
        public int[] Jobs;
        private BigInteger representation;
        public Permutation()
        {
            Jobs = new int[JobsCount];
            representation = -1;
        }
        public Permutation(int[] jobs)
        {
            Jobs = new int[JobsCount];
            Array.Copy(jobs, Jobs, JobsCount);
            representation = -1;
        }
        public static void Init()
        {

        }
        public Permutation(Permutation premutation)
        {
            Jobs = new int[JobsCount];
            Array.Copy(premutation.Jobs, Jobs, JobsCount);
            representation = -1;
        }
        public Permutation(BigInteger representation)
        {
            int[] coefficients = representation.ToCoefficients();
            Jobs = coefficients.ToPermutation(representation);
            this.representation = representation;
        }
        private int Min(int a, int b)
        {
            if (a > b)
                return b;
            return a;
        }
        public BigInteger Representation
        {
            get
            {
                if (representation == -1) 
                    representation = this.ToFactoradic();
                return representation;
            }
            set
            {
                int[] coefficients = value.ToCoefficients();
                Jobs = coefficients.ToPermutation(value);
            }
        }
        public enum DistanceMeasureType
        {
            Linear,
            L1norm,
            L2norm,
            Hamming,
        }

        public BigInteger DistanceTo(DistanceMeasureType measureType, Permutation permutation)
        {
            if (measureType == DistanceMeasureType.Linear)
            {
                BigInteger d = this.Representation - permutation.Representation;
                if (d < 0)
                    return -d;
                return d;
            }
            double distance = 0;
            for (int i = 0; i < JobsCount; i++)
            {
                switch (measureType)
                {
                    case DistanceMeasureType.L1norm:
                        distance += Math.Abs(this.Jobs[i] - permutation.Jobs[i]); break;
                    case DistanceMeasureType.L2norm:
                        distance += (this.Jobs[i] - permutation.Jobs[i]) * (this.Jobs[i] - permutation.Jobs[i]); break;
                }
            }
            switch (measureType)
            {
                case DistanceMeasureType.L1norm:
                    break;
                case DistanceMeasureType.L2norm:
                    distance = Math.Sqrt(distance); break;
            }
            BigInteger bDistance = (BigInteger)distance;
            return bDistance;
        }
        public Double RealDistanceTo(DistanceMeasureType measureType, Permutation permutation, bool signed = false)
        {
            if (measureType == DistanceMeasureType.Linear)
            {
                double d = (double)(this.Representation - permutation.Representation);
                if (signed) return d;
                return Math.Abs(d);
            }
            double distance = 0;
            if (measureType == DistanceMeasureType.Hamming)
            {
                for (int i = 0; i < this.Jobs.Length; i++)
                    if (this.Jobs[i] != permutation.Jobs[i])
                        distance++;
                return distance;
            }
            for (int i = 0; i < JobsCount; i++)
            {
                switch (measureType)
                {
                    case DistanceMeasureType.L1norm:
                        distance += Math.Abs(this.Jobs[i] - permutation.Jobs[i]); break;
                    case DistanceMeasureType.L2norm:
                        distance += (this.Jobs[i] - permutation.Jobs[i]) * (this.Jobs[i] - permutation.Jobs[i]); break;
                }
            }
            if (measureType == DistanceMeasureType.L2norm)
                distance = Math.Sqrt(distance);

            return distance;
        }
        private double cost = -1;
        public double Cost
        {
            get
            {
                if (cost == -1)
                    cost = ComputCost();
                return cost;
            }
        }
        public double ComputCost()
        {
            //return AckleyFCN();
            //return AlpineN1FCN();
            return AckleyN4FCN();
           // return Xin_SheYangN4FCN();
            //return ComputJobSchedulingCost();
        }

        public static Permutation CreateWithExchange(Permutation permutation, int i, int j)
        {
            Permutation p = new Permutation(permutation.Jobs);
            p.Exchange(i, j);
            return p;
        }
        public static Permutation CreateWithInsert(Permutation permutation, int i, int j)
        {
            List<int> newJobs = new List<int>(permutation.Jobs);
            int x = permutation.Jobs[i];
            newJobs.RemoveAt(i);
            newJobs.Insert(j, x);
            Permutation p = new Permutation(newJobs.ToArray());
            return p;
        }
        static Random rand = new Random();
        public static Permutation CreateWithMutation(Permutation permutation)
        {
            int jobsCount=permutation.Jobs.Length;
            int[] newJobs = new int[jobsCount];
            int r = rand.Next(1, jobsCount - 2);
            Array.Copy(permutation.Jobs, r, newJobs, 0, jobsCount-r);
            Array.Copy(permutation.Jobs, 0, newJobs, jobsCount - r, r);
            Permutation p = new Permutation(newJobs);
            return p;
        }
        public void Exchange(int i, int j)
        {
            int x = Jobs[i];
            Jobs[i] = Jobs[j];
            Jobs[j] = x;
        }
        public void Insert(int i, int j)
        {
            List<int> newJobs=new List<int>(Jobs);
            int x = Jobs[i];
            newJobs.RemoveAt(i);
            newJobs.Insert(j, x);
            Jobs = newJobs.ToArray();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("[");
            for (int i = 0; i < JobsCount; i++)
                s.AppendFormat("{0}{1}", (i == 0) ? "" : " ", Jobs[i]);
            s.AppendFormat("] [{0}] ${1}", Representation, Cost);
            return s.ToString();
        }
        
        public static string LBracket = "(";
        public static string RBracket = ")";
        public string ToArrayString(bool bracket = false, string delemeter = ",")
        {
            StringBuilder s = new StringBuilder();
            if (bracket)s.Append(LBracket); 
            for (int i = 0; i < JobsCount; i++)
                s.AppendFormat("{0}{1}", (i == 0) ? "" : delemeter, Jobs[i]);
            if(bracket)s.AppendFormat(RBracket);
            return s.ToString();
        }

        #region IEquatable
        public bool Equals(Permutation other)
        {
            if (other == null)
                return false;
            for (int i = 0; i < JobsCount; i++)
                if (Jobs[i] != other.Jobs[i])
                    return false;
            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Permutation personObj = obj as Permutation;
            if (personObj == null)
                return false;
            else
                return Equals(personObj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (Jobs == null || Jobs.Length == 0)
                {
                    return 0;
                }
                int hash = 17;
                foreach (int element in Jobs)
                {
                    hash = hash * 31 + element;
                }
                return hash;
            }
        }

        public static bool operator ==(Permutation person1, Permutation person2)
        {
            if (((object)person1) == null || ((object)person2) == null)
                return Object.Equals(person1, person2);

            return person1.Equals(person2);
        }

        public static bool operator !=(Permutation person1, Permutation person2)
        {
            if (((object)person1) == null || ((object)person2) == null)
                return !Object.Equals(person1, person2);

            return !(person1.Equals(person2));
        }
        #endregion IEquatable


        #region IComparable
        public int CompareTo(Permutation other)
        {
            return this.Cost.CompareTo(other.Cost);
        }
        #endregion IComparable

        const int COST_RANDOM_SEED = 32000;//(int)DateTime.Now.Ticks
        static int COST_CENTER = 100000;
        static int COST_DIVERGENCE_UP = 90000;
        static int COST_DIVERGENCE_DN = 90000;
        public static Permutation CreadJobs(int jobsCount, int machinesCount, int randomSeed = COST_RANDOM_SEED)
        {
            Random random = new Random(randomSeed);
            Costs = new int[jobsCount, machinesCount];
            List<int> alreadyUsed = new List<int>();
            for (int j = 0; j < machinesCount; j++)
            {
                for (int i = 0; i < jobsCount; i++)
                {
                    int cost;
                    while (true)
                    {
                        cost = COST_CENTER + random.Next(0, COST_DIVERGENCE_UP + COST_DIVERGENCE_DN) - COST_DIVERGENCE_DN;
                        if (!alreadyUsed.Contains(cost))
                        {
                            Costs[i, j] = cost;
                            alreadyUsed.Add(cost);
                            break;
                        }
                    }

                }
            }
            JobsCount = jobsCount;
            MachinesCount = machinesCount;

            InitialPermutation = new int[jobsCount];
            for (int i = 0; i < jobsCount; i++)
                InitialPermutation[i] = i;
            Permutation permutation = new Permutation(InitialPermutation);
            return permutation;
        }
        public static Permutation ReadJobs(string filename, int jobsCount, int machinesCount)
        {
            ////Temp->
            //InitialPermutation = new int[jobsCount];
            //for (int i = 0; i < jobsCount; i++)
            //    InitialPermutation[i] = jobsCount - i - 1;
            //Permutation permutation = new Permutation(InitialPermutation);
            //permutation = new Permutation(new int []{ 4, 3, 2, 1, 0, 9, 8, 7, 6, 5 });
            //return permutation;

            ////<-Temp
            string[] lines = File.ReadAllLines(filename);

            if (lines.Length != machinesCount + 2)
                return null;
            Costs = new int[jobsCount, machinesCount];
            for (int j = 0; j < machinesCount; j++)
            {
                string[] costs = lines[j+2].Split(' ');
                if (costs.Length < jobsCount)
                    return null;
                for (int i = 0; i < jobsCount; i++)
                {
                    int cost;
                    if (!int.TryParse(costs[i], out cost))
                        return null;
                    Costs[i, j] = cost;
                }
            }
            JobsCount = jobsCount;
            MachinesCount = machinesCount;

            InitialPermutation = new int[jobsCount];
            for (int i = 0; i < jobsCount; i++)
                InitialPermutation[i] = -1;

            /* Random premutation */
            for (int i = 0; i < jobsCount; i++)
            {
                int x = rand.Next(0, jobsCount);
                while (InitialPermutation.Contains(x))
                    x = rand.Next(0, jobsCount);
                InitialPermutation[i] = x;
            }

            /* Zero premutation */
            //for (int i = 0; i < jobsCount; i++)
            //{
            //    InitialPermutation[i] = i;
            //}
            Permutation permutation = new Permutation(InitialPermutation);
            return permutation;
        }
        public static double Factorial(int n)
        {
            if (n == 0)
                return 1;
            return n * Factorial(n - 1);
        }

    }
    public enum FinishType
    {
        NotYet,
        Done,
        NoNewPermutation,
        Iteration,
        Time
    }
}
