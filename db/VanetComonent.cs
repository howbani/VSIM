using VSIM.RoadNet.Components;

namespace VSIM.db 
{
    public class VanetComonent
    {

        public double Pox { get; set; }
        public double Poy { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ComponentType ComponentType { get; set; }
        public RoadOrientation RoadOrientation { get; set; }
        public int LanesCount { get; set; }
    }
}
