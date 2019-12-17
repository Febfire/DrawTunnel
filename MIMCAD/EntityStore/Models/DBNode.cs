using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using MIM;
using System;

namespace EntityStore.Models
{
    public class DBNode : DBEntity,IEquatable<DBNode>
    {    
        public override void SetProperty(Entity ent)
        {
            base.SetProperty(ent);

            Node node = ent as Node;
            this.Name = node.Name;

            this.Position = new DBVertice(node.Position.X, node.Position.Y, node.Position.Z);
 
            this.CountOfBind = node.CountOfBind;

            this.NodeColor = node.NodeColor;

            this.Location = node.Location;

        }

        public override string Type
        {
            get
            {
                return "TunnelNode";
            }
        }

        public string Name { get; set; }

        public DBVertice Position { get; set; }

        public UInt32 NodeColor { get; set; }

        public ulong CountOfBind { get; set; }

        public void ToCADObject(Node node)
        {
            node.Name = this.Name;
            node.Position = new Point3d(this.Position.X, this.Position.Y, this.Position.Z);
            node.NodeColor = this.NodeColor;
            node.Location = this.Location;
        }

        public override bool Equals(object other)
        {
            var TunnelNode = other as DBNode;
            if (TunnelNode == null)
                return false;
            else
            {
                return this.HandleValue == TunnelNode.HandleValue;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = this.HandleValue.GetHashCode();
            return hashCode;
        }

        public bool Equals(DBNode other)
        {
            if (other == null)
                return false;
            else
            {
                return this.HandleValue == other.HandleValue;
            }
        }

        public static bool operator ==(DBNode leftHandSide, DBNode rightHandSide)
        {
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);
            return (leftHandSide.Equals(rightHandSide));
        }
        public static bool operator !=(DBNode leftHandSide, DBNode rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }

    }
}
