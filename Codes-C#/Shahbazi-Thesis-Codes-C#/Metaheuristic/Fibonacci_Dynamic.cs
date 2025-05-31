using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class Fibonacci_Dynamic : Metaheuristic
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
        public Fibonacci_Dynamic(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Fibonacci_Dynamic)
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
        }
        #region F/B 
        List<BigInteger> Move(bool upward, int maxIterationCount = 100, List<BigInteger> result = null)
        {
            //startNumber = start;
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
                item = location + Fibonacci_Numbers[index];

                if (item > endNumber)
                {
                    //if (upwardIndex == 4)
                    //    break;
                    location = lastItem;
                    index -= 2;
                    //upwardIndex = 3;
                    continue;
                }
                result.Add(item);
                lastItem = item;
                count++;
                if (item == endNumber)
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
        List<BigInteger> Forward(int maxIterationCount = 100, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger upwardItem = UpwardLocation;
            BigInteger lastUpwardItem = UpwardLocation;
            int upwardIndex = 1;
            int downwardCount = 0;
            int upwardCount = 0;
            while (downwardCount + upwardCount < maxIterationCount)
            {
                upwardIndex++;
                if (upwardIndex >= Fibonacci_Numbers.Count - 1)
                {
                    UpwardLocation = lastUpwardItem;
                    upwardIndex = 2;
                }
                upwardItem = UpwardLocation + Fibonacci_Numbers[upwardIndex];

                if (upwardItem > endNumber)
                {
                    //if (upwardIndex == 4)
                    //    break;
                    UpwardLocation = lastUpwardItem;
                    upwardIndex -= 2;
                    //upwardIndex = 3;
                    continue;
                }
                result.Add(upwardItem);
                lastUpwardItem = upwardItem;
                upwardCount++;
                if (upwardItem == endNumber)
                {
                    UpwardLocation = upwardItem;
                    break;
                }
            }
            return result;
        }
        List<BigInteger> Backward(int maxIterationCount = 100, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger downwardItem = DownwardLocation;
            BigInteger upwardItem = UpwardLocation;
            BigInteger lastDownwardItem = DownwardLocation;
            int downwardIndex = 1;
            int upwardIndex = 1;
            int downwardCount = 0;
            int upwardCount = 0;
            while (downwardCount + upwardCount < maxIterationCount)
            {
                downwardIndex++;
                if (downwardIndex >= Fibonacci_Numbers.Count - 1)
                {
                    DownwardLocation = lastDownwardItem;
                    downwardIndex = 2;
                }
                downwardItem = DownwardLocation - Fibonacci_Numbers[downwardIndex];

                if (downwardItem < 1)
                {
                    //if (upwardIndex == 4)
                    //    break;
                    DownwardLocation = lastDownwardItem;
                    downwardIndex -= 2;
                    //upwardIndex = 3;
                    continue;
                }
                result.Add(downwardItem);
                lastDownwardItem = downwardItem;
                downwardCount++;
                if (downwardItem == 1)
                {
                    DownwardLocation = downwardItem;
                    break;
                }
            }
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
            //IForward(data.CurrentPermutation.Representation, newItems);
            //if (newItems.Count > 0)
            //    IBackward(newItems.Last(), newItems);
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
            //return newPermutation;
            return null;
        }
        protected override void InitializePopulation(Population r)
        {
            r.CurrentPermutation = new Permutation(3);
        }
    }
}
