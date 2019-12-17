using System;
using EntityStore.Models;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using LiteDB;

namespace UI.Model
{
    /// <summary>
    /// 用于属性面板和列表显示中model
    /// </summary>
    public class DPTunnel
    {
        private string _name = "";
        private string _tagData = "";
        private string _location = "";


        DPVerticeCollection _verticeCollection = new DPVerticeCollection();
        bool _displayTag = true;
        public DPTunnel() { }

        public void SetFromDBObject(DBTunnel dbtunnel)
        {
            if (dbtunnel.Type == "T_SquareTunnel")
                this.Type = "方形巷道";
            else if (dbtunnel.Type == "T_CylinderTunnel")
                this.Type = "柱形巷道";

            this.Name = dbtunnel.Name;
            this.HandleValue = dbtunnel.HandleValue;
            this.TagData = dbtunnel.TagData;
            this.Location = dbtunnel.Location;
            this.Segment = dbtunnel.Segment;
            this.Colors = dbtunnel.Colors;
            for (int i = 0; i < dbtunnel.BasePoints.Count; i++)
            {
                DPVertice dp = new DPVertice();
                dp.SetFromDBVertice(dbtunnel.BasePoints[i]);
                _verticeCollection.SetByDBVertice(dp);
            }

            this.DisplayTag = dbtunnel.DisplayTag;

        }
        [Browsable(false), DefaultValue(false)]
        public long HandleValue { get; set; }

        [Browsable(false), DefaultValue(false)]
        public string Type { get; set; }
        
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
                    modify((tunnel) =>
                    {
                        if (tunnel.Name == _name)
                        {
                            return false;
                        }
                        else
                        {
                            tunnel.Name = _name;
                            return true;
                        }
                    });
                }
            }
        }
        [Category("其他"), DisplayName("标注")]
        public string TagData
        {
            get
            {
                return _tagData;
            }
            set
            {
                if (TagData == "")
                {
                    _tagData = value;
                }
                else
                {
                    _tagData = value;
                    modify((tunnel) =>
                    {
                        if (tunnel.TagData == _tagData)
                        {
                            return false;
                        }
                        else
                        {
                            tunnel.TagData = _tagData;
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
                    modify((tunnel) =>
                    {
                        if (tunnel.Location == _location)
                        {
                            return false;
                        }
                        else
                        {
                            tunnel.Location = _location;
                            return true;
                        }
                    });
                }
            }
        }

        [CategoryAttribute("几何图形"), DisplayNameAttribute("坐标集合")]
        [TypeConverter(typeof(DPVerticeCollectionConvert))]
        public DPVerticeCollection VerticeCollection
        {
            get
            {
                List<DBVertice> vertices = _verticeCollection.GetVertices();
                modify((tunnel) =>
                {
                    bool changed = false;

                    if (tunnel.BasePoints.Count != vertices.Count)
                    {
                        tunnel.BasePoints = vertices;
                        changed = true;
                    }
                    else
                    {
                        int i = 0;
                        foreach (var v in vertices)
                        {
                            if (v != tunnel.BasePoints[i])
                            {
                                tunnel.BasePoints = vertices;
                                changed = true;
                                break;
                            }
                            i++;
                        }
                    }

                    return changed;
                });

                return _verticeCollection;
            }
        }
        [CategoryAttribute("其他"), DisplayNameAttribute("显示标注")]
        public bool DisplayTag
        {
            get
            {

                return _displayTag;
            }
            set
            {
                _displayTag = value;
                modify((tunnel) =>
                {
                    if (tunnel.DisplayTag == _displayTag)
                    {
                        return false;
                    }
                    else
                    {
                        tunnel.DisplayTag = _displayTag;
                        return true;
                    }
                });

            }
        }
        [Browsable(false), DefaultValue(false)]
        public int Segment { get; set; }
        [Browsable(false), DefaultValue(false)]
        public List<uint> Colors { get; set; }

        //界面数据修改同步修改litedb
        private void modify(Func<DBTunnel, bool> handler)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);


            DBEntity entity = dbControl.FindOne
                    (Query.EQ("HandleValue", this.HandleValue));
            if (entity == null) return;

            bool changed = false;
            if (entity is DBTunnel)
            {
                DBTunnel tunnel = entity as DBTunnel;
                changed = handler(tunnel);

                if (changed)
                    dbControl.Update(entity, Project.Instance.GetActivePropCtl(doc));
            }        
        }
    }

}
