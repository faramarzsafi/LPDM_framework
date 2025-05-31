using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public static class Factoradic
    {
        static Factoradic()
        {
            int jobsCount = Permutation.JobsCount;
            if (jobsCount <= 0)
            {
                throw new Exception("Invalid Job count");
            }
            Factorial = new BigInteger[jobsCount + 2];
            Factorial[0] = 1;
            for (int i = 1; i <= jobsCount + 1; i++)
                Factorial[i] = Factorial[i - 1] * i;
        }
        public static void Init()
        {

        }
        public static BigInteger[] Factorial;
        public static BigInteger ToInteger(this Permutation permutation)
        {
            int jobsCount = Permutation.JobsCount;
            BigInteger result = 0;
            for (int i = 0; i < jobsCount; ++i)
                result += Factorial[i] * permutation.Jobs[i];
            return result;
        }
        public static int[] ToCoefficients(this BigInteger n, int jobsCount = 0)
        {
            if (jobsCount == 0)
                jobsCount = Permutation.JobsCount;
            if (n > Factorial[jobsCount])
                throw new Exception("Big number!");
            List<int> coefficients = new List<int>();
            //for (int i = 1; i <= jobsCount; i++)
            //{
            //    coefficients.Add((int)(n % i));
            //    n /= i;
            //}
            int i = 1;
            while (i <= jobsCount)
            {
                coefficients.Add((int)(n % i));
                n /= i;
                i++;
            }

            return coefficients.ToArray();
        }
        public static Permutation ToPermutation(this BigInteger n, int jobsCount = 0)
        {
            int[] coefficients = n.ToCoefficients();
            int[] jobs = coefficients.ToPermutation(n);
            return new Permutation(jobs);
        }
        public static int[] perm(int n, int k)
        {
            int i, ind, m = k;
            int[] permuted = new int[n];
            int[] elems = new int[n];

            for (i = 0; i < n; i++) elems[i] = i;

            for (i = 0; i < n; i++)
            {
                ind = m % (n - i);
                m = m / (n - i);
                permuted[i] = elems[ind];
                elems[ind] = elems[n - i - 1];
            }

            return permuted;
        }

        public static int inv(int[] perm)
        {
            int i, k = 0, m = 1;
            int n = perm.Length;
            int[] pos = new int[n];
            int[] elems = new int[n];

            for (i = 0; i < n; i++) { pos[i] = i; elems[i] = i; }

            for (i = 0; i < n - 1; i++)
            {
                k += m * pos[perm[i]];
                m = m * (n - i);
                pos[elems[n - i - 1]] = pos[perm[i]];
                elems[pos[perm[i]]] = elems[n - i - 1];
            }

            return k;
        }
        public static int[] ToPermutation(this int[] coefficients, BigInteger nn)
        {

            int jobsCount = Permutation.JobsCount;
            if (coefficients.Length != jobsCount)
            {
                throw new Exception("X Big number!");
            }
            var jobs = new List<int>(jobsCount);
            for (int i = 0; i < jobsCount; i++)
            {
                jobs.Add(i);
            }
            var coefficientList = new List<int>(coefficients.Reverse());
            var permutationList = new List<int>(jobsCount);
            permutationList.Add(coefficientList[0]);
            jobs.RemoveAt(permutationList[0]);
            coefficientList.RemoveAt(0);
            while (coefficientList.Count > 0)
            {
                int indexOfJob = coefficientList[0];
                int job = jobs[indexOfJob];
                jobs.RemoveAt(indexOfJob);
                coefficientList.RemoveAt(0);
                permutationList.Add(job);
            }
            return permutationList.ToArray();
        }
        public static int[] ToCoefficients(this int[] permutation)
        {

            int jobsCount = Permutation.JobsCount;
            var jobs = new List<int>(jobsCount);
            for (int i = 0; i < jobsCount; i++)
            {
                jobs.Add(i);
            }
            var coefficientList = new List<int>();
            var permutationList = new List<int>(permutation);
            while (permutationList.Count > 0)
            {
                int indexOfJob = jobs.IndexOf(permutationList[0]);
                coefficientList.Add(indexOfJob);
                jobs.RemoveAt(indexOfJob);
                permutationList.RemoveAt(0);
            }
            coefficientList.Reverse();
            return coefficientList.ToArray();
        }
        public static BigInteger ToFactoradic(this Permutation permutation)
        {
            int[] coefficients = permutation.Jobs.ToCoefficients();
            BigInteger result = 0;
            for (int i = 0; i < Permutation.JobsCount; ++i)
                result += Factorial[i] * coefficients[i];
            return result;
        }
        public static BigInteger ToFactoradic(this int[] array, bool isCoefficients=true)
        {
            int[] coefficients;
            if(isCoefficients)
                coefficients = array;
            else
                coefficients = array.ToCoefficients();
            BigInteger result = 0;
            for (int i = 0; i < coefficients.Length; ++i)
                result += Factorial[i] * coefficients[i];
            return result;
        }
    }
}
