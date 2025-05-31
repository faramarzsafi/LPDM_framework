using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public class TabuSearch : Metaheuristic
    {
        //int tabuLiveTimes;
        Permutation p22;
        public TabuSearch(int liveTimes, AlgorithmType algorithmType):base(liveTimes, algorithmType)
        {
        }
    }
}
