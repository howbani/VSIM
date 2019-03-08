using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using VSIM.Operations;
using VSIM.RoadNet.Components;

namespace VSIM.Vpackets
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

        public double CommunicationOverhead { get; set; } 


        /// <summary>
        ///  the number of waitingtimes *PacketQueueRetryTimerInterval
        /// </summary>
        public double QueuingDelayInSecond
        {
            get
            {
                // 0.1 for proccesing the packet.
                return (PublicParamerters.PacketQueueRetryTimerInterval) * PathWaitingTimes;
            }
        }

        /// <summary>
        /// the PropagationAndTransmissionDelay for thw packet along the whole path.
        /// </summary>
        public double PropagationAndTransmissionDelay
        {
            get;set;
        }

        /// <summary>
        ///  QueuingDelayInSecond + PropagationAndTransmissionDelay;
        /// </summary>
        public double TotalDelayInSeconds => QueuingDelayInSecond + PropagationAndTransmissionDelay;

        public int PathWaitingTimes { get; set; } // the number that the packet stored in the buffer in the whol path
        public int HopWaitingTimes { get; set; } // one hop. this indicate the number of waiting-times in the vechile.
        public int HopsVehicles { get; set; }
      //  public double WaitingThreshold => PathWaitingTimes / Hops_V;
        public string VehiclesString { get; set; }
        public int HopsJunctions { get; set; }

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

        //public Stopwatch QueuingDelayStopWatch = new Stopwatch();
    }
}
