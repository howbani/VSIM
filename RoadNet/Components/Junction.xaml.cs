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
using VSIM.Properties;
using VSIM.Routing;

namespace VSIM.RoadNet.Components
{
    /// <summary>
    /// Interaction logic for Junction.xaml
    /// </summary>
    public partial class Junction : UserControl
    {
        private DispatcherTimer TraficSignalingTimer = new DispatcherTimer(); // the trafic sign counter.
        public int IncicdentRoadSegmentCount { get; set; } // the number of raod segment that connected to this junction.
       
        public int JID { get; set; } // junction di
        public  MainWindow _MainWindow;
        // Road segments:
        public RoadSegment ToNorthRoadSegment { get; set; }
        public RoadSegment ToSouthRoadSegment { get; set; }
        public RoadSegment ToEastRoadSegment { get; set; }
        public RoadSegment ToWestRoadSegment { get; set; }

        public SegmentSwitch SegSwitch; // the direction swich manger for the junction.
        public Junction(MainWindow MAINWINDOW)
        {
            InitializeComponent();
            _MainWindow = MAINWINDOW;

            CurrentTraficSignal = TraficSignal.VerticalGreen;
            TraficSignalingTimer.Interval = TimeSpan.FromSeconds(Settings.Default.TraficSignalingTimerInterval);
            TraficSignalingTimer.Start();
            TraficSignalingTimer.Tick += TraficSignalingTimer_Tick;


            Height = PublicParamerters.JunctionHeight;
            Width = PublicParamerters.JunctionWidth;

            JID = MAINWINDOW.MyJunctions.Count;
            MAINWINDOW.MyJunctions.Add(this);
            lbl_junction_id.Text = JID.ToString();
        }

        public TraficSignal CurrentTraficSignal
        {
            get;set;
        }

        /// <summary>
        /// get the road segment that connected to this jucntion.
        /// </summary>
        public List<RoadSegment> AdjacentRoadSegment
        {
            get
            {
                List<RoadSegment> re = new List<RoadSegment>();
                if(ToNorthRoadSegment != null) re.Add(ToNorthRoadSegment);
                if (ToSouthRoadSegment != null) re.Add(ToSouthRoadSegment);
                if (ToEastRoadSegment != null) re.Add(ToEastRoadSegment);
                if (ToWestRoadSegment != null) re.Add(ToWestRoadSegment);
                return re;
            }
        }

        /// <summary>
        /// neighbors junctions.
        /// </summary>
        public List<Junction> Adjacentjunctions
        {
            get
            {
                List<Junction> jun = new List<Junction>();
                foreach (RoadSegment rs in AdjacentRoadSegment)
                {
                    if (rs != null)
                    {
                        foreach (Junction j in rs.MyJunctions)
                        {
                            if (j != null)
                            {
                                if (j != this)
                                {
                                    if (!jun.Contains(j)) { jun.Add(j); }
                                }
                            }
                        }
                    }
                }
                return jun;
            }
        }


        public int VerticalGreenValue { get; set; } // this value depends on the number of road segment that join this junction.
        public int HorizontalGreenValue { get; set; }// this value depends on the number of road segment that join this junction.
        private int _vgc = 0; //  _VerticalGreenCounter: instance counter.
        private int _hgc = 0;  // _HorizontalGreenCounter: instance counter.
        private void TraficSignalingTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                //open
                Action n2e = () => SegSwitch.SwitchNorthToEastSegment();
                Dispatcher.Invoke(n2e, DispatcherPriority.Send);
                Action s2w = () => SegSwitch.SwitchSouthToWestSegment();
                Dispatcher.Invoke(s2w, DispatcherPriority.Send);
                Action e2s = () => SegSwitch.SwitchEastToSouthSegment();
                Dispatcher.Invoke(e2s, DispatcherPriority.Send);
                Action w2n = () => SegSwitch.SwitchWestToNorthSegment();
                Dispatcher.Invoke(w2n, DispatcherPriority.Send);


                if (IncicdentRoadSegmentCount == 2 || IncicdentRoadSegmentCount == 1)
                {
                    // green all the time.
                    CurrentTraficSignal = TraficSignal.VerticalGreen;
                    CurrentTraficSignal = TraficSignal.HorizontalGreen;

                    lbl_ver_north.Foreground = Brushes.Green;
                    lbl_ver_south.Foreground = Brushes.Green;
                    lbl_hor_west.Foreground = Brushes.Green;
                    lbl_hor_east.Foreground = Brushes.Green;
                    // open all.
                    Action n2n = () => SegSwitch.SwitchNorthToNorthSegment();
                    Dispatcher.Invoke(n2n);
                    Action s2s = () => SegSwitch.SwitchSouthToSouthSegment();
                    Dispatcher.Invoke(s2s, DispatcherPriority.Send);

                    // north west or south east.
                    Action n2w = () => SegSwitch.SwitchNorthToWestSegment();
                    Dispatcher.Invoke(n2w);
                    Action s2e = () => SegSwitch.SwitchSouthToEastSegment();
                    Dispatcher.Invoke(s2e, DispatcherPriority.Send);

                    Action e2e = () => SegSwitch.SwitchEastToEastSegment();
                    Dispatcher.Invoke(e2e, DispatcherPriority.Send);
                    Action w2w = () => SegSwitch.SwitchWestToWestSegment();
                    Dispatcher.Invoke(w2w, DispatcherPriority.Send);

                    Action e2n = () => SegSwitch.SwitchEastToNorthSegment();
                    Dispatcher.Invoke(e2n, DispatcherPriority.Send);
                    Action w2s = () => SegSwitch.SwitchWestToSouthSegment();
                    Dispatcher.Invoke(w2s, DispatcherPriority.Send);

                }
                if (IncicdentRoadSegmentCount == 3 || IncicdentRoadSegmentCount == 4)
                {
                    // need to swhich between green and re.
                    if (CurrentTraficSignal == TraficSignal.VerticalGreen)
                    {
                        _vgc++;
                        if (_vgc <= VerticalGreenValue)
                        {
                            Action a1 = () => SegSwitch.SwitchNorthToNorthSegment();
                            Dispatcher.Invoke(a1);
                            Action a2 = () => SegSwitch.SwitchSouthToSouthSegment();
                            Dispatcher.Invoke(a2, DispatcherPriority.Send);


                        }
                        else if (_vgc > VerticalGreenValue && _vgc <= (VerticalGreenValue * 2))
                        {
                            // north west or south east.
                            Action a3 = () => SegSwitch.SwitchNorthToWestSegment();
                            Dispatcher.Invoke(a3);
                            Action a4 = () => SegSwitch.SwitchSouthToEastSegment();
                            Dispatcher.Invoke(a4, DispatcherPriority.Send);


                        }
                        else
                        {
                            CurrentTraficSignal = TraficSignal.HorizontalGreen; // change.
                            lbl_ver_north.Foreground = Brushes.Red;
                            lbl_ver_south.Foreground = Brushes.Red;

                            lbl_hor_west.Foreground = Brushes.Green;
                            lbl_hor_east.Foreground = Brushes.Green;

                            _vgc = 0;
                            _hgc = 0;
                        }
                    }
                    else if (CurrentTraficSignal == TraficSignal.HorizontalGreen)
                    {
                        _hgc++;
                        if (_hgc <= HorizontalGreenValue)
                        {
                            Action a5 = () => SegSwitch.SwitchEastToEastSegment();
                            Dispatcher.Invoke(a5, DispatcherPriority.Send);
                            Action a6 = () => SegSwitch.SwitchWestToWestSegment();
                            Dispatcher.Invoke(a6, DispatcherPriority.Send);
                        }
                        else if (_hgc > HorizontalGreenValue && _hgc <= (HorizontalGreenValue * 2))
                        {
                            Action a7 = () => SegSwitch.SwitchEastToNorthSegment();
                            Dispatcher.Invoke(a7, DispatcherPriority.Send);
                            Action a8 = () => SegSwitch.SwitchWestToSouthSegment();
                            Dispatcher.Invoke(a8, DispatcherPriority.Send);
                        }
                        else
                        {
                            CurrentTraficSignal = TraficSignal.VerticalGreen;
                            lbl_ver_north.Foreground = Brushes.Green;
                            lbl_ver_south.Foreground = Brushes.Green;
                            lbl_hor_west.Foreground = Brushes.Red;
                            lbl_hor_east.Foreground = Brushes.Red;
                            _vgc = 0;
                            _hgc = 0;
                        }
                    }
                }
            });
        }


        public Point CenterLocation => new Point(Margin.Left + Width, Margin.Top + Height);

        public Point Location => new Point(Margin.Left, Margin.Top);

        public Point TopLeftCorner => Location;
        public Point TopRightCorner => new Point(Margin.Left + Width, Margin.Top);

        public Point BottomLeftCorner => new Point(Margin.Left, Margin.Top + Height);

        public Point BottomRightCorner => new Point(Margin.Left + Width, Margin.Top + Height);

        public Point RightCenter => new Point(Margin.Left + Width, Margin.Top + (Height / 2));
        public Point BottomCenter => new Point(Margin.Left + (Width/2), Margin.Top + (Height));


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
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
            Mouse.Capture(null);
        }

        // Start dragging.
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Settings.Default.IsIntialized)
            {
                Mouse.Capture(this);
                Point po = new Point(Mouse.GetPosition(_MainWindow.canvas_vanet).X, Mouse.GetPosition(_MainWindow.canvas_vanet).Y);
                MouseHitType = SetHitType(po);
                SetMouseCursor();
                if (MouseHitType == HitType.None) return;
                LastPoint = Mouse.GetPosition(_MainWindow.canvas_vanet);
                DragInProgress = true;
            }
        }
         

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                Point po = Mouse.GetPosition(_MainWindow.canvas_vanet);
                MouseHitType = SetHitType(po);
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(_MainWindow.canvas_vanet);
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



        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            Junction x = new Components.Junction(_MainWindow);
            Point po = Mouse.GetPosition(_MainWindow.canvas_vanet);
            x.Margin = new Thickness(po.X + 20, po.Y + 10, 0, 0);
            _MainWindow.canvas_vanet.Children.Add(x);
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _MainWindow.MyJunctions.Remove(this);
                _MainWindow.canvas_vanet.Children.Remove(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
         
        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            /*
            if(ToNorthRoadSegment!=null)
            {
                ToNorthRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Red;
            }
            if (ToSouthRoadSegment != null)
            {
                ToSouthRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Red;
            }
            if (ToEastRoadSegment != null)
            {
                ToEastRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Red;
            }
            if (ToWestRoadSegment != null)
            {
                ToWestRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Red;
            }*/

        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            /*
            if (ToNorthRoadSegment != null)
            {
                ToNorthRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Yellow;
            }
            if (ToSouthRoadSegment != null)
            {
                ToSouthRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Yellow;
            }
            if (ToEastRoadSegment != null)
            {
                ToEastRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Yellow;
            }
            if (ToWestRoadSegment != null)
            {
                ToWestRoadSegment.MyYellowLine.border_yellow_line.Background = Brushes.Yellow;
            }*/
        }

        public void ReLoadLanes()
        {
            SegSwitch = new SegmentSwitch(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReLoadLanes();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            /*
            InterRouting re = new Routing.InterRouting(_MAINWINDOW.MyJunctions[7], _MAINWINDOW.MyJunctions[0]);
            List<CandidateJunction> cans = re.CandidateJunction(this);

            CandidateJunction max = cans[0];
            for (int j = 1; j < cans.Count; j++)
            {
                if (cans[j].Pr_Total > max.Pr_Total)
                {
                    max = cans[j];
                }
            }
            _MAINWINDOW.MyRoadSegments[max.RoadSegmentID].border_mian.Background = Brushes.Red;*/
        }

        private void btn_copy_Click(object sender, MouseButtonEventArgs e)
        {
            Point po = Mouse.GetPosition(_MainWindow.canvas_vanet);
            Junction jun = new Junction(_MainWindow);
            jun.Margin = new Thickness(po.X + 10, po.Y + 10, 0, 0);
            _MainWindow.canvas_vanet.Children.Add(jun);
        }

        private void btn_delete_Click(object sender, MouseButtonEventArgs e)
        {
            _MainWindow.MyJunctions.Remove(this);
            _MainWindow.canvas_vanet.Children.Remove(this);
        }
    }
}
