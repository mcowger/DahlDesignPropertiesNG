using SimHub.Plugins.OutputPlugins.GraphicalDash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DahlDesignPropertiesNG
{
    internal class IntervalController
    {
        internal static int MaxCounterVal = 60;
        internal static int[] HzValues = new int[] { 1, 2, 3, 4, 5, 6, 10, 15, 30, 60 };
        /// <summary>
        /// How often per second should should we run something?
        /// </summary>
        /// <param name="CounterVal"></param>
        /// <param name="Hz"></param>
        /// <returns></returns>
        internal static bool ShouldRunUpdate(int CounterVal, int Hz)
        {
            int quickval = CounterVal % (MaxCounterVal / Hz);
            return quickval == 0;
        }
    }

}
