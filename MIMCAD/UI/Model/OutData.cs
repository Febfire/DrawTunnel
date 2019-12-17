using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Model
{
    class OutRelationship
    {
        public OutRelationship()
        {
            VerticeList = new List<OutVertice>();
            EdgeList = new List<Tuple<OutVertice, OutVertice>>();
        }
        public List<OutVertice> VerticeList { get; set; }

        public List<Tuple<OutVertice, OutVertice>> EdgeList { get; set; }
    }

    class OutGeometry
    {
        public string Type { get; set; }
        //巷道导出的数据
        public List<OutVertice> VerticeList { get; set; }
        public List<List<int>> FaceList { get; set; }
        public List<uint> ColorList { get; set; }

        //节点导出的数据
        public OutVertice Position { get; set; }
        public uint Radius { get; set; }
        public uint NodeColor { get; set; }

        public static List<List<int>> toOutFace(List<int> inList)
        {
            List<List<int>> outFaces = new List<List<int>>();
            for (int i = 0; i < inList.Count; i += 4 + 1)
            {
                outFaces.Add(new List<int>());
                outFaces[outFaces.Count - 1].Add(inList[i + 1]);
                outFaces[outFaces.Count - 1].Add(inList[i + 2]);
                outFaces[outFaces.Count - 1].Add(inList[i + 4]);

                outFaces.Add(new List<int>());
                outFaces[outFaces.Count - 1].Add(inList[i + 3]);
                outFaces[outFaces.Count - 1].Add(inList[i + 4]);
                outFaces[outFaces.Count - 1].Add(inList[i + 2]);

            }
            return outFaces;
        }

    }

    class OutVertice
    {
        public OutVertice(double x, double y, double z)
        {
            this.X = Math.Round(x, 5);
            this.Y = Math.Round(y, 5);
            this.Z = Math.Round(z, 5);
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", X.ToString(), Y.ToString(), Z.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is OutVertice)
            {
                var position = obj as OutVertice;
                return position.X == this.X && position.Y == this.Y && position.Z == this.Z;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            string str = String.Format("{0},{1},{2}", X.ToString(), Y.ToString(), Z.ToString());
            return str.GetHashCode();
        }

        public static bool operator ==(OutVertice leftHandSide, OutVertice rightHandSide)
        {
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);
            return (leftHandSide.Equals(rightHandSide));
        }
        public static bool operator !=(OutVertice leftHandSide, OutVertice rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }

        public static OutVertice Point3dToOutVertice(Point3d p)
        {
            return new OutVertice(p.X, p.Y,p.Z);
        }
    }
}
