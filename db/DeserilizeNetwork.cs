using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VSIM.Operations;
using VSIM.RoadNet.Components;

namespace VSIM.db
{
    public class   DeserilizeNetwork
    {
        public static void DesrlizeNetwork(MainWindow _MainWindow, string NetName)
        {

            // clear:
            _MainWindow.Clear();

            // now add them to feild.
            PublicParamerters.NetworkName = NetName;
            List<VanetComonent> ImportedComponents = NetworkTopolgy.ImportNetwok(NetName); // get comp
            _MainWindow.tab_vanet.Text = PublicParamerters.NetworkName;

            

            foreach (VanetComonent vanCom in ImportedComponents)
            {
                if (vanCom.ComponentType == ComponentType.Junction)
                {
                    // junction:
                    Junction jun = new Junction(_MainWindow);
                    jun.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                    jun.Height = vanCom.Height;
                    jun.Width = vanCom.Width;
                    _MainWindow.canvas_vanet.Children.Add(jun);
                }
                else if (vanCom.ComponentType == ComponentType.RoadSegment)
                {
                    if (vanCom.RoadOrientation == RoadOrientation.Horizontal)
                    {
                        RoadSegment hrs = new RoadSegment(_MainWindow, vanCom.LanesCount, RoadOrientation.Horizontal);
                        hrs.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                        hrs.Height = vanCom.Height;
                        hrs.Width = vanCom.Width;
                        _MainWindow.canvas_vanet.Children.Add(hrs);
                    }
                    else if ((vanCom.RoadOrientation == RoadOrientation.Vertical))
                    {
                        RoadSegment vrs = new RoadSegment(_MainWindow, vanCom.LanesCount, RoadOrientation.Vertical);
                        vrs.Margin = new Thickness(vanCom.Pox, vanCom.Poy, 0, 0);
                        vrs.Height = vanCom.Height;
                        vrs.Width = vanCom.Width;
                        _MainWindow.canvas_vanet.Children.Add(vrs);
                    }

                }

                // get max X
            }


           

            BuildRoadNetwork builder = new BuildRoadNetwork(_MainWindow);
            builder.Build();

            double maxX = 0;
            double MaxY = 0;
            for (int j = 0; j < _MainWindow.MyJunctions.Count; j++)
            {
                Junction jun = _MainWindow.MyJunctions[j];
                if (jun.CenterLocation.X > maxX) { maxX = jun.CenterLocation.X; }
                if (jun.CenterLocation.Y > MaxY) { MaxY = jun.CenterLocation.Y; }
            }

            _MainWindow.canvas_vanet.Height = 200 + MaxY;
            _MainWindow.canvas_vanet.Width = 200 + maxX;

        }
    }
}
