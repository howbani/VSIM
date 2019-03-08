using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VSIM.Operations;
using VSIM.Properties;
using VSIM.RoadNet.Components;

namespace VSIM.Routing
{
    public class CandidateJunction
    {
        public int SJID => CurrentJunction.JID; 
        public int RJID => NextJunction.JID;
        public double Perpendiculardistance { get; set; }
        public double AngleDotProdection { get; set; }
        public double Length { get; set; }
        public double ShortestDistance => AngleDotProdection * (AngleDotProdection + Length);
        public double Connectivity { get; set; } 
        public double HeuristicFunction
        {  //heuristic 
            get
            {
                double wiCon = Properties.Settings.Default.WeightConnectivity;
                double wdis = Properties.Settings.Default.WeightShortestDistance;
                return (wdis * ShortestDistance) + (wiCon * Connectivity);
            }
        }
        public double Priority { get; set; }

        public int NumberXfVechiles
        {
            get
            {
                if (NextRoadSegment != null) { return NextRoadSegment.VehiclesCount; } else return -1;
            }
        }
        public int RoadSegmentID
        {
            get
            {
                if (NextRoadSegment != null) { return NextRoadSegment.RID; } else return -1;
            }
        }


        public Junction CurrentJunction { get; set; }// Current sender
        public Junction DestinationJunction { get; set; } // destiantion
        public Junction NextJunction { get; set; } // the candidate junction
        public RoadSegment NextRoadSegment { get; set; } // the candidate segment.
    }

    /// <summary>
    /// Find the the road segments. between two junctions or two segments..
    /// </summary>
    class InterRouting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartJun"></param>
        /// <param name="EndJun"></param>
        /// <returns></returns>
        public RoadSegment GetRoadSegment(Junction StartJun, Junction EndJun)
        {
            RoadSegment re = null;
            foreach (RoadSegment rs in StartJun.AdjacentRoadSegment)
            {
                if (rs != null)
                {
                    Junction t1 = rs.MyJunctions[0];
                    Junction t2 = rs.MyJunctions[1];
                    if (t1.JID == EndJun.JID || t2.JID == EndJun.JID)
                    {
                        re = rs;
                        break;
                    }
                }
            }
            return re;
        }

        /// <summary>
        /// Get the candidates of the i.
        /// get the candidate for the jucntion i.
        /// An Inter-path is a sequence of road intersections that connect the source to the destination, satisfying two key requirements, shorter routing distances, and higher connectivity. To meet these two requirements, we developed a heuristic function with two probability distributions, the connectivity distribution (denoted by ξ ̃_(i,j)) and the shortest distance distribution (denoted by〖 Φ ̃〗_(i,j)).
        /// </summary>
        /// <param name="_i"></param>
        /// <returns></returns>
        public CandidateJunction CandidateJunction(Junction _i, Junction _des)
        {

            List<CandidateJunction> candidateJunctions = new List<Routing.CandidateJunction>();
            ShortestDistanceSelector computer = new ShortestDistanceSelector();
            SegmentConnectivitySelector connect = new SegmentConnectivitySelector();
            // values:
            foreach (Junction _j in _i.Adjacentjunctions)
            {
                RoadSegment roadSegment = GetRoadSegment(_i, _j);
                if (roadSegment != null)
                {
                    CandidateJunction can = new Routing.CandidateJunction();
                    can.CurrentJunction = _i;
                    can.NextJunction = _j;
                    can.NextJunction = _j; // CAN id.
                    can.DestinationJunction = _des;
                    can.NextRoadSegment = roadSegment;

                    // values:
                    //1- shortes distance:
                    can.Perpendiculardistance = computer.Perpendiculardistance(_j.CenterLocation, _i.CenterLocation, _des.CenterLocation);

                    if (_des.JID == _j.JID)
                        can.AngleDotProdection = 1;
                    else
                        computer.AngleDotProdection(_i.CenterLocation, _j.CenterLocation, _des.CenterLocation);

                    can.Length = computer.Length(_i.CenterLocation, _j.CenterLocation);
                    //-2 connectivity.
                    can.Connectivity = connect.SegmentConnectivity(roadSegment.SegmentLength, roadSegment.VehiclesCount, roadSegment.VehicleInterArrivalMean, Settings.Default.CommunicationRange, roadSegment.LanesCount);


                    candidateJunctions.Add(can);
                }
            }

            // get the max priority.
            if (candidateJunctions.Count > 0)
            {
                CandidateJunction max = candidateJunctions[0];
                for (int j = 1; j < candidateJunctions.Count; j++)
                {
                    if (candidateJunctions[j].HeuristicFunction > max.HeuristicFunction)
                    {
                        max = candidateJunctions[j];
                    }
                }

                return max;
            }

            return null;
        }

    }
    /// <summary>
    /// The shortest distance distribution Φ ̃_(i,j) is obtained by combining three probability mass functions, formularized in Eq. (15), the direction angle θ_(i,j), the perpendicular distance ψ ̃_(i,j) and the length of the road segment. The direction distribution assigns higher priority for junctions toward the destination while the perpendicular distance distribution assigns higher priority for junctions that closer to centerline between the source and the destination (see Fig. 2).
    /// </summary>
    public class ShortestDistanceSelector
    {
        /// <summary>
        /// Perpendicular distance distribution ψ ̃_(i,j) allocates higher probability for the junctions that are closer to the central line  l_(s,b)(i.e., a virtual line linking the source junction v_s and the destination v_b). See Fig.2. The perpendicular distance ψ_j from the node 〖 v〗_j to the line l_(s,b) is defined by Eq. (18).  For the source junction〖 v〗_i, we define the normalized perpendicular-distance random variable ψ ̅_i=(ψ ̅_(i,1),ψ ̅_(i,3),…ψ ̅_(i,b_i ) )  by Eq. (19). Furthermore, we define the perpendicular-distance probability distribution, denoted by ψ ̃_i=(ψ ̃_(i,1),ψ ̃_(i,2)…ψ ̃_(i,b_i ) ), by Eq. (20).
        /// </summary>
        /// <param name="j"></param>
        /// <param name="s"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Perpendiculardistance(Point pj, Point psource, Point pdistanction)
        {
            double past = Math.Abs(((pdistanction.Y - psource.Y) * pj.X) - ((pdistanction.X - psource.X) * pj.Y) + (pdistanction.X * psource.Y) - (pdistanction.Y * psource.X));
            double sbDis = Computations.Distance(psource, pdistanction);
            double perDis = past / sbDis;

            // dist: if there is a mistake, then we should consider the normalization.
            double pr = Math.Exp(-perDis);
            return pr;
        }
        /// <summary>
        /// Direction Angle: The direction angle θ_(i,j)between the junction v_i  and the potential forwarder v_j towards the destination junction v_b  is modeled as a dot production of two vectors a ⃗.c ⃗, such that a ⃗=(x_j-x_i,y_j-y_i ) and  c ⃗=(x_b-x_i,y_b-y_i )  and normalized to π  by Eq. (16). The normalized-direction θ ̅_i=(θ ̅_(i,1),θ ̅_(i,2),…θ ̅_(i,b_i )) are injected to the mass function Eq. (17) to obtain the random variable (θ_i ) ̃=(θ ̃_(i,1),θ ̃_(i,2)…θ ̃_(i,b_i ) ). Direction distribution assigns higher probability for the normalized angles 0≤θ ̅_(i,j)≤1/2 that ensure higher routing progress.
        /// </summary>
        /// <param name="isender"></param>
        /// <param name="jcandidate"></param>
        /// <param name="bDestination"></param>
        /// <returns></returns>

        public double AngleDotProdection(Point i, Point j, Point d)
        {
            double axb = (j.X - i.X) * (d.X - i.X) + (j.Y - i.Y) * (d.Y - i.Y);
            double disMul = Computations.Distance(i, d) * Computations.Distance(i, j);
            double angale = Math.Acos(axb / disMul);
            double norAngle = angale / Math.PI;
            if (norAngle <= 0.5)
                return (Math.Pow(((1 - (norAngle * Math.Exp(norAngle))) / (1 + (norAngle * Math.Exp(norAngle)))), 1)); // heigher pri
            else
                return (Math.Pow(((1 - (norAngle * Math.Exp(norAngle))) / (1 + (norAngle * Math.Exp(norAngle)))), 3)); // smaller pri
        }

        /// <summary>
        /// Road Segment Length: This distribution allocates higher probability to the longer segment. Longer segments are more preferable to avoid frequent segment switching that leads to delay the packet.  For the source junction〖 v〗_i, we define the normalized segment length random variable L ̅_i=(L ̅_(i,1),L ̅_(i,3),…L ̅_(i,b_i ) )  by Eq. (21). Furthermore, we define the segment length probability distribution, denoted by L ̃_i=(L ̃_(i,1),L ̃_(i,2)…L ̃_(i,b_i ) ), by the mass function as formulated in Eq. (22).
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double Length(Point i,Point j) 
        {
            double len= Computations.Distance(i, j);
            return (1 - Math.Sqrt(Math.Exp(-len)));
        }

        


    }
     

   
    /// <summary>
    /// Assume that each vehicle is equipped with OBU contains communication unit with transmission range denoted by δ_cr. Vehicle〖 n〗_i is able to communicate with a subsequent vehicle n_j and relay the packets if the inter-distance of 〖 n〗_i and 〖 n〗_j does not exceed δ_cr[22].   As in Eq. (9), the number of vehicles within the communication range of vehicle n_i is given by Eq. (23) where ρ_ij denotes the vehicle density of road segment〖 r〗_ij∈E_G.
    /// </summary>
    public class SegmentConnectivitySelector
    {

        private double myRoadSegmentLength;
        private double myVechilesCountInTheSegment;
        private double myVehicleInterArrivalMean;
        private double myComunicationRange;
        private double numberoflanes;
        

        /// <summary>
        ///  my new one:
        /// </summary>
        /// <param name="_RoadSegmentLength"></param>
        /// <param name="_VechilesCountInTheSegment"></param>
        /// <param name="_myVehicleInterArrivalMean"></param>
        /// <param name="_myComunicationRange"></param>
        /// <param name="_numberoflanes"></param>
        /// <returns></returns>
        public double SegmentConnectivity(double _RoadSegmentLength, double _VechilesCountInTheSegment, double _myVehicleInterArrivalMean, double _myComunicationRange, double _numberoflanes)
        {
            // set parmaters: 
            myRoadSegmentLength = _RoadSegmentLength;
            myVechilesCountInTheSegment = _VechilesCountInTheSegment;
            myComunicationRange = _myComunicationRange;
            myVehicleInterArrivalMean = _myVehicleInterArrivalMean;
            numberoflanes = _numberoflanes;
            double pr = 0;
            double exponent = _RoadSegmentLength / (_myComunicationRange / 2); // dived the _RoadSegmentLength in to myComunicationRange / 2
            double density = (numberoflanes * myVechilesCountInTheSegment * myComunicationRange) / myRoadSegmentLength;
            pr = Math.Pow(1 - Math.Exp(-density), exponent);
            return pr;
        }
    }
}
