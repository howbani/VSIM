using Accord.Statistics.Distributions.Univariate;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VANET_SIM.db;
using VANET_SIM.Operations;
using VANET_SIM.Properties;
using VANET_SIM.RoadNet.Components;
using VANET_SIM.Routing;
using VANET_SIM.UI;

namespace VANET_SIM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public DispatcherTimer RandomSelectSourceNodeTimer = new DispatcherTimer();
        public List<Junction> MyJunctions = new List<Junction>();
        public List<RoadSegment> MyRoadSegments = new List<RoadSegment>();
        public List<VehicleUi> MyVehicles = new List<VehicleUi>();
        public double StreenTimes = 2.01;
        public MainWindow()
        {
            InitializeComponent();

            Height = SystemParameters.FullPrimaryScreenHeight;
            Width = SystemParameters.FullPrimaryScreenWidth;

            vanet_scroller.MaxWidth = SystemParameters.FullPrimaryScreenWidth - 10;
            vanet_scroller.MaxHeight = SystemParameters.FullPrimaryScreenHeight - 60;


            canvas_vanet.Width = StreenTimes * SystemParameters.FullPrimaryScreenWidth;
            canvas_vanet.Height = StreenTimes * SystemParameters.FullPrimaryScreenHeight;

           

            PublicStatistics.LiveStatstics.Topmost = true;
            PublicStatistics.LiveStatstics.Show();
            Settings.Default.IsIntialized = false;

            
            // re-intialize
            /*
          // double tr= MyMath.Combination(10, 2);

            SegmentConnectivitySelector reouter = new SegmentConnectivitySelector();
            double myVechilesCountInTheSegment = 5;
            double myRoadSegmentLength = 1000;
            double myVehicleInterArrivalMean = 5;
            double myComunicationRange = 100;
            double numberoflanes = 3;


            for (int x=1;x<=20;x++)
            {
                // myRoadSegmentLength =(x * 300);
                //Console.WriteLine(myRoadSegmentLength);
                // myComunicationRange = (x * 50);
                // Console.WriteLine(myComunicationRange);
                myVechilesCountInTheSegment=x;
                //Console.WriteLine(myVechilesCountInTheSegment);
                double pro = reouter.SegmentConnectivity1(myRoadSegmentLength, myVechilesCountInTheSegment, myVehicleInterArrivalMean, myComunicationRange, numberoflanes);
                Console.WriteLine(pro);
            }*/

            /*
            double transDis = 10;
            double myComunicationRange = 1000;
            for (int x = 1; x <= 10; x++)
            {
                transDis = (x * 50);
                double pr = Nakagami.GetNakagami(transDis, myComunicationRange);
                Console.WriteLine(pr);
            }*/

            /*
            double si = 100;
            double sj = 0;
            for (int x = 1; x <= 10; x++)
            {
          
                sj = (x * 10);

                double pr = IntraRouting.SpeedDiffrenceDist(si, sj, 120);
                Console.WriteLine(pr);
            }*/

            /*
            double NumberofPacketsInTheBuffer = 1;
            double BufferSize = 50;
            for (int x = 0; x <= 5; x++)
            {

                NumberofPacketsInTheBuffer = (x * 10);
                double pr = IntraRouting.BufferSizeDistribution(NumberofPacketsInTheBuffer, BufferSize);
                Console.WriteLine(pr);
            }
            */






        }



        private void ComponentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Point po = Mouse.GetPosition(canvas_vanet);

            MenuItem item = sender as MenuItem;
            string itemString = item.Header.ToString();
            switch (itemString)
            {
                case "_Add Road Segment":
                    RoadSegment rs = new RoadSegment(this, RoadOrientation.Horizontal);
                    rs.Margin = new Thickness(po.X + 50, po.Y + 50, 0, 0);
                    canvas_vanet.Children.Add(rs);
                    break;
                case "_Add Junction":
                    Junction jun = new Junction(this);
                    jun.Margin = new Thickness(po.X + 50, po.Y + 50, 0, 0);
                    canvas_vanet.Children.Add(jun);
                    break;
                case "_Import Vanet":
                    UiImportTopology uim = new db.UiImportTopology(this);
                    uim.Show();
                    break;
                case "_Export Vanet":
                    UiExportTopology win = new UiExportTopology(this);
                    win.Show();
                    break;
                case "_Clear":
                    {
                        foreach (RoadSegment s in MyRoadSegments)
                        {
                            s.stopGeneratingVehicles();
                        }
                        Settings.Default.IsIntialized = false; // re-intialize
                        canvas_vanet.Children.Clear();
                        MyJunctions.Clear();
                        MyRoadSegments.Clear();
                        MyVehicles.Clear();
                        PublicStatistics.Clear();
                    }
                    break;
                
            }
        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sctim = StreenTimes / 10;
            double x = _slider.Value;
            if (x <= sctim) { x = sctim; }
            var scaler = canvas_vanet.LayoutTransform as ScaleTransform;
            canvas_vanet.LayoutTransform = new ScaleTransform(x, x, SystemParameters.FullPrimaryScreenWidth / 2, SystemParameters.FullPrimaryScreenHeight / 2);
            lbl_zome_percentage.Text = (x * 100).ToString() + "%";


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _slider.Value = 4;
        }

       
        /// <summary>
        /// change packet rate.
        /// </summary>
        /// <param name="s"></param>
        public void ChangePacketRange(double s)
        {
            if (MyVehicles.Count > 0)
            {
                if (s == 0)
                {
                    RandomSelectSourceNodeTimer.Stop();
                    RandomSelectSourceNodeTimer.Interval = TimeSpan.FromSeconds(0);

                }
                else
                {
                    RandomSelectSourceNodeTimer.Tick += GeneratePacket_to_rate;
                    RandomSelectSourceNodeTimer.Interval = TimeSpan.FromSeconds(s);
                    RandomSelectSourceNodeTimer.Start();

                }
            }
            else
            {
                MessageBox.Show("Vehicle are not deployed.");
            }
        }
        /// <summary>
        /// generate packet according to the rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneratePacket_to_rate(object sender, EventArgs e)
        {
            RunPacketGenerator();
        }

        /// <summary>
        /// packet generator
        /// </summary>
        public void RunPacketGenerator()
        {
            // select random vehicle:
            if (Settings.Default.IsIntialized)
            {
                if (MyVehicles.Count > 0)
                {
                    // select the source:

                    int max = MyVehicles.Count;
                    if (max >= 2)
                    {
                        int rand = Convert.ToInt16(RandomeNumberGenerator.GetUniform(max - 1));
                        MyVehicles[rand].RandomDestinationVehicle();
                    }
                }
            }
        }


        private int XCounter = 0;
        private int spesifiedXofpackets = 0;
        /// <summary>
        /// set the timer to generate x of packets
        /// </summary>
        /// <param name="xofpackets"></param>
        public void GenerateXofPackets(int xofpackets)
        {
            XCounter = 0;
            spesifiedXofpackets = xofpackets;
            RandomSelectSourceNodeTimer.Tick += GenerateXofPacketTrick;
            RandomSelectSourceNodeTimer.Interval = TimeSpan.FromSeconds(0.1);
            RandomSelectSourceNodeTimer.Start();
        }

        /// <summary>
        /// generate x of packets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GenerateXofPacketTrick(object sender, EventArgs e)
        {
            XCounter += 1;
            if (spesifiedXofpackets < XCounter)
            {
                RandomSelectSourceNodeTimer.Stop();
                RandomSelectSourceNodeTimer.Interval = TimeSpan.FromSeconds(0);
                spesifiedXofpackets = 0;
                XCounter = 0;
            }
            else
            {
                RunPacketGenerator();
            }
        }


        public void DeplayVechiles() 
        {
            if (MyRoadSegments.Count > 0 && MyJunctions.Count > 0)
            {
                
                foreach (RoadSegment s in MyRoadSegments)
                {
                    s.VehicleInterArrivalMean = Computations.VehicleInterArrivalMean;

                    s.SetAsEntry();
                }

                PublicStatistics.LiveStatstics.lbl_number_of_junctions.Content = MyJunctions.Count;
                PublicStatistics.LiveStatstics.lbl_number_of_road_segments.Content = MyRoadSegments.Count;
                PublicStatistics.LiveStatstics.lbl_max_speed.Content = PublicParamerters.MaxSpeed + "Kmh";
                PublicStatistics.LiveStatstics.lbl_min_speed.Content = PublicParamerters.MinSpeed + "Kmh";
                PublicStatistics.LiveStatstics.lbl_average_speed.Content = PublicParamerters.MeanSpeed + "Kmh";
                PublicStatistics.LiveStatstics.lbl_com_range.Content = PublicParamerters.CommunicationRaduis + "m";
                PublicStatistics.LiveStatstics.lbl_data_packet_size.Content= PublicParamerters.DataPacketLength + "bit";

                Settings.Default.IsIntialized = true;
            }
            else
            {
                MessageBox.Show("Road Network should be added. Road Network>Import Vanet","Error.");
            }
        }

        private void btn_change_packet_rate(object sender, RoutedEventArgs e)
        {
            // send packets options.
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                switch (Header)
                {
                    case "1pck/0": // stop.
                        ChangePacketRange(0);
                        break;
                    case "1pck/1ms": // stop.
                        ChangePacketRange(0.001); // packet per ms.
                        break;
                    case "pck/0.5s": // packet per half second.
                        ChangePacketRange(0.5);
                        break;
                    case "1pck/1s":
                        ChangePacketRange(1);
                        break;
                    case "1pck/3s":
                        ChangePacketRange(2);
                        break;
                    case "1pck/5s":
                        ChangePacketRange(2);
                        break;
                    case "1pck/10s":
                        ChangePacketRange(4);
                        break;
                    case "1pck/15s":
                        ChangePacketRange(6);
                        break;
                    case "1pck/20s":
                        ChangePacketRange(8);
                        break;
                    case "1pck/30s":
                        ChangePacketRange(10);
                        break;

                }
            }
        }

        private void btn_set_number_of_vechiles(object sender, RoutedEventArgs e)
        {
            // send packets options.
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            switch (Header)
            {
                case "Unlimited":
                    Settings.Default.IsSetMaxVehicles = false;
                    DeplayVechiles();
                    break;
                case "1":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 1;
                    DeplayVechiles();
                    break;
                case "5":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 5;
                    DeplayVechiles();
                    break;
                case "10":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 10;
                    DeplayVechiles();
                    break;
                case "20":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 20;
                    DeplayVechiles();
                    break;
                case "30":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 30;
                    DeplayVechiles();
                    break;
                case "50":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 50;
                    DeplayVechiles();
                    break;
                case "80":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 80;
                    DeplayVechiles();
                    break;
                case "100":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 100;
                    DeplayVechiles();
                    break;
                case "150":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 150;
                    DeplayVechiles();
                    break;
                case "200":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 200;
                    DeplayVechiles();
                    break;
                case "250":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 250;
                    DeplayVechiles();
                    break;
                case "300":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 300;
                    DeplayVechiles();
                    break;
                case "350":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 350;
                    DeplayVechiles();
                    break;
                case "400":
                    Settings.Default.IsSetMaxVehicles = true;
                    Settings.Default.MaxNumberOfVehicles = 400;
                    DeplayVechiles();
                    break;
            }
        }



        private void btn_generate_x_of_packets(object sender, RoutedEventArgs e)
        {
            // send packets options.
            MenuItem item = sender as MenuItem;
            string Header = item.Header.ToString();
            if (Settings.Default.IsIntialized)
            {
                switch (Header)
                {
                    case "1": // stop.
                        GenerateXofPackets(1);
                        break;
                    case "3": // stop.
                        GenerateXofPackets(3);
                        break;
                    case "10":
                        GenerateXofPackets(10);
                        break;
                    case "100":
                        GenerateXofPackets(100);
                        break;
                    case "300":
                        GenerateXofPackets(300);
                        break;
                    case "500":
                        GenerateXofPackets(500);
                        break;
                    case "1000":
                        GenerateXofPackets(1000);
                        break;
                    case "1500":
                        GenerateXofPackets(1500);
                        break;
                    case "2000":
                        GenerateXofPackets(2000);
                        break;
                    case "3000":
                        GenerateXofPackets(3000);
                        break;
                    case "5000":
                        GenerateXofPackets(5000);
                        break;
                    case "10000":
                        GenerateXofPackets(10000);
                        break;

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                MessageBoxResult ms = MessageBoxResult.Yes; // MessageBox.Show("Sure?", "Vanet", MessageBoxButton.YesNo);
                if (ms == MessageBoxResult.Yes)
                {
                    PublicStatistics.LiveStatstics.Close();
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch
            {

            }
        }

        private void btn_show_results(object sender, RoutedEventArgs e)
        {
            try
            {
                // send packets options.
                MenuItem item = sender as MenuItem;
                string Header = item.Header.ToString();
                if (Settings.Default.IsIntialized)
                {
                    switch (Header)
                    {
                        case "_Show Packets": // stop.
                            List<object> List = new List<object>();
                            List.AddRange(PublicStatistics.DeleiverdPacketsList);
                            List.AddRange(PublicStatistics.DropedPacketsList);
                            if (List.Count > 0)
                            {
                                UiShowResults sh = new UiShowResults(List);
                                // sh.dg_results.ItemsSource = PublicParamerters.DeleiverdPacketsList;
                                sh.Show();
                            }
                            else
                            {
                                MessageBox.Show("No Results Found!");
                            }
                        
                            break;
                    }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "MianWindow-btn_show_results");
            }
        }

        private void btn_show_v_info(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string Header = item.Name.ToString();
            if (Settings.Default.IsIntialized)
            {
                switch (Header)
                {
                    //op_none
                    case "op_none": // stop.
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.None;
                        break;
                    case "op_VID": // stop.
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.VID;
                        break;
                    case "op_SpeedKMH": // stop.
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.SpeedKMH;
                        break;
                    case "op_SpeedTimer":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.SpeedTimer;
                        break;
                    case "op_RID":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.RID;
                        break;
                    case "op_SJID":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.SJID;
                        break;
                    case "op_DJID":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.DJID;
                        break;
                    case "op_LaneIndex":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.LaneIndex;
                        break;
                    case "op_InstanceLocation":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.InstanceLocation;
                        break;
                    case "op_RemianDistanceToHeadingJunction":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.RemianDistanceToHeadingJunction;
                        break;
                    case "op_TravelledDistanceInMeter":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.TravelledDistanceInMeter;
                        break;
                    case "op_ExceededDistanceInMeter":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.ExceededDistanceInMeter;
                        break;
                    case "op_PacketsQueueLength":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.PacketsQueueLength;
                        break;
                    case "op_JunctionQueueIndex":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.JunctionQueueIndex;
                        break;
                    case "op_RIDpluseLaneIndex":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.RIDpluseLaneIndex;
                        break;
                    //SwitchDirection, Direction

                    case "op_SwitchDirection":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.SwitchDirection;
                        break;
                    case "op_Direction":
                        PublicParamerters.DisplayInfoFlag = VehicleUi.Vehicleinfo.Direction;
                        break;
                }
            }
        }

       
        
    }
}
