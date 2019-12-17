using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using EntityStore.Controller;
using EntityStore.Models;
using LiteDB;
using System;
using System.ComponentModel;
using System.Drawing;

namespace UI.Model
{
    public class DPNode
    {
        private string _name = "";
        private DPVertice _position;
        private string _location = "";
        private Color _color = new Color();

        public DPNode() { }

        public void SetFromDBEntity(DBNode dbNode)
        {
            this.Name = dbNode.Name;
            this.HandleValue = dbNode.HandleValue;
            this.Location = dbNode.Location;
            this.Position = new DPVertice();
            Position.SetFromDBVertice(dbNode.Position);

            uint argb = dbNode.NodeColor & 0xffffffff;
            uint r = (dbNode.NodeColor & 0x00ff0000) >> 16;
            uint g = (dbNode.NodeColor & 0x0000ff00) >> 8;
            uint b = (dbNode.NodeColor & 0x000000ff);
            this.NodeColor = Utils.uint2Color(dbNode.NodeColor);
        }



        [Browsable(false), DefaultValue(false)]
        public long HandleValue { get; set; }

        [Category("基本属性"), DisplayName("名字")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (Name == "")
                {
                    _name = value;
                }
                else
                {
                    _name = value;
                    modify((node) =>
                    {
                        if (node.Name == _name)
                        {
                            return false;
                        }
                        else
                        {
                            node.Name = _name;
                            return true;
                        }
                    });
                }
            }
        }

        [Category("基本属性"), DisplayName("区域")]
        [ReadOnly(true)]
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (Location == "")
                {
                    _location = value;
                }
                else
                {
                    _location = value;
                    modify((node) =>
                    {
                        if (node.Location == _location)
                        {
                            return false;
                        }
                        else
                        {
                            node.Location = _location;
                            return true;
                        }
                    });
                }
            }
        }

        [CategoryAttribute("几何"), DisplayNameAttribute("坐标")]
        public DPVertice Position
        {
            get
            { 
                return _position;
            }
            set
            {
                if (value == null) return;
                if (_position == null)
                {
                    _position = value;
                }

                else
                {
                    _position = value;
                    modify((node) =>
                    {
                        var oldPosition = node.Position;
                        var newPosition = new DBVertice(_position.X, _position.Y, _position.Z);

                        if (newPosition == oldPosition)
                        {
                            return false;
                        }
                        else
                        {
                            node.Position = newPosition;
                            return true;
                        }
                    });
                }
               
            }
        }

        [CategoryAttribute("颜色"), DisplayNameAttribute("颜色")]
        public Color NodeColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                modify((node) =>
                {
                    var color = Utils.color2uint(_color); 
                    if (node.NodeColor == color)
                    {
                        return false;
                    }
                    else
                    {
                        node.NodeColor = color;
                        return true;
                    }
                    
                });
            }
        }

        private void modify(Func<DBNode,bool> handler)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            DBEntity entity = dbControl.FindOne
                    (Query.EQ("HandleValue", this.HandleValue));

            bool changed = false;
            if (entity is DBNode)
            {
                DBNode node = entity as DBNode;

                changed = handler(node);
                if (changed)
                {
                    dbControl.Update(entity, Project.Instance.GetActivePropCtl(doc));
                }
            }            
        }
    }
}
