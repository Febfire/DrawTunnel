using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.DefinedEnitity;
using System;
using System.Collections.Generic;
using EntityStore.Models;

namespace UI.Create
{

    class RoadwayNodeStatic
    {
        private Vector3d _TransformVector;
        private List<BindedRoadways> _BindedRoadways;
        private RoadwayNodeWrapper _Entity;

        public RoadwayNodeStatic(BindedRoadways bindedRoadway)
        {
            _BindedRoadways = new List<BindedRoadways>();
            _BindedRoadways.Add(bindedRoadway);

            if (bindedRoadway.whichSide == -1)
            {
                _TransformVector = bindedRoadway.roadway.EndPoint - new Point3d();
            }
            else if (bindedRoadway.whichSide == 1)
            {
                _TransformVector = bindedRoadway.roadway.StartPoint - new Point3d();
            }
            InitRoadwayNode();
        }

        public RoadwayNodeWrapper GetEntity()
        {
            return _Entity;
        }
        private void InitRoadwayNode()
        {
            if (_Entity == null)
            {
                _Entity = new RoadwayNodeWrapper();
                _Entity.CreateSphere(15);

                Matrix3d mt = Matrix3d.Displacement(_TransformVector);
                _Entity.TransformBy(mt);
            }
        }

        public void modified(object sender, EventArgs e)
        {
            Extents3d extents3d = (Extents3d)_Entity.Bounds;
            Point3d newPosition = new Point3d(
                (extents3d.MaxPoint.X + extents3d.MinPoint.X) / 2,
                (extents3d.MaxPoint.Y + extents3d.MinPoint.Y) / 2,
                (extents3d.MaxPoint.Z + extents3d.MinPoint.Z) / 2);


            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;


            if (tm.TopTransaction == null)
                tm.StartTransaction();

            foreach (var v in _BindedRoadways)
            {
                RoadwayWrapper bdrw = (RoadwayWrapper)tm.GetObject(v.roadway.ObjectId, OpenMode.ForWrite, false);
                if (v.whichSide == -1)
                {
                    bdrw.EndPoint = newPosition;
                }
                else if (v.whichSide == 1)
                {
                    bdrw.StartPoint = newPosition;
                }
            }
        }
    }
}
