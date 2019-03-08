using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VSIM.Properties;
using VSIM.RoadNet.Components;
namespace VSIM.Operations
{
    public static class Computations
    {

        /// <summary>
        /// Random interval. related to speed.
        /// </summary>
        public static double RandomTimeInterval(RoadSegment rs)
        {
            double ranInSegment = RandomSpeedkmh(rs);
            double conver = GetTimeIntervalInSecond(ranInSegment);
            return conver;
        }


        public static TimeSpan RandomTimeSpane(RoadSegment rs)
        {
            double xtim = RandomTimeInterval(rs);
            return TimeSpan.FromSeconds(xtim);
        }

        /// <summary>
        /// random speed in KMH between min and max.
        /// </summary>
        public static double RandomSpeedkmh(RoadSegment rs)
        {
            double ran = RandomeNumberGenerator.GetNormal(rs.MaxAllowedSpeed, PublicParamerters.SpeedStandardDeviation);
            return ran;
        }

        /// <summary>
        /// set speed. in KM/H
        /// </summary>
        /// <param name="speedkhh"></param>
        /// <returns>time interval in seconds</returns>
        public static double GetTimeIntervalInSecond(double speedkhh)
        {
            if (speedkhh <= 0) return 0;
            else if (speedkhh >= Settings.Default.MaxSpeed)
                return UnitsConverter.KmphToTimerInterval(Settings.Default.MaxSpeed); // can't exceed the max speed.
            else return UnitsConverter.KmphToTimerInterval(speedkhh);
        }

        /// <summary>
        /// distance bettween two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            double dx = (p1.X - p2.X);
            dx *= dx;
            double dy = (p1.Y - p2.Y);
            dy *= dy;
            return Math.Sqrt(dx + dy);
        }


        /// <summary>
        /// the maximum speed for each sgment.
        /// </summary>
        public static double MaxAllowedSpeedInSegmentInKm
        {
            get
            {
                double ran = RandomeNumberGenerator.GetNormal(Settings.Default.MaxSpeed, PublicParamerters.SpeedStandardDeviation);
                if (ran > Settings.Default.MaxSpeed * 2)
                {
                    ran = Settings.Default.MaxSpeed + RandomeNumberGenerator.GetUniform(Settings.Default.MinSpeed);
                }

                if (ran <= Settings.Default.MinSpeed)
                {
                    ran = PublicParamerters.MeanSpeed + RandomeNumberGenerator.GetUniform(Settings.Default.MinSpeed);
                }
              
                return ran;
            }
        }

        /// <summary>
        /// get random VehicleInterArrivalMean for each segment. 
        /// </summary>
        public static double VehicleInterArrivalMean
        {
            get
            {
                double ran = RandomeNumberGenerator.GetNormal(PublicParamerters.VehicleInterArrivalMean, PublicParamerters.VehicleInterArrivalStandardDeviation);

                if (ran > PublicParamerters.VehicleInterArrivalMean * 2)
                {
                    ran = PublicParamerters.VehicleInterArrivalMean + RandomeNumberGenerator.GetUniform(2);
                }

                if (ran < PublicParamerters.VehicleInterArrivalStandardDeviation)
                {
                    ran = PublicParamerters.VehicleInterArrivalStandardDeviation + RandomeNumberGenerator.GetUniform(2);
                }

                return ran;

            }
        }

       



    }
}
