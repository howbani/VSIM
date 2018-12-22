using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VANET_SIM.Operations
{

    public static class LaneIndex
    {
        /// <summary>
        ///  random index between 0-3;
        /// </summary>
        public static class RandomLaneIndex
        {
            public static int ZeroToMax
            {
                get
                {
                    double rando = RandomeNumberGenerator.GetUniform(0, PublicParamerters.NumberOfLanes - 1);
                    int x = Convert.ToInt16(rando);
                    return x;
                }
            }

            public static int OneToHalf
            {
                get
                {
                    int halLanes = Convert.ToInt16(PublicParamerters.NumberOfLanes / 2);
                    double rando = RandomeNumberGenerator.GetUniform(0, halLanes - 1);
                    int x = Convert.ToInt16(rando);
                    return x;
                }
            }

            public static int North
            {
                get
                {
                    int halLanes = Convert.ToInt16(PublicParamerters.NumberOfLanes / 2);
                    double rando = RandomeNumberGenerator.GetUniform(halLanes-1);
                    int x = halLanes + Convert.ToInt16(rando);
                    return x;
                }
            }
            public static int South
            {
                get
                {
                    int halLanes = Convert.ToInt16(PublicParamerters.NumberOfLanes / 2);
                    double rando = RandomeNumberGenerator.GetUniform(halLanes-1);
                    int x =Convert.ToInt16(rando);
                    return x;
                }
            }

            public static int West
            {
                get
                {
                    int halLanes = Convert.ToInt16(PublicParamerters.NumberOfLanes / 2);
                    double rando = RandomeNumberGenerator.GetUniform(halLanes - 1);
                    int x = Convert.ToInt16(rando);
                    return x;
                }
            }

            public static int East
            {
                get
                {
                    int halLanes = Convert.ToInt16(PublicParamerters.NumberOfLanes / 2);
                    double rando = RandomeNumberGenerator.GetUniform(halLanes-1);
                    int x = halLanes + Convert.ToInt16(rando);
                    return x;
                }
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
