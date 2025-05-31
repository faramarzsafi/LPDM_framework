using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class AllPermutations : Metaheuristic
    {
        private List<BigInteger> Fibonacci_Numbers = new List<BigInteger>();
        private Dictionary<BigInteger, int> Fibonacci_Numbers_Index = new Dictionary<BigInteger, int>();
        private Permutation[] Fibonacci_Permutations;
        private int Neighborhood_Size;
        private List<Permutation> BestPermutations = new List<Permutation>();
        BigInteger maxNumber;
        BigInteger startNumber;
        BigInteger endNumber;
        public AllPermutations(int Neighborhood_Size) : base(0, AlgorithmType.AllPermutations)
        {
            maxNumber = Factoradic.Factorial[Permutation.JobsCount];
            endNumber = maxNumber;
            startNumber = 1;
            this.Neighborhood_Size = Neighborhood_Size;
        }
 
        static void Print(List<int> items)
        {
            foreach (int item in items)
                Console.Write(" ", item);
        }
        bool forward = false;
        BigInteger last = 0;
        int generate_count = 100000;
        protected override List<Permutation> GeneratePopulation(Population data)
        {
            data.Permutations = new List<Permutation>();
            for (int i = 0; i < this.Neighborhood_Size; i++)
            {
                last++;
                if (last > endNumber)
                    break;
                data.CurrentPermutation = new Permutation(last);
                data.Permutations.Add(data.CurrentPermutation);
            }
            return data.Permutations;
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
