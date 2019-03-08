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
using System.Windows.Shapes;
using VSIM.db;
using VSIM.Operations;
using VSIM.Properties;

namespace VSIM.experments
{
    /// <summary>
    /// Interaction logic for UIsetExperment.xaml
    /// </summary>
    public partial class UIsetExperment : Window
    {
        public bool isCloseUpbale { get; set; }
        MainWindow mainWindow;
        public UIsetExperment( MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;

            // laod the networks names:
            NetworkTopolgy.ImportNetworkNames(comb_network_names); // laod the names of the network.
                                                                   // INTIALIZE THE COM RANGE LIST:
            for (int i = 1; i <= 5; i += 1) combo_back_direction_paramater.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            for (int i = 50; i <= 500; i += 50) comb_com_raduis.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            for (int i = 30; i <= 180; i += 30) { comb_maxSpeed.Items.Add(new ComboBoxItem() { Content = i.ToString() }); comb_minSpeed.Items.Add(new ComboBoxItem() { Content = i.ToString() }); }
            for (int i = 100; i <= 3000; i += 100) { combo_packets.Items.Add(new ComboBoxItem() { Content = i.ToString() }); }
            for (int i = 50; i <= 1400; i += 50) { combo_numb_vehicles.Items.Add(new ComboBoxItem() { Content = i.ToString() }); }
            for (int i = 1; i <= 20; i += 1) { combo_max_attemps.Items.Add(new ComboBoxItem() { Content = i.ToString() }); } // maximum attemps to retransmit the packet.
            for (int i = 1; i <= 50; i += 1) { combo_max_stor_time.Items.Add(new ComboBoxItem() { Content = i.ToString() }); } // maximum attemps to retransmit the packet.
            combo_trafic_ligh.Items.Add(new ComboBoxItem() { Content = "0.01" });
            combo_trafic_ligh.Items.Add(new ComboBoxItem() { Content = "0.1" });
            combo_trafic_ligh.Items.Add(new ComboBoxItem() { Content = "0.5" });
            for (int i = 1; i <= 10; i += 1) { combo_trafic_ligh.Items.Add(new ComboBoxItem() { Content = i.ToString() }); } // maximum attemps to retransmit the packet.
            for(int i = 0; i <= 9; i++) { string s = "0." + i.ToString(); combo_forward_direction_paramater.Items.Add(new ComboBoxItem() { Content = s }); combo_connectivity_wight.Items.Add( new ComboBoxItem() { Content= s }); combo_shortest_distance_weight.Items.Add(new ComboBoxItem() { Content = s }); }
            combo_connectivity_wight.Items.Add(new ComboBoxItem() { Content = 1 });
            combo_shortest_distance_weight.Items.Add(new ComboBoxItem() { Content = 1 });

            // show the defuals values:
            comb_com_raduis.Text = Settings.Default.CommunicationRange.ToString();
            comb_minSpeed.Text = Settings.Default.MinSpeed.ToString();
            combo_packets.Text = Settings.Default.NumberofPackets.ToString();
            comb_maxSpeed.Text = Settings.Default.MaxSpeed.ToString();
            comb_com_raduis.Text = Settings.Default.CommunicationRange.ToString();
            combo_numb_vehicles.Text = Settings.Default.MaxNumberOfVehicles.ToString();
            combo_max_attemps.Text = Settings.Default.MaximumAttemps.ToString();
            combo_max_stor_time.Text= Settings.Default.MaxStoreTime.ToString();
            combo_trafic_ligh.Text= Settings.Default.TraficSignalingTimerInterval.ToString();
            comb_network_names.Text= Settings.Default.NetTopName.ToString();

            combo_shortest_distance_weight.Text = Settings.Default.WeightShortestDistance.ToString();
            combo_connectivity_wight.Text = Settings.Default.WeightConnectivity.ToString();
            combo_forward_direction_paramater.Text = Settings.Default.IntraVehiForwardDirectionPar.ToString();
            combo_back_direction_paramater.Text = Settings.Default.IntraVehiBackwardDirectionPar.ToString();

        }

        private void Btn_set_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.MyRoadSegments.Count > 0 && mainWindow.MyJunctions.Count > 0)
            {
                if (combo_numb_vehicles.SelectedItem != null)
                {
                    if (combo_packets.SelectedItem != null)
                    {
                        if (comb_com_raduis.SelectedItem != null)
                        {
                            Settings.Default.IntraVehiBackwardDirectionPar= Convert.ToDouble((combo_back_direction_paramater.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.IntraVehiForwardDirectionPar= Convert.ToDouble((combo_forward_direction_paramater.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.WeightConnectivity= Convert.ToDouble((combo_connectivity_wight.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.WeightShortestDistance= Convert.ToDouble((combo_shortest_distance_weight.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.TraficSignalingTimerInterval = Convert.ToDouble((combo_trafic_ligh.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.CommunicationRange = Convert.ToDouble((comb_com_raduis.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.MinSpeed = Convert.ToDouble((comb_minSpeed.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.MaxSpeed = Convert.ToDouble((comb_maxSpeed.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.MaximumAttemps= Convert.ToInt16((combo_max_attemps.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.MaxStoreTime= Convert.ToDouble((combo_max_stor_time.SelectedItem as ComboBoxItem).Content.ToString());
                            Settings.Default.NetTopName = (comb_network_names.SelectedItem as ComboBoxItem).Content.ToString();
                            try
                            {
                                string dis = (combo_dist.SelectedItem as ComboBoxItem).Content.ToString();
                                if (dis != null) if (dis != "None" || dis != "") Settings.Default.DistanceBetweenSourceAndDestination = Convert.ToDouble(dis);
                            }
                            catch
                            {

                            }


                            if (Settings.Default.MaxSpeed > Settings.Default.MinSpeed)
                            {
                                Settings.Default.NumberofPackets = Convert.ToInt16((combo_packets.SelectedItem as ComboBoxItem).Content.ToString());
                                Settings.Default.MaxNumberOfVehicles = Convert.ToInt16((combo_numb_vehicles.SelectedItem as ComboBoxItem).Content.ToString()); ;
                                Settings.Default.IsSetMaxVehicles = true;
                                Settings.Default.IsIntialized = true;
                                
                                Settings.Default.Save();

                                // random vehicles:
                                FairVehiclesDistrubutions vegicleDist = new FairVehiclesDistrubutions(mainWindow.MyRoadSegments);
                                vegicleDist.Distribute(Settings.Default.MaxNumberOfVehicles, VehiclesDistrubution.Systemized);

                                // send the packets:
                                mainWindow.GenerateXofPackets(Settings.Default.NumberofPackets);
                                //
                                mainWindow.DisplayInfo();

                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Minspeed should not be greater than Maxspeed.");
                            }

                        }
                        else
                        {
                            MessageBox.Show("set the com. range");
                        }
                    }
                    else
                    {
                        MessageBox.Show("set the number of packets!");
                    }
                }
                else 
                {
                    MessageBox.Show("set the number of vehicles!");
                }
            }
            else
            {
                MessageBox.Show("Road network is empty. please select it and start agian!.");
            }

        }

        private void Comb_network_names_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comb_network_names.SelectedItem != null)
            {
                // clear:
                // import the bnetwork
                string namenet = (comb_network_names.SelectedItem as ComboBoxItem).Content.ToString();
                DeserilizeNetwork.DesrlizeNetwork(mainWindow, namenet);


                combo_dist.Items.Clear();
                int maxDis = Convert.ToInt32(AverageSgmentLength);
                int minDis = Convert.ToInt32(maxDis / 10);
                combo_dist.Items.Add(new ComboBoxItem() { Content = "None" }); 
                for (int i = minDis; i <= maxDis; i += minDis) { combo_dist.Items.Add(new ComboBoxItem() { Content = i.ToString() }); }

                combo_dist.Text = Settings.Default.DistanceBetweenSourceAndDestination.ToString();
                Settings.Default.Cols_net_top = 0;
                Settings.Default.Rows_net_top = 0;
                Settings.Default.HorizontalLength = 0;
                Settings.Default.VerticalLength = 0;
            }
            else
            {
                MessageBox.Show("Please select the network");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double AverageSgmentLength
        {
            get
            {
                double sum = 0;

                foreach (RoadNet.Components.RoadSegment road in mainWindow.MyRoadSegments)
                {
                    sum += road.SegmentLength;
                }
                return sum / 4;
            }
        }



        public void RandomDeployVechiles()
        {
            int RsC = mainWindow.MyRoadSegments.Count;

            // How much f
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (isCloseUpbale)
                {
                    Close();
                }
                else
                {
                    e.Cancel = true;
                    Hide();
                }
            }
            catch
            {

            }
        }

        private void Lbl_link_gen_topo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.OpenTopGen();
        }

        private void Combo_dist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selected = (combo_dist.SelectedItem as ComboBoxItem).Content.ToString();
                if (selected == "None")
                {
                    Settings.Default.DistanceImportance = false;
                    Settings.Default.DistanceBetweenSourceAndDestination = 0;
                }
                else
                {
                    Settings.Default.DistanceImportance = true;
                    Settings.Default.DistanceBetweenSourceAndDestination = Convert.ToDouble(selected);
                }
            }
            catch
            {

            }
        }
    }
}
