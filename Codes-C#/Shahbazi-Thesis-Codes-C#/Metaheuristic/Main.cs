using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Numerics;


namespace Metaheuristic
{
    partial class Program
    {
        public static void Run(string[] args)
        {
            Thesis_16.Benchmark.Run(args);

            //Thesis_16_duplicates.Benchmark.Run(args);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
