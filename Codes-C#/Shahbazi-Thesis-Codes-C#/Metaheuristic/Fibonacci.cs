using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class Fibonacci : Metaheuristic
    {
        private List<BigInteger> _Fibonacci_Numbers=new List<BigInteger>();
        private Permutation[] _Fibonacci_Permutations;
        private long _Neighborhood_Size;
        public Fibonacci(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Fibonacci)
        {
            _Fibonacci_Numbers.Add(1);
            _Fibonacci_Numbers.Add(1);
            int i = 2;
            while (_Fibonacci_Numbers[i - 1] + _Fibonacci_Numbers[i - 2] < Factoradic.Factorial[Permutation.JobsCount])
            {
                _Fibonacci_Numbers.Add(_Fibonacci_Numbers[i - 1] + _Fibonacci_Numbers[i - 2]);
                i++;
            }
            _Neighborhood_Size = _Fibonacci_Numbers.Count;
            _Fibonacci_Permutations = new Permutation[_Neighborhood_Size];
            for (i = 0; i < _Neighborhood_Size; i++)
                _Fibonacci_Permutations[i] = new Permutation(_Fibonacci_Numbers[i]-1);
        }
        protected override List<Permutation> GeneratePopulation(Population data)
        {
            ////With mapping
            //data.NeighborhoodPermutations = new List<Permutation>();
            //for (int i = 0; i < _Neighborhood_Size; i++)
            //{
            //    int[] _Jobs = new int[Permutation.JobsCount];
            //    for (int j = 0; j < data.JobsCount; j++)
            //        _Jobs[j] = data.CurrentPermutation.Jobs[_Fibonacci_Permutations[i].Jobs[j]];
            //    Permutation _New_Permutation = new Permutation(_Jobs);
            //    if (_New_Permutation != data.CurrentPermutation)
            //        data.NeighborhoodPermutations.Add(_New_Permutation);
            //}
            //return data.NeighborhoodPermutations;

            //Without mapping
            data.Permutations = new List<Permutation>();
            BigInteger currentPermutation = data.CurrentPermutation.Representation;
            if (currentPermutation > 4)
                _Neighborhood_Size = 2 * _Neighborhood_Size - 1;
            for (int i = 0; i < _Neighborhood_Size; i++)
            {
                Permutation _New_Permutation = new Permutation(currentPermutation + i);
                data.Permutations.Add(_New_Permutation);
            }
            return data.Permutations;

            //for (int i = 0; i < r.JobsCount; i++)
            //{
            //    if (_1_permutation == null)
            //    {
            //        _1_permutation = r.CurrentPermutation;
            //        _2_permutation = r.CurrentPermutation;
            //    }
            //    BigInteger b1 = _1_permutation.ToFactoradic() + _2_permutation.ToFactoradic();
            //    BigInteger b = _1_permutation.Representation + _2_permutation.Representation;
            //    _2_permutation = new Permutation(_1_permutation);
            //    try
            //    {
            //        _1_permutation = new Permutation(b);
            //    }
            //    catch (Exception e)
            //    {
            //        break;
            //    }
            //    r.NeighborhoodPermutations.Add(_1_permutation);
            //}
            //return r.NeighborhoodPermutations;
        }
        protected Permutation Mutation(Population data)
        {
            Permutation max = data.CurrentPermutation;
            for(int i =0; i<data.Permutations.Count; i++)
                if(data.Permutations[i].Representation > max.Representation)
                    max = data.Permutations[i];
            return max;
        }
        protected override Permutation FindTheBestInPopulation(Population data)
        {
            PopulationBestMember member;
            data.Permutations.Sort();
            data.RefreshHistory();
            int len = data.Permutations.Count > data.LiveTimes ? data.LiveTimes : data.Permutations.Count;
            for (int i = 0; i < len; i++)
            {
                member = data.CheckHistory(data.Permutations[i]);
                if (member != null)
                {
                    data.CurrentPermutation = member.Permutation;
                    return data.CurrentPermutation;
                }
            }
            return Mutation(data);
        }
        protected Permutation FindTheLastInNeighborhood(Population data)
        {
            Permutation newCurrentPermutation=null;
            if (data.Permutations.Count > 0)
                newCurrentPermutation = data.Permutations.Last();
            PopulationBestMember member;
            data.Permutations.Sort();
            data.RefreshHistory();
            int len = data.Permutations.Count > data.LiveTimes ? data.LiveTimes : data.Permutations.Count;
            for (int i = 0; i < len; i++)
            {
                member = data.CheckHistory(data.Permutations[i]);
                if (member != null)
                {
                    data.CurrentPermutation = member.Permutation;
                    break;
                }
            }
            data.CurrentPermutation = newCurrentPermutation;
            return data.CurrentPermutation;
        }

        protected override  Permutation SelectNewMove(Population data)
        {
            Permutation newPermutation = FindTheBestInPopulation(data);
            return newPermutation;
        }
        protected override  Permutation EvaluatePopulation(Population data)
        {
            GeneratePopulation(data);
            Permutation newPermutation = SelectNewMove(data);
            if (newPermutation == null)
                return null;
            UpdateGeneratedPermutations(data);
            return newPermutation;
        }
    }
}
