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
using VSIM.Operations;

namespace VSIM.RoadNet.Components
{
    /// <summary>
    /// Interaction logic for LaneUi.xaml
    /// </summary>
    public partial class LaneUi : UserControl
    {

        public LaneQueue LaneVehicleAndQueue = new LaneQueue();
        private Direction Dir;
        public RoadSegment MyRoadSegment { get; set; }
        public MainWindow _MainWindow { get { return MyRoadSegment._MainWindow; } }
        public string LaneCaption { get; set; }
        //public dis 
        public int NumberofRisingVechilesInLane 
        {
            get { return LaneVehicleAndQueue.CountInLane; }
        }

       
        public LaneUi(string Caption)
        {
            InitializeComponent();
            LaneCaption = Caption;

           
           
        }

        // the current swithc diection
        private Direction swiDir;
        public Direction CurrentSwitchToDirection
        {
            get => swiDir;
            set
            {
                swiDir = value;
                if (LaneDirection == Direction.S)
                {
                    if (value == Direction.W) Dispatcher.Invoke( new Action(()=> lbl_direction_symbol3.Text = "↲"),DispatcherPriority.Send);
                    if (value == Direction.E) Dispatcher.Invoke(new Action(() => lbl_direction_symbol3.Text = "↳"), DispatcherPriority.Send);
                    if (value == Direction.S) Dispatcher.Invoke(new Action(() => lbl_direction_symbol3.Text = "↓"), DispatcherPriority.Send);

                }
                else if (LaneDirection == Direction.N)
                {
                    if (value == Direction.W) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "↰"), DispatcherPriority.Send); 
                    if (value == Direction.E) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "↱"),DispatcherPriority.Send);
                    if (value == Direction.N) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "↑"), DispatcherPriority.Send);
                }
                else if (LaneDirection == Direction.W)
                {

                    if (value == Direction.N) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "⬑"),DispatcherPriority.Send);
                    if (value == Direction.S) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "⬐"),DispatcherPriority.Send);
                    if (value == Direction.W) Dispatcher.Invoke(new Action(() => lbl_direction_symbol1.Text = "←"), DispatcherPriority.Send);
                }
                else if (LaneDirection == Direction.E)
                {
                    if (value == Direction.N) Dispatcher.Invoke(new Action(() => lbl_direction_symbol3.Text = "⬏"),DispatcherPriority.Send);
                    if (value == Direction.S) Dispatcher.Invoke(new Action(() => lbl_direction_symbol3.Text = "⬎"),DispatcherPriority.Send);
                    if (value == Direction.E) Dispatcher.Invoke(new Action(() => lbl_direction_symbol3.Text = "→"), DispatcherPriority.Send);
                }
            }
        }

        /// <summary>
        ///  all my SwitchToDirections d
        /// </summary>
        public List<Direction> SwitchToDirections = new List<Direction>(); // to where the vechile should switch road segment when arrives junction.

        // select one random direction.
        public Direction RandomSwitchToDirection
        {
            get
            {
                if (SwitchToDirections.Count == 1)
                {
                    return SwitchToDirections[0];
                }
                else
                {
                    int x = Convert.ToInt16(RandomeNumberGenerator.GetUniform(SwitchToDirections.Count - 1));
                    return SwitchToDirections[x];
                }
            }
        }


        /// <summary>
        /// get the index of the lane.
        /// </summary>
        public int LaneIndex
        {
            get;set;
        }

        /// <summary>
        ///  Direction of the lane.
        ///  vistual and layout.
        /// </summary>
        public Direction LaneDirection
        {
            get { return Dir; }
            set
            {
                Dir = value;
                switch (Dir.ToString())
                {
                    case "S": // SOUTH
                        lbl_direction_symbol1.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol2.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol3.HorizontalAlignment = HorizontalAlignment.Center;

                        lbl_direction_symbol1.VerticalAlignment = VerticalAlignment.Top;
                        lbl_direction_symbol2.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol3.VerticalAlignment = VerticalAlignment.Bottom;

                        lbl_info.VerticalAlignment = VerticalAlignment.Top;
                        lbl_info.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_info.Margin = new Thickness(0, 5, 0, 0);
                        lbl_direction_symbol1.Text = "↓";
                        lbl_direction_symbol2.Text = "↓";
                        lbl_direction_symbol3.Text = "↓";
                      // if (CurrentSwitchToDirection== Direction.W) lbl_direction_symbol3.Text = "↲";
                     //  if (CurrentSwitchToDirection == Direction.E) lbl_direction_symbol3.Text = "↳";
                       
                        break;
                    case "N": // NORTH

                        lbl_direction_symbol1.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol2.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol3.HorizontalAlignment = HorizontalAlignment.Center;

                        lbl_direction_symbol1.VerticalAlignment = VerticalAlignment.Top;
                        lbl_direction_symbol2.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol3.VerticalAlignment = VerticalAlignment.Bottom;


                        lbl_info.VerticalAlignment = VerticalAlignment.Bottom;
                        lbl_info.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_info.Margin = new Thickness(0, 0, 0, 5);

                        lbl_direction_symbol2.Text = "↑";
                        lbl_direction_symbol3.Text = "↑";
                        lbl_direction_symbol1.Text = "↑";
                       // if (CurrentSwitchToDirection== Direction.ToWest) lbl_direction_symbol1.Text = "↰";
                       // if (CurrentSwitchToDirection == Direction.ToEast) lbl_direction_symbol1.Text = "↱";
                        break;

                    case "W": // WEST

                        lbl_direction_symbol1.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol2.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol3.VerticalAlignment = VerticalAlignment.Center;

                        lbl_direction_symbol1.HorizontalAlignment = HorizontalAlignment.Left;
                        lbl_direction_symbol2.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol3.HorizontalAlignment = HorizontalAlignment.Right;


                        lbl_info.VerticalAlignment = VerticalAlignment.Center;
                        lbl_info.HorizontalAlignment = HorizontalAlignment.Right;
                        lbl_info.Margin = new Thickness(0, 0, 5, 0);

                        lbl_direction_symbol1.Text = "←";
                        lbl_direction_symbol2.Text = "←";
                        lbl_direction_symbol3.Text = "←";
                        //if(CurrentSwitchToDirection== Direction.ToNorth) lbl_direction_symbol1.Text = "⬑";
                       // if (CurrentSwitchToDirection == Direction.ToSouth) lbl_direction_symbol1.Text = "⬐";
                        break;
                    case "E": // EAST
                        lbl_direction_symbol1.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol2.VerticalAlignment = VerticalAlignment.Center;
                        lbl_direction_symbol3.VerticalAlignment = VerticalAlignment.Center;

                        lbl_direction_symbol1.HorizontalAlignment = HorizontalAlignment.Left;
                        lbl_direction_symbol2.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl_direction_symbol3.HorizontalAlignment = HorizontalAlignment.Right;

                        lbl_info.VerticalAlignment = VerticalAlignment.Center;
                        lbl_info.HorizontalAlignment = HorizontalAlignment.Left;
                        lbl_info.Margin = new Thickness(5, 0, 0, 0);
                        lbl_direction_symbol1.Text = "→";
                        lbl_direction_symbol2.Text = "→";
                        lbl_direction_symbol3.Text = "→";
                       // if (CurrentSwitchToDirection == Direction.ToNorth) lbl_direction_symbol3.Text = "⬏";
                       // if(CurrentSwitchToDirection== Direction.ToSouth) lbl_direction_symbol3.Text = "⬎";
                        
                        break;
                }
            }
        }

        /// <summary>
        /// The heading jucntion. that is the junction to which the vechile is moving.
        /// </summary>
        public Junction GetMyHeadingJunction
        {
            get
            {
                if (MyRoadSegment.MyJunctions.Count == 2)
                {
                    if (LaneDirection == Direction.N)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Top < MyRoadSegment.MyJunctions[1].Margin.Top) return MyRoadSegment.MyJunctions[0];
                        else return MyRoadSegment.MyJunctions[1];
                    }
                    else if (LaneDirection == Direction.S)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Top > MyRoadSegment.MyJunctions[1].Margin.Top) return MyRoadSegment.MyJunctions[0];
                        else return MyRoadSegment.MyJunctions[1];
                    }
                    else if (LaneDirection == Direction.W)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Left < MyRoadSegment.MyJunctions[1].Margin.Left) return MyRoadSegment.MyJunctions[0];
                        else return MyRoadSegment.MyJunctions[1];
                    }
                    else if (LaneDirection == Direction.E)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Left > MyRoadSegment.MyJunctions[1].Margin.Left) return MyRoadSegment.MyJunctions[0];
                        else return MyRoadSegment.MyJunctions[1];
                    }
                    else return null;
                }
                else if (MyRoadSegment.MyJunctions.Count == 1)
                {
                    if (LaneDirection == Direction.N)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Top < MyRoadSegment.Margin.Top) return MyRoadSegment.MyJunctions[0];
                        else return null;
                    }
                    else if (LaneDirection == Direction.S)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Top > MyRoadSegment.Margin.Top) return MyRoadSegment.MyJunctions[0];
                        else return null;
                    }
                    else if (LaneDirection == Direction.W)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Left < MyRoadSegment.Margin.Left) return MyRoadSegment.MyJunctions[0];
                        else return null;
                    }
                    else if (LaneDirection == Direction.E)
                    {
                        if (MyRoadSegment.MyJunctions[0].Margin.Left > MyRoadSegment.Margin.Left) return MyRoadSegment.MyJunctions[0];
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }
        /// <summary>
        /// center of the lane. that is where the vechile start move move vertically or horizontally.
        /// </summary>
        public double MyCenterLeft
        {
            get
            {
                if (MyRoadSegment.Roadorientation == RoadOrientation.Horizontal)
                {
                    double re = MyRoadSegment.Margin.Left;
                    return re;
                }
                else if (MyRoadSegment.Roadorientation == RoadOrientation.Vertical)
                {
                    double offsit = 1;
                    if (LaneCaption == LanesCaptions.Lane1)
                        return MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit;
                    else if (LaneCaption == LanesCaptions.Lane2)
                        return (1 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit;
                    else if (LaneCaption == LanesCaptions.Lane3)
                        return (2 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane4)
                        return (3 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane5)
                        return (4 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane6)
                        return (5 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Left + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else return 0;
                }
                else return 0;
            }
        }
        /// <summary>
        /// center of the lane. that is where the vechile move vertically or horizontally.
        /// </summary>
        public double MyCenterTop
        {
            get
            {
                if (MyRoadSegment.Roadorientation == RoadOrientation.Vertical)
                {
                    double re = MyRoadSegment.Margin.Top;
                    return re;
                }
                else if (MyRoadSegment.Roadorientation == RoadOrientation.Horizontal)
                {
                    double offsit = 1;
                    if (LaneCaption == LanesCaptions.Lane1)
                        return MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit;
                    else if (LaneCaption == LanesCaptions.Lane2)
                        return (1 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit;
                    else if (LaneCaption == LanesCaptions.Lane3)
                        return (2 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane4)
                        return (3 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane5)
                        return (4 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else if (LaneCaption == LanesCaptions.Lane6)
                        return (5 * PublicParamerters.LaneWidth) + MyRoadSegment.Margin.Top + (PublicParamerters.LaneWidth / 2) - offsit + 1;
                    else return 0;
                }
                else return 0;
            }
        }

       

        /// <summary>
        /// The entry point of the lane.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point MyEntry(VehicleUi v)
        {
            Point entryCoordinats = new Point();
            if (this.LaneDirection == Direction.N)
            {
                double left = this.MyCenterLeft;
                double top = this.MyCenterTop + MyRoadSegment.Height - v.Height;
                entryCoordinats.X = left;
                entryCoordinats.Y = top;
            }
            else if (this.LaneDirection == Direction.S)
            {
                double left = this.MyCenterLeft;
                double top = this.MyCenterTop;
                entryCoordinats.X = left;
                entryCoordinats.Y = top;
            }
            else if (this.LaneDirection == Direction.E)
            {
                double left = this.MyCenterLeft;
                double top = this.MyCenterTop;
                entryCoordinats.X = left;
                entryCoordinats.Y = top;
            }
            else if (this.LaneDirection == Direction.W)
            {
                double left = this.MyCenterLeft + MyRoadSegment.Width - v.Width;
                double top = this.MyCenterTop;
                entryCoordinats.X = left;
                entryCoordinats.Y = top;
            }
            return entryCoordinats;
        }

        /// <summary>
        /// The exit point in the heading junction.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point MyeExit(VehicleUi v)
        {
            Point entryCoordinats = new Point();
            if (this.LaneDirection == Direction.N)
            {
                double left = MyCenterLeft;
                double top = MyCenterTop + MyRoadSegment.Height - v.Height;
                entryCoordinats.X = left;
                entryCoordinats.Y = top - MyRoadSegment.Height;
            }
            else if (this.LaneDirection == Direction.S)
            {
                double left = this.MyCenterLeft;
                double top = this.MyCenterTop;
                entryCoordinats.X = left;
                entryCoordinats.Y = top + MyRoadSegment.Height;
            }
            else if (this.LaneDirection == Direction.E)
            {
                double left = this.MyCenterLeft;
                double top = this.MyCenterTop;
                entryCoordinats.X = left + MyRoadSegment.Width;
                entryCoordinats.Y = top;
            }
            else if (this.LaneDirection == Direction.W)
            {
                double left = this.MyCenterLeft + MyRoadSegment.Width - v.Width;
                double top = this.MyCenterTop;
                entryCoordinats.X = left - MyRoadSegment.Width;
                entryCoordinats.Y = top;
            }
            return entryCoordinats;
        }

        /// <summary>
        /// determine the nerty of the lane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.IsIntialized == false)
            {
                MenuItem item = sender as MenuItem;
                string itemString = item.Header.ToString();
                switch (itemString)
                {

                    case "Horizontal":
                        if (MyRoadSegment.Roadorientation == RoadOrientation.Vertical)
                        {
                            MyRoadSegment.Roadorientation = RoadOrientation.Horizontal;
                            MyRoadSegment.stack_lanes.Children.Clear();
                            MyRoadSegment.Lanes.Clear();
                            int x = Convert.ToInt16(MyRoadSegment.LanesCount);
                            MyRoadSegment.setHorizontalLayout(x);
                        }
                        break;
                    case "Vertical":
                        if (MyRoadSegment.Roadorientation == RoadOrientation.Horizontal)
                        {
                            MyRoadSegment.Roadorientation = RoadOrientation.Vertical;
                            MyRoadSegment.stack_lanes.Children.Clear();
                            MyRoadSegment.Lanes.Clear();
                            int x = Convert.ToInt16(MyRoadSegment.LanesCount);
                            MyRoadSegment.SetVerticalLayout(x);
                        }
                        break;
                    case "Copy":
                        if (MyRoadSegment.Roadorientation == RoadOrientation.Vertical)
                        {

                            RoadSegment x = new RoadSegment(_MainWindow, MyRoadSegment.LanesCount, RoadOrientation.Vertical);
                            Point po = Mouse.GetPosition(_MainWindow.canvas_vanet);
                            x.Margin = new Thickness(po.X + 20, po.Y + 10, 0, 0);
                            _MainWindow.canvas_vanet.Children.Add(x);
                        }
                        else
                        {
                            RoadSegment x = new RoadSegment(_MainWindow, MyRoadSegment.LanesCount, RoadOrientation.Horizontal);
                            Point po = Mouse.GetPosition(_MainWindow.canvas_vanet);
                            x.Margin = new Thickness(po.X + 20, po.Y + 10, 0, 0);
                            _MainWindow.canvas_vanet.Children.Add(x);
                        }
                        break;
                    case "Two":
                        if (MyRoadSegment.LanesCount != 2)
                        {
                            RoadSegment newx = new RoadSegment(_MainWindow, 2, MyRoadSegment.Roadorientation);
                            newx.Margin = MyRoadSegment.Margin;
                            newx.Height = MyRoadSegment.Height;
                            newx.Width = MyRoadSegment.Width;
                            _MainWindow.canvas_vanet.Children.Add(newx);
                            _MainWindow.canvas_vanet.Children.Remove(MyRoadSegment);
                            _MainWindow.MyRoadSegments.Remove(MyRoadSegment);

                            if(newx.Roadorientation== RoadOrientation.Horizontal)
                            {
                                newx.Height = newx.LanesCount * PublicParamerters.LaneWidth +1.5;
                            }
                            else
                            {
                                newx.Width = newx.LanesCount * PublicParamerters.LaneWidth +1.5;
                            }
                        }

                        break;

                    case "Four":
                        if (MyRoadSegment.LanesCount != 4)
                        {
                            RoadSegment newx = new RoadSegment(_MainWindow, 4, MyRoadSegment.Roadorientation);
                            newx.Margin = MyRoadSegment.Margin;
                            newx.Height = MyRoadSegment.Height;
                            newx.Width = MyRoadSegment.Width;
                            _MainWindow.canvas_vanet.Children.Add(newx);
                            _MainWindow.canvas_vanet.Children.Remove(MyRoadSegment);
                            _MainWindow.MyRoadSegments.Remove(MyRoadSegment);

                            if (newx.Roadorientation == RoadOrientation.Horizontal)
                            {
                                newx.Height = newx.LanesCount * PublicParamerters.LaneWidth+1.5;
                            }
                            else
                            {
                                newx.Width = newx.LanesCount * PublicParamerters.LaneWidth+1.5;
                            }
                        }

                        break;

                    case "Six":
                        if (MyRoadSegment.LanesCount != 6)
                        {
                            RoadSegment newx = new RoadSegment(_MainWindow, 6, MyRoadSegment.Roadorientation);
                            newx.Margin = MyRoadSegment.Margin;
                            newx.Height = MyRoadSegment.Height;
                            newx.Width = MyRoadSegment.Width;
                            _MainWindow.canvas_vanet.Children.Add(newx);
                            _MainWindow.canvas_vanet.Children.Remove(MyRoadSegment);
                            _MainWindow.MyRoadSegments.Remove(MyRoadSegment);

                            if (newx.Roadorientation == RoadOrientation.Horizontal)
                            {
                                newx.Height = newx.LanesCount * PublicParamerters.LaneWidth+1.5;
                            }
                            else
                            {
                                newx.Width = newx.LanesCount * PublicParamerters.LaneWidth+1.5;
                            }
                        }

                        break;
                    case "Delete":
                        try
                        {
                            _MainWindow.MyRoadSegments.Remove(MyRoadSegment);
                            _MainWindow.canvas_vanet.Children.Remove(MyRoadSegment);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("This action is invalid after starting simulation.");
            }
        }

       
    }
}
