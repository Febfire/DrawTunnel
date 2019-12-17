using System;
using System.ComponentModel;
using Autodesk.AutoCAD.Geometry;


namespace EntityStore.Models
{
    public class DBVertice
    {       
        private double _x, _y, _z;
        private long _nodeHandleValue = 0;
        private int _index = -999999999;
        public DBVertice()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.NodeHandleValue = 0;
        }
        public DBVertice(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.NodeHandleValue = 0;
        }
        public DBVertice(Point3d point3d)
        {
            this.X = point3d.X;
            this.Y = point3d.Y;
            this.Z = point3d.Z;
            this.NodeHandleValue = 0;
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        public double X
        {
            get
            {
                return Math.Round(_x, 5);
            }
            set
            {
                _x = value;
            }
        }
        public double Y
        {
            get
            {
                return Math.Round(_y, 5);
            }
            set
            {
                _y = value;
            }
        }
        public double Z
        {
            get
            {
                return Math.Round(_z, 5);
            }
            set
            {
                _z = value;
            }

        }

        public long NodeHandleValue
        {
            get
            {
                return _nodeHandleValue;
            }
            set
            {
                _nodeHandleValue = value;
            }
        }
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", this.X, this.Y, this.Z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is DBVertice)
            {
                var position = obj as DBVertice;
                return position.X == this.X && position.Y == this.Y && position.Z == this.Z;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            int hashCodeX = X.GetHashCode();
            int hashCodeY = Y.GetHashCode();
            int hashCodeZ = Z.GetHashCode();
            return hashCodeX ^ hashCodeY ^ hashCodeZ;
        }

        public static bool operator ==(DBVertice leftHandSide, DBVertice rightHandSide)
        {
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);
            return (leftHandSide.Equals(rightHandSide));
        }
        public static bool operator !=(DBVertice leftHandSide, DBVertice rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }
    }
}
