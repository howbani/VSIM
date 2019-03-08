using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VSIM.RoadNet.Components;

namespace VSIM.Operations
{
    public class NetworkGenrator
    {
        /// <summary>
        ///  build a grid of road segments
        /// </summary>
        /// <param name="rows"> junctions i n rows </param>
        /// <param name="cols">junctions in  cols </param>
        /// <param name="hLen">Horizontal length</param>
        /// <param name="vLen">Vertical length</param>
        /// <param name="laneCount"> number of lanes</param>
        public static void ConstructGird(MainWindow mainWindow, int rows, int cols, double hLen, double vLen, int laneCount)
        {
            /// the length of junction should be custimizeble, according to the number of lanes.
            double JunHw = 1.6 * (laneCount * PublicParamerters.LaneWidth); // the width- hgigh or jubction.
            double startLocationJunction = 20;
            for (int Rindex = 0; Rindex < rows; Rindex++)
            {
                for (int Cindex = 0; Cindex < cols; Cindex++)
                {
                    double JunX = startLocationJunction + (Cindex * hLen);
                    double junY = startLocationJunction + (Rindex * vLen);
                    Junction jun = new Junction(mainWindow);
                    jun.Margin = new Thickness(JunX, junY, 0, 0);
                    jun.Height = JunHw;
                    jun.Width = JunHw;
                    mainWindow.canvas_vanet.Children.Add(jun);

                    // add vertical and horizontal
                    if (Cindex < cols - 1 && Rindex < rows - 1)
                    {
                        // add the horizontal First:
                        RoadSegment hrs = new RoadSegment(mainWindow, laneCount, RoadOrientation.Horizontal);
                        hrs.Height = (laneCount * PublicParamerters.LaneWidth) + 1.5;
                        hrs.Width = hLen - (jun.Width / 2);
                        hrs.Margin = new Thickness(jun.RightCenter.X - (jun.Width / 4), (jun.RightCenter.Y) - (hrs.Height / 2), 0, 0);
                        mainWindow.canvas_vanet.Children.Add(hrs);
                        // add the vertical:
                        RoadSegment vrs = new RoadSegment(mainWindow, laneCount, RoadOrientation.Vertical);
                        vrs.Height = vLen - (jun.Height / 2); ;
                        vrs.Width = (laneCount * PublicParamerters.LaneWidth) + 1.5;
                        vrs.Margin = new Thickness(jun.BottomCenter.X - (vrs.Width / 2), jun.BottomCenter.Y - (jun.Height / 4), 0, 0);
                        mainWindow.canvas_vanet.Children.Add(vrs);
                    }
                    else if (Rindex == rows - 1 && Cindex < cols - 1)
                    {
                        // add the horizontal:
                        RoadSegment hrs = new RoadSegment(mainWindow, laneCount, RoadOrientation.Horizontal);
                        hrs.Height = (laneCount * PublicParamerters.LaneWidth) + 1.5;
                        hrs.Width = hLen- (jun.Width / 2); 
                        hrs.Margin = new Thickness(jun.RightCenter.X - (jun.Width / 4), (jun.RightCenter.Y) - (hrs.Height / 2), 0, 0);
                        mainWindow.canvas_vanet.Children.Add(hrs);
                    }
                    else if (Cindex == cols - 1 && Rindex < rows - 1)
                    {
                        // add the vertical:
                        RoadSegment vrs = new RoadSegment(mainWindow, laneCount, RoadOrientation.Vertical);
                        vrs.Height = vLen - (jun.Height / 2); 
                        vrs.Width = (laneCount * PublicParamerters.LaneWidth) + 1.5;
                        vrs.Margin = new Thickness(jun.BottomCenter.X - (vrs.Width / 2), jun.BottomCenter.Y - (jun.Height / 4), 0, 0);
                        mainWindow.canvas_vanet.Children.Add(vrs);
                    }
                    
                }
            }

            // build:
            BuildRoadNetwork builder = new BuildRoadNetwork(mainWindow);
            builder.Build();
        }
    }
}
