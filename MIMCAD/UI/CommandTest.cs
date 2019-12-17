using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using MIM;
using System.Collections.Generic;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Colors;
using EntityStore.Models;
using UI.Draw;
using ColorBar;
[assembly: CommandClass(typeof(UI.CommandTest))]

namespace UI
{
    static class CommandTest
    {
        [CommandMethod("closeProject",CommandFlags.Session)]
        static public void closeProject()
        {
            Global.CloseProject();
        }


        [CommandMethod("beingSel")]
        static public void beingSel()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptSelectionResult res = ed.SelectAll();

            SelectionSet SS = res.Value;
            if (SS == null)
                return;

            ed.SetImpliedSelection(SS);
        }


        [CommandMethod("nodesvisible")]
        static public void nodesvisible()
        {
            Utils.SelectOne("选择巷道", (ent) => {
                if (ent is BaseTunnel)
                {
                    BaseTunnel tunnel = ent as BaseTunnel;
                    bool visible = tunnel.DisplayNodes;
                    tunnel.DisplayNodes = !visible;
                }

            });
        }


        [CommandMethod("openhtml")]
        static public void openhtml()
        {
            
            Uri uri = new Uri(Project.Instance.RootPath + @"\JsonViewer\index.html");
            Application.ShowModelessWindow(uri);
        }
        

        [CommandMethod("tmp2color")]
        static public void tmp2color()
        {
            ColorManager cm = new ColorManager();
            int[,] cMap = cm.CMap;
            var color = System.Drawing.Color.FromArgb(cMap[10, 0], cMap[10, 1], cMap[10, 2], cMap[10, 3]);

        }

        //相交判断测试
        [CommandMethod("GetIntersect")]
        static public void GetIntersect()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions options = new PromptEntityOptions("选择物体");
            PromptEntityResult res1 = ed.GetEntity(options);
            PromptEntityResult res2 = ed.GetEntity(options);

            ObjectId id1 = res1.ObjectId;
            ObjectId id2 = res2.ObjectId;

            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            Utils.TransactionControl(() =>
            {
                Entity ent1 = (Entity)tm.GetObject(id2, OpenMode.ForWrite, false);
                Entity ent2 = (Entity)tm.GetObject(id1, OpenMode.ForWrite, false);

                Point3dCollection pc = new Point3dCollection();

                ent1.IntersectWith(ent2, Intersect.OnBothOperands, pc, IntPtr.Zero, IntPtr.Zero);
            });

        }

        //相机移动测试
        [CommandMethod("cameramove")]
        static public void cameramove()
        {
            Point3d pMin = new Point3d(-1000, -1000, 0);
            Point3d pMax = new Point3d(1000, 1000, 0);
            Point3d pCenter = new Point3d(0, 0, 0);
            double dFactor = 1;
            Zoom(pMin, pMax, pCenter, dFactor);
        }
        static void Zoom(Point3d pMin, Point3d pMax, Point3d pCenter, double dFactor)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            // Get the extents of the current space when no points 
            // or only a center point is provided
            // Check to see if Model space is current
            if (acCurDb.TileMode == true)
            {
                if (pMin.Equals(new Point3d()) == true &&
                    pMax.Equals(new Point3d()) == true)
                {
                    pMin = acCurDb.Extmin;
                    pMax = acCurDb.Extmax;
                }
            }
            else
            {
                // Check to see if Paper space is current
                if (nCurVport == 1)
                {
                    // Get the extents of Paper space
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = acCurDb.Pextmin;
                        pMax = acCurDb.Pextmax;
                    }
                }
                else
                {
                    // Get the extents of Model space
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = acCurDb.Extmin;
                        pMax = acCurDb.Extmax;
                    }
                }
            }

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Get the current view
                using (ViewTableRecord acView = acDoc.Editor.GetCurrentView())
                {
                    Extents3d eExtents;

                    // Translate WCS coordinates to DCS
                    Matrix3d matWCS2DCS;
                    matWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection);
                    matWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * matWCS2DCS;
                    matWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist,
                                                    acView.ViewDirection,
                                                    acView.Target) * matWCS2DCS;

                    // If a center point is specified, define the min and max 
                    // point of the extents
                    // for Center and Scale modes
                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        pMin = new Point3d(pCenter.X - (acView.Width / 2),
                                            pCenter.Y - (acView.Height / 2), 0);

                        pMax = new Point3d((acView.Width / 2) + pCenter.X,
                                            (acView.Height / 2) + pCenter.Y, 0);
                    }

                    // Create an extents object using a line
                    using (Line acLine = new Line(pMin, pMax))
                    {
                        eExtents = new Extents3d(acLine.Bounds.Value.MinPoint,
                                                    acLine.Bounds.Value.MaxPoint);
                    }

                    // Calculate the ratio between the width and height of the current view
                    double dViewRatio;
                    dViewRatio = (acView.Width / acView.Height);

                    // Tranform the extents of the view
                    matWCS2DCS = matWCS2DCS.Inverse();
                    eExtents.TransformBy(matWCS2DCS);

                    double dWidth;
                    double dHeight;
                    Point2d pNewCentPt;

                    // Check to see if a center point was provided (Center and Scale modes)
                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        dWidth = acView.Width;
                        dHeight = acView.Height;

                        if (dFactor == 0)
                        {
                            pCenter = pCenter.TransformBy(matWCS2DCS);
                        }

                        pNewCentPt = new Point2d(pCenter.X, pCenter.Y);
                    }
                    else // Working in Window, Extents and Limits mode
                    {
                        // Calculate the new width and height of the current view
                        dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X;
                        dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y;

                        // Get the center of the view
                        pNewCentPt = new Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5),
                                                    ((eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5));
                    }

                    // Check to see if the new width fits in current window
                    if (dWidth > (dHeight * dViewRatio)) dHeight = dWidth / dViewRatio;

                    // Resize and scale the view
                    if (dFactor != 0)
                    {
                        acView.Height = dHeight * dFactor;
                        acView.Width = dWidth * dFactor;
                    }

                    // Set the center of the view
                    acView.CenterPoint = pNewCentPt;

                    // Set the current view
                    acDoc.Editor.SetCurrentView(acView);
                }

                // Commit the changes
                acTrans.Commit();
            }
        }


        [CommandMethod("GetEntity")]
        static public void GetEntity()
        {

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions options = new PromptEntityOptions("选择物体");
            PromptEntityResult res = ed.GetEntity(options);

            if (res.Status == PromptStatus.Cancel)
                return;

            ObjectId id1 = res.ObjectId;

            Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            Utils.TransactionControl(() =>
            {
                Entity ent = (Entity)tm.GetObject(id1, OpenMode.ForWrite, false);

                if (ent is BaseTunnel)
                {
                    BaseTunnel tunnel = ent as BaseTunnel;
                    Random rd = new Random();
                    int s = rd.Next(1, 10);
                    tunnel.Segment = s;

                    List<uint> colors = new List<uint>();
                    List<short> temperatures = new List<short>();
                    for (int i = 0; i < s + 1; i++)
                    {
                        short t = (short)rd.Next(1, 99);
                        temperatures.Add(t);
                        uint color = Utils.temperature2uint(t);
                        colors.Add(color);
                    }

                    tunnel.Colors = colors;
                    tunnel.Temperatures = temperatures;

                }
                else if (ent is Node)
                {
                    Node node = ent as Node;
                }

            });
        }

        //纹理测试
        static private void createMaterial(Database db, Transaction myT)
        {
            DBDictionary dic =
                   (DBDictionary)myT.GetObject(db.MaterialDictionaryId, OpenMode.ForWrite, false);

            ImageFileTexture imfttr = new ImageFileTexture();
            imfttr.SourceFileName = @"D:\Geological Project\Geological Source\dbFiles\aaaaa.jpg";

            double uScale = 1.0;
            double vScale = 1.0;
            double uOffset = 0;
            double vOffset = 0;

            double[] p = new double[] {
             uScale,0,0,uScale * uOffset,
             0,vScale,0,vScale * vOffset,
             0,0,1,0,
             0,0,0,1  };

            Matrix3d mx = new Matrix3d(p);

            Mapper mapper = new Mapper(Projection.Planar, Tiling.Crop, Tiling.Crop,
                AutoTransform.TransformObject, mx);

            MaterialMap map = new MaterialMap(Source.File, imfttr, 1, mapper);


            EntityColor eclr = new EntityColor(150, 150, 150);
            MaterialColor mc = new MaterialColor(Method.Override, 1, eclr);
            MaterialDiffuseComponent mdc = new MaterialDiffuseComponent(mc, map);

            MaterialSpecularComponent mck = new MaterialSpecularComponent(mc, map, 0.5);
            MaterialOpacityComponent moc = new MaterialOpacityComponent(1, map);
            MaterialRefractionComponent mrfr = new MaterialRefractionComponent(2, map);

            Material Mat = new Material();

            Mat.Name = "My Material";
            Mat.Description = "New Material";
            Mat.Diffuse = mdc;
            Mat.Specular = mck;
            Mat.Refraction = mrfr;
            Mat.Reflectivity = 1;
            Mat.Reflection = map;
            Mat.Opacity = moc;
            Mat.Ambient = mc;
            Mat.Bump = map;
            Mat.SelfIllumination = 1;


            // MaterialDiffuseComponent diffuseColor = new MaterialDiffuseComponent(color, map);
            // material.Diffuse = diffuseColor;

            // material.Mode = Mode.Realistic;

            dic.SetAt(Mat.Name, Mat);
        }
        //把线全部转化为巷道
        [CommandMethod("LineToTN")]
        static public void transformToTunnel()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            PromptSelectionResult res = ed.SelectAll();

            if (res.Status == PromptStatus.Error)
            {
                return;
            }

            Autodesk.AutoCAD.EditorInput.SelectionSet SS = res.Value;

            var tmpidarray = SS.GetObjectIds();
            var idArray = SS.GetObjectIds();

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            List<DBTunnel> inList = new List<DBTunnel>();

            Utils.TransactionControl(() =>
            {
                foreach (ObjectId id in tmpidarray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForWrite, true);

                    if (!(entity is Line) && !(entity is Autodesk.AutoCAD.DatabaseServices.Polyline))
                        continue;

                    if (entity is Line)
                    {
                        Line line = entity as Line;
                        DBTunnel dbTunnel = new DBTunnel();
                        dbTunnel.BasePoints = new List<DBVertice>
                        { new DBVertice(line.StartPoint), new DBVertice(line.EndPoint) };
                        inList.Add(dbTunnel);
                        line.Erase();
                    }
                    else if (entity is Autodesk.AutoCAD.DatabaseServices.Polyline)
                    {
                        Autodesk.AutoCAD.DatabaseServices.Polyline polyline =
                            entity as Autodesk.AutoCAD.DatabaseServices.Polyline;

                        DBTunnel dbTunnel = new DBTunnel();
                        dbTunnel.BasePoints = new List<DBVertice>();

                        for (int i = 0; i < polyline.NumberOfVertices; i++)
                        {
                            dbTunnel.BasePoints.Add(new DBVertice(polyline.GetPoint3dAt(i)));
                        }

                        if (polyline.Closed == true)
                            dbTunnel.IsClosed = true;

                        inList.Add(dbTunnel);
                        polyline.Erase();

                    }
                }
            });
            var resList = DrawTunnel.StaticDrawSquareTunnel(inList);
        }
    }
}
