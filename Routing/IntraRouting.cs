using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VSIM.Operations;
using VSIM.Properties;
using VSIM.RoadNet.Components;
using VSIM.UI;

namespace VSIM.Routing
{
    
    public class CandidateVehicle
    {
        public int SVID { get; set; } // SENDER VID 
        public int RVID { get { return SelectedVehicle.VID; } } // RECIVER VID
        public double SignalFadingDistribution { get; set; }
        public double SpeedDifferenceDistribution { get; set; }
        public double BufferSizeDistribution { get; set; }
        public double MovingDirection { get; set; }


        public double HeursticFunction => MovingDirection * (SignalFadingDistribution + SpeedDifferenceDistribution + BufferSizeDistribution);

        public double Priority{ get; set; }

        public double NeighborsAssortment => SignalFadingDistribution * SpeedDifferenceDistribution;



        public VehicleUi SelectedVehicle { get; set; } // CANDIDAT VE
    }

    /// <summary>
    /// intra- roudts the packet within a single segment;
    /// </summary>
   public class IntraRouting
    {
        public string PrintList(List<VehicleUi> NodesList)
        {
            string st = "";
            foreach (VehicleUi v in NodesList)
            {
                st += v.VID + "-";
            }
            return st;
        }
        RoadSegment roadSegment;
        public IntraRouting(RoadSegment MyRoadSegment) => roadSegment = MyRoadSegment;

        /// <summary>
        /// isRouted= true when both the sender and the dest vechiles are in the same segment.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="MyroadSegment"></param>
        /// <returns></returns>
        public CandidateVehicle GetCandidateVehicleUis(VehicleUi i, List<VehicleUi> NodesList, VehicleUi desVehicle,bool isRouted)  
        {
            List<CandidateVehicle> candidates= new List<CandidateVehicle>();
            List<CandidateVehicle> neighbors = new List<CandidateVehicle>();
            double sum = 0;

           // Properties.Settings.Default.IntraRDirectionPar = NodesList.Count / 2;
          //  Properties.Settings.Default.Save();
            if (NodesList.Count>0)
            {
                foreach (VehicleUi j in NodesList)
                {
                    if (isRouted)
                    {
                        // des and sender vehicles both are in the same raod segment.
                        CandidateVehicle jcan = new CandidateVehicle();
                        jcan.SVID = i.VID;
                        jcan.SelectedVehicle = j;
                        jcan.BufferSizeDistribution = BufferSizeDistribution(j.PacketQueue.Count, PublicParamerters.BufferSize);
                        jcan.SignalFadingDistribution = SignalFadingDistribution(Computations.Distance(i.InstanceLocation, j.InstanceLocation), Settings.Default.CommunicationRange);
                        jcan.SpeedDifferenceDistribution = SpeedDifferenceDistribution(i.GetSpeedInKMH, j.GetSpeedInKMH, roadSegment.MaxAllowedSpeed);
                        jcan.MovingDirection = MovingDirection(i.InstanceLocation, j.InstanceLocation, desVehicle.InstanceLocation);

                        sum += jcan.HeursticFunction;
                        neighbors.Add(jcan);
                        if (j == desVehicle)
                        {
                            return jcan;
                        }

                        Console.WriteLine("NeighborsAssortment: " + jcan.NeighborsAssortment.ToString());
                    }
                    else
                    {
                        // not in the same segment:
                        CandidateVehicle jcan = new CandidateVehicle();
                        jcan.SVID = i.VID;
                        jcan.SelectedVehicle = j;
                        jcan.BufferSizeDistribution = BufferSizeDistribution(j.PacketQueue.Count, PublicParamerters.BufferSize);
                        jcan.SignalFadingDistribution = SignalFadingDistribution(Computations.Distance(i.InstanceLocation, j.InstanceLocation), Settings.Default.CommunicationRange);
                        jcan.SpeedDifferenceDistribution = SpeedDifferenceDistribution(i.GetSpeedInKMH, j.GetSpeedInKMH, roadSegment.MaxAllowedSpeed);
                        jcan.MovingDirection = MovingDirection(i.InstanceLocation, j.InstanceLocation, desVehicle.EndJunction.CenterLocation);

                        sum += jcan.HeursticFunction;
                        neighbors.Add(jcan);

                        Console.WriteLine("NeighborsAssortment: " + jcan.NeighborsAssortment.ToString());
                    }
                }

                /*-
                int sen = i.VID;
                int deID = desVehicle.VID;
                string nei = PrintList(NodesList);*/


                if (neighbors.Count > 0)
                {
                    double average = (1 / Convert.ToDouble((neighbors.Count)));
                    double Priority_threshould = average;
                    foreach (CandidateVehicle jcan in neighbors)
                    {
                        double x = jcan.HeursticFunction / sum;
                        jcan.Priority = x;
                        if (jcan.Priority >= Priority_threshould)
                        {
                            candidates.Add(jcan);
                        }
                    }
                    
                    // get max:
                    if (candidates.Count == 0)
                    {
                        return null;
                    }
                    else if (candidates.Count == 1)
                    {
                        return candidates[0];
                    }
                    else
                    {
                        // get max:
                        CandidateVehicle max = candidates[0];
                        if (max != null)
                        {
                            for (int j = 1; j < candidates.Count; j++)
                            {
                                if (candidates[j].Priority > max.Priority)
                                {
                                    max = candidates[j];
                                }
                            }
                            return max;
                        }
                    }
                }
            }
            return null;
        }


        



        /// <summary>
        /// Eq. (A.11) the expected probability of successfully receiving a message at (transDistance) distance while considering an intended communication range of (comRange) meter
        /// </summary>
        /// <param name="transDistance">distance between the sender and the receiver</param>
        /// <param name="comRange">an intended communication range</param>
        /// <returns>double </returns>
        private  double  SignalFadingDistribution(double transDistance, double comRange)
        {
            double pr = 0;
            double ratio_1 = transDistance / comRange;
            double ratio_2 = Math.Pow(ratio_1, 2);
            double ratio_4 = Math.Pow(ratio_1, 4);
            double part1 = Math.Exp(-3 * ratio_2);
            double part2 = 1 + (3 * ratio_2) + (4.5 * ratio_4);
            pr = part1 * part2;
            return pr;
        }

        /// <summary>
        /// The Least speed difference between the sender and the receiver enhances network performance as it keeps the inter-distance between the two vehicles within the transmission range, which in turn allows more time to receive packets and reduces packet loss, especially when the packet size is large. This distribution is designed to capture the similarity of speed so that higher priority is assigned to the vehicle with the least speed difference to the sender vehicle. The speed difference is modeled as a random variable such that  the sender vehicle 〖 n〗_i has speed difference variable S ̃_i={S ̃_(i,0),S ̃_(i,1),…,S ̃_(i,q_i ) } given by the mass function Eq.(28) where s_i denotes the current speed of vehicle 〖 n〗_i.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="sj"></param>
        /// <returns></returns>
        private double SpeedDifferenceDistribution(double si, double sj, double maxS)
        {
            if (sj > Settings.Default.MinSpeed)
            {
                double re = 0;
                double difs = Math.Abs(si - sj);
                double nx = (difs / maxS);
                re = Math.Exp(-Math.Pow(nx, 1));
                return re;
            }
            else return 0;
        }

        /// <summary>
        /// In order to reduce the queuing delay, that is, the time a packet waits in the queue until it can be relayed, this distribution assigns higher priority for vehicle with more available buffer (i.e., fewer packets in the queue). Let m ́ be  the buffer size and let m_i≤ m ́   be the current number of packets in the buffer of vehicle 〖 n〗_i. The sender vehicle defines a random variable M ̃_i={M ̃_(i,0),M ̃_(i,1),…,M ̃_(i,q_i ) } by the mass function Eq.(30).
        /// </summary>
        /// <param name="NumberofPacketsInTheBuffer"></param>
        /// <param name="BufferSize"></param>
        /// <returns></returns>
        private double BufferSizeDistribution(double NumberofPacketsInTheBuffer, double BufferSize)
        {
            double re = 0;
            double ratio = (NumberofPacketsInTheBuffer / BufferSize);
            re = Math.Exp(-Math.Pow(ratio, 1)) / (1 + Math.Pow(ratio, 1));
            return re;
        }

       /// <summary>
       ///  the instance location of the the three vechiles. sender , next hop and the destination.
       /// </summary>
       /// <param name="i"></param>
       /// <param name="j"></param>
       /// <param name="d"></param>
       /// <returns></returns>
        private double MovingDirection(Point i, Point j, Point d) 
        {
            double axb = (j.X - i.X) * (d.X - i.X) + (j.Y - i.Y) * (d.Y - i.Y);
            double disMul = Computations.Distance(i, d) * Computations.Distance(i, j);
            double angale = Math.Acos(axb / disMul);
            double norAngle = angale / Math.PI;
            if (norAngle <= 0.5)
            {
                // smaller angle means closer to the destination.
                double bast = norAngle * Math.Exp(norAngle);
                double mak = 1 + (norAngle * Math.Exp(norAngle));
                double mikdar = bast / mak;
                double re = Math.Pow(1 - mikdar, Settings.Default.IntraVehiForwardDirectionPar); // smaller IntraVehiForwardDirectionPar value means heigher pri for the forward dir

                return re;
            }
            else
            {
                return (Math.Pow(((1 - (norAngle * Math.Exp(norAngle))) / (1 + (norAngle * Math.Exp(norAngle)))), Settings.Default.IntraVehiBackwardDirectionPar));  // heigher IntraVehiBackwardDirectionPar smaller prioiry for the node which not in the direction.
            }
        }



    }
}
