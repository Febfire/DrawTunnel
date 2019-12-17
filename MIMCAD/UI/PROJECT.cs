using Autodesk.AutoCAD.ApplicationServices;
using EntityStore.Controller;
using EntityStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using UI.Forms;


namespace UI
{
    //单例类
    class Project
    {
        static private Project instance;
        private Project()
        {
            DwgFilesId = new Dictionary<string, string>();
        }

        static public Project Instance
        {
            get
            {
                if (!CheckValiad())
                    throw new Exception("项目未初始化");

                return instance;
            }
        }

        //创建项目实例
        static public void CreateInstance()
        {
            if (CheckValiad())
                throw new Exception("当前已经存在一个项目");

            instance = new Project();
            instance.TreeManager = new TreeManager();

            Command.OpenTreelist();
        }

        //清除项目实例并释放所有资源
        static public void CleanInstance()
        {
            if (!CheckValiad())
                throw new Exception("项目未初始化");

            Application.DocumentManager.CloseAll();

            foreach (var tmpdb in instance.dbTmpFiles)
            {
                tmpdb.Value.DB.Dispose();
                File.Delete(tmpdb.Value.Name);
            }
            foreach (var maindb in instance.dbMainEntFiles)
            {
                maindb.Value.DB.Dispose();
            }
            foreach (var treedb in instance.dbMainTreeFiles)
            {
                treedb.Value.DB.Dispose();
            }
            foreach (var pc in instance.PropControls)
            {
                pc.Value.PaletteSet.Dispose();
                pc.Value.Dispose();
            }
            foreach (var tc in instance.ProjectTreeLists)
            {
                tc.Value.Dispose();
            }
            foreach (var gc in instance.GridLists)
            {
                gc.Value.PaletteSet.Dispose();
                gc.Value.Dispose();
            }

            instance.TreeManager.PaletteSet.Dispose();
            instance.TreeManager.Dispose();

            instance = null;
        }

        //检查有没有项目实例
        static public bool CheckValiad()
        {
            if (instance == null)
                return false;
            else
                return true;
        }

        //目前打开的dwg文件
        public List<string> OpenedDocs = new List<string>();

        //右边的树控件
        public TreeManager TreeManager { get; private set; }

        //图纸路径名，id
        public Dictionary<string, string> DwgFilesId { get; private set; }

        //项目名称
        public string Name { get { return Global.CurrentProjectName; } }

        //项目根目录
        public string RootPath { get { return Global.CurrentProjectPath; } }

        public string DataPath { get { return Global.CurrentProjectDataPath; } }

        //数据库文件s
        private Dictionary<string, DBEntityControl> dbMainEntFiles = new Dictionary<string, DBEntityControl>();
        private Dictionary<string, DBTreeControl> dbMainTreeFiles = new Dictionary<string, DBTreeControl>();
        private Dictionary<string, DBEntityControl> dbTmpFiles = new Dictionary<string, DBEntityControl>();

        //控件s
        public readonly Dictionary<string, PropertyControl> PropControls = new Dictionary<string, PropertyControl>();
        public readonly Dictionary<string, DevExpress.XtraTreeList.TreeList> ProjectTreeLists = new Dictionary<string, DevExpress.XtraTreeList.TreeList>();
        public readonly Dictionary<string, GridList> GridLists = new Dictionary<string, GridList>();

        public bool ProCtlIsShowing { get; set; }
        public bool TreeListIsShowing { get; set; }
        public bool GridListIsShowing { get; set; }

        //一个dwg图纸中的图形树的层次结构
        private Dictionary<string, List<ProjectTreeNode>> projectTrees = new Dictionary<string, List<ProjectTreeNode>>();

        private Dictionary<string, ProjectTreeNode> currentWorkingSurfaces = new Dictionary<string, ProjectTreeNode>();

        private Dictionary<string, List<ProjectTreeNode>> hiddenWorkingSurfaces = new Dictionary<string, List<ProjectTreeNode>>();

        //给文档添加一个映射ID
        public void MapProjectID(Document doc)
        {

            string path = doc.Name;
            if (!DwgFilesId.ContainsKey(path))
                DwgFilesId.Add(path, CheckDBFlag.GetDbFlag(doc));
        }
        //通过文档对象获取该文档ID
        public string GetDwgId(Document doc)
        {
            string id = "";
            try
            {
                id = DwgFilesId[doc.Name];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                foreach (var pair in DwgFilesId)
                {
                    if (pair.Value == CheckDBFlag.GetDbFlag(doc))
                    {
                        DwgFilesId.Remove(pair.Key);
                        MapProjectID(doc);
                        id = DwgFilesId[doc.Name];
                        break;
                    }
                }
            }
            return id;
        }

        public void DisposeSampleDoc(string docName)
        {
            //图纸的id
            string dwgId = DwgFilesId[docName];

            dbMainEntFiles[dwgId].DB.Dispose() ;
            dbMainTreeFiles[dwgId].DB.Dispose();
            dbTmpFiles[dwgId].DB.Dispose();

            PropControls[dwgId].PaletteSet.Dispose();
            GridLists[dwgId].PaletteSet.Dispose();

            PropControls[dwgId].Dispose();
            ProjectTreeLists[dwgId].Dispose();
            GridLists[dwgId].Dispose();


            dbMainEntFiles.Remove(dwgId);
            dbMainTreeFiles.Remove(dwgId);
            dbTmpFiles.Remove(dwgId);          
              
            PropControls.Remove(dwgId);
            ProjectTreeLists.Remove(dwgId);
            GridLists.Remove(dwgId);
        }

        //工程的树相关函数
        public void CreateProjectTree(Document doc, List<ProjectTreeNode> tree)
        {
            if (projectTrees.ContainsKey(DwgFilesId[doc.Name]))
                projectTrees[DwgFilesId[doc.Name]] = tree;
            else
            {
                projectTrees.Add(DwgFilesId[doc.Name], tree);
            }

        }

        public List<ProjectTreeNode> GetProjectTree(Document doc)
        {
            string projectID = GetDwgId(doc);
            List<ProjectTreeNode> ls = null;
            projectTrees.TryGetValue(projectID, out ls);
            return ls;
        }

        //设置当前的工作面
        public void setCurrentSurface(Document doc, ProjectTreeNode node)
        {
            string projectID = GetDwgId(doc);
            ProjectTreeNode workingNode = null;
            currentWorkingSurfaces.TryGetValue(projectID, out workingNode);
            if (workingNode == null)
            {
                currentWorkingSurfaces.Add(projectID, node);
            }
            else
            {
                currentWorkingSurfaces[projectID] = node;
            }
        }
        //获取当前的工作面
        public ProjectTreeNode getCurrentSurface(Document doc)
        {
            string projectID = GetDwgId(doc);
            ProjectTreeNode workingNode = null;
            currentWorkingSurfaces.TryGetValue(projectID, out workingNode);
            if (workingNode == null) return null;
            else return currentWorkingSurfaces[projectID];
        }

        //设置隐藏的工作面
        public void setHiddenSurface(Document doc, ProjectTreeNode node)
        {
            string projectID = GetDwgId(doc);
            List<ProjectTreeNode> hiddenNodes = null;
            hiddenWorkingSurfaces.TryGetValue(projectID, out hiddenNodes);
            if (hiddenNodes == null)
            {
                var nodes = new List<ProjectTreeNode>();
                nodes.Add(node);
                hiddenWorkingSurfaces.Add(projectID, nodes);
            }
            else
            {
                hiddenWorkingSurfaces[projectID].Add(node);
            }
        }

        //删除隐藏的工作面
        public bool remmoveHiddenSurface(Document doc, ProjectTreeNode node)
        {
            string projectID = GetDwgId(doc);
            List<ProjectTreeNode> hiddenNodes = null;
            hiddenWorkingSurfaces.TryGetValue(projectID, out hiddenNodes);
            if (hiddenNodes == null)
            {
                return false;
            }
            else
            {
               return hiddenWorkingSurfaces[projectID].Remove(node);
            }
        }

        public List<ProjectTreeNode> getHiddenSurfaces(Document doc)
        {
            string projectID = GetDwgId(doc);
            List<ProjectTreeNode> hiddenNodes = null;
            hiddenWorkingSurfaces.TryGetValue(projectID, out hiddenNodes);
            if (hiddenNodes == null) return null;
            else return hiddenWorkingSurfaces[projectID];
        }

        //数据库相关函数
        public DBEntityControl GetTmpEntCol(Document doc)
        {
            string projectID = GetDwgId(doc);
            DBEntityControl value = null;
            dbTmpFiles.TryGetValue(projectID, out value);
            if (value != null)
            {
                return value;
            }
            else
            {
                string tmpFileNum = projectID;
                string tmpDbFilePath = Utils.formatDbFileName(DataPath, tmpFileNum + "(temp)");
                DBEntityControl dbControl = new DBEntityControl(tmpDbFilePath);
                dbTmpFiles.Add(projectID, dbControl);

                return dbControl;
            }
        }

        public DBEntityControl GetTmpEntCol(string dwgname)
        {
            string projectID = DwgFilesId.ContainsKey(dwgname) ? DwgFilesId[dwgname] : null;
            if (projectID == null) return null;
            if (dbTmpFiles.ContainsKey(projectID))
                return dbTmpFiles[projectID];
            else
                throw new Exception("找不到temp database");
        }

        public DBEntityControl GetMainEntCol(Document doc, bool create)
        {
            string projectID = GetDwgId(doc);
            DBEntityControl value = null;
            dbMainEntFiles.TryGetValue(projectID, out value);
            if (value != null)
            {
                return value;
            }
            else
            {
                string mainFileNum = projectID;
                string mainDbFilePath = Utils.formatDbFileName(DataPath, mainFileNum);

                if (!File.Exists(mainDbFilePath) && create)
                {
                    var maindbfile = File.Create(mainDbFilePath);
                    maindbfile.Close();
                }

                DBEntityControl dbControl = new DBEntityControl(mainDbFilePath);
                dbMainEntFiles.Add(projectID, dbControl);

                return dbControl;
            }
        }

        public DBEntityControl GetMainEntCol(string dwgname)
        {
            string projectID = DwgFilesId.ContainsKey(dwgname) ? DwgFilesId[dwgname] : null;
            if (projectID == null) return null;
            if (dbMainEntFiles.ContainsKey(projectID))
                return dbMainEntFiles[projectID];
            else
                throw new Exception("找不到main database");
        }

        public DBTreeControl GetMainTreeCol(Document doc)
        {
            string projectID = GetDwgId(doc);
            DBTreeControl value = null;
            dbMainTreeFiles.TryGetValue(projectID, out value);
            if (value != null)
            {
                return value;
            }
            else
            {
                string mainFileNum = projectID;
                string mainDbFilePath = Utils.formatDbFileName(DataPath, mainFileNum);
                DBTreeControl dbControl = new DBTreeControl(mainDbFilePath);
                dbMainTreeFiles.Add(projectID, dbControl);

                return dbControl;
            }
        }

        //属性面板
        public PropertyControl GetActivePropCtl(Document doc)
        {
            string projectID = GetDwgId(doc);
            PropertyControl propertyControl = null;
            PropControls.TryGetValue(projectID, out propertyControl);

            if (propertyControl != null)
            {
                return propertyControl;
            }
            else
            {
                BindPaletteSet.BindPropertyControl(out propertyControl);
                PropControls.Add(projectID, propertyControl);
                return propertyControl;
            }
        }

        public PropertyControl GetActivePropCtl(string dwgname)
        {
            string projectID = DwgFilesId.ContainsKey(dwgname) ? DwgFilesId[dwgname] : null;
            if (projectID == null) return null;
            PropertyControl propertyControl = null;
            PropControls.TryGetValue(projectID, out propertyControl);

            if (propertyControl != null)
            {
                return propertyControl;
            }
            else
            {
                BindPaletteSet.BindPropertyControl(out propertyControl);
                PropControls.Add(projectID, propertyControl);
                return propertyControl;
            }
        }
        //树状图面板
        public DevExpress.XtraTreeList.TreeList GetActiveTreeLst(Document doc)
        {
            string projectID = GetDwgId(doc);
            DevExpress.XtraTreeList.TreeList treelist = null;
            ProjectTreeLists.TryGetValue(projectID, out treelist);

            if (treelist != null)
            {
                return treelist;
            }
            else
            {
                treelist = new DevExpress.XtraTreeList.TreeList();
                ProjectTreeLists.Add(projectID, treelist);
                TreeManager.InitTreelist1(doc, treelist);
                //Global.TreeManager.BindProjectData();
                return treelist;
            }
        }

        public DevExpress.XtraTreeList.TreeList GetActiveTreeLst(string dwgname)
        {
            string projectID = DwgFilesId.ContainsKey(dwgname) ? DwgFilesId[dwgname] : null;
            if (projectID == null) return null;
            DevExpress.XtraTreeList.TreeList treelist = null;
            ProjectTreeLists.TryGetValue(projectID, out treelist);

            if (treelist != null)
            {
                return treelist;
            }
            else
            {
                treelist = new DevExpress.XtraTreeList.TreeList();
                ProjectTreeLists.Add(projectID, treelist);
                // Global.TreeManager.BindProjectData();
                return treelist;
            }
        }

        public GridList GetActiveGridList(Document doc)
        {
            string projectID = GetDwgId(doc);
            GridList gridList = null;
            GridLists.TryGetValue(projectID, out gridList);

            if (gridList != null)
            {
                return gridList;
            }
            else
            {
                BindPaletteSet.BindGridList(out gridList);
                GridLists.Add(projectID, gridList);
                return gridList;
            }
        }

        public GridList GetActiveGridList(string dwgname)
        {
            string projectID = DwgFilesId.ContainsKey(dwgname) ? DwgFilesId[dwgname] : null;
            if (projectID == null) return null;
            GridList gridList = null;
            GridLists.TryGetValue(projectID, out gridList);

            if (gridList != null)
            {
                return gridList;
            }
            else
            {
                BindPaletteSet.BindGridList(out gridList);
                GridLists.Add(projectID, gridList);
                return gridList;
            }
        }
    }
}
