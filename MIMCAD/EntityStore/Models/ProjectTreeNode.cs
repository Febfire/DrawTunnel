using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using System.Collections.Generic;


namespace EntityStore.Models
{
    public class ProjectTreeNode
    {
        public ProjectTreeNode()
        {
        }

        public ProjectTreeNode(string name)
        {
            this.Name = name;
            this.Children = new List<ProjectTreeNode>();
        }

        public ProjectTreeNode(string name, ProjectTreeNode parentNode)
        {
            this.Name = name;
            SetParentNode(parentNode);
            this.Children = new List<ProjectTreeNode>();
        }
        public int _id { get; set; }
        public string Name { get; set; }
        public string Path { get { return getPath(); } }

        public string ParentPath { get; set; }
        public List<ProjectTreeNode> Children { get; set; }
        protected ProjectTreeNode ParentNode;
        virtual public string getPath()
        {
            string path = "";
            if (ParentNode != null)
                path += ParentNode.getPath() + ">";
            return path += this.Name;
        }

        public ProjectTreeNode GetParentNode()
        {
            return ParentNode;
        }
        //设置父节点，或者父节点改变刷新父节点路径
        public void SetParentNode(ProjectTreeNode parentNode)
        {
            ParentNode = parentNode;
            if (this.ParentNode == null)
            {
                this.ParentNode = null;
            }
            else
            {
                this.ParentPath = this.ParentNode.getPath();
            }
        }
    }

    public class ProjectTreeLeafNode:ProjectTreeNode
    {
        public ProjectTreeLeafNode() { }
        public ProjectTreeLeafNode(string name , long handleValue):base(name)
        {          
            this.HandleValue = handleValue;
        }
        public ProjectTreeLeafNode(string name, ProjectTreeNode parentNode,long handleValue):base(name, parentNode)
        {
            this.HandleValue = handleValue;
            this.ParentNode = parentNode;
        }

        public long HandleValue { get; set; }

        public override string getPath()
        {
            string path = "";
            if (ParentNode != null)
                path += ParentNode.getPath() + ">";
            return path += this.Name + this.HandleValue.ToString();
        }


    } 
}
