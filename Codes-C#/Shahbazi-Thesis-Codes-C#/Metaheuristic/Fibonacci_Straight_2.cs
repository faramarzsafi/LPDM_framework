using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class Fibonacci_Straight_2 : Metaheuristic
    {
        private List<BigInteger> Fibonacci_Numbers = new List<BigInteger>();
        private Dictionary<BigInteger, int> Fibonacci_Numbers_Index = new Dictionary<BigInteger, int>();
        private Permutation[] Fibonacci_Permutations;
        private int Neighborhood_Size;
        private List<Permutation> BestPermutations = new List<Permutation>();
        BigInteger maxNumber;
        BigInteger startNumber;
        BigInteger endNumber;
        BigInteger line1_pos;
        BigInteger line2_pos;
        public Fibonacci_Straight_2(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Fibonacci_Straight_2)
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
            line1_pos = startNumber;
            line2_pos = endNumber;
            Neighborhood_Size = i;
            Fibonacci_Permutations = new Permutation[Neighborhood_Size];
            for (i = 0; i < Neighborhood_Size; i++)
                Fibonacci_Permutations[i] = new Permutation(Fibonacci_Numbers[i]);
        }
        BigInteger FindNeighbors(BigInteger start, bool forward, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (start < 0)
                return -1;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            BigInteger lastItem = -1;
            int index = 1;
            while (index < Fibonacci_Numbers.Count)
            {
                if (forward)
                {
                    cItem = start + Fibonacci_Numbers[index];
                    if (cItem >= endNumber)
                        break;
                }
                else
                {
                    cItem = start - Fibonacci_Numbers[index];
                    if (cItem <= startNumber)
                        break;
                }
                result.Add(cItem);
                lastItem = cItem;
                index++;
            }
            return lastItem;
        }

        #region F/B-old
        int max_repeat_times = 2;
        BigInteger Forward(BigInteger start, List<BigInteger> result = null)
        {
            //startNumber = start;
            if (start < 0)
                return -1;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            BigInteger lastItem = -1;
            int start_index = 1;
            int repeat_times = 0;
            int index = start_index;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start + Fibonacci_Numbers[index];
                if (cItem >= endNumber)
                {
                    if (repeat_times > max_repeat_times)
                        break;
                    repeat_times++;
                    start = start + Fibonacci_Numbers[index - 1];
                    start_index++;
                    index = start_index;
                    continue;
                }
                result.Add(cItem);
                lastItem = cItem;
                index++;
            }
            return lastItem;
        }
        BigInteger Backward(BigInteger start, List<BigInteger> result = null)
        {
            //endNumber = start;
            if (start < 0)
                return -1;
            if (result == null) result = new List<BigInteger>();
            BigInteger cItem = start;
            BigInteger lastItem = -1;
            int start_index = 1;
            int repeat_times = 0;
            int index = start_index;
            while (index < Fibonacci_Numbers.Count)
            {
                cItem = start - Fibonacci_Numbers[index];
                if (cItem <= startNumber)
                {
                    if (repeat_times > max_repeat_times)
                        break;
                    repeat_times++;
                    start = start - Fibonacci_Numbers[index - 1];
                    start_index++;
                    index = start_index;
                    continue;
                }
                index++;
                result.Add(cItem);
                lastItem = cItem;
            }
            for (int i = index; i >= 1; i--)
                if (result[result.Count - 1] <= startNumber)
                    result.RemoveAt(result.Count - 1);
                else
                {
                    startNumber = result[result.Count - 1];
                    break;
                }
            return lastItem;
        }
        #endregion F/B-old 

        static void Print(List<int> items)
        {
            foreach (int item in items)
                Console.Write(" ", item);
        }
        bool forward = false;
        
        protected override List<Permutation> GeneratePopulation(Population data)
        {
            BestPermutations.Add(data.CurrentPermutation);
            forward = !forward;
            List<BigInteger> newItems = new List<BigInteger>();
            line1_pos = Forward(line1_pos, newItems);
            line1_pos = Backward(line1_pos, newItems);
            if(line1_pos >0)
                for (int i = 1; i < 300; i++)
                    newItems.Add(0);
            line2_pos = Backward(line2_pos, newItems);
            line2_pos = Forward(line2_pos, newItems);
            if (line2_pos > 0)
                for (int i = 1; i < 300; i++)
                    newItems.Add(0);
            //line1_pos=FindNeighbors(line1_pos, true, newItems);
            //line1_pos=FindNeighbors(line1_pos, false, newItems);
            //line2_pos = FindNeighbors(line2_pos, false, newItems);
            //line2_pos = FindNeighbors(line2_pos, true, newItems);
            data.Permutations = new List<Permutation>();
            for (int i = 0; i < newItems.Count; i++)
            {
                data.Permutations.Add(new Permutation(newItems[i]));
            }
            return data.Permutations;
        }
        protected List<Permutation> GenerateNeighborhood_old(Population data)
        {
            BestPermutations.Add(data.CurrentPermutation);
            forward = !forward;
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
            if (line1_pos < 0 && line2_pos < 0)
                return null;
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
