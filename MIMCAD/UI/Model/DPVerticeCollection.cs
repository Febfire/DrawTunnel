using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using EntityStore.Models;
using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace UI.Model
{
    [TypeConverter(typeof(DPVerticeCollectionConvert))]
    public class DPVerticeCollection : CollectionBase, ICustomTypeDescriptor
    {
        private long _handleValue;
        public void SetByDBVertice(DPVertice vertice)
        {

            this.List.Add(vertice);
            this._handleValue = vertice.getHandleValue();
        }

        public List<DBVertice> GetVertices()
        {
            List<DBVertice> vertices = new List<DBVertice>();
            int i = 0;
            foreach (var v in this.List)
            {
                DPVertice dpv = v as DPVertice;
                DBVertice dbv = new DBVertice(dpv.X, dpv.Y, dpv.Z);
                dbv.NodeHandleValue = dpv.NodeHandleValue;
                dbv.Index = i;
                dbv.NodeHandleValue = dpv.NodeHandleValue;
                vertices.Add(dbv);
                i++;
            }
            return vertices;
        }

        [TypeConverter(typeof(DPVerticeConvert))]
        public DPVertice this[int index]
        {
            get
            {
                DPVertice v = this.List[index] as DPVertice;
                return v;
            }
        }

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of vertices
            for (int i = 0; i < this.List.Count; i++)
            {

                DPVerticeCollectionPropertyDescriptor pd = new DPVerticeCollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection

            return pds;
        }
    }
}
