using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using VANET_SIM.UI;
using VANET_SIM.Vpackets;

namespace VANET_SIM.Operations
{
    static class PublicStatistics
    {
        public static List<Packet> DeleiverdPacketsList = new List<Packet>();
        public static List<Packet> DropedPacketsList = new List<Packet>();
        public static UiLiveStatstics LiveStatstics = new UiLiveStatstics();

        public static long GneratedPacketsCount
        {
            get
            {
                long re = Convert.ToInt64(LiveStatstics.lbl_Generated_Packets_Count.Content.ToString());
                return re;
            }
            set
            {
                LiveStatstics.lbl_Generated_Packets_Count.Content = value;
            }
        }

        public static double DeleiverdPacketsCount
        {
            get
            {
                double genpackets = Convert.ToDouble(GneratedPacketsCount);
                double DeleiverdPacketsCountDoub = DeleiverdPacketsList.Count;
                double averdeleay = DelaySumInSeconds / DeleiverdPacketsCountDoub;
                double averageHops = HopsSum / DeleiverdPacketsCountDoub;
                double averagRoudis = SumRoutingDistance / DeleiverdPacketsCountDoub;
                SucessRatio = (DeleiverdPacketsCountDoub / genpackets) * 100;


                LiveStatstics.lbl_Delivered_Packets_Count.Content = DeleiverdPacketsList.Count;
                LiveStatstics.lbl_average_hops.Content = averageHops;
                LiveStatstics.lbl_average_delay.Content = averdeleay;
                LiveStatstics.lbl_average_routing_distance.Content = averagRoudis;


                
                return DeleiverdPacketsCountDoub;
            }
        }

        public static double DropedPacketsCount
        {
            get
            {
                LiveStatstics.lbl_Dropped_Packets_Count.Content = DropedPacketsList.Count;
                return DropedPacketsList.Count;
            }
        }
        public static double InQueuePackets
        {
            get
            {
                double re = GneratedPacketsCount - DropedPacketsCount - DeleiverdPacketsCount;
                LiveStatstics.lbl_In_Queue_Packets.Content = re;
                return re;
            }
        }

        public static double SucessRatio
        {
            get => Convert.ToDouble(LiveStatstics.lbl_Average_Success_Ratio.Content);
            set => LiveStatstics.lbl_Average_Success_Ratio.Content = value;
        }

        /// <summary>
        /// delay sum of all delived packets.
        /// </summary>
        public static double DelaySumInSeconds
        {
            get; set;
        }

        /// <summary>
        /// the total hops for experment. just to find the average.
        /// </summary>
        public static double HopsSum
        {
            get; set;
        }

        public static double SumRoutingDistance
        {
            get; set;
        }

        public static void Clear()
        {
            HopsSum = 0;
            DropedPacketsList.Clear();
            DeleiverdPacketsList.Clear();
            SumRoutingDistance = 0;
            DelaySumInSeconds = 0;
            GneratedPacketsCount = 0;
            double x = DeleiverdPacketsCount;
        }
    }
}
