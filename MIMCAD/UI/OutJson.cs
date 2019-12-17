using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MIM;
using Newtonsoft.Json;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using UI.Model;

namespace UI
{
    class OutJson
    {
        /// <summary>
        /// 将当前dwg文件中的巷道关系数据导出
        /// </summary>
        static public void OutputRlv()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;

            OutRelationship outData = new OutRelationship();
            Utils.SelectAll("TUNNEL_SQUARE,TUNNEL_CYLINDER", (idArray) =>
            {
                foreach (var id in idArray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForRead, true);
                    if (entity is BaseTunnel)
                    {
                        BaseTunnel tunnel = entity as BaseTunnel;
                        List<OutVertice> outVertices = new List<OutVertice>();
                        List<Edge<OutVertice>> inEdges = new List<Edge<OutVertice>>();

                        for (int i = 0; i < tunnel.BasePoints.Count; i++)
                        {
                            Point3d vt = tunnel.BasePoints[i];
                            outVertices.Add(new OutVertice(vt.X, vt.Y, vt.Z));
                            if (i > 0)
                            {
                                inEdges.Add(new Edge<OutVertice>(outVertices[i - 1], outVertices[i]));
                            }
                        }
                        AdjacencyGraph<OutVertice, Edge<OutVertice>> graph =
                                new AdjacencyGraph<OutVertice, Edge<OutVertice>>();
                        graph.AddVerticesAndEdgeRange(inEdges);
                        List<Tuple<OutVertice, OutVertice>> outEdges = new List<Tuple<OutVertice, OutVertice>>();

                        foreach (var edge in graph.Edges)
                        {
                            outEdges.Add(new Tuple<OutVertice, OutVertice>(edge.Source, edge.Target));
                        }
                        outData.VerticeList.AddRange(outVertices);
                        outData.EdgeList.AddRange(outEdges);
                    }
                }

            });
            string path = Project.Instance.DataPath + "relationship.json";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException) { }
            }

            string str = JsonConvert.SerializeObject(outData);
            Fs.WriteStr(str, path);
        }

        /// <summary>
        /// 将当前dwg文件中的巷道与节点几何数据导出
        /// </summary>
        public static void OutputGeo()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            List<OutGeometry> outDatas = new List<OutGeometry>();

            Utils.SelectAll("TUNNEL_SQUARE,TUNNEL_CYLINDER,TUNNELNODE", (idArray) =>
            {
                foreach (var id in idArray)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForRead, true);
                    if (entity is BaseTunnel)
                    {
                        BaseTunnel tunnel = entity as BaseTunnel;
                        List<int> faces = tunnel.GetAllFaces();
                        List<Point3d> vertices = tunnel.GetAllVertices();

                        OutGeometry outData = new OutGeometry();
                        outData.Type = "Tunnel";
                        outData.FaceList = OutGeometry.toOutFace(faces);
                        outData.VerticeList = vertices.ConvertAll(
                            new Converter<Point3d, OutVertice>(OutVertice.Point3dToOutVertice));
                        outData.ColorList = tunnel.GetVerticesColors();

                        outDatas.Add(outData);
                    }
                    else if (entity is Node)
                    {
                        Node node = entity as Node;
                        OutGeometry outData = new OutGeometry();
                        outData.Type = "Node";
                        outData.Position = new OutVertice(node.Position.X, node.Position.Y, node.Position.Z);
                        outData.Radius = node.Radius;
                        outData.NodeColor = node.NodeColor;

                        outDatas.Add(outData);

                    }
                }
            });

            string path = Project.Instance.DataPath + "geometry.json";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException) { }
            }

            string str = JsonConvert.SerializeObject(outDatas);
            Fs.WriteStr(str, path);
        }
    }
}
