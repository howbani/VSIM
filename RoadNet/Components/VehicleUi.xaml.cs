using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VANET_SIM.Operations;
using VANET_SIM.Routing;
using VANET_SIM.Vpackets;
using static RandomColorsGenerator;

namespace VANET_SIM.RoadNet.Components
{
    public enum NotationsSign { HasNoPacket, HasPacket, Destination, Default }
    /// <summary>
    /// Interaction logic for VehicleUi.xaml
    /// </summary>
    public partial class VehicleUi : UserControl
    {

        public List<long> WaitingPacketsIDsList = new List<long>(); // the packets that will send to me. 
        public int IndexInQueue { get; set; } // my index in the queue.
        public List<VehicleUi> Intra_Neighbores = new List<VehicleUi>(); // vechiles which are in the same raod segment.
        public List<VehicleUi> Inter_Neighbores = new List<VehicleUi>(); // not in the same segment.
        public Direction VehicleDirection { get { return CurrentLane.LaneDirection; } }

        public DispatcherTimer VehicleNeighboresDiscoveryTimer = new DispatcherTimer(); 
        public DispatcherTimer VechileEnginTimer = new DispatcherTimer();
        public Queue<Packet> PacketQueue = new Queue<Packet>((int)PublicParamerters.BufferSize);
        public DispatcherTimer PacketQueueTimer = new DispatcherTimer(); // the forwarder of the packets.
        public VehicleUi()
        {
            InitializeComponent();
            
            Height = PublicParamerters.VehicleHight;
            Width = PublicParamerters.VehicleHight;
           // txt_vehicle.Foreground = new DarkColore().Random;
           // txt_vehicle.Foreground = Brushes.Yellow;

           
            VechileEnginTimer.Tick += MobilityTimer_Tick;
            VechileEnginTimer.Start();

            PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval);
            PacketQueueTimer.Tick += PacketQueueTimer_Tick;

           
        }

        /*--------------------General--------------------------*/
        #region General
        private int _VID;
        public int VID
        {
            get { return _VID; }
            set
            {
                _VID = value;
                txt_vid.Text = _VID.ToString();
            }
        }

        /// <summary>
        /// visualize the flags in UI of v
        /// </summary>
        /// <param name="sign"></param>
        public void SetNotationSign(NotationsSign sign)
        {
            // packet in the queue
            if (sign == NotationsSign.HasPacket)
            {
                if (PacketQueue.Count >= 1)
                {
                    //grid_notation.Background = Brushes.Red;
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Red), DispatcherPriority.Input);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Transparent), DispatcherPriority.Input);
                }
            } // no packt in the queu
            else if (sign == NotationsSign.HasNoPacket)
            {
                if (PacketQueue.Count == 0)
                {
                    //  grid_notation.Background = Brushes.Transparent;
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Transparent), DispatcherPriority.Input);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Red), DispatcherPriority.Input);
                }
            } // the source
            else if (sign == NotationsSign.Destination)
            {
                if (WaitingPacketsIDsList.Count > 0)
                {
                    Dispatcher.Invoke(new Action(() => txt_vehicle.Foreground = Brushes.DarkOrange), DispatcherPriority.Input);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => txt_vehicle.Foreground = Brushes.Yellow), DispatcherPriority.Input);
                }
            } // the defualt. 
            else if (sign == NotationsSign.Default)
            {
                if (WaitingPacketsIDsList.Count > 0)
                {
                    Dispatcher.Invoke(new Action(() => txt_vehicle.Foreground = Brushes.DarkOrange), DispatcherPriority.Input);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => txt_vehicle.Foreground = Brushes.Yellow), DispatcherPriority.Input);
                }

                if (PacketQueue.Count == 0)
                {
                    //  grid_notation.Background = Brushes.Transparent;
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Transparent), DispatcherPriority.Input);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => grid_notation.Background = Brushes.Red), DispatcherPriority.Input);
                }
            }
        }


        /// <summary>
        /// the junction which this vechile is moving towards.
        /// </summary>
        public Junction EndJunction
        {
            get
            {
                return this.CurrentLane.GetMyHeadingJunction;
            }
        }

        /// <summary>
        /// get the start junction.
        /// </summary>
        public Junction StartJunction
        {
            get
            {
                Junction endJun = EndJunction;
                RoadSegment rs = this.CurrentLane.MyRoadSegment;
                if (rs.MyJunctions[0] != null)
                {
                    if (rs.MyJunctions[0] != endJun) { return rs.MyJunctions[0]; }
                }

                if (rs.MyJunctions[1] != null)
                {
                    if (rs.MyJunctions[1] != endJun) { return rs.MyJunctions[1]; }
                }

                return null;
            }
        }

        public enum Vehicleinfo { SpeedKMH, SwitchDirection, Direction, VID, None, SpeedTimer, RID, SJID, DJID, LaneIndex, InstanceLocation, RemianDistanceToHeadingJunction, TravelledDistanceInMeter, ExceededDistanceInMeter, PacketsQueueLength, JunctionQueueIndex, RIDpluseLaneIndex }
        public void showInfo(Vehicleinfo vinfo)
        {

            if (vinfo == Vehicleinfo.None)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = ""), DispatcherPriority.Send);
            }
            if (vinfo == Vehicleinfo.SwitchDirection)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = CurrentLane.CurrentSwitchToDirection), DispatcherPriority.Send);
            }
            if (vinfo == Vehicleinfo.Direction)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = CurrentLane.LaneDirection), DispatcherPriority.Send);
            }

            if (vinfo == Vehicleinfo.VID)
            {
                txt_vid.Visibility = Visibility.Hidden;
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = VID), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.SpeedKMH)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = GetSpeedInKMH.ToString("0.00")), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.SpeedTimer)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = InstantaneousSpeed.ToString("0.00")), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.RID)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = CurrentLane.MyRoadSegment.RID), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.SJID)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = StartJunction.JID), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.DJID)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = EndJunction.JID), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.LaneIndex)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = CurrentLane.LaneIndex), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.InstanceLocation)
            {
                int X = Convert.ToInt32(InstanceLocation.X);
                int Y = Convert.ToInt32(InstanceLocation.Y);
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = X + "," + Y), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.RemianDistanceToHeadingJunction)
            {

                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = RemianDistanceToHeadingJunction.ToString("0.00")), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.TravelledDistanceInMeter)
            {

                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = TravelledDistanceInMeter.ToString("0.00")), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.ExceededDistanceInMeter)
            {

                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = ExceededDistanceInMeter.ToString("0.00")), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.PacketsQueueLength)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = PacketQueue.Count), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.JunctionQueueIndex)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = IndexInQueue), DispatcherPriority.Send);
            }
            else if (vinfo == Vehicleinfo.RIDpluseLaneIndex)
            {
                Dispatcher.Invoke(new Action(() => lbl_show_info.Content = CurrentLane.MyRoadSegment.RID + "-" + CurrentLane.LaneIndex), DispatcherPriority.Send);
            }




        }

        #endregion





        /*--------------------Onboad unit--------------------------*/
        #region Onboad unit

        /// <summary>
        /// get the next raod segment. Inter_routing.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public RoadSegment MatchJunction(Packet packet)
        {
            packet.SourceJunction = this.StartJunction;
            packet.HeadingJunction = EndJunction; // the end junction of current vehicle. // this needs to be changed when the vechile is close to the end of the jucntion and has a packet.
            InterRouting re = new Routing.InterRouting();
            if (packet.HeadingJunction != packet.DestinationJunction)
            {
                CandidateJunction cand = re.CandidateJunction(packet.HeadingJunction, packet.DestinationJunction); // the i=HeadingJunction. we would like to select the next junction

                //  Console.WriteLine("S JID " + packet.SourceJunction.JID + " D JID" + packet.DestinationJunction.JID);
                //   Console.WriteLine("C JID " + packet.HeadingJunction.JID);
                if (cand != null)
                {
                    // get the dpacket direction:
                    RoadSegment rs = cand.NextRoadSegment;
                    if (rs.Roadorientation == RoadOrientation.Horizontal)
                    {
                        // the first lane is head to EndJunction.
                        if (rs.Lanes[0].GetMyHeadingJunction == EndJunction)
                            packet.Direction = rs.Lanes[(int)PublicParamerters.NumberOfLanes - 1].LaneDirection;
                        else
                            packet.Direction = rs.Lanes[0].LaneDirection;

                    }
                    else
                    {
                        // horizontale:
                        if (rs.Lanes[0].GetMyHeadingJunction == EndJunction)
                            packet.Direction = rs.Lanes[(int)PublicParamerters.NumberOfLanes - 1].LaneDirection;
                        else
                            packet.Direction = rs.Lanes[0].LaneDirection;
                    }

                    // set the segment:
                    packet.CurrentRoadSegment = rs;
                    //  packet.CurrentRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.OrangeRed;
                    //     Console.WriteLine("Inter-Routing(MatchJunction) RID " + rs.RID + " has been selected as new Road Segment ");
                    return rs;
                }
            }
            else
            {
                RoadSegment rs = packet.DestinationVehicle.CurrentLane.MyRoadSegment;
                if (rs.Roadorientation == RoadOrientation.Horizontal)
                {
                    // the first lane is head to EndJunction.
                    if (rs.Lanes[0].GetMyHeadingJunction == EndJunction)
                        packet.Direction = rs.Lanes[(int)PublicParamerters.NumberOfLanes - 1].LaneDirection;
                    else
                        packet.Direction = rs.Lanes[0].LaneDirection;

                }
                else
                {
                    // horizontale:
                    if (rs.Lanes[0].GetMyHeadingJunction == EndJunction)
                        packet.Direction = rs.Lanes[(int)PublicParamerters.NumberOfLanes - 1].LaneDirection;
                    else
                        packet.Direction = rs.Lanes[0].LaneDirection;
                }
                packet.CurrentRoadSegment = rs;
                return rs;
            }

            return null;
        }


        /// <summary>
        /// select the next vechile. Intra_Routing.
        /// </summary>
        /// <param name="candidateVe"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public VehicleUi MatchVehicle(List<VehicleUi> candidateVe, RoadSegment rs, Packet packet)
        {
            IntraRouting rou = new IntraRouting(rs);
            CandidateVehicle can = rou.GetCandidateVehicleUis(this, candidateVe, packet.DestinationVehicle, packet.IsRouted);
            if (can != null)
            {
                VehicleUi next = can.SelectedVehicle;
                if (packet.Hops_V > 3)
                {
                    int lastMinuse1 = Convert.ToInt16(packet.VehiclesString.Split('-')[packet.Hops_V-1]);
                    if (next.VID != lastMinuse1)
                    {
                        return next;
                    }
                    else
                    {
                        // looop: no thing.
                    }
                }
                else
                {
                    return next;
                }
            }

            return null;
        }
        /// <summary>
        /// select the the DestinationVehicle randomly.
        /// </summary>
        public void RandomDestinationVehicle()
        {
            int max = CurrentLane._MainWindow.MyVehicles.Count;
            if (max >= 2)
            {
                int rand = Convert.ToInt16(RandomeNumberGenerator.GetUniform(max - 1));
                if (rand != VID)
                {
                    VehicleUi DestinationVehicle = CurrentLane._MainWindow.MyVehicles[rand];
                    GeneratePacket(DestinationVehicle);

                }
            }
        }

        /// <summary>
        /// when the queue is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PacketQueueTimer_Tick(object sender, EventArgs e)
        {
            // take the top one and send the packet.
            Packet pack = PacketQueue.Peek();
            if (pack != null)
            {
                // has packet.
                PacketQueue.Dequeue(); // remove the packet from the queue.
                Send(pack); // try to send it. if re-sent more than 7 times then it should be removed.
                if (PacketQueue.Count == 0)
                {
                    // has no packet in the queue.
                    PacketQueueTimer.Stop();
                    SetNotationSign(NotationsSign.HasNoPacket);
                }
                else
                {
                    SetNotationSign(NotationsSign.HasPacket);
                }
                
            }
        }

        /// <summary>
        /// generate data packet from this vechile to the TargetVehicle.
        /// </summary>
        /// <param name="DestinationVehicle"></param>
        public void GeneratePacket(VehicleUi DestinationVehicle)
        {
            Packet packet = new Packet();

            packet.Type = PacketType.Data;
            PublicStatistics.GneratedPacketsCount += 1;
            packet.PID = PublicStatistics.GneratedPacketsCount;
            packet.PacketLength = PublicParamerters.DataPacketLength;

            packet.TravelledRoadSegmentString += CurrentLane.MyRoadSegment.RID; // add the first rs.
            packet.VehiclesString += VID; // start by current sender.
            packet.SRID = CurrentLane.MyRoadSegment.RID;
            // set source and distination:
            packet.SourceVehicle = this;
            packet.DestinationVehicle = DestinationVehicle;
            packet.Direction = DestinationVehicle.CurrentLane.LaneDirection;
            packet.CurrentRoadSegment = CurrentLane.MyRoadSegment; // the segment of the vechile.

            packet.EuclideanDistance = Computations.Distance(InstanceLocation, DestinationVehicle.InstanceLocation); // the intial distance 
            packet.RoutingDistance = packet.EuclideanDistance;
            packet.SVID = VID;
            packet.DVID = DestinationVehicle.VID;
            // Console.WriteLine("VID " + VID + "generates PID " + packet.PID + " to be sent to VID " + DestinationVehicle.VID);

            //:
            // packet.CurrentRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Green; //source
            //  packet.DestinationRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Black; // dist

            this.SetNotationSign(NotationsSign.HasPacket);
            DestinationVehicle.SetNotationSign(NotationsSign.Destination);
            DestinationVehicle.WaitingPacketsIDsList.Add(packet.PID); // flage 

            // start count the delay.
            PacketQueue.Enqueue(packet); // add the packet to the queue.
            packet.QueuingDelayStopWatch.Start(); // start
            PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval); // retry after...
            PacketQueueTimer.Start(); // start the timer.
        }




        /// <summary>
        /// send the packet.
        /// </summary>
        /// <param name="packet"></param>
        public void Send(Packet packet)
        {
            if (packet.Type == PacketType.Data)
            {
                // it is time to switch the packet from a segment to a new segment.
                if (RemianDistanceToHeadingJunction <= PublicParamerters.RemianDistanceToHeadingJunctionThreshold)
                {
                    //select the next junction.
                    RoadSegment selectedNextRoadSegment = MatchJunction(packet);
                    if (selectedNextRoadSegment != null)
                    {
                        // select the vechiles that going to the selected next road segment.
                        // select inter-neighbors.
                        CurrentLane.LaneVehicleAndQueue.GetInterNeighbors(this, packet.Direction, selectedNextRoadSegment); // get the inter_neighbors. should be computed before finding the
                        VehicleUi next = MatchVehicle(Inter_Neighbores, selectedNextRoadSegment, packet); // inter_neibors.
                        if (next != null)
                        {
                            packet.QueuingDelayStopWatch.Stop();
                            PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval); // retry after...
                            packet.PropagationAndTransmissionDelay += DelayModel.Delay(this, next);
                            packet.Hops_J += 1;
                            packet.TravelledRoadSegmentString += "-" + selectedNextRoadSegment.RID;
                            packet.Hops_V += 1;
                            packet.RoutingDistance += Computations.Distance(InstanceLocation, next.InstanceLocation);
                            packet.VehiclesString += "-" + next.VID;
                            next.RecievePacket(packet);
                            PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval); // retry after...
                            next.SetNotationSign(NotationsSign.HasPacket);
                            SetNotationSign(NotationsSign.HasNoPacket);
                        }
                        else
                        {
                            packet.TotalWaitingTimes += 1;
                            PacketQueue.Enqueue(packet); // add the packet to the queue.
                            packet.QueuingDelayStopWatch.Start(); // resume the queue delay counter.
                            PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueRetryTimerInterval); // retry after...
                            PacketQueueTimer.Start(); // start the timer.
                        }
                    }
                }
                else
                {
                    CurrentLane.LaneVehicleAndQueue.GetIntraNeighborsTwoWays(this); // find the inter_neigbors.
                    VehicleUi next = MatchVehicle(Intra_Neighbores, CurrentLane.MyRoadSegment, packet); // intra_neighbors.
                    if (next != null)
                    {
                        packet.QueuingDelayStopWatch.Stop();
                        packet.PropagationAndTransmissionDelay += DelayModel.Delay(this, next);
                        packet.Hops_V += 1;
                        packet.RoutingDistance += Computations.Distance(InstanceLocation, next.InstanceLocation);
                        packet.VehiclesString += "-" + next.VID;
                        next.RecievePacket(packet);
                        PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval); // retry after...
                        next.SetNotationSign(NotationsSign.HasPacket);
                        SetNotationSign(NotationsSign.HasNoPacket);
                    }
                    else
                    {
                        packet.TotalWaitingTimes += 1;
                        PacketQueue.Enqueue(packet); // add the packet to the queue.
                        packet.QueuingDelayStopWatch.Start(); // resume the queue delay counter.
                        PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueRetryTimerInterval); // retry after...
                        PacketQueueTimer.Start(); // start the timer.
                    }
                }
            }
            else
            {
                // packet is not data.
            }
        }

        public void RecievePacket(Packet packet)
        {
            if (this == packet.DestinationVehicle)
            {
                packet.QueuingDelayStopWatch.Stop(); // stop the delay counter.
                packet.isDelivered = true; //
                PublicStatistics.DelaySumInSeconds += packet.DelayInSeconds; //  all delayes.
                PublicStatistics.HopsSum += packet.Hops_V;
                PublicStatistics.SumRoutingDistance += packet.RoutingDistance;

                PublicStatistics.DeleiverdPacketsList.Add(packet);
                double x = PublicStatistics.DeleiverdPacketsCount; // trigger the display.
                double xx = PublicStatistics.InQueuePackets; //trigger the display.

                //  Console.WriteLine("-------------------- Packert " + packet.PID + " is sucessfully recived-------------------");
                packet.SourceVehicle.SetNotationSign(NotationsSign.Default);
                WaitingPacketsIDsList.Remove(packet.PID);
                SetNotationSign(NotationsSign.Default);
                packet.DestinationVehicle.SetNotationSign(NotationsSign.Default);
            }
            else
            {
                // re-transmit the packet.
                if (packet.WaitingThreshold <= PublicParamerters.MaximumNumberofRetransmission)
                {
                    // forward the packet.
                    //check if this is the destination segment. then you should no select any
                    PacketQueue.Enqueue(packet); // put the packet in the queue.
                    packet.QueuingDelayStopWatch.Start(); // resume the queue delay counter.
                    PacketQueueTimer.Start(); // run the transmitter.
                    PacketQueueTimer.Interval = TimeSpan.FromSeconds(PublicParamerters.PacketQueueTimerInterval); // retry after...
                }
                else
                {
                    // drop the packet:
                    packet.isDelivered = false; //
                    packet.QueuingDelayStopWatch.Stop(); // stop the delay counter.
                    PublicStatistics.DropedPacketsList.Add(packet);
                    double x = PublicStatistics.DropedPacketsCount;
                    packet.SourceVehicle.SetNotationSign(NotationsSign.Default);
                    packet.DestinationVehicle.WaitingPacketsIDsList.Remove(packet.PID);
                    SetNotationSign(NotationsSign.Default);
                    packet.DestinationVehicle.SetNotationSign(NotationsSign.Default);
                }
            }
        }

        #endregion


        /*--------------------Moblity--------------------------*/
        #region Moblity

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InstantaneousSpeed = Computations.RandomTimeInterval(CurrentLane.MyRoadSegment); //
        }

        /// <summary>
        /// return the remianing time in this segment for this vechile.
        /// </summary>
        public double ResideTimeInTheSegment
        {
            get
            {
                double distance_m = RemianDistanceToHeadingJunction; // the remian distance in m.
                double speed_mps = UnitsConverter.KmhToMps(GetSpeedInKMH);// The mean speed.
                double re = UnitsConverter.GetTimeinSecond(speed_mps, distance_m);
              //  Console.WriteLine("ResideTimeInTheSegment  " + re);
                return re;
            }
        }

        private LaneUi _ulane;
        public LaneUi CurrentLane
        {
            get
            {
                return _ulane;
            }
            set
            {
                _ulane = value;
                _ulane.LaneVehicleAndQueue.AddToLane(this);
                _ulane.lbl_info.Text = _ulane.LaneVehicleAndQueue.CountInLane.ToString();
            }
        }

        /// <summary>
        /// the speed is represented as time interval of the timer.
        /// 
        /// </summary>
        public double InstantaneousSpeed
        {
            get { return VechileEnginTimer.Interval.TotalSeconds; }
            set
            {
                try
                {
                    if (value > 0)
                    {
                        //0.1
                        VechileEnginTimer.Interval = TimeSpan.FromSeconds(value);
                        VechileEnginTimer.Start();
                    }
                    else
                    {
                        VechileEnginTimer.Interval = TimeSpan.FromSeconds(0.1);
                        VechileEnginTimer.Start();
                    }
                }
                catch
                {
                    //0.1
                    VechileEnginTimer.Interval = TimeSpan.FromSeconds(0.1);
                    VechileEnginTimer.Start();
                }
            }
        }


        /// <summary>
        /// convert the time interval to KMH
        /// </summary>
        public double GetSpeedInKMH
        {
            get
            {
                double s = UnitsConverter.TimerIntervalToKmph(InstantaneousSpeed);
                return s;
            }
        }

       
        /// <summary>
        /// the instance location.
        /// </summary>
        public Point InstanceLocation 
        {
            get
            {
                double x = this.Margin.Left;
                double y = this.Margin.Top;
                Point p = new Point(x, y);
                return p;
            }
        }

        /// <summary>
        /// the remian distance to the junction of the this segment.
        /// </summary>
        public double RemianDistanceToHeadingJunction
        {
            get
            {
                Point myInstanceLocation = InstanceLocation;
                Point headingJunction = CurrentLane.MyeExit(this);
                double d = Computations.Distance(myInstanceLocation, headingJunction);
                return d;
            }
        }
        /// <summary>
        /// how long distance the v has been travelled of this segment.
        /// </summary>
        public double TravelledDistanceInMeter
        {
            get
            {
                if (CurrentLane.MyRoadSegment.Roadorientation == RoadOrientation.Vertical)
                {
                    return CurrentLane.MyRoadSegment.Height - RemianDistanceToHeadingJunction;
                }
                else
                {
                    return CurrentLane.MyRoadSegment.Width - RemianDistanceToHeadingJunction;
                }
            }
        }

        /// <summary>
        /// the distance that the vechile cross its heading junction.
        /// </summary>
        public double ExceededDistanceInMeter
        {
            get
            {
               
                Point st = StartJunction.CenterLocation;
                Point head = EndJunction.CenterLocation;
                Point curent = InstanceLocation;

                double dis1 = Computations.Distance(st, head);
                double dis2= Computations.Distance(st, curent);


                return dis2 - dis1;
            }
        }

        public double SpeedDisPercentage = 0.9;
        public bool ChangeLaneFlage = false; 
        double LineUpInJunctionDistance = 10; // when the vehile allowed to line up infront of junction.
        double SlowDownDistance = 5; // the distance the vechile start decrease the speed as the density infront of jucntion is greater.
        double AllowToChangeLaneDistance = 25; // when the distance to the junction is x, then its not allowed to change the lane.


        /// <summary>
        /// move the vechile.
        /// </summary>
        public void StartMove()
        {
            VehicleUi infrontVehicle = CurrentLane.LaneVehicleAndQueue.GetMyFrontVehicle(this); // get in the front.
            showInfo(PublicParamerters.DisplayInfoFlag); // show flage. // info which should be disply.
            double headingDistance = RemianDistanceToHeadingJunction;
            // to north:
            if (VehicleDirection == Direction.N)
            {
                if (headingDistance > 0)
                {
                    double marTop = this.Margin.Top;
                    marTop -= 1;
                    Margin = new Thickness(Margin.Left, marTop, 0, 0);
                    // start decrease the speed according to the heading distance toward the jucntion.
                    if (headingDistance <= SlowDownDistance)
                    {
                        double instanceSpeed = Computations.GetTimeIntervalInSecond(headingDistance); // slow the speed.
                        InstantaneousSpeed = instanceSpeed;
                    }

                    // if has vechile in front: the behind vehicle should change the speed to or change the lane if possible.
                    if (infrontVehicle != null)
                    {
                        //: if still allowed to change the lane then go ahead. if not then the behind vechile should lower its speed.
                        if (headingDistance > AllowToChangeLaneDistance)
                        {
                            ChangeLaneRandomly();
                        }
                        else
                        {
                            // not allowd to change the lane.
                           // double fronVspeed = 5;
                            InstantaneousSpeed = infrontVehicle.InstantaneousSpeed * SpeedDisPercentage;
                        }
                    }
                    
                    // change the lane randomnlly when v is in the middlel of the segment.
                    if (marTop < CurrentLane.MyRoadSegment.Midpoint)
                    {
                        double half = CurrentLane.MyRoadSegment.Height / 2;
                        if (headingDistance > half)
                        {
                            if (!ChangeLaneFlage)
                            {
                                ChangeLaneRandomly();
                                ChangeLaneFlage = true;
                            }
                        }
                    }

                    if(headingDistance< LineUpInJunctionDistance)
                    {
                        CurrentLane.LaneVehicleAndQueue.Enqueue(this); // add to the queue.
                    }
                }
            }
            else if (VehicleDirection == Direction.S)
            {
                if (headingDistance > 0)
                {
                    double marTop = this.Margin.Top;
                    marTop += 1;
                    Margin = new Thickness(Margin.Left, marTop, 0, 0);

                    if (headingDistance <= SlowDownDistance)
                    {
                        double instanceSpeed = Computations.GetTimeIntervalInSecond(headingDistance); // slow the speed.
                        InstantaneousSpeed = instanceSpeed;
                       
                    }

                    if (infrontVehicle != null)
                    {
                        //: if still allowed to change the lane then go ahead. if not then the behind vechile should lower its speed.
                        if (headingDistance > AllowToChangeLaneDistance)
                        {
                            ChangeLaneRandomly();
                        }
                        else
                        {
                            InstantaneousSpeed = infrontVehicle.InstantaneousSpeed * SpeedDisPercentage;
                        }
                    }
                    if (marTop > CurrentLane.MyRoadSegment.Midpoint)
                    {
                        double half = CurrentLane.MyRoadSegment.Height / 2;
                        if (headingDistance > half)
                        {
                            if (!ChangeLaneFlage)
                            {
                                ChangeLaneRandomly();
                                ChangeLaneFlage = true;
                            }
                        }
                    }

                    if (headingDistance < LineUpInJunctionDistance)
                    {
                        CurrentLane.LaneVehicleAndQueue.Enqueue(this); // add to the queue.
                    }
                }
            }
            else if (VehicleDirection == Direction.E)
            {
                if (headingDistance > 0)
                {
                    double marLeft = Margin.Left;
                    double marTop = Margin.Top;
                    marLeft += 1;
                    Margin = new Thickness(marLeft, marTop, 0, 0);


                    if (headingDistance <= SlowDownDistance)
                    {
                        double instanceSpeed = Computations.GetTimeIntervalInSecond(headingDistance); // slow the speed.
                        InstantaneousSpeed = instanceSpeed;
                    }

                    if (infrontVehicle != null)
                    {
                        //: if still allowed to change the lane then go ahead. if not then the behind vechile should lower its speed.
                        if (headingDistance > AllowToChangeLaneDistance)
                        {
                            ChangeLaneRandomly();
                        }
                        else
                        {
                            InstantaneousSpeed = infrontVehicle.InstantaneousSpeed * SpeedDisPercentage;
                        }
                    }
                    if (marLeft > CurrentLane.MyRoadSegment.Midpoint)
                    {
                        double half = CurrentLane.MyRoadSegment.Width / 2;
                        if (headingDistance > half)
                        {
                            if (!ChangeLaneFlage)
                            {
                                ChangeLaneRandomly();
                                ChangeLaneFlage = true;
                            }
                        }
                    }

                    if (headingDistance < LineUpInJunctionDistance)
                    {
                        CurrentLane.LaneVehicleAndQueue.Enqueue(this); // add to the queue.
                    }
                }
            }
            else if (VehicleDirection == Direction.W)
            {
                if (headingDistance > 0)
                {

                    double marLeft = Margin.Left;
                    double marTop = Margin.Top;
                    marLeft -= 1;
                    Margin = new Thickness(marLeft, marTop, 0, 0);



                    if (headingDistance <= SlowDownDistance)
                    {
                        double instanceSpeed = Computations.GetTimeIntervalInSecond(headingDistance); // slow the speed.
                        InstantaneousSpeed = instanceSpeed;
                    }

                    if (infrontVehicle != null)
                    {
                        //: if still allowed to change the lane then go ahead. if not then the behind vechile should lower its speed.
                        if (headingDistance > AllowToChangeLaneDistance)
                        {
                            ChangeLaneRandomly();
                        }
                        else
                        {
                            InstantaneousSpeed = infrontVehicle.InstantaneousSpeed * SpeedDisPercentage;
                        }
                    }
                    if (marLeft < CurrentLane.MyRoadSegment.Midpoint)
                    {
                        double half = CurrentLane.MyRoadSegment.Width / 2;
                        if (headingDistance > half)
                        {
                            if (!ChangeLaneFlage)
                            {
                                ChangeLaneRandomly();
                                ChangeLaneFlage = true;
                            }
                        }
                    }

                    if (headingDistance < LineUpInJunctionDistance)
                    {
                        CurrentLane.LaneVehicleAndQueue.Enqueue(this); // add to the queue.
                    }
                }
            }
        }

        private void MobilityTimer_Tick(object sender, EventArgs e)
        {
            Action open1 = () => StartMove();
            Dispatcher.Invoke(open1,DispatcherPriority.Send);
        }

        /// <summary>
        /// we have three lanes per segments. the index should be 1,2,3. ONLY.
        /// </summary>
        /// <param name="lindex"></param>
        public void ChangeLaneRandomly()
        {
            if (PublicParamerters.NumberOfLanes > 2)
            {
                RoadSegment rs = CurrentLane.MyRoadSegment;
                LaneUi prevLane = CurrentLane;
                LaneUi newlane = null;

                if (prevLane.LaneDirection == Direction.N)
                {
                    newlane = rs.Lanes[LaneIndex.RandomLaneIndex.North];

                    prevLane.LaneVehicleAndQueue.RemoveFromLane(this);
                    this.CurrentLane = newlane;
                    this.Margin = new Thickness(newlane.MyCenterLeft, this.Margin.Top, 0, 0);
                    this.SetVehicleDirection(Direction.N);



                }
                else if (prevLane.LaneDirection == Direction.S)
                {
                    newlane = rs.Lanes[LaneIndex.RandomLaneIndex.South];

                    prevLane.LaneVehicleAndQueue.RemoveFromLane(this);
                    this.CurrentLane = newlane;
                    this.Margin = new Thickness(newlane.MyCenterLeft, this.Margin.Top, 0, 0);
                    this.SetVehicleDirection(Direction.S);

                }

                else if (prevLane.LaneDirection == Direction.E)
                {
                    newlane = rs.Lanes[LaneIndex.RandomLaneIndex.East];

                    prevLane.LaneVehicleAndQueue.RemoveFromLane(this);
                    this.CurrentLane = newlane;
                    this.Margin = new Thickness(this.Margin.Left, newlane.MyCenterTop, 0, 0);
                    this.SetVehicleDirection(Direction.E);
                }

                else if (prevLane.LaneDirection == Direction.W)
                {
                    newlane = rs.Lanes[LaneIndex.RandomLaneIndex.West];

                    prevLane.LaneVehicleAndQueue.RemoveFromLane(this);
                    this.CurrentLane = newlane;
                    this.Margin = new Thickness(this.Margin.Left, newlane.MyCenterTop, 0, 0);
                    this.SetVehicleDirection(Direction.W);

                }

                // change my speed to random.
                double instanceSpeed = Computations.GetTimeIntervalInSecond(Computations.RandomSpeedkmh(CurrentLane.MyRoadSegment)); // slow the speed.
                InstantaneousSpeed = instanceSpeed;

                // display:
                prevLane._MainWindow.Dispatcher.Invoke(new Action(() => prevLane.lbl_info.Text = prevLane.LaneVehicleAndQueue.CountInLane.ToString()), DispatcherPriority.Send);
            }


           
            


        }

        

        public void SetVehicleDirection(Direction dir)
        {
            if (dir == Direction.N)
            {
                txt_vehicle.Text = "▲";
            }
            else if (dir == Direction.S)
            {
                txt_vehicle.Text = "▼";
            }
            else if (dir == Direction.E)
            {
                txt_vehicle.Text = "►";
            }
            else if (dir == Direction.W)
            {
                txt_vehicle.Text = "◄";
            }
        }

        

        private void Txt_vehicle_MouseEnter(object sender, MouseEventArgs e)
        {
            string line = "\r\n";
            ToolTip = new Label
            {
                Content =
                "VID:" + VID + line +
                "RID:" + CurrentLane.MyRoadSegment.RID + line +
                "Instance Speed (KMH):" + GetSpeedInKMH + line +
                "Instance Speed  (TimerInterval in Seconds):" + InstantaneousSpeed + line +
                "Direction:" + CurrentLane.LaneDirection
            };
        }

        #endregion

    }
}
