using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VANET_SIM.Operations
{
    class MyMath
    {
        public static ulong Factorial(ulong num)
        {
            ulong re = 1;
            for (ulong i = 1; i <=num; i++)
            {
                re *= i;
            }
            return re;
        }


        /// <summary>
        /// combination 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double Combination(ulong n, ulong k)
        {
            double fn = MyMath.Factorial(n);
            double fk = MyMath.Factorial(k);
            double fn_k = MyMath.Factorial(n - k);
            double re = fn / (fk * fn_k);
            return re;
        }


        private static ulong Max(ulong n1, ulong n2) { if (n1 > n2) return n1; else return n2; }
        private static ulong Min(ulong n1, ulong n2) { if (n1 < n2) return n1; else return n2; }
    }
}
