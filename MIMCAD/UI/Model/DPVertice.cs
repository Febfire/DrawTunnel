using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using EntityStore.Models;
using LiteDB;
using System;
using System.ComponentModel;
using System.Text;

namespace UI.Model
{
    [TypeConverter(typeof(DPVerticeConvert))]
    public class DPVertice
    {
        private long _handleValue;
        private int _index = -999999999;
        private double _x, _y, _z;
        //private uint _color;
        private long _nodeHandleValue;

        public DPVertice(){}
        public DPVertice(double x,double y,double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void SetFromDBVertice(DBVertice vertice)
        {
            this.X = vertice.X;
            this.Y = vertice.Y;
            this.Z = vertice.Z;
            //this.Color = vertice.Color;
            this.NodeHandleValue = vertice.NodeHandleValue;

            _index = vertice.Index;
            _handleValue = vertice.NodeHandleValue;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.X);
            sb.Append(",");
            sb.Append(this.Y);
            sb.Append(",");
            sb.Append(this.Z);
            return sb.ToString();
        }

        [BrowsableAttribute(false), DefaultValueAttribute(false)]
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;

                modify((vertice) => {
                    if (vertice.Index == _index)
                        return false;
                    else
                    {
                        vertice.Index = _index;
                        return true;
                    }
                });
            }
        }

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;

                modify((vertice) => {
                    if (vertice.X == _x)
                        return false;
                    else
                    {
                        vertice.X = _x;
                        return true;
                    }
                });
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;

                modify((vertice) => {
                    if (vertice.Y == _y)
                        return false;
                    else
                    {
                        vertice.Y = _y;
                        return true;
                    }
                });
            }
        }

        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;

                modify((vertice) => {
                    if (vertice.Z == _z)
                        return false;
                    else
                    {
                        vertice.Z = _z;
                        return true;
                    }                   
                });

            }
        }

        //[BrowsableAttribute(false), DefaultValueAttribute(false)]
        //public uint Color
        //{
        //    get
        //    {
        //        return _color;
        //    }
        //    set
        //    {
        //        _color = value;
        //        modify((vertice) => {
        //            if (vertice.Color == _color)
        //                return false;
        //            else
        //            {
        //                vertice.Color = _color;
        //                return true;
        //            }
        //        });
        //    }
        //}
        [BrowsableAttribute(false), DefaultValueAttribute(false)]
        public long NodeHandleValue
        {
            get
            {
                return _nodeHandleValue;
            }
            set
            {
                _nodeHandleValue = value;
                modify((vertice) => {
                    if (vertice.NodeHandleValue == _nodeHandleValue)
                        return false;
                    else
                    {
                        vertice.NodeHandleValue = _nodeHandleValue;
                        return true;
                    }
                });
            }
        }

        private void modify(Func<DBVertice,bool> handler)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            DBEntity entity = dbControl.FindOne
                    (Query.EQ("HandleValue", this._handleValue));
            if (entity == null) return;

            bool changed = false;
            if (entity.Type == "Tunnel")
            {
                DBTunnel tunnel = entity as DBTunnel;
                changed= handler(tunnel.BasePoints[this._index]);
            }
            else if (entity.Type == "TunnelNode")
            {
                DBNode node = entity as DBNode;
                changed = handler(node.Position);
            }
            if(changed)
            dbControl.Update(entity, Project.Instance.GetActivePropCtl(doc));
        }

        public long getHandleValue()
        {
            return _handleValue;
        }
    }
}
