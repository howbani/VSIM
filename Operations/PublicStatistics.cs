using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using VSIM.Properties;
using VSIM.UI;
using VSIM.Vpackets;

namespace VSIM.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public static class PublicStatistics
    {
        public static List<Packet> DeleiverdPacketsList = new List<Packet>();
        public static List<Packet> DropedPacketsList = new List<Packet>();
        public static UiLiveStatstics LiveStatstics = new UiLiveStatstics();
        /// <summary>
        /// the number of generated packets
        /// </summary>
        public static long GeneratedPacketsCount
        {
            get; set;
        }

        /// <summary>
        /// the number of Deleiverd packets.
        /// </summary>
        public static double DeleiverdPacketsCount
        {
            get
            {
                return DeleiverdPacketsList.Count;
            }
        }

        public static double DeleiverdPacketsRatio
        {
            get
            {
                double genpackets = Convert.ToDouble(GeneratedPacketsCount);

                double SucessRatio = (DeleiverdPacketsCount / genpackets) * 100;

                return SucessRatio;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static double DropedPacketsCount
        {
            get
            {

                double droped = DropedPacketsList.Count;

                return droped;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static double DropedPacketsRatio
        {
            get
            {
                double genpackets = Convert.ToDouble(GeneratedPacketsCount);
                double dropedRatio = (DropedPacketsCount / genpackets) * 100;
                return dropedRatio;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double InQueuePackets
        {
            get
            {
                double re = GeneratedPacketsCount - DropedPacketsCount - DeleiverdPacketsCount;
                return re;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static double InQueuePacketsRatio
        {
            get
            {
                double genpackets = Convert.ToDouble(GeneratedPacketsCount);
                double InQueueRatio = (InQueuePackets / genpackets) * 100;
                return InQueueRatio;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double AverageQueuDelay
        {
            get
            {
                return QueueDelaySumInSeconds / DeleiverdPacketsCount;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static double AverageWaitingTimes
        {
            get
            {
                return WaitingTimesSum / DeleiverdPacketsCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double AveragePropagationgDelay
        {
            get
            {
                return SumPropagationAndTransmissionDelay / DeleiverdPacketsCount;
            }
        }


        /// <summary>
        /// averqueue + propagation+ transmission
        /// </summary>
        public static double TotalDelay
        {
            get
            {
                return (AverageQueuDelay + AveragePropagationgDelay);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AverageHops
        {
            get
            {
                return HopsSum / DeleiverdPacketsCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double AverageRouitngDistance
        {
            get
            {
                return SumRoutingDistance / DeleiverdPacketsCount;
            }
        }


        public static double AverageCommOverhead
        {
            get
            {
                return SumCommunicationOverhead / DeleiverdPacketsCount;
            }
        }


        public static double WaitingTimesSum { get; set; }
        /// <summary>
        /// delay sum of all delived packets.
        /// </summary>
        public static double QueueDelaySumInSeconds { get; set; }
        /// <summary>
        /// the total hops for experment. just to find the average.
        /// </summary>
        public static double HopsSum { get; set; }
        public static double SumRoutingDistance { get; set; }
        public static double SumPropagationAndTransmissionDelay { get; set; }
        public static double SumCommunicationOverhead { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static void InstanceDisplay()
        {
            LiveStatstics.Dispatcher.Invoke((Action)delegate
          {
              LiveStatstics.lbl_Generated_Packets_Count.Content = GeneratedPacketsCount;
              LiveStatstics.lbl_Delivered_Packets_Count.Content = DeleiverdPacketsList.Count;
              LiveStatstics.lbl_Average_Success_Ratio.Content = DeleiverdPacketsRatio.ToString("0.00");


              LiveStatstics.lbl_In_Queue_Packets.Content = InQueuePackets.ToString();
              LiveStatstics.lbl_In_Queue_Packets_Ratio.Content = InQueuePacketsRatio.ToString("0.00");

              LiveStatstics.lbl_average_queue_delay.Content = AverageQueuDelay.ToString("0.00");
              LiveStatstics.lbl_average_waiting_times.Content = AverageWaitingTimes.ToString();
              LiveStatstics.lbl_AveragePropagationDelay.Content = AveragePropagationgDelay.ToString("0.0000");
              LiveStatstics.lbl_average_total_delay.Content = TotalDelay.ToString("0.000");


              LiveStatstics.lbl_average_hops.Content = AverageHops.ToString("0.00");
              LiveStatstics.lbl_average_routing_distance.Content = AverageRouitngDistance.ToString("0.00");




              LiveStatstics.lbl_Dropped_Packets_Count.Content = DropedPacketsCount.ToString();
              LiveStatstics.lbl_Dropped_Packets_Ratio.Content = DropedPacketsRatio.ToString("0.00");
              LiveStatstics.lbl_redundant_pacekts_count.Content = SumCommunicationOverhead.ToString();
              LiveStatstics.lbl_average_overhead_path.Content = AverageCommOverhead.ToString("0.00");
          });

        }

        /// <summary>
        /// print. 
        /// </summary>
        public static List<ValParPair> PrintResults(MainWindow mainWindow)
        {
            if (mainWindow.MyVehicles.Count > 0)
            {
                if (mainWindow.MyRoadSegments.Count > 0)
                {
                    List<ValParPair> res = new List<ValParPair>();
                    res.Add(new ValParPair { Par = "Network Name", Val = Settings.Default.NetTopName });
                    res.Add(new ValParPair { Par = "Junctions Count", Val = mainWindow.MyJunctions.Count.ToString() });
                    res.Add(new ValParPair { Par = "Rows Count", Val = Settings.Default.Rows_net_top.ToString() });
                    res.Add(new ValParPair { Par = "Cols Count", Val = Settings.Default.Cols_net_top.ToString() });
                    res.Add(new ValParPair { Par = "Road Segments Count", Val = mainWindow.MyRoadSegments.Count.ToString() });
                    res.Add(new ValParPair { Par = "HLength(m)", Val = Settings.Default.HorizontalLength.ToString() });
                    res.Add(new ValParPair { Par = "VLength(m)", Val = Settings.Default.VerticalLength.ToString() });
                    res.Add(new ValParPair { Par = "Road-ways", Val = "2" });
                    res.Add(new ValParPair { Par = "Lanes", Val = mainWindow.MyRoadSegments[0].LanesCount.ToString() });
                    res.Add(new ValParPair { Par = "Number of Vehicles", Val = mainWindow.MyVehicles.Count.ToString() });
                    res.Add(new ValParPair { Par = "Average Speed(km/h)", Val = PublicParamerters.MeanSpeed.ToString() });
                    res.Add(new ValParPair { Par = "Max Speed(km/h)", Val = Settings.Default.MaxSpeed.ToString() });
                    res.Add(new ValParPair { Par = "Min Speed(km/h)", Val = Settings.Default.MinSpeed.ToString() });
                    res.Add(new ValParPair { Par = "Comm Range(m)", Val = Settings.Default.CommunicationRange.ToString() });
                    res.Add(new ValParPair { Par = "Packet Size", Val = PublicParamerters.DataPacketLength.ToString() });
                    res.Add(new ValParPair { Par = "Generated Packets Count", Val = GeneratedPacketsCount.ToString() });
                    res.Add(new ValParPair { Par = "Delivered Packets Count", Val = DeleiverdPacketsList.Count.ToString() });
                    res.Add(new ValParPair { Par = "Average Success Ratio(%)", Val = DeleiverdPacketsRatio.ToString("0.00") });
                    res.Add(new ValParPair { Par = "In Queue Packets", Val = InQueuePackets.ToString() });
                    res.Add(new ValParPair { Par = "In Queue Packets Ratio(%)", Val = InQueuePacketsRatio.ToString("0.00") });
                    res.Add(new ValParPair { Par = "Dropped Packets Count", Val = DropedPacketsCount.ToString() });
                    res.Add(new ValParPair { Par = "Dropped Packets Ratio(%)", Val = DropedPacketsRatio.ToString("0.00") });
                    res.Add(new ValParPair { Par = "Average Queue Delay (s)", Val = AverageQueuDelay.ToString("0.00") });
                    res.Add(new ValParPair { Par = "Average Store Times ", Val = AverageWaitingTimes.ToString() });
                    res.Add(new ValParPair { Par = "Average Propagation Delay (s)", Val = AveragePropagationgDelay.ToString("0.0000") });
                    res.Add(new ValParPair { Par = "Average Delay (s)", Val = TotalDelay.ToString("0.000") });
                    res.Add(new ValParPair { Par = "Average Hops", Val = AverageHops.ToString("0.00") });
                    res.Add(new ValParPair { Par = "Average Routing Distance(m)/path", Val = AverageRouitngDistance.ToString() });
                    res.Add(new ValParPair { Par = "Redundant Packets/total", Val = SumCommunicationOverhead.ToString() });
                    res.Add(new ValParPair { Par = "Average Com. Overhead/Path", Val = AverageCommOverhead.ToString("0.00") });
                    res.Add(new ValParPair { Par = "Max. Attemps", Val = Settings.Default.MaximumAttemps.ToString() });
                    res.Add(new ValParPair { Par = "Max. Store Time", Val = Settings.Default.MaxStoreTime.ToString() });
                    res.Add(new ValParPair { Par = "Distance Between Source and Destination (m) ", Val = Settings.Default.DistanceBetweenSourceAndDestination.ToString() });
                    res.Add(new ValParPair { Par = "Trafic Signaling (s) ", Val = Settings.Default.TraficSignalingTimerInterval.ToString() });
                    res.Add(new ValParPair { Par = "Transmission Rate(bps)", Val = PublicParamerters.TransmissionRate.ToString() });

                    //
                    res.Add(new ValParPair { Par = "Frontward Par", Val = Settings.Default.IntraVehiForwardDirectionPar.ToString() });
                    res.Add(new ValParPair { Par = "Backward Par ", Val = Settings.Default.IntraVehiBackwardDirectionPar.ToString() });
                    res.Add(new ValParPair { Par = "Connectivity Par", Val = Settings.Default.WeightConnectivity.ToString() });
                    res.Add(new ValParPair { Par = "Distance Par", Val = Settings.Default.WeightShortestDistance.ToString() });
                    return res;
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Clear()
        {

            HopsSum = 0;
            DropedPacketsList.Clear();
            DeleiverdPacketsList.Clear();
            SumRoutingDistance = 0;
            QueueDelaySumInSeconds = 0;
            GeneratedPacketsCount = 0;
            SumPropagationAndTransmissionDelay = 0;

            LiveStatstics.lbl_Delivered_Packets_Count.Content = "0";
            LiveStatstics.lbl_Average_Success_Ratio.Content = "0";

            LiveStatstics.lbl_average_queue_delay.Content = "0";
            LiveStatstics.lbl_average_waiting_times.Content = "0";
            LiveStatstics.lbl_AveragePropagationDelay.Content = "0";
            LiveStatstics.lbl_average_total_delay.Content = "0";


            LiveStatstics.lbl_average_hops.Content = "0";
            LiveStatstics.lbl_average_routing_distance.Content = "0";

            LiveStatstics.lbl_In_Queue_Packets.Content = "0";
            LiveStatstics.lbl_In_Queue_Packets_Ratio.Content = "0";


            LiveStatstics.lbl_Dropped_Packets_Count.Content = "0";
            LiveStatstics.lbl_Dropped_Packets_Ratio.Content = "0";
            LiveStatstics.lbl_average_overhead_path.Content = "0";
            LiveStatstics.lbl_redundant_pacekts_count.Content = "0";
        }

        

    }


    public class ValParPair
    {
        public string Par { get; set; }
        public string Val { get; set; }
    }
}
