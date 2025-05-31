using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class Fibonacci_Straight : Metaheuristic
    {
        private List<BigInteger> Fibonacci_Numbers = new List<BigInteger>();
        private Dictionary<BigInteger, int> Fibonacci_Numbers_Index = new Dictionary<BigInteger, int>();
        private Permutation[] Fibonacci_Permutations;
        private int Neighborhood_Size;
        private List<Permutation> BestPermutations = new List<Permutation>();
        BigInteger maxNumber;
        BigInteger startNumber;
        BigInteger endNumber;
        public Fibonacci_Straight(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Fibonacci_Straight)
        {
            Fibonacci_Numbers.Add(0);
            Fibonacci_Numbers_Index[0] = 0;
            Fibonacci_Numbers.Add(1);
            Fibonacci_Numbers_Index[1] = 1;
            Fibonacci_Numbers.Add(2);
            Fibonacci_Numbers_Index[2] = 1;
            int i = 3;
            maxNumber = Factoradic.Factorial[Permutation.JobsCount];
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
            maxNumber = Fibonacci_Numbers[i];
            endNumber = maxNumber;
            startNumber = 1;
            Neighborhood_Size = i;
            Fibonacci_Permutations = new Permutation[Neighborhood_Size];
            for (i = 0; i < Neighborhood_Size; i++)
                Fibonacci_Permutations[i] = new Permutation(Fibonacci_Numbers[i]);
        }
        #region F/B 
        List<BigInteger> Forward(BigInteger start, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 0;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber)
                    break;
                index++;
                result.Add(cItem);
            }
            for (int i = index; i >= 1; i--)
                if (result[result.Count - 1] <= startNumber)
                    result.RemoveAt(result.Count - 1);
                else
                {
                    startNumber = result[result.Count - 1];
                    break;
                }
            return result;
        }
        #endregion F/B 

        #region F/B 6
        List<BigInteger> Forward6(BigInteger start, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward6(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 0;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber)
                    break;
                index++;
                result.Add(cItem);
            }
            if (index > 0)
                result.RemoveAt(result.Count - 1);
            if (index > 1)
                startNumber = result[result.Count - 1];
            return result;
        }
        #endregion F/B 6

        #region F/B 5
        List<BigInteger> Forward5(BigInteger start, List<BigInteger> result = null)
        {
            startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward5(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber + 1)
                    break;
                result.Add(cItem);
                index++;
            }
            result.RemoveAt(result.Count - 1);
            return result;
        }
        #endregion F/B 5

        #region F/B 4
        List<BigInteger> Forward4(BigInteger start, List<BigInteger> result = null)
        {
            startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward4(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber + 3)
                    break;
                result.Add(cItem);
                index++;
            }
            result.RemoveAt(result.Count - 1);
            return result;
        }
        #endregion F/B 4

        #region F/B 3
        List<BigInteger> Forward3(BigInteger start, List<BigInteger> result = null)
        {
            startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward3(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber + 10)
                    break;
                result.Add(cItem);
                index++;
            }
            result.RemoveAt(result.Count - 1);
            return result;
        }
        #endregion F/B 3

        #region F/B 2
        List<BigInteger> Forward2(BigInteger start, List<BigInteger> result = null)
        {
            startNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            return result;
        }
        List<BigInteger> Backward2(BigInteger start, List<BigInteger> result = null)
        {
            endNumber = start;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber)
                    break;
                result.Add(cItem);
                index++;
            }
            result.RemoveAt(result.Count - 1);
            return result;
        }
        #endregion F/B 2

        #region F/B 1

        List<BigInteger> Forward1(BigInteger start, List<BigInteger> result = null)
        {
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int startIndex = 0;
            while (Fibonacci_Numbers[startIndex] < start) startIndex++;
            var temp = new List<BigInteger>();
            for (int index = 1; index < startIndex - 1; index++)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem > maxNumber)
                {
                    bool b = Fibonacci_Numbers[index] + Fibonacci_Numbers[index - 1] > maxNumber;
                    int a = 0;
                }
                result.Add(cItem);
                temp.Add(cItem);
            }
            return result;
        }
        List<BigInteger> Backward1(BigInteger start, List<BigInteger> result = null)
        {
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            int startIndex = 0;
            while (Fibonacci_Numbers[startIndex] < start) startIndex++;
            for (int index = startIndex - 2; index >= 0; index--)
            {
                cItem = start - Fibonacci_Numbers[index];
                result.Add(cItem);
            }
            return result;
        }
        #endregion F/B 1

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
            Forward(data.CurrentPermutation.Representation, newItems);
            if (newItems.Count > 0)
                Backward(newItems.Last(), newItems);
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
            return newPermutation;
        }
        protected override void InitializePopulation(Population r)
        {
            r.CurrentPermutation = new Permutation(3);
        }
    }
}
