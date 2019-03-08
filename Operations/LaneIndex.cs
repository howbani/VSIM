using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSIM.Operations
{

    public static class LaneIndex
    {
        /// <summary>
        ///  random index between 0-3;
        /// </summary>
        public static class RandomLaneIndex
        {
            public static int ZeroToMax( int max)
            {
                double rando = RandomeNumberGenerator.GetUniform(0, max - 1);
                int x = Convert.ToInt16(rando);
                return x;
            }

            public static int OneToHalf (int max)
            {
                int halLanes = Convert.ToInt16(max / 2);
                double rando = RandomeNumberGenerator.GetUniform(0, halLanes - 1);
                int x = Convert.ToInt16(rando);
                return x;
            }

            public static int North(int max)
            {
                int halLanes = Convert.ToInt16(max / 2);
                double rando = RandomeNumberGenerator.GetUniform(halLanes - 1);
                int x = halLanes + Convert.ToInt16(rando);
                return x;
            }
            public static int South(int max)
            {
                int halLanes = Convert.ToInt16(max / 2);
                double rando = RandomeNumberGenerator.GetUniform(halLanes - 1);
                int x = Convert.ToInt16(rando);
                return x;
            }

            public static int West(int max)
            {
                int halLanes = Convert.ToInt16(max / 2);
                double rando = RandomeNumberGenerator.GetUniform(halLanes - 1);
                int x = Convert.ToInt16(rando);
                return x;
            }

            public static int East(int max)
            {
                int halLanes = Convert.ToInt16(max / 2);
                double rando = RandomeNumberGenerator.GetUniform(halLanes - 1);
                int x = halLanes + Convert.ToInt16(rando);
                return x;
            }
        }
    
        
    }



    public static class LanesCaptions
    {
        public static string Lane1 = "lane1";
        public static string Lane2 = "lane2";
        public static string Lane3 = "lane3";
        public static string Lane4 = "lane4";
        public static string Lane5 = "lane5";
        public static string Lane6 = "lane6";
    }
}
