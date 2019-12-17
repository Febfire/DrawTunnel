using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.ComponentModel;

namespace EntityStore.Models
{
   
    public abstract class DBEntity
    {

        [BrowsableAttribute(false), DefaultValueAttribute(false)]
        public int _id { get; set; }

        [BrowsableAttribute(false), DefaultValueAttribute(false)]
        public long HandleValue { get; set; }

        [CategoryAttribute("基本属性"), DisplayNameAttribute("类型")]
        public virtual string Type { get; protected set; }

        public string Location { get; set; }

        public virtual void SetProperty(Entity ent)
        {
            HandleValue = ent.Handle.Value;
        }     

    }


}
