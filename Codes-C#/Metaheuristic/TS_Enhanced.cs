using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class TS_Enhanced : Metaheuristic
    {
        public TS_Enhanced(int liveTimes) : base(liveTimes, AlgorithmType.Enhanced) { }
        protected override List<Permutation> GeneratePopulation(Population data)
        {
            data.Permutations = new List<Permutation>();
            for (int i = 0; i < data.JobsCount; i++)
                for (int j = i + 1; j < data.JobsCount; j++)
                {
                    Permutation permutation = Permutation.CreateWithExchange(data.CurrentPermutation, i, j);
                    data.Permutations.Add(permutation);
                }
            return data.Permutations;
        }
        protected Permutation Mutation(Population data)
        {
            for (int i = 0; i < data.JobsCount; i++)
                for (int j = i + 1; j < data.JobsCount; j++)
                {
                    Permutation permutation = Permutation.CreateWithInsert(data.CurrentPermutation, i, j);
                    //data.NeighborhoodPermutations.Add(permutation);
                    PopulationBestMember member = data.CheckHistory(permutation);
                    if (member != null)
                    {
                        data.CurrentPermutation = member.Permutation;
                        return data.CurrentPermutation;
                    }            
                }
            return null;
        }
        protected override Permutation FindTheBestInPopulation(Population data)
        {
            PopulationBestMember member;
            data.Permutations.Sort();
            data.RefreshHistory();
            //
            int len = data.Permutations.Count > data.LiveTimes ? data.LiveTimes : data.Permutations.Count;
            member = data.CheckHistory(data.Permutations[0]);
            if (member != null)
            {
                data.CurrentPermutation = member.Permutation;
                return data.CurrentPermutation;
            }
            //
            return Mutation(data);
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
