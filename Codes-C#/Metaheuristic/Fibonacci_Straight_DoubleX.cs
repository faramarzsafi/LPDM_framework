using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class Fibonacci_Straight_DoubleX : Metaheuristic
    {
        private List<BigInteger> Fibonacci_Numbers = new List<BigInteger>();
        private Dictionary<BigInteger, int> Fibonacci_Numbers_Index = new Dictionary<BigInteger, int>();
        private Permutation[] Fibonacci_Permutations;
        private int Neighborhood_Size;
        private List<Permutation> BestPermutations = new List<Permutation>();
        BigInteger maxNumber;
        BigInteger startNumber;
        BigInteger endNumber;
        BigInteger UpwardLocation;
        BigInteger DownwardLocation;
        public Fibonacci_Straight_DoubleX(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Fibonacci_Straight_DoubleX)
        {
            Fibonacci_Numbers.Add(0);
            Fibonacci_Numbers_Index[0] = 0;
            Fibonacci_Numbers.Add(1);
            Fibonacci_Numbers_Index[1] = 1;
            Fibonacci_Numbers.Add(2);
            Fibonacci_Numbers_Index[2] = 1;
            int i = 3;
            maxNumber = Factoradic.Factorial[Permutation.JobsCount]-1;
            while (true)
            {
                BigInteger item = Fibonacci_Numbers[i - 1] + Fibonacci_Numbers[i - 2];
                if (item >= maxNumber)
                    break;
                Fibonacci_Numbers.Add(item);
                Fibonacci_Numbers_Index[item] = i;
                i++;
            }
            i--;
            //maxNumber = Fibonacci_Numbers[i];
            endNumber = maxNumber;
            startNumber = 1;
            Neighborhood_Size = i;
            Fibonacci_Permutations = new Permutation[Neighborhood_Size];
            for (i = 0; i < Neighborhood_Size; i++)
                Fibonacci_Permutations[i] = new Permutation(Fibonacci_Numbers[i]);
            UpwardLocation = startNumber;
            DownwardLocation = maxNumber;
        }
        #region F/B 
        List<BigInteger> Move(bool upward, int maxIterationCount = 100, List<BigInteger> result = null)
        {
            if (result == null) result = new List<BigInteger>();
            BigInteger location;
            if (upward)
                location = UpwardLocation;
            else
                location = DownwardLocation;
            BigInteger item = location;
            BigInteger lastItem = location;
            int index = 1;
            int count = 0;
            while (count < maxIterationCount)
            {
                index++;
                if (index >= Fibonacci_Numbers.Count - 1)
                {
                    location = lastItem;
                    index = 2;
                }
                if(upward)
                    item = location + Fibonacci_Numbers[index];
                else
                    item = location - Fibonacci_Numbers[index];

                if (upward && item > endNumber || !upward && item < 1)
                {
                    location = lastItem;
                    index -= 2;

                    if (index < 1)
                    {
                        continue;
                    }
                    continue;
                }
                result.Add(item);
                lastItem = item;
                count++;
                if (upward && item == endNumber || !upward && item == 1)
                {
                    location = item;
                    break;
                }
            }
            if (upward)
                UpwardLocation = location;
            else
                DownwardLocation = location;
            return result;
        }
        #endregion F/B 

        static void Print(List<int> items)
        {
            foreach (int item in items)
                Console.Write(" ", item);
        }
        bool forward = false;
        protected override List<Permutation> GeneratePopulation(Population data)
        {
            BestPermutations.Add(data.CurrentPermutation);
            List<BigInteger> newItems = new List<BigInteger>();
            Move(true, 10000, newItems);
            Move(false, 10000, newItems);

            data.Permutations = new List<Permutation>();
            for (int i = 0; i < newItems.Count; i++)
            {
                data.Permutations.Add(new Permutation(newItems[i]));
            }
            return data.Permutations;
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
            //List<Permutation> NeighborhoodPermutations = new List<Permutation>(data.NeighborhoodPermutations);
            //NeighborhoodPermutations.Sort();
            data.RefreshHistory();
            int len = data.Permutations.Count > data.LiveTimes ? data.LiveTimes : data.Permutations.Count;
            //for (int i = 0; i < len; i++)
            //{
            //    tabuItem = data.CheckTabuList(NeighborhoodPermutations[i]);
            //    if (tabuItem != null)
            //    {
            //        data.CurrentPermutation = tabuItem.Permutation;
            //        return data.CurrentPermutation;
            //    }
            //}
            return Mutation(data);
        }
        protected Permutation FindTheLastInNeighborhood(Population data)
        {
            Permutation newCurrentPermutation = null;
            if (data.Permutations.Count > 0)
                newCurrentPermutation = data.Permutations.Last();
            if (newCurrentPermutation == null)
                return null;
            data.CurrentPermutation = newCurrentPermutation;
            return data.CurrentPermutation;
        }

        protected override  Permutation SelectNewMove(Population data)
        {
            Permutation newPermutation = FindTheLastInNeighborhood(data);//FindTheLastInNeighborhood(data);
            return newPermutation;
        }
        protected override  Permutation EvaluatePopulation(Population data)
        {
            GeneratePopulation(data);
            Permutation newPermutation = SelectNewMove(data);
            if (newPermutation == null)
                return null;
            UpdateGeneratedPermutations(data);
            return null;
        }
        protected override void InitializePopulation(Population r)
        {
            r.CurrentPermutation = new Permutation(3);
        }
    }
}
