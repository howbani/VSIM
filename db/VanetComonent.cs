using VANET_SIM.RoadNet.Components;

namespace VANET_SIM.db 
{
    public class VanetComonent
    {

        public double Pox { get; set; }
        public double Poy { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ComponentType ComponentType { get; set; }
        public RoadOrientation RoadOrientation { get; set; }
    }
}
