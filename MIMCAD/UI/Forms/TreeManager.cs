using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using DevExpress.XtraTreeList.Nodes;
using EntityStore.Controller;
using EntityStore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiteDB;
using System.IO;

[assembly: CommandClass(typeof(UI.Forms.TreeManager))]
namespace UI.Forms
{
    public partial class TreeManager : UserControl
    {

        public Autodesk.AutoCAD.Windows.PaletteSet PaletteSet { get; private set; }

        private DevExpress.XtraTreeList.TreeList treeList1;

        public TreeManager()
        {
            InitializeComponent();
            initTreelist2();
        }
        public void ExpandAll()
        {
            treeList1.ExpandAll();
        }

        //设置可见性

        public void BindPaletteSet(Autodesk.AutoCAD.Windows.PaletteSet ps)
        {
            ps.Add("场景管理", this);
            this.PaletteSet = ps;
            this.PaletteSet.KeepFocus = true;
        }

        //**********************************************************************************************************//
        //*********************************************treelist1****************************************************//
        //**********************************************************************************************************//

        public void ShowNullDwgMessage()
        {
            labelControl2.Visible = true;
        }

        public void InitTreelist1(Document doc, DevExpress.XtraTreeList.TreeList treelist)
        {
            treelist.Visible = true;
            treelist.Dock = System.Windows.Forms.DockStyle.Fill;
            treelist.KeyFieldName = "Path";
            treelist.Location = new System.Drawing.Point(0, 0);
            treelist.Name = "treeList1";
            treelist.OptionsBehavior.Editable = false;
            treelist.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
            treelist.ParentFieldName = "ParentPath";
            treelist.RootValue = null;
            treelist.SelectImageList = this.imageList1;
            treelist.Size = new System.Drawing.Size(253, 505);
            treelist.TabIndex = 0;
            treelist.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeList1_GetSelectImage);
            treelist.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeList1_NodeCellStyle);
            treelist.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.treeList1_ValidatingEditor);
            treelist.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.treeList1_InvalidValueException);
            treelist.BeforeDropNode += new DevExpress.XtraTreeList.BeforeDropNodeEventHandler(this.treeList1_BeforeDropNode);
            treelist.AfterDropNode += new DevExpress.XtraTreeList.AfterDropNodeEventHandler(this.treeList1_AfterDropNode);
            treelist.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeList1_PopupMenuShowing);
            treelist.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeList1_CellValueChanged);
            treelist.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList1_MouseDown);

            imageList1.Images.Add(System.Drawing.Image.FromFile(Global.imgPath + @"\a1.ico"));

            imageList1.Images.Add(System.Drawing.Image.FromFile(Global.imgPath + @"\a3.ico"));

            treelist.OptionsBehavior.Editable = false; //不可编辑

            treelist.OptionsView.ShowIndicator = false;//不显示左边的一列 
            treelist.OptionsView.ShowHorzLines = false;//不显示水平线
            treelist.OptionsView.ShowVertLines = false;//垂直线条是否显示

            //聚焦的样式是否只适用于聚焦细胞或所有细胞除了聚焦对象，失去焦点后
            treelist.LookAndFeel.UseWindowsXPTheme = true;
            treelist.OptionsSelection.InvertSelection = true;

            this.tabNavigationPage1.Controls.Add(treelist);

            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            dbControl.EntityInserted += dataInserted; //添加litedb数据库事件，为了litedb数据更新时同步到这个控件
            dbControl.EntityDeleted += dataDeleted;   //..删除..
            dbControl.EntityUpdated += dataUpdated;   //..修改..

            ChangeTreelist1(doc);
            bindProjectData();
        }

        public void ChangeTreelist1(Document doc)
        {
            if (doc == null)
            {
                this.treeList1 = null;
                return;
            }

            if (this.treeList1 != null)
            {
                this.treeList1.Visible = false;
            }

            labelControl2.Visible = false;

            this.treeList1 = Project.Instance.GetActiveTreeLst(doc);

            this.treeList1.Visible = true;
        }

        private void bindProjectData()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var tree = Project.Instance.GetProjectTree(doc);
            treeList1.DataSource = tree;

            //把非Name的列隐藏
            if (treeList1.Columns.Count > 0)
            {

                for (int i = 0; i < treeList1.Columns.Count; i++)
                {
                    if (treeList1.Columns[i].FieldName != "Name")
                        treeList1.Columns[i].Visible = false;
                }
            }

            treeList1.ExpandToLevel(0);
            treeList1.ExpandToLevel(1);
            treeList1.ExpandToLevel(2);
        }

        public void ReflushDataSource()
        {
            treeList1.RefreshDataSource();
        }

        public void ExpandFocus()
        {
            treeList1.FocusedNode.Expand();
        }

        private void treeList1_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            if (e.Node == null) return;
            TreeListNode node = e.Node;

            int Level = node.Level;

            switch (Level)
            {
                case 4:
                    e.NodeImageIndex = 0;
                    break;
                default:
                    e.NodeImageIndex = 1;
                    break;
            }

        }

        //鼠标点击的节点设为焦点
        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            DevExpress.XtraTreeList.TreeList treelist = sender as DevExpress.XtraTreeList.TreeList;
            if (treelist == null) return;

            Point pt = new Point(e.X, e.Y);
            DevExpress.XtraTreeList.TreeListHitInfo ht = treeList1.CalcHitInfo(pt);
            if (ht != null && ht.Node != null)
            {
                treeList1.FocusedNode = ht.Node;

            }
            treeList1.OptionsBehavior.Editable = false;
            treeList1.CloseEditor();
        }
        //节点拖动结束前
        private void treeList1_BeforeDropNode(object sender, DevExpress.XtraTreeList.BeforeDropNodeEventArgs e)
        {
            if (e.SourceNode.Level - e.DestinationNode.Level != 1)
                e.Cancel = true;

            List<ProjectTreeNode> tree = treeList1.DataSource as List<ProjectTreeNode>;
            if (tree[e.SourceNode.Id] is ProjectTreeLeafNode && e.DestinationNode.Level == 3)
            {
                e.Cancel = false;
            }          
        }

        //节点拖动结束后
        private void treeList1_AfterDropNode(object sender, DevExpress.XtraTreeList.AfterDropNodeEventArgs e)
        {
            if (e.IsSuccess == true)
            {
                var sourceNode = getProjectNode(e.Node);
                var destinationNode = getProjectNode(e.DestinationNode);

                sourceNode.GetParentNode().Children.Remove(sourceNode);
                sourceNode.SetParentNode(destinationNode);
                destinationNode.Children.Add(sourceNode);
                changeParentNode(sourceNode);

                List<ProjectTreeLeafNode> ls = new List<ProjectTreeLeafNode>();
                getLeafNodes(sourceNode, ls);
                foreach (var leafNode in ls)
                {
                    modityLeafNode(leafNode);
                }
            }
        }

        //右键弹出菜单
        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            var menu = e.Menu as DevExpress.XtraTreeList.Menu.TreeListNodeMenu;
            if (menu == null) return;

            treeList1.FocusedNode = menu.Node;
            if (treeList1.FocusedNode.Level == 0)
            {
                
                
                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    if (treeList1.FocusedValue.ToString() == "未分配")
                    {
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t1Delete_ItemClick));
                    }
                    else
                    {
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("重命名", t1Edit_ItemClick));
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("添加工作水平", t1AddChild_ItemClick));
                    }                 
                }
            }
            else if (treeList1.FocusedNode.Level == 1)
            {
                if (!(getProjectNode(treeList1.FocusedNode) is ProjectTreeLeafNode) && e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("重命名", t1Edit_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("添加采区", t1AddChild_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t1Delete_ItemClick));
                }
            }
            else if (treeList1.FocusedNode.Level == 2)
            {
                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("重命名", t1Edit_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("添加工作面", t1AddChild_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t1Delete_ItemClick));
                }
            }
            else if (treeList1.FocusedNode.Level == 3)  //工作面节点的菜单
            {
                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("设为当前工作面", t1SetWork_ItemClick));
                   // e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("隐藏/显示工作面", t1HideWork_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("重命名", t1Edit_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t1Delete_ItemClick));
                }
            }
            else if (treeList1.FocusedNode.Level == 4)  //巷道节点的菜单
            {
                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("重命名", t1Edit_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t1Delete_ItemClick));
                }
            }
            else  //应该不会再有下一级节点了
            {
                throw new System.Exception("无效的节点");
            }

        }
        //节点名字改变时调用
        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            List<ProjectTreeNode> tree = treeList1.DataSource as List<ProjectTreeNode>;
            var focusNode = treeList1.FocusedNode;
            var node = getProjectNode(focusNode);

            changeParentNode(node);
            treeList1.RefreshDataSource();
            treeList1.OptionsBehavior.Editable = false;

            List<ProjectTreeLeafNode> ls = new List<ProjectTreeLeafNode>();
            getLeafNodes(node, ls);
            foreach (var leafNode in ls)
            {
                modityLeafNode(leafNode);
            }
        }

        //点击右键菜单“重命名”触发
        private void t1Edit_ItemClick(object sender, EventArgs e)
        {
            treeList1.OptionsBehavior.Editable = !treeList1.OptionsBehavior.Editable;
            treeList1.ShowEditor();
        }
        //点击右键菜单“添加”触发
        private void t1AddChild_ItemClick(object sender, EventArgs e)
        {
            var tree = treeList1.DataSource as List<ProjectTreeNode>;
            var focusNode = treeList1.FocusedNode;
            var pNode = getProjectNode(focusNode);

            string halfName = "";

            if (focusNode.Level == 0) halfName = "水平";
            else if (focusNode.Level == 1) halfName = "采区";
            else if (focusNode.Level == 2) halfName = "工作面";

            //准备一个初始化的名字
            int i = 1;
            string name = "";
            while (true)
            {
                var tmp = pNode.Children.Find((node) =>
                {
                    if (node.Name == halfName + i.ToString()) return true;
                    else return false;
                });
                if (tmp != null)
                {
                    i++;
                }
                else
                {
                    name = halfName + i.ToString();
                    break;
                }
            }

            ProjectTreeNode newNode = new ProjectTreeNode(name, pNode);
            pNode.Children.Add(newNode);
            tree.Add(newNode);
            treeList1.RefreshDataSource();

            focusNode.Expand();

        }
        //点击右键菜单“删除”触发
        private void t1Delete_ItemClick(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var focusNode = treeList1.FocusedNode;

            List<ProjectTreeNode> tree = treeList1.DataSource as List<ProjectTreeNode>;
            var node = getProjectNode(focusNode);
            if (node == null)
            {
                treeList1.Nodes.Remove(focusNode);
                return;
            }

            if (containNode(node, Project.Instance.getCurrentSurface(doc)))
            {
                Autodesk.AutoCAD.ApplicationServices.Application.
                    ShowAlertDialog("删除的节点不能包含当前工作面节点");
                return;
            }

            var pNode = node.GetParentNode();
            if (pNode != null)
            {
                pNode.Children.Remove(node);
            }
           
            List<ProjectTreeLeafNode> ls = new List<ProjectTreeLeafNode>();
            getLeafNodes(node, ls);//得到所有的叶子节点
            treeList1.DeleteNode(focusNode);
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            foreach (var leafNode in ls)
            {
                long handleValue = leafNode.HandleValue;
                dbControl.Delete(Query.EQ("HandleValue", handleValue), this);
            }
            doc.Editor.Regen();
        }
        //点击右键菜单“设为当前工作面”触发
        private void t1SetWork_ItemClick(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            List<ProjectTreeNode> tree = treeList1.DataSource as List<ProjectTreeNode>;
            var focusNode = treeList1.FocusedNode;
            var node = getProjectNode(focusNode);

            //之前的当前工作面节点
            var lstNode = getTreeListNode(Project.Instance.getCurrentSurface(doc));
            //更改当前工作面
            Project.Instance.setCurrentSurface(doc, node);
            //刷新节点颜色
            treeList1.RefreshNode(lstNode);
            treeList1.RefreshNode(focusNode);

        }

        private void t1HideWork_ItemClick(object sender, EventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            var focusNode = treeList1.FocusedNode;
            var node = getProjectNode(focusNode);

            bool hide;

            var hiddenNodes = Project.Instance.getHiddenSurfaces(doc);
          
            if (hiddenNodes == null || hiddenNodes.Contains(node))//已经是隐藏面了
            {
                hide = false;
                Project.Instance.remmoveHiddenSurface(doc,node);
            }
            else
            {
                hide = true;
                Project.Instance.setHiddenSurface(doc, node);
            }
            
            var path = node.getPath();

            Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = db.TransactionManager;
            Utils.SelectAll("TUNNEL_SQUARE,TUNNEL_CYLINDER", (ids) =>
            {
                foreach (var id in ids)
                {
                    Entity entity = (Entity)tm.GetObject(id, OpenMode.ForWrite, true);

                    if (entity is MIM.BaseTunnel)
                    {
                        MIM.BaseTunnel tunnel = entity as MIM.BaseTunnel;
                        if (tunnel.Location == path)
                        {
                            tunnel.DisplayNodes = hide;
                            tunnel.Visible = hide;
                            tunnel.Reflesh();
                        }
                    }
                }

            });
        }

        //验证修改数据是否有效
        private void treeList1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            var tree = treeList1.DataSource as List<ProjectTreeNode>;
            var focusNode = treeList1.FocusedNode;
            var pNode = getProjectNode(focusNode).GetParentNode();
            if (pNode == null) return;

            int level = focusNode.Level;

            foreach (var node in pNode.Children)
            {
                if (node.Name == e.Value.ToString() && node != tree[focusNode.Id] && level != 4)
                {
                    e.ErrorText = "已经包含相同名字的节点";
                    e.Valid = false;
                    treeList1.EditingValue = tree[focusNode.Id].Name;
                    return;
                }
                else if (e.Value.ToString() == "")
                {
                    e.ErrorText = "输入的名字不能为空";
                    e.Valid = false;
                    treeList1.EditingValue = tree[focusNode.Id].Name;
                    return;
                }
            }
            treeList1.OptionsBehavior.Editable = false;
            treeList1.CloseEditor();
        }

        //如果重命名时输入了无效的值，弹出一个出错信息
        private void treeList1_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            //Show the message with the error text specified 
            MessageBox.Show(e.ErrorText);
        }


        //**************************数据库事件的回调函数**************************//

        //数据库插入事件的回调,包含cad撤销事件
        private void dataInserted(object sender, DBEntity entity)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(this))
            {
                return;
            }

            if (!(entity is DBTunnel)) return;

            DBTunnel dbTunnel = entity as DBTunnel;

            var currentWorkingSurface = Project.Instance.getCurrentSurface(doc);
            if (currentWorkingSurface == null) throw new System.Exception("找不到当前工作面");

            //确定工作面
            ProjectTreeNode pNode = null;


            //如果是当前工作面
            if (dbTunnel.Location == currentWorkingSurface.Path)
                pNode = currentWorkingSurface;
            else   //非当前工作面
            {
                var surface = findNodeByPath(dbTunnel.Location);
                //var surface = findParentNodeByPath(dbTunnel.Location);
                if (surface != null)  //工程树中有巷道保存的工作面
                    pNode = surface;
                else
                {
                    pNode = getUnassignedNode();
                }
            }

            ProjectTreeLeafNode nodex =
            new ProjectTreeLeafNode(dbTunnel.Name, pNode, dbTunnel.HandleValue);
            pNode.Children.Add(nodex);
            var projectTree = Project.Instance.GetProjectTree(doc);
            var flag = projectTree.Find((n) =>
            {
                if (n.Path == nodex.Path) return true;
                else return false;
            });
            if (flag == null)
                projectTree.Add(nodex);
            treeList1.RefreshDataSource();

        }
        //数据库删除事件的回调
        private void dataDeleted(object sender, List<DBEntity> entities)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(this))
            {
                return;
            }

            foreach (var entity in entities)
            {
                if (!(entity is DBTunnel)) continue;

                DBTunnel dbTunnel = entity as DBTunnel;

                var projectTree = Project.Instance.GetProjectTree(doc);

                var deleteNode = projectTree.Find((node) =>
                {
                    if (!(node is ProjectTreeLeafNode)) return false;
                    else
                    {
                        ProjectTreeLeafNode leafNode = node as ProjectTreeLeafNode;
                        if (leafNode.HandleValue == dbTunnel.HandleValue) return true;
                        else return false;
                    }
                });
                if (deleteNode != null)
                {
                    deleteNode.GetParentNode().Children.Remove(deleteNode);
                    bool ok = projectTree.Remove(deleteNode);
                    if (ok)
                        treeList1.RefreshDataSource();
                }
            }
        }
        //数据库修改事件的回调
        private void dataUpdated(object sender, DBEntity entity)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            if (dbControl.Senders.Contains(this))
            {
                return;
            }

            if (!(entity is DBTunnel)) return;

            DBTunnel dbTunnel = entity as DBTunnel;
            var projectTree = Project.Instance.GetProjectTree(doc);
            var updatedNode = projectTree.Find((node) =>
            {
                if (!(node is ProjectTreeLeafNode)) return false;
                else
                {
                    ProjectTreeLeafNode leafNode = node as ProjectTreeLeafNode;
                    if (leafNode.HandleValue == dbTunnel.HandleValue) return true;
                    else return false;
                }
            });
            updatedNode.Name = dbTunnel.Name;

            treeList1.RefreshDataSource();
        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var node = getProjectNode(e.Node);
            if (node == Project.Instance.getCurrentSurface(doc))
            {
                e.Appearance.BackColor = Color.Gray;
            }
        }

        //******************************功能函数******************************//
        //根据路径字符串找到对应的路径
        private ProjectTreeNode findNodeByPath(string path)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            string[] names = path.Split('>');
            var tree = Project.Instance.GetProjectTree(doc);
            var findNode = tree.Find((node) =>
            {
                if (node.Path == path) return true;
                else return false;
            });
            return findNode;
        }
        //根据路径字符串找到对应父节点的路径
        private ProjectTreeNode findParentNodeByPath(string path)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            string[] names = path.Split('>');
            string pPath = "";
            for (int i = 0; i < names.Length - 1; i++)
            {
                pPath += names[i];
                if (i < names.Length - 2) pPath += ">";
            }
            var tree = Project.Instance.GetProjectTree(doc);
            var findNode = tree.Find((node) =>
            {
                if (node.Path == pPath) return true;
                else return false;
            });
            return findNode;
        }

        private ProjectTreeNode getProjectNode(TreeListNode node)
        {
            return treeList1.GetDataRecordByNode(node) as ProjectTreeNode;
        }

        private TreeListNode getTreeListNode(ProjectTreeNode node)
        {
            List<ProjectTreeNode> tree = treeList1.DataSource as List<ProjectTreeNode>;
            //之前的当前工作面在treelist中的节点索引值
            int index = tree.IndexOf(node);
            return treeList1.GetNodeByVisibleIndex(index);
        }

        //得到当前工程树中的未分配根节点
        private ProjectTreeNode getUnassignedNode()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var tree = Project.Instance.GetProjectTree(doc);

            var unassignedNode = tree.Find((node) =>
            {
                if (node.Path == "未分配") return true;
                else return false;
            });

            if (unassignedNode == null)
            {
                unassignedNode = new ProjectTreeNode("未分配", null);
                tree.Add(unassignedNode);
            }

            return unassignedNode;
        }


        //修改叶子节点数据到数据库
        private void modityLeafNode(ProjectTreeLeafNode leafNode)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DBEntityControl dbControl = Project.Instance.GetTmpEntCol(doc);
            DBEntity entity = dbControl.FindOne
                    (Query.EQ("HandleValue", leafNode.HandleValue));

            DBTunnel tunnel = entity as DBTunnel;
            tunnel.Location = leafNode.ParentPath;
            tunnel.Name = leafNode.Name;

            dbControl.Update(entity, this);
        }

        //得到某个节点的所有子叶节点
        private void getLeafNodes(ProjectTreeNode node, List<ProjectTreeLeafNode> ls)
        {
            if (node == null) return;
            if (node is ProjectTreeLeafNode)
            {
                ls.Add(node as ProjectTreeLeafNode);
            }
            else
            {
                foreach (var cNode in node.Children)
                {
                    getLeafNodes(cNode, ls);
                }
            }
        }

        //查询某个父节点是否包含一个子节点
        private bool containNode(ProjectTreeNode pNode, ProjectTreeNode findNode)
        {
            if (pNode == null) return false;
            bool contain = false;
            foreach (var node in pNode.Children)
            {
                contain = containNode(node, findNode);
            }
            if (pNode == findNode) return true;

            return contain;
        }

        //改变一个节点时，更改它的所有子节点的父节点路径值
        private void changeParentNode(ProjectTreeNode pNode)
        {
            foreach (var child in pNode.Children)
            {
                child.SetParentNode(pNode);
                changeParentNode(child);
            }
        }

        //**********************************************************************************************************//
        //*********************************************treelist2****************************************************//
        //**********************************************************************************************************//
        public DevExpress.XtraTreeList.TreeList FileTreeControl { get { return this.treeList2; } }
        public List<FilesTreeNode> FileTree = new List<FilesTreeNode>();
        public void initTreelist2()
        {
            treeList2.OptionsBehavior.Editable = false; //不可编辑

            treeList2.OptionsView.ShowIndicator = false;//不显示左边的一列 
            treeList2.OptionsView.ShowHorzLines = false;//不显示水平线
            treeList2.OptionsView.ShowVertLines = false;//垂直线条是否显示

            //聚焦的样式是否只适用于聚焦细胞或所有细胞除了聚焦对象，失去焦点后
            treeList2.LookAndFeel.UseWindowsXPTheme = true;
            treeList2.OptionsSelection.InvertSelection = true;

            string root = Project.Instance.RootPath;
            if (!Fs.RootDirExists())
            {

            }
            else
            {
                labelControl1.Visible = false;
                simpleButton1.Visible = false;
                FilesTreeNode rootNode = new FilesTreeNode(root, null);
                FileTree.Add(rootNode);
                Readfile(rootNode, FileTree);

                treeList2.DataSource = FileTree;

                this.fileSystemWatcher1.Path = root;

                this.fileSystemWatcher1.Created += Watcher_Created;
                this.fileSystemWatcher1.Deleted += Watcher_Deleted;
                this.fileSystemWatcher1.Renamed += Watcher_Renamed;
                this.fileSystemWatcher1.EnableRaisingEvents = true;
                this.fileSystemWatcher1.IncludeSubdirectories = true;

                //把非Name的列隐藏
                if (treeList2.Columns.Count > 0)
                {
                    for (int i = 0; i < treeList2.Columns.Count; i++)
                    {
                        if (treeList2.Columns[i].FieldName != "Name")
                            treeList2.Columns[i].Visible = false;
                    }
                }

                treeList2.ExpandToLevel(0);

                Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentDestroyed += DocumentManager_DocumentDestroyed;

            }
        }

        private void DocumentManager_DocumentDestroyed(object sender, DocumentDestroyedEventArgs e)
        {
            string fileName = e.FileName;
            Project.Instance.OpenedDocs.Remove(fileName);
        }

        //线程锁
        static readonly object mutex = new object();
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (mutex)
            {

                FileTreeControl.Invoke(new Action(delegate ()
                {
                    var treeNode = FileTreeControl.FindNode((node) =>
                    {
                        if (FileTree[node.Id].FullName == e.OldFullPath)
                        {
                            return true;
                        }

                        else
                            return false;
                    });

                    FileTree[treeNode.Id].FullName = e.FullPath;
                    FileTreeControl.RefreshNode(treeNode);

                }));
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (mutex)
            {
                FileTreeControl.Invoke(new Action(delegate ()
                {

                    var treeNode = FileTreeControl.FindNode((node) =>
                    {
                        if (FileTree[node.Id].FullName == e.FullPath)
                        {
                            return true;
                        }

                        else
                            return false;
                    });
                    RemoveNode(FileTree, FileTree[treeNode.Id]);

                    FileTreeControl.RefreshDataSource();
                }));
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (mutex)
            {

                FileTreeControl.Invoke(new Action(delegate ()
                {

                    string path = e.FullPath;
                    string dir = Path.GetDirectoryName(path);

                    var treeNode = FileTreeControl.FindNode((node) =>
                    {
                        if (FileTree[node.Id].FullName == dir)
                        {
                            return true;
                        }

                        else
                            return false;
                    });
                    List<FilesTreeNode> tempTree = new List<FilesTreeNode>();

                    FilesTreeNode newNode = new FilesTreeNode(e.FullPath, FileTree[treeNode.Id]);
                    FileTree[treeNode.Id].Children.Add(newNode);

                    tempTree.Add(newNode);
                    Readfile(newNode, tempTree);

                    FileTree.AddRange(tempTree);
                    FileTreeControl.RefreshDataSource();
                }));
            }
        }

        public class FilesTreeNode
        {
            public FilesTreeNode(string fullName, FilesTreeNode parent)
            {
                this.FullName = fullName;
                this.Parent = parent;
                Children = new List<FilesTreeNode>();
            }
            public FilesTreeNode Myself { get { return this; } }
            public FilesTreeNode Parent { get; set; }
            public string Name { get { return Path.GetFileName(FullName); } }
            public string FullName { get; set; }
            public List<FilesTreeNode> Children { get; set; }

            public bool IsDir { get { return Directory.Exists(FullName); } }

        }
        static private void RemoveNode(List<FilesTreeNode> tree, FilesTreeNode node)
        {
            foreach (var cn in node.Children)
            {
                RemoveNode(tree, cn);
            }
            tree.Remove(node);
        }
        static private void Readfile(FilesTreeNode pNode, List<FilesTreeNode> tree)
        {
            DirectoryInfo dir = new DirectoryInfo(pNode.FullName);
            FileInfo[] files = { };
            DirectoryInfo[] subDirs = { };
            if (dir.Exists)
            {
                files = dir.GetFiles();
                subDirs = dir.GetDirectories();
            }

            foreach (var file in files)
            {
                FilesTreeNode node = new FilesTreeNode(file.FullName, pNode);
                pNode.Children.Add(node);
                tree.Add(node);
            }


            foreach (var subDir in subDirs)
            {
                FilesTreeNode node = new FilesTreeNode(subDir.FullName, pNode);
                pNode.Children.Add(node);
                tree.Add(node);
                Readfile(node, tree);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            initTreelist2();
        }

        //右键的弹出窗口
        private void treeList2_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            var menu = e.Menu as DevExpress.XtraTreeList.Menu.TreeListNodeMenu;
            if (menu == null) return;

            treeList2.FocusedNode = menu.Node;
            List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;

            var node = data[menu.Node.Id];
            if (node.IsDir == true)
            {
                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("添加新dwg文档", t2Create_ItemClick));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("打开文件夹", t2OpenDid_ItemClick));
                }
            }
            else
            {
                string aLastName = node.Name.Substring(data[menu.Node.Id].Name.LastIndexOf(".") + 1,
            (data[menu.Node.Id].Name.Length - data[menu.Node.Id].Name.LastIndexOf(".") - 1));

                if (e.Menu.MenuType == DevExpress.XtraTreeList.Menu.TreeListMenuType.Node)
                {
                    if (aLastName == "dwg")
                    {

                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("打开", t2Open_ItemClick));
                        //e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("删除", t2Delete_ItemClick));
                    }
                }
            }
        }

        //打开dwg文档
        private void t2Open_ItemClick(object sender, EventArgs e)
        {
            int id = treeList2.FocusedNode.Id;
            List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;
            string path = data[id].FullName;
            if (!Project.Instance.OpenedDocs.Contains(path))
            {
                var doc = Fs.openDwg(path);
                Project.Instance.OpenedDocs.Add(doc.Name);
            }

        }

        //新建dwg文档
        private void t2Create_ItemClick(object sender, EventArgs e)
        {
            int id = treeList2.FocusedNode.Id;
            List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;

            string name = null;
            NewDwgForm f = new NewDwgForm();
            f.OK += (object s, string str) =>
            {
                name = str;
                f.Close();
            };
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(f);

            if (name == null) return;

            if (!File.Exists(Fs.FormatDwgName(data[id].FullName, name)))
                Fs.createDwg(data[id].FullName, name);
            else
                Autodesk.AutoCAD.ApplicationServices.Application.
                   ShowAlertDialog("该位置已有同名的文件");

        }

        private void t2Delete_ItemClick(object sender, EventArgs e) { }



        //把不需要显示的类型的文件隐藏
        private void treeList2_CustomRowFilter(object sender, DevExpress.XtraTreeList.CustomRowFilterEventArgs e)
        {
            int id = e.Node.Id;
            List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;
            string aLastName = data[id].Name.Substring(data[id].Name.LastIndexOf(".") + 1,
             (data[id].Name.Length - data[id].Name.LastIndexOf(".") - 1));

            if (aLastName == "dwg" || aLastName == "txt")
                return;

            if (data[id].IsDir)
            {
                DirectoryInfo di = new DirectoryInfo(data[id].FullName);
                if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    e.Handled = true;
                    e.Visible = false;
                    return;
                }
            }
            else
            {
                e.Handled = true;
                e.Visible = false;
            }
        }

        //双击节点打开对应的文件
        private void treeList2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DevExpress.XtraTreeList.TreeList treelist = sender as DevExpress.XtraTreeList.TreeList;
            if (treelist == null) return;

            Point pt = new Point(e.X, e.Y);
            DevExpress.XtraTreeList.TreeListHitInfo ht = treeList2.CalcHitInfo(pt);
            if (ht != null && ht.Node != null)
            {
                var node = ht.Node;
                List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;              
                string path = data[node.Id].FullName;

                string aLastName = path.Substring(path.LastIndexOf(".") + 1,
            (path.Length - path.LastIndexOf(".") - 1));
                if (aLastName == "dwg")
                {
                    if (!Project.Instance.OpenedDocs.Contains(path))
                    {
                        var doc = Fs.openDwg(path);
                        Project.Instance.OpenedDocs.Add(doc.Name);
                    }
                }
                else
                {
                    System.Diagnostics.Process.Start(path);
                }               
            }
        }

        private void tabPane1_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {
            if (e.Page.Caption == "图形资源管理器")
            {
                this.treeList1.RefreshDataSource();
            }
        }

        private void t2OpenDid_ItemClick(object sender, EventArgs e)
        {
            int id = treeList2.FocusedNode.Id;
            List<FilesTreeNode> data = treeList2.DataSource as List<FilesTreeNode>;
            var path = data[id].FullName;
            System.Diagnostics.Process.Start(path);
        }
    }

}
