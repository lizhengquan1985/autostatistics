using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new AnaylyzeApi().Merged("eosusdt"));

            SpotStatisticsBiz.Statistics();

            Console.ReadLine();
        }
    }
}
