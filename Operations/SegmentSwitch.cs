using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.Operations
{
   public class SegmentSwitch
    {

        private Junction _junction;
        // north to north:
        private List<LaneUi> n2n = new List<LaneUi>();
        private List<LaneUi> n2w = new List<LaneUi>();
        private List<LaneUi> n2e = new List<LaneUi>(); 
        // south:
        private List<LaneUi> s2s = new List<LaneUi>();
        private List<LaneUi> s2e = new List<LaneUi>();
        private List<LaneUi> s2w = new List<LaneUi>(); 

        // east:
        private List<LaneUi> e2e = new List<LaneUi>();
        private List<LaneUi> e2n = new List<LaneUi>();
        private List<LaneUi> e2s = new List<LaneUi>(); 

        // west:
        private List<LaneUi> w2w = new List<LaneUi>();
        private List<LaneUi> w2s = new List<LaneUi>();
        private List<LaneUi> w2n = new List<LaneUi>(); 

        public SegmentSwitch(Junction _jun)
        {
            _junction = _jun;

            // n2n:
            if (_junction.ToSouthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToSouthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.N && lan.CurrentSwitchToDirection == Direction.N)
                    {
                        n2n.Add(lan);
                    }
                }
            }
            // laod  n2w lanes:
            if (_junction.ToSouthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToSouthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.N && lan.CurrentSwitchToDirection == Direction.W)
                    {
                        n2w.Add(lan);
                    }
                }
            }

            //n2e
            if (_junction.ToSouthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToSouthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.N && lan.CurrentSwitchToDirection == Direction.E)
                    {
                        n2e.Add(lan);
                    }
                }
            }


            // s2s:
            if (_junction.ToNorthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToNorthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.S && lan.CurrentSwitchToDirection == Direction.S)
                    {
                        s2s.Add(lan);
                    }
                }
            }

            //s2e
            if (_junction.ToNorthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToNorthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.S && lan.CurrentSwitchToDirection == Direction.E)
                    {
                        s2e.Add(lan);
                    }
                }
            }

            //s2w
            if (_junction.ToNorthRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToNorthRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.S && lan.CurrentSwitchToDirection == Direction.W)
                    {
                        s2w.Add(lan);
                    }
                }
            }

            //e2e
            if (_junction.ToWestRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToWestRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.E && lan.CurrentSwitchToDirection == Direction.E)
                    {
                        e2e.Add(lan);
                    }
                }
            }
            //e2n

            if (_junction.ToWestRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToWestRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.E && lan.CurrentSwitchToDirection == Direction.N)
                    {
                        e2n.Add(lan);
                    }
                }
            }

            //e2s
            if (_junction.ToWestRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToWestRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.E && lan.CurrentSwitchToDirection == Direction.S)
                    {
                        e2s.Add(lan);
                    }
                }
            }

            // w2w

            if (_junction.ToEastRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToEastRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.W && lan.CurrentSwitchToDirection == Direction.W)
                    {
                        w2w.Add(lan);
                    }
                }

            }
            // w2s
            if (_junction.ToEastRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToEastRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.W && lan.CurrentSwitchToDirection == Direction.S)
                    {
                        w2s.Add(lan);
                    }
                }
            }
            //w2n

            if (_junction.ToEastRoadSegment != null)
            {
                foreach (LaneUi lan in _junction.ToEastRoadSegment.Lanes)
                {
                    if (lan.LaneDirection == Direction.W && lan.CurrentSwitchToDirection == Direction.N)
                    {
                        w2n.Add(lan);
                    }
                }
            }
        }


        #region North lane
        /// <summary>
        /// from JunctionBottomRoadSegment to JunctionTopRoadSegment
        /// </summary>
        /// <param name="_junction"></param>
        public void SwitchNorthToNorthSegment()
        {
            if (n2n.Count > 0)
            {
                foreach (LaneUi prevlan in n2n)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek(); // take the peek:
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            // take the direct one:
                            RoadSegment tonorthSegment = _junction.ToNorthRoadSegment;
                            if (tonorthSegment != null)
                            {
                                // remove the vehile from
                                int lanIndex = vehicle.CurrentLane.LaneIndex;
                                prevlan.LaneVehicleAndQueue.Dequeue(); // remove from prevlane
                                vehicle.CurrentLane = tonorthSegment.Lanes[lanIndex]; // take the same index.
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                vehicle.ChangeLaneFlage = false;
                                vehicle.Margin = new Thickness(vehicle.CurrentLane.MyCenterLeft, vehicle.Margin.Top, 0, 0);
                                vehicle.SetVehicleDirection(Direction.N);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no Topsegment:
                                // check the right:
                                if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment rightRS = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;
                                    prevlan.lbl_direction_symbol1.Text = "↱";
                                    _junction.ReLoadLanes();

                                }// check the left:
                                else if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment leftRS = _junction.ToWestRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;
                                    prevlan.lbl_direction_symbol1.Text = "↰";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头：north to south:
                                    RoadSegment toSouthSegment = _junction.ToSouthRoadSegment; // select the segment
                                    vehicle.CurrentLane = toSouthSegment.Lanes[LaneIndex.RandomLaneIndex.South];
                                    prevlan.LaneVehicleAndQueue.Dequeue(); // remove from prev:
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); ; // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.S);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();
                                }
                                // change the lane
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// from JunctionBottomRoadSegment to junction left
        /// </summary>
        public void SwitchNorthToWestSegment()
        {
            if (n2w.Count > 0)
            {
                foreach (LaneUi prevlan in n2w)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment switchToRs = _junction.ToWestRoadSegment;
                            if (switchToRs != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = switchToRs.Lanes[LaneIndex.RandomLaneIndex.West]; // the new lane.------------------------------------
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment); ;
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.W);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no left:
                                // check the top: north
                                if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment topRs = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N;
                                    prevlan.lbl_direction_symbol1.Text = "↑";
                                    _junction.ReLoadLanes();

                                }// check the right: east
                                else if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment rightRs = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;
                                    prevlan.lbl_direction_symbol1.Text = "↱";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {

                                    RoadSegment toSouthSegment = _junction.ToSouthRoadSegment; // select the segment
                                    vehicle.CurrentLane = toSouthSegment.Lanes[LaneIndex.RandomLaneIndex.South];
                                    prevlan.LaneVehicleAndQueue.Dequeue(); // remove from prev:
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); ; // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.S);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();

                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// from JunctionBottomRoadSegment to junction right
        /// </summary>
        public void SwitchNorthToEastSegment() 
        {
            if (n2e.Count > 0)
            {
                foreach (LaneUi prevlan in n2e)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment switchToRs = _junction.ToEastRoadSegment;
                            if (switchToRs != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = switchToRs.Lanes[LaneIndex.RandomLaneIndex.East]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.E);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // check the top: north
                                if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment topRs = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N;
                                    prevlan.lbl_direction_symbol1.Text = "↑";
                                    _junction.ReLoadLanes();

                                }// check the left: west
                                else if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment rightRs = _junction.ToWestRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;
                                    prevlan.lbl_direction_symbol1.Text = "↰";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    RoadSegment toSouthSegment = _junction.ToSouthRoadSegment; // select the segment
                                    vehicle.CurrentLane = toSouthSegment.Lanes[LaneIndex.RandomLaneIndex.South];
                                    prevlan.LaneVehicleAndQueue.Dequeue(); // remove from prev:
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.S);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion


        #region South

        // <summary>
        /// from JunctionTopRoadSegment to JunctionBottomRoadSegment
        /// </summary>
        public void SwitchSouthToSouthSegment()
        {
            if (s2s.Count > 0)
            {
                foreach (LaneUi prevlan in s2s)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                       
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toshouthRoad = _junction.ToSouthRoadSegment;
                            if (toshouthRoad != null)
                            {
                                int lanIndex = vehicle.CurrentLane.LaneIndex;
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toshouthRoad.Lanes[lanIndex]; // the new lane.
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                vehicle.ChangeLaneFlage = false;
                                vehicle.Margin = new Thickness(vehicle.CurrentLane.MyCenterLeft, vehicle.Margin.Top, 0, 0);//
                                vehicle.SetVehicleDirection(Direction.S);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no junction bottom:
                                // check the right:
                                if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment rightRS = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;
                                    prevlan.lbl_direction_symbol3.Text = "↳";
                                    _junction.ReLoadLanes();

                                }// check the left:
                                else if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment leftRS = _junction.ToWestRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;
                                    prevlan.lbl_direction_symbol3.Text = "↲";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头：
                                    RoadSegment southRS = _junction.ToNorthRoadSegment;
                                    vehicle.CurrentLane = southRS.Lanes[LaneIndex.RandomLaneIndex.North];
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.N);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  from JunctionTopRoadSegment to junction left
        /// </summary>
        public void SwitchSouthToWestSegment()
        {
            if (s2w.Count > 0)
            {
                foreach (LaneUi prevlan in s2w)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toWestRoad = _junction.ToWestRoadSegment;
                            if (toWestRoad != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toWestRoad.Lanes[LaneIndex.RandomLaneIndex.West]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.W);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no junction bottom:
                                // Check the south:
                                if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment southrRS = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S;
                                    prevlan.lbl_direction_symbol3.Text = "↓";
                                    _junction.ReLoadLanes();

                                }
                                // check the right:
                                else if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment rightRS = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;
                                    prevlan.lbl_direction_symbol3.Text = "↳";
                                    _junction.ReLoadLanes();

                                }
                                else
                                {
                                    // 掉头：
                                    RoadSegment southRS = _junction.ToNorthRoadSegment;
                                    vehicle.CurrentLane = southRS.Lanes[LaneIndex.RandomLaneIndex.North];
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.N);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }
        // <summary>
        /// from JunctionTopRoadSegment to junction right
        /// </summary>
        public void SwitchSouthToEastSegment()
        {

            if (s2e.Count > 0)
            {
                foreach (LaneUi prevlan in s2e)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment ToEastRoad = _junction.ToEastRoadSegment;
                            if (ToEastRoad != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = ToEastRoad.Lanes[LaneIndex.RandomLaneIndex.East]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.E);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no junction bottom:
                                // Check the south:
                                if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment southrRS = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S; // prevlan.SwitchToDirections = Direction.ToSouth;
                                    prevlan.lbl_direction_symbol3.Text = "↓";
                                    _junction.ReLoadLanes();

                                }
                                // check the right:
                                else if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment leftRS = _junction.ToWestRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;
                                    prevlan.lbl_direction_symbol3.Text = "↲";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头：
                                    RoadSegment southRS = _junction.ToNorthRoadSegment;
                                    vehicle.CurrentLane = southRS.Lanes[LaneIndex.RandomLaneIndex.North];
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment); // keep the same speed.
                                    vehicle.SetVehicleDirection(Direction.N);
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region East

        /// <summary>
        /// from junction left to junction right 
        /// </summary>
        public void SwitchEastToEastSegment()
        {
            if (e2e.Count > 0)
            {
                foreach (LaneUi prevlan in e2e)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toEastRoad = _junction.ToEastRoadSegment;
                            if (toEastRoad != null)
                            {
                                int lanIndex = vehicle.CurrentLane.LaneIndex;
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toEastRoad.Lanes[lanIndex]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                vehicle.Margin = new Thickness(vehicle.Margin.Left, vehicle.CurrentLane.MyCenterTop, 0, 0);//
                                vehicle.SetVehicleDirection(Direction.E);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no right (east) segment.
                                // Check the top one:
                                if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment gonorth = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N;// prevlan.SwitchToDirections = Direction.ToNorth;
                                    prevlan.lbl_direction_symbol3.Text = "⬏";
                                    _junction.ReLoadLanes();

                                }
                                // check the south:
                                else if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment gosouth = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S;//  prevlan.SwitchToDirections = Direction.ToSouth;
                                    prevlan.lbl_direction_symbol3.Text = "⬎";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment westRS = _junction.ToWestRoadSegment;
                                    vehicle.CurrentLane = westRS.Lanes[LaneIndex.RandomLaneIndex.West];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.W);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }




        /// <summary>
        /// from junction left to junction top 
        /// </summary>
        public void SwitchEastToNorthSegment()
        {
            if (e2n.Count > 0)
            {
                foreach (LaneUi prevlan in e2n)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toNorthRoad = _junction.ToNorthRoadSegment;
                            if (toNorthRoad != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toNorthRoad.Lanes[LaneIndex.RandomLaneIndex.North]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.N);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no NORTH (TOP) segment.
                                // Check the DIRECT one: 
                                if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment goRight = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;// prevlan.SwitchToDirections = Direction.ToEast;
                                    prevlan.lbl_direction_symbol3.Text = "→";
                                    _junction.ReLoadLanes();

                                }
                                // check the south:
                                else if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment gosouth = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S;//  prevlan.SwitchToDirections = Direction.ToSouth;
                                    prevlan.lbl_direction_symbol3.Text = "⬎";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment westRS = _junction.ToWestRoadSegment;
                                    vehicle.CurrentLane = westRS.Lanes[LaneIndex.RandomLaneIndex.West];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.W);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// from junction left to junction top 
        /// </summary>
        public void SwitchEastToSouthSegment()
        {
            if (e2s.Count > 0)
            {
                foreach (LaneUi prevlan in e2s)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toSouth = _junction.ToSouthRoadSegment;
                            if (toSouth != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toSouth.Lanes[LaneIndex.RandomLaneIndex.South]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.S);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no south (bot) segment.
                                // Check the DIRECT one: 
                                if (_junction.ToEastRoadSegment != null)
                                {
                                    RoadSegment goRight = _junction.ToEastRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.E;//  prevlan.SwitchToDirections = Direction.ToEast;
                                    prevlan.lbl_direction_symbol3.Text = "→";
                                    _junction.ReLoadLanes();

                                }
                                // check the north:
                                else if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment gonorth = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N;//  prevlan.SwitchToDirections = Direction.ToNorth;
                                    prevlan.lbl_direction_symbol3.Text = "⬏";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment westRS = _junction.ToWestRoadSegment;
                                    vehicle.CurrentLane = westRS.Lanes[LaneIndex.RandomLaneIndex.West];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.W);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region West


        /// <summary>
        /// junction right to junction left.
        /// </summary>
        public void SwitchWestToWestSegment()
        {
            if (w2w.Count > 0)
            {
                foreach (LaneUi prevlan in w2w)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment switchToRs = _junction.ToWestRoadSegment;
                            if (switchToRs != null)
                            {
                                int lanIndex = vehicle.CurrentLane.LaneIndex;
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = switchToRs.Lanes[lanIndex]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                vehicle.Margin = new Thickness(vehicle.Margin.Left, vehicle.CurrentLane.MyCenterTop, 0, 0);//
                                vehicle.SetVehicleDirection(Direction.W);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no left (west) segment.
                                // Check the top one:
                                if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment gonorth = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N;//  prevlan.SwitchToDirections = Direction.ToNorth;
                                    prevlan.lbl_direction_symbol1.Text = "⬑";
                                    _junction.ReLoadLanes();

                                }
                                // check the south:
                                else if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment gosouth = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S;//  prevlan.SwitchToDirections = Direction.ToSouth;
                                    prevlan.lbl_direction_symbol1.Text = "⬐";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment goreversers = _junction.ToEastRoadSegment;
                                    vehicle.CurrentLane = goreversers.Lanes[LaneIndex.RandomLaneIndex.East];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.E);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  from junction right to junction bottom
        /// </summary>
        public void SwitchWestToSouthSegment()
        {
            if (w2s.Count > 0)
            {
                foreach (LaneUi prevlan in w2s)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toShouth = _junction.ToSouthRoadSegment;
                            if (toShouth != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toShouth.Lanes[LaneIndex.RandomLaneIndex.South]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.S);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no south (bottom) segment.
                                // Check the direct one:
                                if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment gowest = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;// prevlan.SwitchToDirections = Direction.ToWest;
                                    prevlan.lbl_direction_symbol1.Text = "←";
                                    _junction.ReLoadLanes();

                                }
                                // check the north:
                                else if (_junction.ToNorthRoadSegment != null)
                                {
                                    RoadSegment goNorth = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.N; //  prevlan.SwitchToDirections = Direction.ToNorth;
                                    prevlan.lbl_direction_symbol1.Text = "⬑";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment goreversers = _junction.ToEastRoadSegment;
                                    vehicle.CurrentLane = goreversers.Lanes[LaneIndex.RandomLaneIndex.East];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.E);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  from junction right to junction top
        /// </summary>
        public void SwitchWestToNorthSegment() 
        {
            if (w2n.Count > 0)
            {
                foreach (LaneUi prevlan in w2n)
                {
                    if (prevlan.LaneVehicleAndQueue.CountInQueue > 0)
                    {
                        VehicleUi vehicle = prevlan.LaneVehicleAndQueue.Peek();
                        Junction jun = prevlan.GetMyHeadingJunction; // this.
                        if (jun != null)
                        {
                            RoadSegment toNorth = _junction.ToNorthRoadSegment;
                            if (toNorth != null)
                            {
                                prevlan.LaneVehicleAndQueue.Dequeue();
                                vehicle.CurrentLane = toNorth.Lanes[LaneIndex.RandomLaneIndex.North]; // the new lane.
                                vehicle.ChangeLaneFlage = false;
                                vehicle.InstantaneousSpeed = Computations.RandomTimeInterval(vehicle.CurrentLane.MyRoadSegment);
                                Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                vehicle.SetVehicleDirection(Direction.N);
                                vehicle.VechileEnginTimer.Start();
                            }
                            else
                            {
                                // junction has no north (top) segment.
                                // Check the direct one:
                                if (_junction.ToWestRoadSegment != null)
                                {
                                    RoadSegment gowest = _junction.ToNorthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.W;// prevlan.SwitchToDirections = Direction.ToWest;
                                    prevlan.lbl_direction_symbol1.Text = "←";
                                    _junction.ReLoadLanes();

                                }
                                // check the south:
                                else if (_junction.ToSouthRoadSegment != null)
                                {
                                    RoadSegment goNorth = _junction.ToSouthRoadSegment;
                                    prevlan.CurrentSwitchToDirection = Direction.S;// prevlan.SwitchToDirections = Direction.ToSouth;
                                    prevlan.lbl_direction_symbol1.Text = "⬐";
                                    _junction.ReLoadLanes();
                                }
                                else
                                {
                                    // 掉头： reverse:
                                    prevlan.LaneVehicleAndQueue.Dequeue();
                                    vehicle.ChangeLaneFlage = false;
                                    RoadSegment goreversers = _junction.ToEastRoadSegment;
                                    vehicle.CurrentLane = goreversers.Lanes[LaneIndex.RandomLaneIndex.East];
                                    Point entry = vehicle.CurrentLane.MyEntry(vehicle);
                                    vehicle.VechileEnginTimer.Interval = Computations.RandomTimeSpane(vehicle.CurrentLane.MyRoadSegment);
                                    vehicle.Margin = new Thickness(entry.X, entry.Y, 0, 0);
                                    vehicle.SetVehicleDirection(Direction.E);
                                    vehicle.VechileEnginTimer.Start();
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        #endregion 





    }
}