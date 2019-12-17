using Autodesk.DefinedEnitity;
using System;
using System.ComponentModel;
using System.Globalization;

namespace EntityStore.Models
{

    public class Position
    {
        public class Convert : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
            {
                if (destinationType == typeof(Position))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
            {
                if (destinationType == typeof(System.String) &&
                     value is Position)
                {

                    Position so = (Position)value;

                    return "( " + so.X +
                           ", : " + so.Y +
                           ", " + so.Z + ")";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        private double _x, _y, _z;
        [ReadOnly(true)]
        public double X { get { return Math.Round(_x, 4); } set { _x = value; } }
        [ReadOnly(true)]
        public double Y { get { return Math.Round(_y, 4); } set { _y = value; } }
        [ReadOnly(true)]
        public double Z { get { return Math.Round(_z, 4); } set { _z = value; } }

        public override string ToString()
        {
            return String.Format("(\n{0} \n{1} \n{2})", this.X, this.Y, this.Z);
        }

    }

    public struct BindedRoadways
    {
        public RoadwayWrapper roadway;
        public int whichSide;
    }
}
