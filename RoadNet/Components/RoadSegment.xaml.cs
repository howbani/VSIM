using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using VANET_SIM.Properties;

namespace VANET_SIM.RoadNet.Components
{
    /// <summary>
    /// Interaction logic for RoadSegment.xaml
    /// </summary>
   

    public partial class RoadSegment : UserControl
    {
        private double Yellow_Line_Height_6Lanes = 0.2;
        private double Yellow_Line_Height_4Lanes = 0.2;
        private double Yellow_Line_Height_2Lanes = 0.2;

        public DispatcherTimer RandomSwitchDirectionTimer = new DispatcherTimer(); 
        public DispatcherTimer ArrivalTimer = new DispatcherTimer(); 
        private Canvas myCanvas;
        public YellowLine MyYellowLine { get; set; }
        public List<LaneUi> Lanes = new List<LaneUi>();
        public  MainWindow _MainWindow;
        public RoadOrientation Roadorientation;
        public List<Junction> MyJunctions = new List<Junction>(2);

        /// <summary>
        /// number of vehiles in the segment.
        /// </summary>
        public int VehiclesCount
        {
            get
            {
                int re = 0;
                foreach (LaneUi lan in Lanes)
                {
                    re += lan.NumberofRisingVechilesInLane;
                }
                return re;
            }
        }

       

        public RoadSegment(MainWindow MainWin, RoadOrientation _roadOrientation)
        {
            InitializeComponent();
            _MainWindow = MainWin;
            myCanvas = _MainWindow.canvas_vanet;
            Roadorientation = _roadOrientation;
            _MainWindow.MyRoadSegments.Add(this);
            RID = _MainWindow.MyRoadSegments.Count - 1;

            // max speed:
            MaxAllowedSpeed = Computations.MaxAllowedSpeedInSegmentInKm;

            if (_roadOrientation == RoadOrientation.Vertical)
            {
                int lc = Convert.ToInt16(PublicParamerters.NumberOfLanes);
                SetVerticalLayout(lc);
            }
            else
            {
                int lc = Convert.ToInt16(PublicParamerters.NumberOfLanes);
                setHorizontalLayout(lc);
            }

            // set the switch timer.
            RandomSwitchDirectionTimer.Interval = PublicParamerters.RoadSegmentSwitchDirectionTimerInterval;

            RandomSwitchDirectionTimer.Start();
            RandomSwitchDirectionTimer.Tick += RandomSwitchDirectionTimer_Tick;
        }

      
        /// <summary>
        /// switch the direction of the lanes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomSwitchDirectionTimer_Tick(object sender, EventArgs e)
        {
            if (PublicParamerters.NumberOfLanes <= 4)
            {
                foreach (LaneUi CurrentLane in Lanes) 
                {
                    Direction randomDir = CurrentLane.RandomSwitchToDirection;
                    Junction HeadJunc = CurrentLane.GetMyHeadingJunction;
                    if (HeadJunc != null)
                    {
                        if (CurrentLane.CurrentSwitchToDirection != randomDir)
                        {
                            if (randomDir == Direction.S)
                            {
                                if (HeadJunc.ToSouthRoadSegment != null)
                                {
                                    CurrentLane.CurrentSwitchToDirection = randomDir;
                                   HeadJunc.ReLoadLanes();
                                }
                            }
                            else if (randomDir == Direction.N)
                            {
                                if (HeadJunc.ToNorthRoadSegment != null)
                                {
                                    CurrentLane.CurrentSwitchToDirection = randomDir;
                                   HeadJunc.ReLoadLanes();
                                }
                            }
                            else if (randomDir == Direction.W)
                            {
                                if (HeadJunc.ToWestRoadSegment != null)
                                {
                                    CurrentLane.CurrentSwitchToDirection = randomDir;
                                   HeadJunc.ReLoadLanes();
                                }
                            }
                            else if (randomDir == Direction.E)
                            {
                                if (HeadJunc.ToEastRoadSegment != null)
                                {
                                    CurrentLane.CurrentSwitchToDirection = randomDir;
                                   HeadJunc.ReLoadLanes();
                                }
                            }
                            RandomSwitchDirectionTimer.Interval = PublicParamerters.RoadSegmentSwitchDirectionTimerInterval;
                        }
                    }
                }
            }
            else
            {
                RandomSwitchDirectionTimer.Interval = TimeSpan.FromSeconds(0);
                RandomSwitchDirectionTimer.Stop();
            }
        }

        public void SetRoadColor()
        {
            border_mian.Background = Brushes.Red;
        }
        /// <summary>
        /// set segment ID.
        /// </summary>
        public int RID
        {
            get
            {
                return Convert.ToInt32(txt_segmen_id.Text);
            }
            set
            {
                txt_segmen_id.Text = value.ToString();
            }
        }
        /// <summary>
        /// the maximum allowed speed in this segment.
        /// </summary>
        public double MaxAllowedSpeed
        {
            get; set;
        }

        

        /// <summary>
        /// vehicles per segment
        /// </summary>
        public double Density
        {
            get
            {
                double den = SegmentLength * VehicleArrivalRate;
                return den;
            }
        }



        #region DISTRIBUTION
        /// <summary>
        /// The probability that k vehicles residing within 〖 r〗_ij is given by the mass function Eq. (9) where |r_ij | denotes the length of segment and ρ_ij=λ_ij/E_ij^S is the vehicle density of road segment 〖 r〗_ij (vehicle per meter) [22].
        /// </summary>
        /// <returns></returns>
        public double ProKVehiclesInThisSegment(ulong k)
        {
            if (k >= 0)
            {
                double past = Math.Pow(Density, k);
                double mak = Convert.ToDouble(MyMath.Factorial(k));
                double exp = Math.Exp(-Density);
                double re = (past / mak) * exp;
                return re;

            }
            else
                return 0;
        }


        /// <summary>
        /// Unit: meter
        /// </summary>
        public double SegmentLength
        {
            get
            {
                if (Roadorientation == RoadOrientation.Horizontal) { return Width; }
                else return Height;
            }
        }

        /// <summary>
        /// Unit: vehicles
        /// </summary>
        public double SegmentCapacity
        {
            get
            {
                double countLanes = Lanes.Count;
                double half = countLanes / 2; // here we assume 2 for north/west and 2 for south/east
                return (SegmentLength / PublicParamerters.VehicleHight) * half;
            }
        }
        /// <summary>
        /// Unit: vehicle per one meter
        /// </summary>
        public double MaximumDensity
        {
            get
            {
                return SegmentCapacity / SegmentLength;
            }
        }


        /// <summary>
        /// The time between two successive arrivals in second. // this can be consideded as the timer interval.
        /// </summary>
        public double VehicleInterArrivalMean { get; set; } // one vehile in x seconds for each lane. Random variable. This is a raondom varibal.

        /// <summary>
        /// arrivale rate in possion distrbition. it equal to 1/VehicleInterArrival.
        /// that is to say each second there will be (x=VehicleArrivalRate) of vechiles arrived to this segment.
        /// </summary>
        public double VehicleArrivalRate
        {
            get
            {
                double arrivaleRate = 1 / VehicleInterArrivalMean;
                return arrivaleRate;
            }
        }
        /// <summary>
        /// density the number of vehicle in one meter.
        /// </summary>
        public  double MaxDensity
        {
            get
            {

                return 1 / PublicParamerters.VehicleHight;
            }
        }

        /// <summary>
        ///  The probablity that one vehicle arrives at one second, given that the intervechile between two consuctive vechiles is x=VehicleInterArrival.
        /// </summary>
        public  double Possion
        {
            get
            {
                double exp = VehicleArrivalRate * Math.Exp(-VehicleArrivalRate);
                return exp;
            }
        }

        /// <summary>
        /// determine to generate a vechile or not according to possion distribution.
        /// </summary>
        public  bool GenerateVehilesPossionDistrubtion
        {
            get
            {
                double ran = RandomeNumberGenerator.GetUniform(0, 100);// random number 0- 100
                double poss = Possion * 100; // get possoin probablit that 1 vechile will be generated
                if (ran < poss) return true;
                else return false;
            }
        }

        public void SetAsEntry()
        {
            ArrivalTimer.Tick += ArrivalTimer_Tick;
            ArrivalTimer.Interval = TimeSpan.FromSeconds(VehicleInterArrivalMean);
            ArrivalTimer.Start();
        }

        public void stopGeneratingVehicles()
        {
            Console.Write("Segment:" + RID + "Stoped Timer.");
            ArrivalTimer.Stop();
            
        }
        public void StartGeneratingVehicles()
        {
            ArrivalTimer.Start();
        }

        public void DeployVehicle()
        {
            VehicleUi vehicle = new VehicleUi();
            vehicle.VID = _MainWindow.MyVehicles.Count;
            LaneUi randomLane = Lanes[LaneIndex.RandomLaneIndex.ZeroToMax];
            vehicle.CurrentLane = randomLane;
            _MainWindow.canvas_vanet.Children.Add(vehicle);
            _MainWindow.MyVehicles.Add(vehicle);
            PublicStatistics.LiveStatstics.lbl_number_of_vehicles.Content = _MainWindow.MyVehicles.Count;
            if (randomLane.LaneDirection == Direction.N)
            {
                double left = randomLane.MyCenterLeft;
                double top = randomLane.MyCenterTop + this.Height - vehicle.Height;
                vehicle.SetVehicleDirection(randomLane.LaneDirection);
                vehicle.Margin = new Thickness(left, top, 0, 0);
            }
            else if (randomLane.LaneDirection == Direction.S)
            {
                double left = randomLane.MyCenterLeft;
                double top = randomLane.MyCenterTop;
                vehicle.SetVehicleDirection(randomLane.LaneDirection);
                vehicle.Margin = new Thickness(left, top, 0, 0);
            }
            else if (randomLane.LaneDirection == Direction.E)
            {
                double left = randomLane.MyCenterLeft;
                double top = randomLane.MyCenterTop;
                vehicle.SetVehicleDirection(randomLane.LaneDirection);
                vehicle.Margin = new Thickness(left, top, 0, 0);
            }
            else if (randomLane.LaneDirection == Direction.W)
            {
                double left = randomLane.MyCenterLeft + this.Width - vehicle.Width;
                double top = randomLane.MyCenterTop;
                vehicle.SetVehicleDirection(randomLane.LaneDirection);
                vehicle.Margin = new Thickness(left, top, 0, 0);
            }
        }

        private void ArrivalTimer_Tick(object sender, EventArgs e)
        {
            if (Settings.Default.IsSetMaxVehicles)
            {
                if (_MainWindow.MyVehicles.Count < Settings.Default.MaxNumberOfVehicles)
                {
                    if (GenerateVehilesPossionDistrubtion)
                    {
                        DeployVehicle();
                    }
                }
                else
                {
                    stopGeneratingVehicles();
                }
            }
            else
            {
                if (GenerateVehilesPossionDistrubtion)
                {
                    DeployVehicle();
                }
            }
        }

        #endregion

        #region Layout
        public Point Location
        {
            get { return new Point(Margin.Left, Margin.Top); }
        }
         
        /// <summary>
        /// the middel of the segment.
        /// </summary>
        public double Midpoint 
        {
            get
            {
                if (Roadorientation == RoadOrientation.Vertical)
                {
                    return Margin.Top + (Height / 2);
                }
                else
                {
                    return Margin.Left + (Width / 2);
                }
            }
        }

        public Point TopLeftCorner
        {
            get
            {
                return Location;
            }
        }
        public Point TopRightCorner
        {
            get
            {
                return new Point(Margin.Left + Width, Margin.Top);
            }
        }

        public Point BottomLeftCorner
        {
            get
            {
                return new Point(Margin.Left, Margin.Top + Height);
            }
        }

        public Point BottomRightCorner
        {
            get
            {
                return new Point(Margin.Left + Width, Margin.Top + Height);
            }
        }

        private void RS_MouseEnter(object sender, MouseEventArgs e)
        {
            string line = "\r\n";
            string info =
                "Length:" + SegmentLength + line +
                "MaxSpeed=" + MaxAllowedSpeed + line +
                "Density=" + Density + line +
                "Inter-Arrival=" + VehicleInterArrivalMean + line +
                "Arrival=" + VehicleArrivalRate + line +
                 "RandomSwitchDirectionTimer:" + RandomSwitchDirectionTimer.Interval.TotalSeconds + "s";
            ToolTip = new Label() { Content = info };
        }

        private void RS_MouseLeave(object sender, MouseEventArgs e)
        {
            /*
            foreach (Junction jun in MyJunctions)
            {
                jun._junction.Background = Brushes.Gray;
            }*/
        }

        public void setHorizontalLayout(int lanesCount)
        {
            if (lanesCount == 6)
            {
                Height = 22;
                Width = 100;
                stack_dispaly_in.Orientation = Orientation.Horizontal;
                border_mian.BorderThickness = new Thickness(0, 0.5, 0, 0.5);
                stack_lanes.Orientation = Orientation.Vertical;

                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                stack_lanes.Children.Add(lane1);
                lane1.LaneIndex = 0;
                lane1.Lane.Height = PublicParamerters.LaneWidth;
                lane1.Lane.Width = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane1.LaneDirection = Direction.W;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add(Direction.N);
                lane1.CurrentSwitchToDirection = Direction.N;

                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Height = PublicParamerters.LaneWidth;
                lane2.Lane.Width = Double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane2.LaneDirection = Direction.W;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections.Add(Direction.W);
                lane2.CurrentSwitchToDirection = Direction.W;


                LaneUi lane3 = new LaneUi(LanesCaptions.Lane3);
                lane3.LaneIndex = 2;
                stack_lanes.Children.Add(lane3);
                lane3.Lane.Height = PublicParamerters.LaneWidth;
                lane3.Lane.Width = Double.NaN; // auto.
                lane3.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane3.LaneDirection = Direction.W;
                lane3.MyRoadSegment = this;
                lane3.SwitchToDirections.Add(Direction.S);
                lane3.CurrentSwitchToDirection = Direction.S;

                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Height = Yellow_Line_Height_6Lanes;
                border_yellow_line.border_yellow_line.Width = Double.NaN;
                MyYellowLine = border_yellow_line;

                LaneUi lane4 = new LaneUi(LanesCaptions.Lane4);
                lane4.LaneIndex = 3;
                stack_lanes.Children.Add(lane4);
                lane4.Lane.Height = PublicParamerters.LaneWidth;
                lane4.Lane.Width = double.NaN; // auto.
                lane4.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane4.LaneDirection = Direction.E;
                lane4.MyRoadSegment = this;
                lane4.SwitchToDirections.Add(Direction.N);
                lane4.CurrentSwitchToDirection = Direction.N;

                LaneUi lane5 = new LaneUi(LanesCaptions.Lane5);
                lane5.LaneIndex = 4;
                stack_lanes.Children.Add(lane5);
                lane5.Lane.Height = PublicParamerters.LaneWidth;
                lane5.Lane.Width = double.NaN; // auto.
                lane5.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane5.LaneDirection = Direction.E;
                lane5.MyRoadSegment = this;
                lane5.SwitchToDirections.Add(Direction.E);
                lane5.CurrentSwitchToDirection = Direction.E;

                LaneUi lane6 = new LaneUi(LanesCaptions.Lane6);
                lane6.LaneIndex = 5;
                stack_lanes.Children.Add(lane6);
                lane6.Lane.Height = PublicParamerters.LaneWidth;
                lane6.Lane.Width = double.NaN; // auto.
                lane6.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane6.LaneDirection = Direction.E;
                lane6.MyRoadSegment = this;
                lane6.SwitchToDirections.Add(Direction.S);
                lane6.CurrentSwitchToDirection = Direction.S;

                Lanes.Add(lane1);
                Lanes.Add(lane2);
                Lanes.Add(lane3);
                Lanes.Add(lane4);
                Lanes.Add(lane5);
                Lanes.Add(lane6);
            }
            else if (lanesCount == 4)
            {
                Height = 14.6;
                Width = 100;
                stack_dispaly_in.Orientation = Orientation.Horizontal;
                border_mian.BorderThickness = new Thickness(0, 0.5, 0, 0.5);
                stack_lanes.Orientation = Orientation.Vertical;

                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                stack_lanes.Children.Add(lane1);
                lane1.LaneIndex = 0;
                lane1.Lane.Height = PublicParamerters.LaneWidth;
                lane1.Lane.Width = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane1.LaneDirection = Direction.W;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add(Direction.N);
                lane1.SwitchToDirections.Add(Direction.W);
                lane1.CurrentSwitchToDirection = lane1.RandomSwitchToDirection;

                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Height = PublicParamerters.LaneWidth;
                lane2.Lane.Width = Double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane2.LaneDirection = Direction.W;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections.Add(Direction.W);
                lane2.SwitchToDirections.Add(Direction.S);
                lane2.CurrentSwitchToDirection = lane2.RandomSwitchToDirection;

                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Height = Yellow_Line_Height_4Lanes;
                border_yellow_line.border_yellow_line.Width = Double.NaN;
                MyYellowLine = border_yellow_line;

                LaneUi lane3 = new LaneUi(LanesCaptions.Lane3);
                lane3.LaneIndex = 2;
                stack_lanes.Children.Add(lane3);
                lane3.Lane.Height = PublicParamerters.LaneWidth;
                lane3.Lane.Width = double.NaN; // auto.
                lane3.Lane.BorderThickness = new Thickness(0, 0, 0, 0.3);
                lane3.LaneDirection = Direction.E;
                lane3.MyRoadSegment = this;
                lane3.SwitchToDirections.Add(Direction.N);
                lane3.SwitchToDirections.Add(Direction.E);
                lane3.CurrentSwitchToDirection = lane3.RandomSwitchToDirection;

                LaneUi lane4 = new LaneUi(LanesCaptions.Lane4);
                lane4.LaneIndex = 3;
                stack_lanes.Children.Add(lane4);
                lane4.Lane.Height = PublicParamerters.LaneWidth;
                lane4.Lane.Width = double.NaN; // auto.
                lane4.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane4.LaneDirection = Direction.E;
                lane4.MyRoadSegment = this;
                lane4.SwitchToDirections.Add(Direction.S);
                lane4.SwitchToDirections.Add(Direction.E);
                lane4.CurrentSwitchToDirection = lane4.RandomSwitchToDirection;


                Lanes.Add(lane1);
                Lanes.Add(lane2);
                Lanes.Add(lane3);
                Lanes.Add(lane4);
            }
            else if (lanesCount == 2)
            {
                Height = 8.5;
                Width = 100;
                stack_dispaly_in.Orientation = Orientation.Horizontal;
                border_mian.BorderThickness = new Thickness(0, 0.5, 0, 0.5);
                stack_lanes.Orientation = Orientation.Vertical;

                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                stack_lanes.Children.Add(lane1);
                lane1.LaneIndex = 0;
                lane1.Lane.Height = PublicParamerters.LaneWidth;
                lane1.Lane.Width = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane1.LaneDirection = Direction.W;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add(Direction.N);
                lane1.SwitchToDirections.Add(Direction.W);
                lane1.SwitchToDirections.Add(Direction.S);
                lane1.CurrentSwitchToDirection = lane1.RandomSwitchToDirection;

                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Height = Yellow_Line_Height_2Lanes;
                border_yellow_line.border_yellow_line.Width = Double.NaN;
                MyYellowLine = border_yellow_line;

                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Height = PublicParamerters.LaneWidth;
                lane2.Lane.Width = double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane2.LaneDirection = Direction.E;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections.Add(Direction.N);
                lane2.SwitchToDirections.Add(Direction.E);
                lane2.SwitchToDirections.Add(Direction.S);
                lane2.CurrentSwitchToDirection = lane2.RandomSwitchToDirection;

                Lanes.Add(lane1);
                Lanes.Add(lane2);
            }
        }

        public void SetVerticalLayout(int lanesCount)
        {
            if (lanesCount == 6)
            {
                Height = 100;
                Width = 22;
                stack_dispaly_in.Orientation = Orientation.Vertical;
                border_mian.BorderThickness = new Thickness(0.5, 0, 0.5, 0);
                stack_lanes.Orientation = Orientation.Horizontal;


                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                lane1.LaneIndex = 0;
                stack_lanes.Children.Add(lane1);
                lane1.Lane.Width = PublicParamerters.LaneWidth;
                lane1.Lane.Height = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane1.LaneDirection = Direction.S;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add(Direction.W); //
                lane1.CurrentSwitchToDirection = Direction.W;


                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Width = PublicParamerters.LaneWidth;
                lane2.Lane.Height = Double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane2.LaneDirection = Direction.S;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections.Add( Direction.S); // direct.
                lane2.CurrentSwitchToDirection = Direction.S;

                LaneUi lane3 = new LaneUi(LanesCaptions.Lane3);
                lane3.LaneIndex = 2;
                stack_lanes.Children.Add(lane3);
                lane3.Lane.Width = PublicParamerters.LaneWidth;
                lane3.Lane.Height = Double.NaN; // auto.
                lane3.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane3.LaneDirection = Direction.S;
                lane3.MyRoadSegment = this;
                lane3.SwitchToDirections.Add(Direction.E);
                lane3.CurrentSwitchToDirection = Direction.E;


                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Width = Yellow_Line_Height_6Lanes;
                border_yellow_line.border_yellow_line.Height = Double.NaN;
                MyYellowLine = border_yellow_line;

                LaneUi lane4 = new LaneUi(LanesCaptions.Lane4);
                lane4.LaneIndex = 3;
                stack_lanes.Children.Add(lane4);
                lane4.Lane.Width = PublicParamerters.LaneWidth;
                lane4.Lane.Height = Double.NaN; // auto.
                lane4.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane4.LaneDirection = Direction.N;
                lane4.MyRoadSegment = this;
                lane4.SwitchToDirections.Add(Direction.W);
                lane4.CurrentSwitchToDirection = Direction.W;

                LaneUi lane5 = new LaneUi(LanesCaptions.Lane5);
                lane5.LaneIndex = 4;
                stack_lanes.Children.Add(lane5);
                lane5.Lane.Width = PublicParamerters.LaneWidth;
                lane5.Lane.Height = Double.NaN; // auto.
                lane4.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane5.LaneDirection = Direction.N;
                lane5.MyRoadSegment = this;
                lane5.SwitchToDirections.Add(Direction.N);// direct.
                lane5.CurrentSwitchToDirection = Direction.N;


                LaneUi lane6 = new LaneUi(LanesCaptions.Lane6);
                lane6.LaneIndex = 5;
                stack_lanes.Children.Add(lane6);
                lane6.Lane.Width = PublicParamerters.LaneWidth;
                lane6.Lane.Height = Double.NaN; // auto.
                lane6.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane6.LaneDirection = Direction.N;
                lane6.MyRoadSegment = this;
                lane6.SwitchToDirections.Add(Direction.E);
                lane6.CurrentSwitchToDirection = Direction.E;

                Lanes.Add(lane1);
                Lanes.Add(lane2);
                Lanes.Add(lane3);
                Lanes.Add(lane4);
                Lanes.Add(lane5);
                Lanes.Add(lane6);
            }
            else if (lanesCount == 4)
            {
                Height = 100;
                Width = 14.6;
                stack_dispaly_in.Orientation = Orientation.Vertical;
                border_mian.BorderThickness = new Thickness(0.5, 0, 0.5, 0);
                stack_lanes.Orientation = Orientation.Horizontal;


                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                lane1.LaneIndex = 0;
                stack_lanes.Children.Add(lane1);
                lane1.Lane.Width = PublicParamerters.LaneWidth;
                lane1.Lane.Height = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane1.LaneDirection = Direction.S;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add(Direction.W); // this can go to south too. may be can be set randomly.
                lane1.SwitchToDirections.Add(Direction.S); // two.
                lane1.CurrentSwitchToDirection = lane1.RandomSwitchToDirection;

                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Width = PublicParamerters.LaneWidth;
                lane2.Lane.Height = Double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane2.LaneDirection = Direction.S;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections.Add(Direction.S); // this can be go to east
                lane2.SwitchToDirections.Add(Direction.E);
                lane2.CurrentSwitchToDirection = lane2.RandomSwitchToDirection;

                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Width = Yellow_Line_Height_4Lanes;
                border_yellow_line.border_yellow_line.Height = Double.NaN;

                MyYellowLine = border_yellow_line;

                LaneUi lane3 = new LaneUi(LanesCaptions.Lane3);
                lane3.LaneIndex = 2;
                stack_lanes.Children.Add(lane3);
                lane3.Lane.Width = PublicParamerters.LaneWidth;
                lane3.Lane.Height = Double.NaN; // auto.
                lane3.Lane.BorderThickness = new Thickness(0, 0, 0.3, 0);
                lane3.LaneDirection = Direction.N;
                lane3.MyRoadSegment = this;
                lane3.SwitchToDirections .Add( Direction.W); // can go to north too.
                lane3.SwitchToDirections.Add(Direction.N);
                lane3.CurrentSwitchToDirection = lane3.RandomSwitchToDirection;

                LaneUi lane4 = new LaneUi(LanesCaptions.Lane4);
                lane4.LaneIndex = 3;
                stack_lanes.Children.Add(lane4);
                lane4.Lane.Width = PublicParamerters.LaneWidth;
                lane4.Lane.Height = Double.NaN; // auto.
                lane4.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane4.LaneDirection = Direction.N;
                lane4.MyRoadSegment = this;
                lane4.SwitchToDirections.Add(Direction.N); // can go to east too.
                lane4.SwitchToDirections.Add(Direction.E);
                lane4.CurrentSwitchToDirection = lane4.RandomSwitchToDirection;

                Lanes.Add(lane1);
                Lanes.Add(lane2);
                Lanes.Add(lane3);
                Lanes.Add(lane4);
            }
            else if (lanesCount == 2)
            {
                Height = 100;
                Width = 9.5;
                stack_dispaly_in.Orientation = Orientation.Vertical;
                border_mian.BorderThickness = new Thickness(0.5, 0, 0.5, 0);
                stack_lanes.Orientation = Orientation.Horizontal;


                LaneUi lane1 = new LaneUi(LanesCaptions.Lane1);
                lane1.LaneIndex = 0;
                stack_lanes.Children.Add(lane1);
                lane1.Lane.Width = PublicParamerters.LaneWidth;
                lane1.Lane.Height = Double.NaN; // auto.
                lane1.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane1.LaneDirection = Direction.S;
                lane1.MyRoadSegment = this;
                lane1.SwitchToDirections.Add( Direction.S); //
                lane1.SwitchToDirections.Add(Direction.E); //
                lane1.SwitchToDirections.Add(Direction.W); //
                lane1.CurrentSwitchToDirection = lane1.RandomSwitchToDirection;

                YellowLine border_yellow_line = new YellowLine();
                stack_lanes.Children.Add(border_yellow_line);
                border_yellow_line.border_yellow_line.Height = Yellow_Line_Height_2Lanes;
                border_yellow_line.border_yellow_line.Height = Double.NaN;
                MyYellowLine = border_yellow_line;


                LaneUi lane2 = new LaneUi(LanesCaptions.Lane2);
                lane2.LaneIndex = 1;
                stack_lanes.Children.Add(lane2);
                lane2.Lane.Width = PublicParamerters.LaneWidth;
                lane2.Lane.Height = Double.NaN; // auto.
                lane2.Lane.BorderThickness = new Thickness(0, 0, 0, 0);
                lane2.LaneDirection = Direction.N;
                lane2.MyRoadSegment = this;
                lane2.SwitchToDirections .Add( Direction.N); // direct.
                lane2.SwitchToDirections.Add(Direction.W); // direct.
                lane2.SwitchToDirections.Add(Direction.E); // direct.
                lane2.CurrentSwitchToDirection = lane2.RandomSwitchToDirection;

                Lanes.Add(lane1);
                Lanes.Add(lane2);
            }
        }



        
        

        #endregion


        #region Draage Object

        // The part of the rectangle the mouse is over.
        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        // True if a drag is in progress.
        private bool DragInProgress = false;

        // The drag's last point.
        private Point LastPoint;

        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;

        // Stop dragging.
        private void RS_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
            Mouse.Capture(null);
        }

        // Start dragging.
        private void RS_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Settings.Default.IsIntialized)
            {
                Mouse.Capture(this);
                Point po = new Point(Mouse.GetPosition(myCanvas).X, Mouse.GetPosition(myCanvas).Y);
                MouseHitType = SetHitType(po);
                SetMouseCursor();
                if (MouseHitType == HitType.None) return;
                LastPoint = Mouse.GetPosition(myCanvas);
                DragInProgress = true;
            }

           
        }


        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void RS_MouseMove(object sender, MouseEventArgs e)
        {
           

            if (!DragInProgress)
            {
                Point po = Mouse.GetPosition(myCanvas);
                MouseHitType = SetHitType(po);
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(myCanvas);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = this.Margin.Left;
                double new_y = this.Margin.Top;
                double new_width = this.Width;
                double new_height = this.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.
                    Position = new Point(new_x, new_y);
                    this.Width = new_width;
                    this.Height = new_height;
                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }
        /// <summary>
        /// Real postion of object.
        /// </summary>
        public Point Position
        {
            get
            {
                double x = Margin.Left;
                double y = Margin.Top;
                Point p = new Point(x, y);
                return p;
            }
            set
            {
                Point p = value;
                Margin = new Thickness(p.X, p.Y, 0, 0);
            }
        }

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(Point point)
        {
            double left = this.Margin.Left;
            double top = this.Margin.Top;
            double right = left + this.Width;
            double bottom = top + this.Height;

            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 3;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        // Set a mouse cursor appropriate for the current hit type.
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }



        #endregion

       
    }
}
