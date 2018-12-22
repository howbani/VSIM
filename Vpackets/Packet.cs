using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using VANET_SIM.Operations;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.Vpackets
{
    public enum PacketType {Data,Beacon }
    public class Packet
    {
        public long PID { get; set; } // each packet has one ID.
        public PacketType Type { get; set; }
        public int SVID { get; set; }
        public int DVID { get; set; }
        public bool isDelivered { get; set; }
        public int SRID { get; set; } // raod segment ID

        public double QueuingDelayInSecond => QueuingDelayStopWatch.Elapsed.TotalSeconds;

        public double PropagationAndTransmissionDelay
        {
            get;set;
        }
        public double DelayInSeconds => QueuingDelayInSecond + PropagationAndTransmissionDelay;
        public int TotalWaitingTimes { get; set; } // Per the whole path. 
        public int Hops_V { get; set; }
        public double WaitingThreshold => TotalWaitingTimes / Hops_V;
        public string VehiclesString { get; set; }
        public int Hops_J { get; set; }

        public double EuclideanDistance { get; set; }
        public double RoutingDistance { get; set; }  // 
        public double RoutingEfficiency => (100 * (EuclideanDistance / RoutingDistance));
        public double PacketLength { get; set; }
       

      

       
        public string TravelledRoadSegmentString { get; set; } // list the ID's

       
        /// <summary>
        /// if the current segment and the destination segment are the same.
        /// </summary>
        public bool IsRouted => DestinationRoadSegment == CurrentRoadSegment;


        // Instance setting:
        public Junction HeadingJunction { get; set; } // the current heading junction 
        public Direction Direction { get; set; } //  where the packet should go in the current step.
        public RoadSegment CurrentRoadSegment { get; set; } // the current segment of packet.

        public Junction DestinationJunction { get { return DestinationVehicle.EndJunction; } } // the end junction which the DestinationVehicle going to.
        public Junction SourceJunction { get; set; } //  this should be reconsidered.
        public RoadSegment DestinationRoadSegment { get { return DestinationVehicle.CurrentLane.MyRoadSegment; } }

        public VehicleUi SourceVehicle { get; set; }
        public VehicleUi DestinationVehicle { get; set; }

        public Stopwatch QueuingDelayStopWatch = new Stopwatch();
    }
}
