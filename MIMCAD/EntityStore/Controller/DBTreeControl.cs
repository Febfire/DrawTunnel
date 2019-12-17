using EntityStore.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;


namespace EntityStore.Controller
{
    public class DBTreeControl
    {
        public LiteDatabase DB { get { return _database; } }

        private LiteDatabase _database;

        private LiteCollection<ProjectTreeNode> _collection;
        public string Name { get; private set; }

        public DBTreeControl(string dbName)
        {
            _database = new LiteDatabase(dbName);
            _collection = _database.GetCollection<ProjectTreeNode>("DBProjectTree");
            Name = dbName;
        }

        public void StoreTree(List<ProjectTreeNode> tree)
        {
            _collection.Delete(Query.All());
            foreach (var node in tree)
            {
                _collection.Insert(node);
            }
        }

        public List<ProjectTreeNode> GetTree()
        {
            var rets = _collection.FindAll().ToList();
            return rets;
        }

        //得到第n层的名字
        static public string GetLevelName(string path,int level)
        {
            string[] names = path.Split('>');
            if (names.Count() < level+1) return null;
            else return names[level];
        }


        //通过数据库中的数据重新实例化一个工程树,使各个节点产生父子关系
        public List<ProjectTreeNode> RebuldFromDB(out ProjectTreeNode currentWorkingSurface)
        {
            //数据库中的树信息
            List<ProjectTreeNode> oldTree = _collection.FindAll().ToList();
            List<ProjectTreeNode> tree = new List<ProjectTreeNode>();
            currentWorkingSurface = null;
            //数据库还没有树的信息，可能是一个空的图纸
            if (oldTree.Count == 0)
            {
                ProjectTreeNode node0 = new ProjectTreeNode("煤矿1", null);node0.Children.Clear();
                ProjectTreeNode node1 = new ProjectTreeNode("水平1", node0); node0.Children.Add(node1);
                ProjectTreeNode node2 = new ProjectTreeNode("采区1", node1); node1.Children.Add(node2);
                ProjectTreeNode node3 = new ProjectTreeNode("工作面1", node2); node2.Children.Add(node3);

                tree.Add(node0); tree.Add(node1); tree.Add(node2); tree.Add(node3);
                currentWorkingSurface = node3;
            }
            else //有信息
            {
                List<ProjectTreeNode> level0 = new List<ProjectTreeNode>(); 
                List<ProjectTreeNode> level1 = new List<ProjectTreeNode>();
                List<ProjectTreeNode> level2 = new List<ProjectTreeNode>();
                List<ProjectTreeNode> level3 = new List<ProjectTreeNode>();
                List<ProjectTreeNode> level4 = new List<ProjectTreeNode>();

                foreach (var node in oldTree)
                {
                    if (node.ParentPath == null)
                    {
                        level0.Add(node);
                        node.Children.Clear();
                    }

                    else if (node.ParentPath.Split('>').Count() == 1)
                    {
                        level1.Add(node);
                        node.Children.Clear();
                    }
                    else if (node.ParentPath.Split('>').Count() == 2)
                    {
                        level2.Add(node);
                        node.Children.Clear();
                    }
                    else if (node.ParentPath.Split('>').Count() == 3)
                    {
                        level3.Add(node);
                        node.Children.Clear();
                    }
                    else if (node.ParentPath.Split('>').Count() == 4)
                    {
                        level4.Add(node);
                        node.Children.Clear();
                    }
                }

                foreach (var node in level0)
                {
                    tree.Add(node);
                }

                foreach (var node in level1)
                {
                    ProjectTreeNode newNode = new ProjectTreeNode(node.Name);
                    var pNode = tree.Find((pn) =>
                    {
                        if (pn.Path == node.ParentPath) return true;
                        else return false;
                    });
                    newNode.SetParentNode(pNode);
                    pNode.Children.Add(newNode);
                    tree.Add(newNode);
                }
                foreach (var node in level2)
                {
                    ProjectTreeNode newNode = new ProjectTreeNode(node.Name);
                    var pNode = tree.Find((pn) =>
                    {
                        if (pn.Path == node.ParentPath) return true;
                        else return false;
                    });
                    newNode.SetParentNode(pNode);
                    pNode.Children.Add(newNode);
                    tree.Add(newNode);
                }
                foreach (var node in level3)
                {
                    ProjectTreeNode newNode = new ProjectTreeNode(node.Name);
                    var pNode = tree.Find((pn) =>
                    {
                        if (pn.Path == node.ParentPath) return true;
                        else return false;
                    });
                    newNode.SetParentNode(pNode);
                    pNode.Children.Add(newNode);
                    tree.Add(newNode);
                    currentWorkingSurface = newNode;
                }
                foreach (var node in level4)
                {
                    ProjectTreeLeafNode newNode = new ProjectTreeLeafNode(node.Name, ((ProjectTreeLeafNode)node).HandleValue);
                    var pNode = tree.Find((pn) =>
                    {
                        if (pn.Path == node.ParentPath) return true;
                        else return false;
                    });
                    newNode.SetParentNode(pNode);
                    pNode.Children.Add(newNode);
                    tree.Add(newNode);
                }
            }
            return tree;
        }
    }
}
