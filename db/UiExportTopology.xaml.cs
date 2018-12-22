using System.Collections.Generic;
using System.Windows;
using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.db
{
    /// <summary>
    /// Interaction logic for UiExportTopology.xaml
    /// </summary>
    public partial class UiExportTopology : Window
    {
        MainWindow mianwindow;
        public UiExportTopology(MainWindow _mianwindow) 
        {
            InitializeComponent();
            mianwindow = _mianwindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NetworkTopolgy topolog = new NetworkTopolgy();
            bool isCreated = topolog.createNewTopology(txt_networkName.Text);
            if (isCreated)
            {
                this.WindowState = WindowState.Minimized;

                foreach (Junction jun in mianwindow.MyJunctions)
                {
                    VanetComonent com = new VanetComonent();
                    com.Pox = jun.Margin.Left;
                    com.Poy = jun.Margin.Top;
                    com.Width = jun.Width;
                    com.Height = jun.Height;
                    com.ComponentType = ComponentType.Junction;

                    topolog.SaveVanetComponent(com, txt_networkName.Text);
                }



                foreach (RoadSegment seg in mianwindow.MyRoadSegments)
                {

                    VanetComonent com = new VanetComonent();
                    com.Pox = seg.Margin.Left;
                    com.Poy = seg.Margin.Top;
                    com.Width = seg.Width;
                    com.Height = seg.Height;
                    com.ComponentType = ComponentType.RoadSegment;
                    com.RoadOrientation = seg.Roadorientation;

                    topolog.SaveVanetComponent(com, txt_networkName.Text);
                }





                this.Close();
            }
            else
            {
                MessageBox.Show("please change network name!");
            }
        }
    }
}
