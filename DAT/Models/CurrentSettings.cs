
using DAT.Models.Enums;

namespace DAT.Models
{
    public class CurrentSettings
    {
        public ColorEnum CurrentColor { get; set; }
        public string CurrentMatchLength { get; set; }
        public SideEnum PrimaryColorSide { get; set; }
    }
}
