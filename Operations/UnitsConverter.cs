using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VANET_SIM.Operations
{
   public  static class UnitsConverter
    {
        /// <summary>
        /// KMPH to km/s.
        /// </summary>
        /// <param name="speed_KMph"></param>
        /// <returns></returns>
        public static double KmhToMps(double speed_KMph)
        {
            double dis_Meter = speed_KMph * 1000; // Km to m.
            double time_Second = 3600; // 1h= 3600s.
            double mps = dis_Meter / time_Second;
            return mps;
        }
        /// <summary>
        /// how  long time requires to pass in one pexcil.
        /// this vale should be set to timer that moves the vechile object in the screen.
        /// </summary>
        /// <param name="speed_KMph"></param>
        /// <returns>time interval in second</returns>
        public static double KmphToTimerInterval(double speed_KMph)
        {
            double disInMeter = speed_KMph * 1000; // Km to m.
            double timeInSecond = 3600; // 1h= 3600s.
            // timer interval:
            double interval = timeInSecond / disInMeter;
            // Console.WriteLine("speed:" + speed_KMph + " km/h,  Timer interval=" + interval + "s");
            return interval; // how many m in one s.
        }

        /// <summary>
        /// get kmp from time interval. time is in seconds
        /// </summary>
        /// <param name="timerInterval_Second"></param>
        /// <returns></returns>
        public static double TimerIntervalToKmph(double timerInterval_Second)
        {
            double S_KMH = (3600 / 1000) * (1 / timerInterval_Second);
            return S_KMH;
        }

        /// <summary>
        /// get the time.
        /// </summary>
        /// <param name="speed_mps"></param>
        /// <param name="distance_m"></param>
        /// <returns></returns>
        public static double GetTimeinSecond(double speed_mps, double distance_m)
        {
            return distance_m / speed_mps;
        }

        public static double GetSpeedInmps(double dis_m, double time_s)
        {
            return dis_m / time_s;
        }
        public static double GetDistance(double speed_mps, double time_s)
        {
            return speed_mps * time_s;
        }
       

    }
}
