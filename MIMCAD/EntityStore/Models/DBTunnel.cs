using Autodesk.AutoCAD.DatabaseServices;
using System.ComponentModel;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using MIM;
namespace EntityStore.Models
{
    public class DBTunnel : DBEntity
    {
        static public readonly string Tunnel_type_s = "T_SquareTunnel";
        static public readonly string Tunnel_type_c = "T_CylinderTunnel";
        static public readonly string Tunnel_type_t = "T_TrapezoidalTunnel";

        //--------------基本属性----------------//

        public string Name { get; set; }

        public string TagData { get; set; }

        public bool DisplayTag { get; set; }

        //--------------几何属性----------------//
        [Browsable(false), DefaultValue(false)]
        public List<DBVertice> BasePoints { get; set; }

        public int Segment { get; set; }

        public List<uint> Colors { get; private set; }

        private List<short> _temperatures;
        public List<short> Temperatures
        {
            get
            {
                return _temperatures;
            }
            set
            {
                //value = new List<short>(10);
                //for (int i = 0; i < 10; i++)
                //{
                //    value.Add(30);
                //}
                _temperatures = value;
                Colors = new List<uint>();
                foreach (var temperature in _temperatures)
                {
                    Colors.Add(Utils.temperature2uint(temperature));
                }

            }
        }

        public double Width_t { get; set; }
        public double Width_b { get; set; }
        public double Height { get; set; }

        public double Radius { get; set; }

        public bool IsClosed { get; set; }

        //通过cad对象设置litedb对象
        public override void SetProperty(Entity ent)
        {
            base.SetProperty(ent);

            BaseTunnel tunnel = ent as BaseTunnel;

            if (tunnel.TunnelType == Tunnel_type_s)
            {
                this.Type = Tunnel_type_s;
                this.Width_t = ((SquareTunnel)tunnel).Width_t;
                this.Width_b = ((SquareTunnel)tunnel).Width_b;
                this.Height = ((SquareTunnel)tunnel).Height;
            }
            else if (tunnel.TunnelType == Tunnel_type_t)
            {
                this.Type = Tunnel_type_t;
                this.Width_t = ((SquareTunnel)tunnel).Width_t;
                this.Width_b = ((SquareTunnel)tunnel).Width_b;
                this.Height = ((SquareTunnel)tunnel).Height;
            }
            else if (tunnel.TunnelType == Tunnel_type_c)
            {
                this.Type = Tunnel_type_c;
                this.Radius = ((CylinderTunnel)tunnel).Radius;
            }
            else
                throw new System.Exception("类型错误");

            this.Name = tunnel.Name;

            this.TagData = tunnel.TagData;

            this.Location = tunnel.Location;

            this.Segment = tunnel.Segment;

            this.Temperatures = tunnel.Temperatures;

            this.BasePoints = new List<DBVertice>();

            int i = 0;
            foreach (var point in tunnel.BasePoints)
            {
                DBVertice p;
                if (tunnel.NodesHandle.Count > 0)
                {
                    p = new DBVertice(point.X, point.Y, point.Z);
                    p.NodeHandleValue = tunnel.NodesHandle[i].Value;
                }
                else
                {
                    p = new DBVertice(point.X, point.Y, point.Z);
                }

                p.Index = i;
                if (tunnel.NodesHandle.Count > i)
                    p.NodeHandleValue = tunnel.NodesHandle[i].Value;
                this.BasePoints.Add(p);
                i++;
            }

            this.DisplayTag = tunnel.DisplayTag;
            this.IsClosed = tunnel.Closed;
        }


        //通过数据库对象修改cad对象
        public void ToCADObject(BaseTunnel tunnel)
        {
            int segmentCount = this.BasePoints.Count;

            var dbVertices = this.BasePoints;

            List<Point3d> points3d = new List<Point3d>();
            List<Handle> nodesHandle = new List<Handle>();

            foreach (var dbVertice in dbVertices)
            {
                points3d.Add(new Point3d(dbVertice.X, dbVertice.Y, dbVertice.Z));
                nodesHandle.Add(new Handle(dbVertice.NodeHandleValue));
            }
            for (int i = 0; i < nodesHandle.Count; i++)
            {
                if (nodesHandle[i].Value == 0)  //新产生的Node
                {
                    Node node = new Node();
                    node.Position = points3d[i];
                    node.Location = this.Location;
                    Utils.AppendEntity(node);
                    node.AppendTunnel(tunnel.Handle, i);
                    nodesHandle[i] = node.Handle;

                }
                else   //之前的Node,改变索引
                {
                    Node node = Utils.GetEntityByHandle(nodesHandle[i]) as Node;
                    if (node == null) continue;

                    node.ChangeIndex(tunnel.Handle, i);
                }
            }

            if (this.Type == Tunnel_type_s || this.Type == Tunnel_type_t)
            {
                ((SquareTunnel)tunnel).Width_t = this.Width_t;
                ((SquareTunnel)tunnel).Width_b = this.Width_b;
                ((SquareTunnel)tunnel).Height = this.Height;
            }
            else if (this.Type == Tunnel_type_c)
            {
                ((CylinderTunnel)tunnel).Radius = this.Radius;
            }
            else
                throw new System.Exception("类型错误");

            tunnel.TunnelType = this.Type;
            tunnel.BasePoints = points3d;
            tunnel.NodesHandle = nodesHandle;
            tunnel.Name = this.Name;
            tunnel.TagData = this.TagData;
            tunnel.Location = this.Location;
            tunnel.DisplayTag = this.DisplayTag;
            tunnel.Segment = this.Segment;
            tunnel.Temperatures = this.Temperatures;
            tunnel.Colors = this.Colors;
            tunnel.SetClose(this.IsClosed);
        }

    }


}






