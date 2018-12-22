using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VANET_SIM.Operations;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.db
{
    /// <summary>
    /// Interaction logic for NetwokImport.xaml
    /// </summary>
    public partial class NetwokImport : UserControl
    {
        public MainWindow _MainWindow { set; get; }
        public List<VanetComonent> ImportedComponents = new List<VanetComonent>();

        public UiImportTopology UiImportTopology { get; set; }
        public NetwokImport()
        {
            InitializeComponent();
        }

        private void brn_import_Click(object sender, RoutedEventArgs e)
        {
            ImportedComponents = NetworkTopolgy.ImportNetwok(this);
            PublicParamerters.NetworkName = lbl_network_name.Content.ToString();
            // now add them to feild.
            _MainWindow.tab_vanet.Text = PublicParamerters.NetworkName;
            foreach (VanetComonent vanCom in ImportedComponents)
            {
                if (vanCom.ComponentType == ComponentType.Junction)
                {
                    // junction:
                    Junction jun = new Junction(_MainWindow);
                    jun.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                    jun.Height = vanCom.Height;
                    jun.Width =  vanCom.Width;
                    _MainWindow.canvas_vanet.Children.Add(jun);
                }
                else if (vanCom.ComponentType == ComponentType.RoadSegment)
                {
                    if (vanCom.RoadOrientation == RoadOrientation.Horizontal)
                    {
                        RoadSegment hrs = new RoadSegment(_MainWindow, RoadOrientation.Horizontal);
                        hrs.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                        hrs.Height = vanCom.Height;
                        hrs.Width = vanCom.Width;
                        _MainWindow.canvas_vanet.Children.Add(hrs);
                    }
                    else if ((vanCom.RoadOrientation == RoadOrientation.Vertical))
                    {
                        RoadSegment vrs = new RoadSegment(_MainWindow, RoadOrientation.Vertical);
                        vrs.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                        vrs.Height = vanCom.Height;
                        vrs.Width = vanCom.Width;
                        _MainWindow.canvas_vanet.Children.Add(vrs);
                    }

                }
            }

           
           
            BuildRoadNetwork builder = new BuildRoadNetwork(_MainWindow);
            builder.Build();


           



            try
            {
                UiImportTopology.Close();
            }
            catch
            {

            }
            

           

        }
    }
}
