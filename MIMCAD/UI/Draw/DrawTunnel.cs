using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using EntityStore.Models;
using System.Collections.Generic;
using UI.Jig;
using MIM;
using System;


[assembly: CommandClass(typeof(UI.Draw.DrawTunnel))]
namespace UI.Draw
{
    public class DrawTunnel
    {

        //手工画圆柱形巷道的函数
        [CommandMethod("CylTunnel")]
        static public void DynamicDrawCylinderTunnel(DBTunnel inTunnel = null)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Tunnel Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d firstPoint = res.Value;

            //create Tunnel
            CylinderTunnelJig jig = new CylinderTunnelJig(firstPoint);
            CylinderTunnel tmpTunnel = (CylinderTunnel)jig.GetEntity();

            for (int i = 0; ; i++)
            {
                jig.setPromptCounter(i);
                PromptResult drag = ed.Drag(jig);
                if (drag.Status == PromptStatus.Cancel || drag.Status == PromptStatus.None)//画完了
                {
                    Utils.TransactionControl(() =>
                    {
                        List<Point3d> points = tmpTunnel.BasePoints;
                        points.RemoveAt(points.Count - 1);
                        if (points.Count < 2) return;
                        CylinderTunnel tunnel = new CylinderTunnel();
                        tunnel.TunnelType = DBTunnel.Tunnel_type_c;
                        tunnel.BasePoints = points;
                        tunnel.Radius = 10;
                        tunnel.Name = "巷道";
                        try
                        {
                            tunnel.Location = Project.Instance.getCurrentSurface(doc).Path;
                        }
                        catch (System.Exception) { }
                        
                        Utils.AppendEntity(tunnel);

                        appendNode(tunnel);                  
                        appendTag(tunnel);

                        tunnel.Dispose();
                        tmpTunnel.Dispose();
                    });
                    break;
                }
                else if (drag.Status == PromptStatus.OK)
                {

                }
            }
        }

      
        //手工画方形巷道的函数
        [CommandMethod("SqTunnel")]
        static public void DynamicDrawSquareTunnel(DBTunnel inTunnel = null)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            
            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Tunnel Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d firstPoint = res.Value;

            //create Tunnel
            SquareTunnelJig jig = new SquareTunnelJig(firstPoint);
            SquareTunnel tmpTunnel = (SquareTunnel)jig.GetEntity();

            for (int i = 0; ; i++)
            {
                jig.setPromptCounter(i);
                PromptResult drag = ed.Drag(jig);
                if (drag.Status == PromptStatus.Cancel || drag.Status == PromptStatus.None)//画完了
                {
                    Utils.TransactionControl(() =>
                    {
                        List<Point3d> points = tmpTunnel.BasePoints;
                        points.RemoveAt(points.Count - 1);
                        if (points.Count < 2) return;
                        SquareTunnel tunnel = new SquareTunnel();
                        tunnel.TunnelType = DBTunnel.Tunnel_type_s;
                        tunnel.Width_b = tunnel.Width_t = 10;
                        tunnel.BasePoints = points;
                        tunnel.Name = "巷道";
                        try
                        {
                            tunnel.Location = Project.Instance.getCurrentSurface(doc).Path;
                        }
                        catch (System.Exception) { }
                        Utils.AppendEntity(tunnel);
                        appendNode(tunnel);
                        appendTag(tunnel);

                        tunnel.Dispose();
                        tmpTunnel.Dispose();
                    });
                    break;
                }
                else if (drag.Status == PromptStatus.OK)
                {

                }
            }
        }

        [CommandMethod("TrTunnel")]
        static public void DynamicDrawTrapezoidalTunnel(DBTunnel inTunnel = null)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            //输入起点坐标
            PromptPointOptions opts = new PromptPointOptions("\nEnter Tunnel Start Point:");
            PromptPointResult res = ed.GetPoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return;

            Point3d firstPoint = res.Value;

            //create Tunnel
            SquareTunnelJig jig = new SquareTunnelJig(firstPoint);
            SquareTunnel tmpTunnel = (SquareTunnel)jig.GetEntity();

            for (int i = 0; ; i++)
            {
                jig.setPromptCounter(i);
                PromptResult drag = ed.Drag(jig);
                if (drag.Status == PromptStatus.Cancel || drag.Status == PromptStatus.None)//画完了
                {
                    Utils.TransactionControl(() =>
                    {
                        List<Point3d> points = tmpTunnel.BasePoints;
                        points.RemoveAt(points.Count - 1);
                        if (points.Count < 2) return;
                        SquareTunnel tunnel = new SquareTunnel();
                        tunnel.TunnelType = DBTunnel.Tunnel_type_t;
                        tunnel.Width_b = 12;
                        tunnel.Width_t = 8;
                        tunnel.BasePoints = points;
                        tunnel.Name = "巷道";
                        try
                        {
                            tunnel.Location = Project.Instance.getCurrentSurface(doc).Path;
                        }
                        catch (System.Exception) { }
                        Utils.AppendEntity(tunnel);
                        appendNode(tunnel);
                        appendTag(tunnel);

                        tunnel.Dispose();
                        tmpTunnel.Dispose();
                    });
                    break;
                }
                else if (drag.Status == PromptStatus.OK)
                {

                }
            }
        }

        /**********************************************/

        //通过参数的方式生成方形巷道
        static public List<DBTunnel> StaticDrawSquareTunnel(List<DBTunnel> inList)
        {
            if (inList == null)
                return null;

            List<DBTunnel> outList = new List<DBTunnel>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            foreach (var v in inList)
            {
                if (v.HandleValue > 0)
                {
                    outList.Add(v);
                    continue;
                }
                Utils.TransactionControl(() =>
                {
                    List<Point3d> points = new List<Point3d>();
                    foreach (var point in v.BasePoints)
                    {
                        points.Add(new Point3d(
                            point.X,
                            point.Y,
                            point.Z
                            ));
                    }
                    SquareTunnel tunnel = new SquareTunnel();
                    tunnel.TunnelType = DBTunnel.Tunnel_type_s;
                    tunnel.BasePoints = points;
                    tunnel.SetClose(v.IsClosed);
                    tunnel.Name = (v.Name==null||v.Name=="")?"巷道":v.Name;
                    try
                    {
                        tunnel.Location = (v.Location != null && v.Location != "") ?
                        v.Location : Project.Instance.getCurrentSurface(doc).Path;
                    }
                    catch (System.Exception) { }
                   
                    Utils.AppendEntity(tunnel);
                    appendNode(tunnel);
                    appendTag(tunnel);

                    DBTunnel dbTunnel = new DBTunnel();
                    dbTunnel.SetProperty(tunnel);
                    tunnel.Dispose();
                    outList.Add(dbTunnel);

                });
            }
            return outList;
        }

        //通过参数的方式生成圆柱形巷道
        static public List<DBTunnel> StaticDrawCylinderTunnel(List<DBTunnel> inList)
        {
            if (inList == null)
                return null;

            List<DBTunnel> outList = new List<DBTunnel>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            foreach (var v in inList)
            {
                if (v.HandleValue > 0)
                {
                    outList.Add(v);
                    continue;
                }
                Utils.TransactionControl(() =>
                {
                    List<Point3d> points = new List<Point3d>();
                    foreach (var point in v.BasePoints)
                    {
                        points.Add(new Point3d(
                            point.X,
                            point.Y,
                            point.Z
                            ));
                    }
                    CylinderTunnel tunnel = new CylinderTunnel();
                    tunnel.TunnelType = DBTunnel.Tunnel_type_s;
                    tunnel.BasePoints = points;
                    tunnel.Name = (v.Name == null || v.Name == "") ? "巷道" : v.Name;
                    try
                    {
                        tunnel.Location = (v.Location != null && v.Location != "") ?
                        v.Location : Project.Instance.getCurrentSurface(doc).Path;
                    }
                    catch (System.Exception) { }
                    Utils.AppendEntity(tunnel);
                    appendNode(tunnel);
                    appendTag(tunnel);

                    DBTunnel dbTunnel = new DBTunnel();
                    dbTunnel.SetProperty(tunnel);
                    tunnel.Dispose();
                    outList.Add(dbTunnel);

                });
            }
            return outList;
        }

        //添加节点
        static void appendNode(BaseTunnel tunnel)
        {
            List<Handle> nodesHandle = new List<Handle>();
            for (int j = 0; j < tunnel.BasePoints.Count; j++)
            {
                Node node = new Node();
                node.Position = tunnel.BasePoints[j];
                node.Name = "节点";
                node.Location = tunnel.Location;
                Utils.AppendEntity(node);
                node.AppendTunnel(tunnel.Handle, j);
                nodesHandle.Add(node.Handle);
                node.Dispose();
            }
            tunnel.NodesHandle = nodesHandle;
        }
       
        //添加标注
        static private void appendTag(BaseTunnel tunnel)
        {
            var vvs = tunnel.VerticalVectors;
            var vv = vvs[vvs.Count / 2];

            var cvs = tunnel.CenterVectors;
            var cv = cvs[cvs.Count / 2];

            var av = tunnel.BasePoints[tunnel.BasePoints.Count / 2 - 1] -
            tunnel.BasePoints[tunnel.BasePoints.Count / 2];
            var stdPoint = tunnel.BasePoints[tunnel.BasePoints.Count / 2] + av / 2;
            var lfPoint = stdPoint + vv * cv.Length / 5;

            var endPoint = lfPoint + cv / 7;
            Tag tag = new Tag();
            tag.StartPoint = stdPoint;
            tag.InflectionPoint = lfPoint;
            tag.EndPoint = endPoint;
            Utils.AppendEntity(tag);
            tunnel.TagHandle = tag.Handle;

            tag.Dispose();         
        }
    }
}

