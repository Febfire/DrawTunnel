using System;
using System.ComponentModel;
using System.Globalization;

namespace UI.Model
{
    public class DPVerticeConvert : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string) &&
                 value is DPVertice)
            {

                DPVertice so = (DPVertice)value;

                return so.X + ", " + so.Y + ", " + so.Z;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = value as string;
                string[] strVertice = str.Split(',');

                if(strVertice.Length !=3)
                    return null;
                double x, y, z;
                try
                {
                     x = Convert.ToDouble(strVertice[0]);
                     y = Convert.ToDouble(strVertice[1]);
                     z = Convert.ToDouble(strVertice[2]);
                    if (Math.Abs(x) > 1 << 30 || Math.Abs(y) > 1 << 30 || Math.Abs(z) > 1 << 30)
                    {
                        return null;
                    }
                }
                catch (System.FormatException)
                {
                    return null;
                }
                
                return new DPVertice(x,y,z);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

    }
    internal class DPVerticeCollectionConvert : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DPVerticeCollection)
            {
                DPVerticeCollection col = value as DPVerticeCollection;
                int count = col.Count;
                return String.Format("个数:{0}", count);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
    }
}
