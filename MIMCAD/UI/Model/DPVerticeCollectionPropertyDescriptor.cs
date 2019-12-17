using System;
using System.ComponentModel;
using System.Text;

namespace UI.Model
{
    public class DPVerticeCollectionPropertyDescriptor : PropertyDescriptor
    {
        private DPVerticeCollection collection = null;
        private int index = -1;

        public DPVerticeCollectionPropertyDescriptor(DPVerticeCollection coll, int idx) : 
			base( "#"+idx.ToString(), null )
		{
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return this.collection.GetType();
            }
        }

        public override string DisplayName
        {
            get
            {
                DPVertice vertice = this.collection[index];
                return "坐标:"+index.ToString();
            }
        }

        public override string Description
        {
            get
            {
                DPVertice vertice = this.collection[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(vertice.X);
                sb.Append(",");
                sb.Append(vertice.Y);
                sb.Append(",");
                sb.Append(vertice.Z);
                return sb.ToString();
            }
        }

        public override object GetValue(object component)
        {
            return collection[index];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "#" + index.ToString(); }
        }

        public override Type PropertyType
        {
             get { return this.collection[index].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            //this.collection[index] = value as DPVertice;
        }

        public override object GetEditor(Type editorBaseType)
        {
            return base.GetEditor(editorBaseType);
        }
    }
}
