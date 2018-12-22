using System;
using System.Collections.Generic;
using VANET_SIM.UI;
using VANET_SIM.Vpackets;
using static VANET_SIM.RoadNet.Components.VehicleUi;

namespace VANET_SIM.Operations
{
    public static class PublicParamerters
    {
        public static double TransmissionRate = 2 * 1000000;////2Mbps 100 × 10^6 bit/s , //https://en.wikipedia.org/wiki/Transmission_time
        public static double SpeedOfLight = 299792458;//https://en.wikipedia.org/wiki/Speed_of_light // s

        #region Road Network
        public static string NetworkName { get; set; }
        public static double JunctionWidth => 2 * LaneWidth * NumberOfLanes;
        public static double JunctionHeight => 2 * LaneWidth * NumberOfLanes;
        public static double LaneWidth = 3.3;
        public static double NumberOfLanes = 6;
        /// <summary>
        /// the timer to switch the Trafic Signaling
        /// </summary>
        public static double TraficSignalingTimerInterval = 0.2; 
        /// <summary>
        /// when there is only two lanes in the road segment. the lane can switch to direct  right or left. this timeer is to make an atomatic switch
        /// </summary>
        public static TimeSpan RoadSegmentSwitchDirectionTimerInterval
        {
            get
            {
                double min = 1;
                double max = 2;
                double ran= RandomeNumberGenerator.GetUniform(min, max);
                return TimeSpan.FromSeconds(ran); //ran; // seconds.
            }
        }
        public static Vehicleinfo DisplayInfoFlag { get; set; } // what the info should be displayed when the vehcile is moving.
        #endregion

        #region  Vehicle
        public static double VehicleWidth = 2.0;
        public static double VehicleHight = 2.0;
        public static double CommunicationRaduis = 150; // in m
        public static double RemianDistanceToHeadingJunctionThreshold = CommunicationRaduis * 0.9; // the time when the v should select new v.
        #region speeed
        public static double MaxSpeed = 60;
        public static double MinSpeed = 30;
        public static double MeanSpeed { get { return (MaxSpeed + MinSpeed) / 2; } }
        public static double SpeedStandardDeviation = 5;
        #endregion

        #region VehicleInterArrival
        public static double VehicleInterArrivalMean = 2; // seconds. we have 6
        public static double VehicleInterArrivalStandardDeviation { get { return VehicleInterArrivalMean * 0.3; } } // seconds. // for enery.  
        #endregion

        #endregion


        #region Packets
        public static double BufferSize = 20; // the maximum number of packets allowed in the queue.
        public static double PacketQueueTimerInterval = 0.001; // in seconds.
        public static double PacketQueueRetryTimerInterval = 1; // when no forwarder, how long time need to re-send it.
        public static double DataPacketLength = 1024;
        public static int MaximumNumberofRetransmission = 7; // each packet is allowwed to be transmitted 7 times.
        #endregion




    }
}
