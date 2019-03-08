using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VSIM.RoadNet.Components;

namespace VSIM.Operations
{
    public enum VehiclesDistrubution {Random, Systemized, None } 
   public class FairVehiclesDistrubutions
    {
        List<RoadSegment> _rsList;
        double sumLength = 0;
        public FairVehiclesDistrubutions(List<RoadSegment> rsList )
        {
            _rsList = rsList;
            sumLength = SumLength;
        }

        /// <summary>
        ///  divid the veh faily according to
        /// </summary>
        /// <param name="vcount"></param>
        public void Distribute(int vcount, VehiclesDistrubution deplyment) 
        {
            if (deplyment == VehiclesDistrubution.Random)
            {
                foreach (RoadSegment rs in _rsList)
                {
                    int SegmentShare = Convert.ToInt16(rs.SegmentLength / sumLength * Convert.ToDouble(vcount));
                    // add  myShare of vehicles to the rs.
                    for (int i = 1; i <= SegmentShare; i++)
                    {
                        int laneIndex = i % Convert.ToInt16(rs.LanesCount);
                        rs.DeployVehicleRandomStart(laneIndex);
                        Thread.Sleep(1);
                    }
                }
            }
            else if (deplyment == VehiclesDistrubution.Systemized)
            {
                foreach (RoadSegment rs in _rsList)
                {
                    int SegmentShare = Convert.ToInt16(rs.SegmentLength / sumLength * Convert.ToDouble(vcount)); // the number of v in the raod segment.
                    double step = rs.SegmentLength / SegmentShare; // 
                    for (int vIndex = 0; vIndex < SegmentShare; vIndex++)
                    {
                        int laneIndex = vIndex % Convert.ToInt16(rs.LanesCount);
                        double location = (step * vIndex) + PublicParamerters.VehicleWidth;
                        rs.DeployVehicleRandomStart(laneIndex, location);
                    }
                }
            }
        }


        public double SumLength 
        {
            get
            {
                double sum = 0;

                foreach (RoadSegment road in _rsList)
                {
                    sum += road.SegmentLength;
                }
                return sum;
            }
        }



    }
}
