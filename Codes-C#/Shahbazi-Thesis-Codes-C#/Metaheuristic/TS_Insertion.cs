using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class TS_Insertion : TabuSearch
    {
        public TS_Insertion(int tabuLiveTimes) : base(tabuLiveTimes, AlgorithmType.Insertion) { }
        protected override List<Permutation> GeneratePopulation(Population r)
        {
            r.Permutations = new List<Permutation>();
            for (int i = 0; i < r.JobsCount; i++)
                for (int j = i + 1; j < r.JobsCount; j++)
                {
                    Permutation permutation = Permutation.CreateWithInsert(r.CurrentPermutation, i, j);
                    r.Permutations.Add(permutation);
                }
            return r.Permutations;
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
            return null;
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
