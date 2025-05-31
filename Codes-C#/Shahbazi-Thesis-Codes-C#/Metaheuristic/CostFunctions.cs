using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristic
{
    public partial class Permutation
    {
        public static double MapXY(double x, double x_min, double x_max, double y_min, double y_max)
        {
            double offset = y_min;
            double ratio = (y_max - y_min) / (x_max - x_min);
            return ratio * (x - x_min) + offset;

        }
        public static double AckleyFCN_Map(double x, double x_min, double x_max)
        {
            double y_min = -32;
            double y_max = +32;
            return MapXY(x, x_min, x_max, y_min, y_max);
        }
        public double AckleyFCN()
        {
            //http://benchmarkfcns.xyz/benchmarkfcns/ackleyfcn.html

            double a = 20;
            double b = 0.2;
            double c = 2 * Math.PI;
            double sum_squre = 0;
            double sum_cx = 0;
            for (int i = 0; i < JobsCount; i++)
            {
                double x = AckleyFCN_Map(Jobs[i], 1, JobsCount);
                sum_squre += x * x;
                sum_cx += Math.Cos(c * x);
            }
            return -a * Math.Exp(-b * Math.Sqrt(1 / JobsCount * sum_squre)) - Math.Exp(1 / JobsCount * sum_cx) + a + Math.Exp(1);
        }
        public static double AckleyN4FCN_Map(double x, double x_min, double x_max)
        {
            double y_min = -35;
            double y_max = +36;
            return MapXY(x, x_min, x_max, y_min, y_max);
        }
        public double AckleyN4FCN()
        {
            //http://benchmarkfcns.xyz/benchmarkfcns/ackleyn4fcn.html

            double sum = 0;
            for (int i = 0; i < JobsCount-1; i++)
            {
                double x_i = AckleyN4FCN_Map(Jobs[i], 1, JobsCount);
                double x_ii = AckleyN4FCN_Map(Jobs[i+1], 1, JobsCount);
                sum += Math.Exp(-0.2) * Math.Sqrt(x_i*x_i+x_ii*x_ii) + 3 * (Math.Cos(2 * x_i) + Math.Sin(2 * x_ii));
            }
            return sum;
        }
        
        public static double AlpineN1FCN_Map(double x, double x_min, double x_max)
        {
            double y_min = 0;
            double y_max = 10;
            return MapXY(x, x_min, x_max, y_min, y_max);
        }
        public double AlpineN1FCN()
        {
            //http://benchmarkfcns.xyz/benchmarkfcns/alpinen1fcn.html

            double sum = 0;
            for (int i = 0; i < JobsCount; i++)
            {
                double x_i = AlpineN1FCN_Map(Jobs[i], 1, JobsCount);
                sum += Math.Abs(x_i*Math.Sin(x_i) +0.1*x_i);
            }
            return sum;
        }
        public static double Xin_SheYangN4FCN_Map(double x, double x_min, double x_max)
        {
            double y_min = -10;
            double y_max = 10;
            return MapXY(x, x_min, x_max, y_min, y_max);
        }
        public double Xin_SheYangN4FCN()
        {
            //http://benchmarkfcns.xyz/benchmarkfcns/xinsheyangn4fcn.html

            double sum_sin_2_x_i = 0;
            double sum_x_i_2 = 0;
            double sum_sin_2_sqrt__abs_xi = 0;
            for (int i = 0; i < JobsCount; i++)
            {
                double x_i = Xin_SheYangN4FCN_Map(Jobs[i], 1, JobsCount);
                double sin_x_i = Math.Sin(x_i);
                sum_sin_2_x_i += sin_x_i * sin_x_i;
                sum_x_i_2 += x_i * x_i;
                double sin_sqrt__abs_xi = Math.Sin(Math.Sqrt(Math.Abs(x_i)));
                sum_sin_2_sqrt__abs_xi += sin_sqrt__abs_xi * sin_sqrt__abs_xi;
            }
            return (sum_sin_2_x_i - Math.Exp(-sum_x_i_2))*Math.Exp(-sum_sin_2_x_i);
        }
        public double ComputJobSchedulingCost()
        {
            int[] machinesCost = Enumerable.Repeat(0, JobsCount).ToArray();
            int[] jobsCost = Enumerable.Repeat(0, MachinesCount).ToArray();
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            for (int i = 0; i < MachinesCount; i++)
                queue.Enqueue(-1);
            for (int i = 0; i < JobsCount + MachinesCount - 1; i++)
            {
                if (i < JobsCount)
                    queue.FixedEnqueue(Jobs[i]);
                else
                    queue.FixedEnqueue(-1);
                for (int machine = 0; machine < MachinesCount; machine++)
                {
                    int job = queue.ElementAt(MachinesCount - machine - 1);
                    if (job >= 0)
                    {
                        int newCost;
                        if (machinesCost[job] < jobsCost[machine])
                            newCost = jobsCost[machine];
                        else
                            newCost = machinesCost[job];
                        jobsCost[machine] = machinesCost[job] = newCost + Costs[job, machine];
                    }
                }
            }
            return machinesCost[JobsCount - 1];
        }
    }
}
