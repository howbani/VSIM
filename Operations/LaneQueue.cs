using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.Operations
{
    /// <summary>
    /// quee to manage the vechiles.
    /// </summary>
    public class LaneQueue
    {

        
        private List<VehicleUi> Queue = new List<VehicleUi>();// the vechile that lining up in the juntion.
        #region Queue
        public LaneQueue()
        {

        }

        /// <summary>
        /// add to the tail.
        /// </summary>
        /// <param name="veh"></param>
        public void Enqueue(VehicleUi veh)
        {
            if (!Queue.Contains(veh))
            {
                Queue.Add(veh);
                veh.IndexInQueue = Queue.Count;
                veh.lbl_show_info.Foreground = System.Windows.Media.Brushes.OrangeRed;
            }
        }
        /// <summary>
        /// remove the head.
        /// </summary>
        /// <returns></returns>
        public VehicleUi Dequeue()
        {
            if (Queue.Count > 0)
            {
                if (Queue[0] != null)
                {
                    
                    VehicleUi re = Queue[0];
                    re.lbl_show_info.Foreground = System.Windows.Media.Brushes.Black;
                    LaneUi ulane = re.CurrentLane;
                   
                    RemoveFromLane(re);// from the lane
                    Queue.Remove(re); // from the queue.
                    List<VehicleUi> back = new List<VehicleUi>();
                    back.AddRange(Queue);
                    Queue.Clear();
                    foreach (VehicleUi ve in back)
                    {
                        Enqueue(ve);
                    }
                    ulane._MainWindow.Dispatcher.Invoke(new Action(() => ulane.lbl_info.Text = ulane.LaneVehicleAndQueue.CountInLane.ToString()), DispatcherPriority.Send);
                    return re;
                }
                else return null;
            }
            else return null;
        }
        /// <summary>
        /// the number of vechile in the queue in the junction.
        /// </summary>
        public int CountInQueue { get { return Queue.Count; } }

        /// <summary>
        /// get the head.
        /// </summary>
        /// <returns></returns>
        public VehicleUi Peek()
        {
            if (Queue.Count > 0)
            {
                if (Queue[0] != null)
                {
                    return Queue[0];
                }
                else return null;
            }
            else return null;
        }
        #endregion

        /*-----------------------------------------------------------------------------------------/*
        /*----------------------------------------------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------/*
        /*----------------------------------------------------------------------------------------*/

        #region inlane


        public List<VehicleUi> LaneVechilesList = new List<VehicleUi>(); // vechile that on the lane right now.
        /// <summary>
        /// add to the lane.
        /// </summary>
        /// <param name="v"></param>
        public void AddToLane( VehicleUi v)
        {
            if (!LaneVechilesList.Contains(v))
            {
                LaneVechilesList.Add(v);
            }
        }
        /// <summary>
        /// remove from the lane
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool RemoveFromLane(VehicleUi v)
        {
            return LaneVechilesList.Remove(v);
        }

        /// <summary>
        /// how many vehicle in the lane.
        /// </summary>
        /// <returns></returns>
        public int CountInLane
        {
            get
            {
                return LaneVechilesList.Count;
            }
        }

        /// <summary>
        /// retuen the ve that very close to me.
        /// inputVe should be infront of outVE.
        /// </summary>
        /// <returns></returns>
        public VehicleUi  GetMyFrontVehicle(VehicleUi Currentvehi) 
        {
            foreach (VehicleUi fronV in Currentvehi.CurrentLane.LaneVehicleAndQueue.LaneVechilesList)
            {
                if (fronV != Currentvehi)
                {
                    double traveledDiffrence = fronV.TravelledDistanceInMeter - Currentvehi.TravelledDistanceInMeter;
                    if ((traveledDiffrence > 0) && (traveledDiffrence <= (Currentvehi.Height * 1.3))) // increase the distance between the vechiles when line up.
                    {
                        return fronV;
                    }
                }
            }
            return null;
        }


        
        /// <summary>
        /// get the neighbors for the Currentvehi. in the two ways without consider the direction.
        /// </summary>
        /// <param name="Currentvehi"></param>
        /// <returns></returns>
        public void GetIntraNeighborsTwoWays(VehicleUi Currentvehi)
        {
            Currentvehi.Intra_Neighbores.Clear();
            List<VehicleUi> allInSameDirection = new List<RoadNet.Components.VehicleUi>();
            foreach (LaneUi lane in Currentvehi.CurrentLane.MyRoadSegment.Lanes)
            {
                allInSameDirection.AddRange(lane.LaneVehicleAndQueue.LaneVechilesList);
            }

            foreach (VehicleUi Intra_vehicle in allInSameDirection)
            {
                // neighbore should be infront of mine and in the same direction and within my range.
                if (Intra_vehicle != Currentvehi)
                {
                    double dis = Computations.Distance(Intra_vehicle.InstanceLocation, Currentvehi.InstanceLocation);
                    if (dis < PublicParamerters.CommunicationRaduis)
                    {
                        Currentvehi.Intra_Neighbores.Add(Intra_vehicle);
                    }
                }
            }
        }

        /// <summary>
        /// vehicles which are in the front of me.
        /// </summary>
        /// <param name="Currentvehi"></param>
        /// <returns></returns>
        public void GetIntraNeighborOneWayInfront(VehicleUi Currentvehi)
        {
            Currentvehi.Intra_Neighbores.Clear();
            List<VehicleUi> allInSameDirection = new List<RoadNet.Components.VehicleUi>();
            foreach (LaneUi lane in Currentvehi.CurrentLane.MyRoadSegment.Lanes)
            {
                if (Currentvehi.VehicleDirection == lane.LaneDirection)
                {
                    allInSameDirection.AddRange(lane.LaneVehicleAndQueue.LaneVechilesList);
                }
            }

            foreach (VehicleUi fronV in allInSameDirection)
            {
                if (fronV != Currentvehi)
                {
                    double traveledDiffrence = fronV.TravelledDistanceInMeter - Currentvehi.TravelledDistanceInMeter;
                    if (traveledDiffrence < PublicParamerters.CommunicationRaduis)
                    {
                        if ((traveledDiffrence > 0) && (traveledDiffrence <= (Currentvehi.Height * 2))) // increase the distance between the vechiles when line up.
                        {
                            Currentvehi.Intra_Neighbores.Add(fronV);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// get the neighbors for the vehicle within the roadSegment. note that the vechile now is not in the roadSegment. This required when the vechile is almost in the junction. so it required to switch to a new road segment.
        /// </summary>
        /// <param name="vehicle">current ve</param>
        /// <param name="Packetdirection"> the packet direction </param>
        /// <param name="selectedNextRoadSegment"> the road segment</param>
        /// <returns>the inter_nei for vehicle</returns>
        public void GetInterNeighbors(VehicleUi vehicle, Direction Packetdirection, RoadSegment selectedNextRoadSegment)
        {
            vehicle.Inter_Neighbores.Clear();
            List<VehicleUi> allInSameDirection = new List<RoadNet.Components.VehicleUi>();
            foreach (LaneUi lane in selectedNextRoadSegment.Lanes)
            {
               if (lane.LaneDirection == Packetdirection)
                {
                    allInSameDirection.AddRange(lane.LaneVehicleAndQueue.LaneVechilesList);
                }
            }

            int vid = vehicle.VID;
            int rid = selectedNextRoadSegment.RID;

            List<VehicleUi> neibore = new List<RoadNet.Components.VehicleUi>();
            foreach (VehicleUi Inter_vehicle in allInSameDirection)
            {
                
                double dis = Computations.Distance(vehicle.InstanceLocation, Inter_vehicle.InstanceLocation);
                if (dis < PublicParamerters.CommunicationRaduis)
                {
                    vehicle.Inter_Neighbores.Add(Inter_vehicle);
                    int v_id = Inter_vehicle.VID;
                }
            }

               // consider the vhicles which are in the front of me and in the same direction too.
               GetIntraNeighborOneWayInfront(vehicle);
               vehicle.Inter_Neighbores.AddRange(vehicle.Intra_Neighbores);

        }




        #endregion

    }
}
