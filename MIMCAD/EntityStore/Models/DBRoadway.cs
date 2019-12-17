using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using Autodesk.DefinedEnitity;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace EntityStore.Models
{
    public class DBRoadway : DBCustomEntity
    {
        private string _name;
        private Position _startPoint;
        private Position _endPoint;

        public override void SetProperty(Entity ent)
        {
            this.SetBasicProperty(ent);

            RoadwayWrapper roadway = ent as RoadwayWrapper;

            this.Name = roadway.Name;

            StartPoint = new Position
            {
                X = roadway.StartPoint.X,
                Y = roadway.StartPoint.Y,
                Z = roadway.StartPoint.Z,
            };

            EndPoint = new Position
            {
                X = roadway.EndPoint.X,
                Y = roadway.EndPoint.Y,
                Z = roadway.EndPoint.Z,
            };

        }
        //--------------基本属性----------------//
        [CategoryAttribute("基本属性"), DisplayNameAttribute("类型")]
        public override string Type { get { return "Roadway"; } }
        [CategoryAttribute("基本属性"), DisplayNameAttribute("名字")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (Name == null)
                {
                    _name = value;
                }
                else
                {
                    _name = value;

                    modifiyValue((entity) =>
                    {
                        ((RoadwayWrapper)entity).Name = value;
                    });
                }
            }
        }

        //--------------几何属性----------------//
        //起点坐标
        [CategoryAttribute("几何属性"), DisplayNameAttribute("起点坐标")]
        [ReadOnly(true), TypeConverterAttribute(typeof(Position.Convert))]
        public Position StartPoint
        {
            get
            {
                return _startPoint;
            }
            set
            {
                if (_startPoint == null)
                {
                    _startPoint = value;
                }
                else
                {
                    _startPoint = value;
                    modifiyValue((entity) =>
                    {
                        RoadwayWrapper roadway = (RoadwayWrapper)entity;
                        roadway.StartPoint = new Point3d(value.X, value.Y, value.Z);
                    });
                }
            }
        }

        //终点坐标
        [CategoryAttribute("几何属性"), DisplayNameAttribute("终点坐标")]
        [ReadOnly(true),TypeConverterAttribute(typeof(Position.Convert))]
        public Position EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {

                if (_endPoint == null)
                {
                    _endPoint = value;
                }
                else
                {
                    _endPoint = value;
                    modifiyValue((entity) =>
                    {
                        RoadwayWrapper roadway = (RoadwayWrapper)entity;
                        roadway.EndPoint = new Point3d(value.X, value.Y, value.Z);
                    });
                }
                
            }
        }

        public BindedRoadways StartNode { get; set; }

        public BindedRoadways endNode { get; set; }
    }
}
