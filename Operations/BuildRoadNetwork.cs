using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VANET_SIM.RoadNet.Components;
namespace VANET_SIM.Operations
{
    class BuildRoadNetwork
    {
       private MainWindow _MainWindow { get; set; }
        public BuildRoadNetwork(MainWindow _mainWindow)
        {
            _MainWindow = _mainWindow;
        }

        /// <summary>
        /// check if the segment and the junction are overlapped. and check the oreintation of overlapping.
        /// </summary>
        /// <param name="jun"></param>
        /// <param name="rs"></param>
        public static void AreIntersected(Junction jun, RoadSegment rs)
        {
            double offset = -1;
            if (rs.Roadorientation == RoadOrientation.Vertical)
            {
                if (jun.Margin.Top >= rs.Margin.Top)
                {
                    double distance = offset + Computations.Distance(jun.BottomRightCorner, rs.BottomRightCorner);
                    if (distance < jun.Height)
                    {
                        jun.ToNorthRoadSegment = rs;
                        jun.IncicdentRoadSegmentCount += 1;
                        jun.VerticalGreenValue += 1;
                        rs.MyJunctions.Add(jun);
                        // return true;
                    }
                }
                else if (jun.Margin.Top < rs.Margin.Top)
                {
                    double distance = offset + Computations.Distance(jun.TopLeftCorner, rs.TopLeftCorner);
                    if (distance <= jun.Height)
                    {
                        jun.ToSouthRoadSegment = rs;
                        jun.IncicdentRoadSegmentCount += 1;
                        jun.VerticalGreenValue += 1;
                        rs.MyJunctions.Add(jun);
                        //  return true;
                    }
                }
            }
            else if (rs.Roadorientation == RoadOrientation.Horizontal)
            {
                if (jun.Margin.Left < rs.Margin.Left)
                {
                    double distance = offset + Computations.Distance(jun.TopLeftCorner, rs.TopLeftCorner);
                    if (distance <= jun.Width)
                    {
                        jun.ToEastRoadSegment = rs;
                        jun.IncicdentRoadSegmentCount += 1;
                        jun.HorizontalGreenValue += 1;
                        rs.MyJunctions.Add(jun);
                        // return true;
                    }
                }
                else if (jun.Margin.Left > rs.Margin.Left)
                {
                    double distance = offset + Computations.Distance(jun.TopRightCorner, rs.TopRightCorner);
                    if (distance <= jun.Width)
                    {
                        jun.ToWestRoadSegment = rs;
                        jun.IncicdentRoadSegmentCount += 1;
                        jun.HorizontalGreenValue += 1;
                        rs.MyJunctions.Add(jun);
                        // return true;
                    }
                }
            }

            //  return false;
        }
        public void Build()
        {
            foreach (Junction jun in _MainWindow.MyJunctions)
            {
                foreach (RoadSegment rs in _MainWindow.MyRoadSegments)
                {
                    AreIntersected(jun, rs);
                }

                // after finding the rs for each junction.
               // jun.SegSwitch = new SegmentSwitch(jun);
            }
        }

    }
}
