using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace GradeCalculator.Models
{
    [DataContract]
    public class DataPoint
    {

        [DataMember(Name = "label")]
        public string Label { get; set; }


        [DataMember(Name = "y")]
        public double? Y { get; set; }

        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }
    }
}
